using System;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using Manga_checker.ViewModels;

namespace Manga_checker {
    /// <summary>
    ///     Interaktionslogik für MangaViewer.xaml
    /// </summary>
    public partial class MangaViewer : Window {
        private static Timer loopTimer;
        private string _link;

        public MangaViewer() {
            InitializeComponent();
            //loop timer
            loopTimer = new Timer();
            loopTimer.Interval = 10; // interval in milliseconds
            loopTimer.Enabled = false;
            loopTimer.Elapsed += loopTimerEvent;
            loopTimer.AutoReset = true;
        }

        public string link {
            get { return _link; }
            set { _link = value; }
        }

        private void loopTimerEvent(object source, ElapsedEventArgs e) {
            Application.Current.Dispatcher.BeginInvoke(new Action(() => {
                var x = scviewer.VerticalOffset;
                scviewer.ScrollToVerticalOffset(x + 10);
                x = scviewer.VerticalOffset;
            }));
        }


        private void Button_Click(object sender, RoutedEventArgs e) {
            Close();
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

        private void Button_Click_1(object sender, RoutedEventArgs e) {
            images.Items.Clear();
            Close();
        }
    }
}