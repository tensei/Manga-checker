using System;
using System.Collections.Generic;
using System.Windows.Input;
using MangaChecker.Common;
using MangaChecker.Database;
using MangaChecker.Models;
using PropertyChanged;

namespace MangaChecker.ViewModels {
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
            if (string.IsNullOrEmpty(RSSLink)) {
                RSSLink = "placeholder";
            }
            try {
                var manga = new MangaModel {
                    Name = Name,
                    Site = Site,
                    Chapter = Chapter,
                    RssLink = RSSLink,
                    Date = DateTime.Now
                };
                DebugText.Write($"[Advanced Add] Trying to add {Name} {Chapter} to {Site}");
                var sqliteAddManga = new SqliteAddManga(manga);
            } catch {
                DebugText.Write($"[Advanced Add]{Name} {Chapter} {Site}");
            }
        }
    }
}