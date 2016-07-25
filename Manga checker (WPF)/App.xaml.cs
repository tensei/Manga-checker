using System.Diagnostics;
using System.IO;
using System.Windows;
using Manga_checker.Common;
using Manga_checker.Database;
using Manga_checker.ViewModels;
using Manga_checker.Windows;

namespace Manga_checker {
    /// <summary>
    ///     Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application {
        private void AppStartup(object sender, StartupEventArgs args) {
            if (!Debugger.IsAttached)
                ExceptionHandler.AddGlobalHandlers();

            if (File.Exists("MangaDB.sqlite"))
                Sqlite.UpdateDatabase();
            else {
                Sqlite.SetupDatabase();
            }
            var mainWindow = new MainWindow {
                DataContext = new MainWindowViewModel()
            };
            mainWindow.Show();
        }
    }
}