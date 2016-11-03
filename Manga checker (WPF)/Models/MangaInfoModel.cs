using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaChecker.Models {
    public class MangaInfoModel {
        public string Name { get; set; }
        public string Chapter { get; set; }
        public string Site { get; set; }
        public string Rss { get; set; }
        public string Link { get; set; }
        public string Error { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
    }
}
