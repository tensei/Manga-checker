using System;
using System.Diagnostics;
using System.Globalization;
using System.ServiceModel.Syndication;
using System.Text.RegularExpressions;
using MangaChecker.Common;
using MangaChecker.Database;
using MangaChecker.Models;

namespace MangaChecker.Sites.RSS {
	internal static class Yomanga {
		public static void Check(MangaModel manga, SyndicationFeed rss, string openLinks) {
			try {
				var feed = rss;
				foreach (var item in feed.Items) {
					var title = item.Title.Text;
					if(!title.ToLower().Contains("chapter")) continue;
					if (title.ToLower().Contains(manga.Name.ToLower())) {
						var _chapter = float.Parse(Regex.Match(title, @"chapter ([0-9\.]+)", RegexOptions.IgnoreCase).Groups[1].Value, CultureInfo.InvariantCulture);
						var _currentchapter = float.Parse(manga.Chapter, CultureInfo.InvariantCulture);
						if (_currentchapter >= _chapter) {
							continue;
						}
						if (openLinks.Equals("1")) {
							Process.Start(item.Links[0].Uri.AbsoluteUri);
							manga.Chapter = _chapter.ToString(CultureInfo.InvariantCulture);
							manga.Link = item.Links[0].Uri.AbsoluteUri;
							manga.Date = DateTime.Now;
							Sqlite.UpdateManga(manga);
							DebugText.Write($"[YoManga] Found new Chapter {manga.Name} {_chapter}.");
							break;
						}
					}
				}

				// DebugText.Write(item.Title.Text);
			} catch (Exception ex) {
				DebugText.Write($"[YoManga] Error {manga.Name} {manga.Chapter} {ex.Message}.");
				//throw;
			}
		}
	}
}