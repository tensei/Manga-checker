using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Manga_checker.Handlers;
using Manga_checker.Properties;

namespace Manga_checker.Sites
{
    class KissmangaHTML
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
            string source = new GetSource().get(site);
            matches = Regex.Matches(source, "<a href=\"(.+)\" title=\"(.+)\">", RegexOptions.IgnoreCase);
            var parse = new ParseFile();
            var open = parse.GetValueSettings("open links");
            if (matches.Count >= 1)
            {
                var chp = int.Parse(chapter);
                chp++;
                foreach (Match match in matches.Cast<Match>().Reverse())
                {
                    if (match.Success)
                    {
                        
                        var grptwo = match.Groups[2].Value.ToLower();
                        var grpone = match.Groups[1].Value.ToLower();
                        if (grptwo.Contains(name.ToLower()) && grptwo.Contains(chp.ToString()) && !grptwo.Contains("class=\"clear\""))
                        {
                            if (open.Equals("1"))
                            {
                                Process.Start("http://kissmanga.com/" + grpone);
                                parse.SetManga("kissmanga", name, chp.ToString());
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
