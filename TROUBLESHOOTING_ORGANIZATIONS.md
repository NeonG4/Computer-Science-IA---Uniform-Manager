# Troubleshooting Guide - Organization Loading Issues

## Problem: Seeing "No organizations found" or "Unable to load organizations"

### Quick Diagnosis

The application now shows status messages directly in the organization list instead of error popups. Here's what each message means:

---

## Status Messages

### ? "No organizations found" + "Click 'Create New Org' to get started!"
**Meaning:** Everything is working! You just haven't created any organizations yet.

**This is normal for:**
- First-time users
- Users who haven't been added to any organizations

**What to do:**
1. Click the **"Create New Org"** button
2. Fill in organization details
3. Click "Create Organization"
4. You'll be made Admin automatically

---

### ?? "Unable to connect to server" + "Make sure the Azure Function is running"
**Meaning:** The Azure Function API is not running or not reachable.

**Fix:**
1. Open Visual Studio
2. Set `CompsciAzureFunctionAPI2026` as the startup project
3. Press **F5** to start debugging
4. Wait for "Now listening on: http://localhost:7109"
5. Go back to your WinForms app and login again

---

### ?? "Unable to load organizations" + reason message
**Meaning:** The API is running but returned an error (usually database-related).

**Common reasons:**
- "Invalid object name 'Organizations'" - Database tables don't exist
- "Invalid object name 'UserOrganizations'" - Database tables don't exist

**Fix:**
1. Run the database setup script:
   ```powershell
   sqlcmd -S "(localdb)\MSSQLLocalDB" -i "SetupLocalDatabaseWithOrgs.sql"
   ```
2. Restart the Azure Function
3. Login again

---

## Checking Debug Output

All errors are now logged to the Debug output instead of showing popups. To see detailed errors:

**In Visual Studio:**
1. Go to **View** ? **Output**
2. In the dropdown, select **"Debug"**
3. Look for messages like:
   ```
   Error loading organizations: 500 - {...}
   Network error loading organizations: Connection refused
   ```

This helps developers troubleshoot without annoying users with error popups!

---

## Step-by-Step Troubleshooting

### 1. Check if Database Tables Exist

Run `DiagnosticCheckDatabase.sql`:

```powershell
sqlcmd -S "(localdb)\MSSQLLocalDB" -i "DiagnosticCheckDatabase.sql"
```

**Look for:**
```
? Organizations table exists
? UserOrganizations table exists
```

**If missing:**
```powershell
sqlcmd -S "(localdb)\MSSQLLocalDB" -i "SetupLocalDatabaseWithOrgs.sql"
```

---

### 2. Verify Azure Function is Running

**Check Visual Studio Output:**
Look for:
```
Now listening on: http://localhost:7109
```

**Test the API directly in browser:**
```
http://localhost:7109/api/GetOrganizations?userId=1
```

**Expected Response (no orgs):**
```json
{
  "success": true,
  "message": "No organizations found for this user.",
  "organizations": [],
  "totalCount": 0
}
```

---

### 3. First Time User Experience

**What you should see:**

1. **Login** ? Success
2. **Organization Selector appears** with:
   - Title: "Select Organization"
   - List shows: 
     - "No organizations found"
     - ""
     - "Click 'Create New Org' to get started!"
   - Subtitle: "Ready to create your first organization"
   - "Create New Org" button: **Enabled** ?
   - "Select" button: **Disabled** (grayed out)

3. **Click "Create New Org"**
4. **Fill in the form:**
   - Organization Name: "My School"
   - Organization Code: "MYSCH"
   - Description: (optional)
5. **Click "Create Organization"**
6. **List refreshes** showing your new organization
7. **Select button is now enabled**
8. **Select your organization** and click "Select"
9. **Main form opens** with org name in title

---

## Clean User Experience

The application now provides a **clean, non-intrusive experience**:

### ? No Error Popups for Normal Situations
- No popup when you have no organizations (expected)
- No popup when you haven't selected an org yet
- No popup for common issues

### ? Clear Visual Feedback
- Status messages show directly in the list
- Buttons are enabled/disabled appropriately
- Subtitle updates to guide the user

### ? Debug Information Still Available
- All errors logged to Debug output
- Developers can see full error details
- Stack traces available in debug console

---

## Still Having Issues?

### View Debug Output:
1. Run WinForms app from Visual Studio (F5)
2. View ? Output ? Debug
3. Try to load organizations
4. Check for error messages

### Check Azure Function Logs:
1. Azure Function console shows:
   - SQL errors
   - Connection errors
   - API errors

### Verify Database:
```sql
USE DatabasestorageIA;

-- Check tables exist
SELECT name FROM sys.tables ORDER BY name;

-- Check data
SELECT COUNT(*) FROM Organizations;
SELECT COUNT(*) FROM UserOrganizations;
```

---

## Success Indicators

? **Everything is working when you see:**

**Scenario 1: First Time User**
- List shows: "No organizations found"
- List shows: "Click 'Create New Org' to get started!"
- Subtitle: "Ready to create your first organization"
- Create button: Enabled
- Select button: Disabled

**Scenario 2: Existing Organizations**
- List shows: "School Name (Admin)" or similar
- Subtitle: "You have access to X organization(s)"
- Both buttons: Enabled
- Double-clicking an org selects it

**Scenario 3: API Not Running**
- List shows: "Unable to connect to server"
- List shows: "Make sure the Azure Function is running"
- Both buttons: Disabled
- No error popup (clean UX)

---

## Quick Reference

| What You See | What It Means | What To Do |
|---|---|---|
| "No organizations found" | Normal for new users | Click "Create New Org" |
| "Unable to connect to server" | Function not running | Start Azure Function (F5) |
| "Unable to load organizations" | Database issue | Run SetupLocalDatabaseWithOrgs.sql |
| Organization list with items | ? Working perfectly | Select and continue |

---

## Need More Help?

1. Check **Visual Studio Debug Output** (View ? Output ? Debug)
2. Check **Azure Function Console** for errors
3. Run **DiagnosticCheckDatabase.sql** to verify database
4. Share the debug output messages

The improved UI now handles errors gracefully without interrupting your workflow!
