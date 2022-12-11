using System;
using System.Data.SQLite;
using System.Threading.Tasks;
using DMOrganizerModel.Implementation.Content;
using DMOrganizerModel.Implementation.NavigationTree;
using DMOrganizerModel.Interface;
using DMOrganizerModel.Interface.Content;
using DMOrganizerModel.Interface.Model;
using DMOrganizerModel.Interface.Reference;
using DMOrganizerModel.Interface.NavigationTree;
using System.Collections.Generic;
using System.Linq;

namespace DMOrganizerModel.Implementation.Model
{
    internal sealed class OrganizerModel : IOrganizerModel, IDisposable
    {
        private class NoCaseCollation : SQLiteFunction
        {
            public override int Compare(string param1, string param2)
            {
                return string.Compare(param1, param2);
            }
        }

        #region Properties
        public string Identifier { get; }
        
        private NavigationTreeRoot? m_navigationTreeRoot;
        public NavigationTreeRoot? NavigationTree
        {
            get
            {
                CheckDisposed();
                return m_navigationTreeRoot;
            }
        }
        #endregion

        #region Fields
        private SQLiteConnection? m_Connection;
        private object? m_SyncRoot;
        #endregion

        #region Events
        public event OperationResultEventHandler<IOrganizerModel, ReferenceDecodedEventArgs>? ReferenceDecoded;
        public event OperationResultEventHandler<IOrganizerModel, NavigationTreeReceivedEventArgs>? NavigationTreeReceived;
        public event OperationResultEventHandler<IOrganizerModel>? DataDeleted;
        #endregion

        #region Constructors
        public OrganizerModel(string path)
        {
            m_SyncRoot = new();
            Identifier = path;
            m_Connection = new SQLiteConnection(new SQLiteConnectionStringBuilder
            {
                DataSource = path,
                ForeignKeys = true
            }.ToString());

            try
            {
                m_Connection.Open();
                m_Connection.BindFunction(new SQLiteFunctionAttribute("NOCASE", 3, FunctionType.Collation, SQLiteFunctionFlags.SQLITE_DETERMINISTIC), new NoCaseCollation());

                using (var command = m_Connection.CreateCommand())
                {
                    command.CommandText = 
                    @"BEGIN TRANSACTION;
                    CREATE TABLE IF NOT EXISTS ""Tag"" (
	                    ""ID""	INTEGER NOT NULL UNIQUE,
	                    ""Text""	TEXT NOT NULL CHECK(length(""Text"") > 0) UNIQUE COLLATE NOCASE,
	                    PRIMARY KEY(""ID"" AUTOINCREMENT)
                    );
                    CREATE TABLE IF NOT EXISTS ""Category"" (
	                    ""ID""	INTEGER NOT NULL UNIQUE,
	                    ""Title""	TEXT NOT NULL DEFAULT 'Category' CHECK(length(""Title"") > 0) COLLATE NOCASE,
	                    ""Parent""	INTEGER DEFAULT NULL CHECK(""Parent"" != ""ID""),
	                    UNIQUE(""Parent"",""Title""),
	                    FOREIGN KEY(""Parent"") REFERENCES ""Category""(""ID"") ON DELETE CASCADE ON UPDATE CASCADE,
	                    PRIMARY KEY(""ID"" AUTOINCREMENT)
                    );
                    CREATE TABLE IF NOT EXISTS ""Document"" (
	                    ""CategoryID""	INTEGER,
	                    ""SectionID""	INTEGER NOT NULL UNIQUE,
	                    FOREIGN KEY(""SectionID"") REFERENCES ""Section""(""ID"") ON UPDATE CASCADE ON DELETE CASCADE,
	                    FOREIGN KEY(""CategoryID"") REFERENCES ""Category""(""ID"") ON UPDATE CASCADE ON DELETE CASCADE,
	                    PRIMARY KEY(""SectionID"")
                    );
                    CREATE TABLE IF NOT EXISTS ""Section"" (
	                    ""ID""	INTEGER NOT NULL UNIQUE,
	                    ""Title""	TEXT NOT NULL DEFAULT 'Title' CHECK(length(""Title"") > 0) COLLATE NOCASE,
	                    ""Content""	TEXT NOT NULL DEFAULT '',
	                    ""Parent""	INTEGER CHECK(""ID"" != ""Parent""),
	                    ""OrderIndex""	INTEGER NOT NULL DEFAULT 0,
	                    UNIQUE(""Parent"",""Title""),
	                    UNIQUE(""Parent"",""OrderIndex""),
	                    FOREIGN KEY(""Parent"") REFERENCES ""Section""(""ID"") ON UPDATE CASCADE ON DELETE CASCADE,
	                    PRIMARY KEY(""ID"" AUTOINCREMENT)
                    );
                    CREATE TABLE IF NOT EXISTS ""DocumentsTags"" (
	                    ""DocumentID""	INTEGER NOT NULL,
	                    ""TagID""	INTEGER NOT NULL,
	                    UNIQUE(""DocumentID"",""TagID""),
	                    FOREIGN KEY(""TagID"") REFERENCES ""Tag""(""ID"") ON DELETE CASCADE ON UPDATE CASCADE,
	                    FOREIGN KEY(""DocumentID"") REFERENCES ""Document""(""SectionID"") ON DELETE CASCADE ON UPDATE CASCADE,
	                    PRIMARY KEY(""DocumentID"",""TagID"")
                    );
                    CREATE TRIGGER IF NOT EXISTS propagate_document_deletion AFTER DELETE ON Document
                    BEGIN
	                    DELETE FROM Section WHERE Section.ID=OLD.SectionID;
                    END;
                    CREATE TRIGGER IF NOT EXISTS tags_cleaner AFTER DELETE ON DocumentsTags
                    WHEN NOT EXISTS (SELECT * FROM DocumentsTags WHERE TagID=OLD.TagID)
                    BEGIN
	                    DELETE FROM Tag WHERE ID=OLD.TagID;
                    END;
                    COMMIT;";

                    command.ExecuteNonQuery();
                }

            }
            catch (Exception e)
            {
                throw new Exception("Failed to initialize Model", e);
            }
        }

