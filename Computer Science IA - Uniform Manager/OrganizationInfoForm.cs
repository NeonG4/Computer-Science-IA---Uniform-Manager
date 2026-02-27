using System;
using System.Windows.Forms;
using Computer_Science_IA___Uniform_Manager.Models;

namespace Computer_Science_IA___Uniform_Manager
{
    public partial class OrganizationInfoForm : Form
    {
        private readonly OrganizationDto _organization;

        public OrganizationInfoForm(OrganizationDto organization)
        {
            InitializeComponent();
            _organization = organization;
        }

        private void OrganizationInfoForm_Load(object sender, EventArgs e)
        {
            LoadOrganizationInfo();
        }

        private void LoadOrganizationInfo()
        {
            // Set organization name
            textBoxOrgName.Text = _organization.OrganizationName;

            // Set organization code (both fields)
            textBoxOrgCode.Text = _organization.OrganizationCode;
            textBoxShareCode.Text = _organization.OrganizationCode;

            // Set description
            if (!string.IsNullOrWhiteSpace(_organization.Description))
            {
                textBoxDescription.Text = _organization.Description;
            }
            else
            {
                textBoxDescription.Text = "No description provided.";
                textBoxDescription.ForeColor = System.Drawing.SystemColors.GrayText;
            }

            // Set user's role
            string roleText = GetRoleText(_organization.UserAccountLevel);
            textBoxYourRole.Text = roleText;

            // Color code the role
            textBoxYourRole.BackColor = GetRoleColor(_organization.UserAccountLevel);

            // Update form title
            this.Text = $"{_organization.OrganizationName} - Info";
        }

        private string GetRoleText(int? accountLevel)
        {
            return accountLevel switch
            {
                0 => "Administrator",
                1 => "User",
                2 => "Viewer",
                _ => "Unknown"
            };
        }

        private System.Drawing.Color GetRoleColor(int? accountLevel)
        {
            return accountLevel switch
            {
                0 => System.Drawing.Color.LightGreen,    // Admin - Green
                1 => System.Drawing.Color.LightBlue,     // User - Blue
                2 => System.Drawing.Color.LightYellow,   // Viewer - Yellow
                _ => System.Drawing.Color.LightGray      // Unknown - Gray
            };
        }

        private void ButtonCopyCode_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.SetText(_organization.OrganizationCode);
                
                // Visual feedback
                buttonCopyCode.Text = "? Copied!";
                buttonCopyCode.BackColor = System.Drawing.Color.LightGreen;
                
                // Reset button after 2 seconds
                var timer = new System.Windows.Forms.Timer();
                timer.Interval = 2000;
                timer.Tick += (s, args) =>
                {
                    buttonCopyCode.Text = "?? Copy Code";
                    buttonCopyCode.BackColor = System.Drawing.SystemColors.Control;
                    timer.Stop();
                    timer.Dispose();
                };
                timer.Start();

                MessageBox.Show($"Organization code '{_organization.OrganizationCode}' copied to clipboard!\n\n" +
                    "Share this code with others so they can join your organization.",
                    "Code Copied", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to copy code to clipboard:\n\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ButtonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
