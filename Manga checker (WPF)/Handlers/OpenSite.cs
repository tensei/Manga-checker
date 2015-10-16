using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manga_checker.Handlers
{
    class OpenSite
    {
        private readonly ParseFile _parseFile = new ParseFile();

        public void Open(string site, string name, string chapter, List<string> mlist)
        {
            switch (site)
            {
                case "mangafox":
                {
                        Process.Start("http://mangafox.me/manga/" +
                  name.Replace(":", "_").Replace("(", "").Replace(")", "").Replace(", ", "_")
                      .Replace(" - ", " ")
                      .Replace("-", "_")
                      .Replace(" ", "_")
                      .Replace("'", "_")
                      .Replace("! -", "_")
                      .Replace("!", "")
                      .Replace(". ", "_")
                      .Replace(".", "")
                      .Replace("! ", "_").Replace("-", "_").Replace(":", "_") + "/c" + chapter + "/1.html");
                        break;
                    }
                case "mangareader":
                {
                        //open mangareader site for current chapter
                        if (chapter.Contains(" "))
                        {
                            var chaptersplit = chapter.Split(new[] { " " }, StringSplitOptions.None);
                            Process.Start("http://www.mangareader.net/" +
                                          name.Replace(" ", "-").Replace("!", "").Replace(":", "") + "/" + chaptersplit[0]);
                            
                        }
                        else
                        {
                            Process.Start("http://www.mangareader.net/" +
                                          name.Replace(" ", "-").Replace("!", "").Replace(":", "") + "/" + chapter);
                            
                        }
                        break;
                    }
                case "batoto":
                {
                        foreach (var mangarss in mlist)
                        {
                            if (mangarss.ToLower().Contains(name.ToLower()) &&
                                mangarss.ToLower().Contains(chapter.ToLower()))
                            {
                                var link = mangarss.Split(new[] { "[]" }, StringSplitOptions.None)[1];
                                Process.Start(link);
                                
                                var intcrch = float.Parse(_parseFile.GetValueChapter("batoto", name));
                                if (float.Parse(chapter) > intcrch)
                                {
                                    _parseFile.SetManga("batoto", name, chapter, "true");
                                }
                            }
                        }
                        break;
                    }
            }
        }
    }
}
