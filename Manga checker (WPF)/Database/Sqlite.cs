using System;
using System.Collections.Generic;
using System.Data.SQLite;
using Manga_checker.Classes;
using Manga_checker.Handlers;

namespace Manga_checker.Database {
    public class Sqlite {
        private static readonly List<string> Sites = new List<string> {
            "Mangafox",
            "Mangareader",
            "Mangastream",
            "Batoto",
            "Webtoons",
            "YoManga",
            "Kissmanga",
            "Backlog"
        };

        public static void test() {
            //
            //SetupDatabase();
            //PopulateDb();
            //UpdateManga("Mangafox", "akame ga kill", "64", "www.bitch.com");
            //GetMangas("mangafox");
            //DebugText.Write(GetMangaInfo("batoto", "Prison School").Name);
        }

        public static void PopulateDb() {
            try {
                foreach (var site in Sites) {
                    foreach (var manga in ParseFile.GetManga(site.ToLower())) {
                        var chna = manga.Split(new[] {"[]"}, StringSplitOptions.None);
                        AddManga(site.ToLower(), chna[0], chna[1], "placeholder");
                    }
                }
            }
            catch (Exception e) {
                DebugText.Write(e.Message);
            }
        }

        public static void SetupDatabase() {
            try {
                SQLiteConnection.CreateFile("MangaDB.sqlite");

                var mDbConnection = new SQLiteConnection("Data Source=MangaDB.sqlite;");
                mDbConnection.Open();

                foreach (var site in Sites) {
                    var sql =
                        $"create table {site.ToLower()} (name varchar(50) not null, chapter varchar(20) not null, last_update varchar(100), link varchar(200), rss_url varchar(200))";

                    var command = new SQLiteCommand(sql, mDbConnection);
                    command.ExecuteNonQuery();
                }
                mDbConnection.Close();
            }
            catch (Exception e) {
                DebugText.Write(e.Message);
            }
        }

        public static void AddManga(string site, string name, string chapter, string rss) {
            try {
                var mDbConnection = new SQLiteConnection("Data Source=MangaDB.sqlite;Version=3;");
                mDbConnection.Open();
                var sql =
                    $"insert into {site} (name, chapter, last_update, link, rss_url) values ('{name.Replace("'", "''")}', '{chapter}', '{DateTime.Now}', 'placeholder', '{rss}')";
                var command = new SQLiteCommand(sql, mDbConnection);
                command.ExecuteNonQuery();
                mDbConnection.Close();
            }
            catch (Exception e) {
                DebugText.Write(e.Message);
            }
        }

        public static void DeleteManga(string site, string name, string chapter) {
            try {
                var mDbConnection = new SQLiteConnection("Data Source=MangaDB.sqlite;Version=3;");
                mDbConnection.Open();
                var sql =
                    $"DELETE FROM {site.ToLower()} WHERE chapter = '{chapter}' AND name = '{name.Replace("'", "''")}'";
                var command = new SQLiteCommand(sql, mDbConnection);
                command.ExecuteNonQuery();
                mDbConnection.Close();
            }
            catch (Exception e) {
                DebugText.Write(e.Message);
            }
        }

        public static void UpdateManga(string site, string name, string chapter, string link) {
            try {
                var mDbConnection = new SQLiteConnection("Data Source=MangaDB.sqlite;Version=3;");
                mDbConnection.Open();
                var sql =
                    $"UPDATE {site.ToLower()} SET chapter = '{chapter}', link = '{link}' WHERE name = '{name.Replace("'", "''")}'";
                var command = new SQLiteCommand(sql, mDbConnection);
                command.ExecuteNonQuery();
                mDbConnection.Close();
            }
            catch (Exception e) {
                DebugText.Write(e.Message);
            }
        }

        public static List<MangaInfoViewModel> GetMangas(string site) {
            var mangas = new List<MangaInfoViewModel>();
            try {
                using (var mDbConnection = new SQLiteConnection("Data Source=MangaDB.sqlite;Version=3;")) {
                    mDbConnection.Open();
                    var sql = $"SELECT * FROM {site.ToLower()}";
                    using (var command = new SQLiteCommand(sql, mDbConnection)) {
                        using (var reader = command.ExecuteReader()) {
                            while (reader.Read()) {
                                mangas.Add(new MangaInfoViewModel {
                                    Name = reader["name"].ToString(),
                                    Chapter = reader["chapter"].ToString(),
                                    Site = site,
                                    Link = reader["link"].ToString(),
                                    RSS_Link = reader["rss_url"].ToString(),
                                    Date = reader["last_update"].ToString()
                                });
                            }
                        }
                    }
                    mDbConnection.Close();
                }
            }
            catch (Exception e) {
                DebugText.Write(e.Message);
            }

            return mangas;
        }

        public static MangaInfoViewModel GetMangaInfo(string site, string name) {
            try {
                MangaInfoViewModel m;
                using (var mDbConnection = new SQLiteConnection("Data Source=MangaDB.sqlite;Version=3;")) {
                    mDbConnection.Open();
                    var sql = $"SELECT * FROM {site.ToLower()} WHERE name = '{name}'";
                    using (var command = new SQLiteCommand(sql, mDbConnection)) {
                        using (var reader = command.ExecuteReader()) {
                            while (reader.Read()) {
                                m = new MangaInfoViewModel {
                                    Name = reader["name"].ToString(),
                                    Chapter = reader["chapter"].ToString(),
                                    Site = site,
                                    Link = reader["link"].ToString(),
                                    RSS_Link = reader["rss_url"].ToString(),
                                    Date = reader["last_update"].ToString()
                                };
                                mDbConnection.Close();
                                return m;
                            }
                        }
                    }
                }
                return m = new MangaInfoViewModel();
            }
            catch (Exception e) {
                DebugText.Write(e.Message);
                return new MangaInfoViewModel();
            }
        }

        public static List<string> GetMangaNameList(string site) {
            var mangas = new List<string>();
            try {
                using (var mDbConnection = new SQLiteConnection("Data Source=MangaDB.sqlite;Version=3;")) {
                    mDbConnection.Open();
                    var sql = $"SELECT name FROM {site.ToLower()}";
                    using (var command = new SQLiteCommand(sql, mDbConnection)) {
                        using (var reader = command.ExecuteReader()) {
                            while (reader.Read()) {
                                mangas.Add(reader["name"].ToString());
                            }
                        }
                    }
                    mDbConnection.Close();
                }
            }
            catch (Exception e) {
                DebugText.Write(e.Message);
            }
            return mangas;
        }
    }
}