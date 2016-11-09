using System;
using System.Data.SQLite;
using MangaChecker.Common;

namespace MangaChecker.Database {
	internal class SqliteUpdateSettings {
		public SqliteUpdateSettings(string site, string active) {
			try {
				var mDbConnection = new SQLiteConnection("Data Source=MangaDB.sqlite;Version=3;");
				string sql;
				mDbConnection.Open();
				if (site == "batoto_rss") {
					sql =
						$"UPDATE settings SET link = '{active}', added = (datetime()) WHERE name = '{site}'";
					new SQLiteCommand(sql, mDbConnection).ExecuteNonQuery();
					mDbConnection.Close();
					return;
				}

				sql =
					$"UPDATE settings SET active = {int.Parse(active)}, added = (datetime()) WHERE name = '{site}'";
				new SQLiteCommand(sql, mDbConnection).ExecuteNonQuery();
				mDbConnection.Close();
			} catch (Exception e) {
				DebugText.Write(e.Message);
			}
		}
	}
}