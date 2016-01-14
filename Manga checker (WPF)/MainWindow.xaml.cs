using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Manga_checker.Adding;
using Manga_checker.Classes;
using Manga_checker.Handlers;
using Manga_checker.Properties;
using Manga_checker.Sites;
using Manga_checker.ViewModels;
using MaterialDesignThemes.Wpf;

namespace Manga_checker
{
    /// <summary>
    ///     Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Settings.Default.Debug = "Debug shit goes in here!\n";
            var startup = new StartupInit();
            startup.Setup();
            SetupMangaButtons();
            //var g = new NotificationWindow("Starting in 5...", 0, 5);
            //g.Show();
            //WebtoonsRSS toons = new WebtoonsRSS();
            ////toons.Check();
        }

        //private WebClient web = new WebClient();
        private readonly BatotoRSS _batoto = new BatotoRSS();
        public readonly SolidColorBrush SeparatorColor = new SolidColorBrush(Color.FromRgb(37, 37, 37));
        public readonly SolidColorBrush oncolor = new SolidColorBrush(Color.FromRgb(144, 202, 249));
        public readonly SolidColorBrush transp = new SolidColorBrush(Colors.Transparent);

        private readonly ParseFile _parseFile = new ParseFile();
        public readonly DispatcherTimer Timer = new DispatcherTimer();
        private string _siteSelected = "All";
        public ThreadStart Childref;
        public Thread ChildThread;
        public Thread client;
        public bool clientStatus = false;
        private string force = "";
        //private DataGridMangasItem itm = new DataGridMangasItem();
        public List<string> mlist;


        public void DebugText(string text)
        {
            //Read
            Settings.Default.Debug += text + "\n";
        }
        
        private void SetupMangaButtons()
        {
            if (_parseFile.GetValueSettings("mangareader") == "1")
            {
                MangareaderBtn.Visibility = Visibility.Visible;
            }
            else
            {
                MangareaderBtn.Visibility = Visibility.Collapsed;
            }
            if (_parseFile.GetValueSettings("mangastream") == "1")
            {
                MangastreamBtn.Visibility = Visibility.Visible;
            }
            else
            {
                MangastreamBtn.Visibility = Visibility.Collapsed;
            }
            if (_parseFile.GetValueSettings("mangafox") == "1")
            {
                MangafoxBtn.Visibility = Visibility.Visible;
            }
            else
            {
                MangafoxBtn.Visibility = Visibility.Collapsed;
            }
            if (_parseFile.GetValueSettings("batoto") == "1")
            {
                BatotoBtn.Visibility = Visibility.Visible;
            }
            else
            {
                BatotoBtn.Visibility = Visibility.Collapsed;
            }
            if (_parseFile.GetValueSettings("debug") == "1")
            {
                DebugBtn.Visibility = Visibility.Visible;
            }
            else
            {
                DebugBtn.Visibility = Visibility.Collapsed;
            }
            if (_parseFile.GetValueSettings("kissmanga") == "1")
            {
                KissmangaBtn.Visibility = Visibility.Visible;
            }
            else
            {
                KissmangaBtn.Visibility = Visibility.Collapsed;
            }
            if (_parseFile.GetValueSettings("webtoons") == "1")
            {
                WebtoonsBtn.Visibility = Visibility.Visible;
            }
            else
            {
                WebtoonsBtn.Visibility = Visibility.Collapsed;
            }
            if (_parseFile.GetValueSettings("yomanga") == "1")
            {
                YomangaBtn.Visibility = Visibility.Visible;
            }
            else
            {
                YomangaBtn.Visibility = Visibility.Collapsed;
            }
        }


        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Timer.Stop();
            Close();
        }

        private void MiniBtn_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            //if (parseFile.GetValueSettings("debug") == "1")
            //{
            //    AllocConsole(); // Show Console 
            //    Console.Title = "Manga Checker made by Tensei";
            //}
            Childref = CheckNow;
            ChildThread = new Thread(Childref) {IsBackground = true};
            ChildThread.Start();

            SetupSettingsPanel();
            //parseFile.AddToNotReadList("mangastream", "the seven deadly sins", 44);
            //Timer.Start();

            //YomangaRSS yooRss = new YomangaRSS();
            //yooRss.Check();
            DebugText(Settings.Default.ThreadStatus.ToString());
            _siteSelected = "All";
            // ButtonColorChange();
        }

        private void CheckNow()
        {
            var i = 5;
            var count = 0;
            var ms = new MangastreamRSS();
            var mf = new MangafoxRSS();
            var mr = new MangareaderHTML();
            var ba = new BatotoRSS();
            var kiss = new KissmangaHTML();
            var toons = new WebtoonsRSS();
            var yoo = new YomangaRSS();
            while (true)
            {
                if (force.Equals("force"))
                {
                    i = 0;
                    force = "";
                }
                if (i >= 1)
                {
                    Dispatcher.BeginInvoke(new Action(delegate
                    {
                        StatusLb.Content = "Status: Checking in " + i +
                                           " seconds.";
                    }));
                    Thread.Sleep(1000);
                    i--;

                    Dispatcher.BeginInvoke(new Action(delegate { CounterLbl.Content = count.ToString(); }));
                }
                else
                {
                    if (_parseFile.GetValueSettings("mangastream") == "1")
                    {
                        Dispatcher.BeginInvoke(
                            new Action(delegate { StatusLb.Content = "Status: Checking Mangastream"; }));
                        try
                        {
                            ms.checked_if_new();
                        }
                        catch (Exception mst)
                        {
                            DebugText($"[{DateTime.Now}][Mangastream] Error {mst.Message} {mst.Data}");
                        }
                    }
                    if (_parseFile.GetValueSettings("mangafox") == "1")
                    {
                        Dispatcher.BeginInvoke(new Action(delegate { StatusLb.Content = "Status: Checking Mangafox"; }));
                        foreach (var manga in _parseFile.GetManga("mangafox"))
                        {
                            //debugText(string.Format("[{0}][Mangafox] Checking {1}.", DateTime.Now, manga.Replace("[]", " ")));
                            mf.check_all(manga);
                        }
                    }
                    if (_parseFile.GetValueSettings("mangareader") == "1")
                    {
                        Dispatcher.BeginInvoke(
                            new Action(delegate { StatusLb.Content = "Status: Checking Mangareader"; }));
                        foreach (var manga in _parseFile.GetManga("mangareader"))
                        {
                            try
                            {
                                //debugText(string.Format("[{0}][Mangareader] Checking {1}.", DateTime.Now,manga.Replace("[]", " ")));
                                mr.Check(manga);
                                Thread.Sleep(1000);
                            }
                            catch (Exception mrd)
                            {
                                // lol
                                DebugText(string.Format("[{1}][Mangareader] Error {0} {2}.", manga.Replace("[]", " "),
                                    DateTime.Now, mrd.Message));
                            }
                        }
                    }
                    if (_parseFile.GetValueSettings("batoto") == "1")
                    {
                        Dispatcher.BeginInvoke(new Action(delegate { StatusLb.Content = "Status: Checking Batoto"; }));
                        try
                        {
                            mlist = ba.Get_feed_titles();
                            ba.Check(mlist);
                            Thread.Sleep(1000);
                        }
                        catch (Exception bat)
                        {
                            // lol
                            DebugText(string.Format("[{0}][batoto] Error {1}.", DateTime.Now, bat.Message));
                        }
                    }
                    if (_parseFile.GetValueSettings("kissmanga") == "1")
                    {
                        Dispatcher.BeginInvoke(new Action(delegate { StatusLb.Content = "Status: Checking Kissmanga"; }));
                        foreach (var manga in _parseFile.GetManga("kissmanga"))
                        {
                            try
                            {
                                //DebugText(string.Format("[{0}][Kissmanga] Checking {1}.", DateTime.Now, manga.Replace("[]", " ")));
                                var man = manga.Split(new[] {"[]"}, StringSplitOptions.None);
                                kiss.check(man[0], man[1]);
                                Thread.Sleep(5000);
                            }
                            catch (Exception mrd)
                            {
                                // lol
                                DebugText(string.Format("[{1}][Kissmanga] Error {0} {2}.", manga.Replace("[]", " "),
                                    DateTime.Now, mrd.Message));
                            }
                        }
                    }
                    if (_parseFile.GetValueSettings("webtoons") == "1")
                    {
                        Dispatcher.BeginInvoke(new Action(delegate { StatusLb.Content = "Status: Checking Webtoons"; }));
                        try
                        {
                            toons.Check();
                        }
                        catch (Exception to)
                        {
                            DebugText($"[{DateTime.Now}][Webtoons] Error {to.Message}.");
                        }
                    }
                    if (_parseFile.GetValueSettings("yomanga") == "1")
                    {
                        Dispatcher.BeginInvoke(new Action(delegate { StatusLb.Content = "Status: Checking YoManga"; }));
                        try
                        {
                            yoo.Check();
                        }
                        catch (Exception to)
                        {
                            DebugText($"[{DateTime.Now}][YoManga] Error {to.Message}.");
                        }
                    }
                    //timer2.Start();
                    var waittime = int.Parse(_parseFile.GetValueSettings("refresh time"));

                    Dispatcher.BeginInvoke(
                        new Action(delegate { StatusLb.Content = @"Status: Checking in " + waittime + " seconds."; }));
                    count++;
                    i = waittime;
                }
            }
        }

        

        

        

        private void AllBtn_Click(object sender, RoutedEventArgs e)
        {
            if (DebugTextBox.Visibility == Visibility.Visible)
            {
                DebugTextBox.Visibility = Visibility.Collapsed;
                DataGridMangas.Visibility = Visibility.Visible;
            }
            
            _siteSelected = "All";
            // ButtonColorChange();
            ClosePanels();
        }

        private void MangastreamBtn_Click(object sender, RoutedEventArgs e)
        {
            if (DebugTextBox.Visibility == Visibility.Visible)
            {
                DebugTextBox.Visibility = Visibility.Collapsed;
                DataGridMangas.Visibility = Visibility.Visible;
            }
            

            _siteSelected = "Mangastream";
            // ButtonColorChange();
            ClosePanels();
        }

        private void MangafoxBtn_Click(object sender, RoutedEventArgs e)
        {
            if (DebugTextBox.Visibility == Visibility.Visible)
            {
                DebugTextBox.Visibility = Visibility.Collapsed;
                DataGridMangas.Visibility = Visibility.Visible;
            }
            _siteSelected = "Mangafox";
            // ButtonColorChange();
            ClosePanels();
        }

        private void MangareaderBtn_Click(object sender, RoutedEventArgs e)
        {
            if (DebugTextBox.Visibility == Visibility.Visible)
            {
                DebugTextBox.Visibility = Visibility.Collapsed;
                DataGridMangas.Visibility = Visibility.Visible;
            }
            _siteSelected = "Mangareader";
            // ButtonColorChange();
            ClosePanels();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void OpenSite(string site, string name, string chapter)
        {
            var open = new OpenSite();
            open.Open(site, name, chapter, mlist);
        }

        private void DataGridMangas_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var itemselected = (MangaItemViewModel) DataGridMangas.SelectedItem;
                var name_chapter = new List<string> { itemselected.Name, itemselected.Chapter};
                switch (itemselected.Site)
                {
                    case "Mangafox":
                    {
                        var _openSite = new OpenSite();
                        _openSite.Open("mangafox", name_chapter[0], name_chapter[1], mlist);
                        DebugText(
                            $"[{DateTime.Now}][Debug] Opened {itemselected.Name} {itemselected.Chapter} on {itemselected.Site.ToUpper()}.");
                        break;
                    }
                    case "Mangareader":
                    {
                        var _openSite = new OpenSite();
                        _openSite.Open("mangareader", name_chapter[0], name_chapter[1], mlist);
                        DebugText(
                            $"[{DateTime.Now}][Debug] Opened {itemselected.Name} {itemselected.Chapter} on {itemselected.Site.ToUpper()}.");
                        break;
                    }
                    case "Batoto":
                    {
                        var _openSite = new OpenSite();
                        _openSite.Open("batoto", name_chapter[0], name_chapter[1], mlist);
                        DebugText(
                            $"[{DateTime.Now}][Debug] Opened {itemselected.Name} {itemselected.Chapter} on {itemselected.Site.ToUpper()}.");
                        break;
                    }
                }
            }
            catch (Exception g)
            {
                //do nothing
                DebugText($"[{DateTime.Now}][Error] {g.Message} {g.TargetSite} ");
            }
            
        }

        private void ForceBtn_Click(object sender, RoutedEventArgs e)
        {
            force = "force";
        }

        private void TopMostBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Topmost == false)
            {
                Topmost = true;
                //TopMostBtn.Style = (Style)FindResource("OpBtnOnStyle");
            }
            else
            {
                Topmost = false;
                //TopMostBtn.Style = (Style)FindResource("OpBtnOffStyle");
            }
        }

        private void SettingsBtn_Click(object sender, RoutedEventArgs e)
        {
            if (SettingsPanel.Visibility != 0)
            {
                //SettingsBtn.Style = (Style)FindResource("OpBtnOnStyle");
                SettingsPanel.Visibility = Visibility.Visible;
                return;
            }
            //SettingsBtn.Style = (Style)FindResource("OpBtnOffStyle");
            SettingsPanel.Visibility = Visibility.Collapsed;
            //_settingsWnd.Show();
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            if (AddPanel.Visibility != 0)
            {
                //AddBtn.Style = (Style)FindResource("OpBtnOnStyle");
                linkbox.Text = "";
                AddBtn_Copy.Content = "Add";
                SiteNameLb.Content = "None";
                MangaNameLb.Content = "None";
                ChapterNumLb.Content = "None";
                AddPanel.Visibility = 0;
            }
            else
            {
                //AddBtn.Style = (Style)FindResource("OpBtnOffStyle");
                AddPanel.Visibility = Visibility.Collapsed;
            }
        }

        private MenuItem CreateItem(string site, string name, string ch, string click, string header)
        {
            var item = new MenuItem();
            if (header == "yes")
            {
                item.Header = name;
                item.IsEnabled = false;
            }
            if (click == "yes")
                item.Header = name + " : " + ch;
            item.Click += (sender, e) => OpenSite(site, name, ch);
            return item;
        }

        // ReSharper disable once FunctionComplexityOverflow
        private void DataGridMangas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(DataGridMangas.SelectedIndex.Equals(-1)) return;
            try
            {
                const float lastchc = 3; //last x chapters displayed
                var itemselected = (MangaItemViewModel) DataGridMangas.SelectedItem;
                if (itemselected.Site.Equals("Mangareader") || itemselected.Site.Equals("Mangafox") ||
                    itemselected.Site.Equals("Batoto") || itemselected.Site.Equals("Backlog"))
                {
                    DataGridMangas.ContextMenu.Items.Clear();


                    var name = itemselected.Name;
                    var chapter = itemselected.Chapter;
                    float chfloat;
                    if (itemselected.Site.Equals("Mangareader") && chapter.Contains(" "))
                    {
                        var splitchapter = chapter.Split(new[] {" "}, StringSplitOptions.None);

                        DataGridMangas.ContextMenu.Items.Add(CreateItem(itemselected.Site, "Last 3 Chapter's", "",
                            "no", "yes"));
                        chfloat = float.Parse(splitchapter[0]);
                        for (float i = 0; i < lastchc; i++)
                        {
                            chfloat--;
                            DataGridMangas.ContextMenu.Items.Add(CreateItem(itemselected.Site, name,
                                chfloat + " " + splitchapter[1], "yes", "no"));
                        }
                    }
                    else if (itemselected.Site.Equals("Mangareader") && chapter.Contains(" ") == false)
                    {
                        DataGridMangas.ContextMenu.Items.Add(CreateItem(itemselected.Site, "Last 3 Chapter's", "",
                            "no", "yes"));
                        chfloat = float.Parse(chapter);
                        for (float i = 0; i < lastchc; i++)
                        {
                            chfloat--;
                            DataGridMangas.ContextMenu.Items.Add(CreateItem(itemselected.Site, name,
                                chfloat.ToString(), "yes", "no"));
                        }
                    }
                    if (itemselected.Site.Equals("Mangafox"))
                    {
                        DataGridMangas.ContextMenu.Items.Add(CreateItem(itemselected.Site, "Last 3 Chapter's", "",
                            "no", "yes"));
                        chfloat = float.Parse(chapter);
                        for (float i = 0; i < lastchc; i++)
                        {
                            chfloat--;
                            DataGridMangas.ContextMenu.Items.Add(CreateItem(itemselected.Site, name,
                                chfloat.ToString(), "yes", "no"));
                        }
                    }

                    if (itemselected.Site.Equals("Batoto"))
                    {
                        DataGridMangas.ContextMenu.Items.Add(CreateItem(itemselected.Site, "Last ~3 Chapter's", "",
                            "no", "yes"));
                        chfloat = float.Parse(chapter);
                        float ic = 1;
                        if (mlist != null)
                        {
                            foreach (var mangarss in mlist)
                            {
                                var match = Regex.Match(mangarss, @".+ ch\.(\d*\.?\d*).+", RegexOptions.IgnoreCase);
                                var matchvalue = match.Groups[1].Value;
                                if (chfloat > float.Parse(matchvalue) && mangarss.ToLower().Contains(name.ToLower()) &&
                                    ic <= lastchc)
                                {
                                    DataGridMangas.ContextMenu.Items.Add(CreateItem(itemselected.Site, name,
                                        matchvalue, "yes", "no"));
                                    ic++;
                                }
                            }
                        }
                        else
                        {
                            DataGridMangas.ContextMenu.Items.Add(CreateItem(itemselected.Site, "found nothing", "",
                            "no", "yes"));
                        }
                            
                    }
                    if (itemselected.Site.Equals("Backlog"))
                    {
                        try
                        {
                            var namem = itemselected.Name;
                            //TODO: move to... buttons
                            var item = new MenuItem();
                            //item.Margin = new Thickness(+15, 0, -40, 0);
                            item.Header = "Delete";
                            item.Click += delegate
                            {
                                var dialog = new ConfirmDeleteDialog
                                {
                                    MessageTextBlock =
                                    {
                                        Text = "Deleting " + namem 
                                    },
                                    SiteName = {
                                        Text = "Backlog"
    
                                    }
                                };
                                DialogHost.Show(dialog);
                            };
                            DataGridMangas.ContextMenu.Items.Add(item);
                        }
                        catch (Exception d)
                        {
                            MessageBox.Show(d.Message);
                        }
                    }
                }
                else
                {
                    DataGridMangas.ContextMenu.Items.Clear();
                }
            }
            catch (Exception ex)
            {
                DebugText($"[{DateTime.Now}][Error] {ex}");
            }
        }

        private void BatotoBtn_Click(object sender, RoutedEventArgs e)
        {
            //b.Check();
            if (DebugTextBox.Visibility == Visibility.Visible)
            {
                DebugTextBox.Visibility = Visibility.Collapsed;
                DataGridMangas.Visibility = Visibility.Visible;
            }
            _siteSelected = "Batoto";
            // ButtonColorChange();
            ClosePanels();
        }

        private void DebugBtn_Click(object sender, RoutedEventArgs e)
        {
            _siteSelected = "Debug";
            if (DataGridMangas.Visibility == Visibility.Visible)
            {
                DebugTextBox.Visibility = Visibility.Visible;
                DataGridMangas.Visibility = Visibility.Collapsed;
            }
            // ButtonColorChange();
            ClosePanels();
        }

        private void DebugTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            DebugTextBox.ScrollToEnd();
        }

        private void AddMangaBtn_Click(object sender, RoutedEventArgs e)
        {
            // add the manga
            if (SiteNameLb.Content.ToString().ToLower().Contains("mangareader"))
            {
                if (MangaNameLb.Content.ToString() != "ERROR" ||
                    MangaNameLb.Content.ToString() != "None" && ChapterNumLb.Content.ToString() != "None" ||
                    ChapterNumLb.Content.ToString() != "ERROR")
                {
                    DebugText($"[{DateTime.Now}][Debug] Trying to add {MangaNameLb.Content} {ChapterNumLb.Content}");
                    _parseFile.AddManga("mangareader", MangaNameLb.Content.ToString().ToLower(),
                        ChapterNumLb.Content.ToString(), "");
                    AddBtn_Copy.Content = "Success!";
                }
            }
            if (SiteNameLb.Content.ToString().ToLower().Contains("mangafox"))
            {
                if (!MangaNameLb.Content.ToString().Equals("ERROR") &&
                    MangaNameLb.Content.ToString() != "None" && ChapterNumLb.Content.ToString() != "None" &&
                    ChapterNumLb.Content.ToString() != "ERROR")
                {
                    DebugText($"[{DateTime.Now}][Debug] Trying to add {MangaNameLb.Content} {ChapterNumLb.Content}");
                    _parseFile.AddManga("mangafox", MangaNameLb.Content.ToString().ToLower(),
                        ChapterNumLb.Content.ToString(), "");
                    AddBtn_Copy.Content = "Success!";
                }
            }
            if (SiteNameLb.Content.ToString().ToLower().Contains("mangastream"))
            {
                if (!MangaNameLb.Content.ToString().Equals("ERROR") &&
                    MangaNameLb.Content.ToString() != "None" && ChapterNumLb.Content.ToString() != "None" &&
                    ChapterNumLb.Content.ToString() != "ERROR")
                {
                    DebugText($"[{DateTime.Now}][Debug] Trying to add {MangaNameLb.Content} {ChapterNumLb.Content}");
                    _parseFile.AddManga("mangastream", MangaNameLb.Content.ToString().ToLower(),
                        ChapterNumLb.Content.ToString(), "");
                    AddBtn_Copy.Content = "Success!";
                }
            }
        }

        private void SearchBtn_Click(object sender, RoutedEventArgs e)
        {
            var m = new SearchInfo().search(linkbox.Text.ToLower());
            if (!m.Error.Equals("null"))
            {
                //MessageBox.Show(m.Error);
                return;
            }
            MangaNameLb.Content = m.Name;
            ChapterNumLb.Content = m.Chapter;
            SiteNameLb.Content = m.Site;
        }

        private void BacklogBtn_Click(object sender, RoutedEventArgs e)
        {
            if (DebugTextBox.Visibility == Visibility.Visible)
            {
                DebugTextBox.Visibility = Visibility.Collapsed;
                DataGridMangas.Visibility = Visibility.Visible;
            }
            
            _siteSelected = "Backlog";
            // ButtonColorChange();
            ClosePanels();
        }

       private void backlogaddbtn_Click(object sender, RoutedEventArgs e)
        {
           _parseFile.AddMangatoBacklog("backlog", backlognamebox.Text, backlogchapterbox.Text);
            backlognamebox.Text = string.Empty;
            backlogchapterbox.Text = string.Empty;
            _siteSelected = "Backlog";
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void backlognamebox_DropDownOpened(object sender, EventArgs e)
        {
            backlognamebox.Items.Clear();
            var backl = _parseFile.GetBacklog();
            if (backl.Count.Equals(0))
            {
                return;
            }
            foreach (var manga in backl)
            {
                var name = manga.Split(new[] {": "}, StringSplitOptions.None)[0].Trim();
                backlognamebox.Items.Add(name);
            }
        }

        private void KissmangaBtn_Click(object sender, RoutedEventArgs e)
        {
            if (DebugTextBox.Visibility == Visibility.Visible)
            {
                DebugTextBox.Visibility = Visibility.Collapsed;
                DataGridMangas.Visibility = Visibility.Visible;
            }
           
            _siteSelected = "Kissmanga";
            // ButtonColorChange();
            ClosePanels();
        }
        

        private void WebtoonsBtn_Click(object sender, RoutedEventArgs e)
        {
            if (DebugTextBox.Visibility == Visibility.Visible)
            {
                DebugTextBox.Visibility = Visibility.Collapsed;
                DataGridMangas.Visibility = Visibility.Visible;
            }
            _siteSelected = "Webtoons";
            // ButtonColorChange();
            ClosePanels();
        }


        private void SetupSettingsPanel()
        {
            timebox.Text = Settings.Default.SettingRefreshTime.ToString();
            Settingsrssbox.Text = Settings.Default.SettingBatotoRSS;

            if (Settings.Default.SettingMangastream == "1")
            {
                MangastreamOnOffBtn.Background = oncolor;
            }
            else
            {
                MangastreamOnOffBtn.Background = transp;
            }
            if (Settings.Default.SettingMangareader == "1")
            {
                MangareaderOnOffBtn.Background = oncolor;
            }
            else
            {
                MangareaderOnOffBtn.Background = transp;
            }
            if (Settings.Default.SettingMangafox == "1")
            {
                MangafoxOnOffBtn.Background = oncolor;
            }
            else
            {
                MangafoxOnOffBtn.Background = transp;
            }
            if (Settings.Default.SettingBatoto == "1")
            {
                BatotoOnOffBtn.Background = oncolor;
            }
            else
            {
                BatotoOnOffBtn.Background = transp;
            }
            if (Settings.Default.SettingKissmanga == "1")
            {
                KissmangaOnOffBtn.Background = oncolor;
            }
            else
            {
                KissmangaOnOffBtn.Background = transp;
            }
            if (Settings.Default.SettingWebtoons == "1")
            {
                WebtoonsOnOffBtn.Background = oncolor;
            }
            else
            {
                WebtoonsOnOffBtn.Background = transp;
            }
            if (Settings.Default.SettingYomanga == "1")
            {
                YomangaOnOffBtn.Background = oncolor;
            }
            else
            {
                YomangaOnOffBtn.Background = transp;
            }
            if (Settings.Default.SettingOpenLinks == "1")
            {
                LinkOpenBtn.Background = oncolor;
            }
            else
            {
                LinkOpenBtn.Background = transp;
            }
            if (Settings.Default.ThreadStatus)
            {
                SendinfoOnOffBtn.Background = oncolor;
                DebugText("Starting Client...");
                var connect = new ConnectToServer();
                client = new Thread(connect.Connect) { IsBackground = true };
                client.Start();
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            _parseFile.SetValueSettings("refresh time", timebox.Text);
            _parseFile.SetValueSettings("batoto_rss", Settingsrssbox.Text);
        }

        private void MangastreamOnOffBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!Equals(MangastreamOnOffBtn.Background ,  oncolor))
            {
                MangastreamOnOffBtn.Background = oncolor;
                _parseFile.SetValueSettings("mangastream", "1");
                MangastreamBtn.Visibility = Visibility.Visible;
                MangastreamOnOffBtn.Background = oncolor;

            }
            else
            {
                MangastreamOnOffBtn.Background = transp;
                _parseFile.SetValueSettings("mangastream", "0");
                MangastreamBtn.Visibility = Visibility.Collapsed;
                MangastreamOnOffBtn.Background = transp;
            }
        }

        private void MangareaderOnOffBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!Equals(MangareaderOnOffBtn.Background ,  oncolor))
            {
                MangareaderOnOffBtn.Background = oncolor;
                _parseFile.SetValueSettings("mangareader", "1");
                MangareaderBtn.Visibility = Visibility.Visible;
            }
            else
            {
                MangareaderOnOffBtn.Background = transp;
                _parseFile.SetValueSettings("mangareader", "0");
                MangareaderBtn.Visibility = Visibility.Collapsed;
            }
        }

        private void MangafoxOnOffBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!Equals(MangafoxOnOffBtn.Background ,  oncolor))
            {
                MangafoxOnOffBtn.Background = oncolor;
                _parseFile.SetValueSettings("mangafox", "1");
                MangafoxBtn.Visibility = Visibility.Visible;
            }
            else
            {
                MangafoxOnOffBtn.Background = transp;
                _parseFile.SetValueSettings("mangafox", "0");
                MangafoxBtn.Visibility = Visibility.Collapsed;
            }
        }

        private void KissmangaOnOffBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!Equals(KissmangaOnOffBtn.Background ,  oncolor))
            {
                KissmangaOnOffBtn.Background = oncolor;
                _parseFile.SetValueSettings("kissmanga", "1");
                KissmangaBtn.Visibility = Visibility.Visible;
            }
            else
            {
                KissmangaOnOffBtn.Background = transp;
                _parseFile.SetValueSettings("kissmanga", "0");
                KissmangaBtn.Visibility = Visibility.Collapsed;
            }
        }

        private void BatotoOnOffBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!Equals(BatotoOnOffBtn.Background ,  oncolor))
            {
                BatotoOnOffBtn.Background = oncolor;
                _parseFile.SetValueSettings("batoto", "1");
                BatotoBtn.Visibility = Visibility.Visible;
            }
            else
            {
                BatotoOnOffBtn.Background = transp;
                _parseFile.SetValueSettings("batoto", "0");
                BatotoBtn.Visibility = Visibility.Collapsed;
            }
        }

        private void LinkOpenBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!Equals(LinkOpenBtn.Background ,  oncolor))
            {
                LinkOpenBtn.Background = oncolor;
                _parseFile.SetValueSettings("open links", "1");
            }
            else
            {
                LinkOpenBtn.Background = transp;
                _parseFile.SetValueSettings("open links", "0");
            }
        }

        private void WebtoonsOnOffBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!Equals(WebtoonsOnOffBtn.Background ,  oncolor))
            {
                WebtoonsOnOffBtn.Background = oncolor;
                _parseFile.SetValueSettings("webtoons", "1");
                WebtoonsBtn.Visibility = Visibility.Visible;
            }
            else
            {
                WebtoonsOnOffBtn.Background = transp;
                _parseFile.SetValueSettings("webtoons", "0");
                WebtoonsBtn.Visibility = Visibility.Collapsed;
            }
        }

        private void SendinfoOnOffBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!Equals(SendinfoOnOffBtn.Background, oncolor))
            {
                SendinfoOnOffBtn.Background = oncolor;
                if (!Settings.Default.ThreadStatus)
                {
                    DebugText("Starting Client...");
                    var connect = new ConnectToServer();
                    client = new Thread(connect.Connect) {IsBackground = true};
                    client.Start();
                    Settings.Default.ThreadStatus = true;
                    Settings.Default.Save();
                    DebugText(
                        $"switching Settings.Default.ThreadStatus to true : currently {Settings.Default.ThreadStatus}");
                }
            }
            else
            {
                SendinfoOnOffBtn.Background = transp;
                if (Settings.Default.ThreadStatus)
                {
                    Settings.Default.ThreadStatus = false;
                    Settings.Default.Save();
                    DebugText(
                        $"switching Settings.Default.ThreadStatus to false : currently {Settings.Default.ThreadStatus}");
                }
            }
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        private void exportBtn_Click(object sender, RoutedEventArgs e)
        {
            var cfg = new Config().GetMangaConfig().ToString();
            var basecode = Base64Encode(cfg);
            expimpTextBox.Text = basecode;
            expimpTextBox.Focus();
            expimpTextBox.SelectAll();
            ExpimpLabel.Content = "Copy the text below!";
        }

        private void importBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var cfg = Base64Decode(expimpTextBox.Text);
                var c = new Config();
                var msg = c.Write(cfg);
                DebugText(msg);
                ExpimpLabel.Content = msg;
            }
            catch (Exception d)
            {
                DebugText(d.Message);
            }
        }

        private void Popupbtn_Click(object sender, RoutedEventArgs e)
        {
            if (Options.Visibility != 0)
            {
                Options.Visibility = 0;
                DataGridMangas.Margin = new Thickness(155, 0, 0, 0);
                DebugTextBox.Margin = new Thickness(155, 0, 0, 0);
                return;
            }
            Options.Visibility = Visibility.Collapsed;
            DataGridMangas.Margin = new Thickness(0);
            DebugTextBox.Margin = new Thickness(0);
            ClosePanels();
        }

        public void ClosePanels()
        {
            
            SettingsPanel.Visibility = Visibility.Collapsed;
            AddPanel.Visibility = Visibility.Collapsed;
            //Popupbtn.IsChecked = false;
            //SettingsBtn.Style = (Style)FindResource("OpBtnOffStyle");
            //AddBtn.Style = (Style)FindResource("OpBtnOffStyle");
        }

        private void ButtonColorChange()
        {
            var trans = new SolidColorBrush(Colors.Transparent);
            var butonclickedBG = new SolidColorBrush(Color.FromRgb(144, 202, 249));

            AllBtn.Background = trans;

            MangareaderBtn.Background = trans;

            MangafoxBtn.Background = trans;

            MangastreamBtn.Background = trans;

            BatotoBtn.Background = trans;

            WebtoonsBtn.Background = trans;

            KissmangaBtn.Background = trans;

            DebugBtn.Background = trans;

            BacklogBtn.Background = trans;

            YomangaBtn.Background = trans;

            switch (_siteSelected)
            {
                case "Mangareader":
                {
                    MangareaderBtn.Background = butonclickedBG;
                }
                    break;
                case "Mangafox":
                {
                    MangafoxBtn.Background = butonclickedBG;
                }
                    break;
                case "Mangastream":
                {
                    MangastreamBtn.Background = butonclickedBG;

                }
                    break;
                case "Kissmanga":
                {
                    KissmangaBtn.Background = butonclickedBG;
                }
                    break;
                case "Batoto":
                {
                    BatotoBtn.Background = butonclickedBG;
                }
                    break;
                case "Webtoons":
                {
                    WebtoonsBtn.Background = butonclickedBG;
                }
                    break;
                case "Backlog":
                {
                    BacklogBtn.Background = butonclickedBG;
                }
                    break;

                case "Debug":
                {
                    DebugBtn.Background = butonclickedBG;
                }
                    break;
                case "All":
                {
                    AllBtn.Background = butonclickedBG;
                    break;
                }
                case "Yomanga":
                {
                    YomangaBtn.Background = butonclickedBG;
                    break;
                }

            }
        }

        private void UpdateBatotoBtn_Click(object sender, RoutedEventArgs e)
        {
            var rssList = _batoto.Get_feed_titles();
            var jsMangaList = _parseFile.GetBatotoMangaNames();

            foreach (var rssTitle in rssList)
            {
                var name = rssTitle.Split(new[] { " - " }, StringSplitOptions.None)[0];
                if (jsMangaList.Contains(name) == false)
                {
                    jsMangaList.Add(name);
                    var match = Regex.Match(rssTitle, @".+ ch\.(\d+).+", RegexOptions.IgnoreCase);
                    _parseFile.AddManga("batoto", name, match.Groups[1].Value, "");
                    DebugText(string.Format("[{1}][Batoto] added {0}", name, DateTime.Now));
                }
            }
        }

        private void YomangaBtn_Click(object sender, RoutedEventArgs e)
        {
            if (DebugTextBox.Visibility == Visibility.Visible)
            {
                DebugTextBox.Visibility = Visibility.Collapsed;
                DataGridMangas.Visibility = Visibility.Visible;
            }

            _siteSelected = "Yomanga";
            // ButtonColorChange();
            ClosePanels();
        }

        private void YomangaOnOffBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!Equals(YomangaOnOffBtn.Background, oncolor))
            {
                YomangaOnOffBtn.Background = oncolor;
                _parseFile.SetValueSettings("yomanga", "1");
                YomangaBtn.Visibility = Visibility.Visible;
            }
            else
            {
                YomangaOnOffBtn.Background = transp;
                _parseFile.SetValueSettings("yomanga", "0");
                YomangaBtn.Visibility = Visibility.Collapsed;
            }
        }
    }
}