using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CreamyBurgers
{
    public partial class User : Window
    {
        private double totalAmount = 0;
        private List<(string Name, int Price)> CartItems = new List<(string Name, int Price)>();

        public User()
        {
            InitializeComponent();
            MainPanel(true);
            LoadProducts();
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
        private void LoadProducts()
        {
            string connectionString = "Data Source=creamyburgers.db";

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT name, unitPrice, description FROM products";

                using (var command = new SqliteCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string productName = reader.GetString(0);
                            int productPrice = reader.GetInt32(1);
                            string productDescription = reader.GetString(2);
                            CreateProductCard(productName, productPrice, productDescription);
                        }
                    }
                }
            }
        }
        private void CreateProductCard(string name, int price, string description)
        {
            Border border = new Border
            {
                Style = (Style)FindResource("CardStyle"),
                Margin = new Thickness(10),
                BorderBrush = System.Windows.Media.Brushes.Black,
                BorderThickness = new Thickness(1)
            };
            StackPanel stackPanel = new StackPanel();

            Border imagePlaceholder = new Border
            {
                Height = 150,
                Width = 150,
                BorderBrush = System.Windows.Media.Brushes.Black,
                BorderThickness = new Thickness(1),
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(20)
            };
            stackPanel.Children.Add(imagePlaceholder);

            TextBlock productNameText = new TextBlock
            {
                Text = name,
                FontWeight = FontWeights.Bold,
                FontSize = 16,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            stackPanel.Children.Add(productNameText);

            TextBlock productPriceText = new TextBlock
            {
                Text = price + " Ft",
                FontWeight = FontWeights.Bold,
                FontSize = 16,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            stackPanel.Children.Add(productPriceText);

            TextBlock productDescriptionText = new TextBlock
            {
                Text = "Hozzávalók: " + description,
                TextWrapping = TextWrapping.Wrap,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            stackPanel.Children.Add(productDescriptionText);

            StackPanel buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(20)
            };
            Button minusButton = new Button
            {
                Style = (Style)FindResource("MinusStyle"),
                Tag = name
            };
            minusButton.Click += MinusButton_Click;

            Button plusButton = new Button
            {
                Style = (Style)FindResource("PlusStyle"),
                Tag = name
            };
            plusButton.Click += AddButton_Click;

            TextBlock cartText = new TextBlock
            {
                Text = "Kosárhoz",
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(10, 0, 10, 0)
            };
            buttonPanel.Children.Add(minusButton);
            buttonPanel.Children.Add(cartText);
            buttonPanel.Children.Add(plusButton);
            stackPanel.Children.Add(buttonPanel);
            border.Child = stackPanel;
            ProductsContainer.Children.Add(border);
        }
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            string productName = button.Tag.ToString();
            int productPrice = GetProductPrice(productName);

            AddToCart(productName, productPrice);
            totalAmount += productPrice;
            TotalPriceText.Text = $"{totalAmount} Ft";
            UpdateCartDisplay();
        }
        private void MinusButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            string productName = button.Tag.ToString();

            if (totalAmount > 0)
            {
                int productPrice = GetProductPrice(productName);
                bool removed = RemoveFromCart(productName, productPrice);

                if (removed)
                {
                    totalAmount -= productPrice;
                    TotalPriceText.Text = $"{totalAmount} Ft";
                    UpdateCartDisplay();
                }
            }
        }
        private void PayButton_Click(object sender, RoutedEventArgs e)
        {
            AddTotalAmountToPaidOffers();
            CartItems.Clear();
            UpdateCartDisplay();
            totalAmount = 0;
            TotalPriceText.Text = "0 Ft";
            MessageBox.Show("Sikeres Fizetés, Rendelésem fülön láthatja a leadott rendelését");
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
            MainPanel(true);
            ProfilePanel(false);
            OrdersPanel(false);
        }
        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            MainPanel(false);
            ProfilePanel(true);
            OrdersPanel(false);
        }
        private void OrdersButton_Click(object sender, RoutedEventArgs e)
        {
            MainPanel(false);
            ProfilePanel(false);
            OrdersPanel(true);
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
                        using (var addressCmd = new SqliteCommand(addressQuery, sqlConn))
                        {
                            addressCmd.Parameters.AddWithValue("@userId", Session.UserId);
                            addressCmd.Parameters.AddWithValue("@street", tbStreet.Text);
                            addressCmd.Parameters.AddWithValue("@city", tbCity.Text);
                            addressCmd.Parameters.AddWithValue("@state", tbState.Text);
                            addressCmd.Parameters.AddWithValue("@zipCode", tbZip.Text);
                            addressCmd.Parameters.AddWithValue("@houseNumber", tbHouseNumber.Text);
                            addressCmd.Parameters.AddWithValue("@phoneNumber", tbPhoneNumber.Text);

                            addressCmd.ExecuteNonQuery();
                        }
                    }
                    MessageBox.Show("Data updated successfully!");
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private int GetProductPrice(string productName)
        {
            string connectionString = "Data Source=creamyburgers.db";
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT unitPrice FROM products WHERE name = @name";
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@name", productName);
                    object result = command.ExecuteScalar();
                    if (result != null)
                    {
                        return Convert.ToInt32(result);
                    }
                    else
                    {
                        MessageBox.Show($"Price not found for {productName}.");
                        return 0;
                    }
                }
            }
        }
        private void AddToCart(string productName, int productPrice)
        {
            CartItems.Add((productName, productPrice));
        }
        private void UpdateCartDisplay()
        {
            CartItemsPanel.Children.Clear();

            foreach (var item in CartItems)
            {
                TextBlock cartItem = new TextBlock
                {
                    Text = $"{item.Name} - {item.Price} Ft",
                    FontSize = 12,
                    Margin = new Thickness(5),
                    HorizontalAlignment = HorizontalAlignment.Left
                };
                CartItemsPanel.Children.Add(cartItem);
            }
        }
        private bool RemoveFromCart(string productName, int productPrice)
        {
            var item = CartItems.FirstOrDefault(i => i.Name == productName && i.Price == productPrice);
            if (item != default)
            {
                CartItems.Remove(item);
                return true;
            }
            return false;
        }

        private void AddTotalAmountToPaidOffers()
        {
            Border newBorder = new Border
            {
                BorderThickness = new Thickness(1),
                BorderBrush = Brushes.Black,
                Height = 400,
                Width = 300,
                Margin = new Thickness(5),
                Padding = new Thickness(10),
                Child = new StackPanel()
            };

            StackPanel newPanel = newBorder.Child as StackPanel;
            foreach (var item in CartItems)
            {
                TextBlock itemText = new TextBlock
                {
                    Text = $"{item.Name}: {item.Price} Ft",
                    FontSize = 14,
                    Margin = new Thickness(5),
                    Foreground = Brushes.Black
                };
                newPanel.Children.Add(itemText);
            }
            TextBlock totalAmountText = new TextBlock
            {
                Text = $"Végösszeg: {totalAmount} Ft",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(5),
                Foreground = Brushes.Black
            };
            newPanel.Children.Add(totalAmountText);
            PaidOffers.Children.Add(newBorder);
        }




        private void MainPanel(bool isVisible)
        {
            CartPanel.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
            ProductsContainer.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        private void ProfilePanel(bool isVisible)
        {
            ProfilePanelContainer.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        private void OrdersPanel(bool isVisible)
        {
            PaidOffers.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
