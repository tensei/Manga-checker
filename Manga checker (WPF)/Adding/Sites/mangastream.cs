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
    class mangastream
    {
        public MangaInfo GeInfo(string url)
        {
            MangaInfo info = new MangaInfo();

            try
            {
                WebClient web = new WebClient();
                var source = web.DownloadString(url);
                //  </i> Smokin' Parade <strong>001</strong><em>
                var name = Regex.Match(source, "<h1>(.+)</h1>", RegexOptions.IgnoreCase);
                if (name.Success)
                {
                    info.Name = name.Groups[1].Value.Trim();
                    var chapter = Regex.Match(source, ">([0-9]+) - ", RegexOptions.IgnoreCase);
                    if (chapter.Success)
                    {
                        info.Chapter = chapter.Groups[1].Value.Trim();
                        info.Error = "null";
                        info.Site = "mangastream.com";
                        return info;

                    }
                }
                MessageBox.Show("No Match found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                info.Error = "No Match found";
                info.Name = "ERROR";
                info.Chapter = "ERROR";
                return info;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                info.Error = e.Message;
                info.Name = "ERROR";
                info.Chapter = "ERROR";
                return info;
            }
        }
    }
}
