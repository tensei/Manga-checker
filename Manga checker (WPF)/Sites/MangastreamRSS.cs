using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using Manga_checker.Properties;

namespace Manga_checker
{
    class MangastreamRSS
    {
        //public MainWindow Main;
        public List<string> Get_feed_titles()
        {
            const string url = "http://mangastream.com/rss";
            string xml;
            var mngstr = new List<string>();
            using (var webClient = new WebClient())
            {
                xml = webClient.DownloadString(url);
            }
            xml = Regex.Replace(xml, @"&.+;", "A");
            xml = xml.Replace("pubDate", "pubDateBroke");//.Replace("&rsquo;", "'").Replace("&ldquo;", " ").Replace("&rdquo;", ".");//
            //xml = xml.Replace("&lt;", " ").Replace("&gt;", " ");
            /*;*/
            //var bytes = Encoding.ASCII.GetBytes(xml);
            //xml = WebUtility.HtmlDecode(xml);
            var reader = new XmlTextReader(new StringReader(xml));
            var feed = SyndicationFeed.Load(reader);
            if (feed == null) return mngstr;
            foreach (var mangs in feed.Items)
            {
                mngstr.Add(mangs.Title.Text + "[]" + mangs.Id);
            }
            return mngstr;
        }

        public void checked_if_new()
        {
            ParseFile m = new ParseFile();
            var ms = m.GetManga("mangastream");
            var mslist = Get_feed_titles();
            var mangaName = "";
            var link = "";
            Match ch_;
            var x = "";
            foreach (var manga in ms)
            {
                var trimManga = manga.Split(new[] { "[]" }, StringSplitOptions.None);
                //m.setManga("mangastream", trimManga[0], trimManga[1]);
                foreach (var m_ in mslist)
                {
                    var mch = m_.ToLower().Split(new[] { "[]" }, StringSplitOptions.None);
                    mangaName = mch[0];
                    link = mch[1];
                    if (mangaName.ToLower().Contains(trimManga[0].ToLower()) && mangaName.ToLower().StartsWith(trimManga[0].ToLower()))
                    {
                        //Console.WriteLine(mangaName);
                        ch_ = Regex.Match(mangaName,@".+ (\d+)", RegexOptions.IgnoreCase);
                        //Console.WriteLine(manga +" "+trimManga[0]);
                        x = ch_.Groups[1].Value;
                        //Console.WriteLine(x);
                        if(x.Contains(" "))
                            x = x.Trim();
                        if (x.Equals(string.Empty))
                        {
                            x = "1";}
                        float xfloat = float.Parse(x);
                        //Console.WriteLine(xfloat.ToString());
                        //var mch = m_.ToLower().Split(new[] { "[]" }, StringSplitOptions.None);
                        float ch_plus = float.Parse(trimManga[1]);
                        ch_plus++;
                        //Console.WriteLine(ch_plus);
                        if (xfloat == ch_plus)
                        {
                            System.Diagnostics.Process.Start(link);
                            m.SetManga("mangastream", trimManga[0], xfloat.ToString());
                            //Main.DebugTextBox.Text += string.Format("[Mangastream] {0} {1} Found new Chapter",
                            //    trimManga[0], ch_plus);
                            debugtext(string.Format("[{2}][Mangastream] {0} {1} Found new Chapter", trimManga[0], ch_plus, DateTime.Now));
                        }

                        //var new_mgstr = trimManga[0] + " " + ch_plus;
                        //if (mangaName.Contains(new_mgstr))
                        //{
                        //    System.Diagnostics.Process.Start(mch[1]);
                        //    m.setManga("mangastream", trimManga[0], ch_plus.ToString(), "true");
                        //    Console.WriteLine("[Mangastream] {0} {1} Found new Chapter", trimManga[0], ch_plus);
                        //}
                    }
                }
            }
        }
        public void debugtext(string text)
        {//Read
            Settings.Default.Debug += text + "\n";
            //Write settings to disk
            Settings.Default.Save();
        }
    }
}
