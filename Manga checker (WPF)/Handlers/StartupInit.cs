using Manga_checker.Properties;

namespace Manga_checker.Handlers
{
    class StartupInit
    {
        public void Setup()
        {
            ParseFile parse = new ParseFile();
            Settings.Default.SettingBatoto = parse.GetValueSettings("batoto");
            Settings.Default.SettingMangastream = parse.GetValueSettings("mangastream");
            Settings.Default.SettingMangareader = parse.GetValueSettings("mangareader");
            Settings.Default.SettingMangafox = parse.GetValueSettings("mangafox");
            Settings.Default.SettingKissmanga = parse.GetValueSettings("kissmanga");
            Settings.Default.SettingWebtoons = parse.GetValueSettings("webtoons");
            Settings.Default.SettingOpenLinks = parse.GetValueSettings("open links");
            Settings.Default.SettingBatotoRSS = parse.GetValueSettings("batoto_rss");
            Settings.Default.SettingRefreshTime = int.Parse(parse.GetValueSettings("refresh time"));
            //Settings.Default.Save();
        }
    }
}
