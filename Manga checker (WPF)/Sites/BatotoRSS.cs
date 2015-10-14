using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.ServiceModel.Syndication;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using Manga_checker.Properties;

namespace Manga_checker
{
    class BatotoRSS
    {
        
        ParseFile parse = new ParseFile();
        public List<string> Get_feed_titles()
        {
            string url = parse.GetValueSettings("batoto_rss");
            var mngstr = new List<string>();
            if (url.Equals(""))
            {
                debugtext($"[{DateTime.Now}][ERROR] batoto_rss is empty.");
                return mngstr;
            }
            XmlReader reader = XmlReader.Create(url);
            SyndicationFeed feed = SyndicationFeed.Load(reader);
            reader.Close();
            foreach (var mangs in feed.Items)
            {
                //Console.WriteLine(mangs.Title.Text);
                mngstr.Add(mangs.Title.Text + "[]" + mangs.Links[0].Uri.AbsoluteUri);
            }
            return mngstr;
        }

        public void Check()
        {
            var Batotolist = parse.GetManga("batoto");
            var feedTitles = Get_feed_titles();
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
                        Match match = Regex.Match(rsstitle, @".+ ch\.(\d*\.?\d*).+", RegexOptions.IgnoreCase);
                        if (match.Success)
                        {
                            var open = parse.GetValueSettings("open links");
                            var mathChapter = float.Parse(match.Groups[1].Value);
                            float test = Convert.ToSingle(Math.Ceiling(chapter));
                            if (test.ToString().Contains(".") == false)
                            {
                                test++;
                            }
                            if (chapter < mathChapter && mathChapter <= test && mathChapter != chapter && newCh.Equals(0))
                            {
                                newCh = mathChapter;
                                debugtext(string.Format("[{0}][Batoto] {1} Found new Chapter", DateTime.Now, rsstitle));
                                if (open == "1")
                                {
                                    Process.Start(link);
                                    parse.SetManga("batoto", name, mathChapter.ToString(), "false");
                                    if (parse.GetNotReadList("batoto", name).Contains(mathChapter))
                                    {
                                        parse.RemoveFromNotRead("batoto", name, mathChapter);
                                    }
                                }
                                if (open == "0")
                                {
                                    if(parse.GetNotReadList("batoto", name).Contains(chapter) != true)
                                        parse.AddToNotReadList("batoto", name, mathChapter);
                                        parse.SetValueStatus("batoto", name, "true");
                                }

                            }
                            else if(newCh != 0 && mathChapter > newCh)
                            {
                                if (parse.GetNotReadList("batoto", name).Contains(mathChapter) != true)
                                {
                                    parse.AddToNotReadList("batoto", name, mathChapter);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void debugtext(string text)
        {//Read
            Settings.Default.Debug += text+"\n";
        }
    }
}
