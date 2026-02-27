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
    public partial class ManageOrganizationUsersForm : Form
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private static readonly string API_BASE_URL = ConfigurationManager.AppSettings["ApiBaseUrl"] ?? "http://localhost:7109/api";
        private readonly UserInfo _currentUser;
        private readonly OrganizationDto _organization;
        private List<OrganizationUserDto>? _users;

        public ManageOrganizationUsersForm(UserInfo user, OrganizationDto organization)
        {
            InitializeComponent();
            _currentUser = user;
            _organization = organization;
        }

        private async void ManageOrganizationUsersForm_Load(object sender, EventArgs e)
        {
            this.Text = $"Manage Users - {_organization.OrganizationName}";
            labelTitle.Text = $"Manage Users - {_organization.OrganizationName}";
            await LoadUsersAsync();
        }

        private async Task LoadUsersAsync()
        {
            try
            {
                labelStatus.Text = "Loading users...";
                buttonChangeRole.Enabled = false;
                buttonRemoveUser.Enabled = false;

                var response = await httpClient.GetAsync(
                    $"{API_BASE_URL}/GetOrganizationUsers?organizationId={_organization.OrganizationId}&userId={_currentUser.UserId}");

                var jsonString = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<GetOrganizationUsersResponse>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (result?.Success == true)
                {
                    _users = result.Users;
                    DisplayUsers();
                    UpdateStats();
                    labelStatus.Text = $"{result.TotalCount} user(s)";
                }
                else
                {
                    MessageBox.Show($"Error loading users:\n\n{result?.Message ?? "Unknown error"}",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    labelStatus.Text = "Error loading users";
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
                MessageBox.Show($"Error loading users:\n\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                labelStatus.Text = "Error";
            }
        }

        private void DisplayUsers()
        {
            dataGridViewUsers.DataSource = null;

            if (_users == null || !_users.Any())
            {
                labelStatus.Text = "No users found";
                return;
            }

            var displayList = _users
                .Where(u => u.IsActive)
                .Select(u => new
                {
                    UserId = u.UserId,
                    Name = $"{u.FirstName} {u.LastName}",
                    Email = u.Email,
                    Role = GetRoleText(u.AccountLevel),
                    JoinedDate = u.JoinedDate.ToString("d")
                }).ToList();

            dataGridViewUsers.DataSource = displayList;

            // Hide UserId column
            if (dataGridViewUsers.Columns["UserId"] != null)
                dataGridViewUsers.Columns["UserId"].Visible = false;

            // Set column widths
            if (dataGridViewUsers.Columns["Name"] != null)
                dataGridViewUsers.Columns["Name"].Width = 200;
            if (dataGridViewUsers.Columns["Email"] != null)
                dataGridViewUsers.Columns["Email"].Width = 250;
            if (dataGridViewUsers.Columns["Role"] != null)
                dataGridViewUsers.Columns["Role"].Width = 120;
        }

        private void UpdateStats()
        {
            if (_users == null || !_users.Any())
            {
                labelTotal.Text = "Total: 0";
                labelAdmins.Text = "Admins: 0";
                labelUsers.Text = "Users: 0";
                labelViewers.Text = "Viewers: 0";
                return;
            }

            var activeUsers = _users.Where(u => u.IsActive).ToList();
            int total = activeUsers.Count;
            int admins = activeUsers.Count(u => u.AccountLevel == 0);
            int users = activeUsers.Count(u => u.AccountLevel == 1);
            int viewers = activeUsers.Count(u => u.AccountLevel == 2);

            labelTotal.Text = $"Total: {total}";
            labelAdmins.Text = $"Admins: {admins}";
            labelUsers.Text = $"Users: {users}";
            labelViewers.Text = $"Viewers: {viewers}";
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

        private void DataGridViewUsers_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewUsers.SelectedRows.Count == 0)
            {
                buttonChangeRole.Enabled = false;
                buttonRemoveUser.Enabled = false;
                return;
            }

            var selectedRow = dataGridViewUsers.SelectedRows[0];
            int selectedUserId = (int)selectedRow.Cells["UserId"].Value;

            // Can't modify yourself
            if (selectedUserId == _currentUser.UserId)
            {
                buttonChangeRole.Enabled = false;
                buttonRemoveUser.Enabled = false;
            }
            else
            {
                buttonChangeRole.Enabled = true;
                buttonRemoveUser.Enabled = true;
            }
        }

        private async void ButtonChangeRole_Click(object sender, EventArgs e)
        {
            if (dataGridViewUsers.SelectedRows.Count == 0)
                return;

            var selectedRow = dataGridViewUsers.SelectedRows[0];
            int selectedUserId = (int)selectedRow.Cells["UserId"].Value;

            var user = _users?.FirstOrDefault(u => u.UserId == selectedUserId);
            if (user == null)
                return;

            // Show role selection dialog
            using var roleForm = new Form();
            roleForm.Text = $"Change Role for {user.FirstName} {user.LastName}";
            roleForm.Size = new System.Drawing.Size(400, 250);
            roleForm.StartPosition = FormStartPosition.CenterParent;
            roleForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            roleForm.MaximizeBox = false;
            roleForm.MinimizeBox = false;

            var label = new Label
            {
                Text = $"Select new role for {user.FirstName} {user.LastName}:",
                Location = new System.Drawing.Point(20, 20),
                Size = new System.Drawing.Size(350, 30)
            };

            var comboBox = new ComboBox
            {
                Location = new System.Drawing.Point(20, 60),
                Size = new System.Drawing.Size(350, 30),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            comboBox.Items.AddRange(new object[] { "Administrator", "User", "Viewer" });
            comboBox.SelectedIndex = user.AccountLevel;

            var btnOk = new Button
            {
                Text = "Change Role",
                DialogResult = DialogResult.OK,
                Location = new System.Drawing.Point(190, 120),
                Size = new System.Drawing.Size(180, 40)
            };

            var btnCancel = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Location = new System.Drawing.Point(20, 120),
                Size = new System.Drawing.Size(150, 40)
            };

            roleForm.Controls.AddRange(new Control[] { label, comboBox, btnOk, btnCancel });
            roleForm.AcceptButton = btnOk;
            roleForm.CancelButton = btnCancel;

            if (roleForm.ShowDialog() == DialogResult.OK)
            {
                await UpdateUserRoleAsync(selectedUserId, comboBox.SelectedIndex, 
                    $"{user.FirstName} {user.LastName}");
            }
        }

        private async Task UpdateUserRoleAsync(int targetUserId, int newAccountLevel, string userName)
        {
            try
            {
                buttonChangeRole.Enabled = false;
                buttonRemoveUser.Enabled = false;
                labelStatus.Text = "Updating role...";

                var request = new UpdateUserRoleRequest
                {
                    OrganizationId = _organization.OrganizationId,
                    RequestingUserId = _currentUser.UserId,
                    TargetUserId = targetUserId,
                    NewAccountLevel = newAccountLevel
                };

                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.PutAsync($"{API_BASE_URL}/UpdateOrganizationUserRole", content);
                var jsonString = await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<UpdateUserRoleResponse>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (result?.Success == true)
                {
                    MessageBox.Show($"Role updated successfully for {userName}!\n\n{result.Message}",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadUsersAsync();
                }
                else
                {
                    MessageBox.Show($"Error updating role:\n\n{result?.Message ?? "Unknown error"}",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    labelStatus.Text = "Error";
                    buttonChangeRole.Enabled = true;
                    buttonRemoveUser.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating role:\n\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                labelStatus.Text = "Error";
                buttonChangeRole.Enabled = true;
                buttonRemoveUser.Enabled = true;
            }
        }

        private async void ButtonRemoveUser_Click(object sender, EventArgs e)
        {
            if (dataGridViewUsers.SelectedRows.Count == 0)
                return;

            var selectedRow = dataGridViewUsers.SelectedRows[0];
            int selectedUserId = (int)selectedRow.Cells["UserId"].Value;

            var user = _users?.FirstOrDefault(u => u.UserId == selectedUserId);
            if (user == null)
                return;

            var confirmResult = MessageBox.Show(
                $"Are you sure you want to remove {user.FirstName} {user.LastName} from this organization?\n\n" +
                $"Role: {GetRoleText(user.AccountLevel)}\n" +
                $"Email: {user.Email}\n\n" +
                $"They will lose all access to this organization's data.",
                "Confirm Remove User",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirmResult != DialogResult.Yes)
                return;

            await RemoveUserAsync(selectedUserId, $"{user.FirstName} {user.LastName}");
        }

        private async Task RemoveUserAsync(int targetUserId, string userName)
        {
            try
            {
                buttonChangeRole.Enabled = false;
                buttonRemoveUser.Enabled = false;
                labelStatus.Text = "Removing user...";

                var response = await httpClient.DeleteAsync(
                    $"{API_BASE_URL}/RemoveOrganizationUser?organizationId={_organization.OrganizationId}" +
                    $"&targetUserId={targetUserId}&requestingUserId={_currentUser.UserId}");

                var jsonString = await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<RemoveUserResponse>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (result?.Success == true)
                {
                    MessageBox.Show($"{userName} has been removed from the organization.",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadUsersAsync();
                }
                else
                {
                    MessageBox.Show($"Error removing user:\n\n{result?.Message ?? "Unknown error"}",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    labelStatus.Text = "Error";
                    buttonChangeRole.Enabled = true;
                    buttonRemoveUser.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error removing user:\n\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                labelStatus.Text = "Error";
                buttonChangeRole.Enabled = true;
                buttonRemoveUser.Enabled = true;
            }
        }

        private async void ButtonRefresh_Click(object sender, EventArgs e)
        {
            await LoadUsersAsync();
        }

        private void ButtonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Response Models
        private class GetOrganizationUsersResponse
        {
            public bool Success { get; set; }
            public string Message { get; set; } = string.Empty;
            public List<OrganizationUserDto>? Users { get; set; }
            public int TotalCount { get; set; }
        }

        private class OrganizationUserDto
        {
            public int UserId { get; set; }
            public string Username { get; set; } = string.Empty;
            public string FirstName { get; set; } = string.Empty;
            public string LastName { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public int AccountLevel { get; set; }
            public DateTime JoinedDate { get; set; }
            public bool IsActive { get; set; }
        }

        private class UpdateUserRoleRequest
        {
            public int OrganizationId { get; set; }
            public int RequestingUserId { get; set; }
            public int TargetUserId { get; set; }
            public int NewAccountLevel { get; set; }
        }

        private class UpdateUserRoleResponse
        {
            public bool Success { get; set; }
            public string Message { get; set; } = string.Empty;
        }

        private class RemoveUserResponse
        {
            public bool Success { get; set; }
            public string Message { get; set; } = string.Empty;
        }
    }
}
