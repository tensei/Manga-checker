using System.Threading.Tasks;

namespace Manga_checker.Handlers {
    using System;

    using Manga_checker.Database;
    using Manga_checker.ViewModels;

    using MaterialDesignThemes.Wpf;

    internal class Tools {
        public static void ChangeChaperNum(MangaModel item, string op) {
            if (!item.Chapter.Contains(" ")) {
                var chapter = int.Parse(item.Chapter);
                if (op.Equals("-")) {
                    chapter--;
                }
                else {
                    chapter++;
                }

                item.Chapter = chapter.ToString();
                Sqlite.UpdateManga(item.Site, item.Name, item.Chapter, item.Link, DateTime.Now, false);
            }
        }

        public static void CreateDb() {
            DialogHost.Show(new SetupDatabaseDialog());
        }

        public static async Task<bool> Delete(MangaModel mangaItem)
        {
            var dialog = new ConfirmDeleteDialog
            {
                MessageTextBlock = {
                    Text = "Deleting\n" + mangaItem.Name
                },
                SiteName = {
                    Text = mangaItem.Site
                },
                item = mangaItem
            };
            var x = await DialogHost.Show(dialog);
            return (string)x == "1";
        }
    }
}