using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Input;
using Manga_checker.Common;
using Manga_checker.Database;
using Manga_checker.Sites;

namespace Manga_checker.ViewModels {
    public class SettingViewModel : ViewModelBase {
        public SettingViewModel() {
            if (File.Exists("MangaDB.sqlite")) {
                SetupSettingsPanel();
            }
            SaveCommand = new ActionCommand(SaveBtn_Click);
            MangastreamCommand = new ActionCommand(MangastreamOnOffBtn_Click);
            MangareaderCommand = new ActionCommand(MangareaderOnOffBtn_Click);
            MangafoxCommand = new ActionCommand(MangafoxOnOffBtn_Click);
            MangahereCommand = new ActionCommand(MangahereOnOffBtn_Click);
            BatotoCommand = new ActionCommand(BatotoOnOffBtn_Click);
            KissmangaCommand = new ActionCommand(KissmangaOnOffBtn_Click);
            WebtoonsCommand = new ActionCommand(WebtoonsOnOffBtn_Click);
            YomangaCommand = new ActionCommand(YomangaOnOffBtn_Click);
            LinkOpenCommand = new ActionCommand(LinkOpenBtn_Click);
            UpdateBatotoCommand = new ActionCommand(UpdateBatotoBtn_Click);
            GameOfScanlationCommand = new ActionCommand(GameOfScanlationOnOffBtn_Click);
        }


        private string _timeInternal { get; set; }
        private string _batotoRssInternal { get; set; }
        private string _impexpInternal { get; set; }
        private string _impexpmsgInternal { get; set; }

        private bool _mangastreamOnOff { get; set; }
        private bool _mangareaderOnOff { get; set; }
        private bool _mangafoxOnOff { get; set; }
        private bool _mangahereOnOff { get; set; }
        private bool _batotoOnOff { get; set; }
        private bool _kissmangaOnOff { get; set; }
        private bool _webtoonsOnOff { get; set; }
        private bool _yomangaOnOff { get; set; }
        private bool _linkOpen { get; set; }
        private bool _gameOfScanlationOnOff { get; set; }

        public string Timebox {
            get { return _timeInternal; }
            set {
                if (_timeInternal == value) return;
                _timeInternal = value;
                OnPropertyChanged();
            }
        }

        public string BatotoRss {
            get { return _batotoRssInternal; }
            set {
                if (_batotoRssInternal == value) return;
                _batotoRssInternal = value;
                OnPropertyChanged();
            }
        }

        public string ImportExportText {
            get { return _impexpInternal; }
            set {
                if (_impexpInternal == value) return;
                _impexpInternal = value;
                OnPropertyChanged();
            }
        }

        public string ImportExportMessageText {
            get { return _impexpmsgInternal; }
            set {
                if (_impexpmsgInternal == value) return;
                _impexpmsgInternal = value;
                OnPropertyChanged();
            }
        }

        public bool MangastreamOnOff {
            get { return _mangastreamOnOff; }
            set {
                if (_mangastreamOnOff == value) return;
                _mangastreamOnOff = value;
                OnPropertyChanged();
            }
        }

        public bool MangareaderOnOff {
            get { return _mangareaderOnOff; }
            set {
                if (_mangareaderOnOff == value) return;
                _mangareaderOnOff = value;
                OnPropertyChanged();
            }
        }

        public bool MangafoxOnOff {
            get { return _mangafoxOnOff; }
            set {
                if (_mangafoxOnOff == value) return;
                _mangafoxOnOff = value;
                OnPropertyChanged();
            }
        }


        public bool MangahereOnOff {
            get { return _mangahereOnOff; }
            set {
                if (_mangahereOnOff == value) return;
                _mangahereOnOff = value;
                OnPropertyChanged();
            }
        }

        public bool BatotoOnOff {
            get { return _batotoOnOff; }
            set {
                if (_batotoOnOff == value) return;
                _batotoOnOff = value;
                OnPropertyChanged();
            }
        }

        public bool KissmangaOnOff {
            get { return _kissmangaOnOff; }
            set {
                if (_kissmangaOnOff == value) return;
                _kissmangaOnOff = value;
                OnPropertyChanged();
            }
        }

        public bool WebtoonsOnOff {
            get { return _webtoonsOnOff; }
            set {
                if (_webtoonsOnOff == value) return;
                _webtoonsOnOff = value;
                OnPropertyChanged();
            }
        }

        public bool YomangaOnOff {
            get { return _yomangaOnOff; }
            set {
                if (_yomangaOnOff == value) return;
                _yomangaOnOff = value;
                OnPropertyChanged();
            }
        }

        public bool LinkOpen {
            get { return _linkOpen; }
            set {
                if (_linkOpen == value) return;
                _linkOpen = value;
                OnPropertyChanged();
            }
        }

