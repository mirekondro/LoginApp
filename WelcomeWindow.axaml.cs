using Avalonia.Controls;

namespace LoginApp.Views
{
    public partial class WelcomeWindow : Window
    {
        public WelcomeWindow(string email, string lang)
        {
            InitializeComponent();
            if (lang == "cs")
                WelcomeText.Text = $"Vítejte, {email}!";
            else
                WelcomeText.Text = $"Welcome, {email}!";
            CloseButton.Click += (s, e) => this.Close();
        }
    }
}
