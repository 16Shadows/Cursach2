using System.Data.SQLite;
using System.Collections.Generic;
using DMOrganizerModel.Implementation.Items;

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
                cmd.CommandText = @"SELECT ID FROM Category WHERE Title = @Name AND Parent is NULL

                                    UNION ALL

                                    SELECT Section.ID
                                    FROM(Document INNER JOIN Section ON Document.SectionID= Section.ID)
                                    WHERE Section.Title=@Name
                                    AND Document.CategoryID is NULL

                                    UNION ALL

                                    SELECT Book.ID
                                    FROM Book
                                    WHERE Book.Title IS @Name
                                    AND Book.ID_Parent_Category is NULL;";
                cmd.Parameters.AddWithValue("@Name", name);
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
                cmd.CommandText = $"SELECT SectionID FROM Document WHERE SectionID={documentID} AND CategoryID IS NULL";
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
                cmd.CommandText = $"SELECT ID FROM Category WHERE ID={categoryID} AND Parent IS NULL";
                success = cmd.ExecuteNonQuery() > 0;
            });
            return success;
        }
        public static bool OrganizerHasBook(SyncronizedSQLiteConnection connection, int bookID)
        {
            bool success = false;
            connection.Read(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();
                cmd.CommandText = @"SELECT ID 
                                    FROM Book 
                                    WHERE ID is @BookID 
                                    AND ID_Parent_Category IS NULL;";
                cmd.Parameters.AddWithValue("@BookID", bookID);
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
                cmd.CommandText = $"SELECT SectionID FROM Document WHERE CategoryID IS NULL ORDER BY SectionID";
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
                cmd.CommandText = $"SELECT ID FROM Category WHERE Parent IS NULL ORDER BY ID";
                using SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                    res.Add(reader.GetInt32(0));
            });
            return res;
        }

        public static List<int> GetBooksInOrganizer(SyncronizedSQLiteConnection connection)
        {
            List<int> res = new List<int>();
            connection.Read(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();
                cmd.CommandText = @"SELECT ID 
                                    FROM Book
                                    WHERE ID_Parent_Category IS NULL
                                    ORDER BY ID;";
                using SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                    res.Add(reader.GetInt32(0));
            });
            return res;
        }
        #endregion

        #region Document
        public static int GetDocumentByName(SyncronizedSQLiteConnection connection, string name, int? parentID)
        {
            int res = -1;
            connection.Read(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();
                if (parentID.HasValue)
                    cmd.CommandText = $"SELECT Section.ID FROM (Document INNER JOIN Section ON Section.ID = Document.SectionID) WHERE Section.Title=@title AND Document.CategoryID={parentID.Value}";
                else
                    cmd.CommandText = $"SELECT Section.ID FROM (Document INNER JOIN Section ON Section.ID = Document.SectionID) WHERE Section.Title=@title AND Document.CategoryID IS NULL";
                cmd.Parameters.AddWithValue("@title", name);
                using SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                    res = reader.GetInt32(0);
            });
            return res;
        }

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
                using SQLiteDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                    res = reader.GetInt32(0);
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
                using SQLiteDataReader reader = cmd.ExecuteReader();
                int? id = reader.Read() ? reader.GetInt32(0) : null;
                reader.Close();

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
        public static int GetSectionByName(SyncronizedSQLiteConnection connection, string name, int parentID)
        {
            int res = -1;
            connection.Read(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();
                cmd.CommandText = $"SELECT ID FROM Section WHERE Title=@title AND Parent={parentID}";
                cmd.Parameters.AddWithValue("@title", name);
                using SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                    res = reader.GetInt32(0);
            });
            return res;
        }

        public static List<int> GetSectionsInSection(SyncronizedSQLiteConnection connection, int sectionID)
        {
            List<int> res = new List<int>();
            connection.Read(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();
                cmd.CommandText = $"SELECT ID FROM Section WHERE Parent={sectionID} ORDER BY ID";
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
                cmd.CommandText = $"SELECT ID FROM Section WHERE Title=$title AND Parent={sectionID}";
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
                using SQLiteDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                    res = reader.GetInt32(0);
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
                cmd.CommandText = @"SELECT ID FROM Category WHERE Title=@Name AND Parent=@CategoryID

                                    UNION ALL 

                                    SELECT Section.ID 
                                    FROM (Document INNER JOIN Section ON Document.SectionID=Section.ID) 
                                    WHERE Section.Title=@Name
                                    AND Document.CategoryID=@CategoryID

                                    UNION ALL

                                    SELECT Book.ID 
                                    FROM Book
                                    WHERE Book.Title IS @Name
                                    AND Book.ID_Parent_Category=@CategoryID;";
                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@CategoryID", categoryID);
                //cmd.CommandText = $"(SELECT ID FROM Category WHERE Title=$title AND Parent={categoryID})" +
                //                  " UNION ALL " +
                //                  $"(SELECT Section.ID FROM (Document INNER JOIN Section ON Document.SectionID=Section.ID) WHERE Section.Title=$title AND Document.CategoryID={categoryID})";
                //cmd.Parameters.AddWithValue("$title", name);
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
                using SQLiteDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                    res = reader.GetInt32(0);
            });
            return res;
        }

        public static int GetCategoryByName(SyncronizedSQLiteConnection connection, string name, int? parentID)
        {
            int res = -1;
            connection.Read(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();
                if (parentID.HasValue)
                    cmd.CommandText = $"SELECT ID FROM Category WHERE Title=@title AND Parent={parentID.Value}";
                else
                    cmd.CommandText = $"SELECT ID FROM Category WHERE Title=@title AND Parent IS NULL";
                cmd.Parameters.AddWithValue("@title", name);
                using SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                    res = reader.GetInt32(0);
            });
            return res;
        }

        public static List<int> GetDocumentsInCategory(SyncronizedSQLiteConnection connection, int categoryID)
        {
            List<int> res = new List<int>();
            connection.Read(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();
                cmd.CommandText = $"SELECT SectionID FROM Document WHERE CategoryID={categoryID} ORDER BY SectionID";
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
                cmd.CommandText = $"SELECT ID FROM Category WHERE Parent={categoryID} ORDER BY ID";
                using SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                    res.Add(reader.GetInt32(0));
            });
            return res;
        }

        //get books' ids in category
        public static List<int> GetBooksInCategory(SyncronizedSQLiteConnection connection, int categoryID)
        {
            List<int> res = new List<int>();
            connection.Read(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();
                cmd.CommandText = @"SELECT Book.ID 
                                    FROM Book 
                                    WHERE Book.ID_Parent_Category = @ParentCategoryID
                                    ORDER BY Book.ID;";
                cmd.Parameters.AddWithValue("@ParentCategoryID", categoryID);
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

        //checks if category has Book 
        public static bool CategoryHasBook(SyncronizedSQLiteConnection connection, int categoryID, int bookID)
        {
            int result = -1;
            connection.Read(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();
                cmd.CommandText = @"SELECT Book.ID
                                    FROM Category
                                    WHERE Book.ID_Parent_Category is @CategoryParentID 
                                    AND Page.ID is @BookID;";
                cmd.Parameters.AddWithValue("@CategoryParentID", categoryID);
                cmd.Parameters.AddWithValue("@BookID", bookID);
                using SQLiteDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                    result = reader.GetInt32(0);
            }
            );
            return result > 0;
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
                    cmd.CommandText = $"UPDATE Category SET Parent IS NULL WHERE ID={categoryID}";
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
                                            VALUES (@BookName, @BookParentID);
                                    SELECT last_insert_rowid();";

                cmd.Parameters.AddWithValue("@BookName", name);
                cmd.Parameters.AddWithValue("@BookParentID", parentID);
                using SQLiteDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                    res = reader.GetInt32(0);
            });
            return res;
        }

        //create empty book at root (parent is Organizer, not Category type)
        public static int CreateBook(SyncronizedSQLiteConnection connection, string name)
        {
            int res = -1;
            connection.Write(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();

                cmd.CommandText = @"INSERT INTO Book (Title, ID_Parent_Category) 
                                            VALUES (@BookName, NULL);
                                    SELECT last_insert_rowid();";

                cmd.Parameters.AddWithValue("@BookName", name);
                using SQLiteDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                    res = reader.GetInt32(0);
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
                                            VALUES (@Position, @BookParentID);
                                    SELECT last_insert_rowid();"
                ;

                cmd.Parameters.AddWithValue("@Position", pagePosition);
                cmd.Parameters.AddWithValue("@BookParentID", bookID);
                using SQLiteDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                    res = reader.GetInt32(0);
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

        //check if book has item <IPage> a
        public static bool BookHasPage(SyncronizedSQLiteConnection connection, int bookID, int pagePosition)
        {
            int result = -1;
            connection.Read(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();
                cmd.CommandText = @"SELECT Page.ID
                                    FROM Page
                                    WHERE Page.ID_Parent_Book is @BookParentID 
                                    AND Page.Position is @PagePos;";
                cmd.Parameters.AddWithValue("@BookParentID", bookID);
                cmd.Parameters.AddWithValue("@PagePos", pagePosition);
                using SQLiteDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                    result = reader.GetInt32(0);
            });
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
        //create page
        public static int CreatePage(SyncronizedSQLiteConnection connection, int parentID, int position)
        {
            int res = -1;
            connection.Write(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();

                cmd.CommandText = @"INSERT INTO Page (Position, ID_Parent_Book) 
                                            VALUES (@PagePosition, @BookParentID);
                                    SELECT last_insert_rowid();";
                cmd.Parameters.AddWithValue("@PagePosition", position);
                cmd.Parameters.AddWithValue("@BookParentID", parentID);
                using SQLiteDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                    res = reader.GetInt32(0);
            });
            return res;
        }
        //get page position by id
        public static int GetPagePosition(SyncronizedSQLiteConnection connection, int pageID)
        {
            int result = -1;
            connection.Read(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();
                cmd.CommandText = @"SELECT Page.Position
                                    FROM Page
                                    WHERE Page.ID is @PageID;";
                cmd.Parameters.AddWithValue("@PageID", pageID);
                using SQLiteDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                { result = reader.GetInt32(0); }
            });
            return result;
        }

        //get page id by book and position
        public static int GetPageID(SyncronizedSQLiteConnection connection, int parentBookID, int pagePosition)
        {
            int result = 0;
            connection.Read(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();
                cmd.CommandText = @"SELECT Page.ID 
                                    FROM Page 
                                    WHERE Page.ID_Parent_Book is @ParentBookID
                                    AND Page.Position is @PagePos;";

                cmd.Parameters.AddWithValue("@ParentBookID", parentBookID);
                cmd.Parameters.AddWithValue("@PagePos", pagePosition);
                using SQLiteDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                { result = reader.GetInt32(0); }
                else result = 0;
            });
            return result;
        }

        //get page content
        public static List<int> GetPageContent(SyncronizedSQLiteConnection connection, int pageID)
        {
            List<int> res = new List<int>();
            connection.Read(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();
                cmd.CommandText = @"SELECT Container.ID
                    FROM Container, Set_Page_Containers
                    WHERE Set_Page_Containers.ID_Container = Container.ID
                    AND Set_Page_Containers.ID_Page is @PageID;";
                cmd.Parameters.AddWithValue("@PageID", pageID);
                using SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                    res.Add(reader.GetInt32(0));
            });
            return res;
        }
        
        //page has container, has item
        public static bool PageHasContainer(SyncronizedSQLiteConnection connection, int pageID, int containerID)
        {
            int result = -1;
            connection.Read(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();
                cmd.CommandText = @"SELECT Container.ID
                                    FROM Container, Set_Page_Containers
                                    WHERE Set_Page_Containers.ID_Page = @PageID
                                    AND
                                    Set_Page_Containers.ID_Container = @ContainerID;";
                cmd.Parameters.AddWithValue("@ContainerID", containerID);
                cmd.Parameters.AddWithValue("@PageID", pageID);
                using SQLiteDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                    result = reader.GetInt32(0);
            });
            return result > 0;
        }

        //get pages maximum position to add new page at the end, if no pages returns 0
        public static int MaxPagePosition(SyncronizedSQLiteConnection connection, int parentBookID)
        {
            int result = 0;
            connection.Read(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();
                cmd.CommandText = @"SELECT IFNULL(MAX(Page.Position), 0)
                                    FROM Page 
                                    WHERE Page.ID_Parent_Book is @ParentBookID;";
                
                cmd.Parameters.AddWithValue("@ParentBookID", parentBookID);
                using SQLiteDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                    result = reader.GetInt32(0);
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

                cmd.CommandText = @"BEGIN TRANSACTION;
                                    DELETE FROM Set_Page_Containers
                                    WHERE Set_Page_Containers.ID_Page IS @PageID;
                                    
                                    DELETE FROM Page
                                    WHERE Page.ID IS @PageID;
                                    COMMIT;";
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
                                    AND Page.Position > @BorderPagePos;";
                cmd.Parameters.AddWithValue("@ParentBookID", bookID);
                cmd.Parameters.AddWithValue("@BorderPagePos", borderPosition);
                using SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                    res.Add(reader.GetInt32(0));
            });
            return res;
        }
        #endregion

        #region ObjectContainer

        // create object container
        public static int CreateObjectContainer(SyncronizedSQLiteConnection connection, int type)
        {
            int res = -1;
            connection.Write(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();

                cmd.CommandText = @"INSERT INTO Container (Type) 
                                            VALUES (@Type);
                                    SELECT last_insert_rowid();";

                cmd.Parameters.AddWithValue("@Type", type);
                using SQLiteDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                    res = reader.GetInt32(0);
            });
            return res;
        }
        //delete object container
        public static bool DeleteObjectContainer(SyncronizedSQLiteConnection connection, int containerID)
        {
            int result = -1;
            connection.Write(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();

                cmd.CommandText = @"BEGIN TRANSACTION;
                                    DELETE FROM Set_Page_Containers
                                    WHERE Set_Page_Containers.ID_Container IS @ContainerID;
                                    
                                    DELETE FROM Container
                                    WHERE Container.ID IS @ContainerID;
                                    COMMIT;";
                cmd.Parameters.AddWithValue("@ContainerID", containerID);
                result = cmd.ExecuteNonQuery();
            });
            return result > 0;
        }
        // set parent
        public static bool SetObjectContainerParent(SyncronizedSQLiteConnection connection, int pageParentID, int containerID)
        {
            int result = -1;
            connection.Write(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();

                cmd.CommandText = @"INSERT INTO Set_Page_Containers (ID_Page, ID_Container) 
                                    VALUES (@PageID, @ContainerID);";

                cmd.Parameters.AddWithValue("@PageID", pageParentID);
                cmd.Parameters.AddWithValue("@ContainerID", containerID);
                result = cmd.ExecuteNonQuery();
            });
            return result > 0;
        }

        //get content
        public static List<int> GetContainerContent(SyncronizedSQLiteConnection connection, int containerID)
        {
            List<int> res = new List<int>();
            connection.Read(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();
                cmd.CommandText = @"SELECT ID_Object
                                    FROM  Set_Container_Objects
                                    WHERE Set_Container_Objects.ID_Container is @ContainerID;";
                cmd.Parameters.AddWithValue("@ContainerID", containerID);
                using SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                    res.Add(reader.GetInt32(0));
            });
            return res;
        }
        //get view info (width, height, x, y)
        /// <summary>
        /// Get container view info (width, height, x, y)
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="containerID"></param>
        /// <returns></returns>
        public static List<int> GetContainerViewInfo(SyncronizedSQLiteConnection connection, int containerID)
        {
            //List<int> res = new List<int>(4);
            List<int> res = new List<int> { 0, 0, 0, 0, 0 };
            connection.Read(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();
                cmd.CommandText = @"SELECT Container.Width, Container.Height, Container.CoordinateX, Container.CoordinateY, Container.Type
                                    FROM  Container
                                    WHERE Container.ID is @ContainerID;";
                cmd.Parameters.AddWithValue("@ContainerID", containerID);
                using SQLiteDataReader reader = cmd.ExecuteReader();
                if (reader.Read() && reader.FieldCount > 0 )
                {
                    res[0] = reader.GetInt32(0);
                    res[1] = reader.GetInt32(1);
                    res[2] = reader.GetInt32(2);
                    res[3] = reader.GetInt32(3);
                    res[3] = reader.GetInt32(3);
                }
            });
            return res;
        }

        //set coords
        public static bool SetContainerCoordinates(SyncronizedSQLiteConnection connection, int containerID, int newX, int newY)
        {
            int result = -1;

            connection.Read(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();
                cmd.CommandText = @"UPDATE Container
                                    SET CoordinateX = @NewX, CoordinateY = @NewY
                                    WHERE Container.ID is @ContainerID;";
                cmd.Parameters.AddWithValue("@NewX", newX);
                cmd.Parameters.AddWithValue("@NewY", newY);
                cmd.Parameters.AddWithValue("@ContainerID", containerID);
                result = cmd.ExecuteNonQuery();
            }
            );
            return result > 0;
        }
        //set size
        public static bool SetContainerSize(SyncronizedSQLiteConnection connection, int containerID, int newWidth, int newHeight)
        {
            int result = -1;

            connection.Read(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();
                cmd.CommandText = @"UPDATE Container
                                    SET Width = @NewWidth, Height = @NewHeight
                                    WHERE Container.ID is @ContainerID;";
                cmd.Parameters.AddWithValue("@NewWidth", newWidth);
                cmd.Parameters.AddWithValue("@NewHeight", newHeight);
                cmd.Parameters.AddWithValue("@ContainerID", containerID);
                result = cmd.ExecuteNonQuery();
            }
            );
            return result > 0;
        }
        //has item
        public static bool ContainerHasObject(SyncronizedSQLiteConnection connection, int containerID, int objectID)
        {
            int result = -1;
            connection.Read(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();
                cmd.CommandText = @"SELECT ID_Object
                                    FROM Set_Container_Objects
                                    WHERE Set_Container_Objects.ID_Container = @ContainerID
									AND Set_Container_Objects.ID_Object = @ObjectID;";
                cmd.Parameters.AddWithValue("@ContainerID", containerID);
                cmd.Parameters.AddWithValue("@ObjectID", objectID);
                using SQLiteDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                    result = reader.GetInt32(0);
            });
            return result > 0;
        }
        //set parent of this container
        public static bool SetContainerParent(SyncronizedSQLiteConnection connection, int parentID, int containerID)
        {
            int result = -1;
            connection.Write(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();

                cmd.CommandText = @"INSERT INTO Set_Page_Containers (ID_Page, ID_Container) 
                                    VALUES (@ParentID, @ContainerID);";

                cmd.Parameters.AddWithValue("@ParentID", parentID);
                cmd.Parameters.AddWithValue("@ContainerID", containerID);
                result = cmd.ExecuteNonQuery();
            });
            return result > 0;
        }
        public static List<int> GetContainerCoordinates(SyncronizedSQLiteConnection connection, int containerID)
        {
            List<int> res = new List<int>(2);
            connection.Read(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();
                cmd.CommandText = @"SELECT Container.CoordinateX, Container.CoordinateY
                                    FROM  Container
                                    WHERE Container.ID is @ContainerID;";
                cmd.Parameters.AddWithValue("@ContainerID", containerID);
                using SQLiteDataReader reader = cmd.ExecuteReader();
                reader.Read();
                res[0] = reader.GetInt32(0);
                res[1] = reader.GetInt32(1);
            });
            return res;
        }
        public static List<int> GetContainerSize(SyncronizedSQLiteConnection connection, int containerID)
        {
            List<int> res = new List<int>(2);
            connection.Read(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();
                cmd.CommandText = @"SELECT Container.Width, Container.Height
                                    FROM  Container
                                    WHERE Container.ID is @ContainerID;";
                cmd.Parameters.AddWithValue("@ContainerID", containerID);
                using SQLiteDataReader reader = cmd.ExecuteReader();
                reader.Read();
                res[0] = reader.GetInt32(0);
                res[1] = reader.GetInt32(1);
            });
            return res;
        }
        #endregion

        #region Object
        // create object
        public static int CreateObject(SyncronizedSQLiteConnection connection, string link)
        {
            int res = -1;
            connection.Write(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();

                cmd.CommandText = @"INSERT INTO Object (Link_To_Object) 
                                            VALUES (@Link);
                                    SELECT last_insert_rowid();";

                cmd.Parameters.AddWithValue("@Link", link);
                using SQLiteDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                    res = reader.GetInt32(0);
            });
            return res;
        }
        // delete object
        public static bool DeleteContainerObject(SyncronizedSQLiteConnection connection, int objectID)
        {
            int result = -1;
            connection.Write(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();

                cmd.CommandText = @"DELETE FROM Object
                                    WHERE ID is @ObjectID;";
                cmd.Parameters.AddWithValue("@ObjectID", objectID);
                result = cmd.ExecuteNonQuery();
            });
            return result > 0;
        }
        //set object parent
        public static bool SetObjectParent(SyncronizedSQLiteConnection connection, int objectID, int parentID)
        {
            int result = -1;
            connection.Write(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();

                cmd.CommandText = @"INSERT INTO Set_Container_Objects (ID_Container, ID_Object) 
                                    VALUES (@ContainerID, @ObjectID);";

                cmd.Parameters.AddWithValue("@ParentID", parentID);
                cmd.Parameters.AddWithValue("@ObjectID", objectID);
                result = cmd.ExecuteNonQuery();
            });
            return result > 0;
        }
        //set new link
        public static bool SetObjectLink(SyncronizedSQLiteConnection connection, int objectID, string link)
        {
            int result = -1;
            connection.Write(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();

                cmd.CommandText = @"UPDATE Object
                                    SET Link_To_Object = @Link
                                    WHERE Object.ID is @ObjectID;";

                cmd.Parameters.AddWithValue("@Link", link);
                cmd.Parameters.AddWithValue("@ObjectID", objectID);
                result = cmd.ExecuteNonQuery();
            });
            return result > 0;
        }

        //get all content
        public static List<string> GetObjectContent(SyncronizedSQLiteConnection connection, int objectID)
        {
            List<string> res = new List<string>();
            connection.Read(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();
                cmd.CommandText = @"SELECT Link_To_Object
                                    FROM  Object
                                    WHERE Object.ID is @ObjectID;";
                cmd.Parameters.AddWithValue("@ObjectID", objectID);
                using SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                    res.Add(reader.GetString(0));
            });
            return res;
        }

        //get content
        public static string GetObjectLink(SyncronizedSQLiteConnection connection, int objectID)
        {
            string res = null;
            connection.Read(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();
                cmd.CommandText = @"SELECT Link_To_Object
                                    FROM  Object
                                    WHERE Object.ID is @ObjectID;";
                cmd.Parameters.AddWithValue("@ObjectID", objectID);
                using SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                    res = reader.GetString(0);
            });
            return res;
        }
        //has item
        public static bool ObjectHasLink(SyncronizedSQLiteConnection connection, int objectID, string link)
        {
            int result = -1;
            connection.Read(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();
                cmd.CommandText = @"SELECT object.ID
                                    FROM object
                                    WHERE object.Link_to_Object IS @Link
                                    AND object.ID is @ObjectID;";
                cmd.Parameters.AddWithValue("@Link", link);
                cmd.Parameters.AddWithValue("@ObjectID", objectID);
                using SQLiteDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                    result = reader.GetInt32(0);
            });
            return result > 0;
        }
        #endregion
    }

}
