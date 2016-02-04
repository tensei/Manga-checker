using System;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;
using Manga_checker.Database;
using Manga_checker.Handlers;
using Manga_checker.ViewModels;

namespace Manga_checker.Sites {
    internal class MangareaderHTML {
        public static string Check(MangaViewModel manga) {
            var name = Regex.Replace(manga.Name, "[^0-9a-zA-Z]+", "-").Trim('-').ToLower();
            var ch = manga.Chapter;
            var w = new WebClient();
            MatchCollection m1;
            var FullName = "";
            var mangaa = "";
            var ch_plus = 0;
            if (ch.Contains(" ")) {
                var chsp = manga.Chapter.Split(new[] {" "}, StringSplitOptions.None);
                ch_plus = int.Parse(chsp[0]);
                ch_plus++;
                FullName = manga.Name + " " + ch_plus;

                var url_2 = "http://www.mangareader.net/" + chsp[1] + "/" + name + ".html";
                var htmltxt2 = w.DownloadString(url_2);
                m1 = Regex.Matches(htmltxt2, @"<a href=.+>(.+)</a>.+</li>", RegexOptions.IgnoreCase);
                foreach (Match mangamatch in m1) {
                    mangaa = mangamatch.Groups[1].Value;
                    if (mangaa.ToLower().Contains(FullName)) {
                        var link = "http://www.mangareader.net/" + name + "/" + ch_plus;
                        if (ParseFile.GetValueSettings("open links") == "1") {
                            Process.Start(link);
                            ParseFile.SetManga("mangareader", manga.Name, ch_plus + " " + chsp[1]);
                            Sqlite.UpdateManga("mangareader", manga.Name, ch_plus + " " + chsp[1], link);
                            manga.Chapter = ch_plus + " " + chsp[1];
                            manga.Link = link;
                        }
                        DebugText.Write($"[Mangareader] {manga.Name} {ch_plus} Found new Chapter");
                        return FullName;
                    }
                    FullName = manga.Name + " " + chsp[0];
                }
            }
            else {
                var chsp = ch;
                ch_plus = int.Parse(chsp);
                ch_plus++;
                FullName = manga.Name + " " + ch_plus;
                var url_1 = "http://www.mangareader.net/" + name;
                var htmltext1 = w.DownloadString(url_1);
                m1 = Regex.Matches(htmltext1, @"<a href=.+>(.+)</a>.+</li>", RegexOptions.IgnoreCase);
                foreach (Match mangamatch in m1) {
                    mangaa = mangamatch.Groups[1].Value;
                    if (mangaa.ToLower().Contains(FullName)) {
                        var link = "http://www.mangareader.net/" + name + "/" + ch_plus;
                        if (ParseFile.GetValueSettings("open links") == "1") {
                            Process.Start(link);
                            ParseFile.SetManga("mangareader", manga.Name, ch_plus.ToString());
                            Sqlite.UpdateManga("mangareader", manga.Name, ch_plus + " " + chsp[1], link);
                            manga.Chapter = ch_plus.ToString();
                            manga.Link = link;
                        }
                        DebugText.Write($"[Mangareader] {manga.Name} {ch_plus} Found new Chapter");
                        return FullName;
                    }
                    FullName = manga.Name + " " + chsp;
                }
            }
            return FullName;
        }
    }
}