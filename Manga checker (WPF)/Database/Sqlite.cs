using System;
using System.Collections.Generic;
using System.Data.SQLite;
using Manga_checker.Handlers;
using Manga_checker.ViewModels;

namespace Manga_checker.Database {
    public class Sqlite {
        private static readonly Dictionary<string, string> Sites = new Dictionary<string, string> {
            {"Mangafox", "http://mangafox.me/"},
            {"Mangareader", "http://www.mangareader.net/"},
            {"Mangastream", "http://mangastream.com/"},
            {"Batoto", "http://bato.to/"},
            {"Webtoons", "http://www.webtoons.com/"},
            {"YoManga", "http://yomanga.co/"},
            {"Kissmanga", "http://kissmanga.com/"},
            {"Backlog", "/"}
        };


        private static string[] _otherTables = {"link_collections", "manga_tables"};

        //public static void test() {
        //
        //SetupDatabase();
        //PopulateDb();
        //UpdateManga("Mangafox", "akame ga kill", "64", "www.bitch.com");
        //GetMangas("mangafox");
        //DebugText.Write(GetMangaInfo("batoto", "Prison School").Name);
        //}

        public static void PopulateDb() {
            try {
                foreach (var site in Sites) {
                    foreach (var manga in ParseFile.GetManga(site.Key.ToLower())) {
                        var chna = manga.Split(new[] {"[]"}, StringSplitOptions.None);
                        AddManga(site.Key.ToLower(), chna[0], chna[1], chna.Length.Equals(2) ? "placeholder" : chna[2]);
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
                        $@"CREATE TABLE '{site.Key.ToLower()}' (
	                        'id'	INTEGER PRIMARY KEY AUTOINCREMENT,
	                        'name'	varchar(200) NOT NULL,
	                        'chapter'	varchar(200) NOT NULL,
	                        'last_update'	datetime NOT NULL DEFAULT (datetime()),
	                        'link'	varchar(200),
	                        'rss_url'	varchar(200)
                        );";

                    //var sql =
                    //$"create table {site.ToLower()} (name varchar(50) not null, chapter varchar(20) not null, last_update varchar(100), link varchar(200), rss_url varchar(200))";

                    var command = new SQLiteCommand(sql, mDbConnection);
                    command.ExecuteNonQuery();
                }
                new SQLiteCommand(@"
                        CREATE TABLE 'manga_tables' (
	                        'id'	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	                        'name'	varchar(200) NOT NULL,
	                        'link'	varchar(200) NOT NULL,
	                        'active' INTEGER NOT NULL DEFAULT (0),
	                        'added'	datetime NOT NULL DEFAULT (datetime())
                        );", mDbConnection).ExecuteNonQuery();

                foreach (var keyValuePair in Sites) {
                    new SQLiteCommand(
                        $"INSERT INTO manga_tables (name, link, active) VALUES ('{keyValuePair.Key.ToLower()}', '{keyValuePair.Value}', 0)",
                        mDbConnection).ExecuteNonQuery();
                }

                new SQLiteCommand(
                    @"CREATE TABLE 'link_collection' (
	                        'id'	INTEGER PRIMARY KEY AUTOINCREMENT,
	                        'name'	varchar(200) NOT NULL,
	                        'site'	varchar(200) NOT NULL,
	                        'chapter'	varchar(200) NOT NULL,
	                        'added'	datetime NOT NULL DEFAULT (datetime()),
	                        'link'	varchar(200) NOT NULL
                        );",
                    mDbConnection).ExecuteNonQuery();

                DebugText.Write($"{mDbConnection.Changes} rows affected ");
                mDbConnection.Close();
            }
            catch (Exception e) {
                DebugText.Write(e.Message);
            }
            GetAllTables();
        }

        public static void AddManga(string site, string name, string chapter, string rss) {
            try {
                var mDbConnection = new SQLiteConnection("Data Source=MangaDB.sqlite;Version=3;");
                mDbConnection.Open();
                var sql =
                    $"insert into {site} (name, chapter, last_update, link, rss_url) values ('{name.Replace("'", "''")}', '{chapter}', (datetime()), 'placeholder', '{rss}')";
                var command = new SQLiteCommand(sql, mDbConnection);
                command.ExecuteNonQuery();
                DebugText.Write($"{mDbConnection.Changes} rows affected ");
                mDbConnection.Close();
            }
            catch (Exception e) {
                DebugText.Write(e.Message);
            }
        }

