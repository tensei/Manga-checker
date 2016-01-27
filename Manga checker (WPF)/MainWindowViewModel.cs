using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Manga_checker.Database;
using Manga_checker.Handlers;
using Manga_checker.Properties;
using Manga_checker.Threads;
using Manga_checker.ViewModels;

namespace Manga_checker {
    public class MainWindowViewModel : INotifyPropertyChanged {
        private readonly ObservableCollection<MangaItemViewModel> _mangasInternal =
            new ObservableCollection<MangaItemViewModel>();

        private Visibility _addVisibility;

        private string _currentSite;
        private Visibility _datagridVisibiliy;
        private Visibility _debugVisibility;
        private Visibility _settingsVisibility;
        private string _threadStatus;

        private ThreadStart Childref;
        private Thread ChildThread;

        public List<string> Sites = new List<string> {
            "Mangafox",
            "Mangareader",
            "Mangastream",
            "Batoto",
            "Webtoons",
            "YoManga",
            "Kissmanga"
        };

        public MainWindowViewModel() {
            Mangas = new ReadOnlyObservableCollection<MangaItemViewModel>(_mangasInternal);

            RefreshCommand = new ActionCommand(RunRefresh);
            FillMangastreamCommand = new ActionCommand(FillMangastream);
            FillYoMangaCommand = new ActionCommand(Fillyomanga);
            FillMangafoxCommand = new ActionCommand(FillMangafox);
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
            //TODO run on a background thread, add spinner etc
            Fill_list();

            DebugVisibility = Visibility.Collapsed;
            SettingsVisibility = Visibility.Collapsed;
            AddVisibility = Visibility.Collapsed;
            DataGridVisibility = Visibility.Visible;

            Childref = MainThread.CheckNow;
            ChildThread = new Thread(Childref) {IsBackground = true};
            ChildThread.Start();
            ThreadStatus = "[Running]";
            if (!File.Exists("MangaDB.sqlite")) {
                Sqlite.SetupDatabase();
                if (File.Exists("manga.json")) {
                    Sqlite.PopulateDb();
                }
            }
            //Sqlite.test();
        }

        public ReadOnlyObservableCollection<MangaItemViewModel> Mangas { get; }

        public ICommand RefreshCommand { get; }
        public ICommand FillMangastreamCommand { get; }
        public ICommand FillMangareaderCommand { get; }
        public ICommand FillYoMangaCommand { get; }
        public ICommand FillMangafoxCommand { get; }
        public ICommand FillWebtoonsCommand { get; }
        public ICommand FillBacklogCommand { get; }
        public ICommand FillListCommand { get; }
        public ICommand FillBatotoCommand { get; }
        public ICommand FillKissmangaCommand { get; }
        public ICommand StartStopCommand { get; }
        public ICommand DebugCommand { get; }
        public ICommand SettingsCommand { get; }
        public ICommand AddMangaCommand { get; }

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

        public event PropertyChangedEventHandler PropertyChanged;

        private void RunRefresh() {
            Settings.Default.ForceCheck = "force";
        }

        private void Startstop() {
            switch (ThreadStatus) {
                case "[Running]": {
                    ChildThread.Abort();
                    ThreadStatus = "[Stopped]";
                    break;
                }
                case "[Stopped]": {
                    Childref = MainThread.CheckNow;
                    ChildThread = new Thread(Childref) {IsBackground = true};
                    ChildThread.Start();
                    ThreadStatus = "[Running]";
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
            var gg = new List<string> {"mangafox", "mangareader"};
            foreach (var manga in Sqlite.GetMangas(site.ToLower())) {
                if (manga.Link.Equals("placeholder")) {
                    manga.Link = "";
                }
                var listBoxItem = new MangaItemViewModel {
                    Name = manga.Name,
                    Chapter = manga.Chapter,
                    Site = site,
                    Link = manga.Link
                };
                if (gg.Contains(manga.Site.ToLower())) {
                    listBoxItem.Popup = Visibility.Visible;

                    for (var i = 1; i < 4; i++) {
                        if (manga.Chapter.Contains(" ")) {
                            manga.Chapter = manga.Chapter.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries)[0];
                        }

                        var ch = int.Parse(manga.Chapter);
                        ch = ch - i;
                        var button = new Button {Content = $"{manga.Name} {ch}"};
                        button.Click +=
                            (sender, e) => OpenSite.Open(site, manga.Name, ch.ToString(), new List<string>());
                        listBoxItem._buttons.Add(button);
                    }
                }
                else {
                    var button = new Button {Content = "TODO!", IsEnabled = false};
                    listBoxItem._buttons.Add(button);
                }

                _mangasInternal.Add(listBoxItem);
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
            CurrentSite = "Settings";
            DebugVisibility = Visibility.Collapsed;
            DataGridVisibility = Visibility.Collapsed;
            SettingsVisibility = Visibility.Visible;
            AddVisibility = Visibility.Collapsed;
        }

        public void AddMangaClick() {
            CurrentSite = "Add Manga";
            DebugVisibility = Visibility.Collapsed;
            DataGridVisibility = Visibility.Collapsed;
            SettingsVisibility = Visibility.Collapsed;
            AddVisibility = Visibility.Visible;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}