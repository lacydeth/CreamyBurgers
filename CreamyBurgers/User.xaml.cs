using Microsoft.Data.Sqlite;
using System.Windows;

namespace CreamyBurgers
{
    public partial class User : Window
    {
        public User()
        {
            InitializeComponent();
            btnProfil.Content = Session.Username;
            string conn = "Data Source=creamyburgers.db";

            try
            {
                using (var sqlConn = new SqliteConnection(conn))
                {
                    sqlConn.Open();

                    string query = "SELECT street, city, state, zipCode, country, phoneNumber FROM addresses WHERE username=@username AND password=@password";

                    using (var sqlCommand = new SqliteCommand(query, sqlConn))
                    {
                        using (SqliteDataReader reader = sqlCommand.ExecuteReader())
                        {
                            if (reader.HasRows && reader.Read())
                            {
                                Session.Username = reader["username"].ToString();
                                Session.UserId = Convert.ToInt32(reader["id"]);
                                Session.PermId = Convert.ToInt32(reader["permID"]);
                            }
                            else
                            {
                                MessageBox.Show("Hibás adatok!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
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
            ShowCartPanel(true);
            ProfilePanelContainer.Visibility = Visibility.Collapsed;
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
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
