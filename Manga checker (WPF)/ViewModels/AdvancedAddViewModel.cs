using System;
using System.Collections.Generic;
using System.Windows.Input;
using Manga_checker.Database;
using Manga_checker.Utilities;
using PropertyChanged;

namespace Manga_checker.ViewModels {
    [ImplementPropertyChanged]
    public class AdvancedAddViewModel {
        public AdvancedAddViewModel() {
            //Sqlite.AddManga("backlog", Name, Chapter, "placeholder", DateTime.Now);
            Sites = Sqlite.GetAllTables();
            Sites.Remove("settings");
            Sites.Remove("sqlite_sequence");
            Sites.Remove("link_collection");
            AddCommand = new ActionCommand(AddManga);
        }

        public string Name { get; set; }
        public string Chapter { get; set; }
        public string RSSLink { get; set; }
        public string Site { get; set; }
        public List<string> Sites { get; set; }

        public ICommand AddCommand { get; }

        private void AddManga() {
            if (RSSLink == null || RSSLink == string.Empty) {
                RSSLink = "placeholder";
            }
            try {
                DebugText.Write($"[Advanced Add] Trying to add {Name} {Chapter} to {Site}");
                Sqlite.AddManga(Site, Name, Chapter, RSSLink, DateTime.Now);
            } catch {
                DebugText.Write($"[Advanced Add]{Name} {Chapter} {Site}");
            }
        }
    }
}