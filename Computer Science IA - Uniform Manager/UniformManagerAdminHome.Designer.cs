namespace Computer_Science_IA___Uniform_Manager
{
    partial class UniformManagerAdminHome
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            menuStrip1 = new MenuStrip();
            homeToolStripMenuItem = new ToolStripMenuItem();
            navigateToolStripMenuItem = new ToolStripMenuItem();
            fileToolStripMenuItem = new ToolStripMenuItem();
            editToolStripMenuItem = new ToolStripMenuItem();
            orgToolStripMenuItem = new ToolStripMenuItem();
            switchOrganizationToolStripMenuItem = new ToolStripMenuItem();
            joinOrganizationToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            organizationInfoToolStripMenuItem = new ToolStripMenuItem();
            manageJoinRequestsToolStripMenuItem = new ToolStripMenuItem();
            manageUsersToolStripMenuItem = new ToolStripMenuItem();
            tableLayoutPanel1 = new TableLayoutPanel();
            panelUniforms = new Panel();
            dataGridViewUniforms = new DataGridView();
            labelUniforms = new Label();
            panelStudents = new Panel();
            dataGridViewStudents = new DataGridView();
            labelStudents = new Label();
            panelUsers = new Panel();
            dataGridViewUsers = new DataGridView();
            panelUsersButtons = new Panel();
            buttonChangeUserRole = new Button();
            buttonRemoveUserFromOrg = new Button();
            contextMenuStripUsers = new ContextMenuStrip();
            changeRoleToolStripMenuItem = new ToolStripMenuItem();
            removeFromOrgToolStripMenuItem = new ToolStripMenuItem();
            labelUsers = new Label();
            userToolStripMenuItem = new ToolStripMenuItem();
            menuStrip1.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            panelUniforms.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridViewUniforms).BeginInit();
            panelStudents.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridViewStudents).BeginInit();
            panelUsers.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridViewUsers).BeginInit();
            contextMenuStripUsers.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(24, 24);
            menuStrip1.Items.AddRange(new ToolStripItem[] { homeToolStripMenuItem, navigateToolStripMenuItem, fileToolStripMenuItem, editToolStripMenuItem, orgToolStripMenuItem, userToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Padding = new Padding(4, 1, 0, 1);
            menuStrip1.Size = new Size(980, 24);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // homeToolStripMenuItem
            // 
            homeToolStripMenuItem.Name = "homeToolStripMenuItem";
            homeToolStripMenuItem.Size = new Size(52, 22);
            homeToolStripMenuItem.Text = "Home";
            // 
            // navigateToolStripMenuItem
            // 
            navigateToolStripMenuItem.Name = "navigateToolStripMenuItem";
            navigateToolStripMenuItem.Size = new Size(66, 22);
            navigateToolStripMenuItem.Text = "Navigate";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 22);
            fileToolStripMenuItem.Text = "File";
            // 
            // editToolStripMenuItem
            // 
            editToolStripMenuItem.Name = "editToolStripMenuItem";
            editToolStripMenuItem.Size = new Size(39, 22);
            editToolStripMenuItem.Text = "Edit";
            // 
            // orgToolStripMenuItem
            // 
            orgToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { switchOrganizationToolStripMenuItem, joinOrganizationToolStripMenuItem, toolStripSeparator1, organizationInfoToolStripMenuItem, manageJoinRequestsToolStripMenuItem, manageUsersToolStripMenuItem });
            orgToolStripMenuItem.Name = "orgToolStripMenuItem";
            orgToolStripMenuItem.Size = new Size(90, 22);
            orgToolStripMenuItem.Text = "Organization ";
            // 
            // switchOrganizationToolStripMenuItem
            // 
            switchOrganizationToolStripMenuItem.Name = "switchOrganizationToolStripMenuItem";
            switchOrganizationToolStripMenuItem.Size = new Size(210, 22);
            switchOrganizationToolStripMenuItem.Text = "Switch Organization";
            switchOrganizationToolStripMenuItem.Click += SwitchOrganizationToolStripMenuItem_Click;
            // 
            // joinOrganizationToolStripMenuItem
            // 
            joinOrganizationToolStripMenuItem.Name = "joinOrganizationToolStripMenuItem";
            joinOrganizationToolStripMenuItem.Size = new Size(210, 22);
            joinOrganizationToolStripMenuItem.Text = "Join Organization";
            joinOrganizationToolStripMenuItem.Click += JoinOrganizationToolStripMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(207, 6);
            // 
            // organizationInfoToolStripMenuItem
            // 
            organizationInfoToolStripMenuItem.Name = "organizationInfoToolStripMenuItem";
            organizationInfoToolStripMenuItem.Size = new Size(210, 22);
            organizationInfoToolStripMenuItem.Text = "Organization Info";
            organizationInfoToolStripMenuItem.Click += OrganizationInfoToolStripMenuItem_Click;
            // 
            // manageJoinRequestsToolStripMenuItem
            // 
            manageJoinRequestsToolStripMenuItem.Name = "manageJoinRequestsToolStripMenuItem";
            manageJoinRequestsToolStripMenuItem.Size = new Size(210, 22);
            manageJoinRequestsToolStripMenuItem.Text = "Manage Join Requests";
            manageJoinRequestsToolStripMenuItem.Click += ManageJoinRequestsToolStripMenuItem_Click;
            // 
            // manageUsersToolStripMenuItem
            // 
            manageUsersToolStripMenuItem.Name = "manageUsersToolStripMenuItem";
            manageUsersToolStripMenuItem.Size = new Size(210, 22);
            manageUsersToolStripMenuItem.Text = "Manage Users";
            manageUsersToolStripMenuItem.Click += ManageUsersToolStripMenuItem_Click;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 3;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.34F));
            tableLayoutPanel1.Controls.Add(panelUniforms, 0, 0);
            tableLayoutPanel1.Controls.Add(panelStudents, 1, 0);
            tableLayoutPanel1.Controls.Add(panelUsers, 2, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 24);
            tableLayoutPanel1.Margin = new Padding(2, 2, 2, 2);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new Size(980, 366);
            tableLayoutPanel1.TabIndex = 1;
            // 
            // panelUniforms
            // 
            panelUniforms.Controls.Add(dataGridViewUniforms);
            panelUniforms.Controls.Add(labelUniforms);
            panelUniforms.Dock = DockStyle.Fill;
            panelUniforms.Location = new Point(2, 2);
            panelUniforms.Margin = new Padding(2, 2, 2, 2);
            panelUniforms.Name = "panelUniforms";
            panelUniforms.Size = new Size(322, 362);
            panelUniforms.TabIndex = 0;
            // 
            // dataGridViewUniforms
            // 
            dataGridViewUniforms.AllowUserToAddRows = false;
            dataGridViewUniforms.AllowUserToDeleteRows = false;
            dataGridViewUniforms.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewUniforms.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewUniforms.Dock = DockStyle.Fill;
            dataGridViewUniforms.Location = new Point(0, 24);
            dataGridViewUniforms.Margin = new Padding(2, 2, 2, 2);
            dataGridViewUniforms.Name = "dataGridViewUniforms";
            dataGridViewUniforms.ReadOnly = true;
            dataGridViewUniforms.RowHeadersWidth = 62;
            dataGridViewUniforms.Size = new Size(322, 338);
            dataGridViewUniforms.TabIndex = 1;
            // 
            // labelUniforms
            // 
            labelUniforms.BackColor = SystemColors.ControlDark;
            labelUniforms.Dock = DockStyle.Top;
            labelUniforms.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            labelUniforms.Location = new Point(0, 0);
            labelUniforms.Margin = new Padding(2, 0, 2, 0);
            labelUniforms.Name = "labelUniforms";
            labelUniforms.Size = new Size(322, 24);
            labelUniforms.TabIndex = 0;
            labelUniforms.Text = "Uniforms";
            labelUniforms.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // panelStudents
            // 
            panelStudents.Controls.Add(dataGridViewStudents);
            panelStudents.Controls.Add(labelStudents);
            panelStudents.Dock = DockStyle.Fill;
            panelStudents.Location = new Point(328, 2);
            panelStudents.Margin = new Padding(2, 2, 2, 2);
            panelStudents.Name = "panelStudents";
            panelStudents.Size = new Size(322, 362);
            panelStudents.TabIndex = 1;
            // 
            // dataGridViewStudents
            // 
            dataGridViewStudents.AllowUserToAddRows = false;
            dataGridViewStudents.AllowUserToDeleteRows = false;
            dataGridViewStudents.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewStudents.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewStudents.Dock = DockStyle.Fill;
            dataGridViewStudents.Location = new Point(0, 24);
            dataGridViewStudents.Margin = new Padding(2, 2, 2, 2);
            dataGridViewStudents.Name = "dataGridViewStudents";
            dataGridViewStudents.ReadOnly = true;
            dataGridViewStudents.RowHeadersWidth = 62;
            dataGridViewStudents.Size = new Size(322, 338);
            dataGridViewStudents.TabIndex = 1;
            // 
            // labelStudents
            // 
            labelStudents.BackColor = SystemColors.ControlDark;
            labelStudents.Dock = DockStyle.Top;
            labelStudents.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            labelStudents.Location = new Point(0, 0);
            labelStudents.Margin = new Padding(2, 0, 2, 0);
            labelStudents.Name = "labelStudents";
            labelStudents.Size = new Size(322, 24);
            labelStudents.TabIndex = 0;
            labelStudents.Text = "Students";
            labelStudents.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // panelUsers
            // 
            panelUsers.Controls.Add(dataGridViewUsers);
            panelUsers.Controls.Add(panelUsersButtons);
            panelUsers.Controls.Add(labelUsers);
            panelUsers.Dock = DockStyle.Fill;
            panelUsers.Location = new Point(654, 2);
            panelUsers.Margin = new Padding(2, 2, 2, 2);
            panelUsers.Name = "panelUsers";
            panelUsers.Size = new Size(324, 362);
            panelUsers.TabIndex = 2;
            // 
            // dataGridViewUsers
            // 
            dataGridViewUsers.AllowUserToAddRows = false;
            dataGridViewUsers.AllowUserToDeleteRows = false;
            dataGridViewUsers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewUsers.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewUsers.ContextMenuStrip = contextMenuStripUsers;
            dataGridViewUsers.Dock = DockStyle.Fill;
            dataGridViewUsers.Location = new Point(0, 24);
            dataGridViewUsers.Margin = new Padding(2, 2, 2, 2);
            dataGridViewUsers.MultiSelect = false;
            dataGridViewUsers.Name = "dataGridViewUsers";
            dataGridViewUsers.ReadOnly = true;
            dataGridViewUsers.RowHeadersWidth = 62;
            dataGridViewUsers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewUsers.Size = new Size(324, 298);
            dataGridViewUsers.TabIndex = 1;
            dataGridViewUsers.SelectionChanged += DataGridViewUsers_SelectionChanged;
            // 
            // panelUsersButtons
            // 
            panelUsersButtons.Controls.Add(buttonChangeUserRole);
            panelUsersButtons.Controls.Add(buttonRemoveUserFromOrg);
            panelUsersButtons.Dock = DockStyle.Bottom;
            panelUsersButtons.Location = new Point(0, 322);
            panelUsersButtons.Name = "panelUsersButtons";
            panelUsersButtons.Size = new Size(324, 40);
            panelUsersButtons.TabIndex = 2;
            panelUsersButtons.Visible = false;
            // 
            // buttonChangeUserRole
            // 
            buttonChangeUserRole.BackColor = Color.LightBlue;
            buttonChangeUserRole.Enabled = false;
            buttonChangeUserRole.Location = new Point(5, 5);
            buttonChangeUserRole.Name = "buttonChangeUserRole";
            buttonChangeUserRole.Size = new Size(155, 30);
            buttonChangeUserRole.TabIndex = 0;
            buttonChangeUserRole.Text = "Change Role";
            buttonChangeUserRole.UseVisualStyleBackColor = false;
            buttonChangeUserRole.Click += ButtonChangeUserRole_Click;
            // 
            // buttonRemoveUserFromOrg
            // 
            buttonRemoveUserFromOrg.BackColor = Color.LightCoral;
            buttonRemoveUserFromOrg.Enabled = false;
            buttonRemoveUserFromOrg.Location = new Point(165, 5);
            buttonRemoveUserFromOrg.Name = "buttonRemoveUserFromOrg";
            buttonRemoveUserFromOrg.Size = new Size(155, 30);
            buttonRemoveUserFromOrg.TabIndex = 1;
            buttonRemoveUserFromOrg.Text = "✗ Remove";
            buttonRemoveUserFromOrg.UseVisualStyleBackColor = false;
            buttonRemoveUserFromOrg.Click += ButtonRemoveUserFromOrg_Click;
            // 
            // contextMenuStripUsers
            // 
            contextMenuStripUsers.Items.AddRange(new ToolStripItem[] { changeRoleToolStripMenuItem, removeFromOrgToolStripMenuItem });
            contextMenuStripUsers.Name = "contextMenuStripUsers";
            contextMenuStripUsers.Size = new Size(181, 48);
            // 
            // changeRoleToolStripMenuItem
            // 
            changeRoleToolStripMenuItem.Name = "changeRoleToolStripMenuItem";
            changeRoleToolStripMenuItem.Size = new Size(180, 22);
            changeRoleToolStripMenuItem.Text = "Change Role...";
            changeRoleToolStripMenuItem.Click += ChangeRoleToolStripMenuItem_Click;
            // 
            // removeFromOrgToolStripMenuItem
            // 
            removeFromOrgToolStripMenuItem.Name = "removeFromOrgToolStripMenuItem";
            removeFromOrgToolStripMenuItem.Size = new Size(180, 22);
            removeFromOrgToolStripMenuItem.Text = "Remove from Org";
            removeFromOrgToolStripMenuItem.Click += RemoveFromOrgToolStripMenuItem_Click;
            // 
            // labelUsers
            // 
            labelUsers.BackColor = SystemColors.ControlDark;
            labelUsers.Dock = DockStyle.Top;
            labelUsers.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            labelUsers.Location = new Point(0, 0);
            labelUsers.Margin = new Padding(2, 0, 2, 0);
            labelUsers.Name = "labelUsers";
            labelUsers.Size = new Size(324, 24);
            labelUsers.TabIndex = 0;
            labelUsers.Text = "Users";
            labelUsers.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // userToolStripMenuItem
            // 
            userToolStripMenuItem.Alignment = ToolStripItemAlignment.Right;
            userToolStripMenuItem.Name = "userToolStripMenuItem";
            userToolStripMenuItem.Size = new Size(42, 22);
            userToolStripMenuItem.Text = "User";
            // 
            // UniformManagerAdminHome
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(980, 390);
            Controls.Add(tableLayoutPanel1);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Margin = new Padding(2, 2, 2, 2);
            Name = "UniformManagerAdminHome";
            Text = "Uniform Manager";
            Load += UniformManagerAdminHome_Load;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            tableLayoutPanel1.ResumeLayout(false);
            panelUniforms.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridViewUniforms).EndInit();
            panelStudents.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridViewStudents).EndInit();
            panelUsers.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridViewUsers).EndInit();
            contextMenuStripUsers.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem homeToolStripMenuItem;
        private ToolStripMenuItem navigateToolStripMenuItem;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem editToolStripMenuItem;
        private ToolStripMenuItem orgToolStripMenuItem;
        private ToolStripMenuItem switchOrganizationToolStripMenuItem;
        private ToolStripMenuItem joinOrganizationToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem organizationInfoToolStripMenuItem;
        private TableLayoutPanel tableLayoutPanel1;
        private Panel panelUniforms;
        private DataGridView dataGridViewUniforms;
        private Label labelUniforms;
        private Panel panelStudents;
        private DataGridView dataGridViewStudents;
        private Label labelStudents;
        private Panel panelUsers;
        private DataGridView dataGridViewUsers;
        private Panel panelUsersButtons;
        private Button buttonChangeUserRole;
        private Button buttonRemoveUserFromOrg;
        private ContextMenuStrip contextMenuStripUsers;
        private ToolStripMenuItem changeRoleToolStripMenuItem;
        private ToolStripMenuItem removeFromOrgToolStripMenuItem;
        private Label labelUsers;
        private ToolStripMenuItem userToolStripMenuItem;
        private ToolStripMenuItem manageJoinRequestsToolStripMenuItem;
        private ToolStripMenuItem manageUsersToolStripMenuItem;
    }
}