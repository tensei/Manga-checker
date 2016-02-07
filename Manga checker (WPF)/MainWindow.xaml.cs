using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Manga_checker.Adding;
using Manga_checker.Database;
using Manga_checker.Handlers;
using Manga_checker.Properties;
using Manga_checker.Sites;
using Manga_checker.ViewModels;
using MaterialDesignThemes.Wpf;

namespace Manga_checker {
    /// <summary>
    ///     Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        //private WebClient web = new WebClient();
        public ThreadStart Childref;
        public Thread ChildThread;
        public Thread client;
        public bool clientStatus = false;
        //private DataGridMangasItem itm = new DataGridMangasItem();
        public List<string> mlist;

        public MainWindow() {
            InitializeComponent();
            Settings.Default.Debug = "Debug shit goes in here!\n";
            StartupInit.Setup();
            SetupMangaButtons();
            //var g = new NotificationWindow("Starting in 5...", 0, 5);
            //g.Show();
            //WebtoonsRSS toons = new WebtoonsRSS();
            ////toons.Check();
        }

        private void SetupMangaButtons() {
            MangareaderBtn.Visibility = ParseFile.GetValueSettings("mangareader") == "1" ? Visibility.Visible : Visibility.Collapsed;
            MangastreamBtn.Visibility = ParseFile.GetValueSettings("mangastream") == "1" ? Visibility.Visible : Visibility.Collapsed;
            MangafoxBtn.Visibility = ParseFile.GetValueSettings("mangafox") == "1" ? Visibility.Visible : Visibility.Collapsed;
            BatotoBtn.Visibility = ParseFile.GetValueSettings("batoto") == "1" ? Visibility.Visible : Visibility.Collapsed;
            DebugBtn.Visibility = ParseFile.GetValueSettings("debug") == "1" ? Visibility.Visible : Visibility.Collapsed;
            KissmangaBtn.Visibility = ParseFile.GetValueSettings("kissmanga") == "1" ? Visibility.Visible : Visibility.Collapsed;
            WebtoonsBtn.Visibility = ParseFile.GetValueSettings("webtoons") == "1" ? Visibility.Visible : Visibility.Collapsed;
            YomangaBtn.Visibility = ParseFile.GetValueSettings("yomanga") == "1" ? Visibility.Visible : Visibility.Collapsed;
        }


        private void CloseBtn_Click(object sender, RoutedEventArgs e) {
            Close();
        }

        private void MiniBtn_Click(object sender, RoutedEventArgs e) {
            WindowState = WindowState.Minimized;
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e) {
            SetupSettingsPanel();
            DebugText.Write(Settings.Default.ThreadStatus.ToString());
            // ButtonColorChange();
            if (!File.Exists("MangaDB.sqlite")) {
                Tools.CreateDb();
            }
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }


