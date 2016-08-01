using System;
using System.Diagnostics;
using System.ServiceModel.Syndication;
using MangaChecker.Common;
using MangaChecker.Database;
using MangaChecker.Models;

namespace MangaChecker.Sites {
    internal class YomangaRSS {
        public static void Check(MangaModel manga, SyndicationFeed rss, string openLinks) {
            try {
                var feed = rss;
                var newch = int.Parse(manga.Chapter) + 1;
                var full = manga.Name + " chapter " + newch;
                foreach (var item in feed.Items) {
                    var title = item.Title.Text;
                    if (Equals(full.ToLower(), title.ToLower())) {
                        if (openLinks.Equals("1")) {
                            Process.Start(item.Links[0].Uri.AbsoluteUri);
                            manga.Chapter = newch.ToString();
                            manga.Link = item.Links[0].Uri.AbsoluteUri;
                            manga.Date = DateTime.Now;
                            Sqlite.UpdateManga(manga);
                            DebugText.Write($"[YoManga] Found new Chapter {manga.Name} {newch}.");
                            break;
                        }
                    }
                }

                // DebugText.Write(item.Title.Text);
            } catch (Exception ex) {
                DebugText.Write($"[YoManga] Error {ex.Message}.");
            }
        }
    }
}