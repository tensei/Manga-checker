using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceModel.Syndication;
using System.Text.RegularExpressions;
using System.Xml;
using Manga_checker.Database;
using Manga_checker.Handlers;
using Manga_checker.Properties;
using Manga_checker.ViewModels;

namespace Manga_checker {
    internal class BatotoRSS {

        public static List<string> Get_feed_titles() {
            var url = ParseFile.GetValueSettings("batoto_rss");
            var mngstr = new List<string>();
            if (url.Equals("")) {
                DebugText.Write($"[ERROR] batoto_rss is empty.");
                return mngstr;
            }
            var reader = XmlReader.Create(url);
            var feed = SyndicationFeed.Load(reader);
            reader.Close();
            if (feed != null)
                foreach (var mangs in feed.Items) {
                    //Console.WriteLine(mangs.Title.Text);
                    mngstr.Add(mangs.Title.Text + "[]" + mangs.Links[0].Uri.AbsoluteUri);
                }
            return mngstr;
        }

        public static void Check(List<string> feed, MangaInfoViewModel manga) {
            var feedTitles = feed;
            feedTitles.Reverse();
            var name = manga.Name;
            var chapter = float.Parse(manga.Chapter);
            //chapter++;
            float newCh = 0;
            foreach (var rssmanga in feedTitles) {
                //var rssmanganame = rssmanga.Split(new[] {" - "}, StringSplitOptions.None)[0];

                var rsssplit = rssmanga.Split(new[] {"[]"}, StringSplitOptions.None);
                var rsstitle = rsssplit[0];
                var link = rsssplit[1];
                //DebugText.Write(rsstitle);
                if (rsstitle.ToLower().Contains(name.ToLower())) {
                    var match = Regex.Match(rsstitle, @".+ ch\.(\d*\.?\d*).+", RegexOptions.IgnoreCase);
                    if (match.Success) {
                        var open = ParseFile.GetValueSettings("open links");
                        var mathChapter = float.Parse(match.Groups[1].Value);
                        var test = Convert.ToSingle(Math.Ceiling(chapter));
                        if (test.ToString().Contains(".") == false) {
                            test++;
                        }
                        if (chapter < mathChapter && mathChapter <= test && mathChapter != chapter &&
                            newCh.Equals(0)) {
                            newCh = mathChapter;
                            DebugText.Write(string.Format("[{0}][Batoto] {1} Found new Chapter", DateTime.Now, rsstitle));
                            if (open == "1") {
                                Process.Start(link);
                                ParseFile.SetManga("batoto", name, mathChapter.ToString());
                                Sqlite.UpdateManga("batoto", name, mathChapter.ToString(), link);
                            }
                            if (open == "0") {
                                //
                            }
                        }
                        else if (newCh != 0 && mathChapter > newCh) {
                            //stuff
                        }
                    }
                }
            }
        }
    }
}