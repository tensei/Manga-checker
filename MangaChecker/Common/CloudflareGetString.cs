using System.Net.Http;
using System.Threading.Tasks;
using CloudFlareUtilities;

namespace MangaChecker.Common {
	internal static class CloudflareGetString {
		public static async Task<string> GetAsync(string url) {
			// Create the clearance handler.
			var handler = new ClearanceHandler();

			// Create a HttpClient that uses the handler.
			var client = new HttpClient(handler);

			// Use the HttpClient as usual. Any JS challenge will be solved automatically for you.
			var content = await client.GetStringAsync(url);
			return content;
		}
	}
}