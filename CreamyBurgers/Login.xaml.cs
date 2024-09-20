using System;
using System.Data;
using System.Windows;
using Microsoft.Data.Sqlite;
using EasyEncryption;

namespace CreamyBurgers
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        readonly string conn = "Data Source=creamyburgers.db";

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = tbUsername.Text.Trim();
            string password = EasyEncryption.MD5.ComputeMD5Hash(tbPassword.Password);

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                try
                {
                    using (var sqlConn = new SqliteConnection(conn))
                    {
                        sqlConn.Open();
                        string query = "SELECT id, username, password, permID FROM users WHERE username=@username AND password=@password";

                        using (var sqlCommand = new SqliteCommand(query, sqlConn))
                        {
                            sqlCommand.Parameters.AddWithValue("@username", username);
                            sqlCommand.Parameters.AddWithValue("@password", password);

                            using (SqliteDataReader reader = sqlCommand.ExecuteReader())
                            {
                                if (reader.HasRows && reader.Read()) // Call reader.Read() to advance to the first row
                                {
                                    Session.Username = reader["username"].ToString();
                                    Session.UserId = Convert.ToInt32(reader["id"]);  // Convert to int safely
                                    Session.PermId = Convert.ToInt32(reader["permID"]);  // Convert to int safely

                                    MessageBox.Show($"{Session.Username} {Session.UserId} {Session.PermId}");
                                    sqlConn.Close();
                                    LoadingBeetweenWindows loadingWindow = new LoadingBeetweenWindows();
                                    loadingWindow.Show();
                                    this.Close();
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
            }
            else
            {
                MessageBox.Show("Adj meg felhasználónevet és jelszót!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RegistrationButton_Click(object sender, RoutedEventArgs e)
        {
            Registration registrationWindow = new Registration();
            registrationWindow.Show();
            this.Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
