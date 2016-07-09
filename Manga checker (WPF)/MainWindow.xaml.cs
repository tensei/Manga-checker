using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Manga_checker.Properties;
using Manga_checker.Utilities;
using Manga_checker.ViewModels.Model;

namespace Manga_checker {
    /// <summary>
    ///     Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        // private DataGridMangasItem itm = new DataGridMangasItem();

        public MainWindow() {
            InitializeComponent();
            Settings.Default.Debug = "Debug shit goes in here!\n";
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e) {
            Close();
        }

        private void DebugTextBox_TextChanged(object sender, TextChangedEventArgs e) {
            DebugTextBox.ScrollToEnd();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e) {
            // ButtonColorChange();
            if (!File.Exists("MangaDB.sqlite")) {
                Tools.CreateDb();
            }
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.ChangedButton == MouseButton.Left) DragMove();
        }

        private void MiniBtn_Click(object sender, RoutedEventArgs e) {
            WindowState = WindowState.Minimized;
        }

        private void TopMostBtn_Click(object sender, RoutedEventArgs e) {
            Topmost = Topmost == false;
        }
    }
}