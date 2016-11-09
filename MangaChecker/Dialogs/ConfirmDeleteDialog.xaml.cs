using System.Windows;
using System.Windows.Controls;
using MangaChecker.Database;
using MangaChecker.Models;

namespace MangaChecker.Dialogs {
	/// <summary>
	///     Interaktionslogik für ConfirmDialog.xaml
	/// </summary>
	public partial class ConfirmDeleteDialog : UserControl {
		public MangaModel item;

		public ConfirmDeleteDialog() {
			InitializeComponent();
		}

		private void Button_Click(object sender, RoutedEventArgs e) {
			//ParseFile.RemoveManga(item.Site.ToLower(), item.Name);
			var sqliteDeleteManga = new SqliteDeleteManga(item);
		}
	}
}