        ~OrganizerModel()
        {
            if (m_SyncRoot != null)
                Dispose();
        }
        #endregion

        #region Interface
        public IReference CreateReference(IItem item)
        {
            CheckDisposed();

            if (item == null)
                throw new ArgumentNullException(nameof(item));
            
            if (item is not ItemBase itemInstance)
                throw new ArgumentException("Incompatiable instance", nameof(item));

            return new Reference.Reference(itemInstance);
        }

        public Task DecodeReference(string reference)
        {
            CheckDisposed();

            return Task.Run(() =>
            {
                lock (m_SyncRoot)
                {
                    try
                    {
                        NavigationTreeRoot? root = NavigationTree ?? LoadNavigationTree();

                        //Split into categories and item reference
                        string[] components = reference.Trim().Split('/');
                        //Separate reference to item and anything inside it from categories
                        string itemReference = components[components.Length - 1].Substring(components[components.Length - 1].IndexOf('$')+1);
                        //Remove reference to item from last category
                        if (itemReference.Length > 0)
                            components[components.Length - 1] = components[components.Length - 1].Substring(0, components[components.Length - 1].Length - itemReference.Length - 1);
                        
                        int refOffset = -1;

                        //Traverse the tree down until we reach the necessary category
                        for (int i = 0; i < components.Length - 1; i++)
                        {
                            if (components[i].Length == 0)
                            {
                                InvokeReferenceDecodedEvent(reference, null, OperationResultEventArgs.ErrorType.InvalidReference, "Empty category titles not permitted.");
                                return;
                            }
                            root = root.GetCategory(components[i]);
                            refOffset += components[i].Length + 1;
                            if (root == null)
                            {
                                InvokeReferenceDecodedEvent(reference, null, OperationResultEventArgs.ErrorType.InvalidReference, $"Reference to non-existent: { reference.Trim().Substring(0, refOffset) }");
                                return;
                            }
                        }

                        //Separate item name from reference to a thing within the item
                        string inItemReference = itemReference.Substring(itemReference.IndexOf('#')+1);
                        if (inItemReference.Length > 0)
                            itemReference = itemReference.Substring(0, itemReference.Length - inItemReference.Length - 1);

                        refOffset += itemReference.Length + 1;
                        NavigationTreeNodeBase? item = root.GetItem(itemReference);
                        if (item == null)
                        {
                            InvokeReferenceDecodedEvent(reference, null, OperationResultEventArgs.ErrorType.InvalidReference, $"Reference to non-existent: { reference.Trim().Substring(0, refOffset) }");
                            return;
                        }

                        if (item is NavigationTreeDocument documentNode)
                        {
                            SectionBase? target = documentNode.GetDocument();

                            if (target == null)
                            {
                                InvokeReferenceDecodedEvent(reference, null, OperationResultEventArgs.ErrorType.InvalidReference, $"Reference to non-existent: { reference.Trim().Substring(0, refOffset) }");
                                return;
                            }

                            if (inItemReference.Length < 1)
                            {
                                InvokeReferenceDecodedEvent(reference, new Reference.Reference(target), OperationResultEventArgs.ErrorType.None, null);
                            }
                            else
                            {
                                components = inItemReference.Split('#');

                                for (int i = 0; i < components.Length; i++)
                                {
                                    if (components[i].Length == 0)
                                    {
                                        InvokeReferenceDecodedEvent(reference, null, OperationResultEventArgs.ErrorType.InvalidReference, "Empty section titles not permitted.");
                                        return;
                                    }
                                    target = target.GetSection(components[i]);
                                    refOffset += components[i].Length + 1;
                                    if (target == null)
                                    {
                                        InvokeReferenceDecodedEvent(reference, null, OperationResultEventArgs.ErrorType.InvalidReference, $"Reference to non-existent: { reference.Trim().Substring(0, refOffset) }");
                                        return;
                                    }
                                }

                                InvokeReferenceDecodedEvent(reference, new Reference.Reference(target), OperationResultEventArgs.ErrorType.None, null);
                            }
                            return;
                        }
                        else
                        {
                            InvokeReferenceDecodedEvent(reference, null, OperationResultEventArgs.ErrorType.InvalidReference, "Unsupported reference target.");
                        }
                    }
                    catch(Exception e)
                    {
                        InvokeReferenceDecodedEvent(reference, null, OperationResultEventArgs.ErrorType.InternalError, e.Message);
                    }
                }
            });
        }

