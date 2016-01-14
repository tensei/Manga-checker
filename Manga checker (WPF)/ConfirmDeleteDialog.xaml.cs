using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Manga_checker.Handlers;

namespace Manga_checker
{
    /// <summary>
    /// Interaktionslogik für ConfirmDialog.xaml
    /// </summary>
    public partial class ConfirmDeleteDialog : UserControl
    {
        public ConfirmDeleteDialog()
        {
            InitializeComponent();
        }
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ParseFile _ParseFile = new ParseFile();
            _ParseFile.RemoveManga("backlog", MessageTextBlock.Text.Replace("Deleting ", ""));

        }
    }
}
