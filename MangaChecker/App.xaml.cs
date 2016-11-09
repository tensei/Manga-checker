using System.Diagnostics;
using System.IO;
using System.Windows;
using MangaChecker.Common;
using MangaChecker.Database;
using MangaChecker.ViewModels;
using MangaChecker.Windows;

namespace MangaChecker {
	/// <summary>
	///     Interaktionslogik für "App.xaml"
	/// </summary>
	public partial class App : Application {
		private void AppStartup(object sender, StartupEventArgs args) {
			PluginHost.Instance.Initialize();
			if (!Debugger.IsAttached)
				ExceptionHandler.AddGlobalHandlers();

			if (File.Exists("MangaDB.sqlite"))
				Sqlite.UpdateDatabase();
			else Sqlite.SetupDatabase();
			var mainWindow = new MainWindow {
				DataContext = new MainWindowViewModel()
			};
			mainWindow.Show();
		}
	}
}