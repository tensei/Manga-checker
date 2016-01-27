using System.Windows;
using System.Windows.Controls;
using Manga_checker.Database;
using Manga_checker.ViewModels;

namespace Manga_checker {
    /// <summary>
    ///     Interaktionslogik für ConfirmDialog.xaml
    /// </summary>
    public partial class ConfirmDeleteDialog : UserControl {
        public MangaItemViewModel item;

        public ConfirmDeleteDialog() {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            ParseFile.RemoveManga(item.Site.ToLower(), MessageTextBlock.Text.Replace("Deleting ", ""));
            Sqlite.DeleteManga(item.Site, item.Name, item.Chapter);
        }
    }
}