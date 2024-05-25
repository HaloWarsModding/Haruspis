using System.Data.SQLite;

namespace Data
{
    public class ModsDatabase
    {
        #region Properties
        public DBValues Data { get; set; } = new DBValues();

        public class DBValues
        {
            public string Name { get; set; }
            public string Version { get; set; }
            public string Description { get; set; }
            public string NexusModLink { get; set; }
            public long CurrentPlayTime { get; set; } = 0;
            public DateTime LastPlayedDate { get; set; } = DateTime.MinValue;
        }
        #endregion

        private const string ConnectionString = "Data Source=Haruspis.db;Version=3;";

        public ModsDatabase()
        {
            CreateDatabase();
        }

        private void CreateDatabase()
        {
            using SQLiteConnection connection = new(ConnectionString);
            connection.Open();

            string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS ModInfo (
                    Name TEXT NOT NULL,
                    Version TEXT NOT NULL,
                    Description TEXT,
                    NexusModLink TEXT,
                    PlayTimeInSeconds INTEGER,
                    LastPlayed TEXT,
                    PRIMARY KEY(Name, Version)
                )";

            using SQLiteCommand command = new(createTableQuery, connection);
            _ = command.ExecuteNonQuery();
        }

        public void SaveData()
        {
            using SQLiteConnection connection = new(ConnectionString);
            connection.Open();

            string insertQuery = @"
                INSERT OR REPLACE INTO ModInfo (Name, Version, Description, NexusModLink, PlayTimeInSeconds, LastPlayed)
                VALUES (@Name, @Version, @Description, @NexusModLink, @PlayTimeInSeconds, @LastPlayed)";

            using SQLiteCommand command = new(insertQuery, connection);
            _ = command.Parameters.AddWithValue("@Name", Data.Name);
            _ = command.Parameters.AddWithValue("@Version", Data.Version);
            _ = command.Parameters.AddWithValue("@Description", Data.Description);
            _ = command.Parameters.AddWithValue("@NexusModLink", (object)Data.NexusModLink ?? DBNull.Value);
            _ = command.Parameters.AddWithValue("@PlayTimeInSeconds", Data.CurrentPlayTime);
            _ = command.Parameters.AddWithValue("@LastPlayed", Data.LastPlayedDate.ToString("o"));

            _ = command.ExecuteNonQuery();
        }

        public void LoadData(string name, string version)
        {
            using SQLiteConnection connection = new(ConnectionString);
            connection.Open();

            string selectQuery = @"
                SELECT Name, Version, Description, NexusModLink, PlayTimeInSeconds, LastPlayed
                FROM ModInfo
                WHERE Name = @Name AND Version = @Version";

            using SQLiteCommand command = new(selectQuery, connection);
            _ = command.Parameters.AddWithValue("@Name", name);
            _ = command.Parameters.AddWithValue("@Version", version);

            using SQLiteDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                Data.Name = reader["Name"].ToString();
                Data.Version = reader["Version"].ToString();
                Data.Description = reader["Description"].ToString();
                Data.NexusModLink = reader["NexusModLink"] as string;
                Data.CurrentPlayTime = Convert.ToInt64(reader["PlayTimeInSeconds"]);
                Data.LastPlayedDate = DateTime.Parse(reader["LastPlayed"].ToString());
            }
            else
            {
                // Initialize with default values if not found
                Data.Name = name;
                Data.Version = version;
                Data.Description = string.Empty;
                Data.NexusModLink = null;
                Data.CurrentPlayTime = 0;
                Data.LastPlayedDate = DateTime.MinValue;
            }
        }

        public void UpdateDescription(string name, string version, string newDescription)
        {
            UpdateField(name, version, "Description", newDescription);
        }

        public void UpdateNexusModLink(string name, string version, string newNexusModLink)
        {
            UpdateField(name, version, "NexusModLink", newNexusModLink);
        }

        public void UpdatePlayTime(string name, string version, long newPlayTimeInSeconds)
        {
            UpdateField(name, version, "PlayTimeInSeconds", newPlayTimeInSeconds.ToString());
        }

        public void UpdateLastPlayed(string name, string version, DateTime newLastPlayed)
        {
            UpdateField(name, version, "LastPlayed", newLastPlayed.ToString("o"));
        }

        public void UpdateAllFields(string name, string version, string newDescription, string newNexusModLink, long newPlayTimeInSeconds, DateTime newLastPlayed)
        {
            using SQLiteConnection connection = new(ConnectionString);
            connection.Open();

            string updateQuery = @"
                UPDATE ModInfo
                SET Description = @Description, NexusModLink = @NexusModLink, PlayTimeInSeconds = @PlayTimeInSeconds, LastPlayed = @LastPlayed
                WHERE Name = @Name AND Version = @Version";

            using SQLiteCommand command = new(updateQuery, connection);
            _ = command.Parameters.AddWithValue("@Name", name);
            _ = command.Parameters.AddWithValue("@Version", version);
            _ = command.Parameters.AddWithValue("@Description", newDescription);
            _ = command.Parameters.AddWithValue("@NexusModLink", (object)newNexusModLink ?? DBNull.Value);
            _ = command.Parameters.AddWithValue("@PlayTimeInSeconds", newPlayTimeInSeconds);
            _ = command.Parameters.AddWithValue("@LastPlayed", newLastPlayed.ToString("o"));

            _ = command.ExecuteNonQuery();
        }

        private void UpdateField(string name, string version, string fieldName, string newValue)
        {
            using SQLiteConnection connection = new(ConnectionString);
            connection.Open();

            string updateQuery = $"UPDATE ModInfo SET {fieldName} = @NewValue WHERE Name = @Name AND Version = @Version";

            using SQLiteCommand command = new(updateQuery, connection);
            _ = command.Parameters.AddWithValue("@NewValue", (object)newValue ?? DBNull.Value);
            _ = command.Parameters.AddWithValue("@Name", name);
            _ = command.Parameters.AddWithValue("@Version", version);

            _ = command.ExecuteNonQuery();
        }
    }
}
