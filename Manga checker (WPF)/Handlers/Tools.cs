using System;
using System.Threading.Tasks;
using Manga_checker.Database;
using Manga_checker.Sites;
using Manga_checker.ViewModels;
using MaterialDesignThemes.Wpf;

namespace Manga_checker.Handlers {
    internal class Tools {
        public static void ChangeChaperNum(MangaModel item, string op) {
            if (!item.Chapter.Contains(" ")) {
                var chapter = int.Parse(item.Chapter);
                if (op.Equals("-")) {
                    chapter--;
                    var newDate = item.Date.AddDays(-1);
                    item.Date = newDate;
                }
                else {
                    chapter++;
                    var newDate = item.Date.AddDays(1);
                    item.Date = newDate;
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

            switch (manga.Site) {
                case "mangareader": {
                    MangareaderHTML.Check(manga);
                    break;
                }
                case "mangastream": {
                    var feed = MangastreamRSS.Get_feed_titles();
                    MangastreamRSS.Check(manga, feed);
                    break;
                }
                case "mangafox": {
                    MangafoxRSS.Check(manga);
                    break;
                }
                case "batoto": {
                    var feed = BatotoRSS.Get_feed_titles();
                    BatotoRSS.Check(feed, manga);
                    break;
                }
                case "yomanga": {
                    var feed = RSSReader.Read("http://yomanga.co/reader/feeds/rss") ??
                                RSSReader.Read("http://46.4.102.16/reader/feeds/rss");
                    YomangaRSS.Check(manga, feed);
                    break;
                }
                case "webtoons": {
                    WebtoonsRSS.Check(manga);
                    break;
                }
            }

        }
    }
}