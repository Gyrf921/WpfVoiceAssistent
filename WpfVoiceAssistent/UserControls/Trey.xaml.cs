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
using System.Windows.Shapes;

namespace WpfVoiceAssistent.UserControls
{
    /// <summary>
    /// Логика взаимодействия для Trey.xaml
    /// </summary>
    public partial class Trey : Window
    {
        MainWindow window;
        public Trey()
        {
            InitializeComponent();
            window = new MainWindow(true);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            window.Close();
            this.Close();
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            window.Show();
            window.Visibility = Visibility.Visible;
        }

    }
}
