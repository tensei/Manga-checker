using System;
using System.Text;

namespace Manga_checker.Utilities {
    internal class Base64 {
        public string Base64Encode(string plainText) {
            try {
                var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
                return Convert.ToBase64String(plainTextBytes);
            }
            catch (Exception e) {
                DebugText.Write(e.Message);
                return "null";
            }
        }

        public string Base64Decode(string base64EncodedData) {
            try {
                var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
                return Encoding.UTF8.GetString(base64EncodedBytes);
            }
            catch (Exception e) {
                DebugText.Write(e.Message);
                return "null";
            }
        }
    }
}