        public bool GameOfScanlationOnOff {
            get { return _gameOfScanlationOnOff; }
            set {
                if (_gameOfScanlationOnOff == value) return;
                _gameOfScanlationOnOff = value;
                OnPropertyChanged();
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand MangastreamCommand { get; }
        public ICommand MangareaderCommand { get; }
        public ICommand MangafoxCommand { get; }
        public ICommand MangahereCommand { get; }
        public ICommand BatotoCommand { get; }
        public ICommand KissmangaCommand { get; }
        public ICommand WebtoonsCommand { get; }
        public ICommand YomangaCommand { get; }
        public ICommand GameOfScanlationCommand { get; }
        public ICommand LinkOpenCommand { get; }
        public ICommand UpdateBatotoCommand { get; }
        public ICommand ImportCommand { get; }
        public ICommand ExportCommand { get; }


        private void SetupSettingsPanel() {
            var settings = Sqlite.GetSettings();
            Timebox = settings["refresh time"];
            BatotoRss = settings["batoto_rss"];

            if (settings["mangastream"] == "1") {
                MangastreamOnOff = true;
            }
            if (settings["mangareader"] == "1") {
                MangareaderOnOff = true;
            }
            if (settings["mangafox"] == "1") {
                MangafoxOnOff = true;
            }
            if (settings["mangahere"] == "1") {
                MangahereOnOff = true;
            }
            if (settings["batoto"] == "1") {
                BatotoOnOff = true;
            }
            if (settings["kissmanga"] == "1") {
                KissmangaOnOff = true;
            }
            if (settings["webtoons"] == "1") {
                WebtoonsOnOff = true;
            }
            if (settings["yomanga"] == "1") {
                YomangaOnOff = true;
            }
            if (settings["goscanlation"] == "1") {
                GameOfScanlationOnOff = true;
            }
            if (settings["open links"] == "1") {
                LinkOpen = true;
            }
        }


        private void SaveBtn_Click() {
            Sqlite.UpdateSetting("refresh time", Timebox);
            Sqlite.UpdateSetting("batoto_rss", BatotoRss);
        }

        private void MangastreamOnOffBtn_Click() {
            if (!Equals(MangastreamOnOff, false)) {
                Sqlite.UpdateSetting("mangastream", "1");
            } else {
                Sqlite.UpdateSetting("mangastream", "0");
            }
        }

        private void MangareaderOnOffBtn_Click() {
            if (!Equals(MangareaderOnOff, false)) {
                Sqlite.UpdateSetting("mangareader", "1");
            } else {
                Sqlite.UpdateSetting("mangareader", "0");
            }
        }

        private void MangafoxOnOffBtn_Click() {
            if (!Equals(MangafoxOnOff, false)) {
                Sqlite.UpdateSetting("mangafox", "1");
            } else {
                Sqlite.UpdateSetting("mangafox", "0");
            }
        }

        private void MangahereOnOffBtn_Click() {
            if (!Equals(MangahereOnOff, false)) {
                Sqlite.UpdateSetting("mangahere", "1");
            } else {
                Sqlite.UpdateSetting("mangahere", "0");
            }
        }

        private void KissmangaOnOffBtn_Click() {
            if (!Equals(KissmangaOnOff, false)) {
                Sqlite.UpdateSetting("kissmanga", "1");
            } else {
                Sqlite.UpdateSetting("kissmanga", "0");
            }
        }

        private void BatotoOnOffBtn_Click() {
            if (!Equals(BatotoOnOff, false)) {
                Sqlite.UpdateSetting("batoto", "1");
            } else {
                Sqlite.UpdateSetting("batoto", "0");
            }
        }

        private void LinkOpenBtn_Click() {
            if (!Equals(LinkOpen, false)) {
                Sqlite.UpdateSetting("open links", "1");
            } else {
                Sqlite.UpdateSetting("open links", "0");
            }
        }

        private void WebtoonsOnOffBtn_Click() {
            if (!Equals(WebtoonsOnOff, false)) {
                Sqlite.UpdateSetting("webtoons", "1");
            } else {
                Sqlite.UpdateSetting("webtoons", "0");
            }
        }

        private void YomangaOnOffBtn_Click() {
            if (!Equals(YomangaOnOff, false)) {
                Sqlite.UpdateSetting("yomanga", "1");
            } else {
                Sqlite.UpdateSetting("yomanga", "0");
            }
        }

        private void GameOfScanlationOnOffBtn_Click() {
            if (!Equals(GameOfScanlationOnOff, false)) {
                Sqlite.UpdateSetting("goscanlation", "1");
            } else {
                Sqlite.UpdateSetting("goscanlation", "0");
            }
        }

        private void UpdateBatotoBtn_Click() {
            new Thread(new ThreadStart(delegate {
                var rssList = BatotoRSS.Get_feed_titles();
                var jsMangaList = Sqlite.GetMangaNameList("batoto");
                foreach (var rssManga in rssList) {
                    var name =
                        rssManga[0].ToString().Split(new[] {" - "}, StringSplitOptions.RemoveEmptyEntries)[0];
                    if (!jsMangaList.Contains(name)) {
                        jsMangaList.Add(name);
                        Sqlite.AddManga("batoto", name, (string) rssManga[1], "placeholder",
                            (DateTime) rssManga[3], (string) rssManga[2]);
                        DebugText.Write($"[Batoto] added {(string) rssManga[0]}");
                    }
                }
            })).Start();
        }

        public static string Base64Encode(string plainText) {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData) {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}