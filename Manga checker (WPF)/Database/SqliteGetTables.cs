using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MangaChecker.Common;

namespace MangaChecker.Database {
    class SqliteGetTables {
        public List<string> Tables;
        public SqliteGetTables() {
            var tables = new List<string>();
            try {
                using(var mDbConnection = new SQLiteConnection("Data Source=MangaDB.sqlite;Version=3;")) {
                    mDbConnection.Open();

                    // var sql = "SELECT name FROM sqlite_sequence";
                    var sql = "SELECT name FROM sqlite_master WHERE type = 'table'";
                    using(var command = new SQLiteCommand(sql, mDbConnection)) {
                        using(var reader = command.ExecuteReader()) {
                            while(reader.Read()) {
                                tables.Add(reader.GetString(0));
                            }
                        }
                    }

                    mDbConnection.Close();
                }
            } catch(Exception e) {
                DebugText.Write(e.Message);
            }
            Tables = tables;
        }
    }
}
