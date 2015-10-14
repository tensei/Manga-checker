using System;
using System.Collections.Generic;
using System.IO;
using Manga_checker.Properties;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Manga_checker
{
    internal class ParseFile : IDisposable
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public const string Path = @"manga.json";
        public JObject getMangaFile()
        {
            if (File.Exists(Path))
            {
                var jsmanga = new List<string>();
                //// read JSON directly from a file
                using (var file = File.OpenText(Path))
                using (var reader = new JsonTextReader(file))
                {
                    var o2 = (JObject) JToken.ReadFrom(reader);
                    return o2;
                }
            }
            return null;
        }

        public void debugtext(string text)
        {
            //Read
            Settings.Default.Debug += text + "\n";
            //Write settings to disk
            Settings.Default.Save();
        }

        public List<string> Mangastream_manga()
        {
            var jsmanga = new List<string>();
            var o2 = getMangaFile();
            foreach (var manga in o2["mangastream"].Value<JObject>())
            {
                var ch = JObject.Parse(manga.Value.ToString());
                //ch["chapter"].ToString();
                jsmanga.Add(manga.Key + "[]" + ch["chapter"]);
            }

            return jsmanga;
        }

        public List<string> Mangareader_manga()
        {
            var jsmanga = new List<string>();
            var o2 = getMangaFile();
            foreach (var manga in o2["mangareader"].Value<JObject>())
                {
                    var ch = JObject.Parse(manga.Value.ToString());
                    //ch["chapter"].ToString();
                    jsmanga.Add(manga.Key + "[]" + ch["chapter"]); //
                }

            return jsmanga;
        }

        public List<string> Mangafox_manga()
        {
            var jsmanga = new List<string>();
            var o2 = getMangaFile();
            foreach (var manga in o2["mangafox"].Value<JObject>())
                {
                    var ch = JObject.Parse(manga.Value.ToString());
                    //ch["chapter"].ToString();
                    jsmanga.Add(manga.Key + "[]" + ch["chapter"]);
                }

            return jsmanga;
        }

        public List<string> Batoto_manga()
        {
            var jsmanga = new List<string>();
            var o2 = getMangaFile();
                foreach (var manga in o2["batoto"].Value<JObject>())
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
            var o2 = getMangaFile();
            foreach (var manga in o2["batoto"].Value<JObject>())
                {
                    jsmanga.Add(manga.Key);
                }

            return jsmanga;
        }

        public void setManga(string site, string Name, string Value, string status)
        {
            var file = File.OpenText(Path);
            var jsonResponse = JObject.Parse(file.ReadToEnd());
            try
            {
                jsonResponse[site][Name]["chapter"] = Value;
                jsonResponse[site][Name]["new"] = status;
            }
            catch (Exception)
            {
                AddManga(site, Name, Value, status);
            }

            file.Dispose();
            File.WriteAllText(Path, jsonResponse.ToString());
        }

        public string GetValueSettings(string Name)
        {
            var file = File.OpenText(Path);
            var jsonResponse = JObject.Parse(file.ReadToEnd());
            try
            {
                file.Dispose();
                return jsonResponse["settings"][Name].ToString();
            }
            catch (Exception)
            {
                SetValueSettings(Name, "0");
                file.Dispose();
                return "0";
            }
        }

        public void SetValueSettings(string Name, string Value)
        {
            var file = File.OpenText(Path);
            var jsonResponse = JObject.Parse(file.ReadToEnd());
            jsonResponse["settings"][Name] = Value;
            file.Dispose();
            File.WriteAllText(Path, jsonResponse.ToString());
            file.Dispose();
        }

        public string GetValueStatus(string site, string name)
        {
            var file = File.OpenText(Path);
            var jsonResponse = JObject.Parse(file.ReadToEnd());
            file.Dispose();
            return jsonResponse[site][name]["new"].ToString();
        }

        public void SetValueStatus(string site, string name, string status)
        {
            var file = File.OpenText(Path);
            var jsonResponse = JObject.Parse(file.ReadToEnd());
            file.Dispose();
            jsonResponse[site][name]["new"] = status;
            File.WriteAllText(Path, jsonResponse.ToString());
        }

        public void AddManga(string site, string name, string chapter, string status)
        {
            var ch = JObject.Parse("{'chapter': '" + chapter + "', 'new': '" + status + "', 'not read': []}");
            var file = File.OpenText(Path);
            var jsonResponse = JObject.Parse(file.ReadToEnd());
            jsonResponse[site][name] = ch;
            file.Dispose();
            File.WriteAllText(Path, jsonResponse.ToString());
            debugtext($"[{DateTime.Now}][Debug] Added {site} {name} {chapter} to .json file.");
        }

        public void RemoveManga(string site, string name)
        {
            var file = File.OpenText(Path);
            var jsonResponse = JObject.Parse(file.ReadToEnd());
            jsonResponse[site][name].Parent.Remove();
            file.Dispose();
            File.WriteAllText(Path, jsonResponse.ToString());
            debugtext($"[{DateTime.Now}][Debug] Removed {name} from Backlog.");
        }

        public List<float> GetNotReadList(string site, string name)
        {
            var NotRead = new List<float>();

            var file = File.OpenText(Path);
            var jsonResponse = JObject.Parse(file.ReadToEnd());
            file.Dispose();
            try
            {
                foreach (var it in jsonResponse[site][name]["not read"].Children())
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

            var file = File.OpenText(Path);
            var jsonResponse = JObject.Parse(file.ReadToEnd());
            file.Dispose();
            try
            {
                foreach (var it in jsonResponse[site][name]["not read"].Children())
                {
                    if (NotRead.Contains(float.Parse(it.ToString())) != true)
                        NotRead.Add(float.Parse(it.ToString()));
                }
            }
            finally
            {
                NotRead.Add(chapter);
                jsonResponse[site][name]["not read"] = JToken.FromObject(NotRead);
                File.WriteAllText(Path, jsonResponse.ToString());
            }
        }

        public void RemoveFromNotRead(string site, string name, float chapter)
        {
            var NotRead = new List<float>();

            var file = File.OpenText(Path);
            var jsonResponse = JObject.Parse(file.ReadToEnd());
            foreach (var it in jsonResponse[site][name]["not read"].Children())
            {
                NotRead.Add(float.Parse(it.ToString()));
            }
            NotRead.Remove(chapter);
            jsonResponse[site][name]["not read"] = JToken.FromObject(NotRead);
            file.Dispose();
            File.WriteAllText(Path, jsonResponse.ToString());
        }

        public string GetValueChapter(string site, string Name)
        {
            var file = File.OpenText(Path);
            var jsonResponse = JObject.Parse(file.ReadToEnd());
            file.Dispose();
            return jsonResponse[site][Name]["chapter"].ToString();
        }

        public List<float> GetHigherList(string name)
        {
            var higherList = new List<float>();

            var file = File.OpenText(Path);
            var jsonResponse = JObject.Parse(file.ReadToEnd());
            file.Dispose();
            try
            {
                foreach (var it in jsonResponse["batoto"][name]["higher"].Children())
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

            var file = File.OpenText(Path);
            var jsonResponse = JObject.Parse(file.ReadToEnd());
            file.Dispose();
            try
            {
                foreach (var it in jsonResponse["batoto"][name]["higher"].Children())
                {
                    if (higherList.Contains(float.Parse(it.ToString())) != true)
                        higherList.Add(float.Parse(it.ToString()));
                }
            }
            finally
            {
                higherList.Add(chapter);
                jsonResponse["batoto"][name]["higher"] = JToken.FromObject(higherList);
                File.WriteAllText(Path, jsonResponse.ToString());
            }
        }

        public void AddMangatoBacklog(string site, string name, string chapter)
        {
            try
            {
                var file = File.OpenText(Path);
                var jsonResponse = JObject.Parse(file.ReadToEnd());
                file.Dispose();
                jsonResponse[site][name] = chapter;
                File.WriteAllText(Path, jsonResponse.ToString());
                debugtext($"[{DateTime.Now}][Debug] Added {site} {name} {chapter} to .json file.");
            }
            catch (Exception)
            {
                var ch = JObject.Parse("{'" + name + "': '" + chapter + "'}");
                var file = File.OpenText(Path);
                var jsonResponse = JObject.Parse(file.ReadToEnd());
                jsonResponse[site] = ch;
                file.Dispose();
                File.WriteAllText(Path, jsonResponse.ToString());
                debugtext($"[{DateTime.Now}][Debug] Added {site} {name} {chapter} to .json file.");
            }
        }

        public List<string> GetBacklog()
        {
            try
            {
                var jsmanga = new List<string>();
                //// read JSON directly from a file
                using (var file = File.OpenText(Path))
                using (var reader = new JsonTextReader(file))
                {
                    var o2 = (JObject) JToken.ReadFrom(reader);
                    foreach (var manga in o2["backlog"].Value<JObject>())
                    {
                        jsmanga.Add(manga.Key + " : " + manga.Value);
                    }
                    file.Dispose();
                }
                return jsmanga;
            }
            catch (Exception)
            {
                var ch = JObject.Parse("{}");
                var file = File.OpenText(Path);
                var jsonResponse = JObject.Parse(file.ReadToEnd());
                jsonResponse["backlog"] = ch;
                file.Dispose();
                File.WriteAllText(Path, jsonResponse.ToString());
                return GetBacklog();
            }
        }
    }
}