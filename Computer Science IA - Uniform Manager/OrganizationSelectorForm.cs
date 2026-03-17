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
            try
            {
                if (_currentUser == null || _currentUser.UserId <= 0)
                {
                    MessageBox.Show("Your session has expired. Please log in again.",
                        "Login Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    DialogResult = DialogResult.Cancel;
                    Close();
                    return;
                }

                var firstName = _currentUser.FirstName ?? string.Empty;
                var lastName = _currentUser.LastName ?? string.Empty;
                var displayName = string.Join(" ", new[] { firstName, lastName }.Where(n => !string.IsNullOrWhiteSpace(n)));
                labelUserInfo.Text = string.IsNullOrWhiteSpace(displayName)
                    ? "Logged in as: (unknown)"
                    : $"Logged in as: {displayName}";

                await LoadOrganizationsAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                MessageBox.Show($"Error loading organization selection:\n\n{ex}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.Cancel;
                Close();
            }
        }

        private async Task LoadOrganizationsAsync()
        {
            try
            {
                listBoxOrganizations.Items.Clear();
                listBoxOrganizations.Items.Add("Loading organizations...");

                var response = await httpClient.GetAsync($"{API_BASE_URL}/GetOrganizations?userId={_currentUser.UserId}");

                var jsonString = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrWhiteSpace(jsonString))
                {
                    listBoxOrganizations.Items.Clear();
                    listBoxOrganizations.Items.Add("No organizations");
                    buttonSelect.Enabled = false;
                    return;
                }

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

                // Handle null or empty results
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

                var safeOrganizations = result.Organizations?.Where(o => o != null).ToList() ?? new List<OrganizationDto>();

                if (safeOrganizations.Count == 0)
                {
                    _organizations = new List<OrganizationDto>();
                    listBoxOrganizations.Items.Add("No organizations");
                    labelSubtitle.Text = "Create your first organization to get started";
                    buttonSelect.Enabled = false;
                    return;
                }

                // Successfully loaded organizations
                _organizations = safeOrganizations;

                foreach (var org in _organizations)
                {
                    var roleText = GetRoleText(org.UserAccountLevel);
                    var orgName = string.IsNullOrWhiteSpace(org.OrganizationName)
                        ? "(Unnamed Organization)"
                        : org.OrganizationName;
                    listBoxOrganizations.Items.Add($"{orgName} ({roleText})");
                }

                labelSubtitle.Text = $"You have access to {_organizations.Count} organization(s)";
                buttonSelect.Enabled = _organizations.Count > 0;
            }
            catch (HttpRequestException httpEx)
            {
                // Network error - function not running
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

        private async void ButtonJoinOrg_Click(object sender, EventArgs e)
        {
            var joinOrgForm = new JoinOrganizationForm(_currentUser);
            if (joinOrgForm.ShowDialog() == DialogResult.OK)
            {
                // Show success message with instructions
                MessageBox.Show("Your request has been sent to the organization administrators for approval.\n\n" +
                    "Click 'Refresh' once your request is approved to see the organization in the list.",
                    "Request Sent", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                // Reload organizations in case it was auto-approved
                await LoadOrganizationsAsync();
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
    }
}
