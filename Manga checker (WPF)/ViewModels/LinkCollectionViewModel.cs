using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Manga_checker.Database;
using Manga_checker.Handlers;

namespace Manga_checker.ViewModels {
    public class LinkCollectionViewModel : INotifyPropertyChanged {
        private readonly ObservableCollection<MangaInfoViewModel> _linksCollection = 
            new ObservableCollection<MangaInfoViewModel>(); 

        public MangaInfoViewModel SelectedItem { get; set; }

        public LinkCollectionViewModel() {
            LinkCollection = new ReadOnlyObservableCollection<MangaInfoViewModel>(_linksCollection);
            RefreshCommand = new ActionCommand(FillCollection);
            FillCollection();
        }

        public ICommand RefreshCommand { get; }
        public ReadOnlyObservableCollection<MangaInfoViewModel> LinkCollection { get; }

        public void FillCollection() {
            _linksCollection.Clear();
            foreach (var m in Sqlite.GetHistory()) {
                _linksCollection.Add(m);
                OnPropertyChanged();
            }
        }
        

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(propertyName));
        }
    }
}
