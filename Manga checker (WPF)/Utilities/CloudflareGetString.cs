using CloudFlareUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Manga_checker.Utilities {
    class CloudflareGetString {
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
