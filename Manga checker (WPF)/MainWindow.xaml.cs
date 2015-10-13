using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Manga_checker.Adding;
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
        private string force = "";
        private ListBoxItem itm = new ListBoxItem();
        public List<string> mlist;
        private string SiteSelected = "all";
        private WebClient web = new WebClient();
        private readonly BatotoRSS batoto = new BatotoRSS();
        private readonly SolidColorBrush OffColorBg = new SolidColorBrush(Color.FromArgb(255, 140, 140, 140));
        private readonly SolidColorBrush onColorBg = new SolidColorBrush(Color.FromArgb(255, 5, 157, 228));
        private readonly ParseFile parseFile = new ParseFile();
        private readonly Window1 SettingsWnd = new Window1();
        public readonly DispatcherTimer timer = new DispatcherTimer();
        public ThreadStart childref;
        public Thread childThread;

        public MainWindow()
        {
            InitializeComponent();
            timer.Interval = TimeSpan.FromSeconds(5d);
            timer.Tick += timer_Tick;
            Settings.Default.Debug = "Debug shit goes in here!\n";
            //Write settings to disk
            Settings.Default.Save();

            var g = new NotificationWindow("Starting in 5...", 0, 5);
            g.Show();
        }

        public void debugtext(string text)
        {
            //Read
            Settings.Default.Debug += text + "\n";
            //Write settings to disk
            Settings.Default.Save();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (parseFile.GetValueSettings("mangareader") == "1")
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
            if (parseFile.GetValueSettings("mangastream") == "1")
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
            if (parseFile.GetValueSettings("mangafox") == "1")
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
            if (parseFile.GetValueSettings("batoto") == "1")
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
            if (parseFile.GetValueSettings("debug") == "1")
            {
                DebugBtn.Visibility = Visibility.Visible;
                if (DebugLine.Visibility != Visibility.Visible)
                    DebugLine.Visibility = Visibility.Hidden;
            }
            else
            {
                DebugBtn.Visibility = Visibility.Collapsed;
                DebugBtn.Visibility = Visibility.Collapsed;
            }
            if (SiteSelected == "mangastream")
            {
                listBox.Items.Clear();
                FillMangastream();
            }
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
            SettingsWnd.Close();
            timer.Stop();
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
            childref = CheckNow;
            childThread = new Thread(childref) {IsBackground = true};
            childThread.Start();

            //parseFile.AddToNotReadList("mangastream", "the seven deadly sins", 44);
            timer.Start();

            MangastreamLine.Visibility = Visibility.Collapsed;
            MangafoxLine.Visibility = Visibility.Collapsed;
            MangareaderLine.Visibility = Visibility.Collapsed;
            BatotoLine.Visibility = Visibility.Collapsed;
            DebugLine.Visibility = Visibility.Collapsed;
            BacklogLine.Visibility = Visibility.Collapsed;
            AllLine.Visibility = Visibility.Collapsed;

            Fill_list();
        }

        private void CheckNow()
        {
            var i = 5;
            var _count = 0;
            var ms = new MangastreamRSS();
            var mf = new MangafoxRSS();
            var mr = new MangareaderHTML();
            var ba = new BatotoRSS();

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

                    Dispatcher.BeginInvoke(new Action(delegate { CounterLbl.Content = _count.ToString(); }));
                }
                else
                {
                    if (parseFile.GetValueSettings("mangastream") == "1")
                    {
                        Dispatcher.BeginInvoke(
                            new Action(delegate { StatusLb.Content = "Status: Checking Mangastream"; }));
                        try
                        {
                            ms.checked_if_new();
                        }
                        catch (Exception mst)
                        {
                            //throw;
                            debugtext(string.Format("[{0}][Mangastream] Error {1}", DateTime.Now, mst.Message));
                        }
                    }
                    if (parseFile.GetValueSettings("mangafox") == "1")
                    {
                        Dispatcher.BeginInvoke(new Action(delegate { StatusLb.Content = "Status: Checking Mangafox"; }));
                        foreach (var manga in parseFile.Mangafox_manga())
                        {
                            //debugtext(string.Format("[{0}][Mangafox] Checking {1}.", DateTime.Now, manga.Replace("[]", " ")));
                            mf.check_all(manga);
                        }
                    }
                    if (parseFile.GetValueSettings("mangareader") == "1")
                    {
                        Dispatcher.BeginInvoke(
                            new Action(delegate { StatusLb.Content = "Status: Checking Mangareader"; }));
                        foreach (var manga in parseFile.Mangareader_manga())
                        {
                            try
                            {
                                //debugtext(string.Format("[{0}][Mangareader] Checking {1}.", DateTime.Now,manga.Replace("[]", " ")));
                                mr.Check(manga);
                                Thread.Sleep(1000);
                            }
                            catch (Exception mrd)
                            {
                                // lol
                                debugtext(string.Format("[{1}][Mangareader] Error {0} {2}.", manga.Replace("[]", " "),
                                    DateTime.Now, mrd.Message));
                            }
                        }
                    }
                    if (parseFile.GetValueSettings("batoto") == "1")
                    {
                        Dispatcher.BeginInvoke(new Action(delegate { StatusLb.Content = "Status: Checking Batoto"; }));
                        try
                        {
                            mlist = ba.Get_feed_titles();
                            ba.Check();
                            Thread.Sleep(1000);
                        }
                        catch (Exception bat)
                        {
                            // lol
                            debugtext(string.Format("[{0}][batoto] Error {1}.", DateTime.Now, bat.Message));
                        }
                    }
                    //timer2.Start();
                    var waittime = int.Parse(parseFile.GetValueSettings("refresh time"));

                    Dispatcher.BeginInvoke(
                        new Action(delegate { StatusLb.Content = @"Status: Checking in " + waittime + " seconds."; }));
                    _count++;
                    i = waittime;
                }
            }
        }

        private void FillMangastream()
        {
            var itmheader = new ListBoxItem
            {
                Foreground = OffColorBg,
                Tag = "MangastreamHeader",
                Content = "-Mangastream",
                IsEnabled = false
            };
            listBox.Items.Add(itmheader);
            listBox.Items.Add(new Separator());
            foreach (var manga in parseFile.Mangastream_manga())
            {
                var listBoxItem = new ListBoxItem();
                listBoxItem.Content = manga.Replace("[]", " : ");
                listBoxItem.Foreground = OffColorBg;
                listBoxItem.Tag = "mangastream";
                listBox.Items.Add(listBoxItem);
            }
        }

        private void FillMangareader()
        {
            var itmheader = new ListBoxItem();
            itmheader.Foreground = OffColorBg;
            itmheader.Tag = "MangareaderHeader";
            itmheader.Content = "-Mangareader";
            itmheader.IsEnabled = false;
            listBox.Items.Add(itmheader);
            listBox.Items.Add(new Separator());
            foreach (var manga in parseFile.Mangareader_manga())
            {
                var name = manga.Split(new[] {"[]"}, StringSplitOptions.None);
                if (parseFile.GetValueStatus("mangareader", name[0]) == "true")
                {
                    //NotificationWindow nfw = new NotificationWindow(manga.Replace("[]", " : "));
                    //nfw.Show();
                    var listBoxItem = new ListBoxItem();
                    listBoxItem.Content = manga.Replace("[]", " : ");
                    listBoxItem.Foreground = onColorBg;
                    listBoxItem.Tag = "mangareader";
                    listBox.Items.Add(listBoxItem);
                }
                else
                {
                    var listBoxItem = new ListBoxItem();
                    listBoxItem.Content = manga.Replace("[]", " : ");
                    listBoxItem.Foreground = OffColorBg;
                    listBoxItem.Tag = "mangareader";
                    listBox.Items.Add(listBoxItem);
                }
            }
        }

        private void Fillbatoto()
        {
            var itmheader = new ListBoxItem();
            itmheader.Foreground = OffColorBg;
            itmheader.Tag = "BatotoHeader";
            itmheader.Content = "-Batoto";
            itmheader.IsEnabled = false;
            listBox.Items.Add(itmheader);
            listBox.Items.Add(new Separator());
            foreach (var manga in parseFile.Batoto_manga())
            {
                var name = manga.Split(new[] {"[]"}, StringSplitOptions.None);
                if (parseFile.GetValueStatus("batoto", name[0]) == "true")
                    // && parseFile.GetNotReadList("batoto", name[0]).Count > 0)
                {
                    //NotificationWindow nfw = new NotificationWindow(manga.Replace("[]", " : "));
                    //nfw.Show();
                    var listBoxItem = new ListBoxItem
                    {
                        Content = manga.Replace("[]", " : "),
                        Foreground = onColorBg,
                        Tag = "batoto"
                    };
                    listBox.Items.Add(listBoxItem);
                }
                else
                {
                    var listBoxItem = new ListBoxItem
                    {
                        Content = manga.Replace("[]", " : "),
                        Foreground = OffColorBg,
                        Tag = "batoto"
                    };
                    listBox.Items.Add(listBoxItem);
                }
            }
        }

        private void FillMangafox()
        {
            var itmheader = new ListBoxItem
            {
                Foreground = OffColorBg,
                Tag = "MangafoxHeader",
                Content = "-Mangafox",
                IsEnabled = false
            };
            listBox.Items.Add(itmheader);
            listBox.Items.Add(new Separator());
            foreach (var manga in parseFile.Mangafox_manga())
            {
                var name = manga.Split(new[] {"[]"}, StringSplitOptions.None);
                if (parseFile.GetValueStatus("mangafox", name[0]) == "true")
                {
                    //NotificationWindow nfw = new NotificationWindow(manga.Replace("[]", " : "));
                    //nfw.Show();
                    var listBoxItem = new ListBoxItem
                    {
                        Content = manga.Replace("[]", " : "),
                        Foreground = onColorBg,
                        Tag = "mangafox"
                    };
                    listBox.Items.Add(listBoxItem);
                }
                else
                {
                    var listBoxItem = new ListBoxItem
                    {
                        Content = manga.Replace("[]", " : "),
                        Foreground = OffColorBg,
                        Tag = "mangafox"
                    };
                    listBox.Items.Add(listBoxItem);
                }
            }
        }

        private void FillBacklog()
        {
            var itmheader = new ListBoxItem
            {
                Foreground = OffColorBg,
                Tag = "BacklogHeader",
                Content = "-Backlog",
                IsEnabled = false
            };
            listBox.Items.Add(itmheader);
            listBox.Items.Add(new Separator());
            foreach (var manga in parseFile.GetBacklog())
            {
                var listBoxItem = new ListBoxItem();
                listBoxItem.Content = manga;
                listBoxItem.Foreground = OffColorBg;
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


            SiteSelected = "all";
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
            AllLine.Visibility = Visibility.Visible;

            Fill_list();
            SiteSelected = "all";
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
            AllLine.Visibility = Visibility.Hidden;

            SiteSelected = "mangastream";
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
            AllLine.Visibility = Visibility.Hidden;

            SiteSelected = "mangafox";
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
            MangareaderLine.Visibility = Visibility.Visible;
            AllLine.Visibility = Visibility.Hidden;

            SiteSelected = "mangareader";
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
            if (site == "mangafox")
            {
                //open mangafox site for current chapter
                Process.Start("http://mangafox.me/manga/" +
                              name.Replace(":", "_").Replace("(", "").Replace(")", "").Replace(", ", "_")
                                  .Replace(" - ", " ")
                                  .Replace("-", "_")
                                  .Replace(" ", "_")
                                  .Replace("'", "_")
                                  .Replace("! -", "_")
                                  .Replace("!", "")
                                  .Replace(". ", "_")
                                  .Replace(".", "")
                                  .Replace("! ", "_").Replace("-", "_").Replace(":", "_") + "/c" + chapter + "/1.html");

                if (parseFile.GetNotReadList("mangafox", name).Contains(float.Parse(chapter)))
                    parseFile.RemoveFromNotRead("mangafox", name, float.Parse(chapter));
            }
            if (site == "mangareader")
            {
                //open mangareader site for current chapter
                if (chapter.Contains(" "))
                {
                    var chaptersplit = chapter.Split(new[] {" "}, StringSplitOptions.None);
                    Process.Start("http://www.mangareader.net/" +
                                  name.Replace(" ", "-").Replace("!", "").Replace(":", "") + "/" + chaptersplit[0]);
                    if (parseFile.GetNotReadList("mangareader", name).Contains(float.Parse(chaptersplit[0])))
                        parseFile.RemoveFromNotRead("mangareader", name, float.Parse(chaptersplit[0]));
                }
                else
                {
                    Process.Start("http://www.mangareader.net/" +
                                  name.Replace(" ", "-").Replace("!", "").Replace(":", "") + "/" + chapter);
                    if (parseFile.GetNotReadList("mangareader", name).Contains(float.Parse(chapter)))
                        parseFile.RemoveFromNotRead("mangareader", name, float.Parse(chapter));
                }
            }
            if (site == "batoto")
            {
                foreach (var mangarss in mlist)
                {
                    if (mangarss.ToLower().Contains(name.ToLower()) &&
                        mangarss.ToLower().Contains(chapter.ToLower()))
                    {
                        var link = mangarss.Split(new[] {"[]"}, StringSplitOptions.None)[1];
                        Process.Start(link);
                        if (parseFile.GetNotReadList("batoto", name).Contains(float.Parse(chapter)))
                            parseFile.RemoveFromNotRead("batoto", name, float.Parse(chapter));
                        var intcrch = float.Parse(parseFile.GetValueChapter("batoto", name));
                        if (float.Parse(chapter) > intcrch)
                        {
                            parseFile.setManga("batoto", name, chapter, "true");
                        }
                    }
                }
            }

            if (SiteSelected == "mangastream")
            {
                //open mangafox site for current chapter
            }
            //MessageBox.Show(listBox.SelectedItem.ToString());
            if (SiteSelected == " mangareader")
            {
                listBox.Items.Clear();
                FillMangareader();
            }
            else if (SiteSelected == "mangafox")
            {
                listBox.Items.Clear();
                FillMangafox();
            }
            else if (SiteSelected == "all")
            {
                listBox.Items.Clear();
                Fill_list();
            }
        }

        private void listBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var itemselected = ((ListBoxItem) listBox.SelectedItem);

                if (itemselected.Tag.ToString() == "mangafox")
                {
                    //open mangafox site for current chapter
                    var item = itemselected.Content.ToString();
                    var name = item.Split(new[] {" : "}, StringSplitOptions.None);
                    Process.Start("http://mangafox.me/manga/" +
                                  name[0].Replace(":", "_").Replace("(", "").Replace(")", "").Replace(", ", "_")
                                      .Replace(" - ", " ")
                                      .Replace("-", "_")
                                      .Replace(" ", "_")
                                      .Replace("'", "_")
                                      .Replace("! -", "_")
                                      .Replace("!", "")
                                      .Replace(". ", "_")
                                      .Replace(".", "")
                                      .Replace("! ", "_").Replace("-", "_").Replace(":", "_").Replace("__", "_") + "/c" +
                                  name[1] +
                                  "/1.html");
                    if (itemselected.Foreground.Equals(onColorBg))
                    {
                        parseFile.SetValueStatus("mangafox", name[0], "false");
                    }
                }
                if (itemselected.Tag.ToString() == "mangareader")
                {
                    //open mangareader site for current chapter
                    var item = itemselected.Content.ToString();
                    var name = item.Split(new[] {" : "}, StringSplitOptions.None);
                    if (name[1].Contains(" "))
                    {
                        var chapter = name[1].Split(new[] {" "}, StringSplitOptions.None);
                        Process.Start("http://www.mangareader.net/" +
                                      name[0].Replace(" ", "-").Replace("!", "").Replace(":", "") + "/" + chapter[0]);
                        if (itemselected.Foreground.Equals(onColorBg))
                        {
                            parseFile.SetValueStatus("mangareader", name[0], "false");
                        }
                    }
                    else
                    {
                        Process.Start("http://www.mangareader.net/" +
                                      name[0].Replace(" ", "-").Replace("!", "").Replace(":", "") + "/" + name[1]);
                        if (itemselected.Foreground.Equals(onColorBg))
                        {
                            parseFile.SetValueStatus("mangareader", name[0], "false");
                        }
                    }
                }
                if (itemselected.Tag.ToString() == "batoto")
                {
                    var item = itemselected.Content.ToString();
                    var split = item.Split(new[] {" : "}, StringSplitOptions.None);
                    var name = split[0];
                    var chapter = split[1];
                    var intchapter = float.Parse(chapter);
                    intchapter++;
                    foreach (var mangarss in mlist)
                    {
                        if (mangarss.ToLower().Contains(name.ToLower()) &&
                            mangarss.ToLower().Contains(chapter.ToLower()))
                        {
                            var link = mangarss.Split(new[] {"[]"}, StringSplitOptions.None)[1];
                            Process.Start(link);
                            if (itemselected.Foreground.Equals(onColorBg) &&
                                parseFile.GetNotReadList("batoto", name).Count == 0)
                            {
                                parseFile.SetValueStatus("batoto", name, "false");
                            }
                            if (parseFile.GetNotReadList("batoto", name).Contains(intchapter))
                            {
                                parseFile.setManga("batoto", name, intchapter.ToString(), "true");
                                parseFile.RemoveFromNotRead("batoto", name, intchapter);
                            }
                            else
                            {
                                parseFile.SetValueStatus("batoto", name, "false");
                            }
                        }
                    }
                }
            }
            catch (NullReferenceException)
            {
                //do nothing
                debugtext(string.Format("[{0}][Error] NullReferenceException", DateTime.Now));
            }
            finally
            {
                if (SiteSelected == "mangastream")
                {
                    //open mangafox site for current chapter
                }
                //MessageBox.Show(listBox.SelectedItem.ToString());
                if (SiteSelected == " mangareader")
                {
                    listBox.Items.Clear();
                    FillMangareader();
                }
                else if (SiteSelected == "mangafox")
                {
                    listBox.Items.Clear();
                    FillMangafox();
                }
                else if (SiteSelected == "all")
                {
                    listBox.Items.Clear();
                    Fill_list();
                }
                else if (SiteSelected == "batoto")
                {
                    listBox.Items.Clear();
                    Fillbatoto();
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
            SettingsWnd.Show();
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            if (AddPanel.Visibility == Visibility.Collapsed)
            {
                linkbox.Text = "";
                AddPanel.Visibility = Visibility.Visible;
                AddBtn_Copy.Content = "Add";
                AddBtn_Copy.Foreground = OffColorBg;
                SiteNameLb.Content = "None";
                SiteNameLb.Foreground = OffColorBg;
                MangaNameLb.Content = "None";
                MangaNameLb.Foreground = OffColorBg;
                ChapterNumLb.Content = "None";
                ChapterNumLb.Foreground = OffColorBg;
            }
            else
            {
                AddPanel.Visibility = Visibility.Collapsed;
            }
        }

        private MenuItem CreateItem(string site, string name, string ch, string click, string header)
        {
            var item = new MenuItem();
            item.Foreground = onColorBg;
            item.Margin = new Thickness(+15, 0, -40, 0);
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
                var name = "";
                var chapter = "";
                float chfloat;
                float lastchc = 3; //last x chapters displayed
                string[] splitchapter;
                var itemselected = ((ListBoxItem) listBox.SelectedItem);
                List<float> chlist;
                if (itemselected.Tag.Equals("mangareader") || itemselected.Tag.Equals("mangafox") ||
                    itemselected.Tag.Equals("batoto") || itemselected.Tag.Equals("backlog"))
                {
                    var splititem = itemselected.Content.ToString().Split(new[] {" : "}, StringSplitOptions.None);
                    listBox.ContextMenu.Items.Clear();


                    name = splititem[0];
                    chapter = splititem[1];
                    if (itemselected.Tag.Equals("mangareader") && chapter.Contains(" "))
                    {
                        chlist = parseFile.GetNotReadList("mangareader", name);
                        splitchapter = chapter.Split(new[] {" "}, StringSplitOptions.None);
                        if (chlist.Count != 0)
                        {
                            listBox.ContextMenu.Items.Add(CreateItem(itemselected.Tag.ToString(), "Not read", "", "no",
                                "yes"));
                            foreach (var ch in chlist)
                            {
                                listBox.ContextMenu.Items.Add(CreateItem(itemselected.Tag.ToString(), name,
                                    ch + " " + splitchapter[1], "yes", "no"));
                            }
                        }

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
                        chlist = parseFile.GetNotReadList("mangareader", name);
                        if (chlist.Count != 0)
                        {
                            listBox.ContextMenu.Items.Add(CreateItem(itemselected.Tag.ToString(), "Not read", "", "no",
                                "yes"));
                            foreach (var ch in chlist)
                            {
                                listBox.ContextMenu.Items.Add(CreateItem(itemselected.Tag.ToString(), name,
                                    ch.ToString(),
                                    "yes", "no"));
                            }
                        }
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
                        chlist = parseFile.GetNotReadList("mangafox", name);
                        if (chlist.Count != 0)
                        {
                            listBox.ContextMenu.Items.Add(CreateItem(itemselected.Tag.ToString(), "Not read", "", "no",
                                "yes"));
                            foreach (var ch in chlist)
                            {
                                listBox.ContextMenu.Items.Add(CreateItem(itemselected.Tag.ToString(), name,
                                    ch.ToString(),
                                    "yes", "no"));
                            }
                        }
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
                        chlist = parseFile.GetNotReadList("batoto", name);
                        if (chlist.Count != 0)
                        {
                            listBox.ContextMenu.Items.Add(CreateItem(itemselected.Tag.ToString(), "Not read", "", "no",
                                "yes"));
                            foreach (var ch in chlist)
                            {
                                listBox.ContextMenu.Items.Add(CreateItem(itemselected.Tag.ToString(), name,
                                    ch.ToString(),
                                    "yes", "no"));
                            }
                        }
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
                            item.Foreground = onColorBg;
                            item.Margin = new Thickness(+15, 0, -40, 0);
                            item.Header = "Delete";
                            item.FontWeight = FontWeights.Bold;
                            item.Click += delegate
                            {
                                parseFile.RemoveManga("backlog", namem);
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
                debugtext($"[{DateTime.Now}][Error] {ex}");
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
            AllLine.Visibility = Visibility.Hidden;
            listBox.Items.Clear();
            Fillbatoto();
            SiteSelected = "batoto";
        }

        private void DebugBtn_Click(object sender, RoutedEventArgs e)
        {
            SiteSelected = "debug";
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
            DebugLine.Visibility = Visibility.Visible;
            AllLine.Visibility = Visibility.Hidden;
        }

        private void DebugTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            DebugTextBox.ScrollToEnd();
        }

        private void AddMangaBtn_Click(object sender, RoutedEventArgs e)
        {
            if (AddBtn_Copy.Foreground.Equals(onColorBg))
            {
                // add the manga
                if (SiteNameLb.Content.ToString().ToLower().Contains("mangareader"))
                {
                    if (MangaNameLb.Content.ToString() != "ERROR" ||
                        MangaNameLb.Content.ToString() != "None" && ChapterNumLb.Content.ToString() != "None" ||
                        ChapterNumLb.Content.ToString() != "ERROR")
                    {
                        debugtext($"[{DateTime.Now}][Debug] Trying to add {MangaNameLb.Content} {ChapterNumLb.Content}");
                        parseFile.AddManga("mangareader", MangaNameLb.Content.ToString().ToLower(),
                            ChapterNumLb.Content.ToString(), "true");
                        AddBtn_Copy.Content = "Success!";
                    }
                }
                if (SiteNameLb.Content.ToString().ToLower().Contains("mangafox"))
                {
                    if (!MangaNameLb.Content.ToString().Equals("ERROR") &&
                        MangaNameLb.Content.ToString() != "None" && ChapterNumLb.Content.ToString() != "None" &&
                        ChapterNumLb.Content.ToString() != "ERROR")
                    {
                        debugtext($"[{DateTime.Now}][Debug] Trying to add {MangaNameLb.Content} {ChapterNumLb.Content}");
                        parseFile.AddManga("mangafox", MangaNameLb.Content.ToString().ToLower(),
                            ChapterNumLb.Content.ToString(), "true");
                        AddBtn_Copy.Content = "Success!";
                    }
                }
                if (SiteNameLb.Content.ToString().ToLower().Contains("mangastream"))
                {
                    if (!MangaNameLb.Content.ToString().Equals("ERROR") &&
                        MangaNameLb.Content.ToString() != "None" && ChapterNumLb.Content.ToString() != "None" &&
                        ChapterNumLb.Content.ToString() != "ERROR")
                    {
                        debugtext($"[{DateTime.Now}][Debug] Trying to add {MangaNameLb.Content} {ChapterNumLb.Content}");
                        parseFile.AddManga("mangastream", MangaNameLb.Content.ToString().ToLower(),
                            ChapterNumLb.Content.ToString(), "true");
                        AddBtn_Copy.Content = "Success!";
                    }
                }
            }
            if (linkbox.Text.ToLower().Contains("bato.to/myfollows_rss?secret="))
            {
                var RSSList = batoto.Get_feed_titles();
                var JSMangaList = parseFile.GetBatotoMangaNames();

                foreach (var RSSTitle in RSSList)
                {
                    var name = RSSTitle.Split(new[] {" - "}, StringSplitOptions.None)[0];
                    if (JSMangaList.Contains(name) == false)
                    {
                        JSMangaList.Add(name);
                        var match = Regex.Match(RSSTitle, @".+ ch\.(\d+).+", RegexOptions.IgnoreCase);
                        parseFile.AddManga("batoto", name, match.Groups[1].Value, "false");
                        debugtext(string.Format("[{1}][Batoto] added {0}", name, DateTime.Now));
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
            MangaNameLb.Foreground = onColorBg;
            ChapterNumLb.Content = m.Chapter;
            ChapterNumLb.Foreground = onColorBg;
            SiteNameLb.Content = m.Site;
            SiteNameLb.Foreground = onColorBg;
            AddBtn_Copy.Foreground = onColorBg;
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
            AllLine.Visibility = Visibility.Hidden;

            SiteSelected = "Backlog";
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
            parseFile.AddMangatoBacklog("backlog", backlognamebox.Text, backlogchapterbox.Text);
            backlognamebox.Text = string.Empty;
            backlogchapterbox.Text = string.Empty;
            SiteSelected = "Backlog";
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
            var backl = parseFile.GetBacklog();
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
    }
}