using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Manga_checker.Common;

namespace Manga_checker.ViewModels.Model {
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
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public string Chapter {
            get { return _chapterInternal; }
            set {
                if (_chapterInternal == value) return;
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

        public Visibility ViewVisibility { get; set; } = Visibility.Collapsed;

        public ICommand MinusChapterCommand { get; }
        public ICommand PlusChapterCommand { get; }
        public ICommand RefreshMangaCommand { get; }
        public ICommand ViewCommand { get; }
        public ICommand DeleteMangaCommand { get; }


        private async void Delete() {
            var su = await Tools.Delete(this);
            if (su)
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
                var ChildThread = new Thread(() => Tools.RefreshManga(this)) {IsBackground = true};
                ChildThread.Start();
            } catch {
                //ignored
            }
        }

        private void View() {
            MangaViewer w = new MangaViewer {
                link = Link,
                DataContext = new MangaViewerViewModel { Link = this.Link }
            };
            w.ShowDialog();
        }

        private List<Button> PopulateButtons() {
            var gg = new List<string> {"mangafox", "mangareader", "mangahere"};
            var list = new List<Button>();
            if (gg.Contains(Site.ToLower())) {
                for (var i = 0; i < 3; i++) {
                    if (Chapter.Contains(" ")) {
                        Chapter = Chapter.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries)[0];
                    }

                    var ch = int.Parse(Chapter);
                    ch = ch - i;
                    var button = new Button {
                        Content = $"{ch}",
                        Style = (Style) Application.Current.FindResource("MaterialDesignFlatButton"),
                        Height = 30
                    };
                    button.Click +=
                        (sender, e) => OpenSite.Open(Site, Name, ch.ToString(), new List<string>());
                    list.Add(button);
                }
            } else {
                Separator = Visibility.Collapsed;
            }
            var viewerEnabled = new List<string> {"yomanga", "mangastream"};
            if (viewerEnabled.Contains(Site.ToLower()) && Link != "placeholder" && Link != "") ViewVisibility = Visibility.Visible;
            return list;
        }
    }
}