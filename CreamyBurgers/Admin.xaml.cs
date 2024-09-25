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

namespace CreamyBurgers
{
    /// <summary>
    /// Interaction logic for Admin.xaml
    /// </summary>
    public partial class Admin : Window
    {
        public Admin()
        {
            InitializeComponent();
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

        private void OrdersButton_Click(object sender, RoutedEventArgs e)
        {
            OrderStackPanel.Visibility = Visibility.Visible;
            CreateOrderPanel.Visibility = Visibility.Collapsed;
            string conn = "Data Source=creamyburgers.db";
            OrderStackPanel.Children.Clear();
            try
            {
                using (var sqlConn = new SqliteConnection(conn))
                {
                    sqlConn.Open();
                    string fetchOrdersQuery = @"
                                    SELECT orderId, orderDate, totalAmount, userId 
                                    FROM orders 
                                    ORDER BY orderDate DESC";

                    using (var cmd = new SqliteCommand(fetchOrdersQuery, sqlConn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                long orderId = reader.GetInt64(0);
                                DateTime orderDate = reader.GetDateTime(1);
                                int totalAmount = reader.GetInt32(2);
                                string userId = reader.GetString(3);
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
                                TextBlock userInfo = new TextBlock
                                {
                                    Text = $"Felhasználó ID: {userId}",
                                    FontSize = 12,
                                    FontWeight = FontWeights.Normal
                                };
                                Grid.SetRow(userInfo, 0);
                                orderGrid.Children.Add(userInfo);
                                TextBlock orderInfo = new TextBlock
                                {
                                    Text = $"Rendelés azonosító: {orderId} | Dátum: {orderDate:g}",
                                    FontSize = 14,
                                    FontWeight = FontWeights.Bold
                                };
                                Grid.SetRow(orderInfo, 1);
                                orderGrid.Children.Add(orderInfo);
                                TextBlock totalAmountText = new TextBlock
                                {
                                    Text = $"Összesen: {totalAmount} Ft",
                                    FontSize = 12,
                                    FontWeight = FontWeights.Normal
                                };
                                Grid.SetRow(totalAmountText, 2);
                                orderGrid.Children.Add(totalAmountText);

                                orderCard.Child = orderGrid;
                                OrderStackPanel.Children.Add(orderCard);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


    }
}
