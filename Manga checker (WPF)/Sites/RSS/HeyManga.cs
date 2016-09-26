using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MangaChecker.Common;
using MangaChecker.Database;
using MangaChecker.Models;

namespace MangaChecker.Sites.RSS {
    public static class HeyManga {
        public static void Check(MangaModel manga, SyndicationFeed rss, string openLinks) {
            try {
                var feed = rss;
                foreach(var item in feed.Items.Reverse()) {
                    var title = item.Title.Text;
                    if(title.ToLower().Contains(manga.Name.ToLower())) {

                        var ch = Regex.Match(title, @".+ ([\d\.]+)$").Groups[1].Value;

                        if(float.Parse(manga.Chapter) >= float.Parse(ch) || manga.Date >= item.PublishDate.DateTime) continue;

                        if(openLinks.Equals("1")) {
                            Process.Start(item.Links[0].Uri.AbsoluteUri);
                            manga.Chapter = ch;
                            manga.Link = item.Links[0].Uri.AbsoluteUri;
                            manga.Date = item.PublishDate.DateTime;
                            Sqlite.UpdateManga(manga);
                            DebugText.Write($"[HeyManga] Found new Chapter {manga.Name} {ch}.");
                            break;
                        }
                    }
                }

                // DebugText.Write(item.Title.Text);
            } catch(Exception ex) {
                DebugText.Write($"[HeyManga] Error {ex.Message}.");
            }
        }
    }
}
