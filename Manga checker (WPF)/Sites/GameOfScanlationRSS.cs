using System;
using System.Diagnostics;
using MangaChecker.Common;
using MangaChecker.Database;
using MangaChecker.Models;

namespace MangaChecker.Sites {
    public static class GameOfScanlationRSS {
        public static void Check(MangaModel manga, string openLinks) {
            try {
                var Name = manga.Name;
                var Chapter = int.Parse(manga.Chapter);
                Chapter++;
                var Url = manga.RssLink;
                var rssitems = RSSReader.Read(Url);
                if (rssitems == null) return;
                foreach (var rssitem in rssitems.Items) {
                    if (rssitem.Title.Text.Contains(Chapter.ToString())) {
                        if (openLinks.Equals("1")) {
                            Process.Start(rssitem.Links[0].Uri.AbsoluteUri);
                            var date = DateTime.Now;
                            if (!rssitem.PublishDate.DayOfYear.Equals(1)) {
                                date = rssitem.PublishDate.DateTime;
                            }
                            manga.Chapter = Chapter.ToString();
                            manga.Link = rssitem.Links[0].Uri.AbsoluteUri;
                            manga.Date = date;
                            Sqlite.UpdateManga(manga);
                            DebugText.Write($"[GameOfScanlation] Found new Chapter {Name} {rssitem.Title.Text}.");
                        }
                    }
                }
            } catch (Exception ex) {
                DebugText.Write($"[GameOfScanlation] Error {ex.Message}.");
            }
        }
    }
}