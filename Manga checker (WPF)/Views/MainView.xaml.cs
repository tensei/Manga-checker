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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Manga_checker.Common;
using Manga_checker.ViewModels.Model;

namespace Manga_checker.Views {
    /// <summary>
    /// Interaktionslogik für MainView.xaml
    /// </summary>
    public partial class MainView : UserControl {
        public MainView() {
            InitializeComponent();
        }

        private void DataGridMangas_OnMouseDoubleClick(object sender, MouseButtonEventArgs e) {
            try {
                var itemselected = (MangaModel)DataGridMangas.SelectedItem;
                if (itemselected.Link != "placeholder") {
                    Process.Start(itemselected.Link);
                    itemselected.New = 0;
                }
            } catch (Exception g) {
                // do nothing
                DebugText.Write($"[Error] {g.Message} {g.TargetSite} ");
            }
        }
        
    }
}
