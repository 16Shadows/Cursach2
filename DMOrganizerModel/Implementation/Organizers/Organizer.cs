using System;
using System.Data.SQLite;
using System.Collections.Generic;

using DMOrganizerModel.Interface.Organizer;
using DMOrganizerModel.Implementation.Utility;
using DMOrganizerModel.Interface.Items;
using DMOrganizerModel.Implementation.Items;
using DMOrganizerModel.Interface.References;
using System.Threading.Tasks;
using CSToolbox.Weak;

namespace DMOrganizerModel.Implementation.Organizers
{
    internal sealed class Organizer : IOrganizer, IItemContainerBase
    {
        private class NoCaseCollation : SQLiteFunction
        {
            public override int Compare(string param1, string param2)
            {
                return string.Compare(param1, param2, true);
            }
        }

        public string Path { get; }
        public SyncronizedSQLiteConnection Connection { get; }
        private SQLiteConnection RawConnection { get; }
        private object Lock { get; }

        public Organizer(string path)
        {
            Lock = new object();
            Path = path;
            RawConnection = new SQLiteConnection(new SQLiteConnectionStringBuilder
            {
                DataSource = path,
                ForeignKeys = true
            }.ToString());

            try
            {
                RawConnection.Open();
                RawConnection.BindFunction(new SQLiteFunctionAttribute("NOCASE", 3, FunctionType.Collation, SQLiteFunctionFlags.SQLITE_DETERMINISTIC), new NoCaseCollation());

                using var command = RawConnection.CreateCommand();
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
                CREATE TRIGGER IF NOT EXISTS document_create_title_duplication_guard BEFORE INSERT ON Document
                WHEN EXISTS (
	                SELECT *
	                FROM (Document INNER JOIN Section ON Document.SectionID=Section.ID)
	                WHERE Document.CategoryID=NEW.CategoryID AND Section.Title IN
		                (SELECT Title FROM Section WHERE Section.ID=NEW.SectionID)
                )
                BEGIN
	                SELECT RAISE(ABORT, 'Duplicate title in category');
                END;

                CREATE TRIGGER IF NOT EXISTS document_move_title_duplication_guard BEFORE UPDATE ON Document
                WHEN EXISTS (
	                SELECT *
	                FROM (Document INNER JOIN Section ON Document.SectionID=Section.ID)
	                WHERE Document.CategoryID=NEW.CategoryID AND Section.Title IN
		                (SELECT Title FROM Section WHERE Section.ID=NEW.SectionID)
                )
                BEGIN
	                SELECT RAISE(ABORT, 'Duplicate title in category');
                END;

                CREATE TRIGGER IF NOT EXISTS document_rename_title_duplication_guard BEFORE UPDATE ON Section
                WHEN NEW.Parent IS NULL AND EXISTS (
	                SELECT *
	                FROM (Document INNER JOIN Section ON Document.SectionID=Section.ID)
	                WHERE NEW.Title=Section.Title AND Document.CategoryID IN
		                (SELECT CategoryID FROM Document WHERE Document.SectionID=NEW.ID AND Document.SectionID<>NEW.ID)
                )
                BEGIN
	                SELECT RAISE(ABORT, 'Duplicate title in category');
                END;
                CREATE TRIGGER IF NOT EXISTS category_insert_root_title_duplication_guard BEFORE INSERT ON Category
                WHEN NEW.Parent IS NULL AND EXISTS (SELECT * FROM Category WHERE Parent IS NULL AND Title=NEW.Title)
                BEGIN
	                SELECT RAISE(ABORT, 'Duplicate title');
                END;

                CREATE TRIGGER IF NOT EXISTS category_update_root_title_duplication_guard BEFORE UPDATE ON Category
                WHEN NEW.Parent IS NULL AND EXISTS (SELECT * FROM Category WHERE Parent IS NULL AND Title=NEW.Title AND ID<>NEW.ID)
                BEGIN
	                SELECT RAISE(ABORT, 'Duplicate title');
                END;
                COMMIT;";

                command.ExecuteNonQuery();

                // second module database init
                command.CommandText =
                @"BEGIN TRANSACTION;
                CREATE TABLE IF NOT EXISTS ""Book"" (
	                ""ID""	INTEGER NOT NULL UNIQUE,
	                ""Title"" 	TEXT NOT NULL DEFAULT 'Book' CHECK(length(""Title"") > 0) UNIQUE COLLATE NOCASE,
	                ""ID_Parent_Category""	INTEGER DEFAULT NULL,
	                PRIMARY KEY(""ID"" AUTOINCREMENT),
                FOREIGN KEY(""ID_Parent_Category"") REFERENCES ""Category""(""ID"") ON DELETE CASCADE
                );
                CREATE TABLE IF NOT EXISTS ""Object"" (
	                ""ID""	INTEGER NOT NULL UNIQUE,
	                ""Link_to_Object""	TEXT NOT NULL,
	                PRIMARY KEY(""ID"" AUTOINCREMENT)
                );
                CREATE TABLE IF NOT EXISTS ""Page"" (
	                ""ID""	INTEGER NOT NULL UNIQUE,
	                ""Position""	INTEGER NOT NULL CHECK(""Position"" > 0),
	                ""ID_Parent_Book""	INTEGER NOT NULL,
	                PRIMARY KEY(""ID"" AUTOINCREMENT),
	                FOREIGN KEY(""ID_Parent_Book"") REFERENCES ""Book""(""ID"") ON DELETE CASCADE
                );CREATE TABLE IF NOT EXISTS ""Container"" (
	                ""ID""	INTEGER NOT NULL UNIQUE,
	                ""Type""	INTEGER NOT NULL DEFAULT 1,
	                ""Height""	INTEGER NOT NULL DEFAULT 200,
                ""Width""	INTEGER NOT NULL DEFAULT 200,
	                ""CoordinateX""	INTEGER NOT NULL DEFAULT 0,
                ""CoordinateY""	INTEGER NOT NULL DEFAULT 0,
	                PRIMARY KEY(""ID"" AUTOINCREMENT)
                );
                CREATE TABLE IF NOT EXISTS ""Set_Page_Containers"" (
	                ""ID""	INTEGER NOT NULL UNIQUE,
	                ""ID_Page""	INTEGER NOT NULL,
	                ""ID_Container""	INTEGER NOT NULL,
	                PRIMARY KEY(""ID"" AUTOINCREMENT),
	                FOREIGN KEY(""ID_Container"") REFERENCES ""Container""(""ID""),
	                FOREIGN KEY(""ID_Page"") REFERENCES ""Page""(""ID"") ON DELETE CASCADE
                );
                CREATE TABLE IF NOT EXISTS ""Set_Book_Pages"" (
	                ""ID""	INTEGER NOT NULL UNIQUE,
	                ""ID_Book""	INTEGER NOT NULL,
	                ""ID_Page""	INTEGER NOT NULL,
	                PRIMARY KEY(""ID"" AUTOINCREMENT),
	                FOREIGN KEY(""ID_Page"") REFERENCES ""Page""(""ID""),
	                FOREIGN KEY(""ID_Book"") REFERENCES ""Book""(""ID"") ON DELETE CASCADE
                );
                CREATE TABLE IF NOT EXISTS ""Set_Container_Objects"" (
	                ""ID""	INTEGER NOT NULL UNIQUE,
	                ""ID_Container""	INTEGER NOT NULL,
	                ""ID_Object""	INTEGER NOT NULL,
	                PRIMARY KEY(""ID"" AUTOINCREMENT),
	                FOREIGN KEY(""ID_Object"") REFERENCES ""Object""(""ID""),
	                FOREIGN KEY(""ID_Container"") REFERENCES ""Container""(""ID"") ON DELETE CASCADE
                );
                CREATE TRIGGER IF NOT EXISTS Page_deletion_on_book_deletion AFTER DELETE ON Set_Book_Pages
                BEGIN
                DELETE FROM Page WHERE Page.ID=OLD.ID_Page;
                END;

                CREATE TRIGGER IF NOT EXISTS Container_deletion_on_page_deletion AFTER DELETE ON Set_Page_Containers
                BEGIN
                DELETE FROM Container WHERE Container.ID=OLD.ID_Container;
                END;

                CREATE TRIGGER IF NOT EXISTS Object_deletion_on_container_deletion AFTER DELETE ON Set_Container_Objects
                BEGIN
                DELETE FROM Object WHERE Object.ID=OLD.ID_Object;
                END;

                CREATE TRIGGER Validate_Page_Position_Before_Insert 
                   BEFORE INSERT ON Page
                BEGIN
                   SELECT
                      CASE
	                WHEN Page.Position IN (SELECT Page.Position FROM Page WHERE Page.ID_Parent_Book = NEW.ID_Parent_Book) THEN
   	                  RAISE (ABORT,'Invalid page position.')
                       END;
                END;

                CREATE TRIGGER Validate_Page_Position_Before_Insert 
                   BEFORE INSERT ON Page
                BEGIN
                   SELECT
                      CASE
	                WHEN NEW.Position IN (SELECT Page.Position FROM Page WHERE Page.ID_Parent_Book = NEW.ID_Parent_Book) THEN
   	                  RAISE (ABORT,'Invalid page position.')
                       END;
                END;

                COMMIT;
                ";
                command.ExecuteNonQuery();
            }
            catch
            {
                RawConnection.Close();
            }
            Connection = new SyncronizedSQLiteConnection(RawConnection);
        }

