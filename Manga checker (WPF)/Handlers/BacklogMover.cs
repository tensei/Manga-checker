using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manga_checker.Handlers
{
    class BacklogMover
    {
        public void Move(string site, string name, string chapter)
        {
            ParseFile parse = new ParseFile();
            parse.RemoveManga("backlog", name);
            parse.AddManga(site, name, chapter, "");
        }
    }
}