        public static void DeleteManga(MangaViewModel item) {
            try {
                var mDbConnection = new SQLiteConnection("Data Source=MangaDB.sqlite;Version=3;");
                mDbConnection.Open();
                var sql =
                    $"DELETE FROM {item.Site.ToLower()} WHERE id = {item.Id}";
                var command = new SQLiteCommand(sql, mDbConnection);
                command.ExecuteNonQuery();
                DebugText.Write($"{mDbConnection.Changes} rows affected ");
                mDbConnection.Close();
            }
            catch (Exception e) {
                DebugText.Write(e.Message);
            }
        }

        public static void UpdateManga(string site, string name, string chapter, string link, bool linkcol = true) {
            try {
                name = name.Replace("'", "''");
                var mDbConnection = new SQLiteConnection("Data Source=MangaDB.sqlite;Version=3;");
                mDbConnection.Open();
                var sql =
                    $"UPDATE {site.ToLower()} SET chapter = '{chapter}', link = '{link}', last_update = (datetime()) WHERE name = '{name}'";
                new SQLiteCommand(sql, mDbConnection).ExecuteNonQuery();

                if (!site.ToLower().Equals("backlog") && linkcol) {
                    new SQLiteCommand(
                        $@"INSERT INTO link_collection (name, chapter, added, link, site) VALUES ('{name}', '{chapter}', (datetime()), '{link}', '{site
                            .ToLower()}')", mDbConnection).ExecuteNonQuery();
                }
                DebugText.Write($"{mDbConnection.Changes} rows affected ");
                mDbConnection.Close();
            }
            catch (Exception e) {
                DebugText.Write(e.Message);
            }
        }

