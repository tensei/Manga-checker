using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MangaChecker.Common;
using MangaChecker.Models;

namespace MangaChecker.Database {
    class SqliteGetMangasNotRead {
        public List<MangaModel> mangas = new List<MangaModel>();

        public SqliteGetMangasNotRead() {
            try {
                using(var mDbConnection = new SQLiteConnection("Data Source=MangaDB.sqlite;Version=3;")) {
                    mDbConnection.Open();
                    var sql = $"SELECT * FROM mangasnotread";
                    using(var command = new SQLiteCommand(sql, mDbConnection)) {
                        using(var reader = command.ExecuteReader()) {
                            while(reader.Read()) {
                                mangas.Add(new MangaModel {
                                    Id = reader.GetInt32(0),
                                    Name = reader["name"].ToString(),
                                    Chapter = reader["chapter"].ToString(),
                                    Site = reader["site"].ToString(),
                                    Link = reader["link"].ToString(),
                                    RssLink = reader["rss_url"].ToString(),
                                    Date = (DateTime)reader["last_update"]
                                });
                            }
                        }
                    }
                    mDbConnection.Close();
                }
            } catch(Exception e) {
                DebugText.Write(e.Message);
            }
    }
    }
}
