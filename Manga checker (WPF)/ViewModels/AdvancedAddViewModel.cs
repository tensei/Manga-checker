using System;
using System.Collections.Generic;
using System.Linq;
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
            Sites.Remove("mangasnotread");
            Sites.Sort();
            AddCommand = new ActionCommand(AddManga);
        }

        public string Name { get; set; }
        public string Chapter { get; set; }
        public string RSSLink { get; set; }
        public string Site { get; set; }
        public List<string> Sites { get; set; }
	    public string SuccessText { get; set; }
	    public string ErrorText { get; set; }

        public ICommand AddCommand { get; }

	    private void Reset() {
		    Name = string.Empty;
			Chapter = string.Empty;
			RSSLink = string.Empty;
			Site = string.Empty;
	    }

	    private void AddManga() {
            if (string.IsNullOrEmpty(RSSLink)) {
                RSSLink = "placeholder";
            }
		    if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Chapter) || string.IsNullOrWhiteSpace(Site)) {
			    ErrorText = "Missing Name, Chapter or Site";
			    return;
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
	            if (Tools.RefreshManga(manga)) {
					SuccessText = $"Success adding  {Name} {Chapter} to {Site}";
					var sqliteAddManga = new SqliteAddManga(manga);
					Reset();
					return;
	            }
	            ErrorText = $"Failes Checking {Name} on {Site} make sure everything is correct";
            } catch(Exception e) {
                DebugText.Write($"[Advanced Add]{Name} {Chapter} {Site}");
	            ErrorText = $"Failed -> {e.Message}";
            }
        }
    }
}