using System;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Manga_checker.Database;
using Manga_checker.Utilities;
using Manga_checker.ViewModels.Model;

namespace Manga_checker.Sites {
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
                                Sqlite.UpdateManga("kissmanga", manga.Name, chp.ToString(), "http://kissmanga.com/" + grpone,
                                    DateTime.Now);
                                manga.Chapter = chp.ToString();
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