using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Windows.Threading;
using Manga_checker__WPF_.Properties;

namespace Manga_checker__WPF_
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow AppWindow;
        public readonly DispatcherTimer timer = new DispatcherTimer();
        private string force = "";
        private string SiteSelected = "all";
        private ListBoxItem itm = new ListBoxItem();
        private readonly SolidColorBrush onColorBg = new SolidColorBrush(Color.FromArgb(255, 5, 157, 228));
        private readonly SolidColorBrush OffColorBg = new SolidColorBrush(Color.FromArgb(255, 140, 140, 140));
        private readonly ParseFile parseFile = new ParseFile();
        private readonly Window1 SettingsWnd = new Window1();
        public List<string> mlist;
        

        private WebClient web = new WebClient();
        private BatotoRSS batoto = new BatotoRSS();
        
        public MainWindow()
        {
            InitializeComponent();
            timer.Interval = TimeSpan.FromSeconds(5d);
            timer.Tick += timer_Tick;
            Settings.Default.Debug = "Debug shit goes in here!\n";
            //Write settings to disk
            Settings.Default.Save();

            NotificationWindow g = new NotificationWindow("Starting in 5...", 0, 5);
            g.Show();

        }
        public void debugtext(string text)
        {//Read
            Settings.Default.Debug += text + "\n";
            //Write settings to disk
            Settings.Default.Save();
        }
        private void timer_Tick(object sender, EventArgs e)
        {
            if (parseFile.GetValueSettings("mangareader") == "1")
            {
                MangareaderBtn.Visibility = Visibility.Visible;
                if(MangareaderLine.Visibility != Visibility.Visible)
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
                if(MangastreamLine.Visibility != Visibility.Visible)
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
            ThreadStart childref = CheckNow;
            var childThread = new Thread(childref);
            childThread.IsBackground = true;
            childThread.Start();
            
            //parseFile.AddToNotReadList("mangastream", "the seven deadly sins", 44);
            timer.Start();

            MangastreamLine.Visibility = Visibility.Collapsed;
            MangafoxLine.Visibility = Visibility.Collapsed;
            MangareaderLine.Visibility = Visibility.Collapsed;
            BatotoLine.Visibility = Visibility.Collapsed;
            DebugLine.Visibility = Visibility.Collapsed;
            AllLine.Visibility = Visibility.Collapsed;

            Fill_list();

        }

        private void CheckNow()
        {
            var timer = 5;
            var _count = 0;
            var ms = new MangastreamRSS();
            var mf = new MangafoxRSS();
            var mr = new MangareaderHTML();
            var ba = new BatotoRSS();
            
            while (true)
            {
                if (force == "force")
                {
                    timer = 0;
                    force = "";
                }
                if (timer >= 1)
                {
                    Dispatcher.BeginInvoke(new Action(delegate ()
                    {
                        StatusLb.Content = "Status: Checking in " + timer +
                                           " seconds.";
                    }));
                    Thread.Sleep(1000);
                    timer--;

                    Dispatcher.BeginInvoke(new Action(delegate()
                    {
                        CounterLbl.Content = _count.ToString();
                    }));
                }
                else
                {
                    if (parseFile.GetValueSettings("mangastream") == "1")
                    {

                        Dispatcher.BeginInvoke(new Action(delegate()
                        {
                            StatusLb.Content = "Status: Checking Mangastream";
                        }));
                        try
                        {
                            ms.checked_if_new();
                        }
                        catch (Exception mst)
                        {
                            debugtext(String.Format("[{0}][Mangastream] Error {1}", DateTime.Now, mst.Message));
                        }
                    }
                    if (parseFile.GetValueSettings("mangafox") == "1")
                    {

                        Dispatcher.BeginInvoke(new Action(delegate
                        {
                            StatusLb.Content = "Status: Checking Mangafox";
                        }));
                        foreach (var manga in parseFile.Mangafox_manga())
                        {
                            //debugtext(string.Format("[{0}][Mangafox] Checking {1}.", DateTime.Now, manga.Replace("[]", " ")));
                            mf.check_all(manga);
                        }
                        
                    }
                    if (parseFile.GetValueSettings("mangareader") == "1")
                    {

                        Dispatcher.BeginInvoke(new Action(delegate()
                        {
                            StatusLb.Content = "Status: Checking Mangareader";
                        }));
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
                                debugtext(string.Format("[{1}][Mangareader] Error {0} {2}.", manga.Replace("[]", " "), DateTime.Now, mrd.Message));
                            }
                        }
                    }
                    if (parseFile.GetValueSettings("batoto") == "1")
                    {

                        Dispatcher.BeginInvoke(new Action(delegate ()
                        {
                            StatusLb.Content = "Status: Checking Batoto";
                        }));
                        try
                        {
                            mlist = ba.Get_feed_titles();
                            ba.Check();
                            Thread.Sleep(1000);
                        }
                        catch (Exception bat)
                        {
                            // lol
                            debugtext(String.Format("[{0}][batoto] Error {1}.", DateTime.Now, bat.Message));
                        }
                    }
                    //timer2.Start();
                    var waittime = Int32.Parse(parseFile.GetValueSettings("refresh time"));

                    Dispatcher.BeginInvoke(new Action(delegate()
                    {
                        StatusLb.Content = @"Status: Checking in " + waittime + " seconds.";
                    }));
                    _count++;
                    timer = waittime;
                }
            }
        }

        private void FillMangastream()
        {
            ListBoxItem itmheader = new ListBoxItem();
            itmheader.Foreground = OffColorBg;
            itmheader.Tag = "MangastreamHeader";
            itmheader.Content = "-Mangastream";
            itmheader.IsEnabled = false;
            listBox.Items.Add(itmheader);
            listBox.Items.Add(new Separator());
            foreach (var manga in parseFile.Mangastream_manga())
            {
                ListBoxItem itm = new ListBoxItem();
                itm.Content = manga.Replace("[]", " : ");
                itm.Foreground = OffColorBg;
                itm.Tag = "mangastream";
                listBox.Items.Add(itm);
            }
        }

        private void FillMangareader()
        {
            ListBoxItem itmheader = new ListBoxItem();
            itmheader.Foreground = OffColorBg;
            itmheader.Tag = "MangareaderHeader";
            itmheader.Content = "-Mangareader";
            itmheader.IsEnabled = false;
            listBox.Items.Add(itmheader);
            listBox.Items.Add(new Separator());
            foreach (var manga in parseFile.Mangareader_manga())
            {
                var name = manga.Split(new[] { "[]" }, StringSplitOptions.None);
                if (parseFile.GetValueStatus("mangareader", name[0]) == "true")
                {
                    //NotificationWindow nfw = new NotificationWindow(manga.Replace("[]", " : "));
                    //nfw.Show();
                    ListBoxItem itm = new ListBoxItem();
                    itm.Content = manga.Replace("[]", " : ");
                    itm.Foreground = onColorBg;
                    itm.Tag = "mangareader";
                    listBox.Items.Add(itm);
                }
                else
                {
                    ListBoxItem itm = new ListBoxItem();
                    itm.Content = manga.Replace("[]", " : ");
                    itm.Foreground = OffColorBg;
                    itm.Tag = "mangareader";
                    listBox.Items.Add(itm);
                }
            }
        }

        private void Fillbatoto()
        {
            ListBoxItem itmheader = new ListBoxItem();
            itmheader.Foreground = OffColorBg;
            itmheader.Tag = "BatotoHeader";
            itmheader.Content = "-Batoto";
            itmheader.IsEnabled = false;
            listBox.Items.Add(itmheader);
            listBox.Items.Add(new Separator());
            foreach (var manga in parseFile.Batoto_manga())
            {
                var name = manga.Split(new[] { "[]" }, StringSplitOptions.None);
                if (parseFile.GetValueStatus("batoto", name[0]) == "true")// && parseFile.GetNotReadList("batoto", name[0]).Count > 0)
                {
                    //NotificationWindow nfw = new NotificationWindow(manga.Replace("[]", " : "));
                    //nfw.Show();
                    ListBoxItem itm = new ListBoxItem();
                    itm.Content = manga.Replace("[]", " : ");
                    itm.Foreground = onColorBg;
                    itm.Tag = "batoto";
                    listBox.Items.Add(itm);
                }
                else
                {
                    ListBoxItem itm = new ListBoxItem();
                    itm.Content = manga.Replace("[]", " : ");
                    itm.Foreground = OffColorBg;
                    itm.Tag = "batoto";
                    listBox.Items.Add(itm);
                }
            }
        }

        private void FillMangafox()
        {
            ListBoxItem itmheader = new ListBoxItem();
            itmheader.Foreground = OffColorBg;
            itmheader.Tag = "MangafoxHeader";
            itmheader.Content = "-Mangafox";
            itmheader.IsEnabled = false;
            listBox.Items.Add(itmheader);
            listBox.Items.Add(new Separator());
            foreach (var manga in parseFile.Mangafox_manga())
            {
                var name = manga.Split(new[] { "[]" }, StringSplitOptions.None);
                if (parseFile.GetValueStatus("mangafox", name[0]) == "true")
                {
                    //NotificationWindow nfw = new NotificationWindow(manga.Replace("[]", " : "));
                    //nfw.Show();
                    ListBoxItem itm = new ListBoxItem();
                    itm.Content = manga.Replace("[]", " : ");
                    itm.Foreground = onColorBg;
                    itm.Tag = "mangafox";
                    listBox.Items.Add(itm);
                }
                else
                {
                    ListBoxItem itm = new ListBoxItem();
                    itm.Content = manga.Replace("[]", " : ");
                    itm.Foreground = OffColorBg;
                    itm.Tag = "mangafox";
                    listBox.Items.Add(itm);
                }
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

            ListBoxItem i = new ListBoxItem();
            i.Content = "";
            i.Tag = "blank";
            i.IsEnabled = false;
            listBox.Items.Add(i);
            FillMangafox();

            ListBoxItem i1 = new ListBoxItem();
            i1.Content = "";
            i1.Tag = "blank";
            i1.IsEnabled = false;
            listBox.Items.Add(i1);
            FillMangareader();

            ListBoxItem i2 = new ListBoxItem();
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
            MangastreamLine.Visibility = MangastreamBtn.Visibility == Visibility.Collapsed ? Visibility.Collapsed : Visibility.Hidden;
            MangafoxLine.Visibility = MangafoxBtn.Visibility == Visibility.Collapsed ? Visibility.Collapsed : Visibility.Hidden;
            MangareaderLine.Visibility = MangareaderBtn.Visibility == Visibility.Collapsed ? Visibility.Collapsed : Visibility.Hidden;
            BatotoLine.Visibility = BatotoBtn.Visibility == Visibility.Collapsed ? Visibility.Collapsed : Visibility.Hidden;
            DebugLine.Visibility = DebugBtn.Visibility == Visibility.Collapsed ? Visibility.Collapsed : Visibility.Hidden;

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
            MangafoxLine.Visibility = MangafoxBtn.Visibility == Visibility.Collapsed ? Visibility.Collapsed : Visibility.Hidden;
            MangareaderLine.Visibility = MangareaderBtn.Visibility == Visibility.Collapsed ? Visibility.Collapsed: Visibility.Hidden;
            BatotoLine.Visibility = BatotoBtn.Visibility == Visibility.Collapsed ? Visibility.Collapsed : Visibility.Hidden;
            DebugLine.Visibility = DebugBtn.Visibility == Visibility.Collapsed ? Visibility.Collapsed : Visibility.Hidden;
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

            MangastreamLine.Visibility = MangastreamBtn.Visibility == Visibility.Collapsed ? Visibility.Collapsed : Visibility.Hidden;
            MangafoxLine.Visibility = Visibility.Visible;
            MangareaderLine.Visibility = MangareaderBtn.Visibility == Visibility.Collapsed ? Visibility.Collapsed : Visibility.Hidden;
            BatotoLine.Visibility = BatotoBtn.Visibility == Visibility.Collapsed ? Visibility.Collapsed : Visibility.Hidden;
            DebugLine.Visibility = DebugBtn.Visibility == Visibility.Collapsed ? Visibility.Collapsed : Visibility.Hidden;
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

            MangastreamLine.Visibility = MangastreamBtn.Visibility == Visibility.Collapsed ? Visibility.Collapsed : Visibility.Hidden;
            MangafoxLine.Visibility = MangafoxBtn.Visibility == Visibility.Collapsed ? Visibility.Collapsed : Visibility.Hidden;
            BatotoLine.Visibility = BatotoBtn.Visibility == Visibility.Collapsed ? Visibility.Collapsed : Visibility.Hidden;
            DebugLine.Visibility = DebugBtn.Visibility == Visibility.Collapsed ? Visibility.Collapsed : Visibility.Hidden;
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
                
                if(parseFile.GetNotReadList("mangafox", name).Contains(float.Parse(chapter)))
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
                        var link = mangarss.Split(new[] { "[]" }, StringSplitOptions.None)[1];
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
                                      .Replace("! ", "_").Replace("-", "_").Replace(":", "_") + "/c" + name[1] +
                                  "/1.html");
                    if (itemselected.Foreground == onColorBg)
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
                        if (itemselected.Foreground == onColorBg)
                        {
                            parseFile.SetValueStatus("mangareader", name[0], "false");
                        }
                    }
                    else
                    {
                        Process.Start("http://www.mangareader.net/" +
                                      name[0].Replace(" ", "-").Replace("!", "").Replace(":", "") + "/" + name[1]);
                        if (itemselected.Foreground == onColorBg)
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
                    float intchapter = float.Parse(chapter);
                    intchapter++;
                    foreach (var mangarss in mlist)
                    {
                        if (mangarss.ToLower().Contains(name.ToLower()) &&
                            mangarss.ToLower().Contains(chapter.ToLower()))
                        {
                            var link = mangarss.Split(new[] {"[]"}, StringSplitOptions.None)[1];
                            Process.Start(link);
                            if (itemselected.Foreground == onColorBg &&
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
                debugtext(String.Format("[{0}][Error] NullReferenceException", DateTime.Now));
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
                else if(SiteSelected == "batoto")
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
                AddPanel.Visibility = Visibility.Visible;
            }
            else
            {
                AddPanel.Visibility = Visibility.Collapsed;
            }
        }

        private MenuItem CreateItem(string site, string name, string ch, string click, string header)
        {
            MenuItem item = new MenuItem();
            item.Foreground = onColorBg;
            item.Margin = new Thickness(+15, 0, -40, 0);
            if (header == "yes")
            {
                item.Header = name;
                item.FontWeight = FontWeights.Bold;
                item.IsEnabled = false;
            }
            if(click =="yes")
                item.Header = name + " : " + ch;
                item.Click += (sender, e) => OpenSite(site, name, ch);
            return item;
        }

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
                if (itemselected.Tag.Equals("mangareader") || itemselected.Tag.Equals("mangafox") || itemselected.Tag.Equals("batoto"))
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

                            Match match = Regex.Match(mangarss, @".+ ch\.(\d*\.?\d*).+", RegexOptions.IgnoreCase);
                            var matchvalue = match.Groups[1].Value;
                            if (chfloat > float.Parse(matchvalue) && mangarss.ToLower().Contains(name.ToLower()) && ic <= lastchc)
                            {
                                listBox.ContextMenu.Items.Add(CreateItem(itemselected.Tag.ToString(), name,
                                    matchvalue, "yes", "no"));
                                ic++;
                            }
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
                Console.WriteLine("[Error] "+ex);
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
            MangastreamLine.Visibility = MangastreamBtn.Visibility == Visibility.Collapsed ? Visibility.Collapsed : Visibility.Hidden;
            MangafoxLine.Visibility = MangafoxBtn.Visibility == Visibility.Collapsed ? Visibility.Collapsed : Visibility.Hidden;
            MangareaderLine.Visibility = MangareaderBtn.Visibility == Visibility.Collapsed ? Visibility.Collapsed : Visibility.Hidden;
            DebugLine.Visibility = DebugBtn.Visibility == Visibility.Collapsed ? Visibility.Collapsed : Visibility.Hidden;
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
            BatotoLine.Visibility = BatotoBtn.Visibility == Visibility.Collapsed ? Visibility.Collapsed : Visibility.Hidden;
            MangastreamLine.Visibility = MangastreamBtn.Visibility == Visibility.Collapsed ? Visibility.Collapsed : Visibility.Hidden;
            MangafoxLine.Visibility = MangafoxBtn.Visibility == Visibility.Collapsed ? Visibility.Collapsed : Visibility.Hidden;
            MangareaderLine.Visibility = MangareaderBtn.Visibility == Visibility.Collapsed ? Visibility.Collapsed : Visibility.Hidden;
            DebugLine.Visibility = Visibility.Visible;
            AllLine.Visibility = Visibility.Hidden;
        }

        private void DebugTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            DebugTextBox.ScrollToEnd();
        }
        

        private void AddMangaBtn_Click(object sender, RoutedEventArgs e)
        {
            if (AddBtn.Foreground == onColorBg)
            {
                // add the manga
                if (SiteNameLb.Content.ToString().ToLower().Contains("mangareader"))
                {
                    if (MangaNameLb.Content.ToString() != "Failed" || MangaNameLb.Content.ToString() != "None" && ChapterNumLb.Content.ToString() != "None" || ChapterNumLb.Content.ToString() != "Failed")
                    {
                        parseFile.AddManga("mangareader", MangaNameLb.Content.ToString().ToLower(), ChapterNumLb.Content.ToString(), "true");
                        AddBtn.Content = "Success!";
                    }
                }

            }
            if (linkbox.Text.ToLower().Contains("bato.to/myfollows_rss?secret="))
            {
                var RSSList = batoto.Get_feed_titles();
                var JSMangaList = parseFile.GetBatotoMangaNames();

                foreach (var RSSTitle in RSSList)
                {
                    var name = RSSTitle.Split(new[] { " - " }, StringSplitOptions.None)[0];
                    if (JSMangaList.Contains(name) == false)
                    {
                        JSMangaList.Add(name);
                        Match match = Regex.Match(RSSTitle, @".+ ch\.(\d+).+", RegexOptions.IgnoreCase);
                        parseFile.AddManga("batoto", name, match.Groups[1].Value, "false");
                        Console.WriteLine("[Batoto] added {0}", name);
                    }
                }
            }
        }

        private void SearchBtn_Click(object sender, RoutedEventArgs e)
        {

            //search manga here
            if (linkbox.Text.ToLower().Contains("mangareader.net"))
            {
                //mangareader code
                //MessageBox.Show("mangareader.net link");
                SiteNameLb.Foreground = onColorBg;
                SiteNameLb.Content = "mangareader.net";
                try
                {
                    var html = web.DownloadString(linkbox.Text);
                    Match name = Regex.Match(html, "<h2 class=\"aname\">(.+)</h2>", RegexOptions.IgnoreCase);
                    if (name.Success)
                    {
                        Match chapter = Regex.Match(html, ("<a href=\"/.+/.+\">(.+) (\\d+)</a>"), RegexOptions.IgnoreCase);
                        MangaNameLb.Foreground = onColorBg;
                        MangaNameLb.Content = name.Groups[1].Value;
                        if (chapter.Success && chapter.Groups[1].Value == name.Groups[1].Value)
                        {
                            ChapterNumLb.Foreground = onColorBg;
                            ChapterNumLb.Content = chapter.Groups[2].Value;
                            AddBtn_Copy.Foreground = onColorBg;

                        }
                        else
                        {
                            ChapterNumLb.Foreground = OffColorBg;
                            ChapterNumLb.Content = "Failed";
                            AddBtn_Copy.Foreground = OffColorBg;
                        }
                    }
                    else
                    {
                        MangaNameLb.Foreground = OffColorBg;
                        MangaNameLb.Content = "Failed";
                        ChapterNumLb.Foreground = OffColorBg;
                        ChapterNumLb.Content = "Failed";
                        AddBtn_Copy.Foreground = OffColorBg;
                    }
                }
                catch (Exception)
                {
                    // do stuff here
                }
            }
            else if (linkbox.Text.ToLower().Contains("mangafox.me"))
            {
                //mangafox code
                //MessageBox.Show("mangafox.me link");
                SiteNameLb.Foreground = onColorBg;
                SiteNameLb.Content = "mangafox.me";
            }
            else if (linkbox.Text.ToLower().Contains("readms.com") || linkbox.Text.ToLower().Contains("mangastream.com"))
            {
                //mangareader code
                //MessageBox.Show("mangastream.com || readms.com link");
                SiteNameLb.Foreground = onColorBg;
                SiteNameLb.Content = "readms.com, mangastream.com";
            }
            else if (linkbox.Text.ToLower() == "")
            {

                SiteNameLb.Foreground = OffColorBg;
                SiteNameLb.Content = "None";
                MangaNameLb.Foreground = OffColorBg;
                MangaNameLb.Content = "None";
                ChapterNumLb.Foreground = OffColorBg;
                ChapterNumLb.Content = "None";
            }
            else
            {
                //bb
            }
        }

    }
}
