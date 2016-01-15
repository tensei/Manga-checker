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
        public MangaInfoViewModel GeInfo(string url)
        {
            MangaInfoViewModel InfoViewModel = new MangaInfoViewModel();
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
                        InfoViewModel.Name = title.Groups[1].Value;
                        InfoViewModel.Chapter = title.Groups[2].Value;
                    }
                    else
                    {
                        var title = Regex.Match(item.Title.Text, "(.+) Vol.+ Ch (.+)");
                        InfoViewModel.Name = title.Groups[1].Value.Trim();
                        InfoViewModel.Chapter = title.Groups[2].Value.Trim();
                    }
                    InfoViewModel.Site = "mangafox.me";
                    InfoViewModel.Error = "null";
                    break;
                }

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                InfoViewModel.Error = e.Message;
                InfoViewModel.Name = "ERROR";
                InfoViewModel.Chapter = "ERROR";
                return InfoViewModel;
            }
            return InfoViewModel;
        }
    }
}
