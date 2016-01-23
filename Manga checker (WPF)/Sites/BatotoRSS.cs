using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceModel.Syndication;
using System.Text.RegularExpressions;
using System.Xml;
using Manga_checker.Database;
using Manga_checker.Properties;

namespace Manga_checker
{
    internal class BatotoRSS
    {
        private ParseFile ParseFile = new ParseFile();

        public List<string> Get_feed_titles()
        {
            var url = ParseFile.GetValueSettings("batoto_rss");
            var mngstr = new List<string>();
            if (url.Equals(""))
            {
                debugtext($"[{DateTime.Now}][ERROR] batoto_rss is empty.");
                return mngstr;
            }
            var reader = XmlReader.Create(url);
            var feed = SyndicationFeed.Load(reader);
            reader.Close();
            if (feed != null)
                foreach (var mangs in feed.Items)
                {
                    //Console.WriteLine(mangs.Title.Text);
                    mngstr.Add(mangs.Title.Text + "[]" + mangs.Links[0].Uri.AbsoluteUri);
                }
            return mngstr;
        }

        public void Check(List<string> feed)
        {
            var Batotolist = ParseFile.GetManga("batoto");
            var feedTitles = feed;
            feedTitles.Reverse();
            foreach (var manga in Batotolist)
            {
                var split = manga.Split(new[] {"[]"}, StringSplitOptions.None);
                var name = split[0];
                var chapter = float.Parse(split[1]);
                //chapter++;
                float newCh = 0;
                foreach (var rssmanga in feedTitles)
                {
                    //var rssmanganame = rssmanga.Split(new[] {" - "}, StringSplitOptions.None)[0];

                    var rsssplit = rssmanga.Split(new[] {"[]"}, StringSplitOptions.None);
                    var rsstitle = rsssplit[0];
                    var link = rsssplit[1];
                    //debugText(rsstitle);
                    if (rsstitle.ToLower().Contains(name.ToLower()))
                    {
                        var match = Regex.Match(rsstitle, @".+ ch\.(\d*\.?\d*).+", RegexOptions.IgnoreCase);
                        if (match.Success)
                        {
                            var open = ParseFile.GetValueSettings("open links");
                            var mathChapter = float.Parse(match.Groups[1].Value);
                            var test = Convert.ToSingle(Math.Ceiling(chapter));
                            if (test.ToString().Contains(".") == false)
                            {
                                test++;
                            }
                            if (chapter < mathChapter && mathChapter <= test && mathChapter != chapter &&
                                newCh.Equals(0))
                            {
                                newCh = mathChapter;
                                debugtext(string.Format("[{0}][Batoto] {1} Found new Chapter", DateTime.Now, rsstitle));
                                if (open == "1")
                                {
                                    Process.Start(link);
                                    ParseFile.SetManga("batoto", name, mathChapter.ToString());
                                    Sqlite.UpdateManga("batoto", name, mathChapter.ToString(), link);
                                }
                                if (open == "0")
                                {
                                    //
                                }
                            }
                            else if (newCh != 0 && mathChapter > newCh)
                            {
                                //stuff
                            }
                        }
                    }
                }
            }
        }

        public void debugtext(string text)
        {
//Read
            Settings.Default.Debug += text + "\n";
        }
    }
}