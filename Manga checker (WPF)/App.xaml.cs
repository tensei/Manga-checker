using System.Diagnostics;
using System.Windows;
using Manga_checker.Handlers;
using Manga_checker.ViewModels;

namespace Manga_checker {
    /// <summary>
    ///     Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application {
        private void AppStartup(object sender, StartupEventArgs args) {
            if (!Debugger.IsAttached)
                ExceptionHandler.AddGlobalHandlers();
            var mainWindow = new MainWindow {
                DataContext = new MainWindowViewModel()
            };
            mainWindow.Show();
        }
    }
}