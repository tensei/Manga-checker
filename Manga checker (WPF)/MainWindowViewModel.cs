using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Manga_checker.Classes;
using Manga_checker.Handlers;
using Manga_checker.Properties;
using Manga_checker.Threads;
using Manga_checker.ViewModels;

namespace Manga_checker
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly ObservableCollection<MangaItemViewModel> _mangasInternal =
            new ObservableCollection<MangaItemViewModel>();

        private string _currentSite;
        private string _threadStatus;

        private ThreadStart Childref;
        private Thread ChildThread;

        public MainWindowViewModel()
        {
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
            //TODO run on a background thread, add spinner etc
            Fill_list();


            Childref = MainThread.CheckNow;
            ChildThread = new Thread(Childref) { IsBackground = true };
            ChildThread.Start();
            ThreadStatus = "Running.";
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

        public string CurrentSite
        {
            get { return _currentSite; }
            set
            {
                if (_currentSite == value) return;
                _currentSite = value;
                OnPropertyChanged();
            }
        }
        public string ThreadStatus
        {
            get { return _threadStatus; }
            set
            {
                if (_threadStatus == value) return;
                _threadStatus = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RunRefresh()
        {
            Settings.Default.ForceCheck = "force";
        }

        private void Startstop()
        {
            switch (ThreadStatus)
            {
                case "Running.":
                {
                    ChildThread.Abort();
                    ThreadStatus = "Stopped.";
                    break;
                }
                case "Stopped.":
                {
                    Childref = MainThread.CheckNow;
                    ChildThread = new Thread(Childref) { IsBackground = true };
                    ChildThread.Start();
                    ThreadStatus = "Running.";
                    break;
                }
            }
        }

        private void GetMangas(string site)
        {
            CurrentSite = site;
            foreach (var manga in ParseFile.GetManga(site.ToLower()))
            {

                var chna = manga.Split(new[] { "[]" }, StringSplitOptions.None);
                var listBoxItem = new MangaItemViewModel
                {
                    Name = chna[0],
                    Chapter = chna[1],
                    Site = site
                };
                _mangasInternal.Add(listBoxItem);
            }
        }

        private void FillMangastream()
        {
            _mangasInternal.Clear();
            GetMangas("Mangastream");
        }
        private void FillMangareader()
        {
            _mangasInternal.Clear();
            GetMangas("Mangareader");
        }
        private void Fillbatoto()
        {
            _mangasInternal.Clear();
            GetMangas("Batoto");
        }
        private void FillMangafox()
        {
            _mangasInternal.Clear();
            GetMangas("Mangafox");
        }

        private void FillBacklog()
        {
            _mangasInternal.Clear();
            GetMangas("Backlog");
        }

        public void FillWebtoons()
        {
            _mangasInternal.Clear();
            GetMangas("Webtoons");
        }
        public void Fillyomanga()
        {
            _mangasInternal.Clear();
            GetMangas("YoManga");
        }

        public void FillKissmanga()
        {
            _mangasInternal.Clear();
            GetMangas("Kissmanga");
        }

        public List<string> Sites = new List<string> { "Mangafox", "Mangareader", "Mangastream", "Batoto", "Webtoons", "YoManga", "Kissmanga" }; 

        public void Fill_list()
        {
            _mangasInternal.Clear();
            foreach (var site in Sites)
            {
                GetMangas(site);
            }
            CurrentSite = "All";

        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
