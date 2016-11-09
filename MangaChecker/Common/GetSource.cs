using System;
using System.Text;
using RestSharp;

namespace MangaChecker.Common {
	internal static class GetSource {
		public static string Get(string url) {
			try {
				var client = new RestClient {
					UserAgent =
						"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/53.0.2785.70 Safari/537.36",
					Encoding = Encoding.UTF8,
					Timeout = 60000,
					BaseUrl = new Uri(url)
				};
				var request = new RestRequest();
				var response = client.Execute(request);
				return response.Content;
			} catch (Exception e) {
				DebugText.Write($"{url}\n{e.Message}");
				return null;
			}
		}
	}
}