using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MangaChecker.Common;
using MangaChecker.Database;
using MangaChecker.Models;

namespace MangaChecker.Sites.RSS {
	public static class GameOfScanlation {
		public static async void Check(MangaModel manga, string openLinks) {
			try {
				var Name = manga.Name;
				var Chapter = float.Parse(manga.Chapter, CultureInfo.InvariantCulture);
				var url = manga.RssLink;
				var rssitems = await RssReader.Read(url);
				if (rssitems == null) return;
				foreach (var rssitem in rssitems.Items.Reverse()) {
					var title = rssitem.Title.Text;
					var chapter = float.Parse(Regex.Match(title, @"chapter ?([0-9\.]+)", RegexOptions.IgnoreCase).Groups[1].Value, CultureInfo.InvariantCulture);
					if (Chapter >= chapter) continue;
					if (!openLinks.Equals("1")) continue;
					Process.Start(rssitem.Links[0].Uri.AbsoluteUri);
					var date = DateTime.Now;
					if (!rssitem.PublishDate.DayOfYear.Equals(1)) date = rssitem.PublishDate.DateTime;
					manga.Chapter = chapter.ToString(CultureInfo.InvariantCulture);
					manga.Link = rssitem.Links[0].Uri.AbsoluteUri;
					manga.Date = date;
					Sqlite.UpdateManga(manga);
					DebugText.Write($"[GameOfScanlation] Found new Chapter {Name} {rssitem.Title.Text}.");
				}
			} catch (Exception ex) {
				DebugText.Write($"[GameOfScanlation] Error {ex.Message}.");
			}
		}
	}
}