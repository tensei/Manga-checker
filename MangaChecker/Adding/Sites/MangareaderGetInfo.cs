using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using MangaChecker.Models;

namespace MangaChecker.Adding.Sites {
	internal static class MangareaderGetInfo {
		public static MangaInfoModel Get(string url) {
			var manga = new MangaInfoModel();
			try {
				var web = new WebClient();
				var html = web.DownloadString(url);
				var name = Regex.Match(html, "<h2 class=\"aname\">(.+)</h2>", RegexOptions.IgnoreCase);
				if (name.Success) {
					var chapter = Regex.Match(html, "<a href=\"(.+)\">(.+) (\\d+)</a>", RegexOptions.IgnoreCase);
					manga.Name = name.Groups[2].Value.Trim();
					if (chapter.Success && (chapter.Groups[2].Value == name.Groups[1].Value)) {
						manga.Name = name.Groups[1].Value;
						manga.Chapter = chapter.Groups[3].Value.Trim();
						manga.Site = "mangareader";
						manga.Link = "http://" + manga.Site + chapter.Groups[1].Value.Trim();
						return manga;
					}
				}
			} catch (Exception e) {
				MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				manga.Error = e.Message;
				return manga;
				// do stuff here
			}
			return manga;
		}
	}
}