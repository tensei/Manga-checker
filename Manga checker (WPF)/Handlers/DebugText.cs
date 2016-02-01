using System;
using Manga_checker.Properties;

namespace Manga_checker.Handlers {
    public class DebugText {
        public static void Write(string text) {
            //Read
            Settings.Default.Debug += $"[{DateTime.Now}] {text}\n";
        }
    }
}