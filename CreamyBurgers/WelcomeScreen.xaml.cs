using System;
using System.Windows;
using System.Windows.Threading;

namespace CreamyBurgers
{
    public partial class WelcomeScreen : Window
    {
        private DispatcherTimer textTimer;
        private DispatcherTimer delayTimer;
        private string welcomeMessage = "Üdvözöljük a CreamyBurgers világában!";
        private string descriptionMessage = "Fedezd fel a legjobb ízeket, és rendelj most!";
        private int welcomeIndex = 0;
        private int descriptionIndex = 0;
        private bool isWelcomeComplete = false;

        public WelcomeScreen()
        {
            InitializeComponent();
            StartTextAnimation();
        }

        private void StartTextAnimation()
        {
            textTimer = new DispatcherTimer();
            textTimer.Interval = TimeSpan.FromMilliseconds(30);
            textTimer.Tick += TextTimer_Tick;
            textTimer.Start();
        }

        private void TextTimer_Tick(object sender, EventArgs e)
        {
            if (!isWelcomeComplete)
            {
                if (welcomeIndex < welcomeMessage.Length)
                {
                    WelcomeTextBlock.Text += welcomeMessage[welcomeIndex];
                    welcomeIndex++;
                }
                else
                {
                    isWelcomeComplete = true;
                    textTimer.Interval = TimeSpan.FromMilliseconds(30);
                }
            }
            else
            {
                if (descriptionIndex < descriptionMessage.Length)
                {
                    DescriptionTextBlock.Text += descriptionMessage[descriptionIndex];
                    descriptionIndex++;
                }
                else
                {
                    textTimer.Stop();
                    StartDelayTimer();
                }
            }
        }

        private void StartDelayTimer()
        {
            delayTimer = new DispatcherTimer();
            delayTimer.Interval = TimeSpan.FromSeconds(1); 
            delayTimer.Tick += DelayTimer_Tick;
            delayTimer.Start();
        }

        private void DelayTimer_Tick(object sender, EventArgs e)
        {
            delayTimer.Stop();
            OpenLoginWindow();
        }

        private void OpenLoginWindow()
        {
            MainWindow loginWindow = new MainWindow();
            loginWindow.Show();
            this.Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
