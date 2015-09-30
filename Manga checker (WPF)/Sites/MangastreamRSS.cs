﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using Manga_checker__WPF_.Properties;

namespace Manga_checker__WPF_
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
                xml = Encoding.UTF8.GetString(webClient.DownloadData(url));

            }
            xml = xml.Replace("pubDate", "datee");
            var bytes = Encoding.ASCII.GetBytes(xml);
            var reader = XmlReader.Create(new MemoryStream(bytes));
            var feed = SyndicationFeed.Load(reader);
            foreach (var mangs in feed.Items)
            {
                mngstr.Add(mangs.Title.Text + "[]" + mangs.Id);
            }
            return mngstr;
        }

        public void checked_if_new()
        {
            ParseFile m = new ParseFile();
            var ms = m.Mangastream_manga();
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
                            return;
                        float xfloat = float.Parse(x);
                        //Console.WriteLine(xfloat.ToString());
                        //var mch = m_.ToLower().Split(new[] { "[]" }, StringSplitOptions.None);
                        float ch_plus = float.Parse(trimManga[1]);
                        ch_plus++;
                        //Console.WriteLine(ch_plus);
                        if (xfloat == ch_plus)
                        {
                            System.Diagnostics.Process.Start(mch[1]);
                            m.setManga("mangastream", trimManga[0], ch_plus.ToString(), "true");
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
