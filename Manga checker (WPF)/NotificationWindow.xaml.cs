using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Manga_checker
{
    /// <summary>
    ///     Interaktionslogik für NotificationWindow.xaml
    /// </summary>
    public partial class NotificationWindow : Window
    {
        public readonly DispatcherTimer timer = new DispatcherTimer();

        public NotificationWindow(string text, int num, int time)
        {
            InitializeComponent();

            Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() =>
            {
                var desktopWorkingArea = SystemParameters.WorkArea;
                Left = desktopWorkingArea.Right - Width - 10;
                Top = desktopWorkingArea.Bottom - Height - 10 - (Height + 10)*num;
            }));
            timer.Interval = TimeSpan.FromSeconds(time);
            timer.Tick += timer_Tick;
            label.Text = text;
        }

        public new void Show()
        {
            base.Show();
            timer.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            //set default result if necessary

            timer.Stop();
            Close();
        }

        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            timer.Stop();
            Close();
        }

        //}
        //                    DragMove();
        //     if (e.ChangedButton == MouseButton.Left)
        //{

        //private void Gri(object sender, MouseButtonEventArgs e)
    }
}