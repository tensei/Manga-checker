using System;
using System.Net;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using Manga_checker.Handlers;
using Manga_checker.ViewModels;

namespace Manga_checker.Adding.Sites {
    public class webtoons {

        public static MangaViewModel GetInfo(string url) {
            var manga = new MangaViewModel();
            if (url.Contains("list?")) {
                url = url.Replace("list?", "rss?");
                try {
                    var rss = RSSReader.Read(url);
                    manga.Name = rss.Title.Text;
                    foreach (var item in rss.Items) {
                        DebugText.Write(item.Title.Text);
                        manga.Chapter = item.Title.Text.Replace("Ep. ", "");
                        manga.Link = item.Links[0].Uri.AbsoluteUri;
                        manga.RssLink = url;
                        manga.Site = "webtoons";
                        manga.Error = "null";
                        return manga;
                    }
                }
                catch (Exception e) {
                    DebugText.Write(e.Message);
                    return new MangaViewModel {
                        Error = "error"
                    };
                }
            }
            manga.Error = "error";
            return manga;
        }

    }
}
