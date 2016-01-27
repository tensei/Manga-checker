using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Manga_checker.ViewModels {
    public class MangaItemViewModel {
        public ObservableCollection<Button> _buttons = new ObservableCollection<Button>();

        public MangaItemViewModel() {
            Buttons = new ReadOnlyObservableCollection<Button>(_buttons);
        }

        public string Site { get; set; }
        public string Name { get; set; }
        public string Chapter { get; set; }
        public bool IsEnabled { get; set; } = true;
        public string FullName { get; set; }
        public string Link { get; set; }
        public ReadOnlyObservableCollection<Button> Buttons { get; }
        public Visibility Popup { get; set; } = Visibility.Hidden;
    }
}