using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Computer_Science_IA___Uniform_Manager
{
    public partial class LoginForm : Form
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private const string API_BASE_URL = "http://localhost:7109/api";
        private HashAlgorithm sha256 = SHA256.Create();

        public LoginForm()
        {
            InitializeComponent();
        }

        private async void buttonLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxPassword.Text) || string.IsNullOrEmpty(textBoxUsername.Text))
            {
                MessageBox.Show("Please enter value in all field.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                var loginRequest = new
                {
                    Username = textBoxUsername.Text,
                    Password = textBoxPassword.Text
                };

                var jsonContent = JsonSerializer.Serialize(loginRequest);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync($"{API_BASE_URL}/Login", content);
                var responseBody = await response.Content.ReadAsStringAsync();
                
                var result = JsonSerializer.Deserialize<LoginResponse>(responseBody, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (result != null && result.Success)
                {
                    this.Hide();
                    UniformManager home = new UniformManager();
                    home.ShowDialog();
                }
                else
                {
                    MessageBox.Show(result?.Message ?? "No Account available with this username and password", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show($"Network error: {ex.Message}\nMake sure the Azure Function is running.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Login error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void buttonCreate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxConfirmPasswordCreate.Text) || 
                string.IsNullOrEmpty(textBoxPasswordCreate.Text) || 
                string.IsNullOrEmpty(textBoxUsernameCreate.Text) ||
                string.IsNullOrEmpty(textBoxFirstNameCreate.Text) ||
                string.IsNullOrEmpty(textBoxLastNameCreate.Text) ||
                string.IsNullOrEmpty(textBoxEmailCreate.Text))
            {
                MessageBox.Show("Please enter value in all field.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (textBoxConfirmPasswordCreate.Text != textBoxPasswordCreate.Text)
            {
                MessageBox.Show("Please enter both password same", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (textBoxPasswordCreate.Text.Length < 12)
            {
                MessageBox.Show("Password must be at least 12 characters long.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                var accountRequest = new
                {
                    FirstName = textBoxFirstNameCreate.Text,
                    LastName = textBoxLastNameCreate.Text,
                    Email = textBoxEmailCreate.Text,
                    Password = textBoxPasswordCreate.Text,
                    Username = textBoxUsernameCreate.Text
                };

                var jsonContent = JsonSerializer.Serialize(accountRequest);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync($"{API_BASE_URL}/CreateAccount", content);
                var responseBody = await response.Content.ReadAsStringAsync();
                
                var result = JsonSerializer.Deserialize<CreateAccountResponse>(responseBody, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (result != null && result.Success)
                {
                    MessageBox.Show("Your Account is created. Please login now.", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    // Clear form fields
                    textBoxFirstNameCreate.Clear();
                    textBoxLastNameCreate.Clear();
                    textBoxEmailCreate.Clear();
                    textBoxUsernameCreate.Clear();
                    textBoxPasswordCreate.Clear();
                    textBoxConfirmPasswordCreate.Clear();
                }
                else
                {
                    MessageBox.Show(result?.Message ?? "Failed to create account.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show($"Network error: {ex.Message}\nMake sure the Azure Function is running.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating account: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public UserInfo? User { get; set; }
    }

    public class UserInfo
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int AccountLevel { get; set; }
    }

    public class CreateAccountResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int? UserId { get; set; }
    }
}
