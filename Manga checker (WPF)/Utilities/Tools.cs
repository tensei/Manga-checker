using System.Threading.Tasks;
using Manga_checker.Database;
using Manga_checker.Sites;
using Manga_checker.ViewModels.Model;
using MaterialDesignThemes.Wpf;

namespace Manga_checker.Utilities {
    internal class Tools {
        public static void ChangeChaperNum(MangaModel item, string op) {
            if (!item.Chapter.Contains(" ")) {
                var chapter = int.Parse(item.Chapter);
                if (op.Equals("-")) {
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
                Sqlite.UpdateManga(item.Site, item.Name, item.Chapter, item.Link, item.Date, false);
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
            return (string) x == "1";
        }

        public static void RefreshManga(MangaModel manga) {
            var setting = Sqlite.GetSettings();
            try {
                switch (manga.Site) {
                    case "mangareader": {
                        MangareaderHTML.Check(manga, setting["open links"]);
                        break;
                    }
                    case "mangastream": {
                        var feed = MangastreamRSS.Get_feed_titles();
                        MangastreamRSS.Check(manga, feed, setting["open links"]);
                        break;
                    }
                    case "mangafox": {
                        MangafoxRSS.Check(manga, setting["open links"]);
                        break;
                    }
                    case "mangahere": {
                        MangahereRSS.Check(manga, setting["open links"]);
                        break;
                    }
                    case "batoto": {
                        var feed = BatotoRSS.Get_feed_titles();
                        BatotoRSS.Check(feed, manga, setting["open links"]);
                        break;
                    }
                    case "kissmanga": {
                        KissmangaHTML.Check(manga, setting["open links"]);
                        break;
                    }
                    case "yomanga": {
                        var feed = RSSReader.Read("http://yomanga.co/reader/feeds/rss") ??
                                   RSSReader.Read("http://46.4.102.16/reader/feeds/rss");
                        YomangaRSS.Check(manga, feed, setting["open links"]);
                        break;
                    }
                    case "webtoons": {
                        WebtoonsRSS.Check(manga, setting["open links"]);
                        break;
                    }
                }
            } catch {
                DebugText.Write($"Error refreshing manga");
            }
        }
    }
}