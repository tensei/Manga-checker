using System;
using System.Windows;
using System.Windows.Input;
using MangaChecker.Database;
using MangaChecker.Models;
using PropertyChanged;

namespace MangaChecker.ViewModels {
	[ImplementPropertyChanged]
	public class AddMenuViewModel {
		public AddMenuViewModel() {
			Instance = this;
			AddBacklogCommand = new ActionCommand(AddToBacklog);
			AddNormalCommand = new ActionCommand(NormalClick);
			AddAdvancedCommand = new ActionCommand(AdvancedClick);
		}

		public static AddMenuViewModel Instance { get; set; }

		public NormalAddViewModel NormalAddDataContext { get; set; }
		public AdvancedAddViewModel AdvancedAddDataContext { get; set; }

		public string Name { get; set; }
		public string Chapter { get; set; }
		public int TransInt { get; set; }
		public Visibility TransVis { get; set; } = Visibility.Collapsed;

		public ICommand AddBacklogCommand { get; }
		public ICommand AddNormalCommand { get; }
		public ICommand AddAdvancedCommand { get; }
		//public ICommand AddAdvancedCommand { get; }


		private void NormalClick() {
			NormalAddDataContext = new NormalAddViewModel();
			TransVis = Visibility.Visible;
			TransInt = 0;
			//var d = new NormalAddDialog {DataContext = new NormalAddViewModel()};
			//await DialogHost.Show(d);
		}

		private void AdvancedClick() {
			AdvancedAddDataContext = new AdvancedAddViewModel();
			TransVis = Visibility.Visible;
			TransInt = 1;
			//var d = new AdvancedAddDialog {DataContext = new AdvancedAddViewModel()};
			//await DialogHost.Show(d);
		}

		private void AddToBacklog() {
			var m = new MangaModel {
				Name = Name,
				Chapter = Chapter,
				Site = "backlog",
				RssLink = "placeholder",
				Date = DateTime.Now
			};
			if (Sqlite.GetMangaNameList("backlog").Contains(Name)) Sqlite.UpdateManga(m);
			else Sqlite.AddManga(m);

			Name = string.Empty;
			Chapter = string.Empty;
		}
	}
}