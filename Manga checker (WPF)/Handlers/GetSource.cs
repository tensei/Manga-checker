using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Manga_checker.Handlers
{
    class GetSource
    {
        public string get(string url)
        {
            HttpWebRequest hwr = (HttpWebRequest)WebRequest.Create(url);
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
            var feed = sr.ReadToEnd();
            sr.Dispose();
            if (s != null) s.Dispose();
            resp.Dispose();
            return feed;
        }
    }
}
