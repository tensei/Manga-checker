﻿using System;
using System.Threading.Tasks;
using MangaChecker.Common;
using MangaChecker.Models;

namespace MangaChecker.Adding.Sites {
	public static class WebtoonsGetInfo {
		public static async Task<MangaInfoModel> Get(string url) {
			var manga = new MangaInfoModel();
			if (url.Contains("list?")) {
				url = url.Replace("list?", "rss?");
				try {
					var rss = await RssReader.Read(url);
					manga.Name = rss.Title.Text;
					foreach (var item in rss.Items) {
						DebugText.Write(item.Title.Text);
						manga.Chapter = item.Title.Text.Replace("Ep. ", "");
						manga.Link = item.Links[0].Uri.AbsoluteUri;
						manga.Rss = url;
						manga.Site = "webtoons";
						manga.Date = item.PublishDate.DateTime;
						return manga;
					}
				} catch (Exception e) {
					DebugText.Write(e.Message);
					return new MangaInfoModel {
						Error = "error"
					};
				}
			}
			manga.Error = "error";
			return manga;
		}
	}
}