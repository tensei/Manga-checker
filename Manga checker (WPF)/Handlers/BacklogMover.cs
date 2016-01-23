using Manga_checker.Database;

namespace Manga_checker.Handlers
{
    internal class BacklogMover
    {
        public void Move(string site, string name, string chapter)
        {
            ParseFile.RemoveManga("backlog", name);
            Sqlite.DeleteManga("backlog", name, chapter);
            ParseFile.AddManga(site, name, chapter, "");
            Sqlite.AddManga(site, name, chapter, "placeholder");
        }
    }
}