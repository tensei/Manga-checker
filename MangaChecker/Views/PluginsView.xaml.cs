using MangaChecker.ViewModels;

namespace MangaChecker.Views {
	/// <summary>
	///     Interaction logic for PluginsView.xaml
	/// </summary>
	public partial class PluginsView {
		public PluginsView() {
			InitializeComponent();
			DataContext = new PluginsViewModel();
		}
	}
}