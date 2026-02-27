namespace Computer_Science_IA___Uniform_Manager
{
    partial class ManageJoinRequestsForm
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
            this.dataGridViewRequests = new System.Windows.Forms.DataGridView();
            this.buttonApprove = new System.Windows.Forms.Button();
            this.buttonReject = new System.Windows.Forms.Button();
            this.buttonRefresh = new System.Windows.Forms.Button();
            this.buttonClose = new System.Windows.Forms.Button();
            this.labelStatus = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewRequests)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelTitle
            // 
            this.labelTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.labelTitle.Location = new System.Drawing.Point(0, 0);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(900, 50);
            this.labelTitle.TabIndex = 0;
            this.labelTitle.Text = "Pending Join Requests";
            this.labelTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // dataGridViewRequests
            // 
            this.dataGridViewRequests.AllowUserToAddRows = false;
            this.dataGridViewRequests.AllowUserToDeleteRows = false;
            this.dataGridViewRequests.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewRequests.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewRequests.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewRequests.Location = new System.Drawing.Point(0, 50);
            this.dataGridViewRequests.MultiSelect = false;
            this.dataGridViewRequests.Name = "dataGridViewRequests";
            this.dataGridViewRequests.ReadOnly = true;
            this.dataGridViewRequests.RowHeadersWidth = 62;
            this.dataGridViewRequests.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewRequests.Size = new System.Drawing.Size(900, 400);
            this.dataGridViewRequests.TabIndex = 1;
            this.dataGridViewRequests.SelectionChanged += new System.EventHandler(this.DataGridViewRequests_SelectionChanged);
            // 
            // buttonApprove
            // 
            this.buttonApprove.BackColor = System.Drawing.Color.LightGreen;
            this.buttonApprove.Enabled = false;
            this.buttonApprove.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.buttonApprove.Location = new System.Drawing.Point(20, 15);
            this.buttonApprove.Name = "buttonApprove";
            this.buttonApprove.Size = new System.Drawing.Size(150, 45);
            this.buttonApprove.TabIndex = 2;
            this.buttonApprove.Text = "? Approve";
            this.buttonApprove.UseVisualStyleBackColor = false;
            this.buttonApprove.Click += new System.EventHandler(this.ButtonApprove_Click);
            // 
            // buttonReject
            // 
            this.buttonReject.BackColor = System.Drawing.Color.LightCoral;
            this.buttonReject.Enabled = false;
            this.buttonReject.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.buttonReject.Location = new System.Drawing.Point(190, 15);
            this.buttonReject.Name = "buttonReject";
            this.buttonReject.Size = new System.Drawing.Size(150, 45);
            this.buttonReject.TabIndex = 3;
            this.buttonReject.Text = "? Reject";
            this.buttonReject.UseVisualStyleBackColor = false;
            this.buttonReject.Click += new System.EventHandler(this.ButtonReject_Click);
            // 
            // buttonRefresh
            // 
            this.buttonRefresh.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.buttonRefresh.Location = new System.Drawing.Point(560, 15);
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.Size = new System.Drawing.Size(130, 45);
            this.buttonRefresh.TabIndex = 4;
            this.buttonRefresh.Text = "?? Refresh";
            this.buttonRefresh.UseVisualStyleBackColor = true;
            this.buttonRefresh.Click += new System.EventHandler(this.ButtonRefresh_Click);
            // 
            // buttonClose
            // 
            this.buttonClose.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.buttonClose.Location = new System.Drawing.Point(710, 15);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(130, 45);
            this.buttonClose.TabIndex = 5;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.ButtonClose_Click);
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.ForeColor = System.Drawing.SystemColors.GrayText;
            this.labelStatus.Location = new System.Drawing.Point(365, 25);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(120, 25);
            this.labelStatus.TabIndex = 6;
            this.labelStatus.Text = "Loading...";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.buttonApprove);
            this.panel1.Controls.Add(this.labelStatus);
            this.panel1.Controls.Add(this.buttonReject);
            this.panel1.Controls.Add(this.buttonClose);
            this.panel1.Controls.Add(this.buttonRefresh);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 450);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(900, 75);
            this.panel1.TabIndex = 7;
            // 
            // ManageJoinRequestsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 525);
            this.Controls.Add(this.dataGridViewRequests);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.labelTitle);
            this.MinimumSize = new System.Drawing.Size(800, 400);
            this.Name = "ManageJoinRequestsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Manage Join Requests";
            this.Load += new System.EventHandler(this.ManageJoinRequestsForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewRequests)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.DataGridView dataGridViewRequests;
        private System.Windows.Forms.Button buttonApprove;
        private System.Windows.Forms.Button buttonReject;
        private System.Windows.Forms.Button buttonRefresh;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.Panel panel1;
    }
}
