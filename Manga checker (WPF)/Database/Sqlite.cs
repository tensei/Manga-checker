using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Threading;
using System.Threading.Tasks;
using MangaChecker.Common;
using MangaChecker.Models;

namespace MangaChecker.Database {
    public class Sqlite {
        private static readonly Dictionary<string, string> Sites = GlobalVariables.SitesforDatabaseTables;


        private static string[] _otherTables = {"link_collections", "settings"};

        // public static void test() {
        // SetupDatabase();
        // PopulateDb();
        // UpdateManga("Mangafox", "akame ga kill", "64", "www.bitch.com");
        // GetMangas("mangafox");
        // DebugText.Write(GetMangaInfo("batoto", "Prison School").Name);
        // }

        public static void SetupDatabase() {
            try {
                SQLiteConnection.CreateFile("MangaDB.sqlite");

                var mDbConnection = new SQLiteConnection("Data Source=MangaDB.sqlite;");
                mDbConnection.Open();

                foreach (var site in Sites) {
                    var sql =
                        $@"CREATE TABLE '{site.Key.ToLower()
                            }' (
							'id'	INTEGER PRIMARY KEY AUTOINCREMENT,
							'name'	varchar(200) NOT NULL,
							'chapter'	varchar(200) NOT NULL,
							'last_update'	datetime NOT NULL DEFAULT (datetime()),
							'link'	varchar(200),
							'rss_url'	varchar(200)
						);";

                    // var sql =
                    // $"create table {site.ToLower()} (name varchar(50) not null, chapter varchar(20) not null, last_update varchar(100), link varchar(200), rss_url varchar(200))";
                    var command = new SQLiteCommand(sql, mDbConnection);
                    command.ExecuteNonQuery();
                }

                new SQLiteCommand(@"
						CREATE TABLE 'settings' (
							'id'	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
							'name'	varchar(200) NOT NULL,
							'link'	varchar(200),
							'active' INTEGER NOT NULL DEFAULT (0),
							'added'	datetime NOT NULL DEFAULT (datetime())
						);", mDbConnection).ExecuteNonQuery();

                foreach (var keyValuePair in Sites) {
                    new SQLiteCommand(
                        $"INSERT INTO settings (name, link, active) VALUES ('{keyValuePair.Key.ToLower()}', '{keyValuePair.Value}', 0)",
                        mDbConnection).ExecuteNonQuery();
                    Thread.Sleep(100);
                }

                new SQLiteCommand(
                    $"INSERT INTO settings (name, active) VALUES ('open links', 0)",
                    mDbConnection).ExecuteNonQuery();
                new SQLiteCommand(
                    $"INSERT INTO settings (name, active) VALUES ('show disabled', 1)",
                    mDbConnection).ExecuteNonQuery();
                new SQLiteCommand(
                    $"INSERT INTO settings (name, active) VALUES ('refresh time', 300)",
                    mDbConnection).ExecuteNonQuery();
                new SQLiteCommand(
                    $"INSERT INTO settings (name, link) VALUES ('batoto_rss', '/')",
                    mDbConnection).ExecuteNonQuery();

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
            } catch (Exception e) {
                DebugText.Write(e.Message);
            }
        }

        public static bool AddManga(MangaModel manga) {
            var add = new SqliteAddManga(manga);
            return add.Success;
        }

        public static void DeleteManga(MangaModel item) {
            var sqliteDeleteManga = new SqliteDeleteManga(item);
        }

        public static void DeleteNotReadManga(MangaModel item) {
            var sqliteDeleteManga = new SqliteDeleteNotReadManga(item);
        }

        public static void UpdateManga(MangaModel manga,
            bool linkcol = true) {
            var ud = new SqliteUpdateManga(manga);
        }

        public static List<MangaModel> GetMangas(string site) {
            var get = new SqliteGetMangas(site);
            return get.Mangas;
        }

        public static List<MangaModel> GetMangasNotRead() {
            var get = new SqliteGetMangasNotRead();
            return get.mangas;
        }

        public static async Task<List<MangaModel>> GetMangasAsync(string site) {
            var get = new SqliteGetMangasAsync();
            return await get.GetMangasAsync(site);
        }

        public static MangaModel GetMangaInfo(string site, string name) {
            try {
                MangaModel m;
                using (var mDbConnection = new SQLiteConnection("Data Source=MangaDB.sqlite;Version=3;")) {
                    mDbConnection.Open();
                    var sql = $"SELECT * FROM {site.ToLower()} WHERE name = '{name}'";
                    using (var command = new SQLiteCommand(sql, mDbConnection)) {
                        using (var reader = command.ExecuteReader()) {
                            while (reader.Read()) {
                                m = new MangaModel {
                                    Id = reader.GetInt32(0),
                                    Name = reader["name"].ToString(),
                                    Chapter = reader["chapter"].ToString(),
                                    Site = site,
                                    Link = reader["link"].ToString(),
                                    RssLink = reader["rss_url"].ToString(),
                                    Date = (DateTime) reader["last_update"]
                                };
                                mDbConnection.Close();
                                return m;
                            }
                        }
                    }
                }

                return m = new MangaModel();
            } catch (Exception e) {
                DebugText.Write(e.Message);
                return new MangaModel();
            }
        }

