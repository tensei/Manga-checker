using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.ServiceModel.Syndication;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using Manga_checker__WPF_.Properties;

namespace Manga_checker__WPF_
{
    internal class MangafoxRSS
    {
        //public MainWindow Main;
        public string Get_feed_titles(string url, string chapter, string name)
        {

            var ch_plus = int.Parse(chapter);
            ch_plus++;
            string xml;
            var p = new ParseFile();
            using (var webClient = new WebClient())
            {
                xml = webClient.DownloadString(url);
                //xml = Encoding.UTF8.GetString(webClient.DownloadData(url));
                xml = xml.Replace("pubDate", "datee");
                xml = xml.Replace(@"\u00f1", "").Trim();
                xml = xml.Replace(Convert.ToChar((byte)0x1F), ' ');
            }
            //var bytes = Encoding.ASCII.GetBytes(xml);
            TextReader tr = new StringReader(xml);
            var reader = XmlReader.Create(tr);
            var feed = SyndicationFeed.Load(reader);
            reader.Close();
            reader.Dispose();
            foreach (var mangs in feed.Items)
            {
                //p.setManga("mangafox", name, chapter);
                if (mangs.Title.Text.Contains(ch_plus.ToString()))
                {
                    if (p.GetValueStatus("mangafox", name) == "true" && p.GetNotReadList("mangafox", name).Contains(float.Parse(chapter)) == false)
                    {
                        p.AddToNotReadList("mangafox", name, float.Parse(chapter));
                    }
                    if (p.GetValueSettings("open links") == "1")
                    {
                        Process.Start(mangs.Links[0].Uri.AbsoluteUri);
                        p.setManga("mangafox", name, ch_plus.ToString(), "false");
                    }
                    else
                    {
                        p.setManga("mangafox", name, ch_plus.ToString(), "true");
                    }
                        
                    chapter = ch_plus.ToString();
                    debugtext(string.Format("[{2}][Mangafox] {0} {1} Found new Chapter", name, ch_plus, DateTime.Now));
                }
            }
            reader.Dispose();
            return name + " " + chapter;
        }

        public string check_all(string manga)
        {
            var ch = manga.Split(new[] {"[]"}, StringSplitOptions.None);
            var name = ch[0].Trim()
                .Replace(":", "_")
                .Replace("(", "")
                .Replace(")", "")
                .Replace(", ", "_")
                .Replace(" - ", " ")
                .Replace("-", "_")
                .Replace(" ", "_")
                .Replace("'", "_")
                .Replace("! -", "_")
                .Replace("!", "")
                .Replace(". ", "_")
                .Replace(".", "")
                .Replace("! ", "_").Replace("-", "_").Replace(":", "_");
            try
            {
                return Get_feed_titles("http://mangafox.me/rss/" + name.Replace(" ", "") + ".xml", ch[1], ch[0]);
            }
            catch (Exception e)
            {
                //Main.DebugTextBox.Text += string.Format("[Mangafox] Error {0} {1}", manga, e);
                debugtext(string.Format("[{1}][Mangafox] Error {0} {2}.", manga.Replace("[]", " "), DateTime.Now, e.Message));
                return manga;
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