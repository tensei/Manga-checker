using System;
using System.Collections.Generic;
using System.Data.SQLite;
using MangaChecker.Common;
using MangaChecker.Models;

namespace MangaChecker.Database {
	internal class SqliteGetMangas {
		public List<MangaModel> Mangas = new List<MangaModel>();

		public SqliteGetMangas(string site) {
			try {
				using (var mDbConnection = new SQLiteConnection("Data Source=MangaDB.sqlite;Version=3;")) {
					mDbConnection.Open();
					var sql = $"SELECT * FROM {site.ToLower()}";
					using (var command = new SQLiteCommand(sql, mDbConnection)) {
						using (var reader = command.ExecuteReader()) {
							while (reader.Read())
								Mangas.Add(new MangaModel {
									Id = reader.GetInt32(0),
									Name = reader["name"].ToString(),
									Chapter = reader["chapter"].ToString(),
									Site = site,
									Link = reader["link"].ToString(),
									RssLink = reader["rss_url"].ToString(),
									Date = (DateTime) reader["last_update"]
								});
						}
					}
					mDbConnection.Close();
				}
			} catch (Exception e) {
				DebugText.Write(e.Message);
			}
		}
	}
}