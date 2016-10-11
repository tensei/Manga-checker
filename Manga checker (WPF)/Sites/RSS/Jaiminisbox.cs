using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using MangaChecker.Common;
using MangaChecker.Database;
using MangaChecker.Models;

namespace MangaChecker.Sites.RSS {
    public static class Jaiminisbox {
        public static void Check(MangaModel manga, SyndicationFeed rss, string openLinks) {
            try {
                var feed = rss;
                foreach(var item in feed.Items) {
                    var xyz = item.Links[0].Uri.AbsoluteUri.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                    var ch = xyz[xyz.Length - 3] == "en" ? $"{xyz.Last()}" : $"{xyz[xyz.Length - 2]}.{xyz.Last()}";
                    var title = item.Title.Text;
                    if(!title.Contains(manga.Name) || !(float.Parse(manga.Chapter, CultureInfo.InvariantCulture) < float.Parse(ch, CultureInfo.InvariantCulture)))
                        continue;
                    if(openLinks.Equals("1")) {
                        Process.Start(item.Links[0].Uri.AbsoluteUri);
                        manga.Chapter = ch;
                        manga.Link = item.Links[0].Uri.AbsoluteUri;
                        manga.Date = DateTime.Now;
                        Sqlite.UpdateManga(manga);
                        DebugText.Write($"[Jaiminisbox] Found new Chapter {manga.Name} {ch}.");
                    }
                    break;
                }

                // DebugText.Write(item.Title.Text);
            } catch(Exception ex) {
                DebugText.Write($"[Jaiminisbox] Error {ex.Message}.");
            }
        }
    }
}
