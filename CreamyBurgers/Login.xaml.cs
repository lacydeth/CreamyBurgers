using MySql.Data.MySqlClient;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Windows;
using EasyEncryption;

namespace CreamyBurgers
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        readonly private string conn = "server=localhost;port=3306;database=creamyburgers;user=root;password=";

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = tbUsername.Text.Trim();
            string password = EasyEncryption.MD5.ComputeMD5Hash(tbPassword.Password);


            if (username != "" && password != "")
            {
                try
                {
                    MySqlConnection sqlConn = new MySqlConnection(conn);
                    using (sqlConn)
                    {
                        sqlConn.Open();
                        string query = "SELECT username, password FROM users WHERE username=@username AND password=@password";

                        MySqlDataAdapter adapter = new MySqlDataAdapter(query, sqlConn);
                        adapter.SelectCommand.Parameters.AddWithValue("@username", username);
                        adapter.SelectCommand.Parameters.AddWithValue("@password", password);

                        DataTable table = new DataTable();
                        adapter.Fill(table);

                        if (table.Rows.Count > 0)
                        {
                            sqlConn.Close();
                            LoadingBeetweenWindows loadingWindow = new LoadingBeetweenWindows();
                            loadingWindow.Show();
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Hibás adatok!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                        }

                    }
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message);
                }
            }
            else
            {
                MessageBox.Show("Adj meg felhasználónevet és jelszót!", "Hiba", MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }

            

        private void RegistrationButton_Click(object sender, RoutedEventArgs e)
        {
            Registration registrationWindow = new Registration();
            registrationWindow.Show();
            this.Close(); 
        }

    }
}
