namespace Computer_Science_IA___Uniform_Manager
{
    partial class ManageOrganizationUsersForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.labelTitle = new System.Windows.Forms.Label();
            this.dataGridViewUsers = new System.Windows.Forms.DataGridView();
            this.buttonChangeRole = new System.Windows.Forms.Button();
            this.buttonRemoveUser = new System.Windows.Forms.Button();
            this.buttonRefresh = new System.Windows.Forms.Button();
            this.buttonClose = new System.Windows.Forms.Button();
            this.labelStatus = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBoxStats = new System.Windows.Forms.GroupBox();
            this.labelAdmins = new System.Windows.Forms.Label();
            this.labelUsers = new System.Windows.Forms.Label();
            this.labelViewers = new System.Windows.Forms.Label();
            this.labelTotal = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewUsers)).BeginInit();
            this.panel1.SuspendLayout();
            this.groupBoxStats.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelTitle
            // 
            this.labelTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.labelTitle.Location = new System.Drawing.Point(0, 0);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(1000, 50);
            this.labelTitle.TabIndex = 0;
            this.labelTitle.Text = "Manage Organization Users";
            this.labelTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // dataGridViewUsers
            // 
            this.dataGridViewUsers.AllowUserToAddRows = false;
            this.dataGridViewUsers.AllowUserToDeleteRows = false;
            this.dataGridViewUsers.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewUsers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewUsers.Location = new System.Drawing.Point(20, 60);
            this.dataGridViewUsers.MultiSelect = false;
            this.dataGridViewUsers.Name = "dataGridViewUsers";
            this.dataGridViewUsers.ReadOnly = true;
            this.dataGridViewUsers.RowHeadersWidth = 62;
            this.dataGridViewUsers.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewUsers.Size = new System.Drawing.Size(850, 400);
            this.dataGridViewUsers.TabIndex = 1;
            this.dataGridViewUsers.SelectionChanged += new System.EventHandler(this.DataGridViewUsers_SelectionChanged);
            // 
            // groupBoxStats
            // 
            this.groupBoxStats.Controls.Add(this.labelTotal);
            this.groupBoxStats.Controls.Add(this.labelAdmins);
            this.groupBoxStats.Controls.Add(this.labelUsers);
            this.groupBoxStats.Controls.Add(this.labelViewers);
            this.groupBoxStats.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.groupBoxStats.Location = new System.Drawing.Point(890, 60);
            this.groupBoxStats.Name = "groupBoxStats";
            this.groupBoxStats.Size = new System.Drawing.Size(90, 200);
            this.groupBoxStats.TabIndex = 2;
            this.groupBoxStats.TabStop = false;
            this.groupBoxStats.Text = "Stats";
            // 
            // labelTotal
            // 
            this.labelTotal.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelTotal.Location = new System.Drawing.Point(10, 30);
            this.labelTotal.Name = "labelTotal";
            this.labelTotal.Size = new System.Drawing.Size(150, 30);
            this.labelTotal.TabIndex = 0;
            this.labelTotal.Text = "Total: 0";
            // 
            // labelAdmins
            // 
            this.labelAdmins.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelAdmins.ForeColor = System.Drawing.Color.Green;
            this.labelAdmins.Location = new System.Drawing.Point(10, 70);
            this.labelAdmins.Name = "labelAdmins";
            this.labelAdmins.Size = new System.Drawing.Size(150, 30);
            this.labelAdmins.TabIndex = 1;
            this.labelAdmins.Text = "Admins: 0";
            // 
            // labelUsers
            // 
            this.labelUsers.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelUsers.ForeColor = System.Drawing.Color.Blue;
            this.labelUsers.Location = new System.Drawing.Point(10, 110);
            this.labelUsers.Name = "labelUsers";
            this.labelUsers.Size = new System.Drawing.Size(150, 30);
            this.labelUsers.TabIndex = 2;
            this.labelUsers.Text = "Users: 0";
            // 
            // labelViewers
            // 
            this.labelViewers.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelViewers.ForeColor = System.Drawing.Color.Gray;
            this.labelViewers.Location = new System.Drawing.Point(10, 150);
            this.labelViewers.Name = "labelViewers";
            this.labelViewers.Size = new System.Drawing.Size(150, 30);
            this.labelViewers.TabIndex = 3;
            this.labelViewers.Text = "Viewers: 0";
            // 
            // buttonChangeRole
            // 
            this.buttonChangeRole.BackColor = System.Drawing.Color.LightBlue;
            this.buttonChangeRole.Enabled = false;
            this.buttonChangeRole.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.buttonChangeRole.Location = new System.Drawing.Point(20, 15);
            this.buttonChangeRole.Name = "buttonChangeRole";
            this.buttonChangeRole.Size = new System.Drawing.Size(150, 45);
            this.buttonChangeRole.TabIndex = 3;
            this.buttonChangeRole.Text = "Change Role";
            this.buttonChangeRole.UseVisualStyleBackColor = false;
            this.buttonChangeRole.Click += new System.EventHandler(this.ButtonChangeRole_Click);
            // 
            // buttonRemoveUser
            // 
            this.buttonRemoveUser.BackColor = System.Drawing.Color.LightCoral;
            this.buttonRemoveUser.Enabled = false;
            this.buttonRemoveUser.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.buttonRemoveUser.Location = new System.Drawing.Point(190, 15);
            this.buttonRemoveUser.Name = "buttonRemoveUser";
            this.buttonRemoveUser.Size = new System.Drawing.Size(150, 45);
            this.buttonRemoveUser.TabIndex = 4;
            this.buttonRemoveUser.Text = "? Remove User";
            this.buttonRemoveUser.UseVisualStyleBackColor = false;
            this.buttonRemoveUser.Click += new System.EventHandler(this.ButtonRemoveUser_Click);
            // 
            // buttonRefresh
            // 
            this.buttonRefresh.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.buttonRefresh.Location = new System.Drawing.Point(680, 15);
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.Size = new System.Drawing.Size(130, 45);
            this.buttonRefresh.TabIndex = 5;
            this.buttonRefresh.Text = "?? Refresh";
            this.buttonRefresh.UseVisualStyleBackColor = true;
            this.buttonRefresh.Click += new System.EventHandler(this.ButtonRefresh_Click);
            // 
            // buttonClose
            // 
            this.buttonClose.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.buttonClose.Location = new System.Drawing.Point(830, 15);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(130, 45);
            this.buttonClose.TabIndex = 6;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.ButtonClose_Click);
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.ForeColor = System.Drawing.SystemColors.GrayText;
            this.labelStatus.Location = new System.Drawing.Point(370, 25);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(120, 25);
            this.labelStatus.TabIndex = 7;
            this.labelStatus.Text = "Loading...";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.buttonChangeRole);
            this.panel1.Controls.Add(this.labelStatus);
            this.panel1.Controls.Add(this.buttonRemoveUser);
            this.panel1.Controls.Add(this.buttonClose);
            this.panel1.Controls.Add(this.buttonRefresh);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 475);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1000, 75);
            this.panel1.TabIndex = 8;
            // 
            // ManageOrganizationUsersForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1000, 550);
            this.Controls.Add(this.groupBoxStats);
            this.Controls.Add(this.dataGridViewUsers);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.labelTitle);
            this.MinimumSize = new System.Drawing.Size(900, 500);
            this.Name = "ManageOrganizationUsersForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Manage Users";
            this.Load += new System.EventHandler(this.ManageOrganizationUsersForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewUsers)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBoxStats.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.DataGridView dataGridViewUsers;
        private System.Windows.Forms.Button buttonChangeRole;
        private System.Windows.Forms.Button buttonRemoveUser;
        private System.Windows.Forms.Button buttonRefresh;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBoxStats;
        private System.Windows.Forms.Label labelAdmins;
        private System.Windows.Forms.Label labelUsers;
        private System.Windows.Forms.Label labelViewers;
        private System.Windows.Forms.Label labelTotal;
    }
}
