using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Manga_checker.Database;
using Manga_checker.Handlers;

namespace Manga_checker.ViewModels {
    public class MangaModel : ViewModelBase {
        private string _chapterInternal;
        private DateTime _date;
        private bool _new = false;

        public MangaModel() {
            MinusChapterCommand = new ActionCommand(ChapterMinus);
            PlusChapterCommand = new ActionCommand(ChapterPlus);
            RefreshMangaCommand = new ActionCommand(Refresh);
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

        public bool New {
            get { return _new; }
            set {
                _new = value; 
                OnPropertyChanged();
            }
        }

        public List<Button> Buttons => PopulateButtons();
        public Visibility Separator { get; set; } = Visibility.Visible;


        public ICommand MinusChapterCommand { get; }
        public ICommand PlusChapterCommand { get; }
        public ICommand RefreshMangaCommand { get; }

        public void ChapterMinus() {
            Tools.ChangeChaperNum(this, "-");
        }

        public void ChapterPlus() {
            Tools.ChangeChaperNum(this, "+");
        }

        public void Refresh() {
            try {
                Tools.RefreshManga(this);
            }
            catch {
                //ignored
            }
        }

        public List<Button> PopulateButtons() {
            var gg = new List<string> {"mangafox", "mangareader"};
            var list = new List<Button>();
            if (gg.Contains(Site.ToLower())) {
                for (var i = 0; i < 3; i++) {
                    if (Chapter.Contains(" ")) {
                        Chapter = Chapter.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries)[0];
                    }

                    var ch = int.Parse(Chapter);
                    ch = ch - i;
                    var button = new Button {
                        Content = $"{Name} {ch}",
                        Style = (Style) Application.Current.FindResource("MaterialDesignFlatButton"),
                        Height = 30
                    };
                    button.Click +=
                        (sender, e) => OpenSite.Open(Site, Name, ch.ToString(), new List<string>());
                    list.Add(button);
                }
            }
            else {
                Separator = Visibility.Collapsed;
            }
            return list;
        }
    }
}