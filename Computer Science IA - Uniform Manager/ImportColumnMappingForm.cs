using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Computer_Science_IA___Uniform_Manager
{
    public partial class ImportColumnMappingForm : Form
    {
        public enum ImportType
        {
            Uniforms,
            Students
        }

        private readonly DataTable _importData;
        private readonly ImportType _importType;
        private readonly Dictionary<string, ComboBox> _columnMappings = new Dictionary<string, ComboBox>();

        public Dictionary<string, string> ColumnMapping { get; private set; } = new Dictionary<string, string>();
        public DataTable ImportData => _importData;

        public ImportColumnMappingForm(DataTable data, ImportType importType)
        {
            InitializeComponent();
            _importData = data;
            _importType = importType;
        }

        private void ImportColumnMappingForm_Load(object sender, EventArgs e)
        {
            this.Text = $"Import {_importType} - Column Mapping";
            labelTitle.Text = $"Map Spreadsheet Columns to {_importType} Fields";
            
            // Show preview of data
            dataGridViewPreview.DataSource = _importData;
            labelRecordCount.Text = $"Found {_importData.Rows.Count} record(s)";

            // Build column mapping UI
            BuildColumnMappingUI();
        }

        private void BuildColumnMappingUI()
        {
            panelMappings.Controls.Clear();
            _columnMappings.Clear();

            var requiredFields = GetRequiredFields();
            var optionalFields = GetOptionalFields();
            var sourceColumns = _importData.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToList();

            int yPosition = 10;

            // Add required fields first
            var lblRequired = new Label
            {
                Text = "Required Fields:",
                Font = new Font(Font, FontStyle.Bold),
                Location = new Point(10, yPosition),
                Size = new Size(400, 20),
                ForeColor = Color.DarkRed
            };
            panelMappings.Controls.Add(lblRequired);
            yPosition += 25;

            foreach (var field in requiredFields)
            {
                AddMappingRow(field.Key, field.Value, sourceColumns, ref yPosition, true);
            }

            // Add optional fields
            yPosition += 10;
            var lblOptional = new Label
            {
                Text = "Optional Fields:",
                Font = new Font(Font, FontStyle.Bold),
                Location = new Point(10, yPosition),
                Size = new Size(400, 20)
            };
            panelMappings.Controls.Add(lblOptional);
            yPosition += 25;

            foreach (var field in optionalFields)
            {
                AddMappingRow(field.Key, field.Value, sourceColumns, ref yPosition, false);
            }
        }

        private void AddMappingRow(string fieldName, string description, List<string> sourceColumns, ref int yPosition, bool isRequired)
        {
            // Label for field name
            var lblField = new Label
            {
                Text = $"{description}:",
                Location = new Point(20, yPosition + 3),
                Size = new Size(150, 20),
                ForeColor = isRequired ? Color.DarkRed : Color.Black
            };
            panelMappings.Controls.Add(lblField);

            // ComboBox for column selection
            var cmbColumn = new ComboBox
            {
                Location = new Point(180, yPosition),
                Size = new Size(200, 20),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Tag = fieldName
            };

            cmbColumn.Items.Add("(Skip this field)");
            foreach (var col in sourceColumns)
            {
                cmbColumn.Items.Add(col);
            }

            // Try to auto-match column
            var autoMatch = sourceColumns.FirstOrDefault(c => 
                c.Equals(fieldName, StringComparison.OrdinalIgnoreCase) ||
                c.Equals(description, StringComparison.OrdinalIgnoreCase) ||
                c.Replace(" ", "").Equals(fieldName.Replace(" ", ""), StringComparison.OrdinalIgnoreCase));

            if (autoMatch != null)
            {
                cmbColumn.SelectedItem = autoMatch;
            }
            else
            {
                cmbColumn.SelectedIndex = 0;
            }

            panelMappings.Controls.Add(cmbColumn);
            _columnMappings[fieldName] = cmbColumn;

            yPosition += 30;
        }

        private Dictionary<string, string> GetRequiredFields()
        {
            if (_importType == ImportType.Uniforms)
            {
                return new Dictionary<string, string>
                {
                    { "UniformIdentifier", "Uniform ID" },
                    { "UniformType", "Type" },
                    { "Size", "Size" }
                };
            }
            else // Students
            {
                return new Dictionary<string, string>
                {
                    { "StudentIdentifier", "Student ID" },
                    { "FirstName", "First Name" },
                    { "LastName", "Last Name" },
                    { "Grade", "Grade" }
                };
            }
        }

        private Dictionary<string, string> GetOptionalFields()
        {
            if (_importType == ImportType.Uniforms)
            {
                return new Dictionary<string, string>
                {
                    { "IsCheckedOut", "Checked Out" },
                    { "AssignedStudentId", "Assigned Student" }
                };
            }
            else // Students
            {
                return new Dictionary<string, string>();
            }
        }

        private void ButtonImport_Click(object sender, EventArgs e)
        {
            // Validate required fields
            var requiredFields = GetRequiredFields();
            var missingFields = new List<string>();

            foreach (var field in requiredFields)
            {
                if (_columnMappings.TryGetValue(field.Key, out var comboBox))
                {
                    if (comboBox.SelectedIndex == 0) // "(Skip this field)" selected
                    {
                        missingFields.Add(field.Value);
                    }
                }
            }

            if (missingFields.Any())
            {
                MessageBox.Show(
                    $"Please map all required fields:\n\n• {string.Join("\n• ", missingFields)}",
                    "Missing Required Fields",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            // Build column mapping
            ColumnMapping.Clear();
            foreach (var kvp in _columnMappings)
            {
                if (kvp.Value.SelectedIndex > 0) // Not "(Skip this field)"
                {
                    ColumnMapping[kvp.Key] = kvp.Value.SelectedItem.ToString()!;
                }
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
