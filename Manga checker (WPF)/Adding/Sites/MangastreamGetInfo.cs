using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using MangaChecker.Models;

namespace MangaChecker.Adding.Sites {
    internal static class MangastreamGetInfo {
        public static MangaInfoModel Get(string url) {
            var manga = new MangaInfoModel();

            try {
                var web = new WebClient();
                var source = web.DownloadString(url);
                //  </i> Smokin' Parade <strong>001</strong><em>
                var name = Regex.Match(source, "<h1>(.+)</h1>");
                if (name.Success) {
                    manga.Name = name.Groups[1].Value.Trim();
                    var chapter = Regex.Match(source, "(http://mangastream.com/r/.+)\">(.{1,}?) - .[^<>]+");
                    if (chapter.Success) {
                        manga.Chapter = chapter.Groups[2].Value.Trim();
                        manga.Site = "mangastream";
                        manga.Link = chapter.Groups[1].Value.Trim();
                        return manga;
                    }
                }
                MessageBox.Show("No Match found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                manga.Error = "No Match found";
                return manga;
            } catch (Exception e) {
                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                manga.Error = e.Message;
                return manga;
            }
        }
    }
}