using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;

namespace Manga_checker__WPF_
{
    /// <summary>
    ///     Interaktionslogik für NotificationWindow.xaml
    /// </summary>
    public partial class NotificationWindow : Window
    {
        public readonly DispatcherTimer timer = new DispatcherTimer();
        private string link;

        public NotificationWindow(string text)
        {
            InitializeComponent();

            Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() =>
            {
                var desktopWorkingArea = SystemParameters.WorkArea;
                Left = desktopWorkingArea.Right - Width - 10;
                Top = desktopWorkingArea.Bottom - Height - 10;
            }));
            timer.Interval = TimeSpan.FromSeconds(5);
            timer.Tick += timer_Tick;
            label.Content = text;
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

        private void Gri(object sender, MouseButtonEventArgs e)
        {
             if (e.ChangedButton == MouseButton.Left)
                            DragMove();
        }
    }
}