        ~Organizer()
        {
            RawConnection?.Close();
        }

        #region IOrganizer
        public WeakEvent<IOrganizer, OrganizerItemCreatedEventArgs> OrganizerItemCreated { get; } = new();
        public WeakEvent<IItemContainer<IOrganizerItem>, ItemContainerCurrentContentEventArgs<IOrganizerItem>> ItemContainerCurrentContent { get; } = new();
        public WeakEvent<IItemContainer<IOrganizerItem>, ItemContainerContentChangedEventArgs<IOrganizerItem>> ItemContainerContentChanged { get; } = new();
        
        private void InvokeOrganizerItemCreated(string name, OrganizerItemCreatedEventArgs.ResultType result)
        {
            OrganizerItemCreated?.Invoke(this, new OrganizerItemCreatedEventArgs(name, result));
        }

        private void InvokeItemContainerCurrentContent(IEnumerable<IOrganizerItem> items)
        {
            ItemContainerCurrentContent?.Invoke(this, new ItemContainerCurrentContentEventArgs<IOrganizerItem>(items));
        }

        private void InvokeItemContainerContentChanged(IOrganizerItem item, ItemContainerContentChangedEventArgs<IOrganizerItem>.ChangeType type, ItemContainerContentChangedEventArgs<IOrganizerItem>.ResultType result)
        {
            ItemContainerContentChanged?.Invoke(this, new ItemContainerContentChangedEventArgs<IOrganizerItem>(item, type, result));
        }

