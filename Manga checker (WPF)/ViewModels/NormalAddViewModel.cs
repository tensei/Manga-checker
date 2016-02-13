using System;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using Manga_checker.Adding.Sites;
using Manga_checker.Database;
using Manga_checker.Handlers;
using Manga_checker.ViewModels.Model;

namespace Manga_checker.ViewModels {
    public class NormalAddViewModel : ViewModelBase {
        public NormalAddViewModel() {
            SearchCommand = new ActionCommand(Search);
            AddCommand = new ActionCommand(Add);
            Progressbar = Visibility.Collapsed;
            AddButtonVisibility = Visibility.Collapsed;
        }

        public string Link { get; set; }
        private string _infoLabel { get; set; }
        private Visibility _infoVisi { get; set; }
        private Visibility _progressbar { get; set; }
        private Visibility _addButton { get; set; }

        public Visibility Progressbar {
            get { return _progressbar; }
            set {
                if (_progressbar == value) return;
                _progressbar = value;
                OnPropertyChanged();
            }
        }

        public Visibility InfoVisi {
            get { return _infoVisi; }
            set {
                if (_infoVisi == value) return;
                _infoVisi = value;
                OnPropertyChanged();
            }
        }

        public Visibility AddButtonVisibility {
            get { return _addButton; }
            set {
                if (_addButton == value) return;
                _addButton = value;
                OnPropertyChanged();
            }
        }

        private MangaModel manga { get; set; }

        public string InfoLabel {
            get { return _infoLabel; }
            set {
                if (_infoLabel == value) return;
                _infoLabel = value;
                OnPropertyChanged();
            }
        }

        public ICommand SearchCommand { get; }
        public ICommand AddCommand { get; }

        public void Search() {
            //search stuff
            InfoLabel = "";
            Progressbar = Visibility.Visible;
            var t = new Thread(new ThreadStart(delegate {
                try {
                    //search manga here
                    if (Link.ToLower().Contains("mangareader.net")) {
                        manga = mangareader.GetInfo(Link);
                        InfoLabel = $"{manga.Name}\n{manga.Chapter}\n{manga.Site}";
                    }
                    else if (Link.ToLower().Contains("mangafox.me")) {
                        manga = mangafox.GeInfo(Link);
                        InfoLabel = $"{manga.Name}\n{manga.Chapter}\n{manga.Site}";
                    }
                    else if (Link.ToLower().Contains("readms.com") || Link.ToLower().Contains("mangastream.com")) {
                        manga = mangastream.GetInfo(Link);
                        InfoLabel = $"{manga.Name}\n{manga.Chapter}\n{manga.Site}";
                    }
                    else if (Link.ToLower().Equals(string.Empty)) {
                        manga.Error = "Link empty";
                    }
                    else if (Link.ToLower().Contains("webtoons")) {
                        manga = webtoons.GetInfo(Link);
                        InfoLabel = $"{manga.Name}\n{manga.Chapter}\n{manga.Site}";
                    }
                    else {
                        InfoLabel = "Link not recognized :/";
                    }
                }
                catch (Exception error) {
                    InfoLabel = error.Message;
                    AddButtonVisibility = Visibility.Collapsed;
                }
                Progressbar = Visibility.Collapsed;
                AddButtonVisibility = Visibility.Visible;
            })) {IsBackground = true};
            t.Start();
        }

        public void Add() {
            var name = manga.Name;
            var chapter = manga.Chapter;
            if (manga.Site.ToLower().Contains("mangareader")) {
                if (name != "ERROR" || name != "None" && chapter != "None" || chapter != "ERROR") {
                    DebugText.Write($"[Debug] Trying to add {name} {chapter}");
                    ParseFile.AddManga("mangareader", name.ToLower(), chapter, "");
                    Sqlite.AddManga("mangareader", name, chapter, "placeholder", DateTime.Now, manga.Link);
                    InfoLabel += Sqlite.AddManga("mangareader", name, chapter, "placeholder", DateTime.Now, manga.Link)
                        ? "\nSuccess!"
                        : "\nAlready in list!";
                    return;
                }
            }
            if (manga.Site.ToLower().Contains("mangafox")) {
                if (!name.Equals("ERROR") && name != "None" && chapter != "None" && chapter != "ERROR") {
                    DebugText.Write($"[Debug] Trying to add {name} {chapter}");
                    ParseFile.AddManga("mangafox", name.ToLower(), chapter, "");
                    InfoLabel += Sqlite.AddManga("mangafox", name, chapter, "placeholder", DateTime.Now, manga.Link)
                        ? "\nSuccess!"
                        : "\nAlready in list!";
                    return;
                }
            }
            if (manga.Site.ToLower().Contains("mangastream")) {
                if (!name.Equals("ERROR") && name != "None" && chapter != "None" && chapter != "ERROR") {
                    DebugText.Write($"[Debug] Trying to add {name} {chapter}");
                    ParseFile.AddManga("mangastream", name.ToLower(), chapter, "");
                    InfoLabel += Sqlite.AddManga("mangastream", name, chapter, "placeholder", manga.Date,
                        manga.Link)
                        ? "\nSuccess!"
                        : "\nAlready in list!";
                    return;
                }
            }
            InfoLabel = "failed";
            AddButtonVisibility = Visibility.Collapsed;
        }
    }
}