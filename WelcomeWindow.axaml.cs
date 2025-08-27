using Avalonia.Controls;

namespace LoginApp.Views
{
    public partial class WelcomeWindow : Window
    {
        public WelcomeWindow(string email)
        {
            InitializeComponent();
            WelcomeText.Text = $"Vítejte, {email}!";
            CloseButton.Click += (s, e) => this.Close();
        }
    }
}
