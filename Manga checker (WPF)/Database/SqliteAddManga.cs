using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MangaChecker.Common;
using MangaChecker.Models;

namespace MangaChecker.Database {
    public class SqliteAddManga {
        public readonly bool Success;
        public SqliteAddManga(MangaModel manga) {
            var mangas = new SqliteGetMangaNameList(manga.Site).List;
            mangas = mangas.ConvertAll(i => i.ToLower());

            if(mangas.Contains(manga.Name.ToLower())) {
                Success =  false;
            }
            try {
                var mDbConnection = new SQLiteConnection("Data Source=MangaDB.sqlite;Version=3;");
                mDbConnection.Open();
                var sql =
                    $"insert into {manga.Site} (name, chapter, last_update, link, rss_url) values ('{manga.Name.Replace("'", "''")}', '{manga.Chapter}', datetime('{manga.Date.ToString("yyyy-MM-dd HH:mm:ss")}'), '{manga.Link}', '{manga.RssLink}')";
                var command = new SQLiteCommand(sql, mDbConnection);
                command.ExecuteNonQuery();
                DebugText.Write($"{mDbConnection.Changes} rows affected ");
                mDbConnection.Close();
            } catch(Exception e) {
                DebugText.Write(e.Message);
                Success = false;
            }
            Success = true;
        }
    }
}