        private void InvokeReferenceDecodedEvent(string encodedReference, Reference.Reference? instance, OperationResultEventArgs.ErrorType errorType, string? errorText)
        {
            ReferenceDecoded?.Invoke(this, new ReferenceDecodedEventArgs(encodedReference)
            {
                Error = errorType,
                ErrorText = errorText,
                ReferenceInstance = instance
            });
        }

        public Task DeleteData()
        {
            CheckDisposed();

            return Task.Run(() =>
            {
                Exception? error = null;
                lock (m_SyncRoot)
                {
                    try
                    {
                        if (NavigationTree != null)
                            NavigationTree.Dispose();

                        using (var transaction = m_Connection.BeginTransaction())
                        {
                            using (var command = m_Connection.CreateCommand())
                            {
                                command.CommandText = "DELETE FROM Category";
                                command.ExecuteNonQuery();
                            }
                            using (var command = m_Connection.CreateCommand())
                            {
                                command.CommandText = "DELETE FROM Document";
                                command.ExecuteNonQuery();
                            }
                            using (var command = m_Connection.CreateCommand())
                            {
                                command.CommandText = "DELETE FROM Section";
                                command.ExecuteNonQuery();
                            }
                            using (var command = m_Connection.CreateCommand())
                            {
                                command.CommandText = "DELETE FROM DocumentsTags";
                                command.ExecuteNonQuery();
                            }
                            using (var command = m_Connection.CreateCommand())
                            {
                                command.CommandText = "DELETE FROM Tag";
                                command.ExecuteNonQuery();
                            }
                            transaction.Commit();
                        }

                        m_navigationTreeRoot = LoadNavigationTree();
                    }
                    catch (Exception e)
                    {
                        error = e;
                    }
                }
                DataDeleted?.Invoke(this, new OperationResultEventArgs()
                {
                    Error = error != null ? OperationResultEventArgs.ErrorType.InternalError : OperationResultEventArgs.ErrorType.None,
                    ErrorText = error?.Message
                });
            });
        }

        public Task GetNavigationTree()
        {
            CheckDisposed();

            return Task.Run(() =>
            {
                try
                {
                    lock (m_SyncRoot)
                    {
                        if (NavigationTree != null)
                            InvokeNavigationTreeReceived(OperationResultEventArgs.ErrorType.InternalError, NavigationTree ?? LoadNavigationTree(), null);
                    }
                }
                catch (Exception e)
                {
                    InvokeNavigationTreeReceived(OperationResultEventArgs.ErrorType.InternalError, null, e.Message);
                }
            });
        }
        
