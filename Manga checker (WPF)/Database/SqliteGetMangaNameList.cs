using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MangaChecker.Common;

namespace MangaChecker.Database {
    public class SqliteGetMangaNameList {
        public List<string> List;
        public SqliteGetMangaNameList(string site) {
            var mangas = new List<string>();
            try {
                using(var mDbConnection = new SQLiteConnection("Data Source=MangaDB.sqlite;Version=3;")) {
                    mDbConnection.Open();
                    var sql = $"SELECT name FROM {site.ToLower()}";
                    using(var command = new SQLiteCommand(sql, mDbConnection)) {
                        using(var reader = command.ExecuteReader()) {
                            while(reader.Read()) {
                                mangas.Add(reader["name"].ToString());
                            }
                        }
                    }

                    mDbConnection.Close();
                }
            } catch(Exception e) {
                DebugText.Write(e.Message);
            }

            List = mangas;
        }
    }
}
