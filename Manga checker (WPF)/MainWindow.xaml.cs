namespace Manga_checker {
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
            this.InitializeComponent();
            Settings.Default.Debug = "Debug shit goes in here!\n";
            StartupInit.Setup();
        }

        private void backlogaddbtn_Click(object sender, RoutedEventArgs e) {
            ParseFile.AddMangatoBacklog("backlog", this.backlognamebox.Text, this.backlogchapterbox.Text);
            if (Sqlite.GetMangaNameList("backlog").Contains(this.backlognamebox.Text)) {
                Sqlite.UpdateManga(
                    "backlog", 
                    this.backlognamebox.Text, 
                    this.backlogchapterbox.Text, 
                    "placeholder", 
                    DateTime.Now);
            }
            else {
                Sqlite.AddManga("backlog", this.backlognamebox.Text, this.backlogchapterbox.Text, "placeholder");
            }

            this.backlognamebox.Text = string.Empty;
            this.backlogchapterbox.Text = string.Empty;
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }

        private void DataGridMangas_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            try {
                var itemselected = (MangaModel)this.DataGridMangas.SelectedItem;
                var name_chapter = new List<string> { itemselected.Name, itemselected.Chapter };
                switch (itemselected.Site) {
                    case "Mangafox": {
                        OpenSite.Open("mangafox", name_chapter[0], name_chapter[1], this.mlist);
                        DebugText.Write(
                            $"[Debug] Opened {itemselected.Name} {itemselected.Chapter} on {itemselected.Site.ToUpper()}.");
                        break;
                    }

                    case "Mangareader": {
                        OpenSite.Open("mangareader", name_chapter[0], name_chapter[1], this.mlist);
                        DebugText.Write(
                            $"[Debug] Opened {itemselected.Name} {itemselected.Chapter} on {itemselected.Site.ToUpper()}.");
                        break;
                    }

                    case "Batoto": {
                        OpenSite.Open("batoto", name_chapter[0], name_chapter[1], this.mlist);
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
            this.DebugTextBox.ScrollToEnd();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e) {
            DebugText.Write(Settings.Default.ThreadStatus.ToString());

            // ButtonColorChange();
            if (!File.Exists("MangaDB.sqlite")) {
                Tools.CreateDb();
            }
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.ChangedButton == MouseButton.Left) this.DragMove();
        }

        private void MiniBtn_Click(object sender, RoutedEventArgs e) {
            this.WindowState = WindowState.Minimized;
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e) {
            var regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void SearchBtn_Click(object sender, RoutedEventArgs e) {
            var d = new NormalAddDialog { DataContext = new NormalAddViewModel() };
            DialogHost.Show(d);
        }

        private void TopMostBtn_Click(object sender, RoutedEventArgs e) {
            this.Topmost = this.Topmost == false;
        }
    }
}