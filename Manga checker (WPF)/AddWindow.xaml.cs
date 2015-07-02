using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;

namespace Manga_checker__WPF_
{
    /// <summary>
    /// Interaktionslogik für AddWindow.xaml
    /// </summary>
    public partial class AddWindow : Window
    {
        private WebClient web = new WebClient();
        private readonly SolidColorBrush onColorBg = new SolidColorBrush(Color.FromArgb(255, 5, 157, 228));
        private readonly SolidColorBrush OffColorBg = new SolidColorBrush(Color.FromArgb(255, 124, 124, 124));
        private ParseFile parse = new ParseFile();
        private BatotoRSS batoto = new BatotoRSS();
        public AddWindow()
        {
            InitializeComponent();
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            linkbox.Text = "";
            AddBtn.Foreground = OffColorBg;
            AddBtn.Content = "Add";
            MangaNameLb.Content = "None";
            ChapterNumLb.Content = "None";
            SiteNameLb.Content = "None";
            Hide();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            if (AddBtn.Foreground == onColorBg)
            {
                // add the manga
                if (SiteNameLb.Content.ToString().ToLower().Contains("mangareader"))
                {
                    if (MangaNameLb.Content.ToString() != "Failed" || MangaNameLb.Content.ToString() != "None" && ChapterNumLb.Content.ToString() != "None" || ChapterNumLb.Content.ToString() != "Failed")
                    {
                        parse.AddManga("mangareader", MangaNameLb.Content.ToString().ToLower(), ChapterNumLb.Content.ToString(), "true");
                        AddBtn.Content = "Success!";
                    } 
                }
                
            }
            if (linkbox.Text.ToLower().Contains("bato.to/myfollows_rss?secret="))
            {
                var RSSList = batoto.Get_feed_titles();
                var JSMangaList = parse.GetBatotoMangaNames();

                foreach (var RSSTitle in RSSList)
                {
                    var name = RSSTitle.Split(new[] { " - " }, StringSplitOptions.None)[0];
                    if (JSMangaList.Contains(name) == false)
                    {
                        JSMangaList.Add(name);
                        Match match = Regex.Match(RSSTitle, @".+ ch\.(\d+).+", RegexOptions.IgnoreCase);
                        parse.AddManga("batoto", name, match.Groups[1].Value, "false");
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
                            AddBtn.Foreground = onColorBg;

                        }
                        else
                        {
                            ChapterNumLb.Foreground = OffColorBg;
                            ChapterNumLb.Content = "Failed";
                            AddBtn.Foreground = OffColorBg;
                        }
                    }
                    else
                    {
                        MangaNameLb.Foreground = OffColorBg;
                        MangaNameLb.Content = "Failed";
                        ChapterNumLb.Foreground = OffColorBg;
                        ChapterNumLb.Content = "Failed";
                        AddBtn.Foreground = OffColorBg;
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
