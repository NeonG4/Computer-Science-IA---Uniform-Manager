using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using Computer_Science_IA___Uniform_Manager.Models;

namespace Computer_Science_IA___Uniform_Manager
{
    public partial class OrganizationSelectorForm : Form
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private static readonly string API_BASE_URL = ConfigurationManager.AppSettings["ApiBaseUrl"] ?? "http://localhost:7109/api";
        private readonly UserInfo _currentUser;
        private List<OrganizationDto>? _organizations;

        public OrganizationDto? SelectedOrganization { get; private set; }

        public OrganizationSelectorForm(UserInfo user)
        {
            InitializeComponent();
            _currentUser = user;
        }

        private async void OrganizationSelectorForm_Load(object sender, EventArgs e)
        {
            labelUserInfo.Text = $"Logged in as: {_currentUser.FirstName} {_currentUser.LastName}";
            await LoadOrganizationsAsync();
        }

        private async Task LoadOrganizationsAsync()
        {
            try
            {
                listBoxOrganizations.Items.Clear();
                listBoxOrganizations.Items.Add("Loading organizations...");

                var response = await httpClient.GetAsync($"{API_BASE_URL}/GetOrganizations?userId={_currentUser.UserId}");

                var jsonString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    // Log to console but show user-friendly message in list
                    System.Diagnostics.Debug.WriteLine($"Error loading organizations: {response.StatusCode} - {jsonString}");

                    listBoxOrganizations.Items.Clear();
                    listBoxOrganizations.Items.Add("No organizations");
                    buttonSelect.Enabled = false;
                    return;
                }

                var result = JsonSerializer.Deserialize<GetOrganizationsResponse>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                listBoxOrganizations.Items.Clear();

                // Handle null or empty results gracefully
                if (result == null)
                {
                    System.Diagnostics.Debug.WriteLine("Received null response from GetOrganizations");
                    listBoxOrganizations.Items.Add("No organizations");
                    buttonSelect.Enabled = false;
                    return;
                }

                if (!result.Success)
                {
                    System.Diagnostics.Debug.WriteLine($"GetOrganizations returned error: {result.Message}");
                    listBoxOrganizations.Items.Add("No organizations");
                    buttonSelect.Enabled = false;
                    return;
                }

                // Check if Organizations list is null or empty - this is normal for new users
                if (result.Organizations == null || !result.Organizations.Any())
                {
                    _organizations = new List<OrganizationDto>();
                    listBoxOrganizations.Items.Add("No organizations");
                    labelSubtitle.Text = "Create your first organization to get started";
                    buttonSelect.Enabled = false;
                    return;
                }

                // Successfully loaded organizations
                _organizations = result.Organizations;

                foreach (var org in _organizations)
                {
                    var roleText = GetRoleText(org.UserAccountLevel);
                    listBoxOrganizations.Items.Add($"{org.OrganizationName} ({roleText})");
                }

                labelSubtitle.Text = $"You have access to {_organizations.Count} organization(s)";
                buttonSelect.Enabled = true;
            }
            catch (HttpRequestException httpEx)
            {
                // Network error - likely function not running
                System.Diagnostics.Debug.WriteLine($"Network error loading organizations: {httpEx.Message}");
                listBoxOrganizations.Items.Clear();
                listBoxOrganizations.Items.Add("No organizations");
                buttonSelect.Enabled = false;
            }
            catch (JsonException jsonEx)
            {
                System.Diagnostics.Debug.WriteLine($"JSON parsing error: {jsonEx.Message}");
                listBoxOrganizations.Items.Clear();
                listBoxOrganizations.Items.Add("No organizations");
                buttonSelect.Enabled = false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Unexpected error loading organizations: {ex}");
                listBoxOrganizations.Items.Clear();
                listBoxOrganizations.Items.Add("No organizations");
                buttonSelect.Enabled = false;
            }
        }

        private string GetRoleText(int? accountLevel)
        {
            return accountLevel switch
            {
                0 => "Admin",
                1 => "User",
                2 => "Viewer",
                _ => "Unknown"
            };
        }

        private void ButtonSelect_Click(object sender, EventArgs e)
        {
            SelectOrganization();
        }

        private void ListBoxOrganizations_DoubleClick(object sender, EventArgs e)
        {
            SelectOrganization();
        }

        private void SelectOrganization()
        {
            if (_organizations == null || _organizations.Count == 0)
            {
                // Just don't do anything - the UI already shows the message
                System.Diagnostics.Debug.WriteLine("No organizations to select");
                return;
            }

            if (listBoxOrganizations.SelectedIndex < 0)
            {
                // User didn't select anything - just return silently
                return;
            }

            if (listBoxOrganizations.SelectedIndex >= _organizations.Count)
            {
                System.Diagnostics.Debug.WriteLine($"Invalid selection index: {listBoxOrganizations.SelectedIndex}");
                return;
            }

            SelectedOrganization = _organizations[listBoxOrganizations.SelectedIndex];

            if (SelectedOrganization == null)
            {
                System.Diagnostics.Debug.WriteLine("Selected organization is null");
                return;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private async void ButtonCreateOrg_Click(object sender, EventArgs e)
        {
            var createOrgForm = new CreateOrganizationForm(_currentUser);
            if (createOrgForm.ShowDialog() == DialogResult.OK)
            {
                // Reload organizations after creating a new one
                await LoadOrganizationsAsync();
                buttonSelect.Enabled = true;
            }
        }

        private async void ButtonRefresh_Click(object sender, EventArgs e)
        {
            await LoadOrganizationsAsync();
        }

        private void ButtonLogout_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        // Response Models
        private class GetOrganizationsResponse
        {
            public bool Success { get; set; }
            public string Message { get; set; } = string.Empty;
            public List<OrganizationDto>? Organizations { get; set; }
            public int TotalCount { get; set; }
        }

        private async void buttonJohnOrg_Click(object sender, EventArgs e)
        {
            var createOrgForm = new JoinOrganizationForm(_currentUser);
            if (createOrgForm.ShowDialog() == DialogResult.OK)
            {
                // Reload organizations after creating a new one
                await LoadOrganizationsAsync();
                buttonSelect.Enabled = true;
            }
        }
    }
}
