using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Manga_checker.Adding;
using Manga_checker.Database;
using Manga_checker.Handlers;
using Manga_checker.Properties;
using Manga_checker.ViewModels;
using MaterialDesignThemes.Wpf;

namespace Manga_checker {
    /// <summary>
    ///     Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        // private WebClient web = new WebClient();
        public ThreadStart Childref;

        public Thread ChildThread;

        public Thread client;

        public bool clientStatus = false;

        // private DataGridMangasItem itm = new DataGridMangasItem();
        public List<string> mlist;

        public MainWindow() {
            InitializeComponent();
            Settings.Default.Debug = "Debug shit goes in here!\n";
            StartupInit.Setup();
        }

        private void backlogaddbtn_Click(object sender, RoutedEventArgs e) {
            ParseFile.AddMangatoBacklog("backlog", backlognamebox.Text, backlogchapterbox.Text);
            if (Sqlite.GetMangaNameList("backlog").Contains(backlognamebox.Text)) {
                Sqlite.UpdateManga(
                    "backlog",
                    backlognamebox.Text,
                    backlogchapterbox.Text,
                    "placeholder",
                    DateTime.Now);
            }
            else {
                Sqlite.AddManga("backlog", backlognamebox.Text, backlogchapterbox.Text, "placeholder", DateTime.Now);
            }

            backlognamebox.Text = string.Empty;
            backlogchapterbox.Text = string.Empty;
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e) {
            Close();
        }

        private void DataGridMangas_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            try {
                var itemselected = (MangaModel) DataGridMangas.SelectedItem;
                var name_chapter = new List<string> {itemselected.Name, itemselected.Chapter};
                switch (itemselected.Site) {
                    case "Mangafox": {
                        OpenSite.Open("mangafox", name_chapter[0], name_chapter[1], mlist);
                        DebugText.Write(
                            $"[Debug] Opened {itemselected.Name} {itemselected.Chapter} on {itemselected.Site.ToUpper()}.");
                        break;
                    }

                    case "Mangareader": {
                        OpenSite.Open("mangareader", name_chapter[0], name_chapter[1], mlist);
                        DebugText.Write(
                            $"[Debug] Opened {itemselected.Name} {itemselected.Chapter} on {itemselected.Site.ToUpper()}.");
                        break;
                    }

                    case "Batoto": {
                        OpenSite.Open("batoto", name_chapter[0], name_chapter[1], mlist);
                        DebugText.Write(
                            $"[Debug] Opened {itemselected.Name} {itemselected.Chapter} on {itemselected.Site.ToUpper()}.");
                        break;
                    }
                }
            }
            catch (Exception g) {
                // do nothing
                DebugText.Write($"[Error] {g.Message} {g.TargetSite} ");
            }
        }

        private void DebugTextBox_TextChanged(object sender, TextChangedEventArgs e) {
            DebugTextBox.ScrollToEnd();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e) {
            DebugText.Write(Settings.Default.ThreadStatus.ToString());

            // ButtonColorChange();
            if (!File.Exists("MangaDB.sqlite")) {
                Tools.CreateDb();
            }
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.ChangedButton == MouseButton.Left) DragMove();
        }

        private void MiniBtn_Click(object sender, RoutedEventArgs e) {
            WindowState = WindowState.Minimized;
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e) {
            var regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void SearchBtn_Click(object sender, RoutedEventArgs e) {
            var d = new NormalAddDialog {DataContext = new NormalAddViewModel()};
            DialogHost.Show(d);
        }

        private void TopMostBtn_Click(object sender, RoutedEventArgs e) {
            Topmost = Topmost == false;
        }
    }
}