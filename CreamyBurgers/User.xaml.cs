using Microsoft.Data.Sqlite;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CreamyBurgers
{
    public partial class User : Window
    {
        private double totalAmount = 0;

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
            OffersPanel.Visibility = Visibility.Visible;
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            ShowCartPanel(false);
            ProfilePanelContainer.Visibility = Visibility.Visible;
            OffersPanel.Visibility = Visibility.Collapsed;
        }

        private void OrdersButton_Click(object sender, RoutedEventArgs e)
        {
            ShowCartPanel(false); 
            ProfilePanelContainer.Visibility = Visibility.Collapsed; 
            OffersPanel.Visibility = Visibility.Collapsed;
        }

        private void PayButton_Click(object sender, RoutedEventArgs e)
        {
            // Clear previous orders from the main container
            OrdersContainer.Children.Clear();

            // Loop through each item in the cart and add it to the main container
            foreach (StackPanel itemPanel in CartItemsPanel.Children)
            {
                if (itemPanel.Children[0] is TextBlock itemText)
                {
                    // Create the order summary as a Label
                    Label orderItemLabel = new Label
                    {
                        Content = itemText.Text,
                        Margin = new Thickness(5),
                        FontWeight = FontWeights.Bold,
                        FontSize = 12,
                        BorderBrush = Brushes.SaddleBrown,
                        BorderThickness = new Thickness(2),
                        Padding = new Thickness(10)
                    };

                    // Add the order item label to the main container
                    OrdersContainer.Children.Add(orderItemLabel);
                }
            }

            // Add the total price to the main container
            Label totalPriceLabel = new Label
            {
                Content = $"Összesen: {totalAmount} Ft",
                Margin = new Thickness(5),
                FontWeight = FontWeights.Bold,
                FontSize = 14,
                Foreground = Brushes.Red,
                BorderBrush = Brushes.SaddleBrown,
                BorderThickness = new Thickness(2),
                Padding = new Thickness(10)
            };

            // Add the total price to the main container
            OrdersContainer.Children.Add(totalPriceLabel);

            // Clear the cart and reset total amount
            CartItemsPanel.Children.Clear();
            totalAmount = 0;
            UpdateTotalAmount();

            // Hide the OrdersDockPanel until orders are viewed
            OrdersDockPanel.Visibility = Visibility.Collapsed;
        }










        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            string productName = button.Tag.ToString();
               double productPrice = 0;
            switch (productName)
            {
                case "Krémes Klasszikus":
                    productPrice = 1299;
                    break;
                case "Krémes BBQ Burger":
                    productPrice = 1499;
                    break;
                case "Csípős Krémes Burger":
                    productPrice = 1399;
                    break;
                case "Krémes Bacon-Sajt Burger":
                    productPrice = 1499;
                    break;
                case "Krémes Gombás Burger":
                    productPrice = 1599;
                    break;
                case "Krémes Avokádós Burger":
                    productPrice = 1699;
                    break;
            }
            StackPanel itemPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(5) };
            TextBlock itemText = new TextBlock
            {
                Text = $"{productName} - {productPrice} Ft",
                Margin = new Thickness(5),
                FontWeight = FontWeights.Bold,
                FontSize = 10
            };
            itemPanel.Children.Add(itemText);
            CartItemsPanel.Children.Add(itemPanel);
            totalAmount += productPrice;
            UpdateTotalAmount();
        }


        private void UpdateTotalAmount()
        {
            TotalPriceText.Text = $"{totalAmount} Ft";
        }
        private void MinusButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            string productName = button.Tag.ToString();
            double productPrice = 0;

            switch (productName)
            {
                case "Krémes Klasszikus":
                    productPrice = 1299;
                    break;
                case "Krémes BBQ Burger":
                    productPrice = 1499;
                    break;
                case "Csípős Krémes Burger":
                    productPrice = 1399;
                    break;
                case "Krémes Bacon-Sajt Burger":
                    productPrice = 1499;
                    break;
                case "Krémes Gombás Burger":
                    productPrice = 1599;
                    break;
                case "Krémes Avokádós Burger":
                    productPrice = 1699;
                    break;
            }

            for (int i = 0; i < CartItemsPanel.Children.Count; i++)
            {
                StackPanel itemPanel = CartItemsPanel.Children[i] as StackPanel;

                if (itemPanel != null && itemPanel.Children[0] is TextBlock itemText)
                {
                    if (itemText.Text.StartsWith(productName))
                    {
                        CartItemsPanel.Children.RemoveAt(i);
                        break;
                    }
                }
            }

            if (totalAmount == 0)
            {
                return;
            }
            else
            {
                totalAmount -= productPrice;
                UpdateTotalAmount();
            }
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
