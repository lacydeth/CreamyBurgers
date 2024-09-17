using System.Windows;

namespace CreamyBurgers
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            LoadingBeetweenWindows loadingWindow = new LoadingBeetweenWindows();
            loadingWindow.Show();
            this.Close();
        }

        private void RegistrationButton_Click(object sender, RoutedEventArgs e)
        {
            Registration registrationWindow = new Registration();
            registrationWindow.Show();
            this.Close(); 
        }

    }
}
