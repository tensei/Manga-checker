using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Manga_checker__WPF_.Classes;

namespace Manga_checker__WPF_.Adding.Sites
{
    class mangareader
    {
        public MangaInfo GetInfo(string url)
        {
            MangaInfo info = new MangaInfo();
            try
            {
                WebClient web = new WebClient();
                var html = web.DownloadString(url);
                Match name = Regex.Match(html, "<h2 class=\"aname\">(.+)</h2>", RegexOptions.IgnoreCase);
                if (name.Success)
                {
                    Match chapter = Regex.Match(html, ("<a href=\"/.+/.+\">(.+) (\\d+)</a>"), RegexOptions.IgnoreCase);
                    info.Name = name.Groups[1].Value.Trim();
                    if (chapter.Success && chapter.Groups[1].Value == name.Groups[1].Value)
                    {
                        info.Chapter = chapter.Groups[2].Value.Trim();
                        info.Site = "mangareader.net";
                        info.Error = "null";
                        return info;

                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                info.Error = e.Message;
                info.Name = "ERROR";
                info.Chapter = "ERROR";
                return info;
                // do stuff here
            }
            return info;
        }
    }
}
