namespace Computer_Science_IA___Uniform_Manager
{
    partial class CreateOrganizationForm
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
            this.buttonCreate = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.labelInfo = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelTitle
            // 
            this.labelTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.labelTitle.Location = new System.Drawing.Point(20, 20);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(460, 40);
            this.labelTitle.TabIndex = 0;
            this.labelTitle.Text = "Create New Organization";
            this.labelTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelOrgName
            // 
            this.labelOrgName.AutoSize = true;
            this.labelOrgName.Location = new System.Drawing.Point(40, 80);
            this.labelOrgName.Name = "labelOrgName";
            this.labelOrgName.Size = new System.Drawing.Size(175, 25);
            this.labelOrgName.TabIndex = 1;
            this.labelOrgName.Text = "Organization Name:";
            // 
            // textBoxOrgName
            // 
            this.textBoxOrgName.Location = new System.Drawing.Point(40, 110);
            this.textBoxOrgName.MaxLength = 100;
            this.textBoxOrgName.Name = "textBoxOrgName";
            this.textBoxOrgName.Size = new System.Drawing.Size(420, 31);
            this.textBoxOrgName.TabIndex = 2;
            // 
            // labelOrgCode
            // 
            this.labelOrgCode.AutoSize = true;
            this.labelOrgCode.Location = new System.Drawing.Point(40, 160);
            this.labelOrgCode.Name = "labelOrgCode";
            this.labelOrgCode.Size = new System.Drawing.Size(170, 25);
            this.labelOrgCode.TabIndex = 3;
            this.labelOrgCode.Text = "Organization Code:";
            // 
            // textBoxOrgCode
            // 
            this.textBoxOrgCode.Location = new System.Drawing.Point(40, 190);
            this.textBoxOrgCode.MaxLength = 50;
            this.textBoxOrgCode.Name = "textBoxOrgCode";
            this.textBoxOrgCode.Size = new System.Drawing.Size(420, 31);
            this.textBoxOrgCode.TabIndex = 4;
            // 
            // labelDescription
            // 
            this.labelDescription.AutoSize = true;
            this.labelDescription.Location = new System.Drawing.Point(40, 240);
            this.labelDescription.Name = "labelDescription";
            this.labelDescription.Size = new System.Drawing.Size(176, 25);
            this.labelDescription.TabIndex = 5;
            this.labelDescription.Text = "Description (optional):";
            // 
            // textBoxDescription
            // 
            this.textBoxDescription.Location = new System.Drawing.Point(40, 270);
            this.textBoxDescription.MaxLength = 500;
            this.textBoxDescription.Multiline = true;
            this.textBoxDescription.Name = "textBoxDescription";
            this.textBoxDescription.Size = new System.Drawing.Size(420, 80);
            this.textBoxDescription.TabIndex = 6;
            // 
            // buttonCreate
            // 
            this.buttonCreate.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.buttonCreate.Location = new System.Drawing.Point(260, 420);
            this.buttonCreate.Name = "buttonCreate";
            this.buttonCreate.Size = new System.Drawing.Size(200, 45);
            this.buttonCreate.TabIndex = 7;
            this.buttonCreate.Text = "Create Organization";
            this.buttonCreate.UseVisualStyleBackColor = true;
            this.buttonCreate.Click += new System.EventHandler(this.ButtonCreate_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(40, 420);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(120, 45);
            this.buttonCancel.TabIndex = 8;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.ButtonCancel_Click);
            // 
            // labelInfo
            // 
            this.labelInfo.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelInfo.ForeColor = System.Drawing.SystemColors.GrayText;
            this.labelInfo.Location = new System.Drawing.Point(40, 360);
            this.labelInfo.Name = "labelInfo";
            this.labelInfo.Size = new System.Drawing.Size(420, 45);
            this.labelInfo.TabIndex = 9;
            this.labelInfo.Text = "You will be assigned as an Administrator of this organization.";
            this.labelInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // CreateOrganizationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(500, 490);
            this.Controls.Add(this.labelInfo);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonCreate);
            this.Controls.Add(this.textBoxDescription);
            this.Controls.Add(this.labelDescription);
            this.Controls.Add(this.textBoxOrgCode);
            this.Controls.Add(this.labelOrgCode);
            this.Controls.Add(this.textBoxOrgName);
            this.Controls.Add(this.labelOrgName);
            this.Controls.Add(this.labelTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CreateOrganizationForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Create Organization";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.Label labelOrgName;
        private System.Windows.Forms.TextBox textBoxOrgName;
        private System.Windows.Forms.Label labelOrgCode;
        private System.Windows.Forms.TextBox textBoxOrgCode;
        private System.Windows.Forms.Label labelDescription;
        private System.Windows.Forms.TextBox textBoxDescription;
        private System.Windows.Forms.Button buttonCreate;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label labelInfo;
    }
}
