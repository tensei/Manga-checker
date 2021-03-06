﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Text.RegularExpressions;
using System.Xml;
using MangaChecker.Common;
using MangaChecker.Database;
using MangaChecker.Models;

namespace MangaChecker.Sites.RSS {
	internal class Batoto {
		public static List<List<object>> Get_feed_titles() {
			var settings = Sqlite.GetSettings();
			var url = settings["batoto_rss"];
			var mngstr = new List<List<object>>();
			if (url.Equals("")) {
				DebugText.Write($"[ERROR] batoto_rss is empty.");
				return mngstr;
			}
			XmlReader reader;
			try {
				reader = XmlReader.Create(url);
			} catch (WebException e) {
				DebugText.Write($"[Batoto] {e}");
				return mngstr;
			}
			var feed = SyndicationFeed.Load(reader);
			reader.Close();
			if (feed != null)
				foreach (var mangs in feed.Items) {
					var chapter = Regex.Match(mangs.Title.Text, @".+ ch\.(\d+).+", RegexOptions.IgnoreCase);

					//Console.WriteLine(mangs.Title.Text);
					mngstr.Add(new List<object> {
						mangs.Title.Text,
						chapter.Groups[1].Value,
						mangs.Links[0].Uri.AbsoluteUri,
						mangs.PublishDate.DateTime
					});
				}
			return mngstr;
		}

		public static void Check(IEnumerable<List<object>> feed, MangaModel manga, string openLinks) {
			foreach (var rssmanga in feed.Reverse()) {
				if (!rssmanga[0].ToString().ToLower().Contains(manga.Name.ToLower())) continue;

				var t1 = (DateTime) rssmanga[3];
				var diff = DateTime.Compare(t1.ToUniversalTime(), manga.Date.ToUniversalTime());
				if (diff <= 0) continue;
				var mangaTitle = (string) rssmanga[0];
				var link = (string) rssmanga[2];


				var ch = Regex.Match(mangaTitle, @".+ ch\.([\d\.]+):? r?", RegexOptions.IgnoreCase);
				var chapter = ch.Groups[1].Value.Trim();


				if (chapter == manga.Chapter) continue;


				if (openLinks == "1") {
					Process.Start(link);
					manga.Chapter = chapter;
					manga.Link = link;
					manga.Date = t1;
					Sqlite.UpdateManga(manga);
				}
				DebugText.Write($"[Batoto] {mangaTitle} Found new Chapter");
			}
		}
	}
}