using System;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace NCSC
{
    public partial class RegisterAccount : Form
    {
        public RegisterAccount()
        {
            InitializeComponent();
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void login_label_click(object sender, EventArgs e)
        {
            Form1 loginForm = new Form1();
            loginForm.Show();
            this.Hide();
        }

        private async void loginButton_Click(object sender, EventArgs e)
        {
            string username = usernameField.Text.Trim();
            string password = passwordField.Text;
            string confirmPassword = confirm_pass_field.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                MessageBox.Show("Please fill in all fields.");
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Passwords do not match.");
                return;
            }

            // 🔍 Check for existing users
            string existingUsers = await FirebaseHelper.GetDataAsync("accounts");
            if (!string.IsNullOrWhiteSpace(existingUsers) && existingUsers.Contains($"\"username\":\"{username}\""))
            {
                MessageBox.Show("Username already exists. Please choose another one.");
                return;
            }

            try
            {
                // 🔐 Encrypt password before storing
                string encryptedPassword = EncryptionHelper.Encrypt(password);

                var newUser = new
                {
                    username = username,
                    password = encryptedPassword,
                    created_at = DateTime.UtcNow.ToString("o")
                };

                await FirebaseHelper.PushDataAsync("accounts", newUser);
                MessageBox.Show("Account successfully created!");

                // Clear fields
                usernameField.Text = "";
                passwordField.Text = "";
                confirm_pass_field.Text = "";

                // Redirect to login
                Form1 loginForm = new Form1();
                loginForm.Show();
                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating account: {ex.Message}");
            }
        }
    }
}
