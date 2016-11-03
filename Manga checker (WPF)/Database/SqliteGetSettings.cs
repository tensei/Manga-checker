using System;
using System.Collections.Generic;
using System.Data.SQLite;
using MangaChecker.Common;

namespace MangaChecker.Database {
    internal class SqliteGetSettings {
        public Dictionary<string, string> Settings;

        public SqliteGetSettings() {
            var settings = new Dictionary<string, string>();
            try {
                using (var mDbConnection = new SQLiteConnection("Data Source=MangaDB.sqlite;Version=3;")) {
                    mDbConnection.Open();
                    var sql = $"SELECT * FROM settings";
                    using (var command = new SQLiteCommand(sql, mDbConnection)) {
                        using (var reader = command.ExecuteReader()) {
                            while (reader.Read()) {
                                if (reader["name"].ToString() == "batoto_rss") {
                                    settings[reader["name"].ToString()] = reader["link"].ToString();
                                } else {
                                    settings[reader["name"].ToString()] = reader["active"].ToString();
                                }
                            }
                        }
                    }

                    mDbConnection.Close();
                }
            } catch (Exception e) {
                DebugText.Write(e.Message);
            }

            Settings = settings;
        }
    }
}