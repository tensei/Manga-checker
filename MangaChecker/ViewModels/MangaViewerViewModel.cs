using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using MangaChecker.Common;
using PropertyChanged;

namespace MangaChecker.ViewModels {
	[ImplementPropertyChanged]
	public class MangaViewerViewModel : ViewModelBase {
		private readonly ObservableCollection<Image> _images = new ObservableCollection<Image>();

		public MangaViewerViewModel() {
			Images = new ReadOnlyObservableCollection<Image>(_images);
			Show = new ActionCommand(FillImages);
			Canvas = Visibility.Collapsed;
			Fetchvis = Visibility.Visible;
		}

		public string Link { get; set; }
		public Visibility ErrorVisibility { get; set; } = Visibility.Collapsed;
		public Visibility Canvas { get; set; }
		public Visibility Fetchvis { get; set; }
		public Visibility PbarVisibility { get; set; } = Visibility.Collapsed;
		public ICommand Show { get; }

		public ReadOnlyObservableCollection<Image> Images { get; }

		public string Sites { get; set; } = "0 : 0";

		private async void FillImages() {
			Fetchvis = Visibility.Collapsed;
			PbarVisibility = Visibility.Visible;
			await Task.Run(() => {
				var x = SiteSelector(Link);
				if (x == null) return;
				foreach (var link in x) {
					if (string.IsNullOrEmpty(link)) continue;
					try {
						Application.Current.Dispatcher.BeginInvoke(new Action(() => {
							_images.Add(new Image {
								Source = new BitmapImage(new Uri(link, UriKind.RelativeOrAbsolute))
							});
						}));
						Thread.Sleep(500);
					} catch (Exception) {
						DebugText.Write($"[Error] failed to display img \"{link}\" ");
						continue;
					}
					Canvas = Visibility.Visible;
					PbarVisibility = Visibility.Collapsed;
				}
			});
		}

		private void ShowError() {
			PbarVisibility = Visibility.Collapsed;
			ErrorVisibility = Visibility.Visible;
		}

		private List<string> SiteSelector(string link) {
			var site = new List<string>();
			if (link.Equals("placeholder")) {
				ShowError();
				return null;
			}
			try {
				if (link.Contains("yomanga")) {
					var ret = ImageLinkCollecter.YomangaCollectLinks(Link);
					site = ret.Item1;
					Sites = $"{ret.Item1.Count} : {ret.Item2}";
				}
				if (link.Contains("jaiminisbox")) {
					var ret = ImageLinkCollecter.YomangaCollectLinks(Link);
					site = ret.Item1;
					Sites = $"{ret.Item1.Count} : {ret.Item2}";
				}
				if (link.Contains("kireicake")) {
					var ret = ImageLinkCollecter.YomangaCollectLinks(Link);
					site = ret.Item1;
					Sites = $"{ret.Item1.Count} : {ret.Item2}";
				}
				if (link.Contains("mangastream") || link.Contains("readms")) {
					var ret = ImageLinkCollecter.MangastreamCollectLinks(Link);
					site = ret.Item1;
					Sites = $"{ret.Item1.Count} : {ret.Item2}";
				}
			} catch (Exception) {
				ShowError();
				return null;
			}
			return site;
		}
	}
}