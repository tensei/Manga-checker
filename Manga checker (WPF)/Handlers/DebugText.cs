using System;
using System.IO;
using Manga_checker.Properties;

namespace Manga_checker.Handlers {
    public class DebugText {
        public static void Write(string text) {
            //Read
            Settings.Default.Debug += $"[{DateTime.Now}] {text}\n";
            Log(text);
        }

        private static void Log(string text) {
            if (!Directory.Exists("logs"))
                Directory.CreateDirectory("logs");
            try {
                File.AppendAllText($"logs/{DateTime.Now.ToShortDateString()}-mc.log", $"[{DateTime.Now}] {text}\n");
            }
            catch {
            }
        }
    }
}