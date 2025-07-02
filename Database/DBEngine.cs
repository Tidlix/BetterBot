using System.Data;
using DSharpPlus.Entities;
using Npgsql;
using BetterBot.Configuration;

namespace BetterBot.Database {
    public class DBEngine {
        private static string connString = "errorConnString";
        private static NpgsqlConnection connection = new NpgsqlConnection();

        public static async ValueTask createDB() {
            connString = $"Host={Config.Psql_host};Database={Config.Psql_database};Username={Config.Psql_username};Password={Config.Psql_password};";

            await createConnection();
            await createTables();
        }

        private static async ValueTask createTables() {
            var cmd = new NpgsqlCommand(@"CREATE SCHEMA IF NOT EXISTS data;CREATE TABLE IF NOT EXISTS data.notes (userid BIGINT,title VARCHAR,value VARCHAR(2000));", connection);
            await cmd.ExecuteNonQueryAsync();
        }

        private static async ValueTask createConnection() {
            connection = new NpgsqlConnection(connString);
            await connection.OpenAsync();
        }

        #region Notes
        public static async ValueTask<List<DiscordAutoCompleteChoice>> getNoteChoices(ulong userid, string input) {
            if (connection.State == ConnectionState.Closed) await createConnection();

            input ??= "";
            using var cmd = new NpgsqlCommand("SELECT title FROM data.notes WHERE userid = @userid AND title LIKE @input ORDER BY title", connection);
            cmd.Parameters.AddWithValue("@userid", (long)userid);
            cmd.Parameters.AddWithValue("@input", $"%{input}%");

            var list = new List<DiscordAutoCompleteChoice>();
            using var reader = await cmd.ExecuteReaderAsync();


            while (await reader.ReadAsync()) {
                list.Add(new DiscordAutoCompleteChoice(reader.GetString(0), reader.GetString(0)));
            }

            return list;
        }

        public static async ValueTask<bool> noteExists(ulong userid, string title) {
            string value = await getNoteValue(userid, title);
            return value != "";
        }

        public static async ValueTask<string> getNoteValue(ulong userid, string title) {
            if (connection.State == ConnectionState.Closed) await createConnection();

            using var cmd = new NpgsqlCommand("SELECT value FROM data.notes WHERE userid = @userid AND title = @title", connection);
            cmd.Parameters.AddWithValue("@userid", (long)userid);
            cmd.Parameters.AddWithValue("@title", title);

            var value = "";
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync()) {
                value = reader.GetString(0);
            }

            return value;
        }

        public static async ValueTask createNote(ulong userid, string title, string value) {
            if (connection.State == ConnectionState.Closed) await createConnection();

            using var cmd = new NpgsqlCommand();
            cmd.Connection = connection;

            var parameters = new List<string>();
            var columns = new List<string> { "userid", "title", "value" };
            var values = new List<object> { (long)userid, title, value };

            for (int i = 0; i < columns.Count; i++) {
                string paramName = $"@v{i}";
                parameters.Add(paramName);
                cmd.Parameters.AddWithValue(paramName, values[i]);
            }

            cmd.CommandText = $"INSERT INTO data.notes ({string.Join(", ", columns)}) SELECT {string.Join(", ", parameters)} WHERE NOT EXISTS (SELECT 1 FROM data.notes WHERE userid = @v0 AND title = @v1)";

            await cmd.ExecuteNonQueryAsync();
        }

        public static async ValueTask modifyNote(ulong userid, string title, string value) {
            if (connection.State == ConnectionState.Closed) await createConnection();

            using var cmd = new NpgsqlCommand("UPDATE data.notes SET value = @value WHERE userid = @userid AND title = @title", connection);
            cmd.Parameters.AddWithValue("@userid", (long)userid);
            cmd.Parameters.AddWithValue("@title", title);
            cmd.Parameters.AddWithValue("@value", value);
            await cmd.ExecuteNonQueryAsync();
        }

        public static async ValueTask deleteNote(ulong userid, string title) {
            if (connection.State == ConnectionState.Closed) await createConnection();

            using var cmd = new NpgsqlCommand("DELETE FROM data.notes WHERE userid = @userid AND title = @title", connection);
            cmd.Parameters.AddWithValue("@userid", (long)userid);
            cmd.Parameters.AddWithValue("@title", title);
            await cmd.ExecuteNonQueryAsync();
        }
        #endregion
    }
}
