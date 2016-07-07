using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using Manga_checker.Database;
using Manga_checker.Sites;
using Manga_checker.Utilities;

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
        }

        private Visibility _mangastreamVisibility { get; set; } = Visibility.Collapsed;
        private Visibility _mangareadervVisibility { get; set; } = Visibility.Collapsed;
        private Visibility _mangafoxVisibility { get; set; } = Visibility.Collapsed;
        private Visibility _mangahereVisibility { get; set; } = Visibility.Collapsed;
        private Visibility _batotoVisibility { get; set; } = Visibility.Collapsed;
        private Visibility _yoMangaVisibility { get; set; } = Visibility.Collapsed;
        private Visibility _webtoonsVisibility { get; set; } = Visibility.Collapsed;
        private Visibility _kissmangaVisibility { get; set; } = Visibility.Collapsed;


        public Visibility MangastreamVisibility {
            get { return _mangastreamVisibility; }
            set {
                _mangastreamVisibility = value;
                OnPropertyChanged();
            }
        }

        public Visibility MangareadervVisibility {
            get { return _mangareadervVisibility; }
            set {
                _mangareadervVisibility = value;
                OnPropertyChanged();
            }
        }

        public Visibility MangafoxVisibility {
            get { return _mangafoxVisibility; }
            set {
                _mangafoxVisibility = value;
                OnPropertyChanged();
            }
        }

        public Visibility MangahereVisibility {
            get { return _mangahereVisibility; }
            set {
                _mangahereVisibility = value;
                OnPropertyChanged();
            }
        }

        public Visibility BatotoVisibility {
            get { return _batotoVisibility; }
            set {
                _batotoVisibility = value;
                OnPropertyChanged();
            }
        }

        public Visibility YoMangaVisibility {
            get { return _yoMangaVisibility; }
            set {
                _yoMangaVisibility = value;
                OnPropertyChanged();
            }
        }

        public Visibility WebtoonsVisibility {
            get { return _webtoonsVisibility; }
            set {
                _webtoonsVisibility = value;
                OnPropertyChanged();
            }
        }

        public Visibility KissmangaVisibility {
            get { return _kissmangaVisibility; }
            set {
                _kissmangaVisibility = value;
                OnPropertyChanged();
            }
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
        private bool _sendinfoOnOff { get; set; }

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

        public bool SendinfoOnOff {
            get { return _sendinfoOnOff; }
            set {
                if (_sendinfoOnOff == value) return;
                _sendinfoOnOff = value;
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
        public ICommand SendinfoCommand { get; }
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
                MangastreamVisibility = Visibility.Visible;
            }
            if (settings["mangareader"] == "1") {
                MangareaderOnOff = true;
                MangareadervVisibility = Visibility.Visible;
            }
            if (settings["mangafox"] == "1") {
                MangafoxOnOff = true;
                MangafoxVisibility = Visibility.Visible;
            }
            if (settings["mangahere"] == "1") {
                MangahereOnOff = true;
                MangahereVisibility = Visibility.Visible;
            }
            if (settings["batoto"] == "1") {
                BatotoOnOff = true;
                BatotoVisibility = Visibility.Visible;
            }
            if (settings["kissmanga"] == "1") {
                KissmangaOnOff = true;
                KissmangaVisibility = Visibility.Visible;
            }
            if (settings["webtoons"] == "1") {
                WebtoonsOnOff = true;
                WebtoonsVisibility = Visibility.Visible;
            }
            if (settings["yomanga"] == "1") {
                YomangaOnOff = true;
                YoMangaVisibility = Visibility.Visible;
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
                MangastreamVisibility = Visibility.Visible;
                //MangastreamBtn.Visibility = Visibility.Visible;
            } else {
                Sqlite.UpdateSetting("mangastream", "0");
                MangastreamVisibility = Visibility.Collapsed;
                //MangastreamBtn.Visibility = Visibility.Collapsed;
            }
        }

        private void MangareaderOnOffBtn_Click() {
            if (!Equals(MangareaderOnOff, false)) {
                Sqlite.UpdateSetting("mangareader", "1");
                MangareadervVisibility = Visibility.Visible;
                //MangareaderBtn.Visibility = Visibility.Visible;
            } else {
                Sqlite.UpdateSetting("mangareader", "0");
                MangareadervVisibility = Visibility.Collapsed;
                //MangareaderBtn.Visibility = Visibility.Collapsed;
            }
        }

        private void MangafoxOnOffBtn_Click() {
            if (!Equals(MangafoxOnOff, false)) {
                Sqlite.UpdateSetting("mangafox", "1");
                MangafoxVisibility = Visibility.Visible;
                //MangafoxBtn.Visibility = Visibility.Visible;
            } else {
                Sqlite.UpdateSetting("mangafox", "0");
                MangafoxVisibility = Visibility.Collapsed;
                //MangafoxBtn.Visibility = Visibility.Collapsed;
            }
        }

        private void MangahereOnOffBtn_Click() {
            if (!Equals(MangahereOnOff, false)) {
                Sqlite.UpdateSetting("mangahere", "1");
                MangahereVisibility = Visibility.Visible;
                //MangafoxBtn.Visibility = Visibility.Visible;
            } else {
                Sqlite.UpdateSetting("mangahere", "0");
                MangahereVisibility = Visibility.Collapsed;
                //MangafoxBtn.Visibility = Visibility.Collapsed;
            }
        }

        private void KissmangaOnOffBtn_Click() {
            if (!Equals(KissmangaOnOff, false)) {
                Sqlite.UpdateSetting("kissmanga", "1");
                KissmangaVisibility = Visibility.Visible;
                //KissmangaBtn.Visibility = Visibility.Visible;
            } else {
                Sqlite.UpdateSetting("kissmanga", "0");
                KissmangaVisibility = Visibility.Collapsed;
                //KissmangaBtn.Visibility = Visibility.Collapsed;
            }
        }

        private void BatotoOnOffBtn_Click() {
            if (!Equals(BatotoOnOff, false)) {
                Sqlite.UpdateSetting("batoto", "1");
                BatotoVisibility = Visibility.Visible;
                //BatotoBtn.Visibility = Visibility.Visible;
            } else {
                Sqlite.UpdateSetting("batoto", "0");
                BatotoVisibility = Visibility.Collapsed;
                //BatotoBtn.Visibility = Visibility.Collapsed;
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
                WebtoonsVisibility = Visibility.Visible;
                //WebtoonsBtn.Visibility = Visibility.Visible;
            } else {
                Sqlite.UpdateSetting("webtoons", "0");
                WebtoonsVisibility = Visibility.Collapsed;
                //WebtoonsBtn.Visibility = Visibility.Collapsed;
            }
        }

        private void YomangaOnOffBtn_Click() {
            if (!Equals(YomangaOnOff, false)) {
                Sqlite.UpdateSetting("yomanga", "1");
                YoMangaVisibility = Visibility.Visible;
                //YomangaBtn.Visibility = Visibility.Visible;
            } else {
                Sqlite.UpdateSetting("yomanga", "0");
                YoMangaVisibility = Visibility.Collapsed;
                //YomangaBtn.Visibility = Visibility.Collapsed;
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