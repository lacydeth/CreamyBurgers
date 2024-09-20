using System.Windows;

namespace CreamyBurgers
{
    public partial class User : Window
    {
        public User()
        {
            InitializeComponent();

            // Set window size to full screen, excluding caption height
            this.Width = SystemParameters.PrimaryScreenWidth;
            this.Height = SystemParameters.PrimaryScreenHeight - SystemParameters.WindowCaptionHeight;
            this.WindowStartupLocation = WindowStartupLocation.Manual;
            this.Left = 0;
            this.Top = 0;

            // Initially, show the cart panel and hide the profile panel
            ShowCartPanel(true);
            ProfilePanelContainer.Visibility = Visibility.Collapsed;
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
            // Show the home page and the cart panel
            ShowCartPanel(true);
            ProfilePanelContainer.Visibility = Visibility.Collapsed;
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            // Hide the cart panel and show the profile panel
            ShowCartPanel(false);
            ProfilePanelContainer.Visibility = Visibility.Visible;
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

        private void ShowCartPanel(bool isVisible)
        {
            CartPanel.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
