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
                        buttonCheckOutUniform.Visible = true;
                        buttonUpdateConditions.Visible = true;
                        
                        // Show all context menu items
                        addUniformToolStripMenuItem.Visible = true;
                        editUniformToolStripMenuItem.Visible = true;
                        deleteUniformToolStripMenuItem.Visible = true;
                        toolStripSeparator2.Visible = true;
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
                        buttonCheckOutUniform.Visible = true;
                        buttonUpdateConditions.Visible = true;
                        
                        // Hide admin context menu items, show only user items
                        addUniformToolStripMenuItem.Visible = false;
                        editUniformToolStripMenuItem.Visible = false;
                        deleteUniformToolStripMenuItem.Visible = false;
                        toolStripSeparator2.Visible = false;
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
            cmbType.SelectedIndex = 0;

            var lblSize = new Label { Text = "Size:", Location = new System.Drawing.Point(20, 100), Size = new System.Drawing.Size(100, 20) };
            var numSize = new NumericUpDown { Location = new System.Drawing.Point(130, 98), Size = new System.Drawing.Size(100, 20), Minimum = 1, Maximum = 100, Value = 40 };

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
                lblSize, numSize,
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

                await CreateUniformAsync(txtUniformId.Text.Trim(), cmbType.SelectedIndex, (int)numSize.Value);
            }
        }

        private async Task CreateUniformAsync(string uniformId, int uniformType, int size)
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
            int size = Convert.ToInt32(selectedRow.Cells["Size"].Value);

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
            cmbType.SelectedIndex = uniformType;

            var lblSize = new Label { Text = "Size:", Location = new System.Drawing.Point(20, 100), Size = new System.Drawing.Size(100, 20) };
            var numSize = new NumericUpDown { Location = new System.Drawing.Point(130, 98), Size = new System.Drawing.Size(100, 20), Minimum = 1, Maximum = 100, Value = size };

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
                lblSize, numSize,
                btnSave, btnCancel 
            });
            editForm.AcceptButton = btnSave;
            editForm.CancelButton = btnCancel;

            if (editForm.ShowDialog() == DialogResult.OK)
            {
                await UpdateUniformAsync(uniformId, cmbType.SelectedIndex, (int)numSize.Value);
            }
        }

        private async Task UpdateUniformAsync(string uniformId, int uniformType, int size)
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

            using var checkOutForm = new Form();
            checkOutForm.Text = isCheckedOut ? $"Check In - {uniformId}" : $"Check Out - {uniformId}";
            checkOutForm.Size = new System.Drawing.Size(400, 240);
            checkOutForm.StartPosition = FormStartPosition.CenterParent;
            checkOutForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            checkOutForm.MaximizeBox = false;
            checkOutForm.MinimizeBox = false;

            var lblInfo = new Label
            {
                Text = isCheckedOut 
                    ? $"Currently checked out to: {currentStudent ?? "Unknown"}\n\nCheck in this uniform?"
                    : "Select a student to check out this uniform:",
                Location = new System.Drawing.Point(20, 20),
                Size = new System.Drawing.Size(350, 60),
                AutoSize = false
            };

            ComboBox? cmbStudent = null;
            if (!isCheckedOut)
            {
                cmbStudent = new ComboBox
                {
                    Location = new System.Drawing.Point(20, 85),
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
            }

            var btnAction = new Button
            {
                Text = isCheckedOut ? "Check In" : "Check Out",
                DialogResult = DialogResult.OK,
                Location = new System.Drawing.Point(200, 140),
                Size = new System.Drawing.Size(170, 35)
            };

            var btnCancel = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Location = new System.Drawing.Point(20, 140),
                Size = new System.Drawing.Size(150, 35)
            };

            checkOutForm.Controls.Add(lblInfo);
            if (cmbStudent != null) checkOutForm.Controls.Add(cmbStudent);
            checkOutForm.Controls.AddRange(new Control[] { btnAction, btnCancel });
            checkOutForm.AcceptButton = btnAction;
            checkOutForm.CancelButton = btnCancel;

            if (checkOutForm.ShowDialog() == DialogResult.OK)
            {
                string? studentId = null;
                if (!isCheckedOut && cmbStudent != null && cmbStudent.SelectedItem != null)
                {
                    dynamic selectedItem = cmbStudent.SelectedItem;
                    studentId = selectedItem.Value;
                }

                await CheckOutUniformAsync(uniformId, studentId, !isCheckedOut);
            }
        }

        private async Task CheckOutUniformAsync(string uniformId, string? studentId, bool checkOut)
        {
            try
            {
                var request = new CheckOutUniformRequest
                {
                    UniformIdentifier = uniformId,
                    StudentId = studentId,
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
                    string action = checkOut ? "checked out" : "checked in";
                    MessageBox.Show($"Uniform '{uniformId}' {action} successfully!", 
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

            var lblGrade = new Label { Text = "Grade (1-12):", Location = new System.Drawing.Point(20, 140), Size = new System.Drawing.Size(100, 20) };
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
                    MessageBox.Show("Please fill in all required fields.", "Validation Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                await CreateStudentAsync(txtStudentId.Text.Trim(), txtFirstName.Text.Trim(), 
                    txtLastName.Text.Trim(), (int)numGrade.Value);
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
                    MessageBox.Show($"Student '{firstName} {lastName}' added successfully!", 
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
            editForm.Text = $"Edit Student - {firstName} {lastName}";
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
            var txtFirstName = new TextBox { Location = new System.Drawing.Point(130, 58), Size = new System.Drawing.Size(240, 20), Text = firstName };

            var lblLastName = new Label { Text = "Last Name:", Location = new System.Drawing.Point(20, 100), Size = new System.Drawing.Size(100, 20) };
            var txtLastName = new TextBox { Location = new System.Drawing.Point(130, 98), Size = new System.Drawing.Size(240, 20), Text = lastName };

            var lblGrade = new Label { Text = "Grade (1-12):", Location = new System.Drawing.Point(20, 140), Size = new System.Drawing.Size(100, 20) };
            var numGrade = new NumericUpDown { Location = new System.Drawing.Point(130, 138), Size = new System.Drawing.Size(100, 20), Minimum = 1, Maximum = 12, Value = grade };

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

                await UpdateStudentAsync(studentId, txtFirstName.Text.Trim(), txtLastName.Text.Trim(), (int)numGrade.Value);
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
                    MessageBox.Show($"Student '{firstName} {lastName}' updated successfully!", 
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
            string studentId = selectedRow.Cells["StudentIdentifier"].Value.ToString()!
;
            string firstName = selectedRow.Cells["FirstName"].Value.ToString()!;
            string lastName = selectedRow.Cells["LastName"].Value.ToString()!;

            var confirmResult = MessageBox.Show(
                $"Are you sure you want to delete student '{firstName} {lastName}' (ID: {studentId})?\n\n" +
                $"This action cannot be undone.\n" +
                $"Any uniforms assigned to this student will be unassigned.",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirmResult != DialogResult.Yes) return;

            await DeleteStudentAsync(studentId, $"{firstName} {lastName}");
        }

        private async Task DeleteStudentAsync(string studentId, string studentName)
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
                    MessageBox.Show($"Student '{studentName}' deleted successfully.", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadStudentsAsync();
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

        #endregion

        #region Menu Event Handlers

        private void SwitchOrganizationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            var orgSelector = new OrganizationSelectorForm(_currentUser!);
            if (orgSelector.ShowDialog() == DialogResult.OK && orgSelector.SelectedOrganization != null)
            {
                _currentOrganization = orgSelector.SelectedOrganization;
                this.Text = $"Uniform Manager - {_currentOrganization.OrganizationName} - {_currentUser!.FirstName} {_currentUser.LastName} ({GetAccountLevelText(_currentOrganization.UserAccountLevel)})";
                LoadAllData().Wait();
                this.Show();
            }
            else
            {
                this.Close();
            }
        }

        private void JoinOrganizationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var joinForm = new JoinOrganizationForm(_currentUser!);
            if (joinForm.ShowDialog() == DialogResult.OK)
            {
                MessageBox.Show("You can switch to the new organization from the Organization menu.", 
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void OrganizationInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string info = $"Organization: {_currentOrganization!.OrganizationName}\n" +
                         $"Code: {_currentOrganization.OrganizationCode}\n" +
                         $"Your Role: {GetAccountLevelText(_currentOrganization.UserAccountLevel)}";
            
            if (!string.IsNullOrEmpty(_currentOrganization.Description))
            {
                info += $"\n\nDescription:\n{_currentOrganization.Description}";
            }
            
            MessageBox.Show(info, "Organization Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ManageJoinRequestsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_currentOrganization?.UserAccountLevel != 0)
            {
                MessageBox.Show("Only administrators can manage join requests.", 
                    "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            var requestsForm = new ManageJoinRequestsForm(_currentUser!, _currentOrganization);
            requestsForm.ShowDialog();
        }

        private void ManageUsersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_currentOrganization?.UserAccountLevel != 0)
            {
                MessageBox.Show("Only administrators can manage users.", 
                    "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            var usersForm = new ManageOrganizationUsersForm(_currentUser!, _currentOrganization);
            usersForm.ShowDialog();
            LoadUsersAsync().Wait();
        }

        #endregion

        #region Import Functionality

        private void ImportUniformsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_currentOrganization?.UserAccountLevel != 0)
            {
                MessageBox.Show("Only administrators can import data.", 
                    "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ImportData(ImportColumnMappingForm.ImportType.Uniforms);
        }

        private void ImportStudentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_currentOrganization?.UserAccountLevel != 0)
            {
                MessageBox.Show("Only administrators can import data.", 
                    "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ImportData(ImportColumnMappingForm.ImportType.Students);
        }

        private void ImportData(ImportColumnMappingForm.ImportType importType)
        {
            using var openFileDialog = new OpenFileDialog
            {
                Title = $"Select {importType} Spreadsheet",
                Filter = "Excel Files (*.xlsx;*.xls)|*.xlsx;*.xls|CSV Files (*.csv)|*.csv|All Files (*.*)|*.*",
                FilterIndex = 1
            };

            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return;

            try
            {
                // Load the spreadsheet
                var data = LoadSpreadsheet(openFileDialog.FileName);

                if (data == null || data.Rows.Count == 0)
                {
                    MessageBox.Show("The file is empty or could not be read.", 
                        "Import Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Show column mapping form
                using var mappingForm = new ImportColumnMappingForm(data, importType);
                if (mappingForm.ShowDialog() != DialogResult.OK)
                    return;

                // Process the import
                ProcessImport(mappingForm.ImportData, mappingForm.ColumnMapping, importType);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error importing file:\n\n{ex.Message}", 
                    "Import Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private System.Data.DataTable? LoadSpreadsheet(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLower();

            if (extension == ".csv")
            {
                return LoadCsvFile(filePath);
            }
            else if (extension == ".xlsx" || extension == ".xls")
            {
                return LoadExcelFile(filePath);
            }

            throw new NotSupportedException("File type not supported. Please use CSV or Excel files.");
        }

        private System.Data.DataTable LoadCsvFile(string filePath)
        {
            var dataTable = new System.Data.DataTable();

            using var reader = new StreamReader(filePath);
            
            // Read header
            var headerLine = reader.ReadLine();
            if (string.IsNullOrEmpty(headerLine))
                return dataTable;

            var headers = headerLine.Split(',');
            foreach (var header in headers)
            {
                dataTable.Columns.Add(header.Trim('"', ' '));
            }

            // Read data (limit to 1000 rows for safety)
            int rowCount = 0;
            while (!reader.EndOfStream && rowCount < 1000)
            {
                var line = reader.ReadLine();
                if (string.IsNullOrEmpty(line))
                    continue;

                var values = ParseCsvLine(line);
                if (values.Length == headers.Length)
                {
                    dataTable.Rows.Add(values);
                    rowCount++;
                }
            }

            return dataTable;
        }

        private string[] ParseCsvLine(string line)
        {
            var values = new List<string>();
            bool inQuotes = false;
            var currentValue = new StringBuilder();

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == ',' && !inQuotes)
                {
                    values.Add(currentValue.ToString().Trim());
                    currentValue.Clear();
                }
                else
                {
                    currentValue.Append(c);
                }
            }

            values.Add(currentValue.ToString().Trim());
            return values.ToArray();
        }

        private System.Data.DataTable LoadExcelFile(string filePath)
        {
            // For Excel support, you would need to install a package like EPPlus or ClosedXML
            // For now, show a message that Excel support requires additional setup
            MessageBox.Show(
                "Excel file support requires additional libraries.\n\n" +
                "Please save your Excel file as CSV and import it instead.\n\n" +
                "To add Excel support:\n" +
                "1. Install EPPlus or ClosedXML NuGet package\n" +
                "2. Uncomment the Excel loading code",
                "Excel Support",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            return new System.Data.DataTable();

            // Uncomment this code after installing EPPlus NuGet package:
            /*
            using var package = new OfficeOpenXml.ExcelPackage(new FileInfo(filePath));
            var worksheet = package.Workbook.Worksheets[0];
            var dataTable = new System.Data.DataTable();

            // Get headers from first row
            for (int col = 1; col <= worksheet.Dimension.Columns; col++)
            {
                dataTable.Columns.Add(worksheet.Cells[1, col].Value?.ToString() ?? $"Column{col}");
            }

            // Get data rows (limit to 1000 rows)
            int maxRows = Math.Min(worksheet.Dimension.Rows, 1000);
            for (int row = 2; row <= maxRows; row++)
            {
                var dataRow = dataTable.NewRow();
                for (int col = 1; col <= worksheet.Dimension.Columns; col++)
                {
                    dataRow[col - 1] = worksheet.Cells[row, col].Value?.ToString() ?? string.Empty;
                }
                dataTable.Rows.Add(dataRow);
            }

            return dataTable;
            */
        }

        private async void ProcessImport(System.Data.DataTable data, Dictionary<string, string> columnMapping, ImportColumnMappingForm.ImportType importType)
        {
            if (importType == ImportColumnMappingForm.ImportType.Uniforms)
            {
                await ProcessUniformImport(data, columnMapping);
            }
            else
            {
                await ProcessStudentImport(data, columnMapping);
            }
        }

        private async Task ProcessUniformImport(System.Data.DataTable data, Dictionary<string, string> columnMapping)
        {
            var successCount = 0;
            var errorCount = 0;
            var errors = new List<string>();

            var progressForm = new Form
            {
                Text = "Importing Uniforms",
                Size = new Size(400, 150),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            var progressLabel = new Label
            {
                Text = "Processing records...",
                Location = new Point(20, 20),
                Size = new Size(360, 20)
            };

            var progressBar = new ProgressBar
            {
                Location = new Point(20, 50),
                Size = new Size(360, 23),
                Maximum = data.Rows.Count
            };

            progressForm.Controls.Add(progressLabel);
            progressForm.Controls.Add(progressBar);
            progressForm.Show();

            try
            {
                for (int i = 0; i < data.Rows.Count; i++)
                {
                    var row = data.Rows[i];
                    progressLabel.Text = $"Processing record {i + 1} of {data.Rows.Count}...";
                    progressBar.Value = i + 1;
                    Application.DoEvents();

                    try
                    {
                        var uniformId = row[columnMapping["UniformIdentifier"]].ToString()?.Trim().ToUpper();
                        var typeStr = row[columnMapping["UniformType"]].ToString()?.Trim();
                        var sizeStr = row[columnMapping["Size"]].ToString()?.Trim();

                        if (string.IsNullOrEmpty(uniformId) || string.IsNullOrEmpty(typeStr) || string.IsNullOrEmpty(sizeStr))
                        {
                            errors.Add($"Row {i + 1}: Missing required fields");
                            errorCount++;
                            continue;
                        }

                        // Parse uniform type
                        int uniformType = ParseUniformType(typeStr);
                        if (!int.TryParse(sizeStr, out int size))
                        {
                            errors.Add($"Row {i + 1}: Invalid size '{sizeStr}'");
                            errorCount++;
                            continue;
                        }

                        // Create the uniform silently (no message boxes)
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
                            successCount++;
                        }
                        else
                        {
                            errors.Add($"Row {i + 1} ({uniformId}): {result?.Message ?? "Unknown error"}");
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

            // Show results
            var message = $"Import completed!\n\n" +
                         $"✓ Successfully imported: {successCount}\n" +
                         $"✗ Errors: {errorCount}";

            if (errors.Any())
            {
                message += $"\n\nFirst 5 errors:\n• {string.Join("\n• ", errors.Take(5))}";
                if (errors.Count > 5)
                {
                    message += $"\n• ... and {errors.Count - 5} more error(s)";
                }
            }

            MessageBox.Show(message, "Import Results", MessageBoxButtons.OK, 
                errorCount > 0 ? MessageBoxIcon.Warning : MessageBoxIcon.Information);

            await LoadUniformsAsync();
        }

        private async Task ProcessStudentImport(System.Data.DataTable data, Dictionary<string, string> columnMapping)
        {
            var successCount = 0;
            var errorCount = 0;
            var errors = new List<string>();

            var progressForm = new Form
            {
                Text = "Importing Students",
                Size = new Size(400, 150),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            var progressLabel = new Label
            {
                Text = "Processing records...",
                Location = new Point(20, 20),
                Size = new Size(360, 20)
            };

            var progressBar = new ProgressBar
            {
                Location = new Point(20, 50),
                Size = new Size(360, 23),
                Maximum = data.Rows.Count
            };

            progressForm.Controls.Add(progressLabel);
            progressForm.Controls.Add(progressBar);
            progressForm.Show();

            try
            {
                for (int i = 0; i < data.Rows.Count; i++)
                {
                    var row = data.Rows[i];
                    progressLabel.Text = $"Processing record {i + 1} of {data.Rows.Count}...";
                    progressBar.Value = i + 1;
                    Application.DoEvents();

                    try
                    {
                        var studentId = row[columnMapping["StudentIdentifier"]].ToString()?.Trim().ToUpper();
                        var firstName = row[columnMapping["FirstName"]].ToString()?.Trim();
                        var lastName = row[columnMapping["LastName"]].ToString()?.Trim();
                        var gradeStr = row[columnMapping["Grade"]].ToString()?.Trim();

                        if (string.IsNullOrEmpty(studentId) || string.IsNullOrEmpty(firstName) || 
                            string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(gradeStr))
                        {
                            errors.Add($"Row {i + 1}: Missing required fields");
                            errorCount++;
                            continue;
                        }

                        if (!int.TryParse(gradeStr, out int grade) || grade < 1 || grade > 12)
                        {
                            errors.Add($"Row {i + 1}: Invalid grade '{gradeStr}' (must be 1-12)");
                            errorCount++;
                            continue;
                        }

                        // Create the student silently (no message boxes)
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
                            successCount++;
                        }
                        else
                        {
                            errors.Add($"Row {i + 1} ({studentId} - {firstName} {lastName}): {result?.Message ?? "Unknown error"}");
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

            // Show results
            var message = $"Import completed!\n\n" +
                         $"✓ Successfully imported: {successCount}\n" +
                         $"✗ Errors: {errorCount}";

            if (errors.Any())
            {
                message += $"\n\nFirst 5 errors:\n• {string.Join("\n• ", errors.Take(5))}";
            }

            MessageBox.Show(message, "Import Results", MessageBoxButtons.OK, 
                errorCount > 0 ? MessageBoxIcon.Warning : MessageBoxIcon.Information);

            await LoadStudentsAsync();
        }

        private int ParseUniformType(string typeStr)
        {
            // Try to parse as number first
            if (int.TryParse(typeStr, out int typeInt))
            {
                if (typeInt >= 0 && typeInt <= 6)
                    return typeInt;
            }

            // Try to parse as string
            var typeLower = typeStr.ToLower().Replace(" ", "");
            return typeLower switch
            {
                "concertcoat" or "concert" => 0,
                "drummajorcoat" or "drummajor" or "dm" => 1,
                "hat" => 2,
                "marchingcoat" or "marching" => 3,
                "marchingshorts" or "shorts" => 4,
                "marchingsocks" or "socks" => 5,
                "pants" => 6,
                _ => throw new ArgumentException($"Unknown uniform type: {typeStr}")
            };
        }

        #endregion
    }
}
