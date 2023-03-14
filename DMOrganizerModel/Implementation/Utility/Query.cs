using System.Data.SQLite;
using System.Collections.Generic;
using System.Xml.Linq;
using DMOrganizerModel.Interface.Items;
using DMOrganizerModel.Implementation.Items;
using System.Security.Cryptography;
using System.Reflection.PortableExecutable;
using static System.Data.Entity.Infrastructure.Design.Executor;
using System.Net;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;

namespace DMOrganizerModel.Implementation.Utility
{
    internal static class Query
    {
        #region Organizer
        public static bool HasNameInOrganizer(SyncronizedSQLiteConnection connection, string name)
        {
            bool success = false;
            connection.Read(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();
                cmd.CommandText = $"(SELECT ID FROM Category WHERE Title=$title AND Parent=NULL)" +
                                  " UNION ALL " +
                                  $"(SELECT Section.ID FROM (Document INNER JOIN Section ON Document.SectionID=Section.ID) WHERE Section.Title=$title AND Document.CategoryID=NULL)";
                cmd.Parameters.AddWithValue("$title", name);
                success = cmd.ExecuteScalar() != null;
            });
            return success;
        }

        public static bool OrganizerHasDocument(SyncronizedSQLiteConnection connection, int documentID)
        {
            bool success = false;
            connection.Read(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();
                cmd.CommandText = $"SELECT SectionID FROM Document WHERE SectionID={documentID} AND CategoryID=NULL";
                success = cmd.ExecuteNonQuery() > 0;
            });
            return success;
        }

        public static bool OrganizerHasCategory(SyncronizedSQLiteConnection connection, int categoryID)
        {
            bool success = false;
            connection.Read(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();
                cmd.CommandText = $"SELECT ID FROM Category WHERE ID={categoryID} AND Parent=NULL";
                success = cmd.ExecuteNonQuery() > 0;
            });
            return success;
        }

        public static List<int> GetDocumentsInOrganizer(SyncronizedSQLiteConnection connection)
        {
            List<int> res = new List<int>();
            connection.Read(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();
                cmd.CommandText = $"SELECT SectionID FROM Document WHERE CategoryID=NULL";
                using SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                    res.Add(reader.GetInt32(0));
            });
            return res;
        }

        public static List<int> GetCategoriesInOrganizer(SyncronizedSQLiteConnection connection)
        {
            List<int> res = new List<int>();
            connection.Read(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();
                cmd.CommandText = $"SELECT ID FROM Category WHERE Parent=NULL";
                using SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                    res.Add(reader.GetInt32(0));
            });
            return res;
        }
        #endregion

        #region Document
        public static int CreateDocument(SyncronizedSQLiteConnection connecton, string name, int? parentID)
        {
            int res = -1;
            connecton.Write(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();
                if (parentID.HasValue)
                {
                    cmd.CommandText = $"INSERT INTO Section (Title) VALUES ($title);" +
                                  $"INSERT INTO Document (CategoryID, SectionID) VALUES ({parentID}, last_insert_rowid());" +
                                  $"SELECT SectionID FROM Document WHERE rowid=last_insert_rowid();";
                }
                else
                {
                    cmd.CommandText = $"INSERT INTO Section (Title) VALUES ($title);" +
                                  $"INSERT INTO Document (SectionID) VALUES (last_insert_rowid());" +
                                  $"SELECT SectionID FROM Document WHERE rowid=last_insert_rowid();";
                }
                cmd.Parameters.AddWithValue("$title", name);
                res = (int)cmd.ExecuteScalar();
            });
            return res;
        }

        public static bool DocumentHasTag(SyncronizedSQLiteConnection connection, int documentID, string tag)
        {
            bool success = false;
            connection.Read(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();
                cmd.CommandText = $"SELECT DocumentsTags.TagID FROM (Tag INNER JOIN DocumentsTags ON Tag.ID=DocumentsTags.TagID) WHERE DocumentsTags.DocumentID={documentID} AND Tag.Text=$tag";
                cmd.Parameters.AddWithValue("$tag", tag);
                success = cmd.ExecuteScalar() != null;
            });
            return success;
        }

        public static bool AddDocumentTag(SyncronizedSQLiteConnection connection, int documentID, string tag)
        {
            bool success = false;
            connection.Write(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();
                cmd.CommandText = $"SELECT ID FROM Tag WHERE Text=$tag";
                cmd.Parameters.AddWithValue("$tag", tag);

                int? id = (int?)cmd.ExecuteScalar();

                if (id.HasValue)
                {
                    cmd.CommandText = $"INSERT INTO DocumentsTags (DocumentID, TagID) VALUES ({documentID}, {id})";
                    cmd.Parameters.Clear();
                }
                else
                    cmd.CommandText = "INSERT INTO Tag (Text) VALUES ($tag);" +
                                      $"INSERT INTO DocumentsTags (DocumentID, TagID) VALUES ({documentID}, last_insert_rowid())";
                
                success = cmd.ExecuteNonQuery() > 0;
            });
            return success;
        }

        public static bool RemoveDocumentTag(SyncronizedSQLiteConnection connection, int documentID, string tag)
        {
            bool success = false;
            connection.Write(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();
                cmd.CommandText = "DELETE FROM DocumentsTags WHERE TagID IN (SELECT ID FROM Tag WHERE Text=$tag)";
                cmd.Parameters.AddWithValue("$tag", tag);
                success = cmd.ExecuteNonQuery() > 0;
            });
            return success;
        }

        public static List<string> GetDocumentTags(SyncronizedSQLiteConnection connection, int documentID)
        {
            List<string> result = new List<string>();
            connection.Read(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();
                cmd.CommandText = $"SELECT Tag.Text FROM (Tag INNER JOIN DocumentsTags ON DocumentsTags.TagID=Tag.ID) WHERE DocumentsTags.DocumentID={documentID}";
                using SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                    result.Add(reader.GetString(0));
            });
            return result;
        }

        public static bool SetDocumentParent(SyncronizedSQLiteConnection connection, int documentID, int categoryID)
        {
            bool success = false;
            connection.Write(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();
                cmd.CommandText = $"UPDATE Document SET CategoryID={categoryID} WHERE SectionID={documentID}";
                success = cmd.ExecuteNonQuery() > 0;
            });
            return success;
        }
        #endregion

        #region Section
        public static List<int> GetSectionsInSection(SyncronizedSQLiteConnection connection, int sectionID)
        {
            List<int> res = new List<int>();
            connection.Read(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();
                cmd.CommandText = $"SELECT ID FROM Section WHERE Parent={sectionID}";
                using SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                    res.Add(reader.GetInt32(0));
            });
            return res;
        }

        public static bool HasNameInSection(SyncronizedSQLiteConnection connection, string name, int sectionID)
        {
            bool success = false;
            connection.Read(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();
                cmd.CommandText = $"(SELECT ID FROM Section WHERE Title=$title AND Parent={sectionID})";
                cmd.Parameters.AddWithValue("$title", name);
                success = cmd.ExecuteScalar() != null;
            });
            return success;
        }
        
        public static bool SetSectionParent(SyncronizedSQLiteConnection connection, int sectionID, int parentID)
        {
            bool success = false;
            connection.Write(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();
                cmd.CommandText = $"UPDATE Category SET Parent={parentID} WHERE ID={sectionID}";
                success = cmd.ExecuteNonQuery() > 0;
            });
            return success;
        }

        public static bool SectionHasSection(SyncronizedSQLiteConnection connection, int sectionID, int parentID)
        {
            bool success = false;
            connection.Read(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();
                cmd.CommandText = $"SELECT ID FROM Section WHERE Parent={parentID} AND ID={sectionID}";
                success = cmd.ExecuteNonQuery() > 0;
            });
            return success;
        }

        public static bool SetSectionName(SyncronizedSQLiteConnection connection, int sectionID, string name)
        {
            bool success = false;
            connection.Write(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();
                cmd.CommandText = $"UPDATE Section SET Title=$title WHERE ID={sectionID}";
                cmd.Parameters.AddWithValue("$title", name);
                success = cmd.ExecuteNonQuery() > 0;
            });
            return success;
        }

        public static string GetSectionName(SyncronizedSQLiteConnection connection, int sectionID)
        {
            string res = null;
            connection.Read(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();
                cmd.CommandText = $"SELECT Title FROM Section WHERE ID={sectionID}";
                using SQLiteDataReader reader = cmd.ExecuteReader();
                res = reader.Read() ? reader.GetString(0) : null;
            });
            return res;
        }

        public static bool SetSectionContent(SyncronizedSQLiteConnection connection, int sectionID, string content)
        {
            bool success = false;
            connection.Write(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();
                cmd.CommandText = $"UPDATE Section SET Content=$content WHERE ID={sectionID}";
                cmd.Parameters.AddWithValue("$content", content);
                success = cmd.ExecuteNonQuery() > 0;
            });
            return success;
        }

        public static string GetSectionContent(SyncronizedSQLiteConnection connection, int sectionID)
        {
            string res = null;
            connection.Read(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();
                cmd.CommandText = $"SELECT Content FROM Section WHERE ID={sectionID}";
                using SQLiteDataReader reader = cmd.ExecuteReader();
                res = reader.Read() ? reader.GetString(0) : null;
            });
            return res;
        }

        public static int CreateSection(SyncronizedSQLiteConnection connecton, string name, int parentID)
        {
            int res = -1;
            connecton.Write(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();
                cmd.CommandText = $"INSERT INTO Section (Title, Parent) VALUES ($title, {parentID});" +
                                  $"SELECT last_insert_rowid();";
                cmd.Parameters.AddWithValue("$title", name);
                res = (int)cmd.ExecuteScalar();
            });
            return res;
        }

        public static bool DeleteSection(SyncronizedSQLiteConnection connection, int sectionID)
        {
            bool success = false;
            connection.Write(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();
                cmd.CommandText = $"DELETE FROM Section WHERE ID={sectionID}";
                success = cmd.ExecuteNonQuery() > 0;
            });
            return success;
        }
        #endregion

        #region Category
        public static bool HasNameInCategory(SyncronizedSQLiteConnection connection, string name, int categoryID)
        {
            bool success = false;
            connection.Read(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();
                cmd.CommandText = $"(SELECT ID FROM Category WHERE Title=$title AND Parent={categoryID})" +
                                  " UNION ALL " +
                                  $"(SELECT Section.ID FROM (Document INNER JOIN Section ON Document.SectionID=Section.ID) WHERE Section.Title=$title AND Document.CategoryID={categoryID})";
                cmd.Parameters.AddWithValue("$title", name);
                success = cmd.ExecuteScalar() != null;
            });
            return success;
        }

        public static int CreateCategory(SyncronizedSQLiteConnection connection, string name, int? parentID)
        {
            int res = -1;
            connection.Write(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();
                if (parentID.HasValue)
                {
                    cmd.CommandText = $"INSERT INTO Category (Title, Parent) VALUES ($title, {parentID});" +
                                  $"SELECT last_insert_rowid();";
                }
                else
                {
                    cmd.CommandText = $"INSERT INTO Category (Title) VALUES ($title);" +
                                  $"SELECT last_insert_rowid();";
                }
                
                cmd.Parameters.AddWithValue("$title", name);
                res = (int)cmd.ExecuteScalar();
            });
            return res;
        }

        public static List<int> GetDocumentsInCategory(SyncronizedSQLiteConnection connection, int categoryID)
        {
            List<int> res = new List<int>();
            connection.Read(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();
                cmd.CommandText = $"SELECT SectionID FROM Document WHERE CategoryID={categoryID}";
                using SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                    res.Add(reader.GetInt32(0));
            });
            return res;
        }

        public static List<int> GetCategoriesInCategory(SyncronizedSQLiteConnection connection, int categoryID)
        {
            List<int> res = new List<int>();
            connection.Read(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();
                cmd.CommandText = $"SELECT ID FROM Category WHERE Parent={categoryID}";
                using SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                    res.Add(reader.GetInt32(0));
            });
            return res;
        }

        public static bool CategoryHasDocument(SyncronizedSQLiteConnection connection, int documentID, int categoryID)
        {
            bool success = false;
            connection.Read(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();
                cmd.CommandText = $"SELECT SectionID FROM Document WHERE SectionID={documentID} AND CategoryID={categoryID}";
                success = cmd.ExecuteNonQuery() > 0;
            });
            return success;
        }

        public static bool CategoryHasCategory(SyncronizedSQLiteConnection connection, int categoryID, int parentID)
        {
            bool success = false;
            connection.Read(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();
                cmd.CommandText = $"SELECT ID FROM Category WHERE ID={categoryID} AND Parent={parentID}";
                success = cmd.ExecuteNonQuery() > 0;
            });
            return success;
        }

        public static bool SetCategoryParent(SyncronizedSQLiteConnection connection, int categoryID, int? parentID)
        {
            bool success = false;
            connection.Write(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();
                if (parentID.HasValue)
                    cmd.CommandText = $"UPDATE Category SET Parent={parentID} WHERE ID={categoryID}";
                else
                    cmd.CommandText = $"UPDATE Category SET Parent=NULL WHERE ID={categoryID}";
                success = cmd.ExecuteNonQuery() > 0;
            });
            return success;
        }

        public static bool SetCategoryName(SyncronizedSQLiteConnection connection, int categoryID, string name)
        {
            bool success = false;
            connection.Write(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();
                cmd.CommandText = $"UPDATE Category SET Title=$title WHERE ID={categoryID}";
                cmd.Parameters.AddWithValue("$title", name);
                success = cmd.ExecuteNonQuery() > 0;
            });
            return success;
        }

        public static string GetCategoryName(SyncronizedSQLiteConnection connection, int categoryID)
        {
            string result = null;
            connection.Read(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();
                cmd.CommandText = $"SELECT Title FROM Category WHERE ID={categoryID}";
                using SQLiteDataReader reader = cmd.ExecuteReader();
                result = reader.Read() ? reader.GetString(0) : null;
            });
            return result;
        }

        public static bool DeleteCategory(SyncronizedSQLiteConnection connection, int categoryID)
        {
            bool success = false;
            connection.Write(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();
                cmd.CommandText = $"DELETE FROM Category WHERE ID={categoryID}";
                success = cmd.ExecuteNonQuery() > 0;
            });
            return success;
        }
        #endregion

        #region Book
        //create empty book with parent
        public static int CreateBook(SyncronizedSQLiteConnection connection, string name, int parentID)
        {
            int res = -1;
            connection.Write(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();

                cmd.CommandText = @"INSERT INTO Book (Title, ID_Parent_Category) 
                                            VALUES (@BookName, @BookParentID);";

                cmd.Parameters.AddWithValue("@BookName", name);
                cmd.Parameters.AddWithValue("@BookParentID", parentID);
                res = (int)cmd.ExecuteScalar(); //res = null при ошибке или ID созданной книги
            });
            return res;
        }

        //create book at root (parent is Organizer, not Category type)
        public static int CreateBook(SyncronizedSQLiteConnection connection, string name)
        {
            int res = -1;
            connection.Write(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();

                cmd.CommandText = @"INSERT INTO Book (Title, ID_Parent_Category) 
                                            VALUES (@BookName, NULL);";

                cmd.Parameters.AddWithValue("@BookName", name);
                res = (int)cmd.ExecuteScalar();
            });
            return res;
        }

        //delete book
        public static bool DeleteBook(SyncronizedSQLiteConnection connection, int bookID)
        {
            int result = -1;
            connection.Write(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();

                cmd.CommandText = @"DELETE FROM Book
                                    WHERE ID is @BookID;";
                cmd.Parameters.AddWithValue("@BookID", bookID);
                result = cmd.ExecuteNonQuery();
            });
            return result > 0;
        }

        //add page in the end or in position
        public static int CreatePadeInBook(SyncronizedSQLiteConnection connection, int bookID, int pagePosition)
        {
            int res = -1;
            connection.Write(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();

                cmd.CommandText = @"INSERT INTO Page (Position, ID_Parent_Book) 
                                            VALUES (@Position, @BookParentID);"
                ;

                cmd.Parameters.AddWithValue("@Position", pagePosition);
                cmd.Parameters.AddWithValue("@BookParentID", bookID);
                res = (int)cmd.ExecuteScalar(); //new page's id
            });
            return res;
        }

        //get book name - return string
        public static string GetBookName(SyncronizedSQLiteConnection connection, int bookID)
        {
            string result = null;
            connection.Read(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();
                cmd.CommandText = @"SELECT Book.Title
                                    FROM Book
                                    WHERE Book.ID is @BookID;";
                cmd.Parameters.AddWithValue("@BookID", bookID);
                using SQLiteDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                    { result = reader.GetString(0); }
                else result = null;
            });
            return result;
        }
        //set book name
        public static bool SetBookName(SyncronizedSQLiteConnection connection, int bookID, string bookName)
        {
            int result = -1;
            
            connection.Read(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();
                cmd.CommandText = @"UPDATE Book 
                                    SET Title = @NewName 
                                    WHERE Book.ID is @BookID;";
                cmd.Parameters.AddWithValue("@NewName", bookName);
                cmd.Parameters.AddWithValue("@BookID", bookID);
                result = cmd.ExecuteNonQuery();
            }
            );
            return result > 0;
        }
        //get book content - return IEnumerable<IPage>, ID's of pages
        public static List<int> GetBookContent(SyncronizedSQLiteConnection connection, int bookID)
        {
            List<int> res = new List<int>();
            connection.Read(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();
                cmd.CommandText = @"SELECT Page.ID 
                                    FROM Page 
                                    WHERE Page.ID_Parent_Book = @ParentBookID;";
                cmd.Parameters.AddWithValue("@ParentBookID", bookID);
                using SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                    res.Add(reader.GetInt32(0));
            });
            return res;
        }

        public static int GetPageIDByPosition(SyncronizedSQLiteConnection connection, int bookID, int position)
        {
            int result = -1;

        }

        //check if book has item <IPage> a
        public static bool BookHasPage(SyncronizedSQLiteConnection connection, int bookID, int pageID)
        {
            int result = -1;
            connection.Read(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();
                cmd.CommandText = @"SELECT Page.ID
                                    FROM Page
                                    WHERE Page.ID_Parent_Book is @BookParentID 
                                    AND Page.ID is @PageID;";
                result = (int)cmd.ExecuteScalar();
            }
            );
            return result > 0;
        }

        //set this book's parent with category-parent
        public static bool SetBookParent(SyncronizedSQLiteConnection connection, int bookID, int parentID)
        {
            int result = -1;
            connection.Write(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();

                cmd.CommandText = @"UPDATE Book 
                                    SET ID_Parent_Category = @BookParentID 
                                    WHERE Book.ID is @BookID;";

                cmd.Parameters.AddWithValue("@BookParentID", parentID);
                cmd.Parameters.AddWithValue("@BookID", bookID);
                result = cmd.ExecuteNonQuery();
            });
            return result > 0;
        }

        //set root as book's parent (parent is Organizer, not Category type)
        public static bool SetBookParent(SyncronizedSQLiteConnection connection, int bookID)
        {
            int result = -1;
            connection.Write(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();

                cmd.CommandText = @"UPDATE Book 
                                    SET ID_Parent_Category = NULL 
                                    WHERE Book.ID is @BookID;";
                cmd.Parameters.AddWithValue("@BookID", bookID);
                result = cmd.ExecuteNonQuery();
            });
            return result > 0;
        }
        #endregion

        #region Page
        // PAGE
        //get pages maximum position to add new page at the end, if no pages returns 0
        public static int MaxPagePosition(SyncronizedSQLiteConnection connection, int parentBookID)
        {
            int result = 0;
            connection.Read(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();
                cmd.CommandText = @"SELECT MAX(Page.Position) 
                                    FROM Page 
                                    WHERE Page.ID_Parent_Book is @ParentBookID;";
                
                cmd.Parameters.AddWithValue("@ParentBookID", parentBookID);
                using SQLiteDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                { result = reader.GetInt32(0); }
                else result = 0;
            });
            return result;
        }

        //change page position arg-old and new position
        public static bool SetPagePosition(SyncronizedSQLiteConnection connection, int bookID, int oldPosition, int newPosition)
        {
            int result = -1;
            connection.Write(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();

                cmd.CommandText = @"UPDATE Page 
                                    SET Position = @NewPosition 
                                    WHERE Page.Position is @OldPosition
                                    AND Page.ID_Parent_Book is @BookID;";
                cmd.Parameters.AddWithValue("@NewPosition", newPosition);
                cmd.Parameters.AddWithValue("@OldPosition", oldPosition);
                cmd.Parameters.AddWithValue("@BookID", bookID);
                result = cmd.ExecuteNonQuery();
            });
            return result > 0;
        }

        //remove page
        public static bool DeletePage(SyncronizedSQLiteConnection connection, int pageID)
        {
            int result = -1;
            connection.Write(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();

                cmd.CommandText = @"DELETE FROM Page
                                    WHERE ID is @PageID;";
                cmd.Parameters.AddWithValue("@PageID", pageID);
                result = cmd.ExecuteNonQuery();
            });
            return result > 0;
        }

        //set book as this page's parent
        public static bool SetPageParent(SyncronizedSQLiteConnection connection, int pageID, int parentID)
        {
            int result = -1;
            connection.Write(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();

                cmd.CommandText = @"UPDATE Page 
                                    SET ID_Parent_Book = @ParentID 
                                    WHERE Page.ID is @PageID;";

                cmd.Parameters.AddWithValue("@ParentID", parentID);
                cmd.Parameters.AddWithValue("@PageID", pageID);
                result = cmd.ExecuteNonQuery();
            });
            return result > 0;

        }

        //get pages to change positions - return List<int>, old positions of pages
        public static List<int> GetPagesPositionsToChange(SyncronizedSQLiteConnection connection, int bookID, int borderPosition)
        {
            List<int> res = new List<int>();
            connection.Read(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();
                cmd.CommandText = @"SELECT Page.Position 
                                    FROM Page 
                                    WHERE Page.ID_Parent_Book = @ParentBookID
                                    AND Page.ID >= @BorderPagePos;";
                cmd.Parameters.AddWithValue("@ParentBookID", bookID);
                cmd.Parameters.AddWithValue("@BorderPagePos", borderPosition);
                using SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                    res.Add(reader.GetInt32(0));
            });
            return res;
        }
        #endregion
    }
}
