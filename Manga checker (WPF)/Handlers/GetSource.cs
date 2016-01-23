using System.IO;
using System.Net;
using System.Text;

namespace Manga_checker.Handlers
{
    internal class GetSource
    {
        public string get(string url)
        {
            var hwr = (HttpWebRequest) WebRequest.Create(url);
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
            var feed = sr.ReadToEnd();
            sr.Dispose();
            if (s != null) s.Dispose();
            resp.Dispose();
            return feed;
        }
    }
}