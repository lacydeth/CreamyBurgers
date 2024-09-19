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

namespace CreamyBurgers
{
    /// <summary>
    /// Interaction logic for MyUserSettings.xaml
    /// </summary>
    public partial class MyUserSettings : Window
    {
        public MyUserSettings()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavBar.Visibility == Visibility.Visible)
            {
                NavBar.Visibility = Visibility.Collapsed;
                NavColumn.Width = new GridLength(0);
            }
            else
            {
                NavBar.Visibility = Visibility.Visible;
                NavColumn.Width = new GridLength(200);
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            User mainwindow = new User();
            mainwindow.Show();
            this.Close();
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            MyUserSettings myUserSettings = new MyUserSettings();
            myUserSettings.Show();
            this.Close();
        }

        private void OrderButton_Click(object sender, RoutedEventArgs e)
        {
            //rendelések xaml?

        }
    }
}
