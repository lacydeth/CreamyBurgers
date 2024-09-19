using System;
using System.Windows;

namespace CreamyBurgers
{
    public partial class User : Window
    {
        public User()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void UserButton_Click(object sender, RoutedEventArgs e)
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

        private void NavButton_Click(object sender, RoutedEventArgs e)
        {
            MyUserSettings myUserSettings = new MyUserSettings();
            myUserSettings.Show();
            this.Close();
        }
    }
}