        public void CreateCategory(string name)
        {
            Task.Run(() =>
            {
                if (!NamingRules.IsValidName(name))
                {
                    InvokeOrganizerItemCreated(name, OrganizerItemCreatedEventArgs.ResultType.InvalidName);
                    return;
                }
                bool isUnique = false;
                IOrganizerItem item = null;
                lock (Lock)
                {
                    isUnique = CanHaveItemWithName(name);
                    if (isUnique)
                        item = GetCategory(Query.CreateCategory(Connection, name, null), this);
                }
                if (isUnique)
                {
                    InvokeOrganizerItemCreated(name, OrganizerItemCreatedEventArgs.ResultType.Success);
                    InvokeItemContainerContentChanged(item, ItemContainerContentChangedEventArgs<IOrganizerItem>.ChangeType.ItemAdded, ItemContainerContentChangedEventArgs<IOrganizerItem>.ResultType.Success);
                }
                else
                    InvokeOrganizerItemCreated(name, OrganizerItemCreatedEventArgs.ResultType.DuplicateName);
            });
        }

        public void CreateDocument(string name)
        {
            Task.Run(() =>
            {
                if (!NamingRules.IsValidName(name))
                {
                    InvokeOrganizerItemCreated(name, OrganizerItemCreatedEventArgs.ResultType.InvalidName);
                    return;
                }
                bool isUnique = false;
                IOrganizerItem item = null;
                lock (Lock)
                {
                    isUnique = CanHaveItemWithName(name);
                    if (isUnique)
                        item = GetDocument(Query.CreateDocument(Connection, name, null), this);
                }
                if (isUnique)
                {
                    InvokeOrganizerItemCreated(name, OrganizerItemCreatedEventArgs.ResultType.Success);
                    InvokeItemContainerContentChanged(item, ItemContainerContentChangedEventArgs<IOrganizerItem>.ChangeType.ItemAdded, ItemContainerContentChangedEventArgs<IOrganizerItem>.ResultType.Success);
                }
                else
                    InvokeOrganizerItemCreated(name, OrganizerItemCreatedEventArgs.ResultType.DuplicateName);
            });
        }
        public void CreateBook(string name)
        {
            Task.Run(() =>
            {
                if (!NamingRules.IsValidName(name))
                {
                    InvokeOrganizerItemCreated(name, OrganizerItemCreatedEventArgs.ResultType.InvalidName);
                    return;
                }
                bool isUnique = false;
                IOrganizerItem item = null;
                lock (Lock)
                {
                    isUnique = CanHaveItemWithName(name);
                    if (isUnique)
                        item = GetBook(Query.CreateBook(Connection, name), this);
                }
                if (isUnique)
                {
                    InvokeOrganizerItemCreated(name, OrganizerItemCreatedEventArgs.ResultType.Success);
                    InvokeItemContainerContentChanged(item, ItemContainerContentChangedEventArgs<IOrganizerItem>.ChangeType.ItemAdded, ItemContainerContentChangedEventArgs<IOrganizerItem>.ResultType.Success);
                }
                else
                    InvokeOrganizerItemCreated(name, OrganizerItemCreatedEventArgs.ResultType.DuplicateName);
            });
        }

