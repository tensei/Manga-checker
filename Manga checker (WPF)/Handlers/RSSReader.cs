using System.IO;
using System.Net;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;

namespace Manga_checker
{
    internal class RSSReader
    {
        public SyndicationFeed Read(string url)
        {
            var hwr = (HttpWebRequest) WebRequest.Create(url);
            // attach persistent cookies
            //hwr.CookieContainer = PersistentCookies.GetCookieContainerForUrl(url);
            hwr.Accept = "text/xml, */*";
            hwr.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-us");
            hwr.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.0; .NET CLR 3.5.30729;)";
            hwr.KeepAlive = true;
            hwr.AutomaticDecompression = DecompressionMethods.Deflate |
                                         DecompressionMethods.GZip;

            var resp = (HttpWebResponse) hwr.GetResponse();
            var s = resp.GetResponseStream();
            var cs = string.IsNullOrEmpty(resp.CharacterSet) ? "UTF-8" : resp.CharacterSet;
            var e = Encoding.GetEncoding(cs);

            var sr = new StreamReader(s, e);
            var allXml =
                sr.ReadToEnd().Replace("pubDate", "fuck").Replace("lastBuildDate", "fuck2").Replace("<img src=\"", "");

            // remove any script blocks - they confuse XmlReader
            //allXml = Regex.Replace(allXml,
            //                        "(.*)<script type='text/javascript'>.+?</script>(.*)",
            //                        "$1$2",
            //                        RegexOptions.Singleline);
            sr.Dispose();
            //if (s != null) s.Dispose();
            resp.Dispose();
            //allXml = allXml.Replace("pubDate", "date");
            var xmlr = XmlReader.Create(new StringReader(allXml));
            var feed = SyndicationFeed.Load(xmlr);
            return feed;
        }
    }
}