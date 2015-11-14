using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Manga_checker.Adding;
using Manga_checker.Handlers;
using Manga_checker.Properties;
using Manga_checker.Sites;

namespace Manga_checker
{
    /// <summary>
    ///     Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow AppWindow;
        //private WebClient web = new WebClient();
        private readonly BatotoRSS _batoto = new BatotoRSS();
        private readonly SolidColorBrush _offColorBg = new SolidColorBrush(Color.FromArgb(255, 140, 140, 140));
        private readonly SolidColorBrush _onColorBg = new SolidColorBrush(Color.FromArgb(255, 5, 157, 228));
        private readonly ParseFile _parseFile = new ParseFile();
        private readonly Window1 _settingsWnd = new Window1();
        public readonly DispatcherTimer Timer = new DispatcherTimer();
        private string _siteSelected = "all";
        public ThreadStart Childref;
        public Thread ChildThread;
        private string force = "";
        //private ListBoxItem itm = new ListBoxItem();
        public List<string> mlist;
        
        public MainWindow()
        {
            InitializeComponent();
            Timer.Interval = TimeSpan.FromSeconds(5d);
            Timer.Tick += timer_Tick;
            Settings.Default.Debug = "Debug shit goes in here!\n";

            //var g = new NotificationWindow("Starting in 5...", 0, 5);
            //g.Show();
            //WebtoonsRSS toons = new WebtoonsRSS();
            //toons.Check();
        }

