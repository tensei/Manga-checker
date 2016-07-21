using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Manga_checker.Common {
    internal static class ImageLinkCollecter {
        public static List<string> YomangaCollectLinks(string url) {
            url = url + "page/1";
            var html = GetSource.Get(url) ?? CloudflareGetString.Get(url);
            var match = Regex.Match(html,
                "<div class=\"text\">([0-9]+) ⤵</div>",
                RegexOptions.IgnoreCase);
            var retlist = new List<string>();

            var lastChapterNumber = int.Parse(match.Groups[1].Value);
            for (var i = 1; i <= lastChapterNumber; i++) {
                var slitlink = url.Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries);
                slitlink[slitlink.Length - 1] = i.ToString();
                var newlink = string.Join("/", slitlink);
                var htmlimg = GetSource.Get(newlink.Replace("http:/yo", "http://yo")) ??
                              CloudflareGetString.Get(newlink.Replace("http:/yo", "http://yo"));

                var imgLink = Regex.Match(htmlimg,
                    "<img class=\"open\" src=\"(http://yomanga.co/reader/content/comics.+)\".+");
                retlist.Add(imgLink.Groups[1].Value);
            }
            return retlist;
        }

        public static List<string> MangastreamCollectLinks(string url) {
            //http://mangastream.com/r/my_hero_academia/097/3504/1
            var html = GetSource.Get(url) ?? CloudflareGetString.Get(url);
            var match = Regex.Match(html,
                @"Last Page .([0-9]+).</a>",
                RegexOptions.IgnoreCase);
            var retlist = new List<string>();

            var lastChapterNumber = int.Parse(match.Groups[1].Value);
            for (var i = 1; i <= lastChapterNumber; i++) {
                var slitlink = url.Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries);
                slitlink[slitlink.Length - 1] = i.ToString();
                var newlink = string.Join("/", slitlink);

                var htmlimg = GetSource.Get(newlink.Replace("http:/man", "http://man")) ??
                              CloudflareGetString.Get(newlink.Replace("http:/man", "http://man"));

                var imgLink = Regex.Match(htmlimg, "<img id=\"manga.+\".+src=\"(http://img..+.com/cdn/manga/.+)\"/>");
                retlist.Add(imgLink.Groups[1].Value);
            }
            return retlist;
        }
    }
}