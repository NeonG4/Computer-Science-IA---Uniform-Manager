namespace Computer_Science_IA___Uniform_Manager
{
    partial class ImportColumnMappingForm
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
            labelRecordCount = new Label();
            dataGridViewPreview = new DataGridView();
            labelPreview = new Label();
            panelMappings = new Panel();
            buttonImport = new Button();
            buttonCancel = new Button();
            labelInstructions = new Label();
            ((System.ComponentModel.ISupportInitialize)dataGridViewPreview).BeginInit();
            SuspendLayout();
            // 
            // labelTitle
            // 
            labelTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            labelTitle.Location = new Point(12, 9);
            labelTitle.Name = "labelTitle";
            labelTitle.Size = new Size(776, 30);
            labelTitle.TabIndex = 0;
            labelTitle.Text = "Map Spreadsheet Columns";
            labelTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // labelInstructions
            // 
            labelInstructions.Location = new Point(12, 45);
            labelInstructions.Name = "labelInstructions";
            labelInstructions.Size = new Size(776, 35);
            labelInstructions.TabIndex = 1;
            labelInstructions.Text = "Select which column from your spreadsheet corresponds to each field. Required fields must be mapped.";
            labelInstructions.TextAlign = ContentAlignment.TopCenter;
            // 
            // labelRecordCount
            // 
            labelRecordCount.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            labelRecordCount.Location = new Point(12, 80);
            labelRecordCount.Name = "labelRecordCount";
            labelRecordCount.Size = new Size(776, 20);
            labelRecordCount.TabIndex = 2;
            labelRecordCount.Text = "Found 0 records";
            labelRecordCount.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // panelMappings
            // 
            panelMappings.AutoScroll = true;
            panelMappings.BorderStyle = BorderStyle.FixedSingle;
            panelMappings.Location = new Point(12, 105);
            panelMappings.Name = "panelMappings";
            panelMappings.Size = new Size(420, 380);
            panelMappings.TabIndex = 3;
            // 
            // labelPreview
            // 
            labelPreview.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            labelPreview.Location = new Point(438, 105);
            labelPreview.Name = "labelPreview";
            labelPreview.Size = new Size(350, 20);
            labelPreview.TabIndex = 4;
            labelPreview.Text = "Data Preview (first 100 rows):";
            // 
            // dataGridViewPreview
            // 
            dataGridViewPreview.AllowUserToAddRows = false;
            dataGridViewPreview.AllowUserToDeleteRows = false;
            dataGridViewPreview.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewPreview.Location = new Point(438, 128);
            dataGridViewPreview.Name = "dataGridViewPreview";
            dataGridViewPreview.ReadOnly = true;
            dataGridViewPreview.RowHeadersWidth = 51;
            dataGridViewPreview.Size = new Size(350, 357);
            dataGridViewPreview.TabIndex = 5;
            // 
            // buttonImport
            // 
            buttonImport.BackColor = Color.LightGreen;
            buttonImport.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            buttonImport.Location = new Point(632, 495);
            buttonImport.Name = "buttonImport";
            buttonImport.Size = new Size(156, 40);
            buttonImport.TabIndex = 6;
            buttonImport.Text = "Import Data";
            buttonImport.UseVisualStyleBackColor = false;
            buttonImport.Click += ButtonImport_Click;
            // 
            // buttonCancel
            // 
            buttonCancel.Location = new Point(520, 495);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(106, 40);
            buttonCancel.TabIndex = 7;
            buttonCancel.Text = "Cancel";
            buttonCancel.UseVisualStyleBackColor = true;
            buttonCancel.Click += ButtonCancel_Click;
            // 
            // ImportColumnMappingForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 547);
            Controls.Add(buttonCancel);
            Controls.Add(buttonImport);
            Controls.Add(dataGridViewPreview);
            Controls.Add(labelPreview);
            Controls.Add(panelMappings);
            Controls.Add(labelRecordCount);
            Controls.Add(labelInstructions);
            Controls.Add(labelTitle);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ImportColumnMappingForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Import Column Mapping";
            Load += ImportColumnMappingForm_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridViewPreview).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Label labelTitle;
        private Label labelRecordCount;
        private DataGridView dataGridViewPreview;
        private Label labelPreview;
        private Panel panelMappings;
        private Button buttonImport;
        private Button buttonCancel;
        private Label labelInstructions;
    }
}
