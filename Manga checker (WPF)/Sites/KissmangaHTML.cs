using System;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using MangaChecker.Common;
using MangaChecker.Database;
using MangaChecker.Models;

namespace MangaChecker.Sites {
    internal class KissmangaHTML {
        //TODO: work on this shit
        // weow kissmanga.com/Manga/name
        public static string Check(MangaModel manga, string open) {
            var nameformat = Regex.Replace(manga.Name, "[^0-9a-zA-Z]+", "-").Trim('-').ToLower();
            //DebugText.Write(nameformat);
            var site = "http://kissmanga.com/Manga/" + nameformat;
            MatchCollection matches;
            var source = CloudflareGetString.Get(site);
            matches = Regex.Matches(source, "<a href=\"(.+)\" title=\"(.+)\">", RegexOptions.IgnoreCase);
            if (matches.Count >= 1) {
                var chp = int.Parse(manga.Chapter);
                chp++;
                foreach (var match in matches.Cast<Match>().Reverse()) {
                    if (match.Success) {
                        var grptwo = match.Groups[2].Value.ToLower();
                        var grpone = match.Groups[1].Value.ToLower();
                        if (grptwo.Contains(manga.Name.ToLower()) && grptwo.Contains(chp.ToString()) &&
                            !grptwo.Contains("class=\"clear\"")) {
                            if (open.Equals("1")) {
                                Process.Start("http://kissmanga.com/" + grpone);
                                manga.Chapter = chp.ToString();
                                manga.Link = "http://kissmanga.com/" + grpone;
                                manga.Date = DateTime.Now;
                                Sqlite.UpdateManga(manga);
                                DebugText.Write($"[Kissmanga] Found new Chapter {manga.Name} {grptwo}.");
                                break;
                            }
                        }
                    }
                }
            }
            return manga.FullName;
        }
    }
}