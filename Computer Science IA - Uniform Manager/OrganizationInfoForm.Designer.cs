namespace Computer_Science_IA___Uniform_Manager
{
    partial class OrganizationInfoForm
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
            this.labelOrgName = new System.Windows.Forms.Label();
            this.textBoxOrgName = new System.Windows.Forms.TextBox();
            this.labelOrgCode = new System.Windows.Forms.Label();
            this.textBoxOrgCode = new System.Windows.Forms.TextBox();
            this.labelDescription = new System.Windows.Forms.Label();
            this.textBoxDescription = new System.Windows.Forms.TextBox();
            this.labelYourRole = new System.Windows.Forms.Label();
            this.textBoxYourRole = new System.Windows.Forms.TextBox();
            this.groupBoxShareCode = new System.Windows.Forms.GroupBox();
            this.labelShareInfo = new System.Windows.Forms.Label();
            this.textBoxShareCode = new System.Windows.Forms.TextBox();
            this.buttonCopyCode = new System.Windows.Forms.Button();
            this.buttonClose = new System.Windows.Forms.Button();
            this.panelInfo = new System.Windows.Forms.Panel();
            this.groupBoxShareCode.SuspendLayout();
            this.panelInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelTitle
            // 
            this.labelTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.labelTitle.Location = new System.Drawing.Point(0, 0);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(550, 50);
            this.labelTitle.TabIndex = 0;
            this.labelTitle.Text = "Organization Information";
            this.labelTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelOrgName
            // 
            this.labelOrgName.AutoSize = true;
            this.labelOrgName.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.labelOrgName.Location = new System.Drawing.Point(20, 20);
            this.labelOrgName.Name = "labelOrgName";
            this.labelOrgName.Size = new System.Drawing.Size(173, 28);
            this.labelOrgName.TabIndex = 1;
            this.labelOrgName.Text = "Organization Name:";
            // 
            // textBoxOrgName
            // 
            this.textBoxOrgName.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.textBoxOrgName.Location = new System.Drawing.Point(25, 50);
            this.textBoxOrgName.Name = "textBoxOrgName";
            this.textBoxOrgName.ReadOnly = true;
            this.textBoxOrgName.Size = new System.Drawing.Size(480, 37);
            this.textBoxOrgName.TabIndex = 2;
            // 
            // labelOrgCode
            // 
            this.labelOrgCode.AutoSize = true;
            this.labelOrgCode.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.labelOrgCode.Location = new System.Drawing.Point(20, 100);
            this.labelOrgCode.Name = "labelOrgCode";
            this.labelOrgCode.Size = new System.Drawing.Size(170, 28);
            this.labelOrgCode.TabIndex = 3;
            this.labelOrgCode.Text = "Organization Code:";
            // 
            // textBoxOrgCode
            // 
            this.textBoxOrgCode.Font = new System.Drawing.Font("Courier New", 14F, System.Drawing.FontStyle.Bold);
            this.textBoxOrgCode.Location = new System.Drawing.Point(25, 130);
            this.textBoxOrgCode.Name = "textBoxOrgCode";
            this.textBoxOrgCode.ReadOnly = true;
            this.textBoxOrgCode.Size = new System.Drawing.Size(200, 39);
            this.textBoxOrgCode.TabIndex = 4;
            this.textBoxOrgCode.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // labelDescription
            // 
            this.labelDescription.AutoSize = true;
            this.labelDescription.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.labelDescription.Location = new System.Drawing.Point(20, 185);
            this.labelDescription.Name = "labelDescription";
            this.labelDescription.Size = new System.Drawing.Size(117, 28);
            this.labelDescription.TabIndex = 5;
            this.labelDescription.Text = "Description:";
            // 
            // textBoxDescription
            // 
            this.textBoxDescription.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.textBoxDescription.Location = new System.Drawing.Point(25, 215);
            this.textBoxDescription.Multiline = true;
            this.textBoxDescription.Name = "textBoxDescription";
            this.textBoxDescription.ReadOnly = true;
            this.textBoxDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxDescription.Size = new System.Drawing.Size(480, 80);
            this.textBoxDescription.TabIndex = 6;
            // 
            // labelYourRole
            // 
            this.labelYourRole.AutoSize = true;
            this.labelYourRole.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.labelYourRole.Location = new System.Drawing.Point(270, 100);
            this.labelYourRole.Name = "labelYourRole";
            this.labelYourRole.Size = new System.Drawing.Size(100, 28);
            this.labelYourRole.TabIndex = 7;
            this.labelYourRole.Text = "Your Role:";
            // 
            // textBoxYourRole
            // 
            this.textBoxYourRole.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.textBoxYourRole.Location = new System.Drawing.Point(275, 130);
            this.textBoxYourRole.Name = "textBoxYourRole";
            this.textBoxYourRole.ReadOnly = true;
            this.textBoxYourRole.Size = new System.Drawing.Size(230, 39);
            this.textBoxYourRole.TabIndex = 8;
            this.textBoxYourRole.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // groupBoxShareCode
            // 
            this.groupBoxShareCode.Controls.Add(this.buttonCopyCode);
            this.groupBoxShareCode.Controls.Add(this.textBoxShareCode);
            this.groupBoxShareCode.Controls.Add(this.labelShareInfo);
            this.groupBoxShareCode.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.groupBoxShareCode.Location = new System.Drawing.Point(25, 315);
            this.groupBoxShareCode.Name = "groupBoxShareCode";
            this.groupBoxShareCode.Size = new System.Drawing.Size(480, 140);
            this.groupBoxShareCode.TabIndex = 9;
            this.groupBoxShareCode.TabStop = false;
            this.groupBoxShareCode.Text = "Share Organization";
            // 
            // labelShareInfo
            // 
            this.labelShareInfo.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelShareInfo.ForeColor = System.Drawing.SystemColors.GrayText;
            this.labelShareInfo.Location = new System.Drawing.Point(15, 30);
            this.labelShareInfo.Name = "labelShareInfo";
            this.labelShareInfo.Size = new System.Drawing.Size(450, 25);
            this.labelShareInfo.TabIndex = 0;
            this.labelShareInfo.Text = "Share this code with others to let them join your organization:";
            // 
            // textBoxShareCode
            // 
            this.textBoxShareCode.Font = new System.Drawing.Font("Courier New", 16F, System.Drawing.FontStyle.Bold);
            this.textBoxShareCode.Location = new System.Drawing.Point(15, 60);
            this.textBoxShareCode.Name = "textBoxShareCode";
            this.textBoxShareCode.ReadOnly = true;
            this.textBoxShareCode.Size = new System.Drawing.Size(280, 44);
            this.textBoxShareCode.TabIndex = 1;
            this.textBoxShareCode.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // buttonCopyCode
            // 
            this.buttonCopyCode.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.buttonCopyCode.Location = new System.Drawing.Point(315, 60);
            this.buttonCopyCode.Name = "buttonCopyCode";
            this.buttonCopyCode.Size = new System.Drawing.Size(150, 44);
            this.buttonCopyCode.TabIndex = 2;
            this.buttonCopyCode.Text = "?? Copy Code";
            this.buttonCopyCode.UseVisualStyleBackColor = true;
            this.buttonCopyCode.Click += new System.EventHandler(this.ButtonCopyCode_Click);
            // 
            // buttonClose
            // 
            this.buttonClose.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.buttonClose.Location = new System.Drawing.Point(200, 615);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(150, 45);
            this.buttonClose.TabIndex = 10;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.ButtonClose_Click);
            // 
            // panelInfo
            // 
            this.panelInfo.Controls.Add(this.labelOrgName);
            this.panelInfo.Controls.Add(this.textBoxOrgName);
            this.panelInfo.Controls.Add(this.labelOrgCode);
            this.panelInfo.Controls.Add(this.textBoxOrgCode);
            this.panelInfo.Controls.Add(this.labelYourRole);
            this.panelInfo.Controls.Add(this.textBoxYourRole);
            this.panelInfo.Controls.Add(this.labelDescription);
            this.panelInfo.Controls.Add(this.textBoxDescription);
            this.panelInfo.Controls.Add(this.groupBoxShareCode);
            this.panelInfo.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelInfo.Location = new System.Drawing.Point(0, 50);
            this.panelInfo.Name = "panelInfo";
            this.panelInfo.Size = new System.Drawing.Size(550, 470);
            this.panelInfo.TabIndex = 11;
            // 
            // OrganizationInfoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(550, 680);
            this.Controls.Add(this.panelInfo);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.labelTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OrganizationInfoForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Organization Info";
            this.Load += new System.EventHandler(this.OrganizationInfoForm_Load);
            this.groupBoxShareCode.ResumeLayout(false);
            this.groupBoxShareCode.PerformLayout();
            this.panelInfo.ResumeLayout(false);
            this.panelInfo.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.Label labelOrgName;
        private System.Windows.Forms.TextBox textBoxOrgName;
        private System.Windows.Forms.Label labelOrgCode;
        private System.Windows.Forms.TextBox textBoxOrgCode;
        private System.Windows.Forms.Label labelDescription;
        private System.Windows.Forms.TextBox textBoxDescription;
        private System.Windows.Forms.Label labelYourRole;
        private System.Windows.Forms.TextBox textBoxYourRole;
        private System.Windows.Forms.GroupBox groupBoxShareCode;
        private System.Windows.Forms.Button buttonCopyCode;
        private System.Windows.Forms.TextBox textBoxShareCode;
        private System.Windows.Forms.Label labelShareInfo;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Panel panelInfo;
    }
}
