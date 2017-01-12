using System;
using System.IO;
using System.ServiceModel.Syndication;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace MangaChecker.Common {
	public static class RssReader {
		public static async Task<SyndicationFeed> Read(string url) {
			try {
				string allXml;
				try {
					allXml = await GetSource.GetAsync(url);
				} catch {
					var bytes = Encoding.Default.GetBytes(await CloudflareGetString.GetAsync(url));
					allXml = Encoding.UTF8.GetString(bytes);
				}
				SyndicationFeed feed;
				try {
					var xmlr = XmlReader.Create(new StringReader(allXml));
					feed = SyndicationFeed.Load(xmlr);
				} catch (Exception) {
					allXml = allXml.Replace("pubDate", "fuck").Replace("&#45;", "-")
						.Replace("lastBuildDate", "fuck2");
					allXml = Regex.Replace(allXml, "<img src=\".+\"  />", "fuck");
					var xmlr = XmlReader.Create(new StringReader(allXml));
					feed = SyndicationFeed.Load(xmlr);
				}
				return feed;
			} catch (Exception e) {
				DebugText.Write($"[RSSERROR] {e.Message} {url}");
				return null;
			}
		}
	}
}