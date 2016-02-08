namespace Manga_checker.Sites {
    using System;
    using System.Diagnostics;
    using System.ServiceModel.Syndication;

    using Manga_checker.Database;
    using Manga_checker.Handlers;
    using Manga_checker.ViewModels;

    internal class YomangaRSS {
        public RSSReader RssReader = new RSSReader();

        public static void Check(MangaModel manga, SyndicationFeed rss) {
            try {
                var open = ParseFile.GetValueSettings("open links");
                var feed = rss;
                var newch = int.Parse(manga.Chapter) + 1;
                var full = manga.Name + " chapter " + newch;
                foreach (var item in feed.Items) {
                    var title = item.Title.Text;
                    if (Equals(full.ToLower(), title.ToLower())) {
                        if (open.Equals("1")) {
                            Process.Start(item.Links[0].Uri.AbsoluteUri);
                            ParseFile.SetManga("yomanga", manga.Name, newch.ToString());
                            Sqlite.UpdateManga(
                                "yomanga", 
                                manga.Name, 
                                newch.ToString(), 
                                item.Links[0].Uri.AbsoluteUri, 
                                DateTime.Now);
                            DebugText.Write($"[YoManga] Found new Chapter {manga.Name} {newch}.");
                            break;
                        }
                    }
                }

                // DebugText.Write(item.Title.Text);
            }
            catch (Exception ex) {
                DebugText.Write($"[YoManga] Error {ex.Message}.");
            }
        }
    }
}