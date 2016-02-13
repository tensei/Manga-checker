﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using Manga_checker.Database;
using Manga_checker.Handlers;
using Manga_checker.Properties;
using Manga_checker.Threads;
using Manga_checker.ViewModels.Model;
using MaterialDesignThemes.Wpf;

namespace Manga_checker.ViewModels {
    public class MainWindowViewModel : ViewModelBase {
        private readonly ObservableCollection<MangaModel> _mangasInternal =
            new ObservableCollection<MangaModel>();

        private Visibility _addVisibility;

        private string _currentSite;
        private Visibility _datagridVisibiliy;
        private Visibility _debugVisibility;
        private bool _menuToggle;
        private Visibility _settingsVisibility;
        private string _threadStatus;

        private ThreadStart Childref;
        private Thread ChildThread;
        public HistoryWindow History;

        public PackIconKind PausePlayButtonIcon {
            get { return _pausePlayButtonIcon1; }
            set {
                _pausePlayButtonIcon1 = value;
                OnPropertyChanged();
            }
        }

        public List<string> Sites = new List<string> {
            "Mangafox",
            "MangahereRSS",
            "Mangareader",
            "Mangastream",
            "Batoto",
            "Webtoons",
            "YoManga",
            "Kissmanga"
        };

        private PackIconKind _pausePlayButtonIcon1 = PackIconKind.Pause;

        public MainWindowViewModel() {
            Mangas = new ReadOnlyObservableCollection<MangaModel>(_mangasInternal);

            RefreshCommand = new ActionCommand(RunRefresh);
            FillMangastreamCommand = new ActionCommand(FillMangastream);
            FillYoMangaCommand = new ActionCommand(Fillyomanga);
            FillMangafoxCommand = new ActionCommand(FillMangafox);
            FillMangahereCommand = new ActionCommand(FillMangahere);
            FillMangareaderCommand = new ActionCommand(FillMangareader);
            FillWebtoonsCommand = new ActionCommand(FillWebtoons);
            FillBatotoCommand = new ActionCommand(Fillbatoto);
            FillListCommand = new ActionCommand(Fill_list);
            FillBacklogCommand = new ActionCommand(FillBacklog);
            FillKissmangaCommand = new ActionCommand(FillKissmanga);
            StartStopCommand = new ActionCommand(Startstop);
            DebugCommand = new ActionCommand(DebugClick);
            SettingsCommand = new ActionCommand(SettingClick);
            AddMangaCommand = new ActionCommand(AddMangaClick);
            HistoryCommand = new ActionCommand(ShowHistory);
            DeleteMangaCommand = new ActionCommand(Delete);

            DebugVisibility = Visibility.Collapsed;
            SettingsVisibility = Visibility.Collapsed;
            AddVisibility = Visibility.Collapsed;
            DataGridVisibility = Visibility.Visible;

            ThreadStatus = "[Running]";
            if (!File.Exists("MangaDB.sqlite")) return;
            Sqlite.UpdateDatabase();
            Fill_list();

            Childref = MainThread.CheckNow;
            ChildThread = new Thread(Childref) {IsBackground = true};
            ChildThread.SetApartmentState(ApartmentState.STA);
            ChildThread.Start();
        }


        public MangaModel SelectedItem { get; set; }

        public bool MenuToggleButton {
            get { return _menuToggle; }
            set {
                if (_menuToggle == value) return;
                _menuToggle = value;
                OnPropertyChanged();
            }
        }

        public ReadOnlyObservableCollection<MangaModel> Mangas { get; }

        public ICommand RefreshCommand { get; }
        public ICommand FillMangastreamCommand { get; }
        public ICommand FillMangareaderCommand { get; }
        public ICommand FillYoMangaCommand { get; }
        public ICommand FillMangafoxCommand { get; }
        public ICommand FillMangahereCommand { get; }
        public ICommand FillWebtoonsCommand { get; }
        public ICommand FillBacklogCommand { get; }
        public ICommand FillListCommand { get; }
        public ICommand FillBatotoCommand { get; }
        public ICommand FillKissmangaCommand { get; }
        public ICommand StartStopCommand { get; }
        public ICommand DebugCommand { get; }
        public ICommand SettingsCommand { get; }
        public ICommand AddMangaCommand { get; }
        public ICommand HistoryCommand { get; }
        public ICommand DeleteMangaCommand { get; }

