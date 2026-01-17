# User Account Levels - Implementation Guide

## Overview
This implementation adds three user privilege levels to the Uniform Manager system:
- **Administrator (0)** - Full access to all features
- **User (1)** - Standard access with limited admin rights
- **Viewer (2)** - Read-only access

## Database Changes

### Tables Modified/Created

#### 1. UserInfo Table (Modified)
Added columns:
- `AccountLevel` (INT) - Stores the user privilege level (0, 1, or 2)
- `Username` (NVARCHAR(50)) - Unique username for login
- `CreatedDate` (DATETIME) - When the account was created
- `LastModified` (DATETIME) - Last modification timestamp

Constraints:
- CHECK constraint ensures AccountLevel is between 0-2
- UNIQUE constraint on Username and Email

#### 2. AccountLevels Table (Optional Reference)
Lookup table with account level descriptions

### Migration Steps

**Option 1: For New Database**
- Use `UserInfo.sql` to create the table from scratch

**Option 2: For Existing Database**
1. Run `Migration_AddAccountLevel.sql` on your existing database
2. This will safely add the AccountLevel and Username columns
3. Existing users will default to AccountLevel = 2 (Viewer)
4. Usernames will be auto-generated from email addresses

**Option 3: Create Lookup Table (Optional)**
- Run `AccountLevels.sql` to create reference table

## C# Implementation

### New Classes

#### AccountLevel.cs
- **AccountLevel enum**: Defines the three privilege levels
- **AccountLevelHelper**: Helper methods for working with account levels

```csharp
// Convert from database int to enum
AccountLevel level = AccountLevelHelper.FromInt(2); // Viewer

// Get display name
string name = AccountLevelHelper.GetDisplayName(AccountLevel.Administrator); // "Administrator"

// Check permissions
bool hasAccess = AccountLevelHelper.HasPermission(userLevel, AccountLevel.User);
```

### Updated Classes

#### User.cs
Enhanced with:
- `userId` and `username` properties
- Account level management methods
- Permission checking methods

```csharp
// Create user from database
User user = new User(
    userId: 1,
    username: "john_doe",
    fName: "John",
    lName: "Doe",
    email: "john@example.com",
    hashedPassword: "...",
    accountLevel: AccountLevel.User
);

// Check permissions
if (user.IsAdministrator()) {
    // Show admin features
}

if (user.HasPermission(AccountLevel.User)) {
    // Allow user-level actions
}
```

## Azure Function Updates

The Azure Functions already support AccountLevel:
- **CreateAccount**: New users default to AccountLevel = 2 (Viewer)
- **Login**: Returns user's AccountLevel in the response

## Usage Examples

### Setting User Levels

```csharp
// In your admin panel
User admin = GetCurrentUser();
User targetUser = GetUserById(userId);

bool promoted;
admin.PromoteUser(targetUser, adminPassword, AccountLevel.User, out promoted);

if (promoted) {
    // Update database
    UpdateUserAccountLevel(targetUser.GetUserId(), (int)targetUser.GetAccountLevel());
}
```

### Protecting Features

```csharp
// In UniformManager.cs or other forms
public void ShowAdminFeatures()
{
    if (!currentUser.IsAdministrator()) {
        MessageBox.Show("Access denied. Administrator privileges required.", 
                       "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
    }
    
    // Show admin features
}

public void AllowEditing()
{
    if (currentUser.IsViewer()) {
        btnEdit.Enabled = false;
        btnDelete.Enabled = false;
        // Make everything read-only
    }
}
```

### Database Queries

```sql
-- Get all administrators
SELECT * FROM UserInfo WHERE AccountLevel = 0

-- Count users by level
SELECT 
    AccountLevel,
    COUNT(*) as UserCount
FROM UserInfo
GROUP BY AccountLevel

-- Promote a user to administrator
UPDATE UserInfo 
SET AccountLevel = 0, LastModified = GETDATE()
WHERE UserId = 123
```

## Account Level Meanings

| Level | Name | Numeric Value | Default Permissions |
|-------|------|---------------|---------------------|
| Administrator | Admin | 0 | Full access - manage users, edit all records, system settings |
| User | Standard User | 1 | Create/edit own records, view others, limited features |
| Viewer | Read-Only | 2 | View-only access, cannot create or modify |

## Security Best Practices

1. **New Account Default**: All new accounts default to Viewer (2) for security
2. **Admin Promotion**: Only existing Administrators can promote users
3. **Password Verification**: Promotions require admin password verification
4. **Audit Trail**: Use `LastModified` to track permission changes

## Next Steps

1. Stop your running application
2. Run the migration script: `Migration_AddAccountLevel.sql`
3. Restart your application
4. Create an admin user or promote an existing user to admin
5. Implement UI features based on user account levels

## Testing

```sql
-- Create test users with different levels
INSERT INTO UserInfo (Username, FirstName, LastName, Email, HashedPassword, AccountLevel)
VALUES 
    ('admin', 'Admin', 'User', 'admin@test.com', 'hashed_password', 0),
    ('standard', 'Standard', 'User', 'user@test.com', 'hashed_password', 1),
    ('viewer', 'View', 'Only', 'viewer@test.com', 'hashed_password', 2)
```

## Troubleshooting

**Issue**: Column 'AccountLevel' does not exist
- **Solution**: Run the migration script

**Issue**: Can't set user to Administrator
- **Solution**: Manually update first user in database to AccountLevel = 0

**Issue**: Application won't start after changes
- **Solution**: Stop the application completely, rebuild solution, then restart
