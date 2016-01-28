using System.IO;
using System.Windows;
using Manga_checker.Database;
using Manga_checker.Handlers;

namespace Manga_checker {
    /// <summary>
    ///     Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application {
        private void AppStartup(object sender, StartupEventArgs args) {

            var mainWindow = new MainWindow {
                DataContext = new MainWindowViewModel()
            };
            mainWindow.Show();
        }
    }
}