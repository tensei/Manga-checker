using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Manga_checker.Utilities;

namespace Manga_checker.ViewModels {
    [ImplementPropertyChanged]
    public class MangaViewerViewModel : ViewModelBase{
        public static readonly ObservableCollection<Image> ImagesInternal =
            new ObservableCollection<Image>();

        private BitmapImage _image;
        private Visibility _fetchvis;
        private int _offset;
        private Visibility _canvas;
        public string Link { get; set; }

        public Visibility Canvas {
            get { return _canvas; }
            set { _canvas = value; }
        }
        
        public Visibility fetchvis {
            get { return _fetchvis; }
            set { _fetchvis = value; }
        }

        public MangaViewerViewModel() {
            Images = new ReadOnlyObservableCollection<Image>(ImagesInternal);
            DebugText.Write(Link +"gggggggggg\n");

            show = new ActionCommand(FillImages);
            Canvas = Visibility.Collapsed;
            fetchvis = Visibility.Visible;
            ImagesInternal.Clear();
        }


        public ICommand show { get; }

        private void FillImages() {
            ImagesInternal.Clear();
            List<string> x = new List<string>();
            x = SiteSelector(Link);
            foreach (var link in x) {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(link, UriKind.Absolute);
                bitmap.EndInit();
                bitmap.DecodePixelWidth = 200;
                MangaViewerViewModel.ImagesInternal.Add(new Image{ Source = bitmap });
                Canvas = Visibility.Visible;
                fetchvis = Visibility.Collapsed;
            }
        }

        private List<string> SiteSelector(string link) {
            var site = new List<string>();
            if (link.Contains("yomanga")) {
                site = ImageLinkCollecter.YomangaCollectLinks(Link);
            }
            if (link.Contains("mangastream") || link.Contains("readms")) {
                site = ImageLinkCollecter.MangastreamCollectLinks(Link);
            }

            return site;
        }

        public ReadOnlyObservableCollection<Image> Images { get; }

        public BitmapImage Image {
            get { return _image; }
            set { _image = value; }
        }
       
    }
}
