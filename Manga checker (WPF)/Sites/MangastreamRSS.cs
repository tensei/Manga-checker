using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.ServiceModel.Syndication;
using System.Text.RegularExpressions;
using System.Xml;
using Manga_checker.Database;
using Manga_checker.Handlers;
using Manga_checker.Properties;
using Manga_checker.ViewModels;

namespace Manga_checker {
    internal class MangastreamRSS {
        //public MainWindow Main;
        public static List<string> Get_feed_titles() {
            const string url = "http://mangastream.com/rss";
            string xml;
            var mngstr = new List<string>();

            try {
                using (var webClient = new WebClient()) {
                    xml = webClient.DownloadString(url);
                }
                xml = Regex.Replace(xml, @"&.+;", "A");
                xml = xml.Replace("pubDate", "pubDateBroke");
                //.Replace("&rsquo;", "'").Replace("&ldquo;", " ").Replace("&rdquo;", ".");//
                //xml = xml.Replace("&lt;", " ").Replace("&gt;", " ");
                /*;*/
                //var bytes = Encoding.ASCII.GetBytes(xml);
                //xml = WebUtility.HtmlDecode(xml);
                var reader = new XmlTextReader(new StringReader(xml));
                var feed = SyndicationFeed.Load(reader);
                if (feed == null) return mngstr;
                foreach (var mangs in feed.Items) {
                    mngstr.Add(mangs.Title.Text + "[]" + mangs.Id);
                }
            }
            catch (Exception e) {
                DebugText.Write(e.Message);
            }
            
            return mngstr;
        }

        public static void Check(MangaInfoViewModel manga, List<string> mslist) {
            var mangaName = "";
            var link = "";
            Match ch_;
            var x = "";
                //m.setManga("mangastream", manga.Name, trimManga[1]);
            foreach (var m_ in mslist) {
                var mch = m_.ToLower().Split(new[] {"[]"}, StringSplitOptions.None);
                mangaName = mch[0];
                link = mch[1];
                if (mangaName.ToLower().Contains(manga.Name.ToLower()) &&
                    mangaName.ToLower().StartsWith(manga.Name.ToLower())) {
                    //Console.WriteLine(mangaName);
                    ch_ = Regex.Match(mangaName, @".+ (\d+)", RegexOptions.IgnoreCase);
                    //Console.WriteLine(manga +" "+manga.Name);
                    x = ch_.Groups[1].Value;
                    //Console.WriteLine(x);
                    if (x.Contains(" "))
                        x = x.Trim();
                    if (x.Equals(string.Empty)) {
                        x = "1";
                    }
                    var xfloat = float.Parse(x);
                    //Console.WriteLine(xfloat.ToString());
                    //var mch = m_.ToLower().Split(new[] { "[]" }, StringSplitOptions.None);
                    var ch_plus = float.Parse(manga.Chapter);
                    ch_plus++;
                    //Console.WriteLine(ch_plus);
                    if (xfloat == ch_plus) {
                        Process.Start(link);
                        ParseFile.SetManga("mangastream", manga.Name, xfloat.ToString());
                        Sqlite.UpdateManga("mangastream", manga.Name, xfloat.ToString(), link);
                        //Main.DebugTextBox.Text += string.Format("[Mangastream] {0} {1} Found new Chapter",
                        //    manga.Name, ch_plus);
                        DebugText.Write(string.Format("[{2}][Mangastream] {0} {1} Found new Chapter", manga.Name,
                            ch_plus, DateTime.Now));
                    }

                    //var new_mgstr = manga.Name + " " + ch_plus;
                    //if (mangaName.Contains(new_mgstr))
                    //{
                    //    System.Diagnostics.Process.Start(mch[1]);
                    //    m.setManga("mangastream", manga.Name, ch_plus.ToString(), "true");
                    //    Console.WriteLine("[Mangastream] {0} {1} Found new Chapter", manga.Name, ch_plus);
                    //}
                }
            }
        }
    }
}