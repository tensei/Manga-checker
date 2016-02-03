using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Manga_checker.ViewModels;

namespace Manga_checker {
    /// <summary>
    /// Interaktionslogik für LinkCollectionWindow.xaml
    /// </summary>
    public partial class LinkCollectionWindow : Window {
        public LinkCollectionWindow() {
            InitializeComponent();
        }

        private void Button_Click(object sender,RoutedEventArgs e) {
            Hide();
        }

        private void Grid_MouseDown(object sender,MouseButtonEventArgs e) {
            if(e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void DataGrid_MouseDoubleClick(object sender,MouseButtonEventArgs e) {
            if (DataGrid.SelectedIndex == -1) return;
            var item = (MangaInfoViewModel) DataGrid.SelectedItem;
            Process.Start(item.Link);
        }
    }
}
