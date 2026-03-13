using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using Computer_Science_IA___Uniform_Manager.Models;

namespace Computer_Science_IA___Uniform_Manager
{
    public partial class UniformManagerAdminHome : Form
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private static readonly string API_BASE_URL = ConfigurationManager.AppSettings["ApiBaseUrl"] ?? "http://localhost:7109/api";
        private UserInfo? _currentUser;
        private OrganizationDto? _currentOrganization;
        private List<OrganizationUserDto>? _organizationUsers; // Store full user data for the organization

        public UniformManagerAdminHome()
        {
            InitializeComponent();
        }

        public UniformManagerAdminHome(UserInfo user, OrganizationDto organization) : this()
        {
            _currentUser = user;
            _currentOrganization = organization;
        }

        private async void UniformManagerAdminHome_Load(object sender, EventArgs e)
        {
            // If no user or organization is set, this shouldn't happen 
            if (_currentUser == null || _currentOrganization == null)
            {
                MessageBox.Show("No user or organization information available. Please log in again.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            // Update form title with user info and organization
            this.Text = $"Uniform Manager - {_currentOrganization.OrganizationName} - {_currentUser.FirstName} {_currentUser.LastName} ({GetAccountLevelText(_currentOrganization.UserAccountLevel)})";

            await LoadAllData();
        }

        private string GetAccountLevelText(int? accountLevel)
        {
            return accountLevel switch
            {
                0 => "Administrator",
                1 => "User",
                2 => "Viewer",
                _ => "Unknown"
            };
        }

        private async Task LoadAllData()
        {
            try
            {
                // Load all three tables concurrently
                var uniformsTask = LoadUniformsAsync();
                var studentsTask = LoadStudentsAsync();

                // Load users data - visibility depends on admin status
                var usersTask = LoadUsersAsync();
                await Task.WhenAll(uniformsTask, studentsTask, usersTask);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}\nMake sure the Azure Function is running.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadUniformsAsync()
        {
            try
            {
                var response = await httpClient.GetAsync(
                    $"{API_BASE_URL}/GetUniforms?userId={_currentUser?.UserId}&organizationId={_currentOrganization?.OrganizationId}");
                response.EnsureSuccessStatusCode();
                var jsonString = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<UniformListResponse>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (result?.Success == true && result.Uniforms != null)
                {
                    dataGridViewUniforms.DataSource = result.Uniforms;
                    FormatUniformsGrid();
                    labelUniforms.Text = $"Uniforms ({result.TotalCount})";

                    // Show management buttons based on user role
                    if (_currentOrganization?.UserAccountLevel == 0)
                    {
                        // Admin: Show all buttons
                        panelUniformsButtons.Visible = true;
                        buttonAddUniform.Visible = true;
                        buttonEditUniform.Visible = true;
                        buttonDeleteUniform.Visible = true;
                        buttonAssignUniform.Visible = true;
                        buttonUnassignUniform.Visible = true;
                        buttonCheckOutUniform.Visible = true;
                        buttonUpdateConditions.Visible = true;

                        // Show all context menu items
                        addUniformToolStripMenuItem.Visible = true;
                        editUniformToolStripMenuItem.Visible = true;
                        deleteUniformToolStripMenuItem.Visible = true;
                        toolStripSeparator2.Visible = true;
                        assignUniformToolStripMenuItem.Visible = true;
                        unassignUniformToolStripMenuItem.Visible = true;
                        toolStripSeparator3.Visible = true;
                        checkOutInToolStripMenuItem.Visible = true;
                        updateConditionsToolStripMenuItem.Visible = true;

                        dataGridViewUniforms.ContextMenuStrip = contextMenuStripUniforms;
                    }
                    else if (_currentOrganization?.UserAccountLevel == 1)
                    {
                        // User: Show only check out/in and conditions buttons
                        panelUniformsButtons.Visible = true;
                        buttonAddUniform.Visible = false;
                        buttonEditUniform.Visible = false;
                        buttonDeleteUniform.Visible = false;
                        buttonAssignUniform.Visible = false;
                        buttonUnassignUniform.Visible = false;
                        buttonCheckOutUniform.Visible = true;
                        buttonUpdateConditions.Visible = true;

                        // Hide admin context menu items, show only user items
                        addUniformToolStripMenuItem.Visible = false;
                        editUniformToolStripMenuItem.Visible = false;
                        deleteUniformToolStripMenuItem.Visible = false;
                        toolStripSeparator2.Visible = false;
                        assignUniformToolStripMenuItem.Visible = false;
                        unassignUniformToolStripMenuItem.Visible = false;
                        toolStripSeparator3.Visible = false;
                        checkOutInToolStripMenuItem.Visible = true;
                        updateConditionsToolStripMenuItem.Visible = true;

                        dataGridViewUniforms.ContextMenuStrip = contextMenuStripUniforms;
                    }
                    else
                    {
                        // Viewer: No buttons
                        panelUniformsButtons.Visible = false;
                        dataGridViewUniforms.ContextMenuStrip = null;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading uniforms: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                panelUniformsButtons.Visible = false;
            }
        }

        private async Task LoadStudentsAsync()
        {
            try
            {
                var response = await httpClient.GetAsync(
                    $"{API_BASE_URL}/GetStudents?userId={_currentUser?.UserId}&organizationId={_currentOrganization?.OrganizationId}");
                response.EnsureSuccessStatusCode();
                var jsonString = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<StudentListResponse>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (result?.Success == true && result.Students != null)
                {
                    dataGridViewStudents.DataSource = result.Students;
                    FormatStudentsGrid();
                    labelStudents.Text = $"Students ({result.TotalCount})";

                    // Show management buttons for admins
                    if (_currentOrganization?.UserAccountLevel == 0)
                    {
                        panelStudentsButtons.Visible = true;
                        dataGridViewStudents.ContextMenuStrip = contextMenuStripStudents;
                    }
                    else
                    {
                        panelStudentsButtons.Visible = false;
                        dataGridViewStudents.ContextMenuStrip = null;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading students: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                panelStudentsButtons.Visible = false;
            }
        }

        private async Task LoadUsersAsync()
        {
            try
            {
                // Check if current user is an administrator in this organization
                if (_currentOrganization?.UserAccountLevel != 0)
                {
                    // Non-admin: Show read-only message and hide management buttons
                    dataGridViewUsers.DataSource = null;
                    labelUsers.Text = "Users (Admin Only)";
                    panelUsersButtons.Visible = false;

                    // Hide the context menu for non-admins
                    dataGridViewUsers.ContextMenuStrip = null;
                    return;
                }

                // Admin: Load organization users and show management controls
                var response = await httpClient.GetAsync(
                    $"{API_BASE_URL}/GetOrganizationUsers?organizationId={_currentOrganization.OrganizationId}&userId={_currentUser?.UserId}");

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    labelUsers.Text = "Users (Insufficient Permissions)";
                    panelUsersButtons.Visible = false;
                    dataGridViewUsers.ContextMenuStrip = null;
                    return;
                }

                response.EnsureSuccessStatusCode();
                var jsonString = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<OrganizationUsersResponse>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (result?.Success == true && result.Users != null)
                {
                    // Store full user data
                    _organizationUsers = result.Users;

                    // Display simplified list
                    var displayList = result.Users
                        .Where(u => u.IsActive)
                        .Select(u => new
                        {
                            UserId = u.UserId,
                            Name = $"{u.FirstName} {u.LastName}",
                            Role = GetRoleText(u.AccountLevel),
                            Email = u.Email
                        }).ToList();

                    dataGridViewUsers.DataSource = displayList;
                    FormatOrganizationUsersGrid();
                    labelUsers.Text = $"Users ({result.Users.Count(u => u.IsActive)})";

                    // Show management controls for admins
                    panelUsersButtons.Visible = true;
                    dataGridViewUsers.ContextMenuStrip = contextMenuStripUsers;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading users: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                panelUsersButtons.Visible = false;
                dataGridViewUsers.ContextMenuStrip = null;
            }
        }

        private void FormatUniformsGrid()
        {
            if (dataGridViewUniforms.Columns.Contains("UniformId"))
                dataGridViewUniforms.Columns["UniformId"].Visible = false;

            if (dataGridViewUniforms.Columns.Contains("UniformIdentifier"))
                dataGridViewUniforms.Columns["UniformIdentifier"].HeaderText = "ID";

            if (dataGridViewUniforms.Columns.Contains("UniformType"))
                dataGridViewUniforms.Columns["UniformType"].Visible = false;

            if (dataGridViewUniforms.Columns.Contains("UniformTypeName"))
            {
                dataGridViewUniforms.Columns["UniformTypeName"].HeaderText = "Type";
                dataGridViewUniforms.Columns["UniformTypeName"].DisplayIndex = 1;
            }

            if (dataGridViewUniforms.Columns.Contains("Size"))
                dataGridViewUniforms.Columns["Size"].HeaderText = "Size";

            if (dataGridViewUniforms.Columns.Contains("IsCheckedOut"))
                dataGridViewUniforms.Columns["IsCheckedOut"].HeaderText = "Checked Out";

            if (dataGridViewUniforms.Columns.Contains("AssignedStudentId"))
                dataGridViewUniforms.Columns["AssignedStudentId"].HeaderText = "Assigned To";

            if (dataGridViewUniforms.Columns.Contains("Conditions"))
                dataGridViewUniforms.Columns["Conditions"].Visible = false;

            if (dataGridViewUniforms.Columns.Contains("ConditionNames"))
                dataGridViewUniforms.Columns["ConditionNames"].Visible = false;

            if (dataGridViewUniforms.Columns.Contains("IsInGoodCondition"))
            {
                dataGridViewUniforms.Columns["IsInGoodCondition"].HeaderText = "Good Condition";
                dataGridViewUniforms.Columns["IsInGoodCondition"].Width = 100;
            }

            if (dataGridViewUniforms.Columns.Contains("CreatedDate"))
                dataGridViewUniforms.Columns["CreatedDate"].Visible = false;

            if (dataGridViewUniforms.Columns.Contains("LastModified"))
                dataGridViewUniforms.Columns["LastModified"].Visible = false;
        }

        private void FormatStudentsGrid()
        {
            if (dataGridViewStudents.Columns.Contains("StudentId"))
                dataGridViewStudents.Columns["StudentId"].Visible = false;

            if (dataGridViewStudents.Columns.Contains("StudentIdentifier"))
                dataGridViewStudents.Columns["StudentIdentifier"].HeaderText = "Student ID";

            if (dataGridViewStudents.Columns.Contains("FirstName"))
                dataGridViewStudents.Columns["FirstName"].HeaderText = "First Name";

            if (dataGridViewStudents.Columns.Contains("LastName"))
                dataGridViewStudents.Columns["LastName"].HeaderText = "Last Name";

            if (dataGridViewStudents.Columns.Contains("FullName"))
                dataGridViewStudents.Columns["FullName"].Visible = false;

            if (dataGridViewStudents.Columns.Contains("Grade"))
                dataGridViewStudents.Columns["Grade"].HeaderText = "Grade";

            if (dataGridViewStudents.Columns.Contains("CreatedDate"))
                dataGridViewStudents.Columns["CreatedDate"].Visible = false;

            if (dataGridViewStudents.Columns.Contains("LastModified"))
                dataGridViewStudents.Columns["LastModified"].Visible = false;
        }

        private void FormatOrganizationUsersGrid()
        {
            if (dataGridViewUsers.Columns.Contains("UserId"))
                dataGridViewUsers.Columns["UserId"].Visible = false;

            if (dataGridViewUsers.Columns.Contains("Name"))
            {
                dataGridViewUsers.Columns["Name"].HeaderText = "Name";
                dataGridViewUsers.Columns["Name"].Width = 120;
            }

            if (dataGridViewUsers.Columns.Contains("Role"))
            {
                dataGridViewUsers.Columns["Role"].HeaderText = "Role";
                dataGridViewUsers.Columns["Role"].Width = 80;
            }

            if (dataGridViewUsers.Columns.Contains("Email"))
            {
                dataGridViewUsers.Columns["Email"].HeaderText = "Email";
                dataGridViewUsers.Columns["Email"].Width = 120;
            }
        }

        private string GetRoleText(int accountLevel)
        {
            return accountLevel switch
            {
                0 => "Admin",
                1 => "User",
                2 => "Viewer",
                _ => "Unknown"
            };
        }

        private void DataGridViewUsers_SelectionChanged(object sender, EventArgs e)
        {
            if (!panelUsersButtons.Visible) return;

            if (dataGridViewUsers.SelectedRows.Count == 0)
            {
                buttonChangeUserRole.Enabled = false;
                buttonRemoveUserFromOrg.Enabled = false;
                return;
            }

            var selectedRow = dataGridViewUsers.SelectedRows[0];
            int selectedUserId = (int)selectedRow.Cells["UserId"].Value;

            // Can't modify yourself
            if (selectedUserId == _currentUser?.UserId)
            {
                buttonChangeUserRole.Enabled = false;
                buttonRemoveUserFromOrg.Enabled = false;
            }
            else
            {
                buttonChangeUserRole.Enabled = true;
                buttonRemoveUserFromOrg.Enabled = true;
            }
        }

        private void DataGridViewStudents_SelectionChanged(object sender, EventArgs e)
        {
            if (!panelStudentsButtons.Visible) return;

            bool hasSelection = dataGridViewStudents.SelectedRows.Count > 0;
            buttonEditStudent.Enabled = hasSelection;
            buttonDeleteStudent.Enabled = hasSelection;
        }

        private async void DataGridViewStudents_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // Ignore clicks on header row
            if (e.RowIndex < 0) return;

            // Only allow edit for admins (those who can see the edit button)
            if (buttonEditStudent != null && buttonEditStudent.Visible)
            {
                await EditSelectedStudent();
            }
        }

        private void DataGridViewUniforms_SelectionChanged(object sender, EventArgs e)
        {
            if (!panelUniformsButtons.Visible) return;

            bool hasSelection = dataGridViewUniforms.SelectedRows.Count > 0;

            // Admin buttons - only enable if they're visible
            if (buttonEditUniform.Visible)
            {
                buttonEditUniform.Enabled = hasSelection;
            }

            if (buttonDeleteUniform.Visible)
            {
                buttonDeleteUniform.Enabled = hasSelection;
            }

            // Assign/Unassign buttons - enable based on assignment status
            if (hasSelection && buttonAssignUniform != null && buttonAssignUniform.Visible)
            {
                var selectedRow = dataGridViewUniforms.SelectedRows[0];
                string? assignedStudent = selectedRow.Cells["AssignedStudentId"].Value?.ToString();
                
                // Enable Assign only if NOT assigned
                buttonAssignUniform.Enabled = string.IsNullOrEmpty(assignedStudent);
                
                // Enable Unassign only if IS assigned
                buttonUnassignUniform.Enabled = !string.IsNullOrEmpty(assignedStudent);
            }
            else if (buttonAssignUniform != null && buttonAssignUniform.Visible)
            {
                buttonAssignUniform.Enabled = false;
                buttonUnassignUniform.Enabled = false;
            }

            // User and Admin buttons - always enable if visible
            if (buttonCheckOutUniform.Visible)
            {
                buttonCheckOutUniform.Enabled = hasSelection;
            }

            if (buttonUpdateConditions.Visible)
            {
                buttonUpdateConditions.Enabled = hasSelection;
            }
        }

        private async void DataGridViewUniforms_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // Ignore clicks on header row
            if (e.RowIndex < 0) return;

            // Only allow edit for admins (those who can see the edit button)
            if (buttonEditUniform != null && buttonEditUniform.Visible)
            {
                await EditSelectedUniform();
            }
        }

        private async void ButtonChangeUserRole_Click(object sender, EventArgs e)
        {
            await ChangeSelectedUserRole();
        }

        private async void ChangeRoleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await ChangeSelectedUserRole();
        }

        private async Task ChangeSelectedUserRole()
        {
            if (dataGridViewUsers.SelectedRows.Count == 0) return;

            var selectedRow = dataGridViewUsers.SelectedRows[0];
            int selectedUserId = (int)selectedRow.Cells["UserId"].Value;

            var user = _organizationUsers?.FirstOrDefault(u => u.UserId == selectedUserId);
            if (user == null) return;

            // Show role selection dialog
            using var roleForm = new Form();
            roleForm.Text = $"Change Role - {user.FirstName} {user.LastName}";
            roleForm.Size = new System.Drawing.Size(380, 220);
            roleForm.StartPosition = FormStartPosition.CenterParent;
            roleForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            roleForm.MaximizeBox = false;
            roleForm.MinimizeBox = false;

            var label = new Label
            {
                Text = $"Select new role for {user.FirstName} {user.LastName}:",
                Location = new System.Drawing.Point(15, 15),
                Size = new System.Drawing.Size(340, 25)
            };

            var comboBox = new ComboBox
            {
                Location = new System.Drawing.Point(15, 45),
                Size = new System.Drawing.Size(340, 30),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            comboBox.Items.AddRange(new object[] { "Administrator", "User", "Viewer" });
            comboBox.SelectedIndex = user.AccountLevel;

            var btnOk = new Button
            {
                Text = "Change Role",
                DialogResult = DialogResult.OK,
                Location = new System.Drawing.Point(185, 95),
                Size = new System.Drawing.Size(170, 35)
            };

            var btnCancel = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Location = new System.Drawing.Point(15, 95),
                Size = new System.Drawing.Size(150, 35)
            };

            roleForm.Controls.AddRange(new Control[] { label, comboBox, btnOk, btnCancel });
            roleForm.AcceptButton = btnOk;
            roleForm.CancelButton = btnCancel;

            if (roleForm.ShowDialog() == DialogResult.OK)
            {
                await UpdateUserRoleAsync(selectedUserId, comboBox.SelectedIndex, $"{user.FirstName} {user.LastName}");
            }
        }

        private async Task UpdateUserRoleAsync(int targetUserId, int newAccountLevel, string userName)
        {
            try
            {
                var request = new UpdateUserRoleRequest
                {
                    OrganizationId = _currentOrganization!.OrganizationId,
                    RequestingUserId = _currentUser!.UserId,
                    TargetUserId = targetUserId,
                    NewAccountLevel = newAccountLevel
                };

                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, MediaTypeHeaderValue.Parse("application/json"));

                var response = await httpClient.PutAsync($"{API_BASE_URL}/UpdateOrganizationUserRole", content);
                var jsonString = await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<UpdateUserRoleResponse>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (result?.Success == true)
                {
                    MessageBox.Show($"Role updated for {userName}!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadUsersAsync(); // Refresh the list
                }
                else
                {
                    MessageBox.Show($"Error updating role:\n\n{result?.Message ?? "Unknown error"}",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating role:\n\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void ButtonRemoveUserFromOrg_Click(object sender, EventArgs e)
        {
            await RemoveSelectedUser();
        }

        private async void RemoveFromOrgToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await RemoveSelectedUser();
        }

        private async Task RemoveSelectedUser()
        {
            if (dataGridViewUsers.SelectedRows.Count == 0) return;

            var selectedRow = dataGridViewUsers.SelectedRows[0];
            int selectedUserId = (int)selectedRow.Cells["UserId"].Value;

            var user = _organizationUsers?.FirstOrDefault(u => u.UserId == selectedUserId);
            if (user == null) return;

            var confirmResult = MessageBox.Show(
                $"Remove {user.FirstName} {user.LastName} from this organization?\n\n" +
                $"They will lose all access to this organization's data.",
                "Confirm Remove",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirmResult != DialogResult.Yes) return;

            await RemoveUserAsync(selectedUserId, $"{user.FirstName} {user.LastName}");
        }

        private async Task RemoveUserAsync(int targetUserId, string userName)
        {
            try
            {
                var response = await httpClient.DeleteAsync(
                    $"{API_BASE_URL}/RemoveOrganizationUser?organizationId={_currentOrganization!.OrganizationId}" +
                    $"&targetUserId={targetUserId}&requestingUserId={_currentUser!.UserId}");

                var jsonString = await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<RemoveUserResponse>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (result?.Success == true)
                {
                    MessageBox.Show($"{userName} removed from organization.", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadUsersAsync(); // Refresh the list
                }
                else
                {
                    MessageBox.Show($"Error removing user:\n\n{result?.Message ?? "Unknown error"}",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error removing user:\n\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region Uniform Management

        private async void ButtonAddUniform_Click(object sender, EventArgs e)
        {
            await AddNewUniform();
        }

        private async void AddUniformToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await AddNewUniform();
        }

        private async Task AddNewUniform()
        {
            using var addForm = new Form();
            addForm.Text = "Add New Uniform";
            addForm.Size = new System.Drawing.Size(400, 280);
            addForm.StartPosition = FormStartPosition.CenterParent;
            addForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            addForm.MaximizeBox = false;
            addForm.MinimizeBox = false;

            var lblUniformId = new Label { Text = "Uniform ID:", Location = new System.Drawing.Point(20, 20), Size = new System.Drawing.Size(100, 20) };
            var txtUniformId = new TextBox { Location = new System.Drawing.Point(130, 18), Size = new System.Drawing.Size(240, 20), CharacterCasing = CharacterCasing.Upper };

            var lblType = new Label { Text = "Type:", Location = new System.Drawing.Point(20, 60), Size = new System.Drawing.Size(100, 20) };
            var cmbType = new ComboBox {
                Location = new System.Drawing.Point(130, 58),
                Size = new System.Drawing.Size(240, 20),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbType.Items.AddRange(new object[] {
                "Concert Coat", "Drum Major Coat", "Hat", "Marching Coat",
                "Marching Shorts", "Marching Socks", "Pants"
            });

            var lblSize1 = new Label { Text = "Size:", Location = new System.Drawing.Point(20, 100), Size = new System.Drawing.Size(100, 20) };
            var txtSize1 = new TextBox { Location = new System.Drawing.Point(130, 98), Size = new System.Drawing.Size(100, 20) };
            var cmbSize1 = new ComboBox { Location = new System.Drawing.Point(130, 98), Size = new System.Drawing.Size(100, 20), DropDownStyle = ComboBoxStyle.DropDownList, Visible = false };
            cmbSize1.Items.AddRange(new object[] { "xs", "s", "m", "l", "xl" });

            var lblSize2 = new Label { Text = "Length:", Location = new System.Drawing.Point(240, 100), Size = new System.Drawing.Size(50, 20), Visible = false };
            var txtSize2 = new TextBox { Location = new System.Drawing.Point(290, 98), Size = new System.Drawing.Size(80, 20), Visible = false };
            var cmbSize2 = new ComboBox { Location = new System.Drawing.Point(290, 98), Size = new System.Drawing.Size(80, 20), DropDownStyle = ComboBoxStyle.DropDownList, Visible = false };
            cmbSize2.Items.AddRange(new object[] { "", "xs", "s", "m", "l", "xl" });

            cmbType.SelectedIndexChanged += (s, ev) => {
                int type = cmbType.SelectedIndex;

                txtSize1.Visible = true;
                cmbSize1.Visible = false;
                lblSize2.Visible = false;
                txtSize2.Visible = false;
                cmbSize2.Visible = false;

                if (type == 6) // Pants
                {
                    lblSize1.Text = "Width:";
                    lblSize2.Text = "Length:";
                    lblSize2.Visible = true;
                    txtSize2.Visible = true;
                }
                else if (type == 3) // Marching Coat
                {
                    lblSize1.Text = "Number Size:";
                    lblSize2.Text = "Sleeve:";
                    lblSize2.Visible = true;
                    txtSize2.Visible = true;
                }
                else if (type == 2) // Hat
                {
                    lblSize1.Text = "Size:";
                    txtSize1.Visible = false;
                    cmbSize1.Visible = true;
                    if (cmbSize1.SelectedIndex == -1) cmbSize1.SelectedIndex = 0;
                }
                else if (type == 0) // Concert Coat
                {
                    lblSize1.Text = "Number Size:";
                    lblSize2.Text = "Letter:";
                    lblSize2.Visible = true;
                    cmbSize2.Visible = true;
                    if (cmbSize2.SelectedIndex == -1) cmbSize2.SelectedIndex = 0;
                }
                else
                {
                    lblSize1.Text = "Size:";
                }
            };
            cmbType.SelectedIndex = 0;

            var btnCreate = new Button
            {
                Text = "Add Uniform",
                DialogResult = DialogResult.OK,
                Location = new System.Drawing.Point(200, 180),
                Size = new System.Drawing.Size(170, 35)
            };

            var btnCancel = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Location = new System.Drawing.Point(20, 180),
                Size = new System.Drawing.Size(150, 35)
            };

            addForm.Controls.AddRange(new Control[] {
                lblUniformId, txtUniformId,
                lblType, cmbType,
                lblSize1, txtSize1, cmbSize1,
                lblSize2, txtSize2, cmbSize2,
                btnCreate, btnCancel
            });
            addForm.AcceptButton = btnCreate;
            addForm.CancelButton = btnCancel;

            if (addForm.ShowDialog() == DialogResult.OK)
            {
                if (string.IsNullOrWhiteSpace(txtUniformId.Text))
                {
                    MessageBox.Show("Please enter a uniform ID.", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string finalSize = "";
                int type = cmbType.SelectedIndex;

                if (type == 2) // Hat
                {
                    finalSize = cmbSize1.Text;
                }
                else 
                {
                    if (string.IsNullOrWhiteSpace(txtSize1.Text))
                    {
                        MessageBox.Show("Please enter a size.", "Validation Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    
                    finalSize = txtSize1.Text.Trim();
                    if (type == 6 && !string.IsNullOrWhiteSpace(txtSize2.Text))
                    {
                        finalSize += $"x{txtSize2.Text.Trim()}";
                    }
                    else if (type == 3 && !string.IsNullOrWhiteSpace(txtSize2.Text))
                    {
                        finalSize += $" {txtSize2.Text.Trim()}";
                    }
                    else if (type == 0 && !string.IsNullOrWhiteSpace(cmbSize2.Text))
                    {
                        finalSize += $" {cmbSize2.Text}";
                    }
                }

                await CreateUniformAsync(txtUniformId.Text.Trim(), type, finalSize);
            }
        }

        private async Task CreateUniformAsync(string uniformId, int uniformType, string size)
        {
            try
            {
                var request = new CreateUniformRequest
                {
                    OrganizationId = _currentOrganization!.OrganizationId,
                    UniformIdentifier = uniformId,
                    UniformType = uniformType,
                    Size = size,
                    RequestingUserId = _currentUser!.UserId
                };

                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, MediaTypeHeaderValue.Parse("application/json"));

                var response = await httpClient.PostAsync($"{API_BASE_URL}/CreateUniform", content);
                var jsonString = await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<UniformResponse>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (result?.Success == true)
                {
                    MessageBox.Show($"Uniform '{uniformId}' added successfully!",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadUniformsAsync();
                }
                else
                {
                    MessageBox.Show($"Error adding uniform:\n\n{result?.Message ?? "Unknown error"}",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding uniform:\n\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void ButtonEditUniform_Click(object sender, EventArgs e)
        {
            await EditSelectedUniform();
        }

        private async void EditUniformToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await EditSelectedUniform();
        }

        private async Task EditSelectedUniform()
        {
            if (dataGridViewUniforms.SelectedRows.Count == 0) return;

            var selectedRow = dataGridViewUniforms.SelectedRows[0];
            string uniformId = selectedRow.Cells["UniformIdentifier"].Value.ToString()!;
            int uniformType = Convert.ToInt32(selectedRow.Cells["UniformType"].Value);
            string size = selectedRow.Cells["Size"].Value.ToString()!;

            using var editForm = new Form();
            editForm.Text = $"Edit Uniform - {uniformId}";
            editForm.Size = new System.Drawing.Size(400, 280);
            editForm.StartPosition = FormStartPosition.CenterParent;
            editForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            editForm.MaximizeBox = false;
            editForm.MinimizeBox = false;

            var lblUniformId = new Label { Text = "Uniform ID:", Location = new System.Drawing.Point(20, 20), Size = new System.Drawing.Size(100, 20) };
            var txtUniformId = new TextBox {
                Location = new System.Drawing.Point(130, 18),
                Size = new System.Drawing.Size(240, 20),
                Text = uniformId,
                ReadOnly = true,
                BackColor = System.Drawing.SystemColors.Control
            };

            var lblType = new Label { Text = "Type:", Location = new System.Drawing.Point(20, 60), Size = new System.Drawing.Size(100, 20) };
            var cmbType = new ComboBox {
                Location = new System.Drawing.Point(130, 58),
                Size = new System.Drawing.Size(240, 20),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbType.Items.AddRange(new object[] {
                "Concert Coat", "Drum Major Coat", "Hat", "Marching Coat",
                "Marching Shorts", "Marching Socks", "Pants"
            });

            var lblSize1 = new Label { Text = "Size:", Location = new System.Drawing.Point(20, 100), Size = new System.Drawing.Size(100, 20) };
            var txtSize1 = new TextBox { Location = new System.Drawing.Point(130, 98), Size = new System.Drawing.Size(100, 20) };
            var cmbSize1 = new ComboBox { Location = new System.Drawing.Point(130, 98), Size = new System.Drawing.Size(100, 20), DropDownStyle = ComboBoxStyle.DropDownList, Visible = false };
            cmbSize1.Items.AddRange(new object[] { "xs", "s", "m", "l", "xl" });

            var lblSize2 = new Label { Text = "Length:", Location = new System.Drawing.Point(240, 100), Size = new System.Drawing.Size(50, 20), Visible = false };
            var txtSize2 = new TextBox { Location = new System.Drawing.Point(290, 98), Size = new System.Drawing.Size(80, 20), Visible = false };
            var cmbSize2 = new ComboBox { Location = new System.Drawing.Point(290, 98), Size = new System.Drawing.Size(80, 20), DropDownStyle = ComboBoxStyle.DropDownList, Visible = false };
            cmbSize2.Items.AddRange(new object[] { "", "xs", "s", "m", "l", "xl" });

            // Different clothing types have different size formats, so adjust visible controls based on type
            cmbType.SelectedIndexChanged += (s, ev) => {
                int type = cmbType.SelectedIndex;

                txtSize1.Visible = true;
                cmbSize1.Visible = false;
                lblSize2.Visible = false;
                txtSize2.Visible = false;
                cmbSize2.Visible = false;

                if (type == 6) // Pants
                {
                    lblSize1.Text = "Width:";
                    lblSize2.Text = "Length:";
                    lblSize2.Visible = true;
                    txtSize2.Visible = true;
                }
                else if (type == 3) // Marching Coat
                {
                    lblSize1.Text = "Number Size:";
                    lblSize2.Text = "Sleeve:";
                    lblSize2.Visible = true;
                    txtSize2.Visible = true;
                }
                else if (type == 2) // Hat
                {
                    lblSize1.Text = "Size:";
                    txtSize1.Visible = false;
                    cmbSize1.Visible = true;
                    if (cmbSize1.SelectedIndex == -1) cmbSize1.SelectedIndex = 0;
                }
                else if (type == 0) // Concert Coat
                {
                    lblSize1.Text = "Number Size:";
                    lblSize2.Text = "Sleeve:";
                    lblSize2.Visible = true;
                    cmbSize2.Visible = true;
                    if (cmbSize2.SelectedIndex == -1) cmbSize2.SelectedIndex = 0;
                }
                else
                {
                    lblSize1.Text = "Size:";
                }
            };
            cmbType.SelectedIndex = uniformType;

            // Parse size string to populate text boxes
            if (uniformType == 6 && size.Contains("x"))
            {
                var parts = size.Split('x');
                txtSize1.Text = parts[0].Trim();
                if (parts.Length > 1) txtSize2.Text = parts[1].Trim();
            }
            else if (uniformType == 3 && size.Contains(" "))
            {
                var parts = size.Split(new[] { ' ' }, 2);
                txtSize1.Text = parts[0].Trim();
                if (parts.Length > 1) txtSize2.Text = parts[1].Trim();
            }
            else if (uniformType == 0 && size.Contains(" "))
            {
                var parts = size.Split(new[] { ' ' }, 2);
                txtSize1.Text = parts[0].Trim();
                if (parts.Length > 1) 
                {
                    string letter = parts[1].Trim().ToLower();
                    if (cmbSize2.Items.Contains(letter))
                        cmbSize2.SelectedItem = letter;
                }
            }
            else if (uniformType == 2)
            {
                string hatSize = size.ToLower();
                if (cmbSize1.Items.Contains(hatSize))
                    cmbSize1.SelectedItem = hatSize;
            }
            else
            {
                txtSize1.Text = size;
            }

            var btnSave = new Button
            {
                Text = "Save Changes",
                DialogResult = DialogResult.OK,
                Location = new System.Drawing.Point(200, 180),
                Size = new System.Drawing.Size(170, 35)
            };

            var btnCancel = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Location = new System.Drawing.Point(20, 180),
                Size = new System.Drawing.Size(150, 35)
            };

            editForm.Controls.AddRange(new Control[] {
                lblUniformId, txtUniformId,
                lblType, cmbType,
                lblSize1, txtSize1, cmbSize1,
                lblSize2, txtSize2, cmbSize2,
                btnSave, btnCancel
            });
            editForm.AcceptButton = btnSave;
            editForm.CancelButton = btnCancel;

            if (editForm.ShowDialog() == DialogResult.OK)
            {
                string finalSize = "";
                int type = cmbType.SelectedIndex;

                if (type == 2) // Hat
                {
                    finalSize = cmbSize1.Text;
                }
                else 
                {
                    finalSize = txtSize1.Text.Trim();
                    if (type == 6 && !string.IsNullOrWhiteSpace(txtSize2.Text))
                    {
                        finalSize += $"x{txtSize2.Text.Trim()}";
                    }
                    else if (type == 3 && !string.IsNullOrWhiteSpace(txtSize2.Text))
                    {
                        finalSize += $" {txtSize2.Text.Trim()}";
                    }
                    else if (type == 0 && !string.IsNullOrWhiteSpace(cmbSize2.Text))
                    {
                        finalSize += $" {cmbSize2.Text}";
                    }
                }

                await UpdateUniformAsync(uniformId, type, finalSize);
            }
        }

        private async Task UpdateUniformAsync(string uniformId, int uniformType, string size)
        {
            try
            {
                var request = new UpdateUniformRequest
                {
                    UniformIdentifier = uniformId,
                    UniformType = uniformType,
                    Size = size,
                    RequestingUserId = _currentUser!.UserId
                };

                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, MediaTypeHeaderValue.Parse("application/json"));

                var response = await httpClient.PutAsync($"{API_BASE_URL}/UpdateUniform", content);
                var jsonString = await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<UniformResponse>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (result?.Success == true)
                {
                    MessageBox.Show($"Uniform '{uniformId}' updated successfully!",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadUniformsAsync();
                }
                else
                {
                    MessageBox.Show($"Error updating uniform:\n\n{result?.Message ?? "Unknown error"}",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating uniform:\n\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void ButtonDeleteUniform_Click(object sender, EventArgs e)
        {
            await DeleteSelectedUniform();
        }

        private async void DeleteUniformToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await DeleteSelectedUniform();
        }

        private async Task DeleteSelectedUniform()
        {
            if (dataGridViewUniforms.SelectedRows.Count == 0) return;

            var selectedRow = dataGridViewUniforms.SelectedRows[0];
            string uniformId = selectedRow.Cells["UniformIdentifier"].Value.ToString()!;

            var confirmResult = MessageBox.Show(
                $"Are you sure you want to delete uniform '{uniformId}'?\n\n" +
                $"This action cannot be undone.",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirmResult != DialogResult.Yes) return;

            await DeleteUniformAsync(uniformId);
        }

        private async Task DeleteUniformAsync(string uniformId)
        {
            try
            {
                var response = await httpClient.DeleteAsync(
                    $"{API_BASE_URL}/uniforms/{uniformId}?userId={_currentUser!.UserId}");

                var jsonString = await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<UniformResponse>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (result?.Success == true)
                {
                    MessageBox.Show($"Uniform '{uniformId}' deleted successfully.", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadUniformsAsync();
                }
                else
                {
                    MessageBox.Show($"Error deleting uniform:\n\n{result?.Message ?? "Unknown error"}",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting uniform:\n\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void DeleteAllUniformsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_currentOrganization?.UserAccountLevel != 0)
            {
                MessageBox.Show("Only administrators can delete all uniforms.", "Insufficient Permissions", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirmResult = MessageBox.Show(
                $"Are you sure you want to delete ALL uniforms in {_currentOrganization.OrganizationName}?\n\n" +
                $"This action CANNOT be undone.",
                "Confirm Delete All",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirmResult != DialogResult.Yes) return;

            // Second confirmation
            if (!ConfirmActionWithOrganizationName("delete all uniforms")) return;

            try
            {
                var response = await httpClient.DeleteAsync(
                    $"{API_BASE_URL}/organizations/{_currentOrganization.OrganizationId}/uniforms?userId={_currentUser!.UserId}");

                var jsonString = await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<UniformResponse>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (result?.Success == true)
                {
                    MessageBox.Show(result.Message ?? "All uniforms deleted successfully.", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadUniformsAsync();
                }
                else
                {
                    MessageBox.Show($"Error deleting all uniforms:\n\n{result?.Message ?? "Unknown error"}",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting all uniforms:\n\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void ButtonAssignUniform_Click(object sender, EventArgs e)
        {
            await AssignSelectedUniform();
        }

        private async void AssignUniformToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await AssignSelectedUniform();
        }

        private async Task AssignSelectedUniform()
        {
            if (dataGridViewUniforms.SelectedRows.Count == 0) return;

            var selectedRow = dataGridViewUniforms.SelectedRows[0];
            string uniformId = selectedRow.Cells["UniformIdentifier"].Value.ToString()!;
            string? currentStudent = selectedRow.Cells["AssignedStudentId"].Value?.ToString();

            // Check if already assigned
            if (!string.IsNullOrEmpty(currentStudent))
            {
                MessageBox.Show(
                    $"Uniform '{uniformId}' is already assigned to student {currentStudent}.\n\n" +
                    "Please unassign it first if you want to reassign it to a different student.",
                    "Already Assigned",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            // Show student selection dialog
            using var assignForm = new Form();
            assignForm.Text = $"Assign Uniform - {uniformId}";
            assignForm.Size = new System.Drawing.Size(400, 200);
            assignForm.StartPosition = FormStartPosition.CenterParent;
            assignForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            assignForm.MaximizeBox = false;
            assignForm.MinimizeBox = false;

            var lblInfo = new Label
            {
                Text = "Select the student to assign this uniform to:",
                Location = new System.Drawing.Point(20, 20),
                Size = new System.Drawing.Size(350, 20)
            };

            var cmbStudent = new ComboBox
            {
                Location = new System.Drawing.Point(20, 50),
                Size = new System.Drawing.Size(350, 20),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            var studentsSource = (List<StudentDto>?)dataGridViewStudents.DataSource;
            if (studentsSource != null && studentsSource.Any())
            {
                foreach (var student in studentsSource)
                {
                    cmbStudent.Items.Add(new { Text = $"{student.StudentIdentifier} - {student.FullName}", Value = student.StudentIdentifier });
                }
                cmbStudent.DisplayMember = "Text";
                cmbStudent.ValueMember = "Value";
                cmbStudent.SelectedIndex = 0;
            }
            else
            {
                MessageBox.Show("No students found. Please add students first.",
                    "No Students", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var btnAssign = new Button
            {
                Text = "Assign Uniform",
                DialogResult = DialogResult.OK,
                Location = new System.Drawing.Point(200, 100),
                Size = new System.Drawing.Size(170, 35)
            };

            var btnCancel = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Location = new System.Drawing.Point(20, 100),
                Size = new System.Drawing.Size(150, 35)
            };

            assignForm.Controls.AddRange(new Control[] { lblInfo, cmbStudent, btnAssign, btnCancel });
            assignForm.AcceptButton = btnAssign;
            assignForm.CancelButton = btnCancel;

            if (assignForm.ShowDialog() == DialogResult.OK && cmbStudent.SelectedItem != null)
            {
                dynamic selectedItem = cmbStudent.SelectedItem;
                string studentId = selectedItem.Value;
                await AssignUniformAsync(uniformId, studentId);
            }
        }

        private async Task AssignUniformAsync(string uniformId, string studentId)
        {
            try
            {
                var request = new AssignUniformRequest
                {
                    UniformIdentifier = uniformId,
                    StudentId = studentId,
                    RequestingUserId = _currentUser!.UserId
                };

                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, MediaTypeHeaderValue.Parse("application/json"));

                var response = await httpClient.PostAsync($"{API_BASE_URL}/uniforms/assign", content);
                var jsonString = await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<UniformResponse>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (result?.Success == true)
                {
                    MessageBox.Show($"Uniform '{uniformId}' assigned to student {studentId} successfully!",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadUniformsAsync();
                }
                else
                {
                    MessageBox.Show($"Error assigning uniform:\n\n{result?.Message ?? "Unknown error"}",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error assigning uniform:\n\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void ButtonUnassignUniform_Click(object sender, EventArgs e)
        {
            await UnassignSelectedUniform();
        }

        private async void UnassignUniformToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await UnassignSelectedUniform();
        }

        private async Task UnassignSelectedUniform()
        {
            if (dataGridViewUniforms.SelectedRows.Count == 0) return;

            var selectedRow = dataGridViewUniforms.SelectedRows[0];
            string uniformId = selectedRow.Cells["UniformIdentifier"].Value.ToString()!
;
            string? currentStudent = selectedRow.Cells["AssignedStudentId"].Value?.ToString();
            bool isCheckedOut = Convert.ToBoolean(selectedRow.Cells["IsCheckedOut"].Value);

            // Check if assigned
            if (string.IsNullOrEmpty(currentStudent))
            {
                MessageBox.Show(
                    $"Uniform '{uniformId}' is not assigned to any student.",
                    "Not Assigned",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            // Warn if checked out
            string warningMessage = isCheckedOut
                ? $"Uniform '{uniformId}' is currently checked out to student {currentStudent}.\n\n" +
                  "Unassigning will also check in the uniform.\n\nContinue?"
                : $"Unassign uniform '{uniformId}' from student {currentStudent}?";

            var confirmResult = MessageBox.Show(
                warningMessage,
                "Confirm Unassign",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirmResult != DialogResult.Yes) return;

            await UnassignUniformAsync(uniformId);
        }

        private async Task UnassignUniformAsync(string uniformId)
        {
            try
            {
                var request = new UnassignUniformRequest
                {
                    UniformIdentifier = uniformId,
                    RequestingUserId = _currentUser!.UserId
                };

                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, MediaTypeHeaderValue.Parse("application/json"));

                var response = await httpClient.PostAsync($"{API_BASE_URL}/uniforms/unassign", content);
                var jsonString = await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<UniformResponse>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (result?.Success == true)
                {
                    MessageBox.Show($"Uniform '{uniformId}' unassigned successfully!",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadUniformsAsync();
                }
                else
                {
                    MessageBox.Show($"Error unassigning uniform:\n\n{result?.Message ?? "Unknown error"}",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error unassigning uniform:\n\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void ButtonCheckOutUniform_Click(object sender, EventArgs e)
        {
            await CheckOutSelectedUniform();
        }

        private async void CheckOutInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await CheckOutSelectedUniform();
        }

        private async Task CheckOutSelectedUniform()
        {
            if (dataGridViewUniforms.SelectedRows.Count == 0) return;

            var selectedRow = dataGridViewUniforms.SelectedRows[0];
            string uniformId = selectedRow.Cells["UniformIdentifier"].Value.ToString()!;
            bool isCheckedOut = Convert.ToBoolean(selectedRow.Cells["IsCheckedOut"].Value);
            string? currentStudent = selectedRow.Cells["AssignedStudentId"].Value?.ToString();

            // Check if uniform is assigned
            if (string.IsNullOrEmpty(currentStudent))
            {
                MessageBox.Show(
                    $"Uniform '{uniformId}' is not assigned to any student.\n\n" +
                    "Please assign the uniform to a student first before checking it out.\n\n" +
                    "(Admins can assign uniforms using the 'Assign to Student' button)",
                    "Cannot Check Out",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            // Show confirmation dialog
            string action = isCheckedOut ? "Check In" : "Check Out";
            string message = isCheckedOut
                ? $"Check in uniform '{uniformId}' from student {currentStudent}?\n\n" +
                  "The uniform will remain assigned to the student."
                : $"Check out uniform '{uniformId}' to student {currentStudent}?";

            var confirmResult = MessageBox.Show(
                message,
                action,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirmResult != DialogResult.Yes) return;

            await CheckOutUniformAsync(uniformId, !isCheckedOut);
        }

        private async Task CheckOutUniformAsync(string uniformId, bool checkOut)
        {
            try
            {
                var request = new CheckOutUniformRequest
                {
                    UniformIdentifier = uniformId,
                    CheckOut = checkOut,
                    RequestingUserId = _currentUser!.UserId
                };

                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, MediaTypeHeaderValue.Parse("application/json"));

                var response = await httpClient.PostAsync($"{API_BASE_URL}/uniforms/checkout", content);
                var jsonString = await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<UniformResponse>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (result?.Success == true)
                {
                    MessageBox.Show(result.Message, 
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadUniformsAsync();
                }
                else
                {
                    MessageBox.Show($"Error:\n\n{result?.Message ?? "Unknown error"}",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error:\n\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void ButtonUpdateConditions_Click(object sender, EventArgs e)
        {
            await UpdateSelectedUniformConditions();
        }

        private async void UpdateConditionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await UpdateSelectedUniformConditions();
        }

        private async Task UpdateSelectedUniformConditions()
        {
            if (dataGridViewUniforms.SelectedRows.Count == 0) return;

            var selectedRow = dataGridViewUniforms.SelectedRows[0];
            string uniformId = selectedRow.Cells["UniformIdentifier"].Value.ToString()!;

            using var conditionsForm = new Form();
            conditionsForm.Text = $"Update Conditions - {uniformId}";
            conditionsForm.Size = new System.Drawing.Size(400, 380);
            conditionsForm.StartPosition = FormStartPosition.CenterParent;
            conditionsForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            conditionsForm.MaximizeBox = false;
            conditionsForm.MinimizeBox = false;

            var lblInfo = new Label
            {
                Text = "Select all conditions that apply to this uniform:",
                Location = new System.Drawing.Point(20, 20),
                Size = new System.Drawing.Size(350, 20)
            };

            var chkStain = new CheckBox { Text = "Stain", Location = new System.Drawing.Point(20, 50), Size = new System.Drawing.Size(350, 25), Tag = 0 };
            var chkBrokenButton = new CheckBox { Text = "Broken Button", Location = new System.Drawing.Point(20, 80), Size = new System.Drawing.Size(350, 25), Tag = 1 };
            var chkBrokenZipper = new CheckBox { Text = "Broken Zipper", Location = new System.Drawing.Point(20, 110), Size = new System.Drawing.Size(350, 25), Tag = 2 };
            var chkTorn = new CheckBox { Text = "Torn", Location = new System.Drawing.Point(20, 140), Size = new System.Drawing.Size(350, 25), Tag = 3 };
            var chkMissing = new CheckBox { Text = "Missing", Location = new System.Drawing.Point(20, 170), Size = new System.Drawing.Size(350, 25), Tag = 4 };
            var chkFaded = new CheckBox { Text = "Faded", Location = new System.Drawing.Point(20, 200), Size = new System.Drawing.Size(350, 25), Tag = 5 };

            var btnSave = new Button
            {
                Text = "Save Conditions",
                DialogResult = DialogResult.OK,
                Location = new System.Drawing.Point(200, 270),
                Size = new System.Drawing.Size(170, 35)
            };

            var btnCancel = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Location = new System.Drawing.Point(20, 270),
                Size = new System.Drawing.Size(150, 35)
            };

            conditionsForm.Controls.AddRange(new Control[] { 
                lblInfo,
                chkStain, chkBrokenButton, chkBrokenZipper, 
                chkTorn, chkMissing, chkFaded,
                btnSave, btnCancel 
            });
            conditionsForm.AcceptButton = btnSave;
            conditionsForm.CancelButton = btnCancel;

            if (conditionsForm.ShowDialog() == DialogResult.OK)
            {
                var conditions = new List<int>();
                foreach (Control ctrl in conditionsForm.Controls)
                {
                    if (ctrl is CheckBox chk && chk.Checked && chk.Tag != null)
                    {
                        conditions.Add((int)chk.Tag);
                    }
                }

                await UpdateUniformConditionsAsync(uniformId, conditions.ToArray());
            }
        }

        private async Task UpdateUniformConditionsAsync(string uniformId, int[] conditions)
        {
            try
            {
                var request = new UpdateConditionsRequest
                {
                    UniformIdentifier = uniformId,
                    Conditions = conditions,
                    RequestingUserId = _currentUser!.UserId
                };

                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, MediaTypeHeaderValue.Parse("application/json"));

                var response = await httpClient.PutAsync($"{API_BASE_URL}/uniforms/conditions", content);
                var jsonString = await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<UniformResponse>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (result?.Success == true)
                {
                    MessageBox.Show($"Conditions for uniform '{uniformId}' updated successfully!", 
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadUniformsAsync();
                }
                else
                {
                    MessageBox.Show($"Error updating conditions:\n\n{result?.Message ?? "Unknown error"}",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating conditions:\n\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Student Management

        private async void ButtonAddStudent_Click(object sender, EventArgs e)
        {
            await AddNewStudent();
        }

        private async void AddStudentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await AddNewStudent();
        }

        private async Task AddNewStudent()
        {
            using var addForm = new Form();
            addForm.Text = "Add New Student";
            addForm.Size = new System.Drawing.Size(400, 300);
            addForm.StartPosition = FormStartPosition.CenterParent;
            addForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            addForm.MaximizeBox = false;
            addForm.MinimizeBox = false;

            var lblStudentId = new Label { Text = "Student ID:", Location = new System.Drawing.Point(20, 20), Size = new System.Drawing.Size(100, 20) };
            var txtStudentId = new TextBox { Location = new System.Drawing.Point(130, 18), Size = new System.Drawing.Size(240, 20), CharacterCasing = CharacterCasing.Upper };

            var lblFirstName = new Label { Text = "First Name:", Location = new System.Drawing.Point(20, 60), Size = new System.Drawing.Size(100, 20) };
            var txtFirstName = new TextBox { Location = new System.Drawing.Point(130, 58), Size = new System.Drawing.Size(240, 20) };

            var lblLastName = new Label { Text = "Last Name:", Location = new System.Drawing.Point(20, 100), Size = new System.Drawing.Size(100, 20) };
            var txtLastName = new TextBox { Location = new System.Drawing.Point(130, 98), Size = new System.Drawing.Size(240, 20) };

            var lblGrade = new Label { Text = "Grade:", Location = new System.Drawing.Point(20, 140), Size = new System.Drawing.Size(100, 20) };
            var numGrade = new NumericUpDown { Location = new System.Drawing.Point(130, 138), Size = new System.Drawing.Size(100, 20), Minimum = 1, Maximum = 12, Value = 9 };

            var btnCreate = new Button
            {
                Text = "Add Student",
                DialogResult = DialogResult.OK,
                Location = new System.Drawing.Point(200, 200),
                Size = new System.Drawing.Size(170, 35)
            };

            var btnCancel = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Location = new System.Drawing.Point(20, 200),
                Size = new System.Drawing.Size(150, 35)
            };

            addForm.Controls.AddRange(new Control[] {
                lblStudentId, txtStudentId,
                lblFirstName, txtFirstName,
                lblLastName, txtLastName,
                lblGrade, numGrade,
                btnCreate, btnCancel
            });
            addForm.AcceptButton = btnCreate;
            addForm.CancelButton = btnCancel;

            if (addForm.ShowDialog() == DialogResult.OK)
            {
                if (string.IsNullOrWhiteSpace(txtStudentId.Text) || 
                    string.IsNullOrWhiteSpace(txtFirstName.Text) || 
                    string.IsNullOrWhiteSpace(txtLastName.Text))
                {
                    MessageBox.Show("Please fill in all fields.", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                await CreateStudentAsync(
                    txtStudentId.Text.Trim(), 
                    txtFirstName.Text.Trim(), 
                    txtLastName.Text.Trim(), 
                    (int)numGrade.Value);
            }
        }

        private async Task CreateStudentAsync(string studentId, string firstName, string lastName, int grade)
        {
            try
            {
                var request = new CreateStudentRequest
                {
                    OrganizationId = _currentOrganization!.OrganizationId,
                    StudentIdentifier = studentId,
                    FirstName = firstName,
                    LastName = lastName,
                    Grade = grade,
                    RequestingUserId = _currentUser!.UserId
                };

                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, MediaTypeHeaderValue.Parse("application/json"));

                var response = await httpClient.PostAsync($"{API_BASE_URL}/CreateStudent", content);
                var jsonString = await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<StudentResponse>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (result?.Success == true)
                {
                    MessageBox.Show($"Student '{studentId}' added successfully!",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadStudentsAsync();
                }
                else
                {
                    MessageBox.Show($"Error adding student:\n\n{result?.Message ?? "Unknown error"}",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding student:\n\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void ButtonEditStudent_Click(object sender, EventArgs e)
        {
            await EditSelectedStudent();
        }

        private async void EditStudentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await EditSelectedStudent();
        }

        private async Task EditSelectedStudent()
        {
            if (dataGridViewStudents.SelectedRows.Count == 0) return;

            var selectedRow = dataGridViewStudents.SelectedRows[0];
            string studentId = selectedRow.Cells["StudentIdentifier"].Value.ToString()!;
            string firstName = selectedRow.Cells["FirstName"].Value.ToString()!;
            string lastName = selectedRow.Cells["LastName"].Value.ToString()!;
            int grade = Convert.ToInt32(selectedRow.Cells["Grade"].Value);

            using var editForm = new Form();
            editForm.Text = $"Edit Student - {studentId}";
            editForm.Size = new System.Drawing.Size(400, 300);
            editForm.StartPosition = FormStartPosition.CenterParent;
            editForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            editForm.MaximizeBox = false;
            editForm.MinimizeBox = false;

            var lblStudentId = new Label { Text = "Student ID:", Location = new System.Drawing.Point(20, 20), Size = new System.Drawing.Size(100, 20) };
            var txtStudentId = new TextBox {
                Location = new System.Drawing.Point(130, 18),
                Size = new System.Drawing.Size(240, 20),
                Text = studentId,
                ReadOnly = true,
                BackColor = System.Drawing.SystemColors.Control
            };

            var lblFirstName = new Label { Text = "First Name:", Location = new System.Drawing.Point(20, 60), Size = new System.Drawing.Size(100, 20) };
            var txtFirstName = new TextBox { 
                Location = new System.Drawing.Point(130, 58), 
                Size = new System.Drawing.Size(240, 20),
                Text = firstName
            };

            var lblLastName = new Label { Text = "Last Name:", Location = new System.Drawing.Point(20, 100), Size = new System.Drawing.Size(100, 20) };
            var txtLastName = new TextBox { 
                Location = new System.Drawing.Point(130, 98), 
                Size = new System.Drawing.Size(240, 20),
                Text = lastName
            };

            var lblGrade = new Label { Text = "Grade:", Location = new System.Drawing.Point(20, 140), Size = new System.Drawing.Size(100, 20) };
            var numGrade = new NumericUpDown { 
                Location = new System.Drawing.Point(130, 138), 
                Size = new System.Drawing.Size(100, 20), 
                Minimum = 1, 
                Maximum = 12, 
                Value = grade 
            };

            var btnSave = new Button
            {
                Text = "Save Changes",
                DialogResult = DialogResult.OK,
                Location = new System.Drawing.Point(200, 200),
                Size = new System.Drawing.Size(170, 35)
            };

            var btnCancel = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Location = new System.Drawing.Point(20, 200),
                Size = new System.Drawing.Size(150, 35)
            };

            editForm.Controls.AddRange(new Control[] {
                lblStudentId, txtStudentId,
                lblFirstName, txtFirstName,
                lblLastName, txtLastName,
                lblGrade, numGrade,
                btnSave, btnCancel
            });
            editForm.AcceptButton = btnSave;
            editForm.CancelButton = btnCancel;

            if (editForm.ShowDialog() == DialogResult.OK)
            {
                if (string.IsNullOrWhiteSpace(txtFirstName.Text) || string.IsNullOrWhiteSpace(txtLastName.Text))
                {
                    MessageBox.Show("First name and last name are required.", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                await UpdateStudentAsync(
                    studentId, 
                    txtFirstName.Text.Trim(), 
                    txtLastName.Text.Trim(), 
                    (int)numGrade.Value);
            }
        }

        private async Task UpdateStudentAsync(string studentId, string firstName, string lastName, int grade)
        {
            try
            {
                var request = new UpdateStudentRequest
                {
                    StudentIdentifier = studentId,
                    FirstName = firstName,
                    LastName = lastName,
                    Grade = grade,
                    RequestingUserId = _currentUser!.UserId
                };

                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, MediaTypeHeaderValue.Parse("application/json"));

                var response = await httpClient.PutAsync($"{API_BASE_URL}/UpdateStudent", content);
                var jsonString = await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<StudentResponse>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (result?.Success == true)
                {
                    MessageBox.Show($"Student '{studentId}' updated successfully!",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadStudentsAsync();
                }
                else
                {
                    MessageBox.Show($"Error updating student:\n\n{result?.Message ?? "Unknown error"}",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating student:\n\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void ButtonDeleteStudent_Click(object sender, EventArgs e)
        {
            await DeleteSelectedStudent();
        }

        private async void DeleteStudentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await DeleteSelectedStudent();
        }

        private async Task DeleteSelectedStudent()
        {
            if (dataGridViewStudents.SelectedRows.Count == 0) return;

            var selectedRow = dataGridViewStudents.SelectedRows[0];
            string studentId = selectedRow.Cells["StudentIdentifier"].Value.ToString()!;
            string fullName = selectedRow.Cells["FullName"].Value?.ToString() ?? 
                             $"{selectedRow.Cells["FirstName"].Value} {selectedRow.Cells["LastName"].Value}";

            var confirmResult = MessageBox.Show(
                $"Are you sure you want to delete student '{studentId}' ({fullName})?\n\n" +
                $"This action cannot be undone.\n\n" +
                $"Warning: Any uniforms assigned to this student will be unassigned.",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirmResult != DialogResult.Yes) return;

            await DeleteStudentAsync(studentId);
        }

        private async Task DeleteStudentAsync(string studentId)
        {
            try
            {
                var response = await httpClient.DeleteAsync(
                    $"{API_BASE_URL}/students/{studentId}?userId={_currentUser!.UserId}");

                var jsonString = await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<StudentResponse>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (result?.Success == true)
                {
                    MessageBox.Show($"Student '{studentId}' deleted successfully.", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadStudentsAsync();
                    // Refresh uniforms in case any were assigned to this student
                    await LoadUniformsAsync();
                }
                else
                {
                    MessageBox.Show($"Error deleting student:\n\n{result?.Message ?? "Unknown error"}",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting student:\n\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void DeleteAllStudentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_currentOrganization?.UserAccountLevel != 0)
            {
                MessageBox.Show("Only administrators can delete all students.", "Insufficient Permissions", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirmResult = MessageBox.Show(
                $"Are you sure you want to delete ALL students in {_currentOrganization.OrganizationName}?\n\n" +
                $"This action CANNOT be undone and will unassign all uniforms.",
                "Confirm Delete All",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirmResult != DialogResult.Yes) return;

            // Second confirmation
            if (!ConfirmActionWithOrganizationName("delete all students")) return;

            try
            {
                var response = await httpClient.DeleteAsync(
                    $"{API_BASE_URL}/organizations/{_currentOrganization.OrganizationId}/students?userId={_currentUser!.UserId}");

                var jsonString = await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<StudentResponse>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (result?.Success == true)
                {
                    MessageBox.Show(result.Message ?? "All students deleted successfully.", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadStudentsAsync();
                    await LoadUniformsAsync(); // unassigned uniforms
                }
                else
                {
                    MessageBox.Show($"Error deleting all students:\n\n{result?.Message ?? "Unknown error"}",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting all students:\n\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ConfirmActionWithOrganizationName(string actionDescription)
        {
            using var confirmForm = new Form();
            confirmForm.Text = "Final Confirmation";
            confirmForm.Size = new Size(400, 200);
            confirmForm.StartPosition = FormStartPosition.CenterParent;
            confirmForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            confirmForm.MaximizeBox = false;
            confirmForm.MinimizeBox = false;

            var lblInfo = new Label
            {
                Text = $"To confirm {actionDescription}, type the organization name\n({_currentOrganization!.OrganizationName}):",
                Location = new Point(20, 20),
                Size = new Size(350, 40)
            };

            var txtOrgName = new TextBox
            {
                Location = new Point(20, 70),
                Size = new Size(350, 25)
            };

            var btnConfirm = new Button
            {
                Text = "Confirm Delete",
                DialogResult = DialogResult.OK,
                Location = new Point(200, 110),
                Size = new Size(170, 35),
                Enabled = false
            };

            txtOrgName.TextChanged += (s, ev) =>
            {
                btnConfirm.Enabled = string.Equals(txtOrgName.Text.Trim(), _currentOrganization.OrganizationName, StringComparison.OrdinalIgnoreCase);
            };

            var btnCancel = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Location = new Point(20, 110),
                Size = new Size(150, 35)
            };

            confirmForm.Controls.AddRange(new Control[] { lblInfo, txtOrgName, btnConfirm, btnCancel });
            confirmForm.AcceptButton = btnConfirm;
            confirmForm.CancelButton = btnCancel;

            return confirmForm.ShowDialog() == DialogResult.OK;
        }

        #endregion

        #region Menu Event Handlers

        private void SwitchOrganizationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Close this form and return to organization selector
            var confirmResult = MessageBox.Show(
                "Switch to a different organization?\n\n" +
                "This will close the current view.",
                "Switch Organization",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirmResult == DialogResult.Yes)
            {
                this.DialogResult = DialogResult.Retry; // Signal to show org selector again
                this.Close();
            }
        }

        private void JoinOrganizationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using var joinForm = new JoinOrganizationForm(_currentUser!);
            if (joinForm.ShowDialog() == DialogResult.OK)
            {
                MessageBox.Show(
                    "Your request has been sent!\n\n" +
                    "Once approved by an administrator, you can switch to the new organization.",
                    "Request Sent",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        private void OrganizationInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_currentOrganization == null) return;

            using var infoForm = new OrganizationInfoForm(_currentOrganization);
            infoForm.ShowDialog();
        }

        private void ManageJoinRequestsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Only admins can manage join requests
            if (_currentOrganization?.UserAccountLevel != 0)
            {
                MessageBox.Show(
                    "Only administrators can manage join requests.",
                    "Insufficient Permissions",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (_currentUser == null || _currentOrganization == null) return;

            using var joinRequestsForm = new ManageJoinRequestsForm(_currentUser, _currentOrganization);
            joinRequestsForm.ShowDialog();
        }

        private void ManageUsersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Only admins can manage users
            if (_currentOrganization?.UserAccountLevel != 0)
            {
                MessageBox.Show(
                    "Only administrators can manage users.",
                    "Insufficient Permissions",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (_currentUser == null || _currentOrganization == null) return;

            // Open the user management window
            using var manageUsersForm = new ManageOrganizationUsersForm(_currentUser, _currentOrganization);
            if (manageUsersForm.ShowDialog() == DialogResult.OK)
            {
                // Refresh users list if needed
                LoadUsersAsync().ConfigureAwait(false);
            }
        }

        private void ImportUniformsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_currentOrganization?.UserAccountLevel != 0)
            {
                MessageBox.Show(
                    "Only administrators can import uniforms.",
                    "Insufficient Permissions",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ImportUniforms();
        }

        private async void ImportUniforms()
        {
            try
            {
                // Open file dialog
                using var openFileDialog = new OpenFileDialog
                {
                    Title = "Select Uniforms File to Import",
                    Filter = "CSV Files (*.csv)|*.csv|Excel Files (*.xlsx)|*.xlsx|All Files (*.*)|*.*",
                    FilterIndex = 1
                };

                if (openFileDialog.ShowDialog() != DialogResult.OK)
                    return;

                // Read the file
                DataTable importData;
                try
                {
                    importData = ReadSpreadsheetFile(openFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error reading file:\n\n{ex.Message}",
                        "File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (importData.Rows.Count == 0)
                {
                    MessageBox.Show("The file contains no data to import.",
                        "Empty File", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Show column mapping form
                using var mappingForm = new ImportColumnMappingForm(importData, ImportColumnMappingForm.ImportType.Uniforms);
                if (mappingForm.ShowDialog() != DialogResult.OK)
                    return;

                // Process import
                await ProcessUniformImport(importData, mappingForm.ColumnMapping);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during import:\n\n{ex.Message}",
                    "Import Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task ProcessUniformImport(DataTable data, Dictionary<string, string> columnMapping)
        {
            int successCount = 0;
            int errorCount = 0;
            var errors = new List<string>();

            // Create progress form
            using var progressForm = new Form
            {
                Text = "Importing Uniforms",
                Size = new Size(400, 150),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                ControlBox = false
            };

            var progressBar = new ProgressBar
            {
                Location = new Point(20, 20),
                Size = new Size(350, 30),
                Maximum = data.Rows.Count
            };

            var lblStatus = new Label
            {
                Location = new Point(20, 60),
                Size = new Size(350, 40),
                Text = "Starting import..."
            };

            progressForm.Controls.AddRange(new Control[] { progressBar, lblStatus });
            progressForm.Show();

            try
            {
                for (int i = 0; i < data.Rows.Count; i++)
                {
                    var row = data.Rows[i];
                    progressBar.Value = i + 1;
                    lblStatus.Text = $"Processing record {i + 1} of {data.Rows.Count}...";
                    Application.DoEvents();

                    try
                    {
                        // Extract data from row
                        string uniformId = GetRowValue(row, columnMapping, "UniformIdentifier")?.Trim() ?? "";
                        string typeStr = GetRowValue(row, columnMapping, "UniformType")?.Trim() ?? "";
                        string sizeStr = GetRowValue(row, columnMapping, "Size")?.Trim() ?? "";

                        if (string.IsNullOrWhiteSpace(uniformId))
                        {
                            errors.Add($"Row {i + 1}: Missing Uniform ID");
                            errorCount++;
                            continue;
                        }

                        // Parse uniform type
                        int uniformType = ParseUniformType(typeStr);
                        if (uniformType == -1)
                        {
                            errors.Add($"Row {i + 1} ({uniformId}): Invalid uniform type '{typeStr}'");
                            errorCount++;
                            continue;
                        }

                        // Validate size
                        if (string.IsNullOrWhiteSpace(sizeStr))
                        {
                            errors.Add($"Row {i + 1} ({uniformId}): Missing size");
                            errorCount++;
                            continue;
                        }

                        // Create uniform via API
                        var result = await CreateUniformViaImport(uniformId, uniformType, sizeStr);
                        if (result)
                        {
                            successCount++;
                        }
                        else
                        {
                            errors.Add($"Row {i + 1} ({uniformId}): Failed to create (may already exist)");
                            errorCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Row {i + 1}: {ex.Message}");
                        errorCount++;
                    }
                }
            }
            finally
            {
                progressForm.Close();
            }

            // Refresh the uniforms list
            await LoadUniformsAsync();

            // Show results
            var resultMessage = $"Import completed!\n\n" +
                               $"✓ Successfully imported: {successCount}\n" +
                               $"✗ Errors: {errorCount}";

            if (errors.Any() && errors.Count <= 10)
            {
                resultMessage += $"\n\nErrors:\n{string.Join("\n", errors)}";
            }
            else if (errors.Count > 10)
            {
                resultMessage += $"\n\nShowing first 10 errors:\n{string.Join("\n", errors.Take(10))}\n... and {errors.Count - 10} more";
            }

            MessageBox.Show(resultMessage, "Import Results",
                MessageBoxButtons.OK,
                errorCount == 0 ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
        }

        private async Task<bool> CreateUniformViaImport(string uniformId, int uniformType, string size)
        {
            try
            {
                var request = new CreateUniformRequest
                {
                    OrganizationId = _currentOrganization!.OrganizationId,
                    UniformIdentifier = uniformId.ToUpper(),
                    UniformType = uniformType,
                    Size = size,
                    RequestingUserId = _currentUser!.UserId
                };

                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, MediaTypeHeaderValue.Parse("application/json"));

                var response = await httpClient.PostAsync($"{API_BASE_URL}/CreateUniform", content);
                var jsonString = await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<UniformResponse>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return result?.Success == true;
            }
            catch
            {
                return false;
            }
        }

        private int ParseUniformType(string typeStr)
        {
            if (string.IsNullOrWhiteSpace(typeStr))
                return -1;

            typeStr = typeStr.Trim().ToLower().Replace(" ", "").Replace("-", "");

            return typeStr switch
            {
                "concertcoat" or "concert" or "0" => 0,
                "drummajorcoat" or "drummajor" or "dm" or "1" => 1,
                "hat" or "2" => 2,
                "marchingcoat" or "marching" or "coat" or "3" => 3,
                "marchingshorts" or "shorts" or "4" => 4,
                "marchingsocks" or "socks" or "5" => 5,
                "pants" or "6" => 6,
                _ => -1
            };
        }

        private void ImportStudentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_currentOrganization?.UserAccountLevel != 0)
            {
                MessageBox.Show(
                    "Only administrators can import students.",
                    "Insufficient Permissions",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            ImportStudents();
        }

        private async void ImportStudents()
        {
            try
            {
                // Open file dialog
                using var openFileDialog = new OpenFileDialog
                {
                    Title = "Select Students File to Import",
                    Filter = "CSV Files (*.csv)|*.csv|Excel Files (*.xlsx)|*.xlsx|All Files (*.*)|*.*",
                    FilterIndex = 1
                };

                if (openFileDialog.ShowDialog() != DialogResult.OK)
                    return;

                // Read the file
                DataTable importData;
                try
                {
                    importData = ReadSpreadsheetFile(openFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error reading file:\n\n{ex.Message}",
                        "File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (importData.Rows.Count == 0)
                {
                    MessageBox.Show("The file contains no data to import.",
                        "Empty File", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Show column mapping form
                using var mappingForm = new ImportColumnMappingForm(importData, ImportColumnMappingForm.ImportType.Students);
                if (mappingForm.ShowDialog() != DialogResult.OK)
                    return;

                // Process import
                await ProcessStudentImport(importData, mappingForm.ColumnMapping);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during import:\n\n{ex.Message}",
                    "Import Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task ProcessStudentImport(DataTable data, Dictionary<string, string> columnMapping)
        {
            int successCount = 0;
            int errorCount = 0;
            var errors = new List<string>();

            // Create progress form
            using var progressForm = new Form
            {
                Text = "Importing Students",
                Size = new Size(400, 150),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                ControlBox = false
            };

            var progressBar = new ProgressBar
            {
                Location = new Point(20, 20),
                Size = new Size(350, 30),
                Maximum = data.Rows.Count
            };

            var lblStatus = new Label
            {
                Location = new Point(20, 60),
                Size = new Size(350, 40),
                Text = "Starting import..."
            };

            progressForm.Controls.AddRange(new Control[] { progressBar, lblStatus });
            progressForm.Show();

            try
            {
                for (int i = 0; i < data.Rows.Count; i++)
                {
                    var row = data.Rows[i];
                    progressBar.Value = i + 1;
                    lblStatus.Text = $"Processing record {i + 1} of {data.Rows.Count}...";
                    Application.DoEvents();

                    try
                    {
                        // Extract data from row
                        string studentId = GetRowValue(row, columnMapping, "StudentIdentifier")?.Trim() ?? "";
                        string firstName = GetRowValue(row, columnMapping, "FirstName")?.Trim() ?? "";
                        string lastName = GetRowValue(row, columnMapping, "LastName")?.Trim() ?? "";
                        string gradeStr = GetRowValue(row, columnMapping, "Grade")?.Trim() ?? "";

                        if (string.IsNullOrWhiteSpace(studentId))
                        {
                            errors.Add($"Row {i + 1}: Missing Student ID");
                            errorCount++;
                            continue;
                        }

                        if (string.IsNullOrWhiteSpace(firstName))
                        {
                            errors.Add($"Row {i + 1} ({studentId}): Missing First Name");
                            errorCount++;
                            continue;
                        }

                        if (string.IsNullOrWhiteSpace(lastName))
                        {
                            errors.Add($"Row {i + 1} ({studentId}): Missing Last Name");
                            errorCount++;
                            continue;
                        }

                        // Parse grade with flexible formatting
                        int grade = ParseGrade(gradeStr);
                        if (grade == -1)
                        {
                            errors.Add($"Row {i + 1} ({studentId}): Invalid grade '{gradeStr}'");
                            errorCount++;
                            continue;
                        }

                        // Create student via API
                        var result = await CreateStudentViaImport(studentId, firstName, lastName, grade);
                        if (result)
                        {
                            successCount++;
                        }
                        else
                        {
                            errors.Add($"Row {i + 1} ({studentId}): Failed to create (may already exist)");
                            errorCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Row {i + 1}: {ex.Message}");
                        errorCount++;
                    }
                }
            }
            finally
            {
                progressForm.Close();
            }

            // Refresh the students list
            await LoadStudentsAsync();

            // Show results
            var resultMessage = $"Import completed!\n\n" +
                               $"✓ Successfully imported: {successCount}\n" +
                               $"✗ Errors: {errorCount}";

            if (errors.Any() && errors.Count <= 10)
            {
                resultMessage += $"\n\nErrors:\n{string.Join("\n", errors)}";
            }
            else if (errors.Count > 10)
            {
                resultMessage += $"\n\nShowing first 10 errors:\n{string.Join("\n", errors.Take(10))}\n... and {errors.Count - 10} more";
            }

            MessageBox.Show(resultMessage, "Import Results",
                MessageBoxButtons.OK,
                errorCount == 0 ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
        }

        private int ParseGrade(string gradeStr)
        {
            if (string.IsNullOrWhiteSpace(gradeStr))
                return -1;

            // Remove common words and normalize
            gradeStr = gradeStr.Trim().ToLower()
                .Replace("grade", "")
                .Replace("th", "")
                .Replace("st", "")
                .Replace("nd", "")
                .Replace("rd", "")
                .Replace(" ", "")
                .Trim();

            // Try direct integer parse
            if (int.TryParse(gradeStr, out int grade))
            {
                if (grade >= 1 && grade <= 12)
                    return grade;
            }

            // Try word-to-number conversion
            grade = gradeStr switch
            {
                "first" or "one" or "1" => 1,
                "second" or "two" or "2" => 2,
                "third" or "three" or "3" => 3,
                "fourth" or "four" or "4" => 4,
                "fifth" or "five" or "5" => 5,
                "sixth" or "six" or "6" => 6,
                "seventh" or "seven" or "7" => 7,
                "eighth" or "eight" or "8" => 8,
                "ninth" or "nine" or "9" => 9,
                "tenth" or "ten" or "10" => 10,
                "eleventh" or "eleven" or "11" => 11,
                "twelfth" or "twelve" or "12" => 12,
                "k" or "kindergarten" or "0" => 0, // Optional: support kindergarten
                "freshman" or "freshmen" => 9,
                "sophomore" or "sophomores" => 10,
                "junior" or "juniors" => 11,
                "senior" or "seniors" => 12,
                _ => -1
            };

            // Validate range (1-12, or 0 if kindergarten is supported)
            if (grade >= 1 && grade <= 12)
                return grade;

            return -1;
        }

        private async Task<bool> CreateStudentViaImport(string studentId, string firstName, string lastName, int grade)
        {
            try
            {
                var request = new CreateStudentRequest
                {
                    OrganizationId = _currentOrganization!.OrganizationId,
                    StudentIdentifier = studentId.ToUpper(),
                    FirstName = firstName,
                    LastName = lastName,
                    Grade = grade,
                    RequestingUserId = _currentUser!.UserId
                };

                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, MediaTypeHeaderValue.Parse("application/json"));

                var response = await httpClient.PostAsync($"{API_BASE_URL}/CreateStudent", content);
                var jsonString = await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<StudentResponse>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return result?.Success == true;
            }
            catch
            {
                return false;
            }
        }

        private DataTable ReadSpreadsheetFile(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLower();
            
            if (extension == ".csv")
            {
                return ReadCsvFile(filePath);
            }
            else if (extension == ".xlsx" || extension == ".xls")
            {
                return ReadExcelFile(filePath);
            }
            else
            {
                throw new NotSupportedException("File format not supported. Please use CSV or Excel files.");
            }
        }

        private DataTable ReadCsvFile(string filePath)
        {
            var dataTable = new DataTable();
            var lines = File.ReadAllLines(filePath);

            if (lines.Length == 0)
                return dataTable;

            // Parse header
            var headers = ParseCsvLine(lines[0]);
            foreach (var header in headers)
            {
                dataTable.Columns.Add(header.Trim());
            }

            // Parse data rows
            for (int i = 1; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i]))
                    continue;

                var values = ParseCsvLine(lines[i]);
                var row = dataTable.NewRow();
                
                for (int j = 0; j < Math.Min(values.Length, dataTable.Columns.Count); j++)
                {
                    row[j] = values[j];
                }
                
                dataTable.Rows.Add(row);
            }

            return dataTable;
        }

        private string[] ParseCsvLine(string line)
        {
            var result = new List<string>();
            var currentValue = new StringBuilder();
            bool inQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == ',' && !inQuotes)
                {
                    result.Add(currentValue.ToString());
                    currentValue.Clear();
                }
                else
                {
                    currentValue.Append(c);
                }
            }

            result.Add(currentValue.ToString());
            return result.ToArray();
        }

        private DataTable ReadExcelFile(string filePath)
        {
            // For Excel files, we need a library like EPPlus or ClosedXML
            // For simplicity, I'll provide a basic implementation that tells users to convert to CSV
            throw new NotSupportedException(
                "Excel file support requires additional libraries.\n\n" +
                "Please save your Excel file as CSV format and try again:\n" +
                "1. Open the file in Excel\n" +
                "2. File → Save As\n" +
                "3. Choose 'CSV (Comma delimited) (*.csv)'\n" +
                "4. Import the CSV file");
        }

        private string? GetRowValue(DataRow row, Dictionary<string, string> columnMapping, string fieldName)
        {
            if (columnMapping.TryGetValue(fieldName, out var columnName))
            {
                if (row.Table.Columns.Contains(columnName))
                {
                    return row[columnName]?.ToString();
                }
            }
            return null;
        }
        #endregion
    }
}