        public void DebugText(string text)
        {
            //Read
            Settings.Default.Debug += text + "\n";
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (_parseFile.GetValueSettings("mangareader") == "1")
            {
                MangareaderBtn.Visibility = Visibility.Visible;
                if (MangareaderLine.Visibility != Visibility.Visible)
                    MangareaderLine.Visibility = Visibility.Hidden;
            }
            else
            {
                MangareaderBtn.Visibility = Visibility.Collapsed;
                MangareaderLine.Visibility = Visibility.Collapsed;
            }
            if (_parseFile.GetValueSettings("mangastream") == "1")
            {
                MangastreamBtn.Visibility = Visibility.Visible;
                if (MangastreamLine.Visibility != Visibility.Visible)
                    MangastreamLine.Visibility = Visibility.Hidden;
            }
            else
            {
                MangastreamBtn.Visibility = Visibility.Collapsed;
                MangastreamLine.Visibility = Visibility.Collapsed;
            }
            if (_parseFile.GetValueSettings("mangafox") == "1")
            {
                MangafoxBtn.Visibility = Visibility.Visible;
                if (MangafoxLine.Visibility != Visibility.Visible)
                    MangafoxLine.Visibility = Visibility.Hidden;
            }
            else
            {
                MangafoxBtn.Visibility = Visibility.Collapsed;
                MangafoxLine.Visibility = Visibility.Collapsed;
            }
            if (_parseFile.GetValueSettings("batoto") == "1")
            {
                BatotoBtn.Visibility = Visibility.Visible;
                if (BatotoLine.Visibility != Visibility.Visible)
                    BatotoLine.Visibility = Visibility.Hidden;
            }
            else
            {
                BatotoBtn.Visibility = Visibility.Collapsed;
                BatotoLine.Visibility = Visibility.Collapsed;
            }
            if (_parseFile.GetValueSettings("debug") == "1")
            {
                DebugBtn.Visibility = Visibility.Visible;
                if (DebugLine.Visibility != Visibility.Visible)
                    DebugLine.Visibility = Visibility.Hidden;
            }
            else
            {
                DebugBtn.Visibility = Visibility.Collapsed;
                DebugLine.Visibility = Visibility.Collapsed;
            }
            if (_parseFile.GetValueSettings("kissmanga") == "1")
            {
                KissmangaBtn.Visibility = Visibility.Visible;
                if (KissmangaLine.Visibility != Visibility.Visible)
                    KissmangaLine.Visibility = Visibility.Hidden;
            }
            else
            {
                KissmangaBtn.Visibility = Visibility.Collapsed;
                KissmangaLine.Visibility = Visibility.Collapsed;
            }
            if (_parseFile.GetValueSettings("webtoons") == "1")
            {
                WebtoonsBtn.Visibility = Visibility.Visible;
                if (WebtoonsLine.Visibility != Visibility.Visible)
                    WebtoonsLine.Visibility = Visibility.Hidden;
            }
            else
            {
                WebtoonsBtn.Visibility = Visibility.Collapsed;
                WebtoonsLine.Visibility = Visibility.Collapsed;
            }
            //if (_siteSelected == "mangastream")
            //{
            //    listBox.Items.Clear();
            //    FillMangastream();
            //}
            //MessageBox.Show(listBox.SelectedItem.ToString());
            //if (SiteSelected == " mangareader")
            //{
            //    listBox.Items.Clear();
            //    FillMangareader();
            //}
            //else if (SiteSelected == "mangafox")
            //{
            //    listBox.Items.Clear();
            //    FillMangafox();
            //}
            //else if (SiteSelected == "batoto")
            //{
            //    listBox.Items.Clear();
            //    Fillbatoto();
            //}
            //else if (SiteSelected == "all")
            //{
            //    listBox.Items.Clear();
            //    Fill_list();
            //}
            //else if (SiteSelected == "debug")
            //{
            //    //do nothing
            //}
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            _settingsWnd.Close();
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

            //parseFile.AddToNotReadList("mangastream", "the seven deadly sins", 44);
            Timer.Start();

            MangastreamLine.Visibility = Visibility.Collapsed;
            MangafoxLine.Visibility = Visibility.Collapsed;
            MangareaderLine.Visibility = Visibility.Collapsed;
            BatotoLine.Visibility = Visibility.Collapsed;
            DebugLine.Visibility = Visibility.Collapsed;
            BacklogLine.Visibility = Visibility.Collapsed;
            KissmangaLine.Visibility = Visibility.Collapsed;
            WebtoonsLine.Visibility = Visibility.Collapsed;
            AllLine.Visibility = Visibility.Collapsed;

            Fill_list();
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
                            DebugText($"[{DateTime.Now}][Kissmanga] Error {to.Message}.");
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

        private void FillMangastream()
        {
            var itmheader = new ListBoxItem
            {
                Foreground = _offColorBg,
                Tag = "MangastreamHeader",
                Content = "-Mangastream",
                IsEnabled = false
            };
            listBox.Items.Add(itmheader);
            listBox.Items.Add(new Separator());
            foreach (var manga in _parseFile.GetManga("mangastream"))
            {
                var listBoxItem = new ListBoxItem();
                listBoxItem.Content = manga.Replace("[]", " : ");
                listBoxItem.Foreground = _offColorBg;
                listBoxItem.Tag = "mangastream";
                listBox.Items.Add(listBoxItem);
            }
        }

        private void FillMangareader()
        {
            var itmheader = new ListBoxItem();
            itmheader.Foreground = _offColorBg;
            itmheader.Tag = "MangareaderHeader";
            itmheader.Content = "-Mangareader";
            itmheader.IsEnabled = false;
            listBox.Items.Add(itmheader);
            listBox.Items.Add(new Separator());
            foreach (var manga in _parseFile.GetManga("mangareader"))
            {
                var name = manga.Split(new[] {"[]"}, StringSplitOptions.None);
                var listBoxItem = new ListBoxItem();
                listBoxItem.Content = manga.Replace("[]", " : ");
                listBoxItem.Foreground = _offColorBg;
                listBoxItem.Tag = "mangareader";
                listBox.Items.Add(listBoxItem);
                
            }
        }

        private void Fillbatoto()
        {
            var itmheader = new ListBoxItem();
            itmheader.Foreground = _offColorBg;
            itmheader.Tag = "BatotoHeader";
            itmheader.Content = "-Batoto";
            itmheader.IsEnabled = false;
            listBox.Items.Add(itmheader);
            listBox.Items.Add(new Separator());
            foreach (var manga in _parseFile.GetManga("batoto"))
            {
                var name = manga.Split(new[] {"[]"}, StringSplitOptions.None);
                var listBoxItem = new ListBoxItem
                {
                    Content = manga.Replace("[]", " : "),
                    Foreground = _offColorBg,
                    Tag = "batoto"
                };
                listBox.Items.Add(listBoxItem);
            }
        }

        private void FillMangafox()
        {
            var itmheader = new ListBoxItem
            {
                Foreground = _offColorBg,
                Tag = "MangafoxHeader",
                Content = "-Mangafox",
                IsEnabled = false
            };
            listBox.Items.Add(itmheader);
            listBox.Items.Add(new Separator());
            foreach (var manga in _parseFile.GetManga("mangafox"))
            {
                var name = manga.Split(new[] {"[]"}, StringSplitOptions.None);
                var listBoxItem = new ListBoxItem
                {
                    Content = manga.Replace("[]", " : "),
                    Foreground = _offColorBg,
                    Tag = "mangafox"
                };
                listBox.Items.Add(listBoxItem);
                
            }
        }

        private void FillBacklog()
        {
            var itmheader = new ListBoxItem
            {
                Foreground = _offColorBg,
                Tag = "BacklogHeader",
                Content = "-Backlog",
                IsEnabled = false
            };
            listBox.Items.Add(itmheader);
            listBox.Items.Add(new Separator());
            foreach (var manga in _parseFile.GetBacklog())
            {
                var listBoxItem = new ListBoxItem();
                listBoxItem.Content = manga;
                listBoxItem.Foreground = _offColorBg;
                listBoxItem.Tag = "backlog";
                listBox.Items.Add(listBoxItem);
            }
        }

        public void Fill_list()
        {
            MangastreamLine.Visibility = Visibility.Collapsed;
            MangafoxLine.Visibility = Visibility.Collapsed;
            MangareaderLine.Visibility = Visibility.Collapsed;
            DebugLine.Visibility = Visibility.Collapsed;
            BatotoLine.Visibility = Visibility.Collapsed;
            AllLine.Visibility = Visibility.Visible;


            _siteSelected = "all";
            listBox.Items.Clear();
            FillMangastream();

            var i = new ListBoxItem();
            i.Content = "";
            i.Tag = "blank";
            i.IsEnabled = false;
            listBox.Items.Add(i);
            FillMangafox();

            var i1 = new ListBoxItem();
            i1.Content = "";
            i1.Tag = "blank";
            i1.IsEnabled = false;
            listBox.Items.Add(i1);
            FillMangareader();

            var i2 = new ListBoxItem();
            i2.Content = "";
            i2.Tag = "blank";
            i2.IsEnabled = false;
            listBox.Items.Add(i2);
            Fillbatoto();
        }

        private void AllBtn_Click(object sender, RoutedEventArgs e)
        {
            if (DebugTextBox.Visibility == Visibility.Visible)
            {
                DebugTextBox.Visibility = Visibility.Collapsed;
                listBox.Visibility = Visibility.Visible;
            }
            MangastreamLine.Visibility = MangastreamBtn.Visibility == Visibility.Collapsed
                ? Visibility.Collapsed
                : Visibility.Hidden;
            MangafoxLine.Visibility = MangafoxBtn.Visibility == Visibility.Collapsed
                ? Visibility.Collapsed
                : Visibility.Hidden;
            MangareaderLine.Visibility = MangareaderBtn.Visibility == Visibility.Collapsed
                ? Visibility.Collapsed
                : Visibility.Hidden;
            BatotoLine.Visibility = BatotoBtn.Visibility == Visibility.Collapsed
                ? Visibility.Collapsed
                : Visibility.Hidden;
            DebugLine.Visibility = DebugBtn.Visibility == Visibility.Collapsed
                ? Visibility.Collapsed
                : Visibility.Hidden;
            BacklogLine.Visibility = BacklogBtn.Visibility == Visibility.Collapsed
                ? Visibility.Collapsed
                : Visibility.Hidden;
            KissmangaLine.Visibility = KissmangaBtn.Visibility == Visibility.Collapsed
                 ? Visibility.Collapsed
                 : Visibility.Hidden;
            WebtoonsLine.Visibility = WebtoonsBtn.Visibility == Visibility.Collapsed
                 ? Visibility.Collapsed
                 : Visibility.Hidden;
            AllLine.Visibility = Visibility.Visible;

            Fill_list();
            _siteSelected = "all";
        }

        private void MangastreamBtn_Click(object sender, RoutedEventArgs e)
        {
            if (DebugTextBox.Visibility == Visibility.Visible)
            {
                DebugTextBox.Visibility = Visibility.Collapsed;
                listBox.Visibility = Visibility.Visible;
            }

            MangastreamLine.Visibility = Visibility.Visible;
            MangafoxLine.Visibility = MangafoxBtn.Visibility == Visibility.Collapsed
                ? Visibility.Collapsed
                : Visibility.Hidden;
            MangareaderLine.Visibility = MangareaderBtn.Visibility == Visibility.Collapsed
                ? Visibility.Collapsed
                : Visibility.Hidden;
            BatotoLine.Visibility = BatotoBtn.Visibility == Visibility.Collapsed
                ? Visibility.Collapsed
                : Visibility.Hidden;
            DebugLine.Visibility = DebugBtn.Visibility == Visibility.Collapsed
                ? Visibility.Collapsed
                : Visibility.Hidden;
            BacklogLine.Visibility = BacklogBtn.Visibility == Visibility.Collapsed
                ? Visibility.Collapsed
                : Visibility.Hidden;
            KissmangaLine.Visibility = KissmangaBtn.Visibility == Visibility.Collapsed
                 ? Visibility.Collapsed
                 : Visibility.Hidden;
            WebtoonsLine.Visibility = WebtoonsBtn.Visibility == Visibility.Collapsed
                 ? Visibility.Collapsed
                 : Visibility.Hidden;
            AllLine.Visibility = Visibility.Hidden;

            _siteSelected = "mangastream";
            listBox.Items.Clear();
            FillMangastream();
        }

        private void MangafoxBtn_Click(object sender, RoutedEventArgs e)
        {
            if (DebugTextBox.Visibility == Visibility.Visible)
            {
                DebugTextBox.Visibility = Visibility.Collapsed;
                listBox.Visibility = Visibility.Visible;
            }

            MangastreamLine.Visibility = MangastreamBtn.Visibility == Visibility.Collapsed
                ? Visibility.Collapsed
                : Visibility.Hidden;
            MangafoxLine.Visibility = Visibility.Visible;
            MangareaderLine.Visibility = MangareaderBtn.Visibility == Visibility.Collapsed
                ? Visibility.Collapsed
                : Visibility.Hidden;
            BatotoLine.Visibility = BatotoBtn.Visibility == Visibility.Collapsed
                ? Visibility.Collapsed
                : Visibility.Hidden;
            DebugLine.Visibility = DebugBtn.Visibility == Visibility.Collapsed
                ? Visibility.Collapsed
                : Visibility.Hidden;
            BacklogLine.Visibility = BacklogBtn.Visibility == Visibility.Collapsed
                ? Visibility.Collapsed
                : Visibility.Hidden;
            KissmangaLine.Visibility = KissmangaBtn.Visibility == Visibility.Collapsed
                 ? Visibility.Collapsed
                 : Visibility.Hidden;
            WebtoonsLine.Visibility = WebtoonsBtn.Visibility == Visibility.Collapsed
                 ? Visibility.Collapsed
                 : Visibility.Hidden;
            AllLine.Visibility = Visibility.Hidden;

            _siteSelected = "mangafox";
            listBox.Items.Clear();
            FillMangafox();
        }

        private void MangareaderBtn_Click(object sender, RoutedEventArgs e)
        {
            if (DebugTextBox.Visibility == Visibility.Visible)
            {
                DebugTextBox.Visibility = Visibility.Collapsed;
                listBox.Visibility = Visibility.Visible;
            }

            MangastreamLine.Visibility = MangastreamBtn.Visibility == Visibility.Collapsed
                ? Visibility.Collapsed
                : Visibility.Hidden;
            MangafoxLine.Visibility = MangafoxBtn.Visibility == Visibility.Collapsed
                ? Visibility.Collapsed
                : Visibility.Hidden;
            BatotoLine.Visibility = BatotoBtn.Visibility == Visibility.Collapsed
                ? Visibility.Collapsed
                : Visibility.Hidden;
            DebugLine.Visibility = DebugBtn.Visibility == Visibility.Collapsed
                ? Visibility.Collapsed
                : Visibility.Hidden;
            BacklogLine.Visibility = BacklogBtn.Visibility == Visibility.Collapsed
                ? Visibility.Collapsed
                : Visibility.Hidden;
            KissmangaLine.Visibility = KissmangaBtn.Visibility == Visibility.Collapsed
                 ? Visibility.Collapsed
                 : Visibility.Hidden;
            WebtoonsLine.Visibility = WebtoonsBtn.Visibility == Visibility.Collapsed
                 ? Visibility.Collapsed
                 : Visibility.Hidden;
            MangareaderLine.Visibility = Visibility.Visible;
            AllLine.Visibility = Visibility.Hidden;

            _siteSelected = "mangareader";
            listBox.Items.Clear();
            FillMangareader();
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
            switch (_siteSelected)
            {
                case "mangastream":
                    {
                        //nothing
                        break;
                    }
                case "mangareader":
                    {
                        listBox.Items.Clear();
                        FillMangareader();
                        break;
                    }
                case "mangafox":
                    {
                        listBox.Items.Clear();
                        FillMangafox();
                        break;
                    }
                case "all":
                    {
                        listBox.Items.Clear();
                        Fill_list();
                        break;
                    }
                case "batoto":
                    {
                        listBox.Items.Clear();
                        Fillbatoto();
                        break;
                    }
            }
        }

        private void listBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var itemselected = ((ListBoxItem) listBox.SelectedItem);
                switch (itemselected.Tag.ToString())
                {
                    case "mangafox":
                    {
                        var name_chapter = itemselected.Content.ToString().Split(new[] {" : "}, StringSplitOptions.None);
                        OpenSite _openSite = new OpenSite();
                        _openSite.Open("mangafox", name_chapter[0], name_chapter[1], mlist);
                        DebugText($"[{DateTime.Now}][Debug] Opened {itemselected.Content} on {itemselected.Tag.ToString().ToUpper()}.");
                        break;
                    }
                    case "mangareader":
                    {
                        var name_chapter = itemselected.Content.ToString().Split(new[] { " : " }, StringSplitOptions.None);
                        OpenSite _openSite = new OpenSite();
                        _openSite.Open("mangareader", name_chapter[0], name_chapter[1], mlist);
                        DebugText($"[{DateTime.Now}][Debug] Opened {itemselected.Content} on {itemselected.Tag.ToString().ToUpper()}.");
                        break;
                    }
                    case "batoto":
                    {
                        var name_chapter = itemselected.Content.ToString().Split(new[] { " : " }, StringSplitOptions.None);
                        OpenSite _openSite = new OpenSite();
                        _openSite.Open("batoto", name_chapter[0], name_chapter[1], mlist);
                        DebugText($"[{DateTime.Now}][Debug] Opened {itemselected.Content} on {itemselected.Tag.ToString().ToUpper()}.");
                        break;
                    }
                }
            }
            catch (Exception g)
            {
                //do nothing
                DebugText($"[{DateTime.Now}][Error] {g.Message} {g.TargetSite} ");
            }
            finally
            {
                switch (_siteSelected)
                {
                    case "mangastream":
                    {
                        //nothing
                        break;
                    }
                    case "mangareader":
                    {
                            listBox.Items.Clear();
                            FillMangareader();
                            break;
                    }
                    case "mangafox":
                    {
                            listBox.Items.Clear();
                            FillMangafox();
                            break;
                    }
                    case "all":
                    {
                            listBox.Items.Clear();
                            Fill_list();
                            break;
                    }
                    case "batoto":
                    {
                            listBox.Items.Clear();
                            Fillbatoto();
                            break;
                    }
                }
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
                TopMostBtn.Foreground = new SolidColorBrush(Colors.DodgerBlue);
            }
            else
            {
                Topmost = false;
                TopMostBtn.Foreground = new SolidColorBrush(Colors.Gray);
            }
        }

        private void SettingsBtn_Click(object sender, RoutedEventArgs e)
        {
            _settingsWnd.Show();
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            if (AddPanel.Visibility == Visibility.Collapsed)
            {
                linkbox.Text = "";
                AddPanel.Visibility = Visibility.Visible;
                AddBtn_Copy.Content = "Add";
                AddBtn_Copy.Foreground = _offColorBg;
                SiteNameLb.Content = "None";
                SiteNameLb.Foreground = _offColorBg;
                MangaNameLb.Content = "None";
                MangaNameLb.Foreground = _offColorBg;
                ChapterNumLb.Content = "None";
                ChapterNumLb.Foreground = _offColorBg;
            }
            else
            {
                AddPanel.Visibility = Visibility.Collapsed;
            }
        }

        private MenuItem CreateItem(string site, string name, string ch, string click, string header)
        {
            var item = new MenuItem
            {
                Foreground = _onColorBg,
                Margin = new Thickness(+15, 0, -40, 0)
            };
            if (header == "yes")
            {
                item.Header = name;
                item.FontWeight = FontWeights.Bold;
                item.IsEnabled = false;
            }
            if (click == "yes")
                item.Header = name + " : " + ch;
            item.Click += (sender, e) => OpenSite(site, name, ch);
            return item;
        }

        // ReSharper disable once FunctionComplexityOverflow
        private void listBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                const float lastchc = 3; //last x chapters displayed
                var itemselected = ((ListBoxItem) listBox.SelectedItem);
                if (itemselected.Tag.Equals("mangareader") || itemselected.Tag.Equals("mangafox") ||
                    itemselected.Tag.Equals("batoto") || itemselected.Tag.Equals("backlog"))
                {
                    var splititem = itemselected.Content.ToString().Split(new[] {" : "}, StringSplitOptions.None);
                    listBox.ContextMenu.Items.Clear();


                    var name = splititem[0];
                    var chapter = splititem[1];
                    float chfloat;
                    if (itemselected.Tag.Equals("mangareader") && chapter.Contains(" "))
                    {
                        var splitchapter = chapter.Split(new[] {" "}, StringSplitOptions.None);
                        
                        listBox.ContextMenu.Items.Add(CreateItem(itemselected.Tag.ToString(), "Last 3 Chapter's", "",
                            "no", "yes"));
                        chfloat = float.Parse(splitchapter[0]);
                        for (float i = 0; i < lastchc; i++)
                        {
                            chfloat--;
                            listBox.ContextMenu.Items.Add(CreateItem(itemselected.Tag.ToString(), name,
                                chfloat + " " + splitchapter[1], "yes", "no"));
                        }
                    }
                    else if (itemselected.Tag.Equals("mangareader") && chapter.Contains(" ") == false)
                    {
                        listBox.ContextMenu.Items.Add(CreateItem(itemselected.Tag.ToString(), "Last 3 Chapter's", "",
                            "no", "yes"));
                        chfloat = float.Parse(chapter);
                        for (float i = 0; i < lastchc; i++)
                        {
                            chfloat--;
                            listBox.ContextMenu.Items.Add(CreateItem(itemselected.Tag.ToString(), name,
                                chfloat.ToString(), "yes", "no"));
                        }
                    }
                    if (itemselected.Tag.Equals("mangafox"))
                    {
                        listBox.ContextMenu.Items.Add(CreateItem(itemselected.Tag.ToString(), "Last 3 Chapter's", "",
                            "no", "yes"));
                        chfloat = float.Parse(chapter);
                        for (float i = 0; i < lastchc; i++)
                        {
                            chfloat--;
                            listBox.ContextMenu.Items.Add(CreateItem(itemselected.Tag.ToString(), name,
                                chfloat.ToString(), "yes", "no"));
                        }
                    }

                    if (itemselected.Tag.Equals("batoto"))
                    {
                        listBox.ContextMenu.Items.Add(CreateItem(itemselected.Tag.ToString(), "Last ~3 Chapter's", "",
                            "no", "yes"));
                        chfloat = float.Parse(chapter);
                        float ic = 1;
                        foreach (var mangarss in mlist)
                        {
                            var match = Regex.Match(mangarss, @".+ ch\.(\d*\.?\d*).+", RegexOptions.IgnoreCase);
                            var matchvalue = match.Groups[1].Value;
                            if (chfloat > float.Parse(matchvalue) && mangarss.ToLower().Contains(name.ToLower()) &&
                                ic <= lastchc)
                            {
                                listBox.ContextMenu.Items.Add(CreateItem(itemselected.Tag.ToString(), name,
                                    matchvalue, "yes", "no"));
                                ic++;
                            }
                        }
                    }
                    if (itemselected.Tag.Equals("backlog"))
                    {
                        try
                        {
                            var namem =
                                itemselected.Content.ToString()
                                    .Split(new[] {" : "}, StringSplitOptions.None)[0];
                            //TODO: move to... buttons
                            var item = new MenuItem();
                            item.Foreground = _onColorBg;
                            item.Margin = new Thickness(+15, 0, -40, 0);
                            item.Header = "Delete";
                            item.FontWeight = FontWeights.Bold;
                            item.Click += delegate
                            {
                                _parseFile.RemoveManga("backlog", namem);
                                listBox.Items.Clear();
                                FillBacklog();
                            };
                            listBox.ContextMenu.Items.Add(item);
                        }
                        catch (Exception d)
                        {
                            MessageBox.Show(d.Message);
                        }
                    }
                }
                else
                {
                    listBox.ContextMenu.Items.Clear();
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
                listBox.Visibility = Visibility.Visible;
            }
            BatotoLine.Visibility = Visibility.Visible;
            MangastreamLine.Visibility = MangastreamBtn.Visibility == Visibility.Collapsed
                ? Visibility.Collapsed
                : Visibility.Hidden;
            MangafoxLine.Visibility = MangafoxBtn.Visibility == Visibility.Collapsed
                ? Visibility.Collapsed
                : Visibility.Hidden;
            MangareaderLine.Visibility = MangareaderBtn.Visibility == Visibility.Collapsed
                ? Visibility.Collapsed
                : Visibility.Hidden;
            DebugLine.Visibility = DebugBtn.Visibility == Visibility.Collapsed
                ? Visibility.Collapsed
                : Visibility.Hidden;
            BacklogLine.Visibility = BacklogBtn.Visibility == Visibility.Collapsed
                ? Visibility.Collapsed
                : Visibility.Hidden;
            KissmangaLine.Visibility = KissmangaBtn.Visibility == Visibility.Collapsed
                 ? Visibility.Collapsed
                 : Visibility.Hidden;
            WebtoonsLine.Visibility = WebtoonsBtn.Visibility == Visibility.Collapsed
                 ? Visibility.Collapsed
                 : Visibility.Hidden;
            AllLine.Visibility = Visibility.Hidden;
            listBox.Items.Clear();
            Fillbatoto();
            _siteSelected = "batoto";
        }

        private void DebugBtn_Click(object sender, RoutedEventArgs e)
        {
            _siteSelected = "debug";
            if (listBox.Visibility == Visibility.Visible)
            {
                DebugTextBox.Visibility = Visibility.Visible;
                listBox.Visibility = Visibility.Collapsed;
            }
            BatotoLine.Visibility = BatotoBtn.Visibility == Visibility.Collapsed
                ? Visibility.Collapsed
                : Visibility.Hidden;
            MangastreamLine.Visibility = MangastreamBtn.Visibility == Visibility.Collapsed
                ? Visibility.Collapsed
                : Visibility.Hidden;
            MangafoxLine.Visibility = MangafoxBtn.Visibility == Visibility.Collapsed
                ? Visibility.Collapsed
                : Visibility.Hidden;
            MangareaderLine.Visibility = MangareaderBtn.Visibility == Visibility.Collapsed
                ? Visibility.Collapsed
                : Visibility.Hidden;
            BacklogLine.Visibility = BacklogBtn.Visibility == Visibility.Collapsed
                ? Visibility.Collapsed
                : Visibility.Hidden;
            KissmangaLine.Visibility = KissmangaBtn.Visibility == Visibility.Collapsed
                 ? Visibility.Collapsed
                 : Visibility.Hidden;
            WebtoonsLine.Visibility = WebtoonsBtn.Visibility == Visibility.Collapsed
                 ? Visibility.Collapsed
                 : Visibility.Hidden;
            DebugLine.Visibility = Visibility.Visible;
            AllLine.Visibility = Visibility.Hidden;
        }

        private void DebugTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            DebugTextBox.ScrollToEnd();
        }

        private void AddMangaBtn_Click(object sender, RoutedEventArgs e)
        {
            if (AddBtn_Copy.Foreground.Equals(_onColorBg))
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
            if (linkbox.Text.ToLower().Contains("bato.to/myfollows_rss?secret="))
            {
                var rssList = _batoto.Get_feed_titles();
                var jsMangaList = _parseFile.GetBatotoMangaNames();

                foreach (var rssTitle in rssList)
                {
                    var name = rssTitle.Split(new[] {" - "}, StringSplitOptions.None)[0];
                    if (jsMangaList.Contains(name) == false)
                    {
                        jsMangaList.Add(name);
                        var match = Regex.Match(rssTitle, @".+ ch\.(\d+).+", RegexOptions.IgnoreCase);
                        _parseFile.AddManga("batoto", name, match.Groups[1].Value, "");
                        DebugText(string.Format("[{1}][Batoto] added {0}", name, DateTime.Now));
                    }
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
            MangaNameLb.Foreground = _onColorBg;
            ChapterNumLb.Content = m.Chapter;
            ChapterNumLb.Foreground = _onColorBg;
            SiteNameLb.Content = m.Site;
            SiteNameLb.Foreground = _onColorBg;
            AddBtn_Copy.Foreground = _onColorBg;
        }

        private void BacklogBtn_Click(object sender, RoutedEventArgs e)
        {
            if (DebugTextBox.Visibility == Visibility.Visible)
            {
                DebugTextBox.Visibility = Visibility.Collapsed;
                listBox.Visibility = Visibility.Visible;
            }

            BacklogLine.Visibility = Visibility.Visible;
            MangafoxLine.Visibility = MangafoxBtn.Visibility == Visibility.Collapsed
                ? Visibility.Collapsed
                : Visibility.Hidden;
            MangareaderLine.Visibility = MangareaderBtn.Visibility == Visibility.Collapsed
                ? Visibility.Collapsed
                : Visibility.Hidden;
            BatotoLine.Visibility = BatotoBtn.Visibility == Visibility.Collapsed
                ? Visibility.Collapsed
                : Visibility.Hidden;
            DebugLine.Visibility = DebugBtn.Visibility == Visibility.Collapsed
                ? Visibility.Collapsed
                : Visibility.Hidden;
            MangastreamLine.Visibility = MangastreamBtn.Visibility == Visibility.Collapsed
                ? Visibility.Collapsed
                : Visibility.Hidden;
            KissmangaLine.Visibility = KissmangaBtn.Visibility == Visibility.Collapsed
                 ? Visibility.Collapsed
                 : Visibility.Hidden;
            WebtoonsLine.Visibility = WebtoonsBtn.Visibility == Visibility.Collapsed
                 ? Visibility.Collapsed
                 : Visibility.Hidden;
            AllLine.Visibility = Visibility.Hidden;

            _siteSelected = "Backlog";
            listBox.Items.Clear();
            FillBacklog();
        }

        public void BacklogShow(object sender, RoutedEventArgs e)
        {
            BacklogAddMenu.Visibility = Visibility.Visible;
        }

        private void backlogaddbtn_Click(object sender, RoutedEventArgs e)
        {
            if (backlognamebox.Text == string.Empty || backlogchapterbox.Text == string.Empty)
            {
                BacklogAddMenu.Visibility = Visibility.Collapsed;
                return;
            }
            _parseFile.AddMangatoBacklog("backlog", backlognamebox.Text, backlogchapterbox.Text);
            backlognamebox.Text = string.Empty;
            backlogchapterbox.Text = string.Empty;
            _siteSelected = "Backlog";
            listBox.Items.Clear();
            FillBacklog();
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
                listBox.Visibility = Visibility.Visible;
            }

            KissmangaLine.Visibility = Visibility.Visible;
            MangafoxLine.Visibility = MangafoxBtn.Visibility == Visibility.Collapsed
                ? Visibility.Collapsed
                : Visibility.Hidden;
            MangareaderLine.Visibility = MangareaderBtn.Visibility == Visibility.Collapsed
                ? Visibility.Collapsed
                : Visibility.Hidden;
            BatotoLine.Visibility = BatotoBtn.Visibility == Visibility.Collapsed
                ? Visibility.Collapsed
                : Visibility.Hidden;
            DebugLine.Visibility = DebugBtn.Visibility == Visibility.Collapsed
                ? Visibility.Collapsed
                : Visibility.Hidden;
            MangastreamLine.Visibility = MangastreamBtn.Visibility == Visibility.Collapsed
                ? Visibility.Collapsed
                : Visibility.Hidden;
            BacklogLine.Visibility = backlogaddbtn.Visibility == Visibility.Collapsed
                ? Visibility.Collapsed
                : Visibility.Hidden;
            WebtoonsLine.Visibility = WebtoonsBtn.Visibility == Visibility.Collapsed
                 ? Visibility.Collapsed
                 : Visibility.Hidden;
            AllLine.Visibility = Visibility.Hidden;

            _siteSelected = "Kissmanga";
            listBox.Items.Clear();
            FillKissmanga();
        }

        public void FillKissmanga()
        {
            var itmheader = new ListBoxItem();
            itmheader.Foreground = _offColorBg;
            itmheader.Tag = "KissmangaHeader";
            itmheader.Content = "-Kissmanga";
            itmheader.IsEnabled = false;
            listBox.Items.Add(itmheader);
            listBox.Items.Add(new Separator());
            foreach (var manga in _parseFile.GetManga("kissmanga"))
            {
                var listBoxItem = new ListBoxItem
                {
                    Content = manga.Replace("[]", " : "),
                    Foreground = _offColorBg,
                    Tag = "kissmanga"
                };
                listBox.Items.Add(listBoxItem);
            }
        }
        public void FillWebtoons()
        {
            var itmheader = new ListBoxItem();
            itmheader.Foreground = _offColorBg;
            itmheader.Tag = "WebtoonsHeader";
            itmheader.Content = "-Webtoons";
            itmheader.IsEnabled = false;
            listBox.Items.Add(itmheader);
            listBox.Items.Add(new Separator());
            foreach (var manga in _parseFile.GetManga("webtoons"))
            {
                var splitterino = manga.Split(new[] {"[]"}, StringSplitOptions.None);
                var listBoxItem = new ListBoxItem
                {
                    Content = splitterino[0] + " : " + splitterino[1],
                    Foreground = _offColorBg,
                    Tag = "webtoons"
                };
                listBox.Items.Add(listBoxItem);
            }
        }

        private void WebtoonsBtn_Click(object sender, RoutedEventArgs e)
        {
            if (DebugTextBox.Visibility == Visibility.Visible)
            {
                DebugTextBox.Visibility = Visibility.Collapsed;
                listBox.Visibility = Visibility.Visible;
            }

            WebtoonsLine.Visibility = Visibility.Visible;
            MangafoxLine.Visibility = MangafoxBtn.Visibility == Visibility.Collapsed
                ? Visibility.Collapsed
                : Visibility.Hidden;
            MangareaderLine.Visibility = MangareaderBtn.Visibility == Visibility.Collapsed
                ? Visibility.Collapsed
                : Visibility.Hidden;
            BatotoLine.Visibility = BatotoBtn.Visibility == Visibility.Collapsed
                ? Visibility.Collapsed
                : Visibility.Hidden;
            DebugLine.Visibility = DebugBtn.Visibility == Visibility.Collapsed
                ? Visibility.Collapsed
                : Visibility.Hidden;
            MangastreamLine.Visibility = MangastreamBtn.Visibility == Visibility.Collapsed
                ? Visibility.Collapsed
                : Visibility.Hidden;
            BacklogLine.Visibility = backlogaddbtn.Visibility == Visibility.Collapsed
                ? Visibility.Collapsed
                : Visibility.Hidden;
            KissmangaLine.Visibility = KissmangaBtn.Visibility == Visibility.Collapsed
                 ? Visibility.Collapsed
                 : Visibility.Hidden;
            AllLine.Visibility = Visibility.Hidden;

            _siteSelected = "Webtoons";
            listBox.Items.Clear();
            FillWebtoons();
        }
    }
}