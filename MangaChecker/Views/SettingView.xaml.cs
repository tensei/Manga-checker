using System.Windows.Controls;
using MangaChecker.ViewModels;

namespace MangaChecker.Views {
	/// <summary>
	///     Interaktionslogik für SettingView.xaml
	/// </summary>
	public partial class SettingView : UserControl {
		public SettingView() {
			InitializeComponent();
			DataContext = new SettingViewModel();
		}
	}
}