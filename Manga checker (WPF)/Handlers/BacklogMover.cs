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
            ParseFile.RemoveManga("backlog", name);
            ParseFile.AddManga(site, name, chapter, "");
        }
    }
}
