using System;
using System.Diagnostics;
using MangaChecker.Common;
using MangaChecker.Database;
using MangaChecker.Models;

namespace MangaChecker.Sites.RSS {
    internal class Webtoons {
        // soon...
        public static void Check(MangaModel manga, string openLinks) {
            try {
                var Name = manga.Name;
                var Chapter = int.Parse(manga.Chapter);
                Chapter++;
                var Url = manga.RssLink;
                var rssitems = RssReader.Read(Url);
                if (rssitems == null) return;
                foreach (var rssitem in rssitems.Items) {
                    if (rssitem.Title.Text.Contains(Chapter.ToString())) {
                        if (openLinks.Equals("1")) {
                            Process.Start(rssitem.Links[0].Uri.AbsoluteUri);
                            manga.Chapter = Chapter.ToString();
                            manga.Link = rssitem.Links[0].Uri.AbsoluteUri;
                            manga.Date = DateTime.Now;
                            Sqlite.UpdateManga(manga);
                            DebugText.Write($"[Webtoons] Found new Chapter {Name} {rssitem.Title.Text}.");
                        }
                    }
                }
            } catch (Exception ex) {
                DebugText.Write($"[Webtoons] Error {ex.Message}.");
            }
        }
    }
}