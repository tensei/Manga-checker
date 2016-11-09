using System;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using MangaChecker.Adding.Sites;
using MangaChecker.Common;
using MangaChecker.Database;
using MangaChecker.Models;
using PropertyChanged;

namespace MangaChecker.ViewModels {
	[ImplementPropertyChanged]
	public class NormalAddViewModel {
		public NormalAddViewModel() {
			SearchCommand = new ActionCommand(Search);
			AddCommand = new ActionCommand(Add);
		}

		public string Link { get; set; }

		public Visibility Progressbar { get; set; }

		public Visibility InfoVisi { get; set; }

		public bool AddButtonEnabled { get; set; }

		public MangaInfoModel Manga { get; set; }

		public string ErrorText { get; set; }

		public ICommand SearchCommand { get; }
		public ICommand AddCommand { get; }

		private void Search() {
			Progressbar = Visibility.Visible;
			if (string.IsNullOrEmpty(Link)) {
				if (Manga != null)
					AddMenuViewModel.Instance.NormalAddDataContext = new NormalAddViewModel();
				return;
			}
			var t = new Thread(new ThreadStart(delegate {
				try {
					//search manga here
					if (Link.ToLower().Contains("mangareader.net")) Manga = MangareaderGetInfo.Get(Link);
					else if (Link.ToLower().Contains("mangafox.me")) Manga = MangafoxGetInfo.Get(Link);
					else if (Link.ToLower().Contains("readms.com") || Link.ToLower().Contains("mangastream.com"))
						Manga = MangastreamGetInfo.Get(Link);
					else if (Link.ToLower().Equals(string.Empty)) Manga.Error = "Link empty";
					else if (Link.ToLower().Contains("webtoons")) Manga = WebtoonsGetInfo.Get(Link);
				} catch (Exception error) {
					ErrorText = error.Message;
				}
				AddButtonEnabled = true;
			})) {IsBackground = true};
			t.Start();
		}

		private void Add() {
			var manga = new MangaModel {
				Name = Manga.Name,
				Chapter = Manga.Chapter,
				Link = Manga.Link,
				RssLink = Manga.Rss,
				Date = Manga.Date,
				Site = Manga.Site
			};
			if (Manga.Site.ToLower().Contains("mangareader"))
				if (Manga.Error == null) {
					DebugText.Write($"[Debug] Trying to add {Manga.Name} {Manga.Chapter}");
					ErrorText += Sqlite.AddManga(manga)
						? "\nSuccess!"
						: "\nAlready in list!";
					return;
				}
			if (Manga.Site.ToLower().Contains("mangafox"))
				if (Manga.Error == null) {
					DebugText.Write($"[Debug] Trying to add {Manga.Name} {Manga.Chapter}");
					ErrorText += Sqlite.AddManga(manga)
						? "\nSuccess!"
						: "\nAlready in list!";
					return;
				}
			if (Manga.Site.ToLower().Contains("mangastream"))
				if (Manga.Error == null) {
					DebugText.Write($"[Debug] Trying to add {Manga.Name} {Manga.Chapter}");
					ErrorText += Sqlite.AddManga(manga)
						? "\nSuccess!"
						: "\nAlready in list!";
					return;
				}
			Manga = new MangaInfoModel();
			ErrorText = "failed";
		}
	}
}