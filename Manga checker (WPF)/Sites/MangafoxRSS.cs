using System;
using System.Diagnostics;
using Manga_checker.Database;
using Manga_checker.Properties;

namespace Manga_checker.Sites
{
    internal class MangafoxRSS
    {
        //public MainWindow Main;
        public string Get_feed_titles(string url, string chapter, string name)
        {
            var ch_plus = int.Parse(chapter);
            ch_plus++;
            //HttpWebRequest hwr = (HttpWebRequest)WebRequest.Create(url);
            //// attach persistent cookies
            ////hwr.CookieContainer = PersistentCookies.GetCookieContainerForUrl(url);
            //hwr.Accept = "text/xml, */*";
            //hwr.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-us");
            //hwr.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.0; .NET CLR 3.5.30729;)";
            //hwr.KeepAlive = true;
            //hwr.AutomaticDecompression = DecompressionMethods.Deflate |
            //                             DecompressionMethods.GZip;

            //var resp = (HttpWebResponse) hwr.GetResponse();
            //Stream s = resParseFile.GetResponseStream();
            //string cs = String.IsNullOrEmpty(resParseFile.CharacterSet) ? "UTF-8" : resParseFile.CharacterSet;
            //Encoding e = Encoding.GetEncoding(cs);

            //StreamReader sr = new StreamReader(s, e);
            //var allXml = sr.ReadToEnd();

            //// remove any script blocks - they confuse XmlReader
            ////allXml = Regex.Replace(allXml,
            ////                        "(.*)<script type='text/javascript'>.+?</script>(.*)",
            ////                        "$1$2",
            ////                        RegexOptions.Singleline);
            //sr.Dispose();
            //s.Dispose();
            //resParseFile.Dispose();
            //allXml = allXml.Replace("pubDate", "date");
            //XmlReader xmlr = XmlReader.Create(new StringReader(allXml));
            //var feed = SyndicationFeed.Load(xmlr);
            var rssReader = new RSSReader();
            var feed = rssReader.Read(url);
            foreach (var mangs in feed.Items)
            {
                //ParseFile.setManga("mangafox", name, chapter);
                if (mangs.Title.Text.ToLower().Contains(ch_plus.ToString().ToLower()))
                {
                    if (ParseFile.GetValueSettings("open links") == "1")
                    {
                        Process.Start(mangs.Links[0].Uri.AbsoluteUri);
                        ParseFile.SetManga("mangafox", name, ch_plus.ToString());
                        Sqlite.UpdateManga("mangafox", name, ch_plus.ToString(), mangs.Links[0].Uri.AbsoluteUri);
                    }

                    chapter = ch_plus.ToString();
                    debugtext(string.Format("[{2}][Mangafox] {0} {1} Found new Chapter", name, ch_plus, DateTime.Now));
                }
            }
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
                return Get_feed_titles("http://mangafox.me/rss/" + name.Replace(" ", "").Replace("__", "_") + ".xml",
                    ch[1], ch[0]);
            }
            catch (Exception e)
            {
                //Main.DebugTextBox.Text += string.Format("[Mangafox] Error {0} {1}", manga, e);
                debugtext(string.Format("[{1}][Mangafox] Error {0} {2} {3}.", manga.Replace("[]", " "), DateTime.Now,
                    e.Message, name));
                return manga;
            }
        }

        public void debugtext(string text)
        {
//Read
            Settings.Default.Debug += text + "\n";
            //Write settings to disk
            Settings.Default.Save();
        }
    }
}