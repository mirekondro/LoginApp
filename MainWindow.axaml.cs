using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Avalonia.Media;

namespace LoginApp.Views
{
    public partial class MainWindow : Window
    {
        private Dictionary<string, string> users = new Dictionary<string, string>();

        public MainWindow()
        {
            InitializeComponent();

            RegisterButton.Click += OnRegisterClicked;
            LoginButton.Click += OnLoginClicked;
        }

        private void OnRegisterClicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            string username = UsernameBox.Text ?? "";
            string password = PasswordBox.Text ?? "";

            if (users.ContainsKey(username))
            {
                ShowStatus("Uživatel již existuje.", Brushes.Red);
                return;
            }

            if (!ValidatePassword(password))
            {
                ShowStatus("Heslo musí mít min. 8 znaků a číslo.", Brushes.Red);
                return;
            }

            users[username] = HashPassword(password);
            ShowStatus("Registrace úspěšná!", Brushes.Green);
        }

        private void OnLoginClicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            string username = UsernameBox.Text ?? "";
            string password = PasswordBox.Text ?? "";

            if (users.ContainsKey(username) && users[username] == HashPassword(password))
            {
                ShowStatus("Přihlášení úspěšné!", Brushes.Green);
            }
            else
            {
                ShowStatus("Neplatné jméno nebo heslo.", Brushes.Red);
            }
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
