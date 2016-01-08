using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Manga_checker.Classes;

namespace Manga_checker.Adding.Sites
{
    class mangafox
    {
        public MangaInfo GeInfo(string url)
        {
            MangaInfo info = new MangaInfo();
            WebClient web = new WebClient();
            try
            {
                //title="RSS" href="/rss/one_piece.xml"/><link
                var source = web.DownloadString(url);
                var rsslink = Regex.Match(source, "title=\"RSS\" href=\"(.+)\"/>", RegexOptions.IgnoreCase);
                RSSReader feed = new RSSReader();
                var rss = feed.Read("http://mangafox.me" + rsslink.Groups[1].Value);

                foreach (var item in rss.Items)
                {
                    if (!item.Title.Text.ToLower().Contains("vol"))
                    {
                        var title = Regex.Match(item.Title.Text, "(.+) Ch (.+)");
                        info.Name = title.Groups[1].Value;
                        info.Chapter = title.Groups[2].Value;
                    }
                    else
                    {
                        var title = Regex.Match(item.Title.Text, "(.+) Vol.+ Ch (.+)");
                        info.Name = title.Groups[1].Value.Trim();
                        info.Chapter = title.Groups[2].Value.Trim();
                    }
                    info.Site = "mangafox.me";
                    info.Error = "null";
                    break;
                }

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                info.Error = e.Message;
                info.Name = "ERROR";
                info.Chapter = "ERROR";
                return info;
            }
            return info;
        }
    }
}
