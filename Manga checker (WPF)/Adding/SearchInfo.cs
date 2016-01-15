using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Manga_checker.Adding.Sites;
using Manga_checker.Classes;

namespace Manga_checker.Adding
{
    class SearchInfo
    {
        public MangaInfoViewModel InfoViewModel = new MangaInfoViewModel();
        public MangaInfoViewModel search(string url)
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
                    InfoViewModel = m.GetInfo(url);
                }
                else if (url.ToLower().Contains("mangafox.me"))
                {
                    mangafox m = new mangafox();
                    InfoViewModel = m.GeInfo(url);
                    //InfoViewModel.Site = "mangafox.me";
                    //InfoViewModel.Error = "ERROR Site not Supported yet.";
                    //InfoViewModel.Name = "ERROR";
                    //InfoViewModel.Chapter = "ERROR";
                    ////mangafox code
                    //MessageBox.Show(InfoViewModel.Error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (url.ToLower().Contains("readms.com") || url.ToLower().Contains("mangastream.com"))
                {
                    mangastream m = new mangastream();
                    InfoViewModel = m.GeInfo(url);
                    //InfoViewModel.Site = "readms.com";
                    //InfoViewModel.Error = "ERROR Site not Supported yet.";
                    //InfoViewModel.Name = "ERROR";
                    //InfoViewModel.Chapter = "ERROR";
                    ////mangareader code
                    //MessageBox.Show(InfoViewModel.Error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (url.ToLower().Equals(string.Empty))
                {
                    InfoViewModel.Error = "URL empty";
                }
                else
                {

                    MessageBox.Show("Link not recognized :/", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    InfoViewModel.Error = "Link not recognized :/";
                    InfoViewModel.Name = "ERROR";
                    InfoViewModel.Chapter = "ERROR";
                    InfoViewModel.Site = "ERROR";
                }
                return InfoViewModel;
            }
            catch (Exception error)
            {
                InfoViewModel.Error = error.Message;
                return InfoViewModel;
            }
            
        }
    }
}
