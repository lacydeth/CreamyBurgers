using Microsoft.Data.Sqlite;
using System.Windows;

namespace CreamyBurgers
{
    public partial class User : Window
    {
        public User()
        {
            InitializeComponent();

            ShowCartPanel(true);
            ProfilePanelContainer.Visibility = Visibility.Collapsed;
            LoadData();
        }
        private void LoadData()
        {
            string conn = "Data Source=creamyburgers.db";

            try
            {
                using (var sqlConn = new SqliteConnection(conn))
                {
                    sqlConn.Open();

                    string query = @"
                                    SELECT u.fname, u.lname, a.street, a.city, a.state, a.zipCode, a.houseNumber, a.phoneNumber
                                    FROM users u
                                    LEFT JOIN addresses a ON u.id = a.userId
                                    WHERE u.id=@userId";

                    using (var sqlCommand = new SqliteCommand(query, sqlConn))
                    {
                        sqlCommand.Parameters.AddWithValue("@userId", Session.UserId);

                        using (SqliteDataReader reader = sqlCommand.ExecuteReader())
                        {
                            if (reader.HasRows && reader.Read())
                            {
                                tbFirstName.Text = reader["fname"].ToString();
                                tbLastName.Text = reader["lname"].ToString();

                                tbStreet.Text = reader["street"].ToString();
                                tbCity.Text = reader["city"].ToString();
                                tbState.Text = reader["state"].ToString();
                                tbZip.Text = reader["zipCode"].ToString();
                                tbHouseNumber.Text = reader["houseNumber"].ToString();
                                tbPhoneNumber.Text = reader["phoneNumber"].ToString();
                            }
                            else
                            {
                                tbFirstName.Text = "";
                                tbLastName.Text = "";

                                tbStreet.Text = "";
                                tbCity.Text = "";
                                tbState.Text = "";
                                tbZip.Text = "";
                                tbHouseNumber.Text = "";
                                tbPhoneNumber.Text = "";
                            }
                        }
                    }
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
            string conn = "Data Source=creamyburgers.db";

            try
            {
                using (var sqlConn = new SqliteConnection(conn))
                {
                    sqlConn.Open();

                    string userQuery = @"UPDATE users 
                                 SET fname=@fname, lname=@lname
                                 WHERE id=@userId";

                    using (var userCmd = new SqliteCommand(userQuery, sqlConn))
                    {
                        userCmd.Parameters.AddWithValue("@fname", tbFirstName.Text);
                        userCmd.Parameters.AddWithValue("@lname", tbLastName.Text);
                        userCmd.Parameters.AddWithValue("@userId", Session.UserId);

                        userCmd.ExecuteNonQuery();
                    }

                    string checkQuery = "SELECT COUNT(*) FROM addresses WHERE userId=@userId";

                    using (var checkCmd = new SqliteCommand(checkQuery, sqlConn))
                    {
                        checkCmd.Parameters.AddWithValue("@userId", Session.UserId);
                        int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                        string addressQuery;
                        if (count > 0)
                        {
                            addressQuery = @"UPDATE addresses 
                                                SET street=@street, city=@city, state=@state, zipCode=@zipCode, houseNumber=@houseNumber, phoneNumber=@phoneNumber
                                                WHERE userId=@userId";
                        }
                        else
                        {
                            addressQuery = @"INSERT INTO addresses (userId, street, city, state, zipCode, houseNumber, phoneNumber) 
                                                VALUES (@userId, @street, @city, @state, @zipCode, @houseNumber, @phoneNumber)";
                        }

                        using (var sqlCommand = new SqliteCommand(addressQuery, sqlConn))
                        {
                            sqlCommand.Parameters.AddWithValue("@userId", Session.UserId);
                            sqlCommand.Parameters.AddWithValue("@street", tbStreet.Text);
                            sqlCommand.Parameters.AddWithValue("@city", tbCity.Text);
                            sqlCommand.Parameters.AddWithValue("@state", tbState.Text);
                            sqlCommand.Parameters.AddWithValue("@zipCode", tbZip.Text);
                            sqlCommand.Parameters.AddWithValue("@houseNumber", tbHouseNumber.Text);
                            sqlCommand.Parameters.AddWithValue("@phoneNumber", tbPhoneNumber.Text);

                            sqlCommand.ExecuteNonQuery();
                            MessageBox.Show("Adatai sikeresen mentésre kerültek!", "Mentés", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private void ShowCartPanel(bool isVisible)
        {
            CartPanel.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
