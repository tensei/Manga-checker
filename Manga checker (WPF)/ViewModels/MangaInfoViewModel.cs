using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace Manga_checker.ViewModels {

    public class MangaInfoViewModel : INotifyPropertyChanged{
        public int Id { get; set; }
        public string Name { get; set; }
        public string Chapter { get; set; }
        public string Site { get; set; }
        public string Error { get; set; }
        public string Link { get; set; }
        public string RSS_Link { get; set; }
        public string Date { get; set; }

        public string FullName { get; set; }

        public ObservableCollection<Button> _buttons = new ObservableCollection<Button>();

        public bool IsEnabled { get; set; } = true;
        public ReadOnlyObservableCollection<Button> Buttons { get; }
        public Visibility Popup { get; set; } = Visibility.Hidden;

        public MangaInfoViewModel() {
            Buttons = new ReadOnlyObservableCollection<Button>(_buttons);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(propertyName));
        }
    }
}