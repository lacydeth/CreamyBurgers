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
            LoadProducts();
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

                            // Call a method to dynamically create a card for each product
                            CreateProductCard(productName, productPrice, productDescription);
                        }
                    }
                }
            }
        }

        private void CreateProductCard(string name, int price, string description)
        {
            // Create the outer border for the card
            Border border = new Border
            {
                Style = (Style)FindResource("CardStyle"),
                Margin = new Thickness(20),
                BorderBrush = System.Windows.Media.Brushes.Black,
                BorderThickness = new Thickness(1)
            };

            // Create a StackPanel to hold the card's content
            StackPanel stackPanel = new StackPanel();

            // Create the image placeholder
            Border imagePlaceholder = new Border
            {
                Height = 200,
                Width = 200,
                BorderBrush = System.Windows.Media.Brushes.Black,
                BorderThickness = new Thickness(1),
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(20)
            };
            TextBlock imageText = new TextBlock
            {
                Text = "Kép helye",
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                FontWeight = FontWeights.Bold
            };
            imagePlaceholder.Child = imageText;

            // Add image placeholder to the stack panel
            stackPanel.Children.Add(imagePlaceholder);

            // Create the product name TextBlock
            TextBlock productNameText = new TextBlock
            {
                Text = name,
                FontWeight = FontWeights.Bold,
                FontSize = 16,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            // Add product name to the stack panel
            stackPanel.Children.Add(productNameText);

            // Create the product price TextBlock
            TextBlock productPriceText = new TextBlock
            {
                Text = price + " Ft",
                FontWeight = FontWeights.Bold,
                FontSize = 16,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            // Add product price to the stack panel
            stackPanel.Children.Add(productPriceText);

            // Create the product description TextBlock
            TextBlock productDescriptionText = new TextBlock
            {
                Text = "Hozzávalók: " + description,
                TextWrapping = TextWrapping.Wrap,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            // Add product description to the stack panel
            stackPanel.Children.Add(productDescriptionText);

            // Create the buttons panel
            StackPanel buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(20)
            };

            // Create the minus button
            Button minusButton = new Button
            {
                Style = (Style)FindResource("MinusStyle"),
                Tag = name
            };
            minusButton.Click += MinusButton_Click;

            // Create the plus button
            Button plusButton = new Button
            {
                Style = (Style)FindResource("PlusStyle"),
                Tag = name
            };
            plusButton.Click += AddButton_Click;

            // Create the "Kosárhoz" label
            TextBlock cartText = new TextBlock
            {
                Text = "Kosárhoz",
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(10, 0, 10, 0)
            };

            // Add buttons and label to the button panel
            buttonPanel.Children.Add(minusButton);
            buttonPanel.Children.Add(cartText);
            buttonPanel.Children.Add(plusButton);

            // Add the button panel to the stack panel
            stackPanel.Children.Add(buttonPanel);

            // Set the stack panel as the child of the outer border
            border.Child = stackPanel;

            // Finally, add the card to the parent container (e.g., a UniformGrid or StackPanel in the XAML)
            ProductsContainer.Children.Add(border);  // ProductsContainer is the name of the panel in your XAML
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            string productName = button.Tag.ToString();
            MessageBox.Show($"{productName} added to the cart.");
        }

        private void MinusButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            string productName = button.Tag.ToString();
            MessageBox.Show($"{productName} removed from the cart.");
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
            ProductsContainer.Visibility = Visibility.Visible;
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            ShowCartPanel(false);
            ProfilePanelContainer.Visibility = Visibility.Visible;
            ProductsContainer.Visibility = Visibility.Collapsed;
        }

        private void OrdersButton_Click(object sender, RoutedEventArgs e)
        {
            ShowCartPanel(false); 
            ProfilePanelContainer.Visibility = Visibility.Collapsed;
            ProductsContainer.Visibility = Visibility.Collapsed;
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


            // Hide the OrdersDockPanel until orders are viewed
            OrdersDockPanel.Visibility = Visibility.Collapsed;
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
