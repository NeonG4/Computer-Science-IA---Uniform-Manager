using System;
using System.Configuration;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using Computer_Science_IA___Uniform_Manager.Models;

namespace Computer_Science_IA___Uniform_Manager
{
    public partial class LoginForm : Form
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private static readonly string API_BASE_URL = ConfigurationManager.AppSettings["ApiBaseUrl"] ?? "http://localhost:8001/api";
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

            // Save original text and set loading state
            var originalButtonText = buttonLogin.Text;
            buttonLogin.Enabled = false;
            buttonLogin.Text = "Loading...";

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
                    if (result.User == null || result.User.UserId <= 0)
                    {
                        MessageBox.Show("Login succeeded but user data is missing. Please log in again.",
                            "Login Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    this.Hide();

                    try
                    {
                        // Show organization selector
                        var orgSelector = new OrganizationSelectorForm(result.User);
                        var dialogResult = orgSelector.ShowDialog();

                        if (dialogResult == DialogResult.OK && orgSelector.SelectedOrganization != null)
                        {
                            // Open main form with selected organization
                            UniformManagerHome home = new UniformManagerHome(
                                result.User,
                                orgSelector.SelectedOrganization);
                            home.ShowDialog();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error opening organization selection:\n\n{ex.Message}",
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    this.Close();
                }
                else
                {
                    MessageBox.Show(result?.Message ?? "No Account available with this username and password", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show($"Network error: {ex.Message}\n\nAPI URL: {API_BASE_URL}\nMake sure the Azure Function is running or check your App.config for the correct ApiBaseUrl.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Login error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                buttonLogin.Enabled = true;
                buttonLogin.Text = originalButtonText;
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

            // Save original text and set loading state
            var originalButtonText = buttonCreate.Text;
            buttonCreate.Enabled = false;
            buttonCreate.Text = "Loading...";

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
                    // Store credentials for auto-login
                    string username = textBoxUsernameCreate.Text;
                    string password = textBoxPasswordCreate.Text;
                    
                    // Clear form fields
                    textBoxFirstNameCreate.Clear();
                    textBoxLastNameCreate.Clear();
                    textBoxEmailCreate.Clear();
                    textBoxUsernameCreate.Clear();
                    textBoxPasswordCreate.Clear();
                    textBoxConfirmPasswordCreate.Clear();
                    
                    MessageBox.Show("Your account has been created successfully!\n\nLogging you in...", 
                        "Account Created", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    // Automatically log in the user
                    await AutoLoginAsync(username, password);
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
            finally
            {
                buttonCreate.Enabled = true;
                buttonCreate.Text = originalButtonText;
            }
        }

        private async Task AutoLoginAsync(string username, string password)
        {
            try
            {
                var loginRequest = new
                {
                    Username = username,
                    Password = password
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
                    if (result.User == null || result.User.UserId <= 0)
                    {
                        MessageBox.Show("Account created but user data is missing. Please log in manually.",
                            "Login Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    this.Hide();

                    try
                    {
                        // Show organization selector
                        var orgSelector = new OrganizationSelectorForm(result.User);
                        var dialogResult = orgSelector.ShowDialog();

                        if (dialogResult == DialogResult.OK && orgSelector.SelectedOrganization != null)
                        {
                            // Open main form with selected organization
                            UniformManagerHome home = new UniformManagerHome(
                                result.User,
                                orgSelector.SelectedOrganization);
                            home.ShowDialog();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Account created but failed to open organization selection:\n\n{ex.Message}\n\nPlease log in manually.",
                            "Login Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    this.Close();
                }
                else
                {
                    MessageBox.Show("Account created but automatic login failed. Please log in manually.", 
                        "Login Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Account created but automatic login failed: {ex.Message}\n\nPlease log in manually.", 
                    "Login Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
