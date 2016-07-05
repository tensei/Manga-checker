using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
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
    /// Interaktionslogik für MangaViewer.xaml
    /// </summary>
    public partial class MangaViewer : Window {
        private string _link;
        private static Timer loopTimer;
        public MangaViewer() {
            InitializeComponent();
            //loop timer
            loopTimer = new Timer();
            loopTimer.Interval = 10;// interval in milliseconds
            loopTimer.Enabled = false;
            loopTimer.Elapsed += loopTimerEvent;
            loopTimer.AutoReset = true;
        }
        private void loopTimerEvent(Object source, ElapsedEventArgs e) {
            Application.Current.Dispatcher.BeginInvoke(new Action(() => {
                var x = scviewer.VerticalOffset;
                scviewer.ScrollToVerticalOffset(x + 20);
                x = scviewer.VerticalOffset;
            }));
        }

        public string link {
            get { return _link; }
            set { _link = value;
                DataContext = new MangaViewerViewModel() {
                Link = _link
            };}
        }


        private void Button_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.ChangedButton == MouseButton.Left) DragMove();
        }
        
        private void Button_Click_2(object sender, RoutedEventArgs e) {
            scviewer.ScrollToTop();
        }
        

        private void Image_MouseDown(object sender, MouseButtonEventArgs e) {
            loopTimer.Enabled = true;
        }

        private void img_MouseUp(object sender, MouseButtonEventArgs e) {
            loopTimer.Enabled = false;
        }

        private void Canvas_MouseLeave(object sender, MouseEventArgs e) {
            loopTimer.Enabled = false;
        }
    }
}
