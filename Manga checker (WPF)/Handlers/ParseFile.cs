using System;
using System.Collections.Generic;
using System.IO;
using Manga_checker.Handlers;
using Manga_checker.Properties;
using Newtonsoft.Json.Linq;

namespace Manga_checker
{
    internal class ParseFile
    {
        public const string Path = @"manga.json";
        public const string SettingsPath = @"settings.json";

        public Config config = new Config();

        public void Debugtext(string text)
        {
            //Read
            Settings.Default.Debug += text + "\n";
            //Write settings to disk
            Settings.Default.Save();
        }

        public List<string> GetManga(string site)
        {
            var jsmanga = new List<string>();
            var conf = config.GetMangaConfig();
            if (!conf.ToString().Contains(site))
            {
                conf[site] = JObject.Parse("{}");
                File.WriteAllText(Path, conf.ToString().ToLower());
                return jsmanga;
            }
            var dick = conf[site].Value<JObject>();
            foreach (var manga in dick)
            {
                if (site == "webtoons")
                {
                    var obj = JObject.Parse(manga.Value.ToString());
                    jsmanga.Add(manga.Key + "[]" + obj["chapter"] + "[]" + obj["url"]);
                }
                else
                {
                    jsmanga.Add(manga.Key + "[]" + manga.Value);
                }
            }
            return jsmanga;
           
        }

        public List<string> GetBatotoMangaNames()
        {
            var jsmanga = new List<string>();
            var conf = config.GetMangaConfig();
            foreach (var manga in conf["batoto"].Value<JObject>())
            {
                jsmanga.Add(manga.Key);
            }

            return jsmanga;
        }

        public void SetManga(string site, string Name, string Value)
        {
            var conf = config.GetMangaConfig();
            try
            {
                if (site.Equals("webtoons"))
                {
                    conf[site][Name]["chapter"] = Value;
                }
                else
                {
                    conf[site][Name] = Value;
                }
            }
            catch (Exception)
            {
                AddManga(site, Name, Value, "");
            }

            File.WriteAllText(Path, conf.ToString());
        }

        public string GetValueSettings(string Name)
        {
            var conf = config.GetConfig();
            try
            {
                return conf["settings"][Name].ToString();
            }
            catch (Exception)
            {
                SetValueSettings(Name, "0");
                return "0";
            }
        }

        public void SetValueSettings(string Name, string Value)
        {
            var _config = config.GetConfig();
            _config["settings"][Name] = Value;
            File.WriteAllText(SettingsPath, _config.ToString());
        }

        
        public void AddManga(string site, string name, string chapter, string url)
        {
            var ch = JObject.Parse("{'chapter': '" + chapter + "', 'url': '" + url + "'}");
            var conf = config.GetMangaConfig();
            if (site.Equals("webtoons"))
            {
                conf[site][name] = ch;
            }
            else
            {
                conf[site][name] = chapter;
            }
            File.WriteAllText(Path, conf.ToString());
            Debugtext($"[{DateTime.Now}][Debug] Added {site} {name} {chapter} to .json file.");
        }

        public void RemoveManga(string site, string name)
        {
            var conf = config.GetMangaConfig();
            conf[site][name].Parent.Remove();
            File.WriteAllText(Path, conf.ToString());
            Debugtext($"[{DateTime.Now}][Debug] Removed {name} from Backlog.");
        }

        
        public string GetValueChapter(string site, string Name)
        {
            var conf = config.GetMangaConfig();
            return conf[site][Name].ToString();
        }

        
        
        public void AddMangatoBacklog(string site, string name, string chapter)
        {
            var conf = config.GetMangaConfig();
            try
            {
                conf[site][name] = chapter;
                File.WriteAllText(Path, conf.ToString());
                Debugtext($"[{DateTime.Now}][Debug] Added {site} {name} {chapter} to .json file.");
            }
            catch (Exception)
            {
                var ch = JObject.Parse("{'" + name + "': '" + chapter + "'}");
                conf[site] = ch;
                File.WriteAllText(Path, conf.ToString());
                Debugtext($"[{DateTime.Now}][Debug] Added {site} {name} {chapter} to .json file.");
            }
        }

        public List<string> GetBacklog()
        {
            var conf = config.GetMangaConfig();
            try
            {
                var jsmanga = new List<string>();
                foreach (var manga in conf["backlog"].Value<JObject>())
                {
                    jsmanga.Add(manga.Key + " : " + manga.Value);
                }
                return jsmanga;
            }
            catch (Exception)
            {
                var ch = JObject.Parse("{}");
                conf["backlog"] = ch;
                File.WriteAllText(Path, conf.ToString());
                return GetBacklog();
            }
        }
    }
}