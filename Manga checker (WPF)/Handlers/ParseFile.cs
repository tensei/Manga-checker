using System;
using System.Collections.Generic;
using System.IO;
using Manga_checker.Handlers;
using Manga_checker.Properties;
using Newtonsoft.Json.Linq;

namespace Manga_checker
{
    internal class ParseFile : IDisposable
    {
        public const string Path = @"manga.json";
        public const string SettingsPath = @"settings.json";

        public Config config = new Config();

        public void Dispose()
        {
            throw new NotImplementedException();
        }

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
            foreach (var manga in conf[site].Value<JObject>())
            {
                var ch = JObject.Parse(manga.Value.ToString());
                //ch["chapter"].ToString();
                jsmanga.Add(manga.Key + "[]" + ch["chapter"]);
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

        public void SetManga(string site, string Name, string Value, string status)
        {
            var conf = config.GetMangaConfig();
            try
            {
                conf[site][Name]["chapter"] = Value;
                conf[site][Name]["new"] = status;
            }
            catch (Exception)
            {
                AddManga(site, Name, Value, status);
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

        public string GetValueStatus(string site, string name)
        {
            var conf = config.GetMangaConfig();
            try
            {
                return conf[site][name]["new"].ToString();
            }
            catch (Exception)
            {
                return "false";
            }
        }

        public void SetValueStatus(string site, string name, string status)
        {
            var conf = config.GetMangaConfig();
            conf[site][name]["new"] = status;
            File.WriteAllText(Path, conf.ToString());
        }

        public void AddManga(string site, string name, string chapter, string status)
        {
            var ch = JObject.Parse("{'chapter': '" + chapter + "', 'new': '" + status + "', 'not read': []}");
            var conf = config.GetMangaConfig();
            conf[site][name] = ch;
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

        public List<float> GetNotReadList(string site, string name)
        {
            var NotRead = new List<float>();
            var conf = config.GetMangaConfig();
            try
            {
                foreach (var it in conf[site][name]["not read"].Children())
                {
                    NotRead.Add(float.Parse(it.ToString()));
                }
                return NotRead;
            }
            catch (Exception)
            {
                return NotRead;
            }
        }

        public void AddToNotReadList(string site, string name, float chapter)
        {
            var NotRead = new List<float>();
            var conf = config.GetMangaConfig();
            try
            {
                foreach (var it in conf[site][name]["not read"].Children())
                {
                    if (NotRead.Contains(float.Parse(it.ToString())) != true)
                        NotRead.Add(float.Parse(it.ToString()));
                }
            }
            finally
            {
                NotRead.Add(chapter);
                conf[site][name]["not read"] = JToken.FromObject(NotRead);
                File.WriteAllText(Path, conf.ToString());
            }
        }

        public void RemoveFromNotRead(string site, string name, float chapter)
        {
            var NotRead = new List<float>();
            var conf = config.GetMangaConfig();
            foreach (var it in conf[site][name]["not read"].Children())
            {
                NotRead.Add(float.Parse(it.ToString()));
            }
            NotRead.Remove(chapter);
            conf[site][name]["not read"] = JToken.FromObject(NotRead);
            File.WriteAllText(Path, conf.ToString());
        }

        public string GetValueChapter(string site, string Name)
        {
            var conf = config.GetMangaConfig();
            return conf[site][Name]["chapter"].ToString();
        }

        public List<float> GetHigherList(string name)
        {
            var higherList = new List<float>();
            var conf = config.GetMangaConfig();
            try
            {
                foreach (var it in conf["batoto"][name]["higher"].Children())
                {
                    higherList.Add(float.Parse(it.ToString()));
                }
                return higherList;
            }
            catch (Exception)
            {
                return higherList;
            }
        }

        public void AddHigherList(string name, float chapter)
        {
            var higherList = new List<float>();
            var conf = config.GetMangaConfig();
            try
            {
                foreach (var it in conf["batoto"][name]["higher"].Children())
                {
                    if (higherList.Contains(float.Parse(it.ToString())) != true)
                        higherList.Add(float.Parse(it.ToString()));
                }
            }
            finally
            {
                higherList.Add(chapter);
                conf["batoto"][name]["higher"] = JToken.FromObject(higherList);
                File.WriteAllText(Path, conf.ToString());
            }
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