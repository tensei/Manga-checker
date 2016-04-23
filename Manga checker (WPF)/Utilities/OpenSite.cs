using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Manga_checker.Utilities {
    internal class OpenSite {
        public static void Open(string site, string name, string chapter, List<string> mlist) {
            switch (site.ToLower()) {
                case "mangafox": {
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
                                      .Replace("! ", "_").Replace("-", "_").Replace(":", "_") + "/c" + chapter +
                                  "/1.html");
                    break;
                }
                case "mangareader": {
                    //open mangareader site for current chapter
                    if (chapter.Contains(" ")) {
                        var chaptersplit = chapter.Split(new[] {" "}, StringSplitOptions.None);
                        Process.Start("http://www.mangareader.net/" +
                                      name.Replace(" ", "-").Replace("!", "").Replace(":", "") + "/" + chaptersplit[0]);
                    }
                    else {
                        Process.Start("http://www.mangareader.net/" +
                                      name.Replace(" ", "-").Replace("!", "").Replace(":", "") + "/" + chapter);
                    }
                    break;
                }
                case "batoto": {
                    foreach (var mangarss in mlist) {
                        if (mangarss.ToLower().Contains(name.ToLower()) &&
                            mangarss.ToLower().Contains(chapter.ToLower())) {
                            var link = mangarss.Split(new[] {"[]"}, StringSplitOptions.None)[1];
                            Process.Start(link);
                        }
                    }
                    break;
                }
            }
        }
    }
}