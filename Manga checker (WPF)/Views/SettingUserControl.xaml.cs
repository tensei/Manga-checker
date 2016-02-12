using System.Windows.Controls;
using Manga_checker.ViewModels;

namespace Manga_checker.Views {
    /// <summary>
    ///     Interaktionslogik für SettingUserControl.xaml
    /// </summary>
    public partial class SettingUserControl : UserControl {
        public SettingUserControl() {
            InitializeComponent();
            DataContext = new SettingViewModel();
        }
    }
}