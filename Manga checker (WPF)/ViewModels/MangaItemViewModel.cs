﻿namespace Manga_checker.ViewModels
{
    public class MangaItemViewModel
    {
        public string Site { get; set; }
        public string Name { get; set; }
        public string Chapter { get; set; }
        public bool IsEnabled { get; set; } = true;
        public string FullName { get; set; }
    }
}