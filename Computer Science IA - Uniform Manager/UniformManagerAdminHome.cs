using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
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
            // If no user or organization is set, this shouldn't happen - but handle gracefully
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
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading uniforms: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                dataGridViewUniforms.Columns["UniformType"].HeaderText = "Type";
            
            if (dataGridViewUniforms.Columns.Contains("IsCheckedOut"))
                dataGridViewUniforms.Columns["IsCheckedOut"].HeaderText = "Checked Out";
            
            if (dataGridViewUniforms.Columns.Contains("AssignedStudentId"))
                dataGridViewUniforms.Columns["AssignedStudentId"].HeaderText = "Assigned To";
            
            if (dataGridViewUniforms.Columns.Contains("CreatedDate"))
                dataGridViewUniforms.Columns["CreatedDate"].Visible = false;
            
            if (dataGridViewUniforms.Columns.Contains("LastModified"))
                dataGridViewUniforms.Columns["LastModified"].Visible = false;
            
            if (dataGridViewUniforms.Columns.Contains("Conditions"))
                dataGridViewUniforms.Columns["Conditions"].Visible = false;
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
                var content = new StringContent(json, Encoding.UTF8, "application/json");

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
                var content = new StringContent(json, Encoding.UTF8, "application/json");

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
                var content = new StringContent(json, Encoding.UTF8, "application/json");

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

        private async void SwitchOrganizationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var orgSelector = new OrganizationSelectorForm(_currentUser!);
            if (orgSelector.ShowDialog() == DialogResult.OK && orgSelector.SelectedOrganization != null)
            {
                _currentOrganization = orgSelector.SelectedOrganization;
                
                // Update form title
                this.Text = $"Uniform Manager - {_currentOrganization.OrganizationName} - {_currentUser!.FirstName} {_currentUser.LastName} ({GetAccountLevelText(_currentOrganization.UserAccountLevel)})";
                
                // Reload all data for the new organization
                await LoadAllData();
            }
        }

        private async void JoinOrganizationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var joinOrgForm = new JoinOrganizationForm(_currentUser!);
            if (joinOrgForm.ShowDialog() == DialogResult.OK)
            {
                MessageBox.Show("Your request has been sent to the organization administrators for approval.\n\nUse 'Organization > Switch Organization' once approved.", 
                    "Request Sent", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void OrganizationInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_currentOrganization == null)
            {
                MessageBox.Show("No organization is currently selected.", 
                    "No Organization", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var orgInfoForm = new OrganizationInfoForm(_currentOrganization);
            orgInfoForm.ShowDialog();
        }

        private void ManageJoinRequestsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_currentOrganization == null)
            {
                MessageBox.Show("No organization is currently selected.", 
                    "No Organization", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Check if user is admin
            if (_currentOrganization.UserAccountLevel != 0)
            {
                MessageBox.Show("Only administrators can manage join requests.", 
                    "Insufficient Permissions", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var manageRequestsForm = new ManageJoinRequestsForm(_currentUser!, _currentOrganization);
            manageRequestsForm.ShowDialog();
        }

        private void ManageUsersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_currentOrganization == null)
            {
                MessageBox.Show("No organization is currently selected.", 
                    "No Organization", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Check if user is admin
            if (_currentOrganization.UserAccountLevel != 0)
            {
                MessageBox.Show("Only administrators can manage users.", 
                    "Insufficient Permissions", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var manageUsersForm = new ManageOrganizationUsersForm(_currentUser!, _currentOrganization);
            manageUsersForm.ShowDialog();
        }

        // Response Models
        private class UniformListResponse
        {
            public bool Success { get; set; }
            public string Message { get; set; } = string.Empty;
            public List<UniformDto>? Uniforms { get; set; }
            public int TotalCount { get; set; }
        }

        private class UniformDto
        {
            public int UniformId { get; set; }
            public string UniformIdentifier { get; set; } = string.Empty;
            public int UniformType { get; set; }
            public int Size { get; set; }
            public bool IsCheckedOut { get; set; }
            public string? AssignedStudentId { get; set; }
        }

        private class StudentListResponse
        {
            public bool Success { get; set; }
            public string Message { get; set; } = string.Empty;
            public List<StudentDto>? Students { get; set; }
            public int TotalCount { get; set; }
        }

        private class StudentDto
        {
            public int StudentId { get; set; }
            public string StudentIdentifier { get; set; } = string.Empty;
            public string FirstName { get; set; } = string.Empty;
            public string LastName { get; set; } = string.Empty;
            public string FullName { get; set; } = string.Empty;
            public int Grade { get; set; }
            public DateTime CreatedDate { get; set; }
            public DateTime? LastModified { get; set; }
        }

        private class UserDto
        {
            public int UserId { get; set; }
            public string Username { get; set; } = string.Empty;
            public string FirstName { get; set; } = string.Empty;
            public string LastName { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public int AccountLevel { get; set; }
        }

        // Organization User Management Models
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

        private class OrganizationUsersResponse
        {
            public bool Success { get; set; }
            public string Message { get; set; } = string.Empty;
            public List<OrganizationUserDto>? Users { get; set; }
            public int TotalCount { get; set; }
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

        private class CreateStudentRequest
        {
            public int OrganizationId { get; set; }
            public string StudentIdentifier { get; set; } = string.Empty;
            public string FirstName { get; set; } = string.Empty;
            public string LastName { get; set; } = string.Empty;
            public int Grade { get; set; }
            public int RequestingUserId { get; set; }
        }

        private class UpdateStudentRequest
        {
            public string StudentIdentifier { get; set; } = string.Empty;
            public string? FirstName { get; set; }
            public string? LastName { get; set; }
            public int? Grade { get; set; }
            public int RequestingUserId { get; set; }
        }

        private class StudentResponse
        {
            public bool Success { get; set; }
            public string Message { get; set; } = string.Empty;
            public StudentDto? Student { get; set; }
        }
    }
}
