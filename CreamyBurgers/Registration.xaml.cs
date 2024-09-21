using System;
using System.Windows;
using Microsoft.Data.Sqlite;
using EasyEncryption;

namespace CreamyBurgers
{
    public partial class Registration : Window
    {
        public Registration()
        {
            InitializeComponent();
        }

        readonly string conn = "Data Source=creamyburgers.db";

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var sqlConn = new SqliteConnection(conn))
                {
                    sqlConn.Open();
                    string query = "INSERT INTO users (username, password, email, permID, CreationDate) VALUES (@username, @password, @email, @permID, @creationdate)";
                    
                    using (var sqlCommand = new SqliteCommand(query, sqlConn))
                    {
                        sqlCommand.Parameters.AddWithValue("@username", tbUsername.Text.ToLower());
                        sqlCommand.Parameters.AddWithValue("@password", EasyEncryption.MD5.ComputeMD5Hash(tbPassword.Password));
                        sqlCommand.Parameters.AddWithValue("@email", tbEmail.Text);
                        sqlCommand.Parameters.AddWithValue("@permID", 1); 
                        sqlCommand.Parameters.AddWithValue("@creationdate", DateTime.Now);

                        int status = sqlCommand.ExecuteNonQuery();

                        if (status > 0)
                        {
                            MessageBox.Show("Sikeres regisztráció!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                            sqlConn.Close();
                            MainWindow mainWindow = new MainWindow();
                            mainWindow.Show();
                            this.Close();
                        }
                    }
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
