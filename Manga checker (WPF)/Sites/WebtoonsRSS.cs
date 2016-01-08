using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Manga_checker.Handlers;

namespace Manga_checker.Sites
{
    class WebtoonsRSS
    {
        //soon...
        private readonly DebugText debug = new DebugText();
        public void Check()
        {
            try
            {
                ParseFile parse = new ParseFile();
                var Mangas = parse.GetManga("webtoons");
                var open = parse.GetValueSettings("open links");
                foreach (var manga in Mangas)
                {
                    var splitterino = manga.Split(new[] { "[]" }, StringSplitOptions.None);
                    var Name = splitterino[0];
                    var Chapter = int.Parse(splitterino[1]);
                    Chapter++;
                    var Url = splitterino[2];
                    var rssitems = new RSSReader().Read(Url);
                    foreach (var rssitem in rssitems.Items)
                    {
                        if (rssitem.Title.Text.Contains(Chapter.ToString()))
                        {
                            if (open.Equals("1"))
                            {
                                Process.Start(rssitem.Links[0].Uri.AbsoluteUri);
                                parse.SetManga("webtoons", Name, Chapter.ToString());
                                debug.Write($"[{DateTime.Now}][Kissmanga] Found new Chapter {Name} {rssitem.Title.Text}.");
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                debug.Write($"[{DateTime.Now}][Webtoons] Error {ex.Message}.");
            }
            
        }
    }
}
