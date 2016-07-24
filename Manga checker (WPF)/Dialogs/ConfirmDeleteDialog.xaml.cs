﻿using System.Windows;
using System.Windows.Controls;
using Manga_checker.Database;
using Manga_checker.Models;

namespace Manga_checker.Dialogs {
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
            Sqlite.DeleteManga(item);
        }
    }
}