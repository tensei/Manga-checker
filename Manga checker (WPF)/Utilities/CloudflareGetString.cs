using System.Net.Http;
using CloudFlareUtilities;

namespace Manga_checker.Utilities {
    internal class CloudflareGetString {
        public static string Get(string url) {
            // Create the clearance handler.
            var handler = new ClearanceHandler();

            // Create a HttpClient that uses the handler.
            var client = new HttpClient(handler);

            // Use the HttpClient as usual. Any JS challenge will be solved automatically for you.
            var content = client.GetStringAsync(url).Result;
            return content;
        }
    }
}