        public static List<string> GetMangaNameList(string site) {
            var mangas = new SqliteGetMangaNameList(site);
            return mangas.List;
        }

        public static List<string> GetAllTables() {
            var tables = new SqliteGetTables();
            return tables.Tables;
        }

        public static void UpdateDatabase() {
            var existingTables = GetAllTables();
            Thread.Sleep(100);
            if (!existingTables.Contains("settings")) {
                var mDbConnection = new SQLiteConnection("Data Source=MangaDB.sqlite;");
                mDbConnection.Open();
                new SQLiteCommand(@"
						CREATE TABLE 'settings' (
							'id'	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
							'name'	varchar(200) NOT NULL,
							'link'	varchar(200),
							'active' INTEGER NOT NULL DEFAULT (0),
							'added'	datetime NOT NULL DEFAULT (datetime())
						);", mDbConnection).ExecuteNonQuery();
                DebugText.Write($"Added table settings to Database");
                new SQLiteCommand(
                    $"INSERT INTO settings (name, link, active) VALUES ('open links', '/',  0)",
                    mDbConnection).ExecuteNonQuery();
                new SQLiteCommand(
                    $"INSERT INTO settings (name, link, active) VALUES ('show disabled', '/',  1)",
                    mDbConnection).ExecuteNonQuery();
                new SQLiteCommand(
                    $"INSERT INTO settings (name, link, active) VALUES ('refresh time', '/', 300)",
                    mDbConnection).ExecuteNonQuery();
                new SQLiteCommand(
                    $"INSERT INTO settings (name, active, link) VALUES ('batoto_rss', 300, '/')",
                    mDbConnection).ExecuteNonQuery();
            }

            foreach (var keyValuePair in Sites) {
                if (!existingTables.Contains(keyValuePair.Key.ToLower())) {
                    var mDbConnection = new SQLiteConnection("Data Source=MangaDB.sqlite;");
                    mDbConnection.Open();
                    var sql =
                        $@"CREATE TABLE '{keyValuePair.Key.ToLower()
                            }' (
						'id'	INTEGER PRIMARY KEY AUTOINCREMENT,
						'name'	varchar(200) NOT NULL,
						'chapter'	varchar(200) NOT NULL,
						'last_update'	datetime NOT NULL DEFAULT (datetime()),
						'link'	varchar(200),
						'rss_url'	varchar(200)
					);";

                    // var sql =
                    // $"create table {site.ToLower()} (name varchar(50) not null, chapter varchar(20) not null, last_update varchar(100), link varchar(200), rss_url varchar(200))";
                    new SQLiteCommand(sql, mDbConnection).ExecuteNonQuery();
                    new SQLiteCommand(
                        $"INSERT INTO settings (name, link, active) VALUES ('{keyValuePair.Key.ToLower()}', '{keyValuePair.Value}', 0)",
                        mDbConnection).ExecuteNonQuery();
                    DebugText.Write($"Added table {keyValuePair.Key} to Database");
                    mDbConnection.Close();
                }

                Thread.Sleep(100);
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
            if (!existingTables.Contains("mangasnotread")) {
                var mDbConnection = new SQLiteConnection("Data Source=MangaDB.sqlite;");
                mDbConnection.Open();
                new SQLiteCommand(
                    @"CREATE TABLE 'mangasnotread' (
							'id'	INTEGER PRIMARY KEY AUTOINCREMENT,
							'name'	varchar(200) NOT NULL,
							'site'	varchar(200) NOT NULL,
							'chapter'	varchar(200) NOT NULL,
							'last_update'	datetime NOT NULL DEFAULT (datetime()),
							'link'	varchar(200) NOT NULL,
							'rss_url'  varchar(200)
						);",
                    mDbConnection).ExecuteNonQuery();
                DebugText.Write($"Added table mangasnotread to Database");
            }
        }

        public static List<MangaModel> GetHistory() {
            var mangas = new List<MangaModel>();
            try {
                using (var mDbConnection = new SQLiteConnection("Data Source=MangaDB.sqlite;Version=3;")) {
                    mDbConnection.Open();
                    var sql = $"SELECT * FROM link_collection";
                    using (var command = new SQLiteCommand(sql, mDbConnection)) {
                        using (var reader = command.ExecuteReader()) {
                            while (reader.Read()) {
                                mangas.Add(new MangaModel {
                                    Id = reader.GetInt32(0),
                                    Name = reader["name"].ToString(),
                                    Chapter = reader["chapter"].ToString(),
                                    Site = reader["site"].ToString(),
                                    Link = reader["link"].ToString(),
                                    Date = (DateTime) reader["added"]
                                });
                            }
                        }
                    }

                    mDbConnection.Close();
                }
            } catch (Exception e) {
                DebugText.Write(e.Message);
            }
            return mangas;
        }

        public static Dictionary<string, string> GetSettings() {
            var settings = new SqliteGetSettings();
            return settings.Settings;
        }


        public static void UpdateSetting(string site, string active) {
            var sqliteUpdateSettings = new SqliteUpdateSettings(site, active);
        }
    }
}