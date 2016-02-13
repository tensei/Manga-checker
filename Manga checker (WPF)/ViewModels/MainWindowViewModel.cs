using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
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
        public static readonly ObservableCollection<MangaModel> MangasInternal =
            new ObservableCollection<MangaModel>();

        private Visibility _addVisibility;

        public static string _currentSite;
        private Visibility _datagridVisibiliy;
        private Visibility _debugVisibility;
        private bool _menuToggle;
        private Visibility _settingsVisibility;
        private string _threadStatus;

        private ThreadStart Childref;
        private Thread ChildThread;
        private HistoryWindow History;

        public PackIconKind PausePlayButtonIcon {
            get { return _pausePlayButtonIcon1; }
            set {
                _pausePlayButtonIcon1 = value;
                OnPropertyChanged();
            }
        }

        private readonly List<string> _sites = new List<string> {
            "Mangafox",
            "Mangahere",
            "Mangareader",
            "Mangastream",
            "Batoto",
            "Webtoons",
            "YoManga",
            "Kissmanga"
        };

        private PackIconKind _pausePlayButtonIcon1 = PackIconKind.Pause;

        public MainWindowViewModel() {
            Mangas = new ReadOnlyObservableCollection<MangaModel>(MangasInternal);

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

        private void ShowHistory() {
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

        private async Task GetMangas(string site) {
            CurrentSite = site;
            SettingsVisibility = Visibility.Collapsed;
            AddVisibility = Visibility.Collapsed;
            DebugVisibility = Visibility.Collapsed;
            DataGridVisibility = Visibility.Visible;
            foreach (var manga in await Sqlite.GetMangasAsync(site.ToLower())) {
                if (manga.Link.Equals("placeholder")) {
                    manga.Link = "";
                }
                MangasInternal.Add(manga);
            }
        }

        private async void FillMangastream() {
            MangasInternal.Clear();
            await GetMangas("Mangastream");
        }

        private async void FillMangareader() {
            MangasInternal.Clear();
            await GetMangas("Mangareader");
        }

        private async void Fillbatoto() {
            MangasInternal.Clear();
            await GetMangas("Batoto");
        }

        private async void FillMangafox() {
            MangasInternal.Clear();
            await GetMangas("Mangafox");
        }

        private async void FillMangahere() {
            MangasInternal.Clear();
            await GetMangas("Mangahere");
        }

        private async void FillBacklog() {
            MangasInternal.Clear();
            await GetMangas("Backlog");
        }

        private async void FillWebtoons() {
            MangasInternal.Clear();
            await GetMangas("Webtoons");
        }

        private async void Fillyomanga() {
            MangasInternal.Clear();
            await GetMangas("YoManga");
        }

        private async void FillKissmanga() {
            MangasInternal.Clear();
            await GetMangas("Kissmanga");
        }

        private async void Fill_list() {
            MenuToggleButton = false;
            MangasInternal.Clear();
            foreach (var site in _sites) {
                await GetMangas(site);
            }
            CurrentSite = "All";
        }

        private void DebugClick() {
            CurrentSite = "Debug";
            DebugVisibility = Visibility.Visible;
            DataGridVisibility = Visibility.Collapsed;
            SettingsVisibility = Visibility.Collapsed;
            AddVisibility = Visibility.Collapsed;
        }

        private void SettingClick() {
            MenuToggleButton = false;
            CurrentSite = "Settings";
            DebugVisibility = Visibility.Collapsed;
            DataGridVisibility = Visibility.Collapsed;
            SettingsVisibility = Visibility.Visible;
            AddVisibility = Visibility.Collapsed;
        }

        private void AddMangaClick() {
            MenuToggleButton = false;
            CurrentSite = "Add Manga";
            DebugVisibility = Visibility.Collapsed;
            DataGridVisibility = Visibility.Collapsed;
            SettingsVisibility = Visibility.Collapsed;
            AddVisibility = Visibility.Visible;
        }

        private async void Delete() {
            var su = await Tools.Delete(SelectedItem);
            if (su)
                MangasInternal.Remove(SelectedItem);
        }
    }
}