using System;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using MangaChecker.Adding.Sites;
using MangaChecker.Common;
using MangaChecker.Database;
using MangaChecker.Models;
using PropertyChanged;

namespace MangaChecker.ViewModels {
    [ImplementPropertyChanged]
    public class NormalAddViewModel : ViewModelBase {
        public NormalAddViewModel() {
            SearchCommand = new ActionCommand(Search);
            AddCommand = new ActionCommand(Add);
            Progressbar = Visibility.Collapsed;
            AddButtonVisibility = Visibility.Collapsed;
        }

        public string Link { get; set; }

        public Visibility Progressbar { get; set; }

        public Visibility InfoVisi { get; set; }

        public Visibility AddButtonVisibility { get; set; }

        private MangaModel manga { get; set; }

        public string InfoLabel { get; set; }

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
                    } else if (Link.ToLower().Contains("mangafox.me")) {
                        manga = mangafox.GeInfo(Link);
                        InfoLabel = $"{manga.Name}\n{manga.Chapter}\n{manga.Site}";
                    } else if (Link.ToLower().Contains("readms.com") || Link.ToLower().Contains("mangastream.com")) {
                        manga = mangastream.GetInfo(Link);
                        InfoLabel = $"{manga.Name}\n{manga.Chapter}\n{manga.Site}";
                    } else if (Link.ToLower().Equals(string.Empty)) {
                        manga.Error = "Link empty";
                    } else if (Link.ToLower().Contains("webtoons")) {
                        manga = webtoons.GetInfo(Link);
                        InfoLabel = $"{manga.Name}\n{manga.Chapter}\n{manga.Site}";
                    } else {
                        InfoLabel = "Link not recognized :/";
                    }
                } catch (Exception error) {
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

                    InfoLabel += Sqlite.AddManga(manga)
                        ? "\nSuccess!"
                        : "\nAlready in list!";
                    return;
                }
            }
            if (manga.Site.ToLower().Contains("mangafox")) {
                if (!name.Equals("ERROR") && name != "None" && chapter != "None" && chapter != "ERROR") {
                    DebugText.Write($"[Debug] Trying to add {name} {chapter}");
                    InfoLabel += Sqlite.AddManga(manga)
                        ? "\nSuccess!"
                        : "\nAlready in list!";
                    return;
                }
            }
            if (manga.Site.ToLower().Contains("mangastream")) {
                if (!name.Equals("ERROR") && name != "None" && chapter != "None" && chapter != "ERROR") {
                    DebugText.Write($"[Debug] Trying to add {name} {chapter}");
                    InfoLabel += Sqlite.AddManga(manga)
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