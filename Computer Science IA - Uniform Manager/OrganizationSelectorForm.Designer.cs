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
            labelTitle = new Label();
            labelSubtitle = new Label();
            listBoxOrganizations = new ListBox();
            buttonSelect = new Button();
            buttonCreateOrg = new Button();
            buttonJoinOrg = new Button();
            buttonRefresh = new Button();
            buttonLogout = new Button();
            labelUserInfo = new Label();
            panelHeader = new Panel();
            panelFooter = new Panel();
            panelHeader.SuspendLayout();
            panelFooter.SuspendLayout();
            SuspendLayout();
            // 
            // labelTitle
            // 
            labelTitle.Dock = DockStyle.Top;
            labelTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            labelTitle.Location = new Point(0, 18);
            labelTitle.Margin = new Padding(2, 0, 2, 0);
            labelTitle.Name = "labelTitle";
            labelTitle.Size = new Size(420, 30);
            labelTitle.TabIndex = 0;
            labelTitle.Text = "Select Organization";
            labelTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // labelSubtitle
            // 
            labelSubtitle.Dock = DockStyle.Top;
            labelSubtitle.Font = new Font("Segoe UI", 10F);
            labelSubtitle.ForeColor = SystemColors.GrayText;
            labelSubtitle.Location = new Point(0, 0);
            labelSubtitle.Margin = new Padding(2, 0, 2, 0);
            labelSubtitle.Name = "labelSubtitle";
            labelSubtitle.Size = new Size(420, 18);
            labelSubtitle.TabIndex = 1;
            labelSubtitle.Text = "Choose an organization to manage";
            labelSubtitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // listBoxOrganizations
            // 
            listBoxOrganizations.Font = new Font("Segoe UI", 12F);
            listBoxOrganizations.FormattingEnabled = true;
            listBoxOrganizations.ItemHeight = 21;
            listBoxOrganizations.Location = new Point(35, 90);
            listBoxOrganizations.Margin = new Padding(2, 2, 2, 2);
            listBoxOrganizations.Name = "listBoxOrganizations";
            listBoxOrganizations.Size = new Size(351, 151);
            listBoxOrganizations.TabIndex = 2;
            listBoxOrganizations.DoubleClick += ListBoxOrganizations_DoubleClick;
            // 
            // buttonSelect
            // 
            buttonSelect.Font = new Font("Segoe UI", 12F);
            buttonSelect.Location = new Point(280, 252);
            buttonSelect.Margin = new Padding(2, 2, 2, 2);
            buttonSelect.Name = "buttonSelect";
            buttonSelect.Size = new Size(106, 27);
            buttonSelect.TabIndex = 3;
            buttonSelect.Text = "Select";
            buttonSelect.UseVisualStyleBackColor = true;
            buttonSelect.Click += ButtonSelect_Click;
            // 
            // buttonCreateOrg
            // 
            buttonCreateOrg.Font = new Font("Segoe UI", 10F);
            buttonCreateOrg.Location = new Point(10, 6);
            buttonCreateOrg.Margin = new Padding(2, 2, 2, 2);
            buttonCreateOrg.Name = "buttonCreateOrg";
            buttonCreateOrg.Size = new Size(95, 27);
            buttonCreateOrg.TabIndex = 4;
            buttonCreateOrg.Text = "Create New";
            buttonCreateOrg.UseVisualStyleBackColor = true;
            buttonCreateOrg.Click += ButtonCreateOrg_Click;
            // 
            // buttonJoinOrg
            // 
            buttonJoinOrg.Font = new Font("Segoe UI", 10F);
            buttonJoinOrg.Location = new Point(110, 6);
            buttonJoinOrg.Margin = new Padding(2, 2, 2, 2);
            buttonJoinOrg.Name = "buttonJoinOrg";
            buttonJoinOrg.Size = new Size(95, 27);
            buttonJoinOrg.TabIndex = 5;
            buttonJoinOrg.Text = "Join Existing";
            buttonJoinOrg.UseVisualStyleBackColor = true;
            buttonJoinOrg.Click += ButtonJoinOrg_Click;
            // 
            // buttonRefresh
            // 
            buttonRefresh.Font = new Font("Segoe UI", 10F);
            buttonRefresh.Location = new Point(315, 6);
            buttonRefresh.Margin = new Padding(2, 2, 2, 2);
            buttonRefresh.Name = "buttonRefresh";
            buttonRefresh.Size = new Size(95, 27);
            buttonRefresh.TabIndex = 6;
            buttonRefresh.Text = "Refresh";
            buttonRefresh.UseVisualStyleBackColor = true;
            buttonRefresh.Click += ButtonRefresh_Click;
            // 
            // buttonLogout
            // 
            buttonLogout.Location = new Point(35, 258);
            buttonLogout.Margin = new Padding(2, 2, 2, 2);
            buttonLogout.Name = "buttonLogout";
            buttonLogout.Size = new Size(70, 21);
            buttonLogout.TabIndex = 5;
            buttonLogout.Text = "Logout";
            buttonLogout.UseVisualStyleBackColor = true;
            buttonLogout.Click += ButtonLogout_Click;
            // 
            // labelUserInfo
            // 
            labelUserInfo.AutoSize = true;
            labelUserInfo.Font = new Font("Segoe UI", 9F);
            labelUserInfo.ForeColor = SystemColors.GrayText;
            labelUserInfo.Location = new Point(35, 66);
            labelUserInfo.Margin = new Padding(2, 0, 2, 0);
            labelUserInfo.Name = "labelUserInfo";
            labelUserInfo.Size = new Size(138, 15);
            labelUserInfo.TabIndex = 6;
            labelUserInfo.Text = "Logged in as: User Name";
            // 
            // panelHeader
            // 
            panelHeader.Controls.Add(labelTitle);
            panelHeader.Controls.Add(labelSubtitle);
            panelHeader.Dock = DockStyle.Top;
            panelHeader.Location = new Point(0, 0);
            panelHeader.Margin = new Padding(2, 2, 2, 2);
            panelHeader.Name = "panelHeader";
            panelHeader.Size = new Size(420, 48);
            panelHeader.TabIndex = 7;
            // 
            // panelFooter
            // 
            panelFooter.Controls.Add(buttonCreateOrg);
            panelFooter.Controls.Add(buttonJoinOrg);
            panelFooter.Controls.Add(buttonRefresh);
            panelFooter.Dock = DockStyle.Bottom;
            panelFooter.Location = new Point(0, 295);
            panelFooter.Margin = new Padding(2, 2, 2, 2);
            panelFooter.Name = "panelFooter";
            panelFooter.Size = new Size(420, 48);
            panelFooter.TabIndex = 8;
            // 
            // OrganizationSelectorForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(420, 343);
            Controls.Add(panelFooter);
            Controls.Add(panelHeader);
            Controls.Add(buttonSelect);
            Controls.Add(labelUserInfo);
            Controls.Add(buttonLogout);
            Controls.Add(listBoxOrganizations);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(2, 2, 2, 2);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "OrganizationSelectorForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Select Organization";
            Load += OrganizationSelectorForm_Load;
            panelHeader.ResumeLayout(false);
            panelFooter.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.Label labelSubtitle;
        private System.Windows.Forms.ListBox listBoxOrganizations;
        private System.Windows.Forms.Button buttonSelect;
        private System.Windows.Forms.Button buttonCreateOrg;
        private System.Windows.Forms.Button buttonJoinOrg;
        private System.Windows.Forms.Button buttonRefresh;
        private System.Windows.Forms.Button buttonLogout;
        private System.Windows.Forms.Label labelUserInfo;
        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.Panel panelFooter;
    }
}
