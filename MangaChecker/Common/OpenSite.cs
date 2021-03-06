﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace MangaChecker.Common {
	internal class OpenSite {
		public static void Open(string site, string name, string chapter, IEnumerable<string> mlist) {
			switch (site.ToLower()) {
				case "mangafox": {
					name = Regex.Replace(name, "[^0-9a-zA-Z]+", "_").Trim('_');
					Process.Start("http://mangafox.me/manga/" + name + "/c" + chapter +
								"/1.html");
					break;
				}
				case "mangareader": {
					//open mangareader site for current chapter
					if (chapter.Contains(" ")) {
						name = Regex.Replace(name, "[^0-9a-zA-Z]+", "-").Trim('-');
						var chaptersplit = chapter.Split(new[] {" "}, StringSplitOptions.None);
						Process.Start("http://www.mangareader.net/" + name + "/" + chaptersplit[0]);
					} else {
						Process.Start("http://www.mangareader.net/" + name + "/" + chapter);
					}
					break;
				}
				case "batoto": {
					foreach (var mangarss in mlist) {
						if (!mangarss.ToLower().Contains(name.ToLower()) ||
							!mangarss.ToLower().Contains(chapter.ToLower())) continue;
						var link = mangarss.Split(new[] {"[]"}, StringSplitOptions.None)[1];
						Process.Start(link);
					}
					break;
				}
			}
		}
	}
}