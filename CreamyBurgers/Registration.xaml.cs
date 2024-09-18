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
            CreateUsersTable();
        }

        readonly private string conn = "Data Source=creamyburgers.db";
        private void CreateUsersTable()
        {
            try
            {
                using (var sqlConn = new SqliteConnection(conn))
                {
                    sqlConn.Open();
                    string createTableQuery = @"
                        CREATE TABLE IF NOT EXISTS users (
                            id INTEGER PRIMARY KEY AUTOINCREMENT,
                            username TEXT NOT NULL UNIQUE,
                            password TEXT NOT NULL,
                            email TEXT NOT NULL UNIQUE,
                            permID INTEGER,
                            CreationDate DATETIME DEFAULT CURRENT_TIMESTAMP
                        )";

                    using (var sqlCommand = new SqliteCommand(createTableQuery, sqlConn))
                    {
                        sqlCommand.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba az adatbázis létrehozása során: {ex.Message}");
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Regisztrációs sikeres!");
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
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
                            MessageBox.Show("Sikeres Regisztráció");
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
