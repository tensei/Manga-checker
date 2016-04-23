using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Manga_checker.Database;
using Manga_checker.Utilities;

namespace Manga_checker {
    /// <summary>
    ///     Interaktionslogik für SetupDatabaseDialog.xaml
    /// </summary>
    public partial class SetupDatabaseDialog : UserControl {
        public SetupDatabaseDialog() {
            InitializeComponent();
            //Application.Current.Dispatcher.BeginInvoke(new Action(delegate {

            //}));
            start();
        }

        private void start() {
            new Thread(new ThreadStart(delegate {
                Thread.Sleep(3000);
                DebugText.Write("Creating Database");
                Application.Current.Dispatcher.BeginInvoke(new Action(delegate { status.Content = "Creating Database"; }));
                Sqlite.SetupDatabase();
                Application.Current.Dispatcher.BeginInvoke(new Action(delegate {
                    status.Content = "FINISHED";
                    ProgressBar.Visibility = Visibility.Collapsed;
                    closeBtn.Visibility = Visibility.Visible;
                }));
            })).Start();
        }
    }
}