        private void InvokeNavigationTreeReceived(OperationResultEventArgs.ErrorType errorType, INavigationTreeRoot? root, string? message)
        {
            NavigationTreeReceived?.Invoke(this, new NavigationTreeReceivedEventArgs()
            {
                Error = OperationResultEventArgs.ErrorType.InternalError,
                ErrorText = message,
                NavigationTree = root
            });
        }

        private void CheckDisposed()
        {
            if (m_SyncRoot == null || m_Connection == null)
                throw new ObjectDisposedException(nameof(OrganizerModel));
        }

        public void Dispose()
        {
            CheckDisposed();

            m_navigationTreeRoot = null;
            m_Connection.Close();
            m_Connection = null;
            m_SyncRoot = null;
        }
        #endregion

        #region Methods
        public NavigationTreeRoot LoadNavigationTree()
        {
            CheckDisposed();

            lock(m_SyncRoot)
            {
                if (NavigationTree != null)
                    return NavigationTree;
                
                try
                {
                    m_navigationTreeRoot = new NavigationTreeRoot(this);
                
                    //Load categories at root
                    using (var command = m_Connection.CreateCommand())
                    {
                        command.CommandText = $"SELECT ID FROM Category WHERE Parent IS NULL";
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                                m_navigationTreeRoot.AddCategory(LoadCategoryNode(m_navigationTreeRoot, reader.GetInt32(0)));
                        }
                    }
                
                    //Load documents at root
                    using (var command = m_Connection.CreateCommand())
                    {
                        command.CommandText = $"SELECT SectionID FROM Document WHERE CategoryID IS NULL";
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                                m_navigationTreeRoot.AddItem(LoadDocumentNode(m_navigationTreeRoot, reader.GetInt32(0)));
                        }
                    }
                }
                catch (Exception e)
                {
                    NavigationTree?.Dispose();
                    m_navigationTreeRoot = null;
                    throw new Exception("Navigation tree loading failed due to underlying exception", e);
                }
            }
            
