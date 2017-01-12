using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MangaChecker.Common;
using MangaChecker.Database;
using MangaChecker.Models;

namespace MangaChecker.Sites.HTML {
	internal class MangareaderHTML {
		public static async void Check(MangaModel manga, string openLinks) {
			var name = Regex.Replace(manga.Name, "[^0-9a-zA-Z]+", "-").Trim('-').ToLower();
			//DebugText.Write(name);
			var ch = manga.Chapter.Trim(' ');
			MatchCollection m1;
			var FullName = "";
			var mangaa = "";
			var ch_plus = 0;
			if (ch.Contains(" ")) {
				var chsp = manga.Chapter.Split(new[] {" "}, StringSplitOptions.None);
				ch_plus = int.Parse(chsp[0]);
				ch_plus++;
				FullName = manga.Name + " " + ch_plus;

				var url_2 = "http://www.mangareader.net/" + chsp[1] + "/" + name;
				var htmltxt2 = await GetSource.GetAsync(url_2 + ".html") ?? await GetSource.GetAsync(url_2);
				m1 = Regex.Matches(htmltxt2, @"<a href=.+>(.+)</a>.+</li>", RegexOptions.IgnoreCase);
				foreach (Match mangamatch in m1) {
					mangaa = mangamatch.Groups[1].Value;
					if (mangaa.ToLower().Contains(FullName)) {
						var link = "http://www.mangareader.net/" + name + "/" + ch_plus;
						if (openLinks == "1") {
							Process.Start(link);
							manga.Chapter = ch_plus + " " + chsp[1];
							manga.Link = link;
							manga.Date = DateTime.Now;
							Sqlite.UpdateManga(manga);
						}
						DebugText.Write($"[Mangareader] {manga.Name} {ch_plus} Found new Chapter");
						return;
					}
					FullName = manga.Name + " " + chsp[0];
				}
			} else {
				var chsp = ch;
				ch_plus = int.Parse(chsp);
				ch_plus++;
				FullName = manga.Name + " " + ch_plus;
				var url_1 = "http://www.mangareader.net/" + name;
				var htmltext1 = await GetSource.GetAsync(url_1) ?? await GetSource.GetAsync(url_1 + ".html");
				m1 = Regex.Matches(htmltext1, @"<a href=.+>(.+)</a>.+</li>", RegexOptions.IgnoreCase);
				foreach (Match mangamatch in m1) {
					mangaa = mangamatch.Groups[1].Value;
					if (mangaa.ToLower().Contains(FullName)) {
						var link = "http://www.mangareader.net/" + name + "/" + ch_plus;
						if (openLinks == "1") {
							Process.Start(link);
							manga.Chapter = ch_plus.ToString();
							manga.Link = link;
							manga.Date = DateTime.Now;
							Sqlite.UpdateManga(manga);
						}
						DebugText.Write($"[Mangareader] {manga.Name} {ch_plus} Found new Chapter");
						return;
					}
					FullName = manga.Name + " " + chsp;
				}
			}
			return;
		}
	}
}