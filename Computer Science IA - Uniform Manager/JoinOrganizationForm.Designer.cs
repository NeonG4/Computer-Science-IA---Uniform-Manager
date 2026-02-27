namespace Computer_Science_IA___Uniform_Manager
{
    partial class JoinOrganizationForm
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
            this.labelOrgCode = new System.Windows.Forms.Label();
            this.textBoxOrgCode = new System.Windows.Forms.TextBox();
            this.labelInfo = new System.Windows.Forms.Label();
            this.buttonJoin = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.labelRole = new System.Windows.Forms.Label();
            this.comboBoxRole = new System.Windows.Forms.ComboBox();
            this.labelMessage = new System.Windows.Forms.Label();
            this.textBoxMessage = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // labelTitle
            // 
            this.labelTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.labelTitle.Location = new System.Drawing.Point(20, 20);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(460, 40);
            this.labelTitle.TabIndex = 0;
            this.labelTitle.Text = "Request to Join Organization";
            this.labelTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelOrgCode
            // 
            this.labelOrgCode.AutoSize = true;
            this.labelOrgCode.Location = new System.Drawing.Point(40, 80);
            this.labelOrgCode.Name = "labelOrgCode";
            this.labelOrgCode.Size = new System.Drawing.Size(170, 25);
            this.labelOrgCode.TabIndex = 1;
            this.labelOrgCode.Text = "Organization Code:";
            // 
            // textBoxOrgCode
            // 
            this.textBoxOrgCode.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.textBoxOrgCode.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.textBoxOrgCode.Location = new System.Drawing.Point(40, 110);
            this.textBoxOrgCode.MaxLength = 50;
            this.textBoxOrgCode.Name = "textBoxOrgCode";
            this.textBoxOrgCode.Size = new System.Drawing.Size(420, 39);
            this.textBoxOrgCode.TabIndex = 2;
            this.textBoxOrgCode.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // labelRole
            // 
            this.labelRole.AutoSize = true;
            this.labelRole.Location = new System.Drawing.Point(40, 170);
            this.labelRole.Name = "labelRole";
            this.labelRole.Size = new System.Drawing.Size(144, 25);
            this.labelRole.TabIndex = 3;
            this.labelRole.Text = "Request Role As:";
            // 
            // comboBoxRole
            // 
            this.comboBoxRole.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxRole.FormattingEnabled = true;
            this.comboBoxRole.Items.AddRange(new object[] {
            "Viewer (Read-only)",
            "User (Can check-out uniforms)",
            "Admin (Full access)"});
            this.comboBoxRole.Location = new System.Drawing.Point(40, 200);
            this.comboBoxRole.Name = "comboBoxRole";
            this.comboBoxRole.Size = new System.Drawing.Size(420, 33);
            this.comboBoxRole.TabIndex = 4;
            // 
            // labelMessage
            // 
            this.labelMessage.AutoSize = true;
            this.labelMessage.Location = new System.Drawing.Point(40, 250);
            this.labelMessage.Name = "labelMessage";
            this.labelMessage.Size = new System.Drawing.Size(265, 25);
            this.labelMessage.TabIndex = 5;
            this.labelMessage.Text = "Message to Admins (Optional):";
            // 
            // textBoxMessage
            // 
            this.textBoxMessage.Location = new System.Drawing.Point(40, 280);
            this.textBoxMessage.MaxLength = 500;
            this.textBoxMessage.Multiline = true;
            this.textBoxMessage.Name = "textBoxMessage";
            this.textBoxMessage.PlaceholderText = "Tell the admins why you want to join...";
            this.textBoxMessage.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxMessage.Size = new System.Drawing.Size(420, 80);
            this.textBoxMessage.TabIndex = 6;
            // 
            // labelInfo
            // 
            this.labelInfo.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelInfo.ForeColor = System.Drawing.SystemColors.GrayText;
            this.labelInfo.Location = new System.Drawing.Point(40, 380);
            this.labelInfo.Name = "labelInfo";
            this.labelInfo.Size = new System.Drawing.Size(420, 60);
            this.labelInfo.TabIndex = 7;
            this.labelInfo.Text = "Your request will be sent to the organization administrators for approval. You will be notified once your request has been reviewed.";
            this.labelInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // buttonJoin
            // 
            this.buttonJoin.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.buttonJoin.Location = new System.Drawing.Point(260, 460);
            this.buttonJoin.Name = "buttonJoin";
            this.buttonJoin.Size = new System.Drawing.Size(200, 45);
            this.buttonJoin.TabIndex = 8;
            this.buttonJoin.Text = "Send Request";
            this.buttonJoin.UseVisualStyleBackColor = true;
            this.buttonJoin.Click += new System.EventHandler(this.ButtonJoin_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(40, 460);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(120, 45);
            this.buttonCancel.TabIndex = 9;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.ButtonCancel_Click);
            // 
            // JoinOrganizationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(500, 530);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonJoin);
            this.Controls.Add(this.labelInfo);
            this.Controls.Add(this.textBoxMessage);
            this.Controls.Add(this.labelMessage);
            this.Controls.Add(this.comboBoxRole);
            this.Controls.Add(this.labelRole);
            this.Controls.Add(this.textBoxOrgCode);
            this.Controls.Add(this.labelOrgCode);
            this.Controls.Add(this.labelTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "JoinOrganizationForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Join Organization";
            this.Load += new System.EventHandler(this.JoinOrganizationForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.Label labelOrgCode;
        private System.Windows.Forms.TextBox textBoxOrgCode;
        private System.Windows.Forms.Label labelRole;
        private System.Windows.Forms.ComboBox comboBoxRole;
        private System.Windows.Forms.Label labelMessage;
        private System.Windows.Forms.TextBox textBoxMessage;
        private System.Windows.Forms.Label labelInfo;
        private System.Windows.Forms.Button buttonJoin;
        private System.Windows.Forms.Button buttonCancel;
    }
}
