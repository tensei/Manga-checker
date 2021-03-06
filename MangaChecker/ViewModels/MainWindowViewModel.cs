﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MangaChecker.Common;
using MangaChecker.Database;
using MangaChecker.Models;
using MangaChecker.Properties;
using MangaChecker.Threads;
using MangaChecker.Windows;
using MaterialDesignThemes.Wpf;
using PropertyChanged;

namespace MangaChecker.ViewModels {
	[ImplementPropertyChanged]
	public class MainWindowViewModel : ViewModelBase {
		public static readonly ObservableCollection<MangaModel> MangasInternal =
			new ObservableCollection<MangaModel>();

		public static MainWindowViewModel Instance;

		private readonly List<string> _sites = GlobalVariables.DataGridFillSites;
		
		private ListBoxItem _selectedSite;

		private ThreadStart Childref;
		public Thread ChildThread;
		private HistoryWindow History;

		public MainWindowViewModel() {
			Instance = this;
			SnackbarMessageQueue = new SnackbarMessageQueue();
			Mangas = new ReadOnlyObservableCollection<MangaModel>(MangasInternal);
			NewMangas = new ReadOnlyObservableCollection<MangaModel>(GlobalVariables.NewMangasInternal);
			ListboxItemNames = new ReadOnlyObservableCollection<ListBoxItem>(GlobalVariables.ListboxItemNames);
			RefreshCommand = new ActionCommand(RunRefresh);
			StartStopCommand = new ActionCommand(Startstop);
			HistoryCommand = new ActionCommand(ShowHistory);
			FillListCommand = new ActionCommand(Fill_list);
			NewCommand = new ActionCommand(ShowNew);
			CloseCommand = new ActionCommand(Close);

			ThreadStatus = "[Running]";
			Fill_list();

			Childref = MainThread.CheckNow;
			ChildThread = new Thread(Childref) {IsBackground = true};
			//ChildThread.SetApartmentState(ApartmentState.STA);
			ChildThread.Start();
			Sqlite.GetMangasNotRead().ForEach(x => GlobalVariables.NewMangasInternal.Add(x));
		}

		public ReadOnlyObservableCollection<ListBoxItem> ListboxItemNames { get; }

		public PackIconKind PausePlayButtonIcon { get; set; } = PackIconKind.Pause;


		public ListBoxItem SelectedSite {
			get { return _selectedSite; }
			set {
				GetItems(value.Content.ToString());
				_selectedSite = value;
			}
		}

		public MangaModel SelectedItem { get; set; }

		public int DrawerIndex { get; set; }


		public bool MenuToggleButton { get; set; } = true;
		public ReadOnlyObservableCollection<MangaModel> Mangas { get; }
		public ReadOnlyObservableCollection<MangaModel> NewMangas { get; }

		public ICommand RefreshCommand { get; }
		public ICommand FillListCommand { get; }
		public ICommand StartStopCommand { get; }
		public ICommand HistoryCommand { get; }
		public ICommand NewCommand { get; }
		public ICommand CloseCommand { get; }

		public string ThreadStatus { get; set; }

		private bool FillingList { get; set; }

		public int SelectedIndex { get; set; }

		public SnackbarMessageQueue SnackbarMessageQueue { get; }


		private static void Close() {
			Instance = null;
			Application.Current.Shutdown();
		}

		private async void GetItems(string site) {
			if (site.ToLower().Equals("all")) {
				Fill_list();
				return;
			}
			MangasInternal.Clear();
			await GetMangas(site);
		}

		private void ShowHistory() {
			if (History != null) {
				History.Show();
			} else {
				History = new HistoryWindow {
					DataContext = new HistoryViewModel(),
					ShowActivated = false,
					Owner = Application.Current.MainWindow,
					WindowStartupLocation = WindowStartupLocation.CenterOwner
				};
				History.Show();
			}
		}

		private void ShowNew() {
			DrawerIndex = 6;
		}

		private void RunRefresh() {
			Settings.Default.ForceCheck = "force";
		}

		private void Startstop() {
			switch (ThreadStatus) {
				case "[Running]": {
					ChildThread.Abort();
					ThreadStatus = "[Stopped]";
					PausePlayButtonIcon = PackIconKind.Play;
					break;
				}
				case "[Stopped]": {
					Childref = MainThread.CheckNow;
					ChildThread = new Thread(Childref) {IsBackground = true};
					ChildThread.Start();
					ThreadStatus = "[Running]";
					PausePlayButtonIcon = PackIconKind.Pause;
					break;
				}
			}
		}

		private async Task GetMangas(string site) {
			if (FillingList) return;
			FillingList = true;
			var m = await Sqlite.GetMangasAsync(site.ToLower());
			var ordered = m.OrderByDescending(a => a.Date);
			foreach (var manga in ordered) {
				if (manga.Link.Equals("placeholder")) manga.Link = "";
				MangasInternal.Add(manga);
			}
			FillingList = false;
		}

		private async void Fill_list() {
			if (FillingList)
				return;
			FillingList = true;
			var all = new List<MangaModel>();
			foreach (var site in _sites) all.AddRange(await Sqlite.GetMangasAsync(site.ToLower()));
			var allordered = all.OrderByDescending(a => a.Date);
			MangasInternal.Clear();
			foreach (var manga in allordered) {
				if (manga.Link.Equals("placeholder")) manga.Link = "";
				MangasInternal.Add(manga);
			}
			SelectedIndex = 0;
			FillingList = false;
		}
	}
}