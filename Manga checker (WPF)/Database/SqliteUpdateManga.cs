using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MangaChecker.Common;
using MangaChecker.Models;

namespace MangaChecker.Database {
    public class SqliteUpdateManga {
        private void addToNew(MangaModel manga) {
            Application.Current.Dispatcher.BeginInvoke(new Action(() => {
                GlobalVariables.NewMangasInternal.Add(manga);
            }));
        }
        public SqliteUpdateManga(MangaModel manga,
            bool linkcol = true) {
            try {
                manga.Name = manga.Name.Replace("'", "''");
                var mDbConnection = new SQLiteConnection("Data Source=MangaDB.sqlite;Version=3;");
                mDbConnection.Open();
                var sql =
                    $"UPDATE { manga.Site.ToLower()} SET " +
                    $"chapter = '{ manga.Chapter}', " +
                    $"link = '{ manga.Link}', " +
                    $"last_update = datetime('{ manga.Date.ToString("yyyy-MM-dd HH:mm:ss")}') " +
                    $"WHERE name = '{ manga.Name}'";

                new SQLiteCommand(sql, mDbConnection).ExecuteNonQuery();

                if(!manga.Site.ToLower().Equals("backlog") && linkcol) {
                    new SQLiteCommand(
                        $@"INSERT INTO link_collection (name, chapter, added, link, site) VALUES ('{ manga.Name}', '{ manga.Chapter
                            }', (datetime()), '{ manga.Link}', '{ manga.Site
                                .ToLower()}')", mDbConnection).ExecuteNonQuery();
                    addToNew(manga);
                    new SQLiteCommand(
                        $@"INSERT INTO mangasnotread (name, chapter, last_update, link, site) VALUES ('{ manga.Name}', '{ manga.Chapter
                            }', (datetime()), '{ manga.Link}', '{ manga.Site
                                .ToLower()}')", mDbConnection).ExecuteNonQuery();

                }
                DebugText.Write($"{mDbConnection.Changes} rows affected ");
                mDbConnection.Close();
            } catch(Exception e) {
                DebugText.Write(e.Message);
            }
        }
    }
}
