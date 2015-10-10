using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manga_checker__WPF_.Handlers
{
    class BacklogMover
    {
        public void Move(string site, string name, string chapter)
        {
            using (ParseFile parse = new ParseFile())
            {
                parse.RemoveManga("backlog", name, chapter);
                parse.AddManga(site, name, chapter, "false");

            }
        }
    }
}
