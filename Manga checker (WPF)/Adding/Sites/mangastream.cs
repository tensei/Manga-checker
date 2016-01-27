using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Manga_checker.Classes;

namespace Manga_checker.Adding.Sites {
    internal class mangastream {
        public MangaInfoViewModel GeInfo(string url) {
            var InfoViewModel = new MangaInfoViewModel();

            try {
                var web = new WebClient();
                var source = web.DownloadString(url);
                //  </i> Smokin' Parade <strong>001</strong><em>
                var name = Regex.Match(source, "<h1>(.+)</h1>", RegexOptions.IgnoreCase);
                if (name.Success) {
                    InfoViewModel.Name = name.Groups[1].Value.Trim();
                    var chapter = Regex.Match(source, ">([0-9]+) - ", RegexOptions.IgnoreCase);
                    if (chapter.Success) {
                        InfoViewModel.Chapter = chapter.Groups[1].Value.Trim();
                        InfoViewModel.Error = "null";
                        InfoViewModel.Site = "mangastream.com";
                        return InfoViewModel;
                    }
                }
                MessageBox.Show("No Match found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                InfoViewModel.Error = "No Match found";
                InfoViewModel.Name = "ERROR";
                InfoViewModel.Chapter = "ERROR";
                return InfoViewModel;
            }
            catch (Exception e) {
                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                InfoViewModel.Error = e.Message;
                InfoViewModel.Name = "ERROR";
                InfoViewModel.Chapter = "ERROR";
                return InfoViewModel;
            }
        }
    }
}