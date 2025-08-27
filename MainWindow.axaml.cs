using Avalonia.Controls;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace LoginApp.Views
{
    public partial class MainWindow : Window
    {
        private Dictionary<string, string> users = new Dictionary<string, string>();

        public MainWindow()
        {
            InitializeComponent();

            LoginButton.Click += OnLoginClicked;
            ShowRegisterButton.Click += (s, e) => ShowRegisterForm(true);

            RegisterButton.Click += OnRegisterClicked;
            BackToLoginButton.Click += (s, e) => ShowRegisterForm(false);
        }

        private void ShowRegisterForm(bool showRegister)
        {
            LoginPanel.IsVisible = !showRegister;
            RegisterPanel.IsVisible = showRegister;
            StatusText.Text = "";
        }

        private void OnLoginClicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            string username = LoginUsernameBox.Text ?? "";
            string password = LoginPasswordBox.Text ?? "";

            if (users.ContainsKey(username) && users[username] == HashPassword(password))
            {
                ShowStatus("Přihlášení úspěšné!", Brushes.Green);
            }
            else
            {
                ShowStatus("Neplatné jméno nebo heslo.", Brushes.Red);
            }
        }

        private void OnRegisterClicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            string firstName = FirstNameBox.Text ?? "";
            string lastName = LastNameBox.Text ?? "";
            string email = EmailBox.Text ?? "";
            string password = RegisterPasswordBox.Text ?? "";

            if (string.IsNullOrWhiteSpace(firstName) ||
                string.IsNullOrWhiteSpace(lastName) ||
                string.IsNullOrWhiteSpace(email))
            {
                ShowStatus("Vyplňte všechna pole!", Brushes.Red);
                return;
            }

            if (users.ContainsKey(email))
            {
                ShowStatus("Tento email je již registrován.", Brushes.Red);
                return;
            }

            if (!ValidatePassword(password))
            {
                ShowStatus("Heslo musí mít min. 8 znaků a číslo.", Brushes.Red);
                return;
            }

            users[email] = HashPassword(password);
            ShowStatus("Registrace úspěšná! Nyní se můžete přihlásit.", Brushes.Green);
            ShowRegisterForm(false);
        }

        private bool ValidatePassword(string password)
        {
            if (password.Length < 8)
                return false;

            foreach (char c in password)
            {
                if (char.IsDigit(c)) return true;
            }
            return false;
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha.ComputeHash(inputBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }

        private void ShowStatus(string message, IBrush color)
        {
            StatusText.Text = message;
            StatusText.Foreground = color;
        }
    }
}