        public void RequestItemContainerCurrentContent()
        {
            Task.Run(() =>
            {
                List<IOrganizerItem> items = new List<IOrganizerItem>();
                lock (Lock)
                {
                    foreach (int id in Query.GetCategoriesInOrganizer(Connection))
                        items.Add(GetCategory(id, this));
                    foreach (int id in Query.GetDocumentsInOrganizer(Connection))
                        items.Add(GetDocument(id, this));
                    foreach (int id in Query.GetBooksInOrganizer(Connection))
                        items.Add(GetBook(id, this));
                }
                InvokeItemContainerCurrentContent(items);
            });
        }

        public void MakeParentOf(IOrganizerItem item)
        {
            if (item is not IOrganizerItem itemTyped || item is not Item itemBase)
                throw new ArgumentTypeException(nameof(item), "Unsupported item type.");

            Task.Run(() =>
            {
                bool isUnique = false;
                lock (Lock)
                {
                    isUnique = CanBeParentOf(itemTyped);
                    if (isUnique)
                        itemBase.SetParent(this);
                }
                InvokeItemContainerContentChanged(item, ItemContainerContentChangedEventArgs<IOrganizerItem>.ChangeType.ItemAdded, isUnique ? ItemContainerContentChangedEventArgs<IOrganizerItem>.ResultType.Success : ItemContainerContentChangedEventArgs<IOrganizerItem>.ResultType.DuplicateItem);
            });
        }

        public IReference DecodeReference(string reference)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Caching
        private ConcurrentWeakCache<int, Category> CategoriesCache { get; } = new(_ => throw new InvalidOperationException("Cannot create parentless item."));
        private ConcurrentWeakCache<int, Section> SectionsCache { get; } = new(_ => throw new InvalidOperationException("Cannot create parentless item."));
        private ConcurrentWeakCache<int, BookPage> PagesCache { get; } = new(_ => throw new InvalidOperationException("Cannot create parentless item."));
        private ConcurrentWeakCache<int, Book> BooksCache { get; } = new(_ => throw new InvalidOperationException("Cannot create parentless item."));
        private ConcurrentWeakCache<int, ObjectContainer> ObjectContainerCache { get; } = new(_ => throw new InvalidOperationException("Cannot create parentless item."));
        private ConcurrentWeakCache<int, ContainerObject> ContainerObjectCache { get; } = new(_ => throw new InvalidOperationException("Cannot create parentless item."));

        public Category GetCategory(int id, IItemContainerBase parent) => CategoriesCache.GetValue(id, id => new Category(id, parent, this));

        public Section GetSection(int id, IItemContainerBase parent) => SectionsCache.GetValue(id, id => new Section(id, parent, this));

        public Document GetDocument(int id, IItemContainerBase parent) => (Document)SectionsCache.GetValue(id, id => new Document(id, parent, this));

        public BookPage GetPage(int id, IItemContainerBase parent) => PagesCache.GetValue(id, id => new BookPage(id, parent, this));

        public Book GetBook(int id, IItemContainerBase parent) => BooksCache.GetValue(id, id => new Book(id, parent, this));
        public ObjectContainer GetObjectContainer(int id, IItemContainerBase parent) => ObjectContainerCache.GetValue(id, id => new ObjectContainer(id, parent, this));
        public ContainerObject GetObject(int id, IItemContainerBase parent) => ContainerObjectCache.GetValue(id, id => new ContainerObject(id, parent, this));
        #endregion

        private bool CanBeParentOf(IOrganizerItem item)
        {
            if (item is not INamedItemBase itemTyped)
                throw new ArgumentTypeException(nameof(item), "Unsupported item type.");

            return CanHaveItemWithName(itemTyped.GetName());
        }

        public bool CanHaveItemWithName(string name)
        {
            return !Query.HasNameInOrganizer(Connection, name);
        }

        private bool HasItem(IOrganizerItem item)
        {
            if (item is not Item)
                throw new ArgumentTypeException(nameof(item), "Invalid item type.");

            return (item is Document doc && Query.OrganizerHasDocument(Connection, doc.ItemID)) ||
                   (item is Category cat && Query.OrganizerHasCategory(Connection, cat.ItemID)) ||
                   (item is Book book && Query.OrganizerHasBook(Connection, book.ItemID));
        }

        public void OnItemRemoved(IItem item)
        {
            if (item is not IOrganizerItem itemTyped)
                throw new ArgumentTypeException(nameof(item), "Invalid item type");
            else if (!HasItem(itemTyped))
                throw new ArgumentException("This container does not have such item.", nameof(item));

            InvokeItemContainerContentChanged(itemTyped, ItemContainerContentChangedEventArgs<IOrganizerItem>.ChangeType.ItemRemoved, ItemContainerContentChangedEventArgs<IOrganizerItem>.ResultType.Success);
        }
    }
}
