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
                
                // Only load users if current user is an administrator in this organization
                Task? usersTask = null;
                if (_currentOrganization?.UserAccountLevel == 0)
                {
                    usersTask = LoadUsersAsync();
                    await Task.WhenAll(uniformsTask, studentsTask, usersTask);
                }
                else
                {
                    await Task.WhenAll(uniformsTask, studentsTask);
                    dataGridViewUsers.DataSource = null;
                    labelUsers.Text = "Users (Admin Only)";
                }
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
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading students: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async Task LoadUsersAsync()
        {
            try
            {
                var response = await httpClient.GetAsync($"{API_BASE_URL}/GetUsers?userId={_currentUser?.UserId}");
                
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    labelUsers.Text = "Users (Insufficient Permissions)";
                    return;
                }

                response.EnsureSuccessStatusCode();
                var jsonString = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<UserListResponse>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (result?.Success == true && result.Users != null)
                {
                    dataGridViewUsers.DataSource = result.Users;
                    FormatUsersGrid();
                    labelUsers.Text = $"Users ({result.TotalCount})";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading users: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            
            if (dataGridViewStudents.Columns.Contains("CreatedDate"))
                dataGridViewStudents.Columns["CreatedDate"].Visible = false;
            
            if (dataGridViewStudents.Columns.Contains("LastModified"))
                dataGridViewStudents.Columns["LastModified"].Visible = false;
        }

        private void FormatUsersGrid()
        {
            if (dataGridViewUsers.Columns.Contains("UserId"))
                dataGridViewUsers.Columns["UserId"].Visible = false;
            
            if (dataGridViewUsers.Columns.Contains("FirstName"))
                dataGridViewUsers.Columns["FirstName"].HeaderText = "First Name";
            
            if (dataGridViewUsers.Columns.Contains("LastName"))
                dataGridViewUsers.Columns["LastName"].HeaderText = "Last Name";
            
            if (dataGridViewUsers.Columns.Contains("AccountLevel"))
            {
                dataGridViewUsers.Columns["AccountLevel"].HeaderText = "Access Level";
            }
        }

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

        // Response Models
        private class UniformListResponse
        {
            public bool Success { get; set; }
            public string Message { get; set; } = string.Empty;
            public List<UniformDto>? Uniforms { get; set; }
            public int TotalCount { get; set; }
        }

        private class StudentListResponse
        {
            public bool Success { get; set; }
            public string Message { get; set; } = string.Empty;
            public List<StudentDto>? Students { get; set; }
            public int TotalCount { get; set; }
        }

        private class UserListResponse
        {
            public bool Success { get; set; }
            public string Message { get; set; } = string.Empty;
            public List<UserDto>? Users { get; set; }
            public int TotalCount { get; set; }
        }

        // DTOs for API responses
        private class UniformDto
        {
            public int UniformId { get; set; }
            public string UniformIdentifier { get; set; } = string.Empty;
            public int UniformType { get; set; }
            public int Size { get; set; }
            public bool IsCheckedOut { get; set; }
            public string? AssignedStudentId { get; set; }
        }

        private class StudentDto
        {
            public int StudentId { get; set; }
            public string StudentIdentifier { get; set; } = string.Empty;
            public string FirstName { get; set; } = string.Empty;
            public string LastName { get; set; } = string.Empty;
            public int Grade { get; set; }
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
    }
}
