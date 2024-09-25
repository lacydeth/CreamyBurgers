using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Data.Sqlite;
namespace CreamyBurgers
{
    public partial class Admin : Window
    {
        private InventoryItem selectedInventoryItem;
        public Admin()
        {
            InitializeComponent();
            LoadInventory();
            LoadOrders();
        }
        private void Logout_buttonClick(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
                CreateOrderPanel.Visibility = Visibility.Visible;
                OrderStackPanel.Visibility = Visibility.Collapsed;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("mentve");
        }

        private void LoadOrders()
        {
            string connString = "Data Source=creamyburgers.db";
            List<Order> orders = new List<Order>();

            try
            {
                using (var conn = new SqliteConnection(connString))
                {
                    conn.Open();
                    string query = @"SELECT orderId, userId, orderDate, totalAmount FROM orders";

                    using (var cmd = new SqliteCommand(query, conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                orders.Add(new Order
                                {
                                    OrderId = reader.GetInt32(0),
                                    UserId = reader.GetInt32(1),
                                    OrderDate = reader.GetDateTime(2),
                                    TotalAmount = reader.GetInt32(3)
                                });
                            }
                        }
                    }
                }

                OrdersDataGrid.ItemsSource = orders;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading orders: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadInventory()
        {
            string connString = "Data Source=creamyburgers.db";
            List<InventoryItem> inventoryItems = new List<InventoryItem>();

            try
            {
                using (var conn = new SqliteConnection(connString))
                {
                    conn.Open();
                    string query = @"SELECT id, name, quantity FROM inventory";

                    using (var cmd = new SqliteCommand(query, conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                inventoryItems.Add(new InventoryItem
                                {
                                    InventoryId = reader.GetInt32(0),
                                    Name = reader.GetString(1),
                                    Quantity = reader.GetInt32(2)
                                });
                            }
                        }
                    }
                }

                InventoryDataGrid.ItemsSource = inventoryItems;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading inventory: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CreateOrderPanel.Visibility = Visibility.Visible;
            OrderStackPanel.Visibility = Visibility.Collapsed;
        }
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string name = NameTextBox.Text;
            if (int.TryParse(QuantityTextBox.Text, out int quantity))
            {
                string connString = "Data Source=creamyburgers.db";
                try
                {
                    using (var conn = new SqliteConnection(connString))
                    {
                        conn.Open();
                        string query = @"INSERT INTO inventory (name, quantity) VALUES (@name, @quantity)";
                        using (var cmd = new SqliteCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@name", name);
                            cmd.Parameters.AddWithValue("@quantity", quantity);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    LoadInventory(); // Refresh inventory after adding
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error adding inventory: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Invalid quantity. Please enter a numeric value.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void InventoryDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (InventoryDataGrid.SelectedItem is InventoryItem item)
            {
                selectedInventoryItem = item;
                NameTextBox.Text = item.Name;
                QuantityTextBox.Text = item.Quantity.ToString();
                UpdateButton.IsEnabled = true;
                DeleteButton.IsEnabled = true;
            }
            else
            {
                selectedInventoryItem = null;
                NameTextBox.Clear();
                QuantityTextBox.Clear();
                UpdateButton.IsEnabled = false;
                DeleteButton.IsEnabled = false;
            }
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedInventoryItem != null && int.TryParse(QuantityTextBox.Text, out int quantity))
            {
                string connString = "Data Source=creamyburgers.db";
                try
                {
                    using (var conn = new SqliteConnection(connString))
                    {
                        conn.Open();
                        string query = @"UPDATE inventory SET name = @name, quantity = @quantity WHERE id = @id";
                        using (var cmd = new SqliteCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@name", NameTextBox.Text);
                            cmd.Parameters.AddWithValue("@quantity", quantity);
                            cmd.Parameters.AddWithValue("@id", selectedInventoryItem.InventoryId);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    LoadInventory(); // Refresh inventory after updating
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating inventory: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Invalid input. Please ensure all fields are filled correctly.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedInventoryItem != null)
            {
                string connString = "Data Source=creamyburgers.db";
                try
                {
                    using (var conn = new SqliteConnection(connString))
                    {
                        conn.Open();
                        string query = @"DELETE FROM inventory WHERE id = @id";
                        using (var cmd = new SqliteCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@id", selectedInventoryItem.InventoryId);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    LoadInventory(); // Refresh inventory after deletion
                    NameTextBox.Clear();
                    QuantityTextBox.Clear();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting inventory: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
    public class Order
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public int TotalAmount { get; set; }
    }
    public class InventoryItem
    {
        public int InventoryId { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
    }
}
