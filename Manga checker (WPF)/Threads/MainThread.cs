using System;
using System.Threading;
using MangaChecker.Common;
using MangaChecker.Database;
using MangaChecker.Properties;
using MangaChecker.Sites;
using MangaChecker.Sites.HTML;
using MangaChecker.Sites.RSS;

namespace MangaChecker.Threads {
    internal static class MainThread {
        public static void CheckNow() {
            var i = 5;
            var count = 0;
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
                } else {
                    var setting = Sqlite.GetSettings();
                    if (setting["mangastream"] == "1") {
                        Settings.Default.StatusLabel = "Status: Checking Mangastream";
                        var mslist = Mangastream.Get_feed_titles();
                        foreach (var manga in Sqlite.GetMangas("mangastream")) {
                            try {
                                Mangastream.Check(manga, mslist, setting["open links"]);
                            } catch (Exception mst) {
                                DebugText.Write($"[Mangastream] Error {mst.Message} {mst.Data}");
                            }
                        }
                    }
                    Thread.Sleep(100);
                    if (setting["batoto"] == "1") {
                        Settings.Default.StatusLabel = "Status: Checking Batoto";
                        var _mlist = Batoto.Get_feed_titles();
                        foreach (var manga in Sqlite.GetMangas("batoto")) {
                            try {
                                Batoto.Check(_mlist, manga, setting["open links"]);
                            } catch (Exception bat) {
                                DebugText.Write($"[batoto] Error {bat.Message}.");
                            }
                        }
                    }
                    Thread.Sleep(100);
                    if (setting["webtoons"] == "1") {
                        Settings.Default.StatusLabel = "Status: Checking Webtoons";
                        foreach (var manga in Sqlite.GetMangas("webtoons")) {
                            try {
                                Webtoons.Check(manga, setting["open links"]);
                            } catch (Exception to) {
                                DebugText.Write($"[Webtoons] Error {to.Message}.");
                            }
                        }
                    }
                    Thread.Sleep(100);
                    if (setting["goscanlation"] == "1") {
                        Settings.Default.StatusLabel = "Status: Checking GameOfScanlation";
                        foreach (var manga in Sqlite.GetMangas("goscanlation")) {
                            try {
                                GameOfScanlation.Check(manga, setting["open links"]);
                            } catch (Exception to) {
                                DebugText.Write($"[GameOfScanlation] Error {to.Message}.");
                            }
                        }
                    }
                    Thread.Sleep(100);
                    if (setting["yomanga"] == "1") {
                        Settings.Default.StatusLabel = "Status: Checking YoManga";
                        var rss = RssReader.Read("http://yomanga.co/reader/feeds/rss");
                        if (rss != null) {
                            foreach (var manga in Sqlite.GetMangas("yomanga")) {
                                try {
                                    Yomanga.Check(manga, rss, setting["open links"]);
                                } catch (Exception to) {
                                    DebugText.Write($"[YoManga] Error {to.Message}.");
                                }
                            }
                        }
                    }
                    Thread.Sleep(100);
                    if (setting["kireicake"] == "1") {
                        Settings.Default.StatusLabel = "Status: Checking KireiCake";
                        var rss = RssReader.Read("http://reader.kireicake.com/rss.xml");
                        if (rss != null) {
                            foreach (var manga in Sqlite.GetMangas("kireicake")) {
                                try {
                                    KireiCake.Check(manga, rss, setting["open links"]);
                                } catch (Exception to) {
                                    DebugText.Write($"[KireiCake] Error {to.Message}.");
                                }
                            }
                        }
                    }
                    Thread.Sleep(100);
                    if (setting["jaiminisbox"] == "1") {
                        Settings.Default.StatusLabel = "Status: Checking Jaiminisbox";
                        var rss = RssReader.Read("https://jaiminisbox.com/reader/rss.xml");
                        if (rss != null) {
                            foreach (var manga in Sqlite.GetMangas("jaiminisbox")) {
                                try {
                                    Jaiminisbox.Check(manga, rss, setting["open links"]);
                                } catch (Exception to) {
                                    DebugText.Write($"[Jaiminisbox] Error {to.Message}.");
                                }
                            }
                        }
                    }
                    Thread.Sleep(100);
                    if (setting["mangafox"] == "1") {
                        Settings.Default.StatusLabel = "Status: Checking Mangafox";
                        foreach (var manga in Sqlite.GetMangas("mangafox")) {
                            //DebugText.Write(string.Format("[{0}][Mangafox] Checking {1}.", DateTime.Now, manga.Replace("[]", " ")));
                            try {
                                Mangafox.Check(manga, setting["open links"]);
                            } catch (Exception mst) {
                                DebugText.Write($"[mangafox] Error {mst.Message} {mst.Data}");
                            }
                        }
                    }
                    Thread.Sleep(100);
                    if (setting["mangahere"] == "1") {
                        Settings.Default.StatusLabel = "Status: Checking Mangahere";
                        foreach (var manga in Sqlite.GetMangas("mangahere")) {
                            //DebugText.Write(string.Format("[{0}][Mangafox] Checking {1}.", DateTime.Now, manga.Replace("[]", " ")));
                            try {
                                Mangahere.Check(manga, setting["open links"]);
                            } catch (Exception mst) {
                                DebugText.Write($"[mangahere] Error {mst.Message} {mst.Data}");
                            }
                        }
                    }
                    Thread.Sleep(100);
                    if (setting["mangareader"] == "1") {
                        Settings.Default.StatusLabel = "Status: Checking Mangareader";
                        foreach (var manga in Sqlite.GetMangas("mangareader")) {
                            try {
                                //DebugText.Write(string.Format("[{0}][Mangareader] Checking {1}.", DateTime.Now,manga.Replace("[]", " ")));
                                MangareaderHTML.Check(manga, setting["open links"]);
                            } catch (Exception mrd) {
                                DebugText.Write($"[Mangareader] Error {manga.Name} {mrd.Message}.");
                            }
                        }
                    }
                    Thread.Sleep(100);
                    if (setting["kissmanga"] == "1") {
                        Settings.Default.StatusLabel = "Status: Checking Kissmanga";
                        foreach (var manga in Sqlite.GetMangas("kissmanga")) {
                            try {
                                //DebugText.Write(string.Format("[{0}][Kissmanga] Checking {1}.", DateTime.Now, manga.Replace("[]", " ")));
                                KissmangaHTML.Check(manga, setting["open links"]);
                            } catch (Exception mrd) {
                                // lol
                                DebugText.Write($"[Kissmanga] Error {manga.Name} {mrd.Message}.");
                            }
                        }
                    }
                    Thread.Sleep(100);
                    if (setting["heymanga"] == "1") {
                        Settings.Default.StatusLabel = "Status: Checking HeyManga";
                        var rss = RssReader.Read("https://www.heymanga.me/feed/");
                        if (rss != null) {
                            foreach (var manga in Sqlite.GetMangas("heymanga")) {
                                try {
                                    HeyManga.Check(manga, rss, setting["open links"]);
                                } catch (Exception to) {
                                    DebugText.Write($"[HeyManga] Error {to.Message}.");
                                }
                            }
                        }
                    }
                    //timer2.Start();
                    var waittime = int.Parse(setting["refresh time"]);

                    Settings.Default.StatusLabel = @"Status: Checking in " + waittime + " seconds.";
                    count++;
                    i = waittime;
                }
            }
            // ReSharper disable once FunctionNeverReturns
        }
    }
}