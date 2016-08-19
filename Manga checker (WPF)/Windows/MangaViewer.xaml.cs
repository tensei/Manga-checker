using System;
using System.Collections.ObjectModel;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MangaChecker.Common;

namespace MangaChecker.Windows {
    /// <summary>
    ///     Interaktionslogik für MangaViewer.xaml
    /// </summary>
    public partial class MangaViewer {
        private static Timer _loopTimer;

        public int Direction;

        public MangaViewer() {
            InitializeComponent();
            //loop timer
            _loopTimer = new Timer {
                Interval = 10,
                Enabled = false
            };
            // interval in milliseconds
            _loopTimer.Elapsed += loopTimerEvent;
            _loopTimer.AutoReset = true;
        }

        public string link { get; set; }

        private void loopTimerEvent(object source, ElapsedEventArgs e) {
            Application.Current.Dispatcher.BeginInvoke(new Action(() => {
                var x = scviewer.VerticalOffset;
                scviewer.ScrollToVerticalOffset(x + Direction);
                x = scviewer.VerticalOffset;
            }));
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.ChangedButton == MouseButton.Left) DragMove();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e) {
            scviewer.ScrollToTop();
        }


        private void Image_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.ChangedButton == MouseButton.Left) {
                _loopTimer.Enabled = true;
                Direction = 10;
            }
            if (e.ChangedButton == MouseButton.Right) {
                _loopTimer.Enabled = true;
                Direction = -10;
            }
        }

        private void img_MouseUp(object sender, MouseButtonEventArgs e) {
            _loopTimer.Enabled = false;
        }

        private void Canvas_MouseLeave(object sender, MouseEventArgs e) {
            _loopTimer.Enabled = false;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e) {
            images.Items.Clear();
            Close();
        }

        private void MetroWindow_Closing(object sender, EventArgs e) {
            GlobalVariables.ImagesInternal = new ObservableCollection<Image>();
            GC.Collect();
        }
    }
}