using System;
using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using Computer_Science_IA___Uniform_Manager.Models;

namespace Computer_Science_IA___Uniform_Manager
{
    public partial class JoinOrganizationForm : Form
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private static readonly string API_BASE_URL = ConfigurationManager.AppSettings["ApiBaseUrl"] ?? "http://localhost:7109/api";
        private readonly UserInfo _currentUser;

        public JoinOrganizationForm(UserInfo user)
        {
            InitializeComponent();
            _currentUser = user;
        }

        private void JoinOrganizationForm_Load(object sender, EventArgs e)
        {
            // Set default role to Viewer
            comboBoxRole.SelectedIndex = 0;
            textBoxOrgCode.Focus();
        }

        private async void ButtonJoin_Click(object sender, EventArgs e)
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(textBoxOrgCode.Text))
            {
                MessageBox.Show("Please enter an organization code.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxOrgCode.Focus();
                return;
            }

            if (comboBoxRole.SelectedIndex < 0)
            {
                MessageBox.Show("Please select a role.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                comboBoxRole.Focus();
                return;
            }

            // Disable button to prevent double-click
            buttonJoin.Enabled = false;
            buttonJoin.Text = "Joining...";

            try
            {
                await JoinOrganizationAsync();
            }
            finally
            {
                buttonJoin.Enabled = true;
                buttonJoin.Text = "Request to Join";
            }
        }

        private async Task JoinOrganizationAsync()
        {
            try
            {
                // Map combo box index to account level (reverse order in UI)
                int accountLevel = comboBoxRole.SelectedIndex switch
                {
                    0 => 2, // Viewer
                    1 => 1, // User
                    2 => 0, // Admin
                    _ => 2  // Default to Viewer
                };

                var request = new JoinOrganizationRequest
                {
                    OrganizationCode = textBoxOrgCode.Text.Trim().ToUpper(),
                    UserId = _currentUser.UserId,
                    RequestedAccountLevel = accountLevel,
                    RequestMessage = string.IsNullOrWhiteSpace(textBoxMessage.Text) ? null : textBoxMessage.Text.Trim()
                };

                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync($"{API_BASE_URL}/JoinOrganization", content);
                var jsonString = await response.Content.ReadAsStringAsync();
                
                var result = JsonSerializer.Deserialize<JoinOrganizationResponse>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (result?.Success == true)
                {
                    string message = result.RequiresApproval
                        ? $"Request sent successfully!\n\n{result.Message}\n\nYou'll be able to access the organization once an administrator approves your request."
                        : $"Successfully joined organization!\n\n{result.Message}";
                    
                    MessageBox.Show(message, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show($"Failed to join organization:\n\n{result?.Message ?? "Unknown error"}", 
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (HttpRequestException httpEx)
            {
                MessageBox.Show($"Network error:\n\n{httpEx.Message}\n\nMake sure the Azure Function is running.", 
                    "Network Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error joining organization:\n\n{ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        // Request/Response Models
        private class JoinOrganizationRequest
        {
            public string OrganizationCode { get; set; } = string.Empty;
            public int UserId { get; set; }
            public int RequestedAccountLevel { get; set; }
            public string? RequestMessage { get; set; }
        }

        private class JoinOrganizationResponse
        {
            public bool Success { get; set; }
            public string Message { get; set; } = string.Empty;
            public bool RequiresApproval { get; set; }
        }
    }
}
