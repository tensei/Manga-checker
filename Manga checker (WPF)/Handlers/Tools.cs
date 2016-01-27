using Manga_checker.ViewModels;
using MaterialDesignThemes.Wpf;

namespace Manga_checker.Handlers {
    internal class Tools {
        public static void Delete(MangaItemViewModel mangaItem) {
            var dialog = new ConfirmDeleteDialog {
                MessageTextBlock = {
                    Text = "Deleting\n" + mangaItem.Name
                },
                SiteName = {
                    Text = mangaItem.Site
                },
                item = mangaItem
            };
            DialogHost.Show(dialog);
        }
    }
}