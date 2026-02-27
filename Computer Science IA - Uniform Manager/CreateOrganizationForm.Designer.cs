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
            labelTitle = new Label();
            labelOrgName = new Label();
            textBoxOrgName = new TextBox();
            labelOrgCode = new Label();
            textBoxOrgCode = new TextBox();
            labelDescription = new Label();
            textBoxDescription = new TextBox();
            buttonCreate = new Button();
            buttonCancel = new Button();
            labelInfo = new Label();
            SuspendLayout();
            // 
            // labelTitle
            // 
            labelTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            labelTitle.Location = new Point(11, 12);
            labelTitle.Margin = new Padding(2, 0, 2, 0);
            labelTitle.Name = "labelTitle";
            labelTitle.Size = new Size(325, 24);
            labelTitle.TabIndex = 0;
            labelTitle.Text = "Create New Organization";
            labelTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // labelOrgName
            // 
            labelOrgName.AutoSize = true;
            labelOrgName.Location = new Point(28, 48);
            labelOrgName.Margin = new Padding(2, 0, 2, 0);
            labelOrgName.Name = "labelOrgName";
            labelOrgName.Size = new Size(113, 15);
            labelOrgName.TabIndex = 1;
            labelOrgName.Text = "Organization Name:";
            // 
            // textBoxOrgName
            // 
            textBoxOrgName.Location = new Point(28, 66);
            textBoxOrgName.Margin = new Padding(2, 2, 2, 2);
            textBoxOrgName.MaxLength = 100;
            textBoxOrgName.Name = "textBoxOrgName";
            textBoxOrgName.Size = new Size(295, 23);
            textBoxOrgName.TabIndex = 2;
            // 
            // labelOrgCode
            // 
            labelOrgCode.AutoSize = true;
            labelOrgCode.Location = new Point(28, 96);
            labelOrgCode.Margin = new Padding(2, 0, 2, 0);
            labelOrgCode.Name = "labelOrgCode";
            labelOrgCode.Size = new Size(109, 15);
            labelOrgCode.TabIndex = 3;
            labelOrgCode.Text = "Organization Code:";
            // 
            // textBoxOrgCode
            // 
            textBoxOrgCode.Location = new Point(28, 114);
            textBoxOrgCode.Margin = new Padding(2, 2, 2, 2);
            textBoxOrgCode.MaxLength = 50;
            textBoxOrgCode.Name = "textBoxOrgCode";
            textBoxOrgCode.Size = new Size(295, 23);
            textBoxOrgCode.TabIndex = 4;
            // 
            // labelDescription
            // 
            labelDescription.AutoSize = true;
            labelDescription.Location = new Point(28, 144);
            labelDescription.Margin = new Padding(2, 0, 2, 0);
            labelDescription.Name = "labelDescription";
            labelDescription.Size = new Size(125, 15);
            labelDescription.TabIndex = 5;
            labelDescription.Text = "Description (optional):";
            // 
            // textBoxDescription
            // 
            textBoxDescription.Location = new Point(28, 162);
            textBoxDescription.Margin = new Padding(2, 2, 2, 2);
            textBoxDescription.MaxLength = 500;
            textBoxDescription.Multiline = true;
            textBoxDescription.Name = "textBoxDescription";
            textBoxDescription.Size = new Size(295, 50);
            textBoxDescription.TabIndex = 6;
            // 
            // buttonCreate
            // 
            buttonCreate.Font = new Font("Segoe UI", 11F);
            buttonCreate.Location = new Point(182, 252);
            buttonCreate.Margin = new Padding(2, 2, 2, 2);
            buttonCreate.Name = "buttonCreate";
            buttonCreate.Size = new Size(140, 27);
            buttonCreate.TabIndex = 7;
            buttonCreate.Text = "Create Organization";
            buttonCreate.UseVisualStyleBackColor = true;
            buttonCreate.Click += ButtonCreate_Click;
            // 
            // buttonCancel
            // 
            buttonCancel.Location = new Point(28, 252);
            buttonCancel.Margin = new Padding(2, 2, 2, 2);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(84, 27);
            buttonCancel.TabIndex = 8;
            buttonCancel.Text = "Cancel";
            buttonCancel.UseVisualStyleBackColor = true;
            buttonCancel.Click += ButtonCancel_Click;
            // 
            // labelInfo
            // 
            labelInfo.Font = new Font("Segoe UI", 9F);
            labelInfo.ForeColor = SystemColors.GrayText;
            labelInfo.Location = new Point(28, 216);
            labelInfo.Margin = new Padding(2, 0, 2, 0);
            labelInfo.Name = "labelInfo";
            labelInfo.Size = new Size(294, 27);
            labelInfo.TabIndex = 9;
            labelInfo.Text = "You will be assigned as an Administrator of this organization.";
            labelInfo.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // CreateOrganizationForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(350, 294);
            Controls.Add(labelInfo);
            Controls.Add(buttonCancel);
            Controls.Add(buttonCreate);
            Controls.Add(textBoxDescription);
            Controls.Add(labelDescription);
            Controls.Add(textBoxOrgCode);
            Controls.Add(labelOrgCode);
            Controls.Add(textBoxOrgName);
            Controls.Add(labelOrgName);
            Controls.Add(labelTitle);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(2, 2, 2, 2);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "CreateOrganizationForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Create Organization";
            ResumeLayout(false);
            PerformLayout();
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
