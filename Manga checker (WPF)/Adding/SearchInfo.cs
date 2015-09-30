using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Manga_checker__WPF_.Adding.Sites;
using Manga_checker__WPF_.Classes;

namespace Manga_checker__WPF_.Adding
{
    class SearchInfo
    {
        public MangaInfo info = new MangaInfo();
        public MangaInfo search(string url)
        {
            var web = new WebClient();
            try
            {
                //search manga here
                if (url.ToLower().Contains("mangareader.net"))
                {
                    //mangareader code
                    //MessageBox.Show("mangareader.net link");
                    mangareader m = new mangareader();
                    info = m.GetInfo(url);
                }
                else if (url.ToLower().Contains("mangafox.me"))
                {
                    mangafox m = new mangafox();
                    info = m.GeInfo(url);
                    //info.Site = "mangafox.me";
                    //info.Error = "ERROR Site not Supported yet.";
                    //info.Name = "ERROR";
                    //info.Chapter = "ERROR";
                    ////mangafox code
                    //MessageBox.Show(info.Error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (url.ToLower().Contains("readms.com") || url.ToLower().Contains("mangastream.com"))
                {
                    mangastream m = new mangastream();
                    info = m.GeInfo(url);
                    //info.Site = "readms.com";
                    //info.Error = "ERROR Site not Supported yet.";
                    //info.Name = "ERROR";
                    //info.Chapter = "ERROR";
                    ////mangareader code
                    //MessageBox.Show(info.Error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (url.ToLower().Equals(string.Empty))
                {
                    info.Error = "URL empty";
                }
                else
                {

                    MessageBox.Show("Link not recognized :/", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    info.Error = "Link not recognized :/";
                    info.Name = "ERROR";
                    info.Chapter = "ERROR";
                    info.Site = "ERROR";
                }
                return info;
            }
            catch (Exception error)
            {
                info.Error = error.Message;
                return info;
            }
            
        }
    }
}
