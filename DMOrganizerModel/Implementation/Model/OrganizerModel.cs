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
        
        public NavigationTreeRoot? NavigationTree { get; private set; }
        #endregion

        #region Fields
        private readonly SQLiteConnection m_Connection;
        private readonly object m_SyncRoot;
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

                    CREATE TABLE IF NOT EXISTS ""DocumentsTags"" (
	                    ""DocumentID""	INTEGER NOT NULL,
	                    ""TagID""	INTEGER NOT NULL,
	                    FOREIGN KEY(""TagID"") REFERENCES ""Tag""(""ID"") ON DELETE CASCADE ON UPDATE CASCADE,
	                    FOREIGN KEY(""DocumentID"") REFERENCES ""Document""(""ID"") ON DELETE CASCADE ON UPDATE CASCADE,
	                    PRIMARY KEY(""DocumentID"",""TagID"")
                    );

                    CREATE TABLE IF NOT EXISTS ""Document"" (
	                    ""ID""	INTEGER NOT NULL UNIQUE,
	                    ""Title""	TEXT NOT NULL DEFAULT 'Document' CHECK(length(""Title"") > 0) COLLATE NOCASE,
	                    ""Content""	TEXT NOT NULL DEFAULT '',
	                    ""Parent""	INTEGER,
	                    FOREIGN KEY(""Parent"") REFERENCES ""Category""(""ID"") ON DELETE CASCADE ON UPDATE CASCADE,
	                    UNIQUE(""Parent"",""Title""),
	                    PRIMARY KEY(""ID"" AUTOINCREMENT)
                    );

                    CREATE TABLE IF NOT EXISTS ""Category"" (
	                    ""ID""	INTEGER NOT NULL UNIQUE,
	                    ""Title""	TEXT NOT NULL DEFAULT 'Category' CHECK(length(""Title"") > 0) COLLATE NOCASE,
	                    ""Parent""	INTEGER DEFAULT NULL CHECK(""Parent"" != ""ID""),
	                    FOREIGN KEY(""Parent"") REFERENCES ""Category""(""ID"") ON DELETE CASCADE ON UPDATE CASCADE,
	                    UNIQUE(""Parent"",""Title""),
	                    PRIMARY KEY(""ID"" AUTOINCREMENT)
                    );

                    CREATE TABLE IF NOT EXISTS ""Section"" (
	                    ""ID""	INTEGER NOT NULL UNIQUE,
	                    ""Title""	TEXT NOT NULL DEFAULT 'Title' CHECK(length(""Title"") > 0) COLLATE NOCASE,
	                    ""Content""	TEXT NOT NULL DEFAULT '',
	                    PRIMARY KEY(""ID"" AUTOINCREMENT)
                    );

                    CREATE TABLE IF NOT EXISTS ""Subsections"" (
	                    ""SectionID""	INTEGER NOT NULL DEFAULT 0,
	                    ""SubsectionID""	INTEGER NOT NULL DEFAULT 0 UNIQUE,
	                    ""OrderIndex""	INTEGER NOT NULL DEFAULT 0,
	                    ""ID""	INTEGER NOT NULL UNIQUE,
	                    FOREIGN KEY(""SubsectionID"") REFERENCES ""Section""(""ID"") ON UPDATE CASCADE ON DELETE CASCADE,
	                    FOREIGN KEY(""SectionID"") REFERENCES ""Section""(""ID"") ON UPDATE CASCADE ON DELETE NO ACTION,
	                    UNIQUE(""SectionID"",""OrderIndex""),
	                    PRIMARY KEY(""ID"" AUTOINCREMENT)
                    );

                    CREATE TABLE IF NOT EXISTS ""DocumentsSections"" (
	                    ""DocumentID""	INTEGER NOT NULL DEFAULT 0,
	                    ""SectionID""	INTEGER NOT NULL DEFAULT 0 UNIQUE,
	                    ""OrderIndex""	INTEGER DEFAULT 0,
	                    ""ID""	INTEGER NOT NULL UNIQUE,
	                    FOREIGN KEY(""DocumentID"") REFERENCES ""Document""(""ID"") ON UPDATE CASCADE,
	                    FOREIGN KEY(""SectionID"") REFERENCES ""Section""(""ID"") ON UPDATE CASCADE ON DELETE CASCADE,
	                    UNIQUE(""DocumentID"",""OrderIndex""),
	                    PRIMARY KEY(""ID"" AUTOINCREMENT)
                    );

                    CREATE TRIGGER IF NOT EXISTS section_title_insert_duplication_guard BEFORE INSERT ON DocumentsSections
                    WHEN EXISTS (
	                    SELECT * FROM (Section INNER JOIN DocumentsSections ON Section.ID=DocumentsSections.SectionID)
	                    WHERE DocumentsSections.DocumentID=NEW.DocumentID AND
		                    Section.Title IN (SELECT Section.Title FROM Section WHERE Section.ID=NEW.SectionID)
                    )
                    BEGIN
	                    SELECT RAISE(ABORT, 'Duplicate Title of section');
                    END;

                    CREATE TRIGGER IF NOT EXISTS section_title_update_duplication_guard BEFORE UPDATE ON Section
                    WHEN EXISTS (
	                    SELECT * FROM (Section INNER JOIN DocumentsSections ON Section.ID=DocumentsSections.SectionID)
	                    WHERE Section.Title=NEW.Title AND DocumentsSections.DocumentID IN
		                    (SELECT DocumentID FROM DocumentsSections WHERE SectionID=NEW.ID)
                    )
                    BEGIN
	                    SELECT RAISE(ABORT, 'Duplicate Title of section');
                    END;

                    CREATE TRIGGER IF NOT EXISTS subsection_title_insert_duplication_guard BEFORE INSERT ON Subsections
                    WHEN EXISTS (
	                    SELECT * FROM (Section INNER JOIN Subsections ON Section.ID=Subsections.SectionID)
	                    WHERE Subsections.SectionID=NEW.SectionID AND
		                    Section.Title IN (SELECT Section.Title FROM Section WHERE Section.ID=NEW.SectionID)
                    )
                    BEGIN
	                    SELECT RAISE(ABORT, 'Duplicate Title of section');
                    END;

                    CREATE TRIGGER IF NOT EXISTS subsection_title_update_duplication_guard BEFORE UPDATE ON Section
                    WHEN EXISTS (
	                    SELECT * FROM (Section INNER JOIN Subsections ON Section.ID=Subsections.SectionID)
	                    WHERE Section.Title=NEW.Title AND Subsections.SectionID IN
		                    (SELECT SectionID FROM Subsections WHERE SubsectionID=NEW.ID)
                    )
                    BEGIN
	                    SELECT RAISE(ABORT, 'Duplicate Title of section');
                    END;

                    CREATE TRIGGER IF NOT EXISTS tags_cleanup AFTER DELETE ON DocumentsTags
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
        #endregion

        #region Methods
        public IReference CreateReference(IItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            if (item is not ItemBase itemInstance)
                throw new ArgumentException("Incompatiable instance", nameof(item));

            return new Reference.Reference(itemInstance);
        }

        /*
         * Example valid refs (given existance of respective entities:
         * Category1/Category2$document#section1 = Section 'section1' of document 'document' in category 'Category2' in category 'Category1'
        */
        public Task DecodeReference(string reference)
        {
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
                            SectionBase target = documentNode.GetDocument();

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
                                command.CommandText = "DELETE FROM DocumentsSections";
                                command.ExecuteNonQuery();
                            }
                            using (var command = m_Connection.CreateCommand())
                            {
                                command.CommandText = "DELETE FROM DocumentsTags";
                                command.ExecuteNonQuery();
                            }
                            using (var command = m_Connection.CreateCommand())
                            {
                                command.CommandText = "DELETE FROM Section";
                                command.ExecuteNonQuery();
                            }
                            using (var command = m_Connection.CreateCommand())
                            {
                                command.CommandText = "DELETE FROM Subsections";
                                command.ExecuteNonQuery();
                            }
                            using (var command = m_Connection.CreateCommand())
                            {
                                command.CommandText = "DELETE FROM Tag";
                                command.ExecuteNonQuery();
                            }
                            transaction.Commit();
                        }

                        NavigationTree = LoadNavigationTree();
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
            return Task.Run(() =>
            {
                try
                {
                    lock (m_SyncRoot)
                    {
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

        public NavigationTreeRoot LoadNavigationTree()
        {
            lock(m_SyncRoot)
            {
                if (NavigationTree != null)
                    return NavigationTree;
                
                try
                {
                    NavigationTree = new NavigationTreeRoot(this);
                
                    //Load categories at root
                    using (var command = m_Connection.CreateCommand())
                    {
                        command.CommandText = $"SELECT id FROM Category WHERE Parent IS NULL";
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                                NavigationTree.AddCategory(LoadCategoryNode(NavigationTree, reader.GetInt32(0)));
                        }
                    }
                
                    //Load documents at root
                    using (var command = m_Connection.CreateCommand())
                    {
                        command.CommandText = $"SELECT id FROM Document WHERE Parent IS NULL";
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                                NavigationTree.AddItem(LoadDocumentNode(NavigationTree, reader.GetInt32(0)));
                        }
                    }
                }
                catch (Exception e)
                {
                    NavigationTree?.Dispose();
                    NavigationTree = null;
                    throw new Exception("Navigation tree loading failed due to underlying exception", e);
                }
            }
            
            return NavigationTree;
        }

        private NavigationTreeCategory LoadCategoryNode(INavigationTreeRootInternal parent, int id)
        {
            NavigationTreeCategory? category = null;
            try
            {
                lock(m_SyncRoot)
                {
                    //Load category data
                    using (var command = m_Connection.CreateCommand())
                    {
                        command.CommandText = $"SELECT name FROM Category WHERE ID={id}";
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                                category = new NavigationTreeCategory(this, parent, reader.GetString(0));
                            else
                                throw new ArgumentOutOfRangeException(nameof(id), "Invalid category ID");
                        }
                    }

                    //Load categories at root
                    using (var command = m_Connection.CreateCommand())
                    {
                        command.CommandText = $"SELECT id FROM Category WHERE Parent={id}";
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                                category.AddCategory(LoadCategoryNode(category, reader.GetInt32(0)));
                        }
                    }

                    //Load file at root
                    using (var command = m_Connection.CreateCommand())
                    {
                        command.CommandText = $"SELECT id FROM Document WHERE Parent={id}";
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

        private NavigationTreeDocument LoadDocumentNode(INavigationTreeRootInternal parent, int id)
        {
            NavigationTreeDocument? document = null;
            try
            {                
                lock (m_SyncRoot)
                {
                    using (var command = m_Connection.CreateCommand())
                    {
                        command.CommandText = $"SELECT name FROM Category WHERE ID={id}";
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                                document = new NavigationTreeDocument(this, parent, reader.GetString(0));
                            else
                                throw new ArgumentOutOfRangeException(nameof(id), "Invalid document ID");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Document loading failed due to underlying exception", e);
            }
            finally
            {
                document?.Dispose();
            }
            return document;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
