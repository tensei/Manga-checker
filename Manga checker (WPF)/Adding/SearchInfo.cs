using System;
using System.Net;
using System.Windows.Forms;
using Manga_checker.Adding.Sites;
using Manga_checker.ViewModels;

namespace Manga_checker.Adding {
    internal class SearchInfo {
        public MangaModel InfoViewModel = new MangaModel();

        public  MangaModel search(string url) {
            var web = new WebClient();
            try {
                ////search manga here
                //if (url.ToLower().Contains("mangareader.net")) {
                //    //mangareader code
                //    //MessageBox.Show("mangareader.net link");
                //    var m = new mangareader();
                //    InfoViewModel = m.GetInfo(url);
                //}
                //else if (url.ToLower().Contains("mangafox.me")) {
                //    var m = new mangafox();
                //    InfoViewModel = m.GeInfo(url);
                //    //InfoViewModel.Site = "mangafox.me";
                //    //InfoViewModel.Error = "ERROR Site not Supported yet.";
                //    //InfoViewModel.Name = "ERROR";
                //    //InfoViewModel.Chapter = "ERROR";
                //    ////mangafox code
                //    //MessageBox.Show(InfoViewModel.Error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //}
                //else if (url.ToLower().Contains("readms.com") || url.ToLower().Contains("mangastream.com")) {
                //    var m = new mangastream();
                //    InfoViewModel = m.GeInfo(url);
                //    //InfoViewModel.Site = "readms.com";
                //    //InfoViewModel.Error = "ERROR Site not Supported yet.";
                //    //InfoViewModel.Name = "ERROR";
                //    //InfoViewModel.Chapter = "ERROR";
                //    ////mangareader code
                //    //MessageBox.Show(InfoViewModel.Error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //}
                //else if (url.ToLower().Equals(string.Empty)) {
                //    InfoViewModel.Error = "URL empty";
                //}
                //else if(url.ToLower().Contains("webtoons")) {
                //    InfoViewModel = webtoons.GetInfo(url);
                //}
                //else {
                //    MessageBox.Show("Link not recognized :/", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //    InfoViewModel.Error = "Link not recognized :/";
                //    InfoViewModel.Name = "ERROR";
                //    InfoViewModel.Chapter = "ERROR";
                //    InfoViewModel.Site = "ERROR";
                //}
                return InfoViewModel;
            }
            catch (Exception error) {
                InfoViewModel.Error = error.Message;
                return InfoViewModel;
            }
        }
    }
}