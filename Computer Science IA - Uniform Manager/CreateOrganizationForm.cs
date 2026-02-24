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
    public partial class CreateOrganizationForm : Form
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private static readonly string API_BASE_URL = ConfigurationManager.AppSettings["ApiBaseUrl"] ?? "http://localhost:7109/api";
        private readonly UserInfo _currentUser;

        public CreateOrganizationForm(UserInfo user)
        {
            InitializeComponent();
            _currentUser = user;
        }

        private async void ButtonCreate_Click(object sender, EventArgs e)
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(textBoxOrgName.Text))
            {
                MessageBox.Show("Please enter an organization name.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxOrgName.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(textBoxOrgCode.Text))
            {
                MessageBox.Show("Please enter an organization code.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxOrgCode.Focus();
                return;
            }

            // Disable button to prevent double-click
            buttonCreate.Enabled = false;
            buttonCreate.Text = "Creating...";

            try
            {
                await CreateOrganizationAsync();
            }
            finally
            {
                buttonCreate.Enabled = true;
                buttonCreate.Text = "Create Organization";
            }
        }

        private async Task CreateOrganizationAsync()
        {
            try
            {
                var request = new CreateOrganizationRequest
                {
                    OrganizationName = textBoxOrgName.Text.Trim(),
                    OrganizationCode = textBoxOrgCode.Text.Trim(),
                    Description = textBoxDescription.Text.Trim(),
                    UserId = _currentUser.UserId
                };

                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync($"{API_BASE_URL}/CreateOrganization", content);
                var jsonString = await response.Content.ReadAsStringAsync();
                
                var result = JsonSerializer.Deserialize<CreateOrganizationResponse>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (result?.Success == true)
                {
                    MessageBox.Show($"Organization '{request.OrganizationName}' created successfully!\n\nYou have been assigned as an Administrator.",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show($"Failed to create organization: {result?.Message ?? "Unknown error"}", 
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating organization: {ex.Message}\n\nMake sure the Azure Function is running.", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        // Request/Response Models
        private class CreateOrganizationRequest
        {
            public string OrganizationName { get; set; } = string.Empty;
            public string OrganizationCode { get; set; } = string.Empty;
            public string? Description { get; set; }
            public int UserId { get; set; }
        }

        private class CreateOrganizationResponse
        {
            public bool Success { get; set; }
            public string Message { get; set; } = string.Empty;
        }
    }
}
