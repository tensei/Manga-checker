using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Manga_checker.Classes
{
    class ListViewMangaItem
    {
        public string Site { get; set; }
        public string Name { get; set; }
        public string Chapter { get; set; }
        public bool IsEnabled { get; set; } = true;
    }
}
