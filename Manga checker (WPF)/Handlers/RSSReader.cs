using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Manga_checker
{
    class RSSReader
    {
        public SyndicationFeed Read(string url)
        {
            HttpWebRequest hwr = (HttpWebRequest)WebRequest.Create(url);
            // attach persistent cookies
            //hwr.CookieContainer = PersistentCookies.GetCookieContainerForUrl(url);
            hwr.Accept = "text/xml, */*";
            hwr.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-us");
            hwr.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.0; .NET CLR 3.5.30729;)";
            hwr.KeepAlive = true;
            hwr.AutomaticDecompression = DecompressionMethods.Deflate |
                                         DecompressionMethods.GZip;

            var resp = (HttpWebResponse)hwr.GetResponse();
            Stream s = resp.GetResponseStream();
            string cs = String.IsNullOrEmpty(resp.CharacterSet) ? "UTF-8" : resp.CharacterSet;
            Encoding e = Encoding.GetEncoding(cs);

            StreamReader sr = new StreamReader(s, e);
            var allXml = sr.ReadToEnd();

            // remove any script blocks - they confuse XmlReader
            //allXml = Regex.Replace(allXml,
            //                        "(.*)<script type='text/javascript'>.+?</script>(.*)",
            //                        "$1$2",
            //                        RegexOptions.Singleline);
            sr.Dispose();
            s.Dispose();
            resp.Dispose();
            //allXml = allXml.Replace("pubDate", "date");
            XmlReader xmlr = XmlReader.Create(new StringReader(allXml));
            var feed = SyndicationFeed.Load(xmlr);
            return feed;
        }
    }
}
