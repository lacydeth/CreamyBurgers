using System;
using System.Threading.Tasks;
using System.Windows;

namespace CreamyBurgers
{
    public partial class LoadingBeetweenWindows : Window
    {
        private string[] statusMessages = new string[]
        {
            "Rendelési előzmények betöltése...",
            "Új frissítések keresése...",
            "Biztonsági ellenőrzés végrehajtása...",
            "Kérjük, ellenőrizze a hálózati kapcsolatot...",
            "Adatbázis kapcsolat létrehozása...",
            "Rendelési információk előkészítése...",
            "Új funkciók aktiválása..."
        };

        public LoadingBeetweenWindows()
        {
            InitializeComponent();
            Loaded += LoadingBetweenWindows_Loaded;
        }

        private async void LoadingBetweenWindows_Loaded(object sender, RoutedEventArgs e)
        {
            await ShowLoadingMessages();
            OpenUserWindow();
        }

        private async Task ShowLoadingMessages()
        {
           
            Random random = new Random();
            int randomIndex = random.Next(statusMessages.Length);
            lblStatus.Text = statusMessages[randomIndex];
            await Task.Delay(1000);

            lblStatus.Text = "Adatok szinkronizálása...";
            await Task.Delay(1000);

            lblStatus.Text = "Felhasználói adatok betöltése...";
            await Task.Delay(1000);
        }

        private void OpenUserWindow()
        {
            User userWindow = new User();
            userWindow.Show();
            this.Close();
        }
    }
}
