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
            tableLayoutPanel1 = new TableLayoutPanel();
            panelUniforms = new Panel();
            dataGridViewUniforms = new DataGridView();
            labelUniforms = new Label();
            panelStudents = new Panel();
            dataGridViewStudents = new DataGridView();
            labelStudents = new Label();
            panelUsers = new Panel();
            dataGridViewUsers = new DataGridView();
            labelUsers = new Label();
            menuStrip1.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            panelUniforms.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridViewUniforms).BeginInit();
            panelStudents.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridViewStudents).BeginInit();
            panelUsers.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridViewUsers).BeginInit();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(24, 24);
            menuStrip1.Items.AddRange(new ToolStripItem[] { homeToolStripMenuItem, navigateToolStripMenuItem, fileToolStripMenuItem, editToolStripMenuItem, orgToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(1400, 33);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // homeToolStripMenuItem
            // 
            homeToolStripMenuItem.Name = "homeToolStripMenuItem";
            homeToolStripMenuItem.Size = new Size(77, 29);
            homeToolStripMenuItem.Text = "Home";
            // 
            // navigateToolStripMenuItem
            // 
            navigateToolStripMenuItem.Name = "navigateToolStripMenuItem";
            navigateToolStripMenuItem.Size = new Size(98, 29);
            navigateToolStripMenuItem.Text = "Navigate";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(54, 29);
            fileToolStripMenuItem.Text = "File";
            // 
            // editToolStripMenuItem
            // 
            editToolStripMenuItem.Name = "editToolStripMenuItem";
            editToolStripMenuItem.Size = new Size(58, 29);
            editToolStripMenuItem.Text = "Edit";
            // 
            // orgToolStripMenuItem
            // 
            orgToolStripMenuItem.Name = "orgToolStripMenuItem";
            orgToolStripMenuItem.Size = new Size(59, 29);
            orgToolStripMenuItem.Text = "Org";
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
            tableLayoutPanel1.Location = new Point(0, 33);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new Size(1400, 617);
            tableLayoutPanel1.TabIndex = 1;
            // 
            // panelUniforms
            // 
            panelUniforms.Controls.Add(dataGridViewUniforms);
            panelUniforms.Controls.Add(labelUniforms);
            panelUniforms.Dock = DockStyle.Fill;
            panelUniforms.Location = new Point(3, 3);
            panelUniforms.Name = "panelUniforms";
            panelUniforms.Size = new Size(460, 611);
            panelUniforms.TabIndex = 0;
            // 
            // dataGridViewUniforms
            // 
            dataGridViewUniforms.AllowUserToAddRows = false;
            dataGridViewUniforms.AllowUserToDeleteRows = false;
            dataGridViewUniforms.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewUniforms.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewUniforms.Dock = DockStyle.Fill;
            dataGridViewUniforms.Location = new Point(0, 40);
            dataGridViewUniforms.Name = "dataGridViewUniforms";
            dataGridViewUniforms.ReadOnly = true;
            dataGridViewUniforms.RowHeadersWidth = 62;
            dataGridViewUniforms.Size = new Size(460, 571);
            dataGridViewUniforms.TabIndex = 1;
            // 
            // labelUniforms
            // 
            labelUniforms.BackColor = SystemColors.ControlDark;
            labelUniforms.Dock = DockStyle.Top;
            labelUniforms.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            labelUniforms.Location = new Point(0, 0);
            labelUniforms.Name = "labelUniforms";
            labelUniforms.Size = new Size(460, 40);
            labelUniforms.TabIndex = 0;
            labelUniforms.Text = "Uniforms";
            labelUniforms.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // panelStudents
            // 
            panelStudents.Controls.Add(dataGridViewStudents);
            panelStudents.Controls.Add(labelStudents);
            panelStudents.Dock = DockStyle.Fill;
            panelStudents.Location = new Point(469, 3);
            panelStudents.Name = "panelStudents";
            panelStudents.Size = new Size(460, 611);
            panelStudents.TabIndex = 1;
            // 
            // dataGridViewStudents
            // 
            dataGridViewStudents.AllowUserToAddRows = false;
            dataGridViewStudents.AllowUserToDeleteRows = false;
            dataGridViewStudents.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewStudents.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewStudents.Dock = DockStyle.Fill;
            dataGridViewStudents.Location = new Point(0, 40);
            dataGridViewStudents.Name = "dataGridViewStudents";
            dataGridViewStudents.ReadOnly = true;
            dataGridViewStudents.RowHeadersWidth = 62;
            dataGridViewStudents.Size = new Size(460, 571);
            dataGridViewStudents.TabIndex = 1;
            // 
            // labelStudents
            // 
            labelStudents.BackColor = SystemColors.ControlDark;
            labelStudents.Dock = DockStyle.Top;
            labelStudents.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            labelStudents.Location = new Point(0, 0);
            labelStudents.Name = "labelStudents";
            labelStudents.Size = new Size(460, 40);
            labelStudents.TabIndex = 0;
            labelStudents.Text = "Students";
            labelStudents.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // panelUsers
            // 
            panelUsers.Controls.Add(dataGridViewUsers);
            panelUsers.Controls.Add(labelUsers);
            panelUsers.Dock = DockStyle.Fill;
            panelUsers.Location = new Point(935, 3);
            panelUsers.Name = "panelUsers";
            panelUsers.Size = new Size(462, 611);
            panelUsers.TabIndex = 2;
            // 
            // dataGridViewUsers
            // 
            dataGridViewUsers.AllowUserToAddRows = false;
            dataGridViewUsers.AllowUserToDeleteRows = false;
            dataGridViewUsers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewUsers.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewUsers.Dock = DockStyle.Fill;
            dataGridViewUsers.Location = new Point(0, 40);
            dataGridViewUsers.Name = "dataGridViewUsers";
            dataGridViewUsers.ReadOnly = true;
            dataGridViewUsers.RowHeadersWidth = 62;
            dataGridViewUsers.Size = new Size(462, 571);
            dataGridViewUsers.TabIndex = 1;
            // 
            // labelUsers
            // 
            labelUsers.BackColor = SystemColors.ControlDark;
            labelUsers.Dock = DockStyle.Top;
            labelUsers.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            labelUsers.Location = new Point(0, 0);
            labelUsers.Name = "labelUsers";
            labelUsers.Size = new Size(462, 40);
            labelUsers.TabIndex = 0;
            labelUsers.Text = "Users";
            labelUsers.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // UniformManagerAdminHome
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1400, 650);
            Controls.Add(tableLayoutPanel1);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
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
        private TableLayoutPanel tableLayoutPanel1;
        private Panel panelUniforms;
        private DataGridView dataGridViewUniforms;
        private Label labelUniforms;
        private Panel panelStudents;
        private DataGridView dataGridViewStudents;
        private Label labelStudents;
        private Panel panelUsers;
        private DataGridView dataGridViewUsers;
        private Label labelUsers;
    }
}