        public static List<MangaViewModel> GetMangas(string site) {
            var mangas = new List<MangaViewModel>();
            try {
                using (var mDbConnection = new SQLiteConnection("Data Source=MangaDB.sqlite;Version=3;")) {
                    mDbConnection.Open();
                    var sql = $"SELECT * FROM {site.ToLower()}";
                    using (var command = new SQLiteCommand(sql, mDbConnection)) {
                        using (var reader = command.ExecuteReader()) {
                            while (reader.Read()) {
                                mangas.Add(new MangaViewModel {
                                    Id = reader.GetInt32(0),
                                    Name = reader["name"].ToString(),
                                    Chapter = reader["chapter"].ToString(),
                                    Site = site,
                                    Link = reader["link"].ToString(),
                                    RssLink = reader["rss_url"].ToString(),
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

        public static MangaViewModel GetMangaInfo(string site, string name) {
            try {
                MangaViewModel m;
                using (var mDbConnection = new SQLiteConnection("Data Source=MangaDB.sqlite;Version=3;")) {
                    mDbConnection.Open();
                    var sql = $"SELECT * FROM {site.ToLower()} WHERE name = '{name}'";
                    using (var command = new SQLiteCommand(sql, mDbConnection)) {
                        using (var reader = command.ExecuteReader()) {
                            while (reader.Read()) {
                                m = new MangaViewModel {
                                    Id = reader.GetInt32(0),
                                    Name = reader["name"].ToString(),
                                    Chapter = reader["chapter"].ToString(),
                                    Site = site,
                                    Link = reader["link"].ToString(),
                                    RssLink = reader["rss_url"].ToString(),
                                    Date = reader["last_update"].ToString()
                                };
                                mDbConnection.Close();
                                return m;
                            }
                        }
                    }
                }
                return m = new MangaViewModel();
            }
            catch (Exception e) {
                DebugText.Write(e.Message);
                return new MangaViewModel();
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

        public static List<string> GetAllTables() {
            var tables = new List<string>();
            try {
                using (var mDbConnection = new SQLiteConnection("Data Source=MangaDB.sqlite;Version=3;")) {
                    mDbConnection.Open();
                    //var sql = "SELECT name FROM sqlite_sequence";
                    var sql = "SELECT name FROM sqlite_master WHERE type = 'table'";
                    using (var command = new SQLiteCommand(sql, mDbConnection)) {
                        using (var reader = command.ExecuteReader()) {
                            while (reader.Read()) {
                                tables.Add(reader.GetString(0));
                            }
                        }
                    }
                    mDbConnection.Close();
                }
            }
            catch (Exception e) {
                DebugText.Write(e.Message);
            }
            //DebugText.Write($"tables\n{string.Join("\n", tables)}");
            return tables;
        }

        public static void UpdateDatabase() {
            var existingTables = GetAllTables();
            if (!existingTables.Contains("manga_tables")) {
                var mDbConnection = new SQLiteConnection("Data Source=MangaDB.sqlite;");
                mDbConnection.Open();
                new SQLiteCommand(@"
                        CREATE TABLE 'manga_tables' (
	                        'id'	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	                        'name'	varchar(200) NOT NULL,
	                        'link'	varchar(200) NOT NULL,
	                        'active' INTEGER NOT NULL DEFAULT (0),
	                        'added'	datetime NOT NULL DEFAULT (datetime())
                        );", mDbConnection).ExecuteNonQuery();
                DebugText.Write($"Added table manga_tables to Database");
            }

            foreach (var keyValuePair in Sites) {
                if (!existingTables.Contains(keyValuePair.Key.ToLower())) {
                    var mDbConnection = new SQLiteConnection("Data Source=MangaDB.sqlite;");
                    mDbConnection.Open();
                    var sql =
                        $@"CREATE TABLE '{keyValuePair.Key.ToLower()}' (
	                    'id'	INTEGER PRIMARY KEY AUTOINCREMENT,
	                    'name'	varchar(200) NOT NULL,
	                    'chapter'	varchar(200) NOT NULL,
	                    'last_update'	datetime NOT NULL DEFAULT (datetime()),
	                    'link'	varchar(200),
	                    'rss_url'	varchar(200)
                    );";

                    //var sql =
                    //$"create table {site.ToLower()} (name varchar(50) not null, chapter varchar(20) not null, last_update varchar(100), link varchar(200), rss_url varchar(200))";

                    new SQLiteCommand(sql, mDbConnection).ExecuteNonQuery();
                    new SQLiteCommand(
                        $"INSERT INTO manga_tables (name, link, active) VALUES ('{keyValuePair.Key.ToLower()}', '{keyValuePair.Value}', 0)",
                        mDbConnection).ExecuteNonQuery();
                    DebugText.Write($"Added table {keyValuePair.Key} to Database");
                    mDbConnection.Close();
                }
            }

            if (!existingTables.Contains("link_collection")) {
                var mDbConnection = new SQLiteConnection("Data Source=MangaDB.sqlite;");
                mDbConnection.Open();
                new SQLiteCommand(
                    @"CREATE TABLE 'link_collection' (
	                        'id'	INTEGER PRIMARY KEY AUTOINCREMENT,
	                        'name'	varchar(200) NOT NULL,
	                        'site'	varchar(200) NOT NULL,
	                        'chapter'	varchar(200) NOT NULL,
	                        'added'	datetime NOT NULL DEFAULT (datetime()),
	                        'link'	varchar(200) NOT NULL
                        );",
                    mDbConnection).ExecuteNonQuery();
                DebugText.Write($"Added table link_collections to Database");
            }
        }

        public static List<MangaViewModel> GetHistory() {
            var mangas = new List<MangaViewModel>();
            try {
                using (var mDbConnection = new SQLiteConnection("Data Source=MangaDB.sqlite;Version=3;")) {
                    mDbConnection.Open();
                    var sql = $"SELECT * FROM link_collection";
                    using (var command = new SQLiteCommand(sql, mDbConnection)) {
                        using (var reader = command.ExecuteReader()) {
                            while (reader.Read()) {
                                mangas.Add(new MangaViewModel {
                                    Id = reader.GetInt32(0),
                                    Name = reader["name"].ToString(),
                                    Chapter = reader["chapter"].ToString(),
                                    Site = reader["site"].ToString(),
                                    Link = reader["link"].ToString(),
                                    Date = reader["added"].ToString()
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
    }
}