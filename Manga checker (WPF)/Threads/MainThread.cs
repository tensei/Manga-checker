using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Manga_checker.Handlers;
using Manga_checker.Properties;
using Manga_checker.Sites;
using MaterialDesignThemes.Wpf;

namespace Manga_checker.Threads {
    internal class MainThread {
        private static List<string> _mlist;

        public static void CheckNow() {
            var i = 5;
            var count = 0;
            var ms = new MangastreamRSS();
            var mf = new MangafoxRSS();
            var mr = new MangareaderHTML();
            var ba = new BatotoRSS();
            var kiss = new KissmangaHTML();
            var toons = new WebtoonsRSS();
            var yoo = new YomangaRSS();
            while (true) {
                if (Settings.Default.ForceCheck.Equals("force")) {
                    i = 3;
                    Settings.Default.ForceCheck = "";
                }
                if (i >= 1) {
                    Settings.Default.StatusLabel = "Status: Checking in " + i + " seconds.";
                    Thread.Sleep(1000);
                    i--;

                    Settings.Default.CounterLabel = count.ToString();
                }
                else {
                    if (ParseFile.GetValueSettings("mangastream") == "1") {
                        Settings.Default.StatusLabel = "Status: Checking Mangastream";
                        try {
                            ms.checked_if_new();
                        }
                        catch (Exception mst) {
                            DebugText.Write($"[{DateTime.Now}][Mangastream] Error {mst.Message} {mst.Data}");
                        }
                    }
                    if (ParseFile.GetValueSettings("mangafox") == "1") {
                        Settings.Default.StatusLabel = "Status: Checking Mangafox";
                        foreach (var manga in ParseFile.GetManga("mangafox")) {
                            //debugText(string.Format("[{0}][Mangafox] Checking {1}.", DateTime.Now, manga.Replace("[]", " ")));
                            mf.check_all(manga);
                        }
                    }
                    if (ParseFile.GetValueSettings("mangareader") == "1") {
                        Settings.Default.StatusLabel = "Status: Checking Mangareader";
                        foreach (var manga in ParseFile.GetManga("mangareader")) {
                            try {
                                //debugText(string.Format("[{0}][Mangareader] Checking {1}.", DateTime.Now,manga.Replace("[]", " ")));
                                mr.Check(manga);
                                Thread.Sleep(1000);
                            }
                            catch (Exception mrd) {
                                // lol
                                DebugText.Write(string.Format("[{1}][Mangareader] Error {0} {2}.",
                                    manga.Replace("[]", " "),
                                    DateTime.Now, mrd.Message));
                            }
                        }
                    }
                    if (ParseFile.GetValueSettings("batoto") == "1") {
                        Settings.Default.StatusLabel = "Status: Checking Batoto";
                        try {
                            _mlist = ba.Get_feed_titles();
                            ba.Check(_mlist);
                            Thread.Sleep(1000);
                        }
                        catch (Exception bat) {
                            // lol
                            DebugText.Write(string.Format("[{0}][batoto] Error {1}.", DateTime.Now, bat.Message));
                        }
                    }
                    if (ParseFile.GetValueSettings("kissmanga") == "1") {
                        Settings.Default.StatusLabel = "Status: Checking Kissmanga";
                        foreach (var manga in ParseFile.GetManga("kissmanga")) {
                            try {
                                //DebugText(string.Format("[{0}][Kissmanga] Checking {1}.", DateTime.Now, manga.Replace("[]", " ")));
                                var man = manga.Split(new[] {"[]"}, StringSplitOptions.None);
                                kiss.check(man[0], man[1]);
                                Thread.Sleep(5000);
                            }
                            catch (Exception mrd) {
                                // lol
                                DebugText.Write(string.Format("[{1}][Kissmanga] Error {0} {2}.",
                                    manga.Replace("[]", " "),
                                    DateTime.Now, mrd.Message));
                            }
                        }
                    }
                    if (ParseFile.GetValueSettings("webtoons") == "1") {
                        Settings.Default.StatusLabel = "Status: Checking Webtoons";
                        try {
                            toons.Check();
                        }
                        catch (Exception to) {
                            DebugText.Write($"[{DateTime.Now}][Webtoons] Error {to.Message}.");
                        }
                    }
                    if (ParseFile.GetValueSettings("yomanga") == "1") {
                        Settings.Default.StatusLabel = "Status: Checking YoManga";
                        try {
                            yoo.Check();
                        }
                        catch (Exception to) {
                            DebugText.Write($"[{DateTime.Now}][YoManga] Error {to.Message}.");
                        }
                    }
                    //timer2.Start();
                    var waittime = int.Parse(ParseFile.GetValueSettings("refresh time"));

                    Settings.Default.StatusLabel = @"Status: Checking in " + waittime + " seconds.";
                    count++;
                    i = waittime;
                }
            }
        }
    }
}