using System.Data.SQLite;
using System.Collections.Generic;

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
    }
}
