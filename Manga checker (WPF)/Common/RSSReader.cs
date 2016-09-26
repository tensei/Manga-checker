using System;
using System.IO;
using System.Net;
using System.ServiceModel.Syndication;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;

namespace MangaChecker.Common {
    public static class RssReader {
        public static SyndicationFeed Read(string url) {
            try {
                string allXml;
                try {
                    allXml = GetSource.Get(url);
                } catch {
                    var bytes = Encoding.Default.GetBytes(CloudflareGetString.Get(url));
                    allXml = Encoding.UTF8.GetString(bytes);
                }
                SyndicationFeed feed;
                try {
                    var xmlr = XmlReader.Create(new StringReader(allXml));
                    feed = SyndicationFeed.Load(xmlr);
                } catch (Exception) {
                    allXml = allXml.Replace("pubDate", "fuck")
                        .Replace("lastBuildDate", "fuck2");
                    allXml = Regex.Replace(allXml, "<img src=\".+\"  />", "fuck");
                    var xmlr = XmlReader.Create(new StringReader(allXml));
                    feed = SyndicationFeed.Load(xmlr);
                }
                return feed;
            } catch (Exception e) {
                DebugText.Write($"[RSSERROR] {e.Message} {url}");
                return null;
            }
        }
    }
}