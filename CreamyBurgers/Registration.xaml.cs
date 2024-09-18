using MySql.Data.MySqlClient;
using System.Windows;

namespace CreamyBurgers
{
    public partial class Registration : Window
    {
        public Registration()
        {
            InitializeComponent();
        }

        readonly private string conn = "server=localhost;port=3306;database=creamyburgers;user=root;password=";
        
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
                MySqlConnection sqlConn = new MySqlConnection(conn);
                using (sqlConn)
                {
                    sqlConn.Open();
                    string query = "INSERT into users (username, password, email, permID, CreationDate) VALUES (@username, @password, @email, @permID, @creationdate)";

                    MySqlCommand sqlCommand = new MySqlCommand(query, sqlConn);
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
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }

        }
    }
}
