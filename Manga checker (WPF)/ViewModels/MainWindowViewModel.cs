using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Manga_checker.Common;
using Manga_checker.Database;
using Manga_checker.Models;
using Manga_checker.Properties;
using Manga_checker.Threads;
using Manga_checker.Windows;
using MaterialDesignThemes.Wpf;
using PropertyChanged;
using MessageBox = System.Windows.Forms.MessageBox;

namespace Manga_checker.ViewModels {
    [ImplementPropertyChanged]
    public class MainWindowViewModel : ViewModelBase {
        public static readonly ObservableCollection<MangaModel> MangasInternal =
            new ObservableCollection<MangaModel>();

        public static string _currentSite;

        private readonly List<string> _sites = GlobalVariables.DataGridFillSites;

        private Visibility _addVisibility;
        private Visibility _datagridVisibiliy;
        private Visibility _debugVisibility;
        private bool _menuToggle;

        private PackIconKind _pausePlayButtonIcon1 = PackIconKind.Pause;
        private ListBoxItem _selectedSite;
        private Visibility _settingsVisibility;
        private string _threadStatus;

        private ThreadStart Childref;
        private Thread ChildThread;
        private HistoryWindow History;

        public MainWindowViewModel() {
            Mangas = new ReadOnlyObservableCollection<MangaModel>(MangasInternal);
            NewMangas = new ReadOnlyObservableCollection<MangaModel>(GlobalVariables.NewMangasInternal);
            ListboxItemNames = new ReadOnlyObservableCollection<ListBoxItem>(GlobalVariables.ListboxItemNames);
            RefreshCommand = new ActionCommand(RunRefresh);
            StartStopCommand = new ActionCommand(Startstop);
            DebugCommand = new ActionCommand(DebugClick);
            SettingsCommand = new ActionCommand(SettingClick);
            AddMangaCommand = new ActionCommand(AddMangaClick);
            HistoryCommand = new ActionCommand(ShowHistory);
            FillListCommand = new ActionCommand(Fill_list);
            NewCommand = new ActionCommand(ShowNew);

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

        public ReadOnlyObservableCollection<ListBoxItem> ListboxItemNames { get; }

        public PackIconKind PausePlayButtonIcon {
            get { return _pausePlayButtonIcon1; }
            set {
                _pausePlayButtonIcon1 = value;
                OnPropertyChanged();
            }
        }


        public ListBoxItem SelectedSite {
            get { return _selectedSite; }
            set {
                getItems(value.Content.ToString());
                _selectedSite = value;
            }
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
        public ReadOnlyObservableCollection<MangaModel> NewMangas { get; }

        public ICommand RefreshCommand { get; }
        public ICommand FillListCommand { get; }
        public ICommand StartStopCommand { get; }
        public ICommand DebugCommand { get; }
        public ICommand SettingsCommand { get; }
        public ICommand AddMangaCommand { get; }
        public ICommand HistoryCommand { get; }
        public ICommand NewCommand { get; }

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

        public bool FillingList { get; set; }

        public int SelectedIndex { get; set; }

        public int SelectedIndexTransitioner { get; set; }

        private async void getItems(string site) {
            if (site == "DEBUG") {
                DebugClick();
                return;
            }
            if (site.ToLower().Equals("all")) {
                MangasInternal.Clear();
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
            SelectedIndexTransitioner = 4;
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
            FillingList = false;
        }

        private async void Fill_list() {
            SelectedIndexTransitioner = 0;
            MenuToggleButton = false;
            MangasInternal.Clear();
            foreach (var site in _sites) {
                await GetMangas(site);
            }
            CurrentSite = "All";
            SelectedIndex = 0;
        }

        private void DebugClick() {
            SelectedIndexTransitioner = 1;
            CurrentSite = "Debug";
            DebugVisibility = Visibility.Visible;
            DataGridVisibility = Visibility.Collapsed;
            SettingsVisibility = Visibility.Collapsed;
            AddVisibility = Visibility.Collapsed;
        }

        private void SettingClick() {
            SelectedIndexTransitioner = 3;
            MenuToggleButton = false;
            CurrentSite = "Settings";
            DebugVisibility = Visibility.Collapsed;
            DataGridVisibility = Visibility.Collapsed;
            SettingsVisibility = Visibility.Visible;
            AddVisibility = Visibility.Collapsed;
        }

        private void AddMangaClick() {
            SelectedIndexTransitioner = 2;
            MenuToggleButton = false;
            CurrentSite = "Add Manga";
            DebugVisibility = Visibility.Collapsed;
            DataGridVisibility = Visibility.Collapsed;
            SettingsVisibility = Visibility.Collapsed;
            AddVisibility = Visibility.Visible;
        }
    }
}