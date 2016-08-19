using System;
using System.Data.SQLite;
using MangaChecker.Common;
using MangaChecker.Models;

namespace MangaChecker.Database {
    internal class SqliteDeleteNotReadManga {
        public SqliteDeleteNotReadManga(MangaModel item) {
            try {
                var mDbConnection = new SQLiteConnection("Data Source=MangaDB.sqlite;Version=3;");
                mDbConnection.Open();
                var sql =
                    $"DELETE FROM mangasnotread WHERE name = '{item.Name.Replace("'", "''")}' AND chapter = '{item.Chapter}'";
                var command = new SQLiteCommand(sql, mDbConnection);
                command.ExecuteNonQuery();
                DebugText.Write($"{mDbConnection.Changes} rows affected ");
                mDbConnection.Close();
            } catch (Exception e) {
                DebugText.Write(e.Message);
            }
        }
    }
}