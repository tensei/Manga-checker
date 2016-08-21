using System.Threading.Tasks;
using MangaChecker.Database;
using MangaChecker.Dialogs;
using MangaChecker.Models;
using MangaChecker.Sites;
using MangaChecker.Sites.HTML;
using MangaChecker.Sites.RSS;
using MaterialDesignThemes.Wpf;

namespace MangaChecker.Common {
    internal class Tools {
        public static void ChangeChaperNum(MangaModel item, string op) {
            if(!item.Chapter.Contains(" ")) {
                var chapter = int.Parse(item.Chapter);
                if(op.Equals("-")) {
                    chapter--;
                    try {
                        var newDate = item.Date.AddDays(-1);
                        item.Date = newDate;
                    } catch {
                        //ignored
                    }
                } else {
                    chapter++;
                    try {
                        var newDate = item.Date.AddDays(1);
                        item.Date = newDate;
                    } catch {
                        //ignored
                    }
                }

                item.Chapter = chapter.ToString();
                var sqliteUpdateManga = new SqliteUpdateManga(item, false);
            }
        }

        public static void CreateDb() {
            DialogHost.Show(new SetupDatabaseDialog());
        }

        public static async Task<bool> Delete(MangaModel mangaItem) {
            var dialog = new ConfirmDeleteDialog {
                MessageTextBlock = {
                    Text = "Deleting\n" + mangaItem.Name
                },
                SiteName = {
                    Text = mangaItem.Site
                },
                item = mangaItem
            };
            var x = await DialogHost.Show(dialog);
            return (string)x == "1";
        }

        public static void RefreshManga(MangaModel manga) {
            var setting = Sqlite.GetSettings();
            try {
                switch(manga.Site) {
                    case "mangareader": {
                            MangareaderHTML.Check(manga, setting["open links"]);
                            break;
                        }
                    case "mangastream": {
                            var feed = Mangastream.Get_feed_titles();
                            Mangastream.Check(manga, feed, setting["open links"]);
                            break;
                        }
                    case "mangafox": {
                            Mangafox.Check(manga, setting["open links"]);
                            break;
                        }
                    case "mangahere": {
                            Mangahere.Check(manga, setting["open links"]);
                            break;
                        }
                    case "batoto": {
                            var feed = Batoto.Get_feed_titles();
                            Batoto.Check(feed, manga, setting["open links"]);
                            break;
                        }
                    case "kissmanga": {
                            KissmangaHTML.Check(manga, setting["open links"]);
                            break;
                        }
                    case "yomanga": {
                            var feed = RSSReader.Read("http://yomanga.co/reader/feeds/rss") ??
                                       RSSReader.Read("http://46.4.102.16/reader/feeds/rss");
                            Yomanga.Check(manga, feed, setting["open links"]);
                            break;
                        }
                    case "webtoons": {
                            Webtoons.Check(manga, setting["open links"]);
                            break;
                        }
                    case "kireicake": {
                            var rss = RSSReader.Read("http://reader.kireicake.com/rss.xml");
                            KireiCake.Check(manga, rss, setting["open links"]);
                            break;
                        }
                    case "jaiminisbox": {
                            var rss = RSSReader.Read("https://jaiminisbox.com/reader/rss.xml");
                            Jaiminisbox.Check(manga, rss, setting["open links"]);
                            break;
                        }
                    case "goscanlation": {
                            GameOfScanlation.Check(manga, setting["open links"]);
                            break;
                        }
                }
            } catch {
                DebugText.Write($"Error refreshing manga");
            }
        }
    }
}