        public string CurrentSite {
            get { return _currentSite; }
            set {
                if (_currentSite == value) return;
                _currentSite = value;
                OnPropertyChanged();
            }
        }

        public string ThreadStatus {
            get { return _threadStatus; }
            set {
                if (_threadStatus == value) return;
                _threadStatus = value;
                OnPropertyChanged();
            }
        }

        public Visibility DataGridVisibility {
            get { return _datagridVisibiliy; }
            set {
                if (_datagridVisibiliy == value) return;
                _datagridVisibiliy = value;
                OnPropertyChanged();
            }
        }

        public Visibility DebugVisibility {
            get { return _debugVisibility; }
            set {
                if (_debugVisibility == value) return;
                _debugVisibility = value;
                OnPropertyChanged();
            }
        }

        public Visibility SettingsVisibility {
            get { return _settingsVisibility; }
            set {
                if (_settingsVisibility == value) return;
                _settingsVisibility = value;
                OnPropertyChanged();
            }
        }

        public Visibility AddVisibility {
            get { return _addVisibility; }
            set {
                if (_addVisibility == value) return;
                _addVisibility = value;
                OnPropertyChanged();
            }
        }

        public void ShowHistory() {
            if (History != null) {
                History.Show();
            }
            else {
                History = new HistoryWindow {
                    DataContext = new HistoryViewModel(),
                    ShowActivated = false,
                    Owner = Application.Current.MainWindow,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };
                History.Show();
            }
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

        private void GetMangas(string site) {
            CurrentSite = site;
            SettingsVisibility = Visibility.Collapsed;
            AddVisibility = Visibility.Collapsed;
            DebugVisibility = Visibility.Collapsed;
            DataGridVisibility = Visibility.Visible;
            foreach (var manga in Sqlite.GetMangas(site.ToLower())) {
                if (manga.Link.Equals("placeholder")) {
                    manga.Link = "";
                }
                _mangasInternal.Add(manga);
            }
        }

        private void FillMangastream() {
            _mangasInternal.Clear();
            GetMangas("Mangastream");
        }

        private void FillMangareader() {
            _mangasInternal.Clear();
            GetMangas("Mangareader");
        }

        private void Fillbatoto() {
            _mangasInternal.Clear();
            GetMangas("Batoto");
        }

        private void FillMangafox() {
            _mangasInternal.Clear();
            GetMangas("Mangafox");
        }

        private void FillMangahere() {
            _mangasInternal.Clear();
            GetMangas("MangahereRSS");
        }

        private void FillBacklog() {
            _mangasInternal.Clear();
            GetMangas("Backlog");
        }

        public void FillWebtoons() {
            _mangasInternal.Clear();
            GetMangas("Webtoons");
        }

        public void Fillyomanga() {
            _mangasInternal.Clear();
            GetMangas("YoManga");
        }

        public void FillKissmanga() {
            _mangasInternal.Clear();
            GetMangas("Kissmanga");
        }

        public void Fill_list() {
            MenuToggleButton = false;
            _mangasInternal.Clear();
            foreach (var site in Sites) {
                GetMangas(site);
            }
            CurrentSite = "All";
        }

        public void DebugClick() {
            CurrentSite = "Debug";
            DebugVisibility = Visibility.Visible;
            DataGridVisibility = Visibility.Collapsed;
            SettingsVisibility = Visibility.Collapsed;
            AddVisibility = Visibility.Collapsed;
        }

        public void SettingClick() {
            MenuToggleButton = false;
            CurrentSite = "Settings";
            DebugVisibility = Visibility.Collapsed;
            DataGridVisibility = Visibility.Collapsed;
            SettingsVisibility = Visibility.Visible;
            AddVisibility = Visibility.Collapsed;
        }

        public void AddMangaClick() {
            MenuToggleButton = false;
            CurrentSite = "Add Manga";
            DebugVisibility = Visibility.Collapsed;
            DataGridVisibility = Visibility.Collapsed;
            SettingsVisibility = Visibility.Collapsed;
            AddVisibility = Visibility.Visible;
        }

        public async void Delete() {
            var su = await Tools.Delete(SelectedItem);
            if (su)
                _mangasInternal.Remove(SelectedItem);
        }
    }
}