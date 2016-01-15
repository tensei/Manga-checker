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
    class mangareader
    {
        public MangaInfoViewModel GetInfo(string url)
        {
            MangaInfoViewModel InfoViewModel = new MangaInfoViewModel();
            try
            {
                WebClient web = new WebClient();
                var html = web.DownloadString(url);
                Match name = Regex.Match(html, "<h2 class=\"aname\">(.+)</h2>", RegexOptions.IgnoreCase);
                if (name.Success)
                {
                    Match chapter = Regex.Match(html, ("<a href=\"/.+/.+\">(.+) (\\d+)</a>"), RegexOptions.IgnoreCase);
                    InfoViewModel.Name = name.Groups[1].Value.Trim();
                    if (chapter.Success && chapter.Groups[1].Value == name.Groups[1].Value)
                    {
                        InfoViewModel.Chapter = chapter.Groups[2].Value.Trim();
                        InfoViewModel.Site = "mangareader.net";
                        InfoViewModel.Error = "null";
                        return InfoViewModel;

                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                InfoViewModel.Error = e.Message;
                InfoViewModel.Name = "ERROR";
                InfoViewModel.Chapter = "ERROR";
                return InfoViewModel;
                // do stuff here
            }
            return InfoViewModel;
        }
    }
}
