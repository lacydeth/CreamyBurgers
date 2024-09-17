using System;
using System.Threading.Tasks;
using System.Windows;

namespace CreamyBurgers
{
    public partial class LoadingWindow : Window
    {
        MainWindow newwindow = new MainWindow();
        private string fullText = "Üdvözöljük a Creamy Burgers-ben!";
        private int currentIndex = 0;

        public LoadingWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await TypeTextAsync();
        }

        private async Task TypeTextAsync()
        {
            while (currentIndex < fullText.Length)
            {
                lblfelirat.Text += fullText[currentIndex];
                currentIndex++;
                await Task.Delay(50);
            }
            await Task.Delay(1500);
            this.Close();
            newwindow.Show();
        }
    }
}
