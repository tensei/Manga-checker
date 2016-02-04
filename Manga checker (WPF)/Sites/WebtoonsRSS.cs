using System;
using System.Diagnostics;
using Manga_checker.Database;
using Manga_checker.Handlers;
using Manga_checker.ViewModels;

namespace Manga_checker.Sites {
    internal class WebtoonsRSS {
        //soon...
        public static void Check(MangaViewModel manga) {
            try {
                var open = ParseFile.GetValueSettings("open links");
                var Name = manga.Name;
                var Chapter = int.Parse(manga.Chapter);
                Chapter++;
                var Url = manga.RssLink;
                var rssitems = RSSReader.Read(Url);
                if (rssitems == null) return;
                foreach (var rssitem in rssitems.Items) {
                    if (rssitem.Title.Text.Contains(Chapter.ToString())) {
                        if (open.Equals("1")) {
                            Process.Start(rssitem.Links[0].Uri.AbsoluteUri);
                            ParseFile.SetManga("webtoons", Name, Chapter.ToString());
                            Sqlite.UpdateManga("webtoons", Name, Chapter.ToString(),
                                rssitem.Links[0].Uri.AbsoluteUri);
                            DebugText.Write(
                                $"[Webtoons] Found new Chapter {Name} {rssitem.Title.Text}.");
                        }
                    }
                }
            }
            catch (Exception ex) {
                DebugText.Write($"[Webtoons] Error {ex.Message}.");
            }
        }
    }
}