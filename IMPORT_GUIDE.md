# Import Guide - Uniform Manager

This guide explains how to import uniforms and students from spreadsheet files (CSV format).

## General Instructions

1. From the main menu, select **Tools** ? **Import Uniforms** or **Import Students**
2. Choose your CSV file
3. Map the columns from your spreadsheet to the required fields
4. Review the import results

## File Format Requirements

### Supported File Types
- **CSV (Comma-separated values)** - Recommended ?
- **Excel (.xlsx, .xls)** - Convert to CSV first

### Converting Excel to CSV
1. Open your file in Excel
2. Click **File** ? **Save As**
3. Choose **CSV (Comma delimited) (*.csv)**
4. Save and import the CSV file

---

## Importing Uniforms

### Required Columns
Your spreadsheet must include these columns:

| Field | Description | Examples |
|-------|-------------|----------|
| **Uniform ID** | Unique identifier for the uniform | `UC001`, `JACKET-42`, `H123` |
| **Type** | Type of uniform | `Concert Coat`, `Hat`, `Marching Coat` |
| **Size** | Numeric size (1-100) | `42`, `38`, `50` |

### Uniform Type Options

You can use any of these formats (case-insensitive):

| Type | Accepted Values |
|------|----------------|
| Concert Coat | `Concert Coat`, `concert`, `0` |
| Drum Major Coat | `Drum Major Coat`, `drummajor`, `dm`, `1` |
| Hat | `Hat`, `2` |
| Marching Coat | `Marching Coat`, `marching`, `coat`, `3` |
| Marching Shorts | `Marching Shorts`, `shorts`, `4` |
| Marching Socks | `Marching Socks`, `socks`, `5` |
| Pants | `Pants`, `6` |

### Example Uniform CSV

```csv
Uniform ID,Type,Size
UC001,Concert Coat,42
UC002,Hat,7
UC003,Marching Coat,38
UC004,Pants,40
DM001,Drum Major,44
```

---

## Importing Students

### Required Columns
Your spreadsheet must include these columns:

| Field | Description | Examples |
|-------|-------------|----------|
| **Student ID** | Unique identifier for the student | `S12345`, `2024001`, `SMITH-J` |
| **First Name** | Student's first name | `John`, `Mary`, `Alex` |
| **Last Name** | Student's last name | `Smith`, `Johnson`, `Garcia` |
| **Grade** | Student's grade level (1-12) | `9`, `11th`, `11th Grade` |

### Grade Format Options

The import supports many different grade formats:

#### Numeric Formats
- `9`, `10`, `11`, `12` (plain numbers)
- `9th`, `10th`, `11th`, `12th` (ordinal numbers)
- `9th Grade`, `10th Grade` (with "Grade" word)
- `9 grade`, `10 grade` (space instead of "th")

#### Word Formats
- `First` through `Twelfth` (spelled out)
- `One` through `Twelve` (number words)
- `Freshman` ? 9th grade
- `Sophomore` ? 10th grade
- `Junior` ? 11th grade
- `Senior` ? 12th grade

### Example Student CSV

```csv
Student ID,First Name,Last Name,Grade
S12345,John,Smith,9
S12346,Mary,Johnson,11th
S12347,Alex,Garcia,11th Grade
S12348,Sarah,Brown,Sophomore
S12349,Mike,Davis,12
```

---

## Column Mapping

After selecting your file, you'll see the **Column Mapping** screen:

1. **Required Fields** (shown in red) - Must be mapped
2. **Optional Fields** - Can be skipped
3. **Auto-Matching** - The system tries to match columns automatically
4. **Manual Mapping** - Select the correct column from your file for each field

### Tips for Column Mapping
- Column names don't have to match exactly
- The system will try to auto-match similar column names
- You can skip optional fields by selecting "(Skip this field)"
- Make sure all required fields are mapped before importing

---

## Import Process

### Progress Indicator
During import, you'll see:
- Progress bar showing % complete
- Current record being processed
- Total record count

### Import Results
After import completes, you'll see:
- ? **Successfully imported** - Number of records added
- ? **Errors** - Number of records that failed
- **Error details** - Up to 10 error messages with row numbers

### Common Import Errors

| Error | Cause | Solution |
|-------|-------|----------|
| "Missing [Field]" | Required field is empty | Fill in the field in your spreadsheet |
| "Invalid uniform type" | Type not recognized | Use one of the accepted type values |
| "Invalid size" | Size is not 1-100 | Check the size values |
| "Invalid grade" | Grade is not 1-12 | Use grades 1-12 or accepted formats |
| "Failed to create (may already exist)" | ID already in system | Use unique IDs or remove duplicates |

---

## Tips for Successful Imports

### Before Importing
1. ? **Check your data** - Review for completeness and accuracy
2. ? **Remove duplicates** - Ensure all IDs are unique
3. ? **Clean your data** - Remove extra spaces, special characters
4. ? **Test with small batch** - Try importing 5-10 records first
5. ? **Backup existing data** - Export current data before large imports

### During Import
- ?? Watch the progress indicator
- ?? Don't close the window while importing
- ?? Be patient with large files (100+ records)

### After Import
- ?? Review the import results carefully
- ?? Check for any error messages
- ?? Verify imported data in the main grid
- ?? Fix any errors and re-import failed records

---

## Sample Templates

### Uniform Import Template
```csv
Uniform ID,Type,Size
UC001,Concert Coat,42
UC002,Hat,7
UC003,Marching Coat,38
```

### Student Import Template
```csv
Student ID,First Name,Last Name,Grade
S001,John,Smith,9
S002,Jane,Doe,10
S003,Bob,Johnson,11
```

---

## Troubleshooting

### "Error reading file"
- ? Make sure file is saved as CSV
- ? Close the file in Excel before importing
- ? Check file isn't corrupted

### "Empty File"
- ? Ensure file has data rows (not just headers)
- ? Check that rows aren't all blank

### "Excel file support requires additional libraries"
- ? Save your Excel file as CSV format
- ? Follow the conversion steps above

### Many records failing with same error
- ? Check column mapping is correct
- ? Verify data format matches requirements
- ? Look at first error message for the pattern

---

## Administrator Notes

- ?? **Only Administrators** can import data
- ?? Imports are **organization-specific** (won't affect other organizations)
- ?? All imports are **logged** with the importing user's ID
- ?? **Duplicate IDs** will be rejected (not overwritten)
- ?? To update existing records, edit them individually

---

## Need Help?

If you encounter issues:
1. Check this guide for solutions
2. Verify your CSV format matches the examples
3. Try a small test import first
4. Review error messages carefully
5. Contact your system administrator

---

**Last Updated:** December 2024
**Version:** 1.0
