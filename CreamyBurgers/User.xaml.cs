using System;
using System.Windows;
using System.Windows.Controls;

namespace CreamyBurgers
{
    public partial class User : Window
    {
        public User()
        {
            InitializeComponent();

            this.Width = SystemParameters.PrimaryScreenWidth;
            this.Height = SystemParameters.PrimaryScreenHeight - SystemParameters.WindowCaptionHeight;
            this.WindowStartupLocation = WindowStartupLocation.Manual;
            this.Left = 0;
            this.Top = 0;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            User homePage = new User();
            homePage.Show();
            this.Close();
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProfilePanelContainer.Visibility == Visibility.Visible)
            {
                ProfilePanelContainer.Visibility = Visibility.Collapsed;
            }
            else
            {
                ProfilePanelContainer.Visibility = Visibility.Visible;
            }
        }

        private void OrdersButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            User homePage = new User();
            homePage.Show();
            this.Close();
        }
    }
}
