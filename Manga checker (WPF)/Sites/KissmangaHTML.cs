using System;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Manga_checker.Database;
using Manga_checker.Handlers;
using Manga_checker.Properties;

namespace Manga_checker.Sites
{
    internal class KissmangaHTML
    {
        //TODO: work on this shit
        // weow kissmanga.com/Manga/name
        public void check(string name, string chapter)
        {
            var nameformat =
                name.Trim()
                    .Replace(" ", "-")
                    .Replace("!", "")
                    .Replace("+", "-")
                    .Replace("(", "")
                    .Replace(")", "")
                    .Replace("+", "")
                    .Replace(":", "")
                    .Replace(",", "")
                    .Replace("--", "-")
                    .Replace("---", "-")
                    .Replace("  ", "-");

            var site = "http://kissmanga.com/Manga/" + nameformat;
            MatchCollection matches;
            var source = new GetSource().get(site);
            matches = Regex.Matches(source, "<a href=\"(.+)\" title=\"(.+)\">", RegexOptions.IgnoreCase);
            var open = ParseFile.GetValueSettings("open links");
            if (matches.Count >= 1)
            {
                var chp = int.Parse(chapter);
                chp++;
                foreach (var match in matches.Cast<Match>().Reverse())
                {
                    if (match.Success)
                    {
                        var grptwo = match.Groups[2].Value.ToLower();
                        var grpone = match.Groups[1].Value.ToLower();
                        if (grptwo.Contains(name.ToLower()) && grptwo.Contains(chp.ToString()) &&
                            !grptwo.Contains("class=\"clear\""))
                        {
                            if (open.Equals("1"))
                            {
                                Process.Start("http://kissmanga.com/" + grpone);
                                ParseFile.SetManga("kissmanga", name, chp.ToString());
                                Sqlite.UpdateManga("kissmanga", name, chp.ToString(), "http://kissmanga.com/" + grpone);
                                debugtext($"[{DateTime.Now}][Kissmanga] Found new Chapter {name} {grptwo}.");
                                break;
                            }
                        }
                    }
                }
            }
        }

        public void debugtext(string text)
        {
            Settings.Default.Debug += text + "\n";
        }
    }
}