using System.Collections.ObjectModel;
using System.Windows.Input;
using Manga_checker.Database;

namespace Manga_checker.ViewModels {
    public class HistoryViewModel : ViewModelBase {
        private readonly ObservableCollection<MangaViewModel> _linksCollection =
            new ObservableCollection<MangaViewModel>();

        public HistoryViewModel() {
            LinkCollection = new ReadOnlyObservableCollection<MangaViewModel>(_linksCollection);
            RefreshCommand = new ActionCommand(FillCollection);
            FillCollection();
        }

        public MangaViewModel SelectedItem { get; set; }

        public ICommand RefreshCommand { get; }
        public ReadOnlyObservableCollection<MangaViewModel> LinkCollection { get; }

        public void FillCollection() {
            _linksCollection.Clear();
            foreach (var m in Sqlite.GetHistory()) {
                _linksCollection.Add(m);
                OnPropertyChanged();
            }
        }
    }
}