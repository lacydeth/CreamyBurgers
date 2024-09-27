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
        public List<(string Name, int Price)> CartItems = new List<(string Name, int Price)>();
        public User()
        {
            InitializeComponent();
            MainPanel(true);
            LoadProducts();
            LoadData();
            LoadUserOrders();
            OrdersPanel(false);
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
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
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
            if (CartItems.Count == 0)
            {
                MessageBox.Show("A kosarad üres!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string conn = "Data Source=creamyburgers.db";
            try
            {
                using (var sqlConn = new SqliteConnection(conn))
                {
                    sqlConn.Open();

                    using (var transaction = sqlConn.BeginTransaction())
                    {
                        string insertOrderQuery = @"
                                                    INSERT INTO orders (userId, orderDate, totalAmount) 
                                                    VALUES (@userId, @orderDate, @totalAmount);
                                                    SELECT last_insert_rowid()";

                        long orderId;
                        using (var orderCmd = new SqliteCommand(insertOrderQuery, sqlConn, transaction))
                        {
                            orderCmd.Parameters.AddWithValue("@userId", Session.UserId);
                            orderCmd.Parameters.AddWithValue("@orderDate", DateTime.Now);
                            orderCmd.Parameters.AddWithValue("@totalAmount", totalAmount);

                            orderId = (long)orderCmd.ExecuteScalar();
                            if (orderId <= 0)
                            {
                                MessageBox.Show($"Hiba a rendelés leadása során!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }
                        }

                        foreach (var cartItem in CartItems)
                        {
                            int productId = GetProductId(cartItem.Name);
                            if (productId == 0)
                            {
                                MessageBox.Show($"Termék megtalálható: {cartItem.Name}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                                continue;
                            }

                            var requiredIngredients = GetProductIngredients(productId, sqlConn, transaction);

                            if (!DeductInventoryStock(requiredIngredients, sqlConn, transaction))
                            {
                                MessageBox.Show($"Nincs megfelelő mennyiség készletén a termékhez: {cartItem.Name}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                                transaction.Rollback();
                                return;
                            }

                            int unitPrice = cartItem.Price;
                            int quantity = 1;

                            string insertOrderItemQuery = @"
                                                            INSERT INTO orderItems (orderId, productId, quantity, unitPrice, subtotal) 
                                                            VALUES (@orderId, @productId, @quantity, @unitPrice, @subtotal)";

                            using (var orderItemCmd = new SqliteCommand(insertOrderItemQuery, sqlConn, transaction))
                            {
                                orderItemCmd.Parameters.AddWithValue("@orderId", orderId);
                                orderItemCmd.Parameters.AddWithValue("@productId", productId);
                                orderItemCmd.Parameters.AddWithValue("@quantity", quantity);
                                orderItemCmd.Parameters.AddWithValue("@unitPrice", unitPrice);
                                orderItemCmd.Parameters.AddWithValue("@subtotal", unitPrice * quantity);

                                orderItemCmd.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();

                        CartItems.Clear();
                        UpdateCartDisplay();
                        totalAmount = 0;
                        TotalPriceText.Text = "0 Ft";
                        LoadUserOrders();
                        OrdersPanel(false);
                        MessageBox.Show("Sikeresen leadta rendelését!", "Rendelés", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private List<(int inventoryId, int quantityNeeded)> GetProductIngredients(int productId, SqliteConnection sqlConn, SqliteTransaction transaction)
        {
            List<(int inventoryId, int quantityNeeded)> ingredients = new List<(int inventoryId, int quantityNeeded)>();

            string query = "SELECT inventoryId, quantityNeeded FROM productInventory WHERE productId = @productId";
            using (var command = new SqliteCommand(query, sqlConn, transaction))
            {
                command.Parameters.AddWithValue("@productId", productId);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int inventoryId = reader.GetInt32(0);
                        int quantityNeeded = reader.GetInt32(1);
                        ingredients.Add((inventoryId, quantityNeeded));
                    }
                }
            }

            return ingredients;
        }
        private bool DeductInventoryStock(List<(int inventoryId, int quantityNeeded)> ingredients, SqliteConnection sqlConn, SqliteTransaction transaction)
        {
            foreach (var ingredient in ingredients)
            {
                int inventoryId = ingredient.inventoryId;
                int quantityNeeded = ingredient.quantityNeeded;

                string checkStockQuery = "SELECT quantity FROM inventory WHERE id = @inventoryId";
                using (var checkCmd = new SqliteCommand(checkStockQuery, sqlConn, transaction))
                {
                    checkCmd.Parameters.AddWithValue("@inventoryId", inventoryId);
                    int currentStock = Convert.ToInt32(checkCmd.ExecuteScalar());

                    if (currentStock < quantityNeeded)
                    {
                        return false;
                    }

                    string updateStockQuery = "UPDATE inventory SET quantity = quantity - @quantityNeeded WHERE id = @inventoryId";
                    using (var updateCmd = new SqliteCommand(updateStockQuery, sqlConn, transaction))
                    {
                        updateCmd.Parameters.AddWithValue("@quantityNeeded", quantityNeeded);
                        updateCmd.Parameters.AddWithValue("@inventoryId", inventoryId);
                        updateCmd.ExecuteNonQuery();
                    }
                }
            }

            return true; 
        }


        private int GetProductId(string productName)
        {
            string connectionString = "Data Source=creamyburgers.db";
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT productId FROM products WHERE name = @name";
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
                        throw new Exception($"Product ID nem található: {productName}");
                    }
                }
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
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
                    MessageBox.Show($"Adatait sikeresen frissítette!", "Profil", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
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
        private void LoadUserOrders()
        {
            PaidOffersPanel.Children.Clear();

            string conn = "Data Source=creamyburgers.db";
            try
            {
                using (var sqlConn = new SqliteConnection(conn))
                {
                    sqlConn.Open();
                    string fetchOrdersQuery = @"
                                                SELECT orderId, orderDate, totalAmount 
                                                FROM orders 
                                                WHERE userId = @userId
                                                ORDER BY orderDate DESC";

                    using (var cmd = new SqliteCommand(fetchOrdersQuery, sqlConn))
                    {
                        cmd.Parameters.AddWithValue("@userId", Session.UserId);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (!reader.HasRows)
                            {
                                PaidOffers.Visibility = Visibility.Collapsed;
                                return;
                            }

                            PaidOffers.Visibility = Visibility.Visible;

                            while (reader.Read())
                            {
                                long orderId = reader.GetInt64(0);
                                DateTime orderDate = reader.GetDateTime(1);
                                int totalAmount = reader.GetInt32(2);

                                Border orderCard = new Border
                                {
                                    Background = new SolidColorBrush(Colors.LightGray),
                                    CornerRadius = new CornerRadius(10),
                                    Margin = new Thickness(5),
                                    Padding = new Thickness(10),
                                    BorderThickness = new Thickness(1),
                                    BorderBrush = new SolidColorBrush(Colors.DarkGray)
                                };

                                Grid orderGrid = new Grid();
                                orderGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                                orderGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                                orderGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                                TextBlock orderInfo = new TextBlock
                                {
                                    Text = $"Rendelés azonosító: {orderId} | Dátum: {orderDate.ToString("g")}",
                                    FontSize = 14,
                                    FontWeight = FontWeights.Bold
                                };
                                Grid.SetRow(orderInfo, 0);
                                orderGrid.Children.Add(orderInfo);

                                TextBlock totalAmountText = new TextBlock
                                {
                                    Text = $"Összesen: {totalAmount} Ft",
                                    FontSize = 12,
                                    FontWeight = FontWeights.Normal
                                };
                                Grid.SetRow(totalAmountText, 1);
                                orderGrid.Children.Add(totalAmountText);
                                orderCard.Child = orderGrid;
                                PaidOffersPanel.Children.Add(orderCard);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
