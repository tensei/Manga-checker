using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Manga_checker.Common {
    public static class GlobalVariables {

        public static readonly Dictionary<string, string> SitesforDatabaseTables = new Dictionary<string, string> {
            {"Mangafox", "http://mangafox.me/"},
            {"Mangahere", "http://mangahere.co/"},
            {"Mangareader", "http://www.mangareader.net/"},
            {"Mangastream", "http://mangastream.com/"},
            {"Batoto", "http://bato.to/"},
            {"Webtoons", "http://www.webtoons.com/"},
            {"YoManga", "http://yomanga.co/"},
            {"Kissmanga", "http://kissmanga.com/"},
            {"Backlog", "/"},
            {"GoScanlation", "https://gameofscanlation.moe/" }
        };

        public static readonly List<string> DataGridFillSites = new List<string> {
            "Mangafox",
            "Mangahere",
            "Mangareader",
            "Mangastream",
            "Batoto",
            "Webtoons",
            "YoManga",
            "Kissmanga",
            "GoScanlation"
        };

        private static readonly Dictionary<string, string> _listboxItemNames = new Dictionary<string, string> {
            {"All", null},
            {"Mangareader" ,null},
            {"Mangafox" ,null},
            {"Mangahere" ,null},
            {"Mangastream"  ,null},
            {"Batoto" ,null},
            {"Kissmanga" ,null},
            {"Webtoons" ,null},
            {"Yomanga" ,null},
            {"GoScanlation" ,"GameOfScanlation"},
            {"Backlog" ,null},
            {"DEBUG" ,null}
        };

        public static ObservableCollection<ListBoxItem> ListboxItemNames => createListboxItemNames();

        private static ObservableCollection<ListBoxItem> createListboxItemNames() {
            var obC = new ObservableCollection<ListBoxItem>();
            foreach (var listboxItemName in _listboxItemNames) {
                var x = new ListBoxItem {Content = listboxItemName.Key, ToolTip = listboxItemName.Value};
                obC.Add(x);
            }
            return obC;
        }
    }
}
