using System.Diagnostics;
using System.Windows.Input;
using MangaChecker.Models;

namespace MangaChecker.Windows {
	/// <summary>
	///     Interaktionslogik für HistoryWindow.xaml
	/// </summary>
	public partial class HistoryWindow {
		public HistoryWindow() {
			InitializeComponent();
		}

		private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
			if (DataGrid.SelectedIndex == -1) return;
			var item = (MangaModel) DataGrid.SelectedItem;
			Process.Start(item.Link);
		}
	}
}