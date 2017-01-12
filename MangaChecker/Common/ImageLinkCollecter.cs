using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MangaChecker.Common {
	internal static class ImageLinkCollecter {
		public static async Task<Tuple<List<string>, string>> YomangaCollectLinks(string url) {
			if (!url.EndsWith("page/1"))
				url = url + "page/1";
			var html = await GetSource.GetAsync(url) ?? await CloudflareGetString.GetAsync(url);
			var match = Regex.Match(html,
				"<div class=\"text\">([0-9]+) ⤵</div>",
				RegexOptions.IgnoreCase);
			var retlist = new List<string>();

			var lastChapterNumber = int.Parse(match.Groups[1].Value);
			var slitlink = url.Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries);
			for (var i = 1; i <= lastChapterNumber; i++) {
				slitlink[slitlink.Length - 1] = i.ToString();
				var newlink = string.Join("/", slitlink);
				newlink = newlink.Replace(":/", "://");

				var htmlimg = await GetSource.GetAsync(newlink) ??
							await CloudflareGetString.GetAsync(newlink);
				//meh regex takes a while
				var imgLink = Regex.Match(htmlimg,
					@"([https|http]+://[a-z]+\.?[a-z]+?\.[a-z]+.+/content/comics/.+[\.jpg|\.png|\.gif])");
				retlist.Add(imgLink.Groups[1].Value);
			}
			return new Tuple<List<string>, string>(retlist, match.Groups[1].Value);
		}

		public static async Task<Tuple<List<string>, string>> MangastreamCollectLinks(string url) {
			//http://mangastream.com/r/my_hero_academia/097/3504/1
			var html = await GetSource.GetAsync(url) ?? await CloudflareGetString.GetAsync(url);
			var match = Regex.Match(html,
				@"Last Page .([0-9]+).</a>",
				RegexOptions.IgnoreCase);
			var retlist = new List<string>();

			var lastChapterNumber = int.Parse(match.Groups[1].Value);
			for (var i = 1; i <= lastChapterNumber; i++) {
				var slitlink = url.Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries);
				slitlink[slitlink.Length - 1] = i.ToString();
				var newlink = string.Join("/", slitlink);

				var htmlimg = await GetSource.GetAsync(newlink.Replace("http:/man", "http://man")) ??
							await CloudflareGetString.GetAsync(newlink.Replace("http:/man", "http://man"));

				var imgLink = Regex.Match(htmlimg, "<img id=\"manga.+\".+src=\"(http://img..+.com/cdn/manga/.+)\"/>");
				retlist.Add(imgLink.Groups[1].Value);
			}
			return new Tuple<List<string>, string>(retlist, match.Groups[1].Value);
		}
	}
}