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
        private readonly DebugText DebugText = new DebugText();
        public void Check()
        {
            try
            {

                var Mangas = ParseFile.GetManga("webtoons");
                var open = ParseFile.GetValueSettings("open links");
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
                                ParseFile.SetManga("webtoons", Name, Chapter.ToString());
                                DebugText.Write($"[{DateTime.Now}][Kissmanga] Found new Chapter {Name} {rssitem.Title.Text}.");
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                DebugText.Write($"[{DateTime.Now}][Webtoons] Error {ex.Message}.");
            }
            
        }
    }
}
