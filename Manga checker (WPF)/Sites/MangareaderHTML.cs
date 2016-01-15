using System;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using Manga_checker.Properties;

namespace Manga_checker
{
    internal class MangareaderHTML
    {
        public string Check(string mangafull)
        {
            var splitManga = mangafull.Split(new[] {"[]"}, StringSplitOptions.None);
            var name = splitManga[0];
            var ch = splitManga[1];
            var w = new WebClient();
            MatchCollection m1;
            var FullName = "";
            var mangaa = "";
            var ch_plus = 0;
            if (ch.Contains(" "))
            {
                var chsp = ch.Split(new[] {" "}, StringSplitOptions.None);
                ch_plus = Int32.Parse(chsp[0]);
                ch_plus++;
                FullName = name + " " + ch_plus;
                var url_2 = "http://www.mangareader.net/" + chsp[1] + "/" +
                            name.Replace(" ", "-").Replace("!", "").Replace(":", "") + ".html";
                var htmltxt2 = w.DownloadString(url_2);
                m1 = Regex.Matches(htmltxt2, @"<a href=.+>(.+)</a>.+</li>", RegexOptions.IgnoreCase);
                foreach (Match manga in m1)
                {
                    mangaa = manga.Groups[1].Value;
                    if (mangaa.ToLower().Contains(FullName))
                    {
                        if (ParseFile.GetValueSettings("open links") == "1")
                        {
                            Process.Start("http://www.mangareader.net/" +
                                          name.Replace(" ", "-").Replace("!", "").Replace(":", "") +
                                          "/" +
                                          ch_plus);
                            ParseFile.SetManga("mangareader", name, ch_plus + " " + chsp[1]);
                        }
                        else
                        {
                            ParseFile.SetManga("mangareader", name, ch_plus + " " + chsp[1]);
                        }
                        debugtext(string.Format("[{2}][Mangareader] {0} {1} Found new Chapter", name, ch_plus, DateTime.Now));
                        return FullName;
                    }
                    FullName = name + " " + chsp[0];
                }
            }
            else
            {
                var chsp = ch;
                ch_plus = Int32.Parse(chsp);
                ch_plus++;
                FullName = name + " " + ch_plus;
                var url_1 = "http://www.mangareader.net/" + name.Replace(" ", "-").Replace("!", "").Replace(":", "");
                var htmltext1 = w.DownloadString(url_1);
                m1 = Regex.Matches(htmltext1, @"<a href=.+>(.+)</a>.+</li>", RegexOptions.IgnoreCase);
                foreach (Match manga in m1)
                {
                    mangaa = manga.Groups[1].Value;
                    if (mangaa.ToLower().Contains(FullName))
                    {
                        if (ParseFile.GetValueSettings("open links") == "1")
                        {
                            Process.Start("http://www.mangareader.net/" +
                                          name.Replace(" ", "-").Replace("!", "").Replace(":", "") +
                                          "/" +
                                          ch_plus);
                            ParseFile.SetManga("mangareader", name, ch_plus.ToString());
                        }
                        else
                        {
                            ParseFile.SetManga("mangareader", name, ch_plus.ToString());
                        }
                        debugtext(string.Format("[{2}][Mangareader] {0} {1} Found new Chapter", name, ch_plus, DateTime.Now));
                        return FullName;
                    }
                    FullName = name + " " + chsp;
                }
            }
            return FullName;
        }
        public void debugtext(string text)
        {//Read
            Settings.Default.Debug += text + "\n";
            //Write settings to disk
            Settings.Default.Save();
        }
    }
}