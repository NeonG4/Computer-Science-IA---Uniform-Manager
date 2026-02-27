using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using Computer_Science_IA___Uniform_Manager.Models;

namespace Computer_Science_IA___Uniform_Manager
{
    public partial class ManageJoinRequestsForm : Form
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private static readonly string API_BASE_URL = ConfigurationManager.AppSettings["ApiBaseUrl"] ?? "http://localhost:7109/api";
        private readonly UserInfo _currentUser;
        private readonly OrganizationDto _organization;
        private List<JoinRequestDto>? _requests;

        public ManageJoinRequestsForm(UserInfo user, OrganizationDto organization)
        {
            InitializeComponent();
            _currentUser = user;
            _organization = organization;
        }

        private async void ManageJoinRequestsForm_Load(object sender, EventArgs e)
        {
            this.Text = $"Join Requests - {_organization.OrganizationName}";
            labelTitle.Text = $"Pending Join Requests - {_organization.OrganizationName}";
            await LoadRequestsAsync();
        }

        private async Task LoadRequestsAsync()
        {
            try
            {
                labelStatus.Text = "Loading requests...";
                buttonApprove.Enabled = false;
                buttonReject.Enabled = false;

                var response = await httpClient.GetAsync(
                    $"{API_BASE_URL}/GetJoinRequests?organizationId={_organization.OrganizationId}&userId={_currentUser.UserId}");
                
                var jsonString = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<GetJoinRequestsResponse>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (result?.Success == true)
                {
                    _requests = result.Requests;
                    DisplayRequests();
                    labelStatus.Text = $"{result.TotalCount} pending request(s)";
                }
                else
                {
                    MessageBox.Show($"Error loading requests:\n\n{result?.Message ?? "Unknown error"}",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    labelStatus.Text = "Error loading requests";
                }
            }
            catch (HttpRequestException httpEx)
            {
                MessageBox.Show($"Network error:\n\n{httpEx.Message}\n\nMake sure the Azure Function is running.",
                    "Network Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                labelStatus.Text = "Network error";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading requests:\n\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                labelStatus.Text = "Error";
            }
        }

        private void DisplayRequests()
        {
            dataGridViewRequests.DataSource = null;

            if (_requests == null || !_requests.Any())
            {
                labelStatus.Text = "No pending requests";
                return;
            }

            var displayList = _requests.Select(r => new
            {
                RequestId = r.RequestId,
                Name = r.UserName,
                Email = r.UserEmail,
                RequestedRole = GetRoleText(r.RequestedAccountLevel),
                Message = r.RequestMessage ?? "(No message)",
                RequestDate = r.RequestedDate.ToString("g")
            }).ToList();

            dataGridViewRequests.DataSource = displayList;

            // Hide RequestId column
            if (dataGridViewRequests.Columns["RequestId"] != null)
                dataGridViewRequests.Columns["RequestId"].Visible = false;
        }

        private string GetRoleText(int accountLevel)
        {
            return accountLevel switch
            {
                0 => "Administrator",
                1 => "User",
                2 => "Viewer",
                _ => "Unknown"
            };
        }

        private void DataGridViewRequests_SelectionChanged(object sender, EventArgs e)
        {
            bool hasSelection = dataGridViewRequests.SelectedRows.Count > 0;
            buttonApprove.Enabled = hasSelection;
            buttonReject.Enabled = hasSelection;
        }

        private async void ButtonApprove_Click(object sender, EventArgs e)
        {
            await ReviewRequestAsync(true);
        }

        private async void ButtonReject_Click(object sender, EventArgs e)
        {
            await ReviewRequestAsync(false);
        }

        private async Task ReviewRequestAsync(bool approved)
        {
            if (dataGridViewRequests.SelectedRows.Count == 0)
                return;

            var selectedRow = dataGridViewRequests.SelectedRows[0];
            int requestId = (int)selectedRow.Cells["RequestId"].Value;

            var request = _requests?.FirstOrDefault(r => r.RequestId == requestId);
            if (request == null)
                return;

            string action = approved ? "approve" : "reject";
            var confirmResult = MessageBox.Show(
                $"Are you sure you want to {action} the request from {request.UserName}?\n\n" +
                $"Requested Role: {GetRoleText(request.RequestedAccountLevel)}\n" +
                $"Message: {request.RequestMessage ?? "(No message)"}",
                $"Confirm {action.ToUpper()}",
                MessageBoxButtons.YesNo,
                approved ? MessageBoxIcon.Question : MessageBoxIcon.Warning);

            if (confirmResult != DialogResult.Yes)
                return;

            try
            {
                buttonApprove.Enabled = false;
                buttonReject.Enabled = false;
                labelStatus.Text = $"{action}ing request...";

                var reviewRequest = new ReviewJoinRequestRequest
                {
                    RequestId = requestId,
                    ReviewerId = _currentUser.UserId,
                    Approved = approved,
                    ReviewNotes = null
                };

                var json = JsonSerializer.Serialize(reviewRequest);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync($"{API_BASE_URL}/ReviewJoinRequest", content);
                var jsonString = await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<ReviewJoinRequestResponse>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (result?.Success == true)
                {
                    MessageBox.Show($"Request {action}d successfully!\n\n{result.Message}",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadRequestsAsync(); // Refresh the list
                }
                else
                {
                    MessageBox.Show($"Error {action}ing request:\n\n{result?.Message ?? "Unknown error"}",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    labelStatus.Text = "Error";
                    buttonApprove.Enabled = true;
                    buttonReject.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error {action}ing request:\n\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                labelStatus.Text = "Error";
                buttonApprove.Enabled = true;
                buttonReject.Enabled = true;
            }
        }

        private async void ButtonRefresh_Click(object sender, EventArgs e)
        {
            await LoadRequestsAsync();
        }

        private void ButtonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Response Models
        private class GetJoinRequestsResponse
        {
            public bool Success { get; set; }
            public string Message { get; set; } = string.Empty;
            public List<JoinRequestDto>? Requests { get; set; }
            public int TotalCount { get; set; }
        }

        private class JoinRequestDto
        {
            public int RequestId { get; set; }
            public int OrganizationId { get; set; }
            public string OrganizationName { get; set; } = string.Empty;
            public int UserId { get; set; }
            public string UserName { get; set; } = string.Empty;
            public string UserEmail { get; set; } = string.Empty;
            public int RequestedAccountLevel { get; set; }
            public int Status { get; set; }
            public string? RequestMessage { get; set; }
            public DateTime RequestedDate { get; set; }
            public int? ReviewedBy { get; set; }
            public DateTime? ReviewedDate { get; set; }
            public string? ReviewNotes { get; set; }
        }

        private class ReviewJoinRequestRequest
        {
            public int RequestId { get; set; }
            public int ReviewerId { get; set; }
            public bool Approved { get; set; }
            public string? ReviewNotes { get; set; }
        }

        private class ReviewJoinRequestResponse
        {
            public bool Success { get; set; }
            public string Message { get; set; } = string.Empty;
        }
    }
}
