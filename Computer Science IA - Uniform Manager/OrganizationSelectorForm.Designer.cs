namespace Computer_Science_IA___Uniform_Manager
{
    partial class OrganizationSelectorForm
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
            this.labelSubtitle = new System.Windows.Forms.Label();
            this.listBoxOrganizations = new System.Windows.Forms.ListBox();
            this.buttonSelect = new System.Windows.Forms.Button();
            this.buttonCreateOrg = new System.Windows.Forms.Button();
            this.buttonLogout = new System.Windows.Forms.Button();
            this.labelUserInfo = new System.Windows.Forms.Label();
            this.panelHeader = new System.Windows.Forms.Panel();
            this.panelFooter = new System.Windows.Forms.Panel();
            this.panelHeader.SuspendLayout();
            this.panelFooter.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelTitle
            // 
            this.labelTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelTitle.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.labelTitle.Location = new System.Drawing.Point(0, 0);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(600, 50);
            this.labelTitle.TabIndex = 0;
            this.labelTitle.Text = "Select Organization";
            this.labelTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelSubtitle
            // 
            this.labelSubtitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelSubtitle.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.labelSubtitle.ForeColor = System.Drawing.SystemColors.GrayText;
            this.labelSubtitle.Location = new System.Drawing.Point(0, 50);
            this.labelSubtitle.Name = "labelSubtitle";
            this.labelSubtitle.Size = new System.Drawing.Size(600, 30);
            this.labelSubtitle.TabIndex = 1;
            this.labelSubtitle.Text = "Choose an organization to manage";
            this.labelSubtitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // listBoxOrganizations
            // 
            this.listBoxOrganizations.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.listBoxOrganizations.FormattingEnabled = true;
            this.listBoxOrganizations.ItemHeight = 32;
            this.listBoxOrganizations.Location = new System.Drawing.Point(50, 150);
            this.listBoxOrganizations.Name = "listBoxOrganizations";
            this.listBoxOrganizations.Size = new System.Drawing.Size(500, 260);
            this.listBoxOrganizations.TabIndex = 2;
            this.listBoxOrganizations.DoubleClick += new System.EventHandler(this.ListBoxOrganizations_DoubleClick);
            // 
            // buttonSelect
            // 
            this.buttonSelect.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.buttonSelect.Location = new System.Drawing.Point(350, 10);
            this.buttonSelect.Name = "buttonSelect";
            this.buttonSelect.Size = new System.Drawing.Size(200, 45);
            this.buttonSelect.TabIndex = 3;
            this.buttonSelect.Text = "Select";
            this.buttonSelect.UseVisualStyleBackColor = true;
            this.buttonSelect.Click += new System.EventHandler(this.ButtonSelect_Click);
            // 
            // buttonCreateOrg
            // 
            this.buttonCreateOrg.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.buttonCreateOrg.Location = new System.Drawing.Point(50, 10);
            this.buttonCreateOrg.Name = "buttonCreateOrg";
            this.buttonCreateOrg.Size = new System.Drawing.Size(200, 45);
            this.buttonCreateOrg.TabIndex = 4;
            this.buttonCreateOrg.Text = "Create New Org";
            this.buttonCreateOrg.UseVisualStyleBackColor = true;
            this.buttonCreateOrg.Click += new System.EventHandler(this.ButtonCreateOrg_Click);
            // 
            // buttonLogout
            // 
            this.buttonLogout.Location = new System.Drawing.Point(50, 430);
            this.buttonLogout.Name = "buttonLogout";
            this.buttonLogout.Size = new System.Drawing.Size(100, 35);
            this.buttonLogout.TabIndex = 5;
            this.buttonLogout.Text = "Logout";
            this.buttonLogout.UseVisualStyleBackColor = true;
            this.buttonLogout.Click += new System.EventHandler(this.ButtonLogout_Click);
            // 
            // labelUserInfo
            // 
            this.labelUserInfo.AutoSize = true;
            this.labelUserInfo.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelUserInfo.ForeColor = System.Drawing.SystemColors.GrayText;
            this.labelUserInfo.Location = new System.Drawing.Point(50, 110);
            this.labelUserInfo.Name = "labelUserInfo";
            this.labelUserInfo.Size = new System.Drawing.Size(200, 25);
            this.labelUserInfo.TabIndex = 6;
            this.labelUserInfo.Text = "Logged in as: User Name";
            // 
            // panelHeader
            // 
            this.panelHeader.Controls.Add(this.labelTitle);
            this.panelHeader.Controls.Add(this.labelSubtitle);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Location = new System.Drawing.Point(0, 0);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(600, 80);
            this.panelHeader.TabIndex = 7;
            // 
            // panelFooter
            // 
            this.panelFooter.Controls.Add(this.buttonCreateOrg);
            this.panelFooter.Controls.Add(this.buttonSelect);
            this.panelFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelFooter.Location = new System.Drawing.Point(0, 480);
            this.panelFooter.Name = "panelFooter";
            this.panelFooter.Size = new System.Drawing.Size(600, 70);
            this.panelFooter.TabIndex = 8;
            // 
            // OrganizationSelectorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 550);
            this.Controls.Add(this.panelFooter);
            this.Controls.Add(this.panelHeader);
            this.Controls.Add(this.labelUserInfo);
            this.Controls.Add(this.buttonLogout);
            this.Controls.Add(this.listBoxOrganizations);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OrganizationSelectorForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Select Organization";
            this.Load += new System.EventHandler(this.OrganizationSelectorForm_Load);
            this.panelHeader.ResumeLayout(false);
            this.panelFooter.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.Label labelSubtitle;
        private System.Windows.Forms.ListBox listBoxOrganizations;
        private System.Windows.Forms.Button buttonSelect;
        private System.Windows.Forms.Button buttonCreateOrg;
        private System.Windows.Forms.Button buttonLogout;
        private System.Windows.Forms.Label labelUserInfo;
        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.Panel panelFooter;
    }
}
