using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using MangaChecker.Common;
using MangaChecker.Database;
using MangaChecker.Models;

namespace MangaChecker.Sites.RSS {
	internal class Mangafox {
		//public MainWindow Main;
		public static void Get_feed_titles(string url, MangaModel manga, string openLinks) {
			var ch_plus = int.Parse(manga.Chapter);
			ch_plus++;
			var feed = RssReader.Read(url);
			if (feed == null) return;
			foreach (var mangs in feed.Items)
				if (mangs.Title.Text.ToLower().Contains(ch_plus.ToString().ToLower())) {
					if (openLinks == "1") {
						Process.Start(mangs.Links[0].Uri.AbsoluteUri);
						manga.Chapter = ch_plus.ToString();
						manga.Link = mangs.Links[0].Uri.AbsoluteUri;
						manga.Date = DateTime.Now;
						Sqlite.UpdateManga(manga);
					}

					DebugText.Write($"[Mangafox] {manga.Name} {ch_plus} Found new Chapter");
				}
		}

		public static void Check(MangaModel manga, string openLinks) {
			var name = Regex.Replace(manga.Name, "[^0-9a-zA-Z]+", "_").Trim('_').ToLower();
			//DebugText.Write(Regex.Replace("tes__ygr___rhut_","[^0-9a-zA-Z]+","_").Trim('_')); //test regex output
			try {
				Get_feed_titles("http://mangafox.me/rss/" + name + ".xml",
					manga, openLinks);
			} catch (Exception e) {
				//Main.DebugTextBox.Text += string.Format("[Mangafox] Error {0} {1}", manga, e);
				DebugText.Write($"[Mangafox] Error {manga.Name} {e.Message} {name}.");
			}
		}
	}
}