            return m_navigationTreeRoot;
        }

        private NavigationTreeCategory LoadCategoryNode(NavigationTreeRoot parent, int id)
        {
            CheckDisposed();

            NavigationTreeCategory? category = null;
            try
            {
                lock(m_SyncRoot)
                {
                    //Load category data
                    using (var command = m_Connection.CreateCommand())
                    {
                        command.CommandText = $"SELECT Title FROM Category WHERE ID={id}";
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                                category = new NavigationTreeCategory(this, parent, reader.GetString(0), id);
                            else
                                throw new ArgumentOutOfRangeException(nameof(id), "Invalid category ID");
                        }
                    }

                    //Load categories at root
                    using (var command = m_Connection.CreateCommand())
                    {
                        command.CommandText = $"SELECT ID FROM Category WHERE Parent={id}";
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                                category.AddCategory(LoadCategoryNode(category, reader.GetInt32(0)));
                        }
                    }

                    //Load file at root
                    using (var command = m_Connection.CreateCommand())
                    {
                        command.CommandText = $"SELECT ID FROM Document WHERE Document.CategoryID={id}";
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                                category.AddItem(LoadDocumentNode(category, reader.GetInt32(0)));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                category?.Dispose();
                throw new Exception("Navigation tree loading failed due to underlying exception", e);
            }

            return category;
        }

        private NavigationTreeDocument LoadDocumentNode(NavigationTreeRoot parent, int id)
        {
            CheckDisposed();

            NavigationTreeDocument? document = null;
            try
            {                
                lock (m_SyncRoot)
                {
                    using (var command = m_Connection.CreateCommand())
                    {
                        command.CommandText = $"SELECT Title FROM Section WHERE ID={id}";
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                                document = new NavigationTreeDocument(this, parent, reader.GetString(0), id);
                            else
                                throw new ArgumentOutOfRangeException(nameof(id), "Invalid document ID");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                document?.Dispose();
                throw new Exception("Document node loading failed due to underlying exception", e);
            }
            return document;
        }

        public Document LoadDocument(NavigationTreeDocument treeNode)
        {
            CheckDisposed();

            Document? document = null;
            try
            {
                lock (m_SyncRoot)
                {
                    using (var command = m_Connection.CreateCommand())
                    {
                        command.CommandText = $"SELECT Title, Content FROM Section WHERE ID={treeNode.ItemID}";
                        using (var reader = command.ExecuteReader())
                        {
                            if (!reader.Read())
                                throw new ArgumentException("Invalid document id", nameof(treeNode));

                            document = new Document(this, treeNode, reader.GetString(0), reader.GetString(1), treeNode.ItemID);
                        }
                    }
                    using (var command = m_Connection.CreateCommand())
                    {
                        command.CommandText = $"SELECT Tag.Text FROM (Tag INNER JOIN DocumentsTags ON Tag.ID = DocumentsTags.TagID) WHERE DocumentsTags.DocumentID={treeNode.ItemID}";
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                                document.Tags.Add(reader.GetString(0));
                        }
                    }
                    using (var command = m_Connection.CreateCommand())
                    {
                        command.CommandText = $"SELECT ID FROM Section WHERE Parent={treeNode.ItemID}";
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                                document.AddSection(LoadSection(document, reader.GetInt32(0)));
                        }    
                    }
                }
                return document;
            }
            catch (Exception e)
            {
                document?.Dispose();
                throw new Exception("Document loading failed due to underlying exception", e);
            }
        }

        public Section LoadSection(SectionBase parent, int id)
        {
            CheckDisposed();

            Section? section = null;
            try
            {
                lock (m_SyncRoot)
                {
                    using (var command = m_Connection.CreateCommand())
                    {
                        command.CommandText = $"SELECT Title, Content, OrderIndex FROM Section WHERE ID={id}";
                        using (var reader = command.ExecuteReader())
                        {
                            if (!reader.Read())
                                throw new ArgumentException("Invalid section id", nameof(id));

                            section = new Section(this, parent, reader.GetString(0), reader.GetString(1), reader.GetInt32(2), id);
                        }
                    }
                    using (var command = m_Connection.CreateCommand())
                    {
                        command.CommandText = $"SELECT ID FROM Section WHERE Parent={id}";
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                                section.AddSection(LoadSection(section, reader.GetInt32(0)));
                        }    
                    }
                }
                return section;
            }
            catch (Exception e)
            {
                section?.Dispose();
                throw new Exception("Document loading failed due to underlying exception", e);
            }
        }

        public void ChangeDocumentParent(NavigationTreeDocument doc, int? newID)
        {
            CheckDisposed();

            lock (m_SyncRoot)
            {
                try
                {
                    if (newID.HasValue)
                    {
                        using (var command = m_Connection.CreateCommand())
                        {
                            command.CommandText = $"SELECT id FROM Category WHERE ID={newID.Value}";
                            using (var reader = command.ExecuteReader())
                            {
                                if (!reader.Read())
                                    throw new ArgumentException("Invalid parent id", nameof(newID));
                            }
                        }
                        using (var command = m_Connection.CreateCommand())
                        {
                            command.CommandText = $"UPDATE Document SET CategoryID = {newID.Value} WHERE ID={doc.ItemID}";
                            if (command.ExecuteNonQuery() == 0)
                                throw new ArgumentException("Invalid document id", nameof(doc));
                        }
                    }
                    else
                    {
                        using (var command = m_Connection.CreateCommand())
                        {
                            command.CommandText = $"UPDATE Document SET CategoryID = NULL WHERE ID={doc.ItemID}";
                            if (command.ExecuteNonQuery() == 0)
                                throw new ArgumentException("Invalid document id", nameof(doc));
                        }
                    }
                }
                catch (Exception e)
                {
                    throw new Exception("Document parent change failed to due underlying exception", e);
                }
            }
        }

        public void ChangeDocumentTitle(NavigationTreeDocument doc, string title)
        {
            CheckDisposed();

            if (!StorageModel.IsValidTitle(title))
                throw new ArgumentException("Invalid title", nameof(title));

            lock (m_SyncRoot)
            {
                try
                {
                    using (var command = m_Connection.CreateCommand())
                    {
                        command.CommandText = $"UPDATE Section Set Title=$title WHERE ID={doc.ItemID}";
                        command.Parameters.AddWithValue("title", title);
                        if (command.ExecuteNonQuery() == 0)
                            throw new ArgumentException("Invalid document ID", nameof(doc));
                    }
                }
                catch (Exception e)
                {
                    throw new Exception("Document title change failed to due underlying exception", e);
                }
            }
        }

        public NavigationTreeCategory CreateCategory(NavigationTreeRoot parent, string title)
        {
            CheckDisposed();
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));
            else if (title == null)
                throw new ArgumentNullException(nameof(title));
            else if (!StorageModel.IsValidTitle(title))
                throw new ArgumentException("Invalid title", nameof(title));
            else if (parent.GetCategory(title) != null)
                throw new ArgumentException("Duplicate title", nameof(title));

            NavigationTreeCategory? category = null;
            try
            {
                lock (m_SyncRoot)
                {
                    int id;

                    using (var command = m_Connection.CreateCommand())
                    {
                        if (parent is NavigationTreeCategory parentCategory)
                            command.CommandText = $"INSERT INTO Category (Title, Parent) VALUES ($title, {parentCategory.ItemID}); SELECT last_insert_rowid();";
                        else
                            command.CommandText = $"INSERT INTO Category (Title) VALUES ($title); SELECT last_insert_rowid();";

                        command.Parameters.AddWithValue("title", title);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                                id = reader.GetInt32(0);
                            else
                                throw new Exception("Insert failed");
                        }
                    }
                    category = new NavigationTreeCategory(this, parent, title, id);
                    return category;
                }
            }
            catch (Exception e)
            {
                category?.Dispose();
                throw new Exception("Category creation failed to due underlying exception", e);
            }
        }

        public NavigationTreeDocument CreateDocument(NavigationTreeRoot parent, string title)
        {
            CheckDisposed();
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));
            else if (title == null)
                throw new ArgumentNullException(nameof(title));
            else if (!StorageModel.IsValidTitle(title))
                throw new ArgumentException("Invalid title", nameof(title));
            else if (parent.GetItem(title) != null)
                throw new ArgumentException("Duplicate title", nameof(title));

            NavigationTreeDocument? document = null;
            try
            {
                lock (m_SyncRoot)
                {
                    int id;

                    using (var command = m_Connection.CreateCommand())
                    {
                        if (parent is NavigationTreeCategory parentCategory)
                            command.CommandText = $"INSERT INTO Section (Title) VALUES ($title); INSERT INTO Document (CategoryID, SectionID) VALUES ({parentCategory.ItemID}, last_insert_rowid()); SELECT SectionID FROM Document WHERE ROWID=last_insert_rowid();";
                        else
                            command.CommandText = $"INSERT INTO Section (Title) VALUES ($title); INSERT INTO Document (SectionID) VALUES (last_insert_rowid()); SELECT SectionID FROM Document WHERE ROWID=last_insert_rowid();";

                        command.Parameters.AddWithValue("title", title);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                                id = reader.GetInt32(0);
                            else
                                throw new Exception("Insert failed");
                        }
                    }
                    document = new NavigationTreeDocument(this, parent, title, id);
                    return document;
                }
            }
            catch (Exception e)
            {
                document?.Dispose();
                throw new Exception("Category creation failed to due underlying exception", e);
            }
        }
        
        public void DeleteDocument(NavigationTreeDocument document)
        {
            CheckDisposed();
            if (document == null)
                throw new ArgumentNullException(nameof(document));
            
            try
            {
                lock (m_SyncRoot)
                {
                    using (var command = m_Connection.CreateCommand())
                    {
                        command.CommandText = $"DELETE FROM Document WHERE SectionID={document.ItemID}";
                        if (command.ExecuteNonQuery() == 0)
                            throw new ArgumentException("Invalid document id", nameof(document));
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Document deletion failed to due underlying exception", e);
            }
        }

        public void DeleteCategory(NavigationTreeCategory category)
        {
            CheckDisposed();
            if (category == null)
                throw new ArgumentNullException(nameof(category));
            
            try
            {
                lock (m_SyncRoot)
                {
                    using (var command = m_Connection.CreateCommand())
                    {
                        command.CommandText = $"DELETE FROM Category WHERE ID={category.ItemID}";
                        if (command.ExecuteNonQuery() == 0)
                            throw new ArgumentException("Invalid category id", nameof(category));
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Category deletion failed to due underlying exception", e);
            }
        }
        #endregion
    }
}
