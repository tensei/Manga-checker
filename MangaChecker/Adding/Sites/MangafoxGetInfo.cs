using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using MangaChecker.Common;
using MangaChecker.Models;

namespace MangaChecker.Adding.Sites {
	internal static class MangafoxGetInfo {
		public static async Task<MangaInfoModel> Get(string url) {
			var manga = new MangaInfoModel();
			var web = new WebClient();
			try {
				//title="RSS" href="/rss/one_piece.xml"/><link
				var source = web.DownloadString(url);
				var rsslink = Regex.Match(source, "title=\"RSS\" href=\"(.+)\"/>", RegexOptions.IgnoreCase);
				var rss = await RssReader.Read("http://mangafox.me" + rsslink.Groups[1].Value);

				if (rss.Equals(null)) {
					manga.Error = "null";
					return manga;
				}

				foreach (var item in rss.Items) {
					if (!item.Title.Text.ToLower().Contains("vol")) {
						var title = Regex.Match(item.Title.Text, "(.+) Ch (.+)");
						manga.Name = title.Groups[1].Value;
						manga.Chapter = title.Groups[2].Value;
						manga.Link = item.Links[0].Uri.AbsoluteUri;
					} else {
						var title = Regex.Match(item.Title.Text, "(.+) Vol.+ Ch (.+)");
						manga.Name = title.Groups[1].Value.Trim();
						manga.Chapter = title.Groups[2].Value.Trim();
						manga.Link = item.Links[0].Uri.AbsoluteUri;
					}
					manga.Site = "mangafox";
					break;
				}
			} catch (Exception e) {
				MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				manga.Error = e.Message;
				return manga;
			}
			return manga;
		}
	}
}