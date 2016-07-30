using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Manga_checker.Common;
using Manga_checker.ViewModels;
using Manga_checker.Windows;

namespace Manga_checker.Models {
    public class MangaModel : ViewModelBase {
        private string _chapterInternal;
        private DateTime _date;
        private int _new;

        public MangaModel() {
            MinusChapterCommand = new ActionCommand(ChapterMinus);
            PlusChapterCommand = new ActionCommand(ChapterPlus);
            RefreshMangaCommand = new ActionCommand(Refresh);
            ViewCommand = new ActionCommand(View);
            DeleteMangaCommand = new ActionCommand(Delete);
            RemoveNewCommand = new ActionCommand(RemoveFromNewList);
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public string Chapter {
            get { return _chapterInternal; }
            set {
                if(_chapterInternal == value)
                    return;
                _chapterInternal = value;
                OnPropertyChanged();
            }
        }

        public string Site { get; set; }
        public string Error { get; set; }
        public string Link { get; set; }
        public string RssLink { get; set; }

        public DateTime Date {
            get { return _date; }
            set {
                _date = value;
                OnPropertyChanged();
            }
        }

        public string FullName => $"{Name} {Chapter}";

        //public ObservableCollection<Button> _buttons = new ObservableCollection<Button>();

        public int New {
            get { return _new; }
            set {
                _new = value;
                OnPropertyChanged();
            }
        }

        public List<Button> Buttons => PopulateButtons();
        public Visibility Separator { get; set; } = Visibility.Visible;

        public Visibility ViewVisibility => setNewVisibility();

        public ICommand MinusChapterCommand { get; }
        public ICommand PlusChapterCommand { get; }
        public ICommand RefreshMangaCommand { get; }
        public ICommand ViewCommand { get; }
        public ICommand DeleteMangaCommand { get; }
        public ICommand RemoveNewCommand { get; }

        private void RemoveFromNewList() {
            GlobalVariables.NewMangasInternal.Remove(this);
        }

        private async void Delete() {
            var su = await Tools.Delete(this);
            if(su)
                MainWindowViewModel.MangasInternal.Remove(this);
        }

        private void ChapterMinus() {
            Tools.ChangeChaperNum(this, "-");
        }

        private void ChapterPlus() {
            Tools.ChangeChaperNum(this, "+");
        }

        private void Refresh() {
            try {
                var childThread = new Thread(() => Tools.RefreshManga(this)) { IsBackground = true };
                childThread.Start();
            } catch {
                //ignored
            }
        }

        private void View() {
            var w = new MangaViewer {
                link = Link,
                DataContext = new MangaViewerViewModel { Link = Link }
            };
            w.ShowDialog();
        }

        private Visibility setNewVisibility() {
            if(GlobalVariables.ViewerEnabled.Contains(Site.ToLower()) && Link != "placeholder") {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        private List<Button> PopulateButtons() {
            var gg = new List<string> { "mangafox", "mangareader", "mangahere" };
            var list = new List<Button>();
            if(gg.Contains(Site.ToLower())) {
                for(var i = 0; i < 3; i++) {
                    if(Chapter.Contains(" ")) {
                        Chapter = Chapter.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries)[0];
                    }

                    var ch = int.Parse(Chapter);
                    ch = ch - i;
                    var button = new Button {
                        Content = $"{ch}",
                        Style = (Style)Application.Current.FindResource("MaterialDesignFlatButton"),
                        Height = 30
                    };
                    button.Click +=
                        (sender, e) => OpenSite.Open(Site, Name, ch.ToString(), new List<string>());
                    list.Add(button);
                }
            } else {
                Separator = Visibility.Collapsed;
            }
            return list;
        }
    }
}