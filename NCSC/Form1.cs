using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace NCSC
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            show_password.CheckedChanged += ShowPassword_CheckedChanged;
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void ShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            TogglePasswordVisibility();
        }

        private void TogglePasswordVisibility()
        {
            if (show_password.Checked)
            {
                // Show password - remove password character
                passwordField.UseSystemPasswordChar = false;
                passwordField.PasswordChar = '\0';
            }
            else
            {
                // Hide password - use system password character
                passwordField.UseSystemPasswordChar = true;
                passwordField.PasswordChar = '*';
            }
        }

        private void label6_Click(object sender, EventArgs e)
        {
            RegisterAccount registerForm = new RegisterAccount();
            registerForm.Show();
            this.Hide();
        }

        private void label3_Click(object sender, EventArgs e) { }

        private void label2_Click(object sender, EventArgs e) { }

        private async void loginButton_Click(object sender, EventArgs e)
        {
            await ValidateLoginAsync();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter)
            {
                _ = ValidateLoginAsync(); // ENTER triggers login
                return true;
            }
            else if (keyData == (Keys.Control | Keys.R))
            {
                RegisterAccount registerForm = new RegisterAccount();
                registerForm.Show();
                this.Hide(); // CTRL + R triggers register form
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private async Task ValidateLoginAsync()
        {
            string enteredUsername = usernameField.Text.Trim();
            string enteredPassword = passwordField.Text;

            if (string.IsNullOrWhiteSpace(enteredUsername) || string.IsNullOrWhiteSpace(enteredPassword))
            {
                MessageBox.Show("Please enter both username and password.");
                return;
            }

            string rawData = await FirebaseHelper.GetDataAsync("accounts");

            if (string.IsNullOrWhiteSpace(rawData) || rawData == "null")
            {
                MessageBox.Show("No accounts found.");
                return;
            }

            try
            {
                var users = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(rawData);

                foreach (var user in users.Values)
                {
                    string storedUsername = user.username;
                    string encryptedPassword = user.password;

                    string decryptedPassword = EncryptionHelper.Decrypt(encryptedPassword);

                    if (storedUsername.Equals(enteredUsername, StringComparison.OrdinalIgnoreCase)
                        && decryptedPassword == enteredPassword)
                    {
                        string userRole = user.role != null ? user.role.ToString() : "user";
                        Dashboard dashboard = new Dashboard(userRole);
                        dashboard.Show();
                        this.Hide();
                        return;
                    }
                }

                MessageBox.Show("Invalid username or password.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Login error: {ex.Message}");
            }
        }
    }
}
