using System;
using System.IO;
using System.Net;
using System.ServiceModel.Syndication;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Manga_checker.Common {
    public class RSSReader {
        public static SyndicationFeed Read(string url) {
            try {
                var hwr = (HttpWebRequest) WebRequest.Create(url);
                // attach persistent cookies
                //hwr.CookieContainer = PersistentCookies.GetCookieContainerForUrl(url);
                hwr.Accept = "text/xml, */*";
                hwr.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-us");
                hwr.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.0; .NET CLR 3.5.30729;)";
                hwr.KeepAlive = true;
                hwr.AutomaticDecompression = DecompressionMethods.Deflate |
                                             DecompressionMethods.GZip;
                string allXml;
                try {
                    using (var resp = (HttpWebResponse) hwr.GetResponse()) {
                        using (var s = resp.GetResponseStream()) {
                            var cs = string.IsNullOrEmpty(resp.CharacterSet) ? "UTF-8" : resp.CharacterSet;
                            var e = Encoding.GetEncoding(cs);
                            using (var sr = new StreamReader(s, e)) {
                                allXml = sr.ReadToEnd();
                            }
                        }
                    }
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
                DebugText.Write($"[YoManga] {e.Message}");
                return null;
            }
        }
    }
}