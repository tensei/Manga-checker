using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using MessageBox = System.Windows.Forms.MessageBox;

namespace MangaChecker.ViewModels {
    [ImplementPropertyChanged]
    public class MainWindowViewModel : ViewModelBase {
        public static readonly ObservableCollection<MangaModel> MangasInternal =
            new ObservableCollection<MangaModel>();


        private readonly List<string> _sites = GlobalVariables.DataGridFillSites;

        private ThreadStart Childref;
        private Thread ChildThread;
        private HistoryWindow History;
        private ListBoxItem _selectedSite;

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

            ThreadStatus = "[Running]";
            Fill_list();

            Childref = MainThread.CheckNow;
            ChildThread = new Thread(Childref) {IsBackground = true};
            ChildThread.SetApartmentState(ApartmentState.STA);
            ChildThread.Start();
            Sqlite.GetMangasNotRead().ForEach(x => GlobalVariables.NewMangasInternal.Add(x));
        }

        public ReadOnlyObservableCollection<ListBoxItem> ListboxItemNames { get; }

        public PackIconKind PausePlayButtonIcon { get; set; } = PackIconKind.Pause;


        public ListBoxItem SelectedSite {
            get { return _selectedSite; }
            set {
                getItems(value.Content.ToString());
                _selectedSite = value;
            }
        }

        public MangaModel SelectedItem { get; set; }

        public bool MenuToggleButton { get; set; }

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

        public string CurrentSite { get; set; }

        public string ThreadStatus { get; set; }

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
        }

        private void SettingClick() {
            SelectedIndexTransitioner = 3;
            MenuToggleButton = false;
            CurrentSite = "Settings";
        }

        private void AddMangaClick() {
            SelectedIndexTransitioner = 2;
            MenuToggleButton = false;
            CurrentSite = "Add Manga";
        }
    }
}