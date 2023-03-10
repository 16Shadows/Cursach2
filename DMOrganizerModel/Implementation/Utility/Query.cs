using System.Data.SQLite;
using System.Collections.Generic;
using System.Xml.Linq;
using Windows.ApplicationModel;
using DMOrganizerModel.Implementation.Items;

namespace DMOrganizerModel.Implementation.Utility
{
    internal static class Query
    {
        #region Section
        public static bool HasDuplicateNameInSection(SyncronizedSQLiteConnection connection, string name, int sectionID)
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

        public static string GetSectionName(SyncronizedSQLiteConnection connection, int sectionID)
        {
            string res = null;
            connection.Read(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();
                cmd.CommandText = $"(SELECT Title FROM Section WHERE ID={sectionID})";
                using SQLiteDataReader reader = cmd.ExecuteReader();
            });
            return res;
        }

        public static bool SetSectionParent(SyncronizedSQLiteConnection connection, int sectionID, int? parentID)
        {
            bool success = false;
            connection.Write(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();
                if (parentID.HasValue)
                    cmd.CommandText = $"UPDATE Category SET Parent={parentID} WHERE ID={sectionID}";
                else
                    cmd.CommandText = $"UPDATE Category SET Parent=NULL WHERE ID={sectionID}";
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
        #endregion

        #region Category
        public static bool HasDuplicateNameInCategory(SyncronizedSQLiteConnection connection, string name, int categoryID)
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
        
        public static int CreateDocument(SyncronizedSQLiteConnection connecton, string name, int parentID)
        {
            int res = -1;
            connecton.Write(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();
                cmd.CommandText = $"INSERT INTO Section (Title) VALUES ($title);" +
                                  $"INSERT INTO Document (CategoryID, SectionID) VALUES ({parentID}, last_insert_rowid());" +
                                  $"SELECT SectionID FROM Document WHERE rowid=last_insert_rowid();";
                cmd.Parameters.AddWithValue("$title", name);
                res = (int)cmd.ExecuteScalar();
            });
            return res;
        }

        public static int CreateCategory(SyncronizedSQLiteConnection connection, string name, int parentID)
        {
            int res = -1;
            connection.Write(con =>
            {
                using SQLiteCommand cmd = con.CreateCommand();
                cmd.CommandText = $"INSERT INTO Category (Title, Parent) VALUES ($title, {parentID});" +
                                  $"SELECT last_insert_rowid();";
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
