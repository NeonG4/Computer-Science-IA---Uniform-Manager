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
            components = new System.ComponentModel.Container();
            menuStrip1 = new MenuStrip();
            homeToolStripMenuItem = new ToolStripMenuItem();
            navigateToolStripMenuItem = new ToolStripMenuItem();
            fileToolStripMenuItem = new ToolStripMenuItem();
            importUniformsToolStripMenuItem = new ToolStripMenuItem();
            importStudentsToolStripMenuItem = new ToolStripMenuItem();
            editToolStripMenuItem = new ToolStripMenuItem();
            searchAndEditStudentToolStripMenuItem = new ToolStripMenuItem();
            promoteStudentsToolStripMenuItem = new ToolStripMenuItem();
            unassignAllUniformsToolStripMenuItem = new ToolStripMenuItem();
            deleteAllUniformsToolStripMenuItem = new ToolStripMenuItem();
            deleteAllStudentsToolStripMenuItem = new ToolStripMenuItem();
            orgToolStripMenuItem = new ToolStripMenuItem();
            switchOrganizationToolStripMenuItem = new ToolStripMenuItem();
            joinOrganizationToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            organizationInfoToolStripMenuItem = new ToolStripMenuItem();
            manageJoinRequestsToolStripMenuItem = new ToolStripMenuItem();
            manageUsersToolStripMenuItem = new ToolStripMenuItem();
            userToolStripMenuItem = new ToolStripMenuItem();
            tableLayoutPanel1 = new TableLayoutPanel();
            panelUniforms = new Panel();
            dataGridViewUniforms = new DataGridView();
            contextMenuStripUniforms = new ContextMenuStrip(components);
            addUniformToolStripMenuItem = new ToolStripMenuItem();
            editUniformToolStripMenuItem = new ToolStripMenuItem();
            deleteUniformToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator2 = new ToolStripSeparator();
            assignUniformToolStripMenuItem = new ToolStripMenuItem();
            unassignUniformToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator3 = new ToolStripSeparator();
            checkOutInToolStripMenuItem = new ToolStripMenuItem();
            updateConditionsToolStripMenuItem = new ToolStripMenuItem();
            panelUniformsButtons = new Panel();
            buttonAddUniform = new Button();
            buttonEditUniform = new Button();
            buttonDeleteUniform = new Button();
            buttonAssignUniform = new Button();
            buttonUnassignUniform = new Button();
            buttonCheckOutUniform = new Button();
            buttonUpdateConditions = new Button();
            labelUniforms = new Label();
            panelStudents = new Panel();
            dataGridViewStudents = new DataGridView();
            contextMenuStripStudents = new ContextMenuStrip(components);
            addStudentToolStripMenuItem = new ToolStripMenuItem();
            editStudentToolStripMenuItem = new ToolStripMenuItem();
            deleteStudentToolStripMenuItem = new ToolStripMenuItem();
            panelStudentsButtons = new Panel();
            buttonAddStudent = new Button();
            buttonEditStudent = new Button();
            buttonDeleteStudent = new Button();
            labelStudents = new Label();
            panelUsers = new Panel();
            dataGridViewUsers = new DataGridView();
            contextMenuStripUsers = new ContextMenuStrip(components);
            changeRoleToolStripMenuItem = new ToolStripMenuItem();
            removeFromOrgToolStripMenuItem = new ToolStripMenuItem();
            panelUsersButtons = new Panel();
            buttonChangeUserRole = new Button();
            buttonRemoveUserFromOrg = new Button();
            labelUsers = new Label();
            toolStripMenuItem1 = new ToolStripSeparator();
            toolStripMenuItem2 = new ToolStripSeparator();
            menuStrip1.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            panelUniforms.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridViewUniforms).BeginInit();
            contextMenuStripUniforms.SuspendLayout();
            panelUniformsButtons.SuspendLayout();
            panelStudents.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridViewStudents).BeginInit();
            contextMenuStripStudents.SuspendLayout();
            panelStudentsButtons.SuspendLayout();
            panelUsers.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridViewUsers).BeginInit();
            contextMenuStripUsers.SuspendLayout();
            panelUsersButtons.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(24, 24);
            menuStrip1.Items.AddRange(new ToolStripItem[] { homeToolStripMenuItem, navigateToolStripMenuItem, fileToolStripMenuItem, editToolStripMenuItem, orgToolStripMenuItem, userToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Padding = new Padding(5, 1, 0, 1);
            menuStrip1.Size = new Size(1120, 26);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // homeToolStripMenuItem
            // 
            homeToolStripMenuItem.Name = "homeToolStripMenuItem";
            homeToolStripMenuItem.Size = new Size(64, 24);
            homeToolStripMenuItem.Text = "Home";
            // 
            // navigateToolStripMenuItem
            // 
            navigateToolStripMenuItem.Name = "navigateToolStripMenuItem";
            navigateToolStripMenuItem.Size = new Size(83, 24);
            navigateToolStripMenuItem.Text = "Navigate";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { importUniformsToolStripMenuItem, importStudentsToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(46, 24);
            fileToolStripMenuItem.Text = "File";
            // 
            // importUniformsToolStripMenuItem
            // 
            importUniformsToolStripMenuItem.Name = "importUniformsToolStripMenuItem";
            importUniformsToolStripMenuItem.Size = new Size(210, 26);
            importUniformsToolStripMenuItem.Text = "Import Uniforms...";
            importUniformsToolStripMenuItem.Click += ImportUniformsToolStripMenuItem_Click;
            // 
            // importStudentsToolStripMenuItem
            // 
            importStudentsToolStripMenuItem.Name = "importStudentsToolStripMenuItem";
            importStudentsToolStripMenuItem.Size = new Size(210, 26);
            importStudentsToolStripMenuItem.Text = "Import Students...";
            importStudentsToolStripMenuItem.Click += ImportStudentsToolStripMenuItem_Click;
            // 
            // editToolStripMenuItem
            // 
            editToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { searchAndEditStudentToolStripMenuItem, toolStripMenuItem1, checkMissingAssignmentsToolStripMenuItem, promoteStudentsToolStripMenuItem, unassignAllUniformsToolStripMenuItem, toolStripMenuItem2, deleteAllUniformsToolStripMenuItem, deleteAllStudentsToolStripMenuItem });
            editToolStripMenuItem.Name = "editToolStripMenuItem";
            editToolStripMenuItem.Size = new Size(49, 24);
            editToolStripMenuItem.Text = "Edit";
            // 
            // searchAndEditStudentToolStripMenuItem
            // 
            searchAndEditStudentToolStripMenuItem.Name = "searchAndEditStudentToolStripMenuItem";
            searchAndEditStudentToolStripMenuItem.Size = new Size(265, 26);
            searchAndEditStudentToolStripMenuItem.Text = "Search and Edit Student...";
            searchAndEditStudentToolStripMenuItem.Click += SearchAndEditStudentToolStripMenuItem_Click;
            // 
            // promoteStudentsToolStripMenuItem
            // 
            promoteStudentsToolStripMenuItem.Name = "promoteStudentsToolStripMenuItem";
            promoteStudentsToolStripMenuItem.Size = new Size(265, 26);
            promoteStudentsToolStripMenuItem.Text = "Age Students by a Grade...";
            promoteStudentsToolStripMenuItem.Click += PromoteStudentsToolStripMenuItem_Click;
            // 
            // checkMissingAssignmentsToolStripMenuItem
            // 
            checkMissingAssignmentsToolStripMenuItem.Name = "checkMissingAssignmentsToolStripMenuItem";
            checkMissingAssignmentsToolStripMenuItem.Size = new Size(265, 26);
            checkMissingAssignmentsToolStripMenuItem.Text = "Check Missing Assignments...";
            checkMissingAssignmentsToolStripMenuItem.Click += CheckMissingAssignmentsToolStripMenuItem_Click;
            // 
            // unassignAllUniformsToolStripMenuItem
            // 
            unassignAllUniformsToolStripMenuItem.Name = "unassignAllUniformsToolStripMenuItem";
            unassignAllUniformsToolStripMenuItem.Size = new Size(265, 26);
            unassignAllUniformsToolStripMenuItem.Text = "Unassign All Uniforms...";
            unassignAllUniformsToolStripMenuItem.Click += UnassignAllUniformsToolStripMenuItem_Click;
            // 
            // deleteAllUniformsToolStripMenuItem
            // 
            deleteAllUniformsToolStripMenuItem.Name = "deleteAllUniformsToolStripMenuItem";
            deleteAllUniformsToolStripMenuItem.Size = new Size(265, 26);
            deleteAllUniformsToolStripMenuItem.Text = "Delete All Uniforms...";
            deleteAllUniformsToolStripMenuItem.Click += DeleteAllUniformsToolStripMenuItem_Click;
            // 
            // deleteAllStudentsToolStripMenuItem
            // 
            deleteAllStudentsToolStripMenuItem.Name = "deleteAllStudentsToolStripMenuItem";
            deleteAllStudentsToolStripMenuItem.Size = new Size(265, 26);
            deleteAllStudentsToolStripMenuItem.Text = "Delete All Students...";
            deleteAllStudentsToolStripMenuItem.Click += DeleteAllStudentsToolStripMenuItem_Click;
            // 
            // orgToolStripMenuItem
            // 
            orgToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { switchOrganizationToolStripMenuItem, joinOrganizationToolStripMenuItem, toolStripSeparator1, organizationInfoToolStripMenuItem, manageJoinRequestsToolStripMenuItem, manageUsersToolStripMenuItem });
            orgToolStripMenuItem.Name = "orgToolStripMenuItem";
            orgToolStripMenuItem.Size = new Size(113, 24);
            orgToolStripMenuItem.Text = "Organization ";
            // 
            // switchOrganizationToolStripMenuItem
            // 
            switchOrganizationToolStripMenuItem.Name = "switchOrganizationToolStripMenuItem";
            switchOrganizationToolStripMenuItem.Size = new Size(239, 26);
            switchOrganizationToolStripMenuItem.Text = "Switch Organization";
            switchOrganizationToolStripMenuItem.Click += SwitchOrganizationToolStripMenuItem_Click;
            // 
            // joinOrganizationToolStripMenuItem
            // 
            joinOrganizationToolStripMenuItem.Name = "joinOrganizationToolStripMenuItem";
            joinOrganizationToolStripMenuItem.Size = new Size(239, 26);
            joinOrganizationToolStripMenuItem.Text = "Join Organization";
            joinOrganizationToolStripMenuItem.Click += JoinOrganizationToolStripMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(236, 6);
            // 
            // organizationInfoToolStripMenuItem
            // 
            organizationInfoToolStripMenuItem.Name = "organizationInfoToolStripMenuItem";
            organizationInfoToolStripMenuItem.Size = new Size(239, 26);
            organizationInfoToolStripMenuItem.Text = "Organization Info";
            organizationInfoToolStripMenuItem.Click += OrganizationInfoToolStripMenuItem_Click;
            // 
            // manageJoinRequestsToolStripMenuItem
            // 
            manageJoinRequestsToolStripMenuItem.Name = "manageJoinRequestsToolStripMenuItem";
            manageJoinRequestsToolStripMenuItem.Size = new Size(239, 26);
            manageJoinRequestsToolStripMenuItem.Text = "Manage Join Requests";
            manageJoinRequestsToolStripMenuItem.Click += ManageJoinRequestsToolStripMenuItem_Click;
            // 
            // manageUsersToolStripMenuItem
            // 
            manageUsersToolStripMenuItem.Name = "manageUsersToolStripMenuItem";
            manageUsersToolStripMenuItem.Size = new Size(239, 26);
            manageUsersToolStripMenuItem.Text = "Manage Users";
            manageUsersToolStripMenuItem.Click += ManageUsersToolStripMenuItem_Click;
            // 
            // userToolStripMenuItem
            // 
            userToolStripMenuItem.Alignment = ToolStripItemAlignment.Right;
            userToolStripMenuItem.Name = "userToolStripMenuItem";
            userToolStripMenuItem.Size = new Size(52, 24);
            userToolStripMenuItem.Text = "User";
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
            tableLayoutPanel1.Location = new Point(0, 26);
            tableLayoutPanel1.Margin = new Padding(2, 3, 2, 3);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new Size(1120, 494);
            tableLayoutPanel1.TabIndex = 1;
            // 
            // panelUniforms
            // 
            panelUniforms.Controls.Add(dataGridViewUniforms);
            panelUniforms.Controls.Add(panelUniformsButtons);
            panelUniforms.Controls.Add(labelUniforms);
            panelUniforms.Dock = DockStyle.Fill;
            panelUniforms.Location = new Point(2, 3);
            panelUniforms.Margin = new Padding(2, 3, 2, 3);
            panelUniforms.Name = "panelUniforms";
            panelUniforms.Size = new Size(369, 488);
            panelUniforms.TabIndex = 0;
            // 
            // dataGridViewUniforms
            // 
            dataGridViewUniforms.AllowUserToAddRows = false;
            dataGridViewUniforms.AllowUserToDeleteRows = false;
            dataGridViewUniforms.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewUniforms.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewUniforms.ContextMenuStrip = contextMenuStripUniforms;
            dataGridViewUniforms.Dock = DockStyle.Fill;
            dataGridViewUniforms.Location = new Point(0, 0);
            dataGridViewUniforms.Margin = new Padding(2, 3, 2, 3);
            dataGridViewUniforms.MultiSelect = false;
            dataGridViewUniforms.Name = "dataGridViewUniforms";
            dataGridViewUniforms.ReadOnly = true;
            dataGridViewUniforms.RowHeadersWidth = 62;
            dataGridViewUniforms.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewUniforms.Size = new Size(369, 355);
            dataGridViewUniforms.TabIndex = 1;
            dataGridViewUniforms.CellDoubleClick += DataGridViewUniforms_CellDoubleClick;
            dataGridViewUniforms.SelectionChanged += DataGridViewUniforms_SelectionChanged;
            // 
            // contextMenuStripUniforms
            // 
            contextMenuStripUniforms.ImageScalingSize = new Size(20, 20);
            contextMenuStripUniforms.Items.AddRange(new ToolStripItem[] { addUniformToolStripMenuItem, editUniformToolStripMenuItem, deleteUniformToolStripMenuItem, toolStripSeparator2, assignUniformToolStripMenuItem, unassignUniformToolStripMenuItem, toolStripSeparator3, checkOutInToolStripMenuItem, updateConditionsToolStripMenuItem });
            contextMenuStripUniforms.Name = "contextMenuStripUniforms";
            contextMenuStripUniforms.Size = new Size(212, 184);
            // 
            // addUniformToolStripMenuItem
            // 
            addUniformToolStripMenuItem.Name = "addUniformToolStripMenuItem";
            addUniformToolStripMenuItem.Size = new Size(211, 24);
            addUniformToolStripMenuItem.Text = "Add Uniform...";
            addUniformToolStripMenuItem.Click += AddUniformToolStripMenuItem_Click;
            // 
            // editUniformToolStripMenuItem
            // 
            editUniformToolStripMenuItem.Name = "editUniformToolStripMenuItem";
            editUniformToolStripMenuItem.Size = new Size(211, 24);
            editUniformToolStripMenuItem.Text = "Edit Uniform...";
            editUniformToolStripMenuItem.Click += EditUniformToolStripMenuItem_Click;
            // 
            // deleteUniformToolStripMenuItem
            // 
            deleteUniformToolStripMenuItem.Name = "deleteUniformToolStripMenuItem";
            deleteUniformToolStripMenuItem.Size = new Size(211, 24);
            deleteUniformToolStripMenuItem.Text = "Delete Uniform";
            deleteUniformToolStripMenuItem.Click += DeleteUniformToolStripMenuItem_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(208, 6);
            // 
            // assignUniformToolStripMenuItem
            // 
            assignUniformToolStripMenuItem.Name = "assignUniformToolStripMenuItem";
            assignUniformToolStripMenuItem.Size = new Size(211, 24);
            assignUniformToolStripMenuItem.Text = "Assign to Student...";
            assignUniformToolStripMenuItem.Click += AssignUniformToolStripMenuItem_Click;
            // 
            // unassignUniformToolStripMenuItem
            // 
            unassignUniformToolStripMenuItem.Name = "unassignUniformToolStripMenuItem";
            unassignUniformToolStripMenuItem.Size = new Size(211, 24);
            unassignUniformToolStripMenuItem.Text = "Unassign Student";
            unassignUniformToolStripMenuItem.Click += UnassignUniformToolStripMenuItem_Click;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new Size(208, 6);
            // 
            // checkOutInToolStripMenuItem
            // 
            checkOutInToolStripMenuItem.Name = "checkOutInToolStripMenuItem";
            checkOutInToolStripMenuItem.Size = new Size(211, 24);
            checkOutInToolStripMenuItem.Text = "Check Out/In...";
            checkOutInToolStripMenuItem.Click += CheckOutInToolStripMenuItem_Click;
            // 
            // updateConditionsToolStripMenuItem
            // 
            updateConditionsToolStripMenuItem.Name = "updateConditionsToolStripMenuItem";
            updateConditionsToolStripMenuItem.Size = new Size(211, 24);
            updateConditionsToolStripMenuItem.Text = "Update Conditions...";
            updateConditionsToolStripMenuItem.Click += UpdateConditionsToolStripMenuItem_Click;
            // 
            // panelUniformsButtons
            // 
            panelUniformsButtons.Controls.Add(buttonAddUniform);
            panelUniformsButtons.Controls.Add(buttonEditUniform);
            panelUniformsButtons.Controls.Add(buttonDeleteUniform);
            panelUniformsButtons.Controls.Add(buttonAssignUniform);
            panelUniformsButtons.Controls.Add(buttonUnassignUniform);
            panelUniformsButtons.Controls.Add(buttonCheckOutUniform);
            panelUniformsButtons.Controls.Add(buttonUpdateConditions);
            panelUniformsButtons.Dock = DockStyle.Bottom;
            panelUniformsButtons.Location = new Point(0, 355);
            panelUniformsButtons.Margin = new Padding(3, 4, 3, 4);
            panelUniformsButtons.Name = "panelUniformsButtons";
            panelUniformsButtons.Size = new Size(369, 133);
            panelUniformsButtons.TabIndex = 2;
            panelUniformsButtons.Visible = false;
            // 
            // buttonAddUniform
            // 
            buttonAddUniform.BackColor = Color.LightGreen;
            buttonAddUniform.Location = new Point(6, 7);
            buttonAddUniform.Margin = new Padding(3, 4, 3, 4);
            buttonAddUniform.Name = "buttonAddUniform";
            buttonAddUniform.Size = new Size(114, 37);
            buttonAddUniform.TabIndex = 0;
            buttonAddUniform.Text = "+ Add";
            buttonAddUniform.UseVisualStyleBackColor = false;
            buttonAddUniform.Click += ButtonAddUniform_Click;
            // 
            // buttonEditUniform
            // 
            buttonEditUniform.BackColor = Color.LightBlue;
            buttonEditUniform.Enabled = false;
            buttonEditUniform.Location = new Point(126, 7);
            buttonEditUniform.Margin = new Padding(3, 4, 3, 4);
            buttonEditUniform.Name = "buttonEditUniform";
            buttonEditUniform.Size = new Size(114, 37);
            buttonEditUniform.TabIndex = 1;
            buttonEditUniform.Text = "Edit";
            buttonEditUniform.UseVisualStyleBackColor = false;
            buttonEditUniform.Click += ButtonEditUniform_Click;
            // 
            // buttonDeleteUniform
            // 
            buttonDeleteUniform.BackColor = Color.LightCoral;
            buttonDeleteUniform.Enabled = false;
            buttonDeleteUniform.Location = new Point(246, 7);
            buttonDeleteUniform.Margin = new Padding(3, 4, 3, 4);
            buttonDeleteUniform.Name = "buttonDeleteUniform";
            buttonDeleteUniform.Size = new Size(114, 37);
            buttonDeleteUniform.TabIndex = 2;
            buttonDeleteUniform.Text = "✗ Delete";
            buttonDeleteUniform.UseVisualStyleBackColor = false;
            buttonDeleteUniform.Click += ButtonDeleteUniform_Click;
            // 
            // buttonAssignUniform
            // 
            buttonAssignUniform.BackColor = Color.LightSkyBlue;
            buttonAssignUniform.Enabled = false;
            buttonAssignUniform.Location = new Point(6, 49);
            buttonAssignUniform.Margin = new Padding(3, 4, 3, 4);
            buttonAssignUniform.Name = "buttonAssignUniform";
            buttonAssignUniform.Size = new Size(177, 37);
            buttonAssignUniform.TabIndex = 5;
            buttonAssignUniform.Text = "Assign to Student";
            buttonAssignUniform.UseVisualStyleBackColor = false;
            buttonAssignUniform.Click += ButtonAssignUniform_Click;
            // 
            // buttonUnassignUniform
            // 
            buttonUnassignUniform.BackColor = Color.LightSalmon;
            buttonUnassignUniform.Enabled = false;
            buttonUnassignUniform.Location = new Point(189, 49);
            buttonUnassignUniform.Margin = new Padding(3, 4, 3, 4);
            buttonUnassignUniform.Name = "buttonUnassignUniform";
            buttonUnassignUniform.Size = new Size(171, 37);
            buttonUnassignUniform.TabIndex = 6;
            buttonUnassignUniform.Text = "Unassign Student";
            buttonUnassignUniform.UseVisualStyleBackColor = false;
            buttonUnassignUniform.Click += ButtonUnassignUniform_Click;
            // 
            // buttonCheckOutUniform
            // 
            buttonCheckOutUniform.BackColor = Color.LightYellow;
            buttonCheckOutUniform.Enabled = false;
            buttonCheckOutUniform.Location = new Point(6, 92);
            buttonCheckOutUniform.Margin = new Padding(3, 4, 3, 4);
            buttonCheckOutUniform.Name = "buttonCheckOutUniform";
            buttonCheckOutUniform.Size = new Size(177, 37);
            buttonCheckOutUniform.TabIndex = 3;
            buttonCheckOutUniform.Text = "Check Out/In";
            buttonCheckOutUniform.UseVisualStyleBackColor = false;
            buttonCheckOutUniform.Click += ButtonCheckOutUniform_Click;
            // 
            // buttonUpdateConditions
            // 
            buttonUpdateConditions.BackColor = Color.LightGoldenrodYellow;
            buttonUpdateConditions.Enabled = false;
            buttonUpdateConditions.Location = new Point(189, 92);
            buttonUpdateConditions.Margin = new Padding(3, 4, 3, 4);
            buttonUpdateConditions.Name = "buttonUpdateConditions";
            buttonUpdateConditions.Size = new Size(171, 37);
            buttonUpdateConditions.TabIndex = 4;
            buttonUpdateConditions.Text = "Update Conditions";
            buttonUpdateConditions.UseVisualStyleBackColor = false;
            buttonUpdateConditions.Click += ButtonUpdateConditions_Click;
            // 
            // labelUniforms
            // 
            labelUniforms.Location = new Point(0, 0);
            labelUniforms.Name = "labelUniforms";
            labelUniforms.Size = new Size(114, 31);
            labelUniforms.TabIndex = 3;
            // 
            // panelStudents
            // 
            panelStudents.Controls.Add(dataGridViewStudents);
            panelStudents.Controls.Add(panelStudentsButtons);
            panelStudents.Controls.Add(labelStudents);
            panelStudents.Dock = DockStyle.Fill;
            panelStudents.Location = new Point(375, 3);
            panelStudents.Margin = new Padding(2, 3, 2, 3);
            panelStudents.Name = "panelStudents";
            panelStudents.Size = new Size(369, 488);
            panelStudents.TabIndex = 1;
            // 
            // dataGridViewStudents
            // 
            dataGridViewStudents.AllowUserToAddRows = false;
            dataGridViewStudents.AllowUserToDeleteRows = false;
            dataGridViewStudents.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewStudents.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewStudents.ContextMenuStrip = contextMenuStripStudents;
            dataGridViewStudents.Dock = DockStyle.Fill;
            dataGridViewStudents.Location = new Point(0, 32);
            dataGridViewStudents.Margin = new Padding(2, 3, 2, 3);
            dataGridViewStudents.MultiSelect = false;
            dataGridViewStudents.Name = "dataGridViewStudents";
            dataGridViewStudents.ReadOnly = true;
            dataGridViewStudents.RowHeadersWidth = 62;
            dataGridViewStudents.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewStudents.Size = new Size(369, 403);
            dataGridViewStudents.TabIndex = 1;
            dataGridViewStudents.CellDoubleClick += DataGridViewStudents_CellDoubleClick;
            dataGridViewStudents.SelectionChanged += DataGridViewStudents_SelectionChanged;
            // 
            // contextMenuStripStudents
            // 
            contextMenuStripStudents.ImageScalingSize = new Size(20, 20);
            contextMenuStripStudents.Items.AddRange(new ToolStripItem[] { addStudentToolStripMenuItem, editStudentToolStripMenuItem, deleteStudentToolStripMenuItem });
            contextMenuStripStudents.Name = "contextMenuStripStudents";
            contextMenuStripStudents.Size = new Size(178, 76);
            // 
            // addStudentToolStripMenuItem
            // 
            addStudentToolStripMenuItem.Name = "addStudentToolStripMenuItem";
            addStudentToolStripMenuItem.Size = new Size(177, 24);
            addStudentToolStripMenuItem.Text = "Add Student...";
            addStudentToolStripMenuItem.Click += AddStudentToolStripMenuItem_Click;
            // 
            // editStudentToolStripMenuItem
            // 
            editStudentToolStripMenuItem.Name = "editStudentToolStripMenuItem";
            editStudentToolStripMenuItem.Size = new Size(177, 24);
            editStudentToolStripMenuItem.Text = "Edit Student...";
            editStudentToolStripMenuItem.Click += EditStudentToolStripMenuItem_Click;
            // 
            // deleteStudentToolStripMenuItem
            // 
            deleteStudentToolStripMenuItem.Name = "deleteStudentToolStripMenuItem";
            deleteStudentToolStripMenuItem.Size = new Size(177, 24);
            deleteStudentToolStripMenuItem.Text = "Delete Student";
            deleteStudentToolStripMenuItem.Click += DeleteStudentToolStripMenuItem_Click;
            // 
            // panelStudentsButtons
            // 
            panelStudentsButtons.Controls.Add(buttonAddStudent);
            panelStudentsButtons.Controls.Add(buttonEditStudent);
            panelStudentsButtons.Controls.Add(buttonDeleteStudent);
            panelStudentsButtons.Dock = DockStyle.Bottom;
            panelStudentsButtons.Location = new Point(0, 435);
            panelStudentsButtons.Margin = new Padding(3, 4, 3, 4);
            panelStudentsButtons.Name = "panelStudentsButtons";
            panelStudentsButtons.Size = new Size(369, 53);
            panelStudentsButtons.TabIndex = 2;
            panelStudentsButtons.Visible = false;
            // 
            // buttonAddStudent
            // 
            buttonAddStudent.BackColor = Color.LightGreen;
            buttonAddStudent.Location = new Point(6, 7);
            buttonAddStudent.Margin = new Padding(3, 4, 3, 4);
            buttonAddStudent.Name = "buttonAddStudent";
            buttonAddStudent.Size = new Size(114, 40);
            buttonAddStudent.TabIndex = 0;
            buttonAddStudent.Text = "+ Add";
            buttonAddStudent.UseVisualStyleBackColor = false;
            buttonAddStudent.Click += ButtonAddStudent_Click;
            // 
            // buttonEditStudent
            // 
            buttonEditStudent.BackColor = Color.LightBlue;
            buttonEditStudent.Enabled = false;
            buttonEditStudent.Location = new Point(126, 7);
            buttonEditStudent.Margin = new Padding(3, 4, 3, 4);
            buttonEditStudent.Name = "buttonEditStudent";
            buttonEditStudent.Size = new Size(114, 40);
            buttonEditStudent.TabIndex = 1;
            buttonEditStudent.Text = "Edit";
            buttonEditStudent.UseVisualStyleBackColor = false;
            buttonEditStudent.Click += ButtonEditStudent_Click;
            // 
            // buttonDeleteStudent
            // 
            buttonDeleteStudent.BackColor = Color.LightCoral;
            buttonDeleteStudent.Enabled = false;
            buttonDeleteStudent.Location = new Point(246, 7);
            buttonDeleteStudent.Margin = new Padding(3, 4, 3, 4);
            buttonDeleteStudent.Name = "buttonDeleteStudent";
            buttonDeleteStudent.Size = new Size(114, 40);
            buttonDeleteStudent.TabIndex = 2;
            buttonDeleteStudent.Text = "✗ Delete";
            buttonDeleteStudent.UseVisualStyleBackColor = false;
            buttonDeleteStudent.Click += ButtonDeleteStudent_Click;
            // 
            // labelStudents
            // 
            labelStudents.BackColor = SystemColors.ControlDark;
            labelStudents.Dock = DockStyle.Top;
            labelStudents.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            labelStudents.Location = new Point(0, 0);
            labelStudents.Margin = new Padding(2, 0, 2, 0);
            labelStudents.Name = "labelStudents";
            labelStudents.Size = new Size(369, 32);
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
            panelUsers.Location = new Point(748, 3);
            panelUsers.Margin = new Padding(2, 3, 2, 3);
            panelUsers.Name = "panelUsers";
            panelUsers.Size = new Size(370, 488);
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
            dataGridViewUsers.Location = new Point(0, 32);
            dataGridViewUsers.Margin = new Padding(2, 3, 2, 3);
            dataGridViewUsers.MultiSelect = false;
            dataGridViewUsers.Name = "dataGridViewUsers";
            dataGridViewUsers.ReadOnly = true;
            dataGridViewUsers.RowHeadersWidth = 62;
            dataGridViewUsers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewUsers.Size = new Size(370, 403);
            dataGridViewUsers.TabIndex = 1;
            dataGridViewUsers.SelectionChanged += DataGridViewUsers_SelectionChanged;
            // 
            // contextMenuStripUsers
            // 
            contextMenuStripUsers.ImageScalingSize = new Size(20, 20);
            contextMenuStripUsers.Items.AddRange(new ToolStripItem[] { changeRoleToolStripMenuItem, removeFromOrgToolStripMenuItem });
            contextMenuStripUsers.Name = "contextMenuStripUsers";
            contextMenuStripUsers.Size = new Size(198, 52);
            // 
            // changeRoleToolStripMenuItem
            // 
            changeRoleToolStripMenuItem.Name = "changeRoleToolStripMenuItem";
            changeRoleToolStripMenuItem.Size = new Size(197, 24);
            changeRoleToolStripMenuItem.Text = "Change Role...";
            changeRoleToolStripMenuItem.Click += ChangeRoleToolStripMenuItem_Click;
            // 
            // removeFromOrgToolStripMenuItem
            // 
            removeFromOrgToolStripMenuItem.Name = "removeFromOrgToolStripMenuItem";
            removeFromOrgToolStripMenuItem.Size = new Size(197, 24);
            removeFromOrgToolStripMenuItem.Text = "Remove from Org";
            removeFromOrgToolStripMenuItem.Click += RemoveFromOrgToolStripMenuItem_Click;
            // 
            // panelUsersButtons
            // 
            panelUsersButtons.Controls.Add(buttonChangeUserRole);
            panelUsersButtons.Controls.Add(buttonRemoveUserFromOrg);
            panelUsersButtons.Dock = DockStyle.Bottom;
            panelUsersButtons.Location = new Point(0, 435);
            panelUsersButtons.Margin = new Padding(3, 4, 3, 4);
            panelUsersButtons.Name = "panelUsersButtons";
            panelUsersButtons.Size = new Size(370, 53);
            panelUsersButtons.TabIndex = 2;
            panelUsersButtons.Visible = false;
            // 
            // buttonChangeUserRole
            // 
            buttonChangeUserRole.BackColor = Color.LightBlue;
            buttonChangeUserRole.Enabled = false;
            buttonChangeUserRole.Location = new Point(6, 7);
            buttonChangeUserRole.Margin = new Padding(3, 4, 3, 4);
            buttonChangeUserRole.Name = "buttonChangeUserRole";
            buttonChangeUserRole.Size = new Size(177, 40);
            buttonChangeUserRole.TabIndex = 0;
            buttonChangeUserRole.Text = "Change Role";
            buttonChangeUserRole.UseVisualStyleBackColor = false;
            buttonChangeUserRole.Click += ButtonChangeUserRole_Click;
            // 
            // buttonRemoveUserFromOrg
            // 
            buttonRemoveUserFromOrg.BackColor = Color.LightCoral;
            buttonRemoveUserFromOrg.Enabled = false;
            buttonRemoveUserFromOrg.Location = new Point(189, 7);
            buttonRemoveUserFromOrg.Margin = new Padding(3, 4, 3, 4);
            buttonRemoveUserFromOrg.Name = "buttonRemoveUserFromOrg";
            buttonRemoveUserFromOrg.Size = new Size(177, 40);
            buttonRemoveUserFromOrg.TabIndex = 1;
            buttonRemoveUserFromOrg.Text = "✗ Remove";
            buttonRemoveUserFromOrg.UseVisualStyleBackColor = false;
            buttonRemoveUserFromOrg.Click += ButtonRemoveUserFromOrg_Click;
            // 
            // labelUsers
            // 
            labelUsers.BackColor = SystemColors.ControlDark;
            labelUsers.Dock = DockStyle.Top;
            labelUsers.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            labelUsers.Location = new Point(0, 0);
            labelUsers.Margin = new Padding(2, 0, 2, 0);
            labelUsers.Name = "labelUsers";
            labelUsers.Size = new Size(370, 32);
            labelUsers.TabIndex = 0;
            labelUsers.Text = "Users";
            labelUsers.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new Size(262, 6);
            // 
            // toolStripMenuItem2
            // 
            toolStripMenuItem2.Name = "toolStripMenuItem2";
            toolStripMenuItem2.Size = new Size(262, 6);
            // 
            // UniformManagerAdminHome
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1120, 520);
            Controls.Add(tableLayoutPanel1);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Margin = new Padding(2, 3, 2, 3);
            Name = "UniformManagerAdminHome";
            Text = "Uniform Manager";
            Load += UniformManagerAdminHome_Load;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            tableLayoutPanel1.ResumeLayout(false);
            panelUniforms.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridViewUniforms).EndInit();
            contextMenuStripUniforms.ResumeLayout(false);
            panelUniformsButtons.ResumeLayout(false);
            panelStudents.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridViewStudents).EndInit();
            contextMenuStripStudents.ResumeLayout(false);
            panelStudentsButtons.ResumeLayout(false);
            panelUsers.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridViewUsers).EndInit();
            contextMenuStripUsers.ResumeLayout(false);
            panelUsersButtons.ResumeLayout(false);
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
        private ToolStripMenuItem checkMissingAssignmentsToolStripMenuItem;
        private ToolStripMenuItem searchAndEditStudentToolStripMenuItem;
        private ToolStripMenuItem promoteStudentsToolStripMenuItem;
        private ToolStripMenuItem unassignAllUniformsToolStripMenuItem;
        private ToolStripMenuItem deleteAllUniformsToolStripMenuItem;
        private ToolStripMenuItem deleteAllStudentsToolStripMenuItem;
        private TableLayoutPanel tableLayoutPanel1;
        private Panel panelUniforms;
        private DataGridView dataGridViewUniforms;
        private Panel panelUniformsButtons;
        private Button buttonAddUniform;
        private Button buttonEditUniform;
        private Button buttonDeleteUniform;
        private Button buttonAssignUniform;
        private Button buttonUnassignUniform;
        private Button buttonCheckOutUniform;
        private Button buttonUpdateConditions;
        private ContextMenuStrip contextMenuStripUniforms;
        private ToolStripMenuItem addUniformToolStripMenuItem;
        private ToolStripMenuItem editUniformToolStripMenuItem;
        private ToolStripMenuItem deleteUniformToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem assignUniformToolStripMenuItem;
        private ToolStripMenuItem unassignUniformToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripMenuItem checkOutInToolStripMenuItem;
        private ToolStripMenuItem updateConditionsToolStripMenuItem;
        private Label labelUniforms;
        private Panel panelStudents;
        private DataGridView dataGridViewStudents;
        private Panel panelStudentsButtons;
        private Button buttonAddStudent;
        private Button buttonEditStudent;
        private Button buttonDeleteStudent;
        private ContextMenuStrip contextMenuStripStudents;
        private ToolStripMenuItem addStudentToolStripMenuItem;
        private ToolStripMenuItem editStudentToolStripMenuItem;
        private ToolStripMenuItem deleteStudentToolStripMenuItem;
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
        private ToolStripMenuItem importUniformsToolStripMenuItem;
        private ToolStripMenuItem importStudentsToolStripMenuItem;
        private ToolStripSeparator toolStripMenuItem1;
        private ToolStripSeparator toolStripMenuItem2;
    }
}