        private void DataGridMangas_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            try {
                var itemselected = (MangaViewModel) DataGridMangas.SelectedItem;
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
                //do nothing
                DebugText.Write($"[Error] {g.Message} {g.TargetSite} ");
            }
        }

        private void TopMostBtn_Click(object sender, RoutedEventArgs e) {
            Topmost = Topmost == false;
        }

        private void DebugTextBox_TextChanged(object sender, TextChangedEventArgs e) {
            DebugTextBox.ScrollToEnd();
        }
        
        private void SearchBtn_Click(object sender, RoutedEventArgs e) {
            var d = new NormalAddDialog {
                DataContext = new NormalAddViewModel()
            };
            DialogHost.Show(d);
        }

        private void backlogaddbtn_Click(object sender, RoutedEventArgs e) {
            ParseFile.AddMangatoBacklog("backlog", backlognamebox.Text, backlogchapterbox.Text);
            if (Sqlite.GetMangaNameList("backlog").Contains(backlognamebox.Text)) {
                Sqlite.UpdateManga("backlog", backlognamebox.Text, backlogchapterbox.Text, "placeholder");
            }
            else {
                Sqlite.AddManga("backlog", backlognamebox.Text, backlogchapterbox.Text, "placeholder");
            }
            backlognamebox.Text = string.Empty;
            backlogchapterbox.Text = string.Empty;
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e) {
            var regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        
        private void SetupSettingsPanel() {
            timebox.Text = Settings.Default.SettingRefreshTime.ToString();
            Settingsrssbox.Text = Settings.Default.SettingBatotoRSS;

            if (Settings.Default.SettingMangastream == "1") {
                MangastreamOnOffBtn.IsChecked = true;
            }
            if (Settings.Default.SettingMangareader == "1") {
                MangareaderOnOffBtn.IsChecked = true;
            }
            if (Settings.Default.SettingMangafox == "1") {
                MangafoxOnOffBtn.IsChecked = true;
            }
            if (Settings.Default.SettingBatoto == "1") {
                BatotoOnOffBtn.IsChecked = true;
            }
            if (Settings.Default.SettingKissmanga == "1") {
                KissmangaOnOffBtn.IsChecked = true;
            }
            if (Settings.Default.SettingWebtoons == "1") {
                WebtoonsOnOffBtn.IsChecked = true;
            }
            if (Settings.Default.SettingYomanga == "1") {
                YomangaOnOffBtn.IsChecked = true;
            }
            if (Settings.Default.SettingOpenLinks == "1") {
                LinkOpenBtn.IsChecked = true;
            }
            if (Settings.Default.ThreadStatus) {
                SendinfoOnOffBtn.IsChecked = true;
                DebugText.Write("Starting Client...");
                var connect = new ConnectToServer();
                client = new Thread(connect.Connect) {IsBackground = true};
                client.Start();
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e) {
            ParseFile.SetValueSettings("refresh time", timebox.Text);
            ParseFile.SetValueSettings("batoto_rss", Settingsrssbox.Text);
        }

        private void MangastreamOnOffBtn_Click(object sender, RoutedEventArgs e) {
            if (!Equals(MangastreamOnOffBtn.IsChecked, false)) {
                ParseFile.SetValueSettings("mangastream", "1");
                MangastreamBtn.Visibility = Visibility.Visible;
            }
            else {
                ParseFile.SetValueSettings("mangastream", "0");
                MangastreamBtn.Visibility = Visibility.Collapsed;
            }
        }

        private void MangareaderOnOffBtn_Click(object sender, RoutedEventArgs e) {
            if (!Equals(MangareaderOnOffBtn.IsChecked, false)) {
                ParseFile.SetValueSettings("mangareader", "1");
                MangareaderBtn.Visibility = Visibility.Visible;
            }
            else {
                ParseFile.SetValueSettings("mangareader", "0");
                MangareaderBtn.Visibility = Visibility.Collapsed;
            }
        }

        private void MangafoxOnOffBtn_Click(object sender, RoutedEventArgs e) {
            if (!Equals(MangafoxOnOffBtn.IsChecked, false)) {
                ParseFile.SetValueSettings("mangafox", "1");
                MangafoxBtn.Visibility = Visibility.Visible;
            }
            else {
                ParseFile.SetValueSettings("mangafox", "0");
                MangafoxBtn.Visibility = Visibility.Collapsed;
            }
        }

        private void KissmangaOnOffBtn_Click(object sender, RoutedEventArgs e) {
            if (!Equals(KissmangaOnOffBtn.IsChecked, false)) {
                ParseFile.SetValueSettings("kissmanga", "1");
                KissmangaBtn.Visibility = Visibility.Visible;
            }
            else {
                ParseFile.SetValueSettings("kissmanga", "0");
                KissmangaBtn.Visibility = Visibility.Collapsed;
            }
        }

        private void BatotoOnOffBtn_Click(object sender, RoutedEventArgs e) {
            if (!Equals(BatotoOnOffBtn.IsChecked, false)) {
                ParseFile.SetValueSettings("batoto", "1");
                BatotoBtn.Visibility = Visibility.Visible;
            }
            else {
                ParseFile.SetValueSettings("batoto", "0");
                BatotoBtn.Visibility = Visibility.Collapsed;
            }
        }

        private void LinkOpenBtn_Click(object sender, RoutedEventArgs e) {
            if (!Equals(LinkOpenBtn.IsChecked, false)) {
                ParseFile.SetValueSettings("open links", "1");
            }
            else {
                ParseFile.SetValueSettings("open links", "0");
            }
        }

        private void WebtoonsOnOffBtn_Click(object sender, RoutedEventArgs e) {
            if (!Equals(WebtoonsOnOffBtn.IsChecked, false)) {
                ParseFile.SetValueSettings("webtoons", "1");
                WebtoonsBtn.Visibility = Visibility.Visible;
            }
            else {
                ParseFile.SetValueSettings("webtoons", "0");
                WebtoonsBtn.Visibility = Visibility.Collapsed;
            }
        }

        private void SendinfoOnOffBtn_Click(object sender, RoutedEventArgs e) {
            if (!Equals(SendinfoOnOffBtn.IsChecked, false)) {
                if (!Settings.Default.ThreadStatus) {
                    DebugText.Write("Starting Client...");
                    var connect = new ConnectToServer();
                    client = new Thread(connect.Connect) {IsBackground = true};
                    client.Start();
                    Settings.Default.ThreadStatus = true;
                    Settings.Default.Save();
                    DebugText.Write(
                        $"switching Settings.Default.ThreadStatus to true : currently {Settings.Default.ThreadStatus}");
                }
            }
            else {
                if (Settings.Default.ThreadStatus) {
                    Settings.Default.ThreadStatus = false;
                    Settings.Default.Save();
                    DebugText.Write(
                        $"switching Settings.Default.ThreadStatus to false : currently {Settings.Default.ThreadStatus}");
                }
            }
        }

        public static string Base64Encode(string plainText) {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData) {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }

        private void exportBtn_Click(object sender, RoutedEventArgs e) {
            var cfg = Config.GetMangaConfig().ToString();
            var basecode = Base64Encode(cfg);
            ExpimpTextBox.Text = basecode;
            ExpimpTextBox.Focus();
            ExpimpTextBox.SelectAll();
            ExpimpLabel.Content = "Copy the text below!";
        }

        private void importBtn_Click(object sender, RoutedEventArgs e) {
            try {
                var cfg = Base64Decode(ExpimpTextBox.Text);
                var c = new Config();
                var msg = c.Write(cfg);
                DebugText.Write(msg);
                ExpimpLabel.Content = msg;
            }
            catch (Exception d) {
                DebugText.Write(d.Message);
            }
        }

        private void UpdateBatotoBtn_Click(object sender, RoutedEventArgs e) {
            var rssList = BatotoRSS.Get_feed_titles();
            var jsMangaList = ParseFile.GetBatotoMangaNames();

            foreach (var rssTitle in rssList) {
                var name = rssTitle.Split(new[] {" - "}, StringSplitOptions.None)[0];
                if (!jsMangaList.Contains(name)) {
                    jsMangaList.Add(name);
                    var match = Regex.Match(rssTitle, @".+ ch\.(\d+).+", RegexOptions.IgnoreCase);
                    ParseFile.AddManga("batoto", name, match.Groups[1].Value, "");
                    Sqlite.AddManga("batoto", name, match.Groups[1].Value, "placeholder");
                    DebugText.Write($"[Batoto] added {name}");
                }
            }
        }

        private void YomangaOnOffBtn_Click(object sender, RoutedEventArgs e) {
            if (!Equals(YomangaOnOffBtn.IsChecked, true)) {
                ParseFile.SetValueSettings("yomanga", "1");
                YomangaBtn.Visibility = Visibility.Visible;
            }
            else {
                ParseFile.SetValueSettings("yomanga", "0");
                YomangaBtn.Visibility = Visibility.Collapsed;
            }
        }

        //                }
        //                        chfloat.ToString(), "yes", "no"));
        //                    DataGridMangas.ContextMenu.Items.Add(CreateItem(itemselected.Site, name,
        //                    chfloat--;
        //                for (float i = 0; i < lastchc; i++) {
        //                chfloat = float.Parse(chapter);
        //                    "no", "yes"));
        //                DataGridMangas.ContextMenu.Items.Add(CreateItem(itemselected.Site, "Last 3 Chapter's", "",
        //            if (itemselected.Site.Equals("Mangafox")) {
        //            }
        //                }
        //                        chfloat.ToString(), "yes", "no"));
        //                    DataGridMangas.ContextMenu.Items.Add(CreateItem(itemselected.Site, name,
        //                    chfloat--;
        //                for (float i = 0; i < lastchc; i++) {
        //                chfloat = float.Parse(chapter);
        //                    "no", "yes"));
        //                DataGridMangas.ContextMenu.Items.Add(CreateItem(itemselected.Site, "Last 3 Chapter's", "",
        //            else if (itemselected.Site.Equals("Mangareader") && chapter.Contains(" ") == false) {
        //            }
        //                }
        //                        chfloat + " " + splitchapter[1], "yes", "no"));
        //                    DataGridMangas.ContextMenu.Items.Add(CreateItem(itemselected.Site, name,
        //                    chfloat--;
        //                for (float i = 0; i < lastchc; i++) {
        //                chfloat = float.Parse(splitchapter[0]);
        //                    "no", "yes"));

        //                DataGridMangas.ContextMenu.Items.Add(CreateItem(itemselected.Site, "Last 3 Chapter's", "",
        //                var splitchapter = chapter.Split(new[] {" "}, StringSplitOptions.None);
        //            if (itemselected.Site.Equals("Mangareader") && chapter.Contains(" ")) {
        //            float chfloat;
        //            var chapter = itemselected.Chapter;


        //            var name = itemselected.Name;
        //            DataGridMangas.ContextMenu.Items.Clear();
        //            itemselected.Site.Equals("Batoto") || itemselected.Site.Equals("Backlog")) {
        //        if (itemselected.Site.Equals("Mangareader") || itemselected.Site.Equals("Mangafox") ||
        //        var itemselected = (MangaViewModel) DataGridMangas.SelectedItem;
        //        const float lastchc = 3; //last x chapters displayed
        //    try {
        //    if (DataGridMangas.SelectedIndex.Equals(-1)) return;

        //private void DataGridMangas_SelectionChanged(object sender, SelectionChangedEventArgs e) {
        //            }

        //            if (itemselected.Site.Equals("Batoto")) {
        //                DataGridMangas.ContextMenu.Items.Add(CreateItem(itemselected.Site, "Last ~3 Chapter's", "",
        //                    "no", "yes"));
        //                chfloat = float.Parse(chapter);
        //                float ic = 1;
        //                if (mlist != null) {
        //                    foreach (var mangarss in mlist) {
        //                        var match = Regex.Match(mangarss, @".+ ch\.(\d*\.?\d*).+", RegexOptions.IgnoreCase);
        //                        var matchvalue = match.Groups[1].Value;
        //                        if (chfloat > float.Parse(matchvalue) && mangarss.ToLower().Contains(name.ToLower()) &&
        //                            ic <= lastchc) {
        //                            DataGridMangas.ContextMenu.Items.Add(CreateItem(itemselected.Site, name,
        //                                matchvalue, "yes", "no"));
        //                            ic++;
        //                        }
        //                    }
        //                }
        //                else {
        //                    DataGridMangas.ContextMenu.Items.Add(CreateItem(itemselected.Site, "found nothing", "",
        //                        "no", "yes"));
        //                }
        //            }
        //            if (itemselected.Site.Equals("Backlog")) {
        //                try {
        //                    var namem = itemselected.Name;
        //                    //TODO: move to... buttons
        //                    var item = new MenuItem();
        //                    //item.Margin = new Thickness(+15, 0, -40, 0);
        //                    item.Header = "Delete";
        //                    item.Click += delegate { Tools.Delete(itemselected); };
        //                    DataGridMangas.ContextMenu.Items.Add(item);
        //                }
        //                catch (Exception d) {
        //                    MessageBox.Show(d.Message);
        //                }
        //            }
        //        }
        //        else {
        //            DataGridMangas.ContextMenu.Items.Clear();
        //        }
        //    }
        //    catch (Exception ex) {
        //        DebugText.Write($"[Error] {ex}");
        //    }
        //}
    }
}