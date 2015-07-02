using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Manga_checker__WPF_
{
    /// <summary>
    ///     Interaktionslogik für Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        private readonly SolidColorBrush OffColorBg = new SolidColorBrush(Color.FromArgb(255, 35, 35, 35));
        private readonly SolidColorBrush onColorBg = new SolidColorBrush(Color.FromArgb(255, 5, 157, 228));
        private readonly ParseFile parse = new ParseFile();

        public Window1()
        {
            InitializeComponent();
            settings_set_value();
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            parse.SetValueSettings("refresh time", timebox.Text);
            parse.SetValueSettings("batoto_rss", rssbox.Text);
        }

        public void settings_set_value()
        {
            timebox.Text = parse.GetValueSettings("refresh time");
            rssbox.Text = parse.GetValueSettings("batoto_rss");
            if (parse.GetValueSettings("mangastream") == "1")
            {
                MangastreamBtn.Content = "ON";
                MangastreamBtn.Background = onColorBg;
            }
            else
            {
                MangastreamBtn.Content = "OFF";
                MangastreamBtn.Background = OffColorBg;
            }
            if (parse.GetValueSettings("mangareader") == "1")
            {
                MangareaderBtn.Content = "ON";
                MangareaderBtn.Background = onColorBg;
            }
            else
            {
                MangareaderBtn.Content = "OFF";
                MangareaderBtn.Background = OffColorBg;
            }
            if (parse.GetValueSettings("mangafox") == "1")
            {
                MangafoxBtn.Content = "ON";
                MangafoxBtn.Background = onColorBg;
            }
            else
            {
                MangafoxBtn.Content = "OFF";
                MangafoxBtn.Background = OffColorBg;
            }
            if (parse.GetValueSettings("batoto") == "1")
            {
                BatotoBtn.Content = "ON";
                BatotoBtn.Background = onColorBg;
            }
            else
            {
                BatotoBtn.Content = "OFF";
                BatotoBtn.Background = OffColorBg;
            }
            if (parse.GetValueSettings("kissmanga") == "1")
            {
                KissmangaBtn.Content = "ON";
                KissmangaBtn.Background = onColorBg;
            }
            else
            {
                KissmangaBtn.Content = "OFF";
                KissmangaBtn.Background = OffColorBg;
            }
            if (parse.GetValueSettings("open links") == "1")
            {
                LinkOpenBtn.Background = onColorBg;
                LinkOpenBtn.Content = "ON";
            }
            else
            {
                LinkOpenBtn.Background = OffColorBg;
                LinkOpenBtn.Content = "OFF";
            }
        }

        private void MangastreamBtn_Click(object sender, RoutedEventArgs e)
        {
            if (MangastreamBtn.Background == OffColorBg)
            {
                MangastreamBtn.Background = onColorBg;
                MangastreamBtn.Content = "ON";
                parse.SetValueSettings("mangastream", "1");
            }
            else
            {
                MangastreamBtn.Background = OffColorBg;
                MangastreamBtn.Content = "OFF";
                parse.SetValueSettings("mangastream", "0");
            }
        }

        private void MangareaderBtn_Click(object sender, RoutedEventArgs e)
        {
            if (MangareaderBtn.Background == OffColorBg)
            {
                MangareaderBtn.Background = onColorBg;
                MangareaderBtn.Content = "ON";
                parse.SetValueSettings("mangareader", "1");
            }
            else
            {
                MangareaderBtn.Background = OffColorBg;
                MangareaderBtn.Content = "OFF";
                parse.SetValueSettings("mangareader", "0");
            }
        }

        private void MangafoxBtn_Click(object sender, RoutedEventArgs e)
        {
            if (MangafoxBtn.Background == OffColorBg)
            {
                MangafoxBtn.Background = onColorBg;
                MangafoxBtn.Content = "ON";
                parse.SetValueSettings("mangafox", "1");
            }
            else
            {
                MangafoxBtn.Background = OffColorBg;
                MangafoxBtn.Content = "OFF";
                parse.SetValueSettings("mangafox", "0");
            }
        }

        private void KissmangaBtn_Click(object sender, RoutedEventArgs e)
        {
            if (KissmangaBtn.Background == OffColorBg)
            {
                KissmangaBtn.Background = onColorBg;
                KissmangaBtn.Content = "ON";
                parse.SetValueSettings("kissmanga", "1");
            }
            else
            {
                KissmangaBtn.Background = OffColorBg;
                KissmangaBtn.Content = "OFF";
                parse.SetValueSettings("kissmanga", "0");
            }
        }

        private void BatotoBtn_Click(object sender, RoutedEventArgs e)
        {
            if (BatotoBtn.Background == OffColorBg)
            {
                BatotoBtn.Background = onColorBg;
                BatotoBtn.Content = "ON";
                parse.SetValueSettings("batoto", "1");
            }
            else
            {
                BatotoBtn.Background = OffColorBg;
                BatotoBtn.Content = "OFF";
                parse.SetValueSettings("batoto", "0");
            }
        }

        private void LinkOpenBtn_Click(object sender, RoutedEventArgs e)
        {
            if (LinkOpenBtn.Background == OffColorBg)
            {
                LinkOpenBtn.Background = onColorBg;
                LinkOpenBtn.Content = "ON";
                parse.SetValueSettings("open links", "1");
            }
            else
            {
                LinkOpenBtn.Background = OffColorBg;
                LinkOpenBtn.Content = "OFF";
                parse.SetValueSettings("open links", "0");
            }
        }
    }
}