using System;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;
using MangaChecker.Common;
using MangaChecker.Models;

namespace MangaChecker.Views {
    /// <summary>
    ///     Interaktionslogik für MainView.xaml
    /// </summary>
    public partial class MainView : UserControl {
        public MainView() {
            InitializeComponent();
        }

        private void DataGridMangas_OnMouseDoubleClick(object sender, MouseButtonEventArgs e) {
            try {
                var itemselected = (MangaModel) DataGridMangas.SelectedItem;
                if (itemselected.Link != "placeholder") {
                    Process.Start(itemselected.Link);
                    itemselected.New = 0;
                }
            } catch (Exception g) {
                // do nothing
                DebugText.Write($"[Error] {g.Message} {g.TargetSite} ");
            }
        }
    }
}