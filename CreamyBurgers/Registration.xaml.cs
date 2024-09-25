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
                if (!IsRegistrationValid())
                    return;

                RegisterUser(tbUsername.Text, tbPassword.Password, tbEmail.Text);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void RegisterUser(string username, string password, string email)
        {
            using (var sqlConn = new SqliteConnection(conn))
            {
                sqlConn.Open();

                string query = "INSERT INTO users (username, password, email, permID, CreationDate) VALUES (@username, @password, @email, @permID, @creationdate)";

                using (var sqlCommand = new SqliteCommand(query, sqlConn))
                {
                    sqlCommand.Parameters.AddWithValue("@username", username.ToLower());
                    sqlCommand.Parameters.AddWithValue("@password", EasyEncryption.MD5.ComputeMD5Hash(password));
                    sqlCommand.Parameters.AddWithValue("@email", email);
                    sqlCommand.Parameters.AddWithValue("@permID", 1);
                    sqlCommand.Parameters.AddWithValue("@creationdate", DateTime.Now);

                    int status = sqlCommand.ExecuteNonQuery();

                    if (status > 0)
                    {
                        MessageBox.Show("Sikeres regisztráció!", "Regisztráció", MessageBoxButton.OK, MessageBoxImage.Information);
                        MainWindow mainWindow = new MainWindow();
                        mainWindow.Show();
                        this.Close();
                    }
                }
            }
        }

        private bool IsRegistrationValid()
        {
            if (!IsValidUsername(tbUsername.Text))
                return false;

            if (!IsValidPassword(tbPassword.Password))
                return false;
            if (!IsValidEmail(tbEmail.Text))
                return false;

            return true;
        }

        private bool IsValidUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                ShowErrorMessage("A felhasználónév nem lehet üres!");
                return false;
            }

            if (username.Contains(" "))
            {
                ShowErrorMessage("A felhasználónév nem tarthalmazhat szóközt!");
                return false;
            }

            return true;
        }

        private bool IsValidPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                ShowErrorMessage("A jelszó nem lehet üres!");
                return false;
            }

            if (password.Length < 6)
            {
                ShowErrorMessage("A jelszónak legalább 6 karakter hosszúnak kell lennie!");
                return false;
            }

            return true;
        }
        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                ShowErrorMessage("Az email nem lehet üres!");
                return false;
            }
            if (!email.Contains("@"))
            {
                ShowErrorMessage("Érvénytelen email cím!");
                return false;
            }
            return true;
        }

        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
        }


        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
