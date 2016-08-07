using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MangaChecker.Common;
using MangaChecker.Models;

namespace MangaChecker.Database {
    class SqliteDeleteNotReadManga {

        public SqliteDeleteNotReadManga(MangaModel item) {
            try {
                var mDbConnection = new SQLiteConnection("Data Source=MangaDB.sqlite;Version=3;");
                mDbConnection.Open();
                var sql =
                    $"DELETE FROM mangasnotread WHERE name = '{item.Name}' AND chapter = '{item.Chapter}'";
                var command = new SQLiteCommand(sql, mDbConnection);
                command.ExecuteNonQuery();
                DebugText.Write($"{mDbConnection.Changes} rows affected ");
                mDbConnection.Close();
            } catch(Exception e) {
                DebugText.Write(e.Message);
            }
        }
    }
}
