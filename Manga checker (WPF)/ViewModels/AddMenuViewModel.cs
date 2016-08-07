using System;
using System.Windows.Input;
using MangaChecker.Adding;
using MangaChecker.Database;
using MangaChecker.Models;
using MaterialDesignThemes.Wpf;
using PropertyChanged;

namespace MangaChecker.ViewModels {
    [ImplementPropertyChanged]
    public class AddMenuViewModel {

        public AddMenuViewModel() {
            AddBacklogCommand = new ActionCommand(AddToBacklog);
            AddNormalCommand = new ActionCommand(NormalClick);
            AddAdvancedCommand = new ActionCommand(AdvancedClick);
        }

        public string Name { get; set; }
        public string Chapter { get; set; }

        public ICommand AddBacklogCommand { get; }
        public ICommand AddNormalCommand { get; }
        public ICommand AddAdvancedCommand { get; }
        //public ICommand AddAdvancedCommand { get; }

        private static async void NormalClick() {
            var d = new NormalAddDialog {DataContext = new NormalAddViewModel()};
            await DialogHost.Show(d);
        }

        private static async void AdvancedClick() {
            var d = new AdvancedAddDialog {DataContext = new AdvancedAddViewModel()};
            await DialogHost.Show(d);
        }

        private void AddToBacklog() {
                var m = new MangaModel {
                    Name = Name,
                    Chapter = Chapter,
                    Site = "backlog",
                    RssLink = "placeholder",
                    Date = DateTime.Now
                };
            if (Sqlite.GetMangaNameList("backlog").Contains(Name)) {
                Sqlite.UpdateManga(m);
            } else {
                Sqlite.AddManga(m);
            }

            Name = string.Empty;
            Chapter = string.Empty;
        }
    }
}