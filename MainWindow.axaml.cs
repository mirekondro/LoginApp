using Avalonia.Controls;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace LoginApp.Views
{
    public partial class MainWindow : Window
    {
        private Dictionary<string, string> users = new Dictionary<string, string>();
        private const string DataFile = "users.json";
        private string currentLang = "cs";

        private readonly Dictionary<string, Dictionary<string, string>> lang = new()
        {
            ["cs"] = new()
            {
                ["Language"] = "Jazyk:",
                ["LoginUser"] = "Uživatelské jméno:",
                ["LoginPass"] = "Heslo:",
                ["Login"] = "Přihlášení",
                ["Register"] = "Registrace",
                ["FirstName"] = "Jméno:",
                ["LastName"] = "Příjmení:",
                ["Email"] = "Email:",
                ["RegPass"] = "Heslo:",
                ["DoRegister"] = "Dokončit registraci",
                ["Back"] = "Zpět",
                ["FillAll"] = "Vyplňte všechna pole!",
                ["EmailUsed"] = "Tento email je již registrován.",
                ["BadPass"] = "Heslo musí mít min. 8 znaků a číslo.",
                ["RegSuccess"] = "Registrace úspěšná! Nyní se můžete přihlásit.",
                ["LoginBad"] = "Neplatné jméno nebo heslo.",
                ["LoginOk"] = "Přihlášení úspěšné!"
            },
            ["en"] = new()
            {
                ["Language"] = "Language:",
                ["LoginUser"] = "Username:",
                ["LoginPass"] = "Password:",
                ["Login"] = "Login",
                ["Register"] = "Register",
                ["FirstName"] = "First name:",
                ["LastName"] = "Last name:",
                ["Email"] = "Email:",
                ["RegPass"] = "Password:",
                ["DoRegister"] = "Complete registration",
                ["Back"] = "Back",
                ["FillAll"] = "Fill in all fields!",
                ["EmailUsed"] = "This email is already registered.",
                ["BadPass"] = "Password must be at least 8 characters and contain a number.",
                ["RegSuccess"] = "Registration successful! You can now log in.",
                ["LoginBad"] = "Invalid username or password.",
                ["LoginOk"] = "Login successful!"
            }
        };

        public MainWindow()
        {
            InitializeComponent();
            LoadUsers();

            LoginButton.Click += OnLoginClicked;
            ShowRegisterButton.Click += (s, e) => ShowRegisterForm(true);
            RegisterButton.Click += OnRegisterClicked;
            BackToLoginButton.Click += (s, e) => ShowRegisterForm(false);
            LanguageCombo.SelectionChanged += OnLanguageChanged;

            LanguageCombo.SelectedIndex = 0; // default čeština
            ApplyLanguage();
        }

        private void OnLanguageChanged(object? sender, SelectionChangedEventArgs e)
        {
            currentLang = LanguageCombo.SelectedIndex == 0 ? "cs" : "en";
            ApplyLanguage();
        }

        private void ApplyLanguage()
        {
            LanguageLabel.Text = lang[currentLang]["Language"];
            LoginUserLabel.Text = lang[currentLang]["LoginUser"];
            LoginPassLabel.Text = lang[currentLang]["LoginPass"];
            LoginButton.Content = lang[currentLang]["Login"];
            ShowRegisterButton.Content = lang[currentLang]["Register"];
            FirstNameLabel.Text = lang[currentLang]["FirstName"];
            LastNameLabel.Text = lang[currentLang]["LastName"];
            EmailLabel.Text = lang[currentLang]["Email"];
            RegPassLabel.Text = lang[currentLang]["RegPass"];
            RegisterButton.Content = lang[currentLang]["DoRegister"];
            BackToLoginButton.Content = lang[currentLang]["Back"];
        }

        private void ShowRegisterForm(bool showRegister)
        {
            LoginPanel.IsVisible = !showRegister;
            RegisterPanel.IsVisible = showRegister;
            StatusText.Text = "";
        }

        private void OnLoginClicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            string email = LoginUsernameBox.Text ?? "";
            string password = LoginPasswordBox.Text ?? "";

            if (users.ContainsKey(email) && users[email] == HashPassword(password))
            {
                ShowStatus(lang[currentLang]["LoginOk"], Brushes.Green);
                var welcome = new WelcomeWindow(email, currentLang);
                welcome.Show();
                this.Close();
            }
            else
            {
                ShowStatus(lang[currentLang]["LoginBad"], Brushes.Red);
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
                ShowStatus(lang[currentLang]["FillAll"], Brushes.Red);
                return;
            }

            if (users.ContainsKey(email))
            {
                ShowStatus(lang[currentLang]["EmailUsed"], Brushes.Red);
                return;
            }

            if (!ValidatePassword(password))
            {
                ShowStatus(lang[currentLang]["BadPass"], Brushes.Red);
                return;
            }

            users[email] = HashPassword(password);
            SaveUsers();
            ShowStatus(lang[currentLang]["RegSuccess"], Brushes.Green);
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

        private void LoadUsers()
        {
            if (File.Exists(DataFile))
            {
                string json = File.ReadAllText(DataFile);
                users = JsonConvert.DeserializeObject<Dictionary<string, string>>(json)
                        ?? new Dictionary<string, string>();
            }
        }

        private void SaveUsers()
        {
            string json = JsonConvert.SerializeObject(users, Formatting.Indented);
            File.WriteAllText(DataFile, json);
        }
    }
}
