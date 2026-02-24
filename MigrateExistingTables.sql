-- =============================================
-- Migration Script - Add Organization Support to Existing Tables
-- Run this to upgrade existing Students and Uniforms tables
-- =============================================

USE [DatabasestorageIA]
GO

PRINT '========================================='
PRINT 'Migrating Existing Tables to Multi-Org'
PRINT '========================================='
PRINT ''

-- =============================================
-- 1. Ensure Organizations table exists with default org
-- =============================================
PRINT 'Checking for default organization...'
GO

-- Create a default organization if none exists
IF NOT EXISTS (SELECT * FROM [dbo].[Organizations])
BEGIN
    PRINT 'Creating default organization...'
    INSERT INTO [dbo].[Organizations] ([OrganizationName], [OrganizationCode], [Description])
    VALUES ('Default Organization', 'DEFAULT', 'Migrated data from single-org system')
    PRINT '? Default organization created'
END
ELSE
BEGIN
    PRINT '? Organizations already exist'
END
GO

-- =============================================
-- 2. Add OrganizationId to Students table
-- =============================================
PRINT ''
PRINT 'Migrating Students table...'
GO

-- Check if OrganizationId column exists
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Students') AND name = 'OrganizationId')
BEGIN
    PRINT 'Adding OrganizationId column to Students...'
    
    -- Add the column (allow NULL temporarily)
    ALTER TABLE [dbo].[Students] ADD [OrganizationId] INT NULL
    PRINT '? Column added'
    
    -- Get the first organization ID (default org)
    DECLARE @DefaultOrgId INT
    SELECT @DefaultOrgId = MIN([OrganizationId]) FROM [dbo].[Organizations]
    
    -- Update all existing students to default organization
    PRINT 'Assigning existing students to default organization...'
    UPDATE [dbo].[Students] SET [OrganizationId] = @DefaultOrgId WHERE [OrganizationId] IS NULL
    PRINT '? Students assigned'
    
    -- Make column NOT NULL
    ALTER TABLE [dbo].[Students] ALTER COLUMN [OrganizationId] INT NOT NULL
    PRINT '? Column constraint updated'
    
    -- Add foreign key
    IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Students_Organization')
    BEGIN
        ALTER TABLE [dbo].[Students] ADD CONSTRAINT [FK_Students_Organization] 
            FOREIGN KEY ([OrganizationId]) REFERENCES [Organizations]([OrganizationId]) ON DELETE CASCADE
        PRINT '? Foreign key added'
    END
    
    -- Drop old unique constraint on StudentIdentifier if it exists
    IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'UQ__Students' AND object_id = OBJECT_ID('dbo.Students'))
    BEGIN
        DECLARE @ConstraintName NVARCHAR(200)
        SELECT @ConstraintName = name FROM sys.indexes 
        WHERE name LIKE 'UQ__Students%' AND object_id = OBJECT_ID('dbo.Students')
        
        IF @ConstraintName IS NOT NULL
        BEGIN
            EXEC('ALTER TABLE [dbo].[Students] DROP CONSTRAINT [' + @ConstraintName + ']')
            PRINT '? Old unique constraint removed'
        END
    END
    
    -- Add new unique constraint on (OrganizationId, StudentIdentifier)
    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'UQ_Students_OrgIdentifier')
    BEGIN
        ALTER TABLE [dbo].[Students] ADD CONSTRAINT [UQ_Students_OrgIdentifier] 
            UNIQUE ([OrganizationId], [StudentIdentifier])
        PRINT '? New unique constraint added'
    END
    
    -- Add index on OrganizationId
    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Students_Organization')
    BEGIN
        CREATE NONCLUSTERED INDEX [IX_Students_Organization]
        ON [dbo].[Students] ([OrganizationId])
        PRINT '? Index added'
    END
    
    PRINT '? Students table migration complete'
END
ELSE
BEGIN
    PRINT '? Students table already has OrganizationId'
END
GO

-- =============================================
-- 3. Add OrganizationId to Uniforms table
-- =============================================
PRINT ''
PRINT 'Migrating Uniforms table...'
GO

-- Check if OrganizationId column exists
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Uniforms') AND name = 'OrganizationId')
BEGIN
    PRINT 'Adding OrganizationId column to Uniforms...'
    
    -- Add the column (allow NULL temporarily)
    ALTER TABLE [dbo].[Uniforms] ADD [OrganizationId] INT NULL
    PRINT '? Column added'
    
    -- Get the first organization ID (default org)
    DECLARE @DefaultOrgId INT
    SELECT @DefaultOrgId = MIN([OrganizationId]) FROM [dbo].[Organizations]
    
    -- Update all existing uniforms to default organization
    PRINT 'Assigning existing uniforms to default organization...'
    UPDATE [dbo].[Uniforms] SET [OrganizationId] = @DefaultOrgId WHERE [OrganizationId] IS NULL
    PRINT '? Uniforms assigned'
    
    -- Make column NOT NULL
    ALTER TABLE [dbo].[Uniforms] ALTER COLUMN [OrganizationId] INT NOT NULL
    PRINT '? Column constraint updated'
    
    -- Add foreign key
    IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Uniforms_Organization')
    BEGIN
        ALTER TABLE [dbo].[Uniforms] ADD CONSTRAINT [FK_Uniforms_Organization] 
            FOREIGN KEY ([OrganizationId]) REFERENCES [Organizations]([OrganizationId]) ON DELETE CASCADE
        PRINT '? Foreign key added'
    END
    
    -- Drop old unique constraint on UniformIdentifier if it exists
    IF EXISTS (SELECT * FROM sys.indexes WHERE name LIKE 'UQ__Uniforms%' AND object_id = OBJECT_ID('dbo.Uniforms'))
    BEGIN
        DECLARE @ConstraintName NVARCHAR(200)
        SELECT @ConstraintName = name FROM sys.indexes 
        WHERE name LIKE 'UQ__Uniforms%' AND object_id = OBJECT_ID('dbo.Uniforms')
        
        IF @ConstraintName IS NOT NULL
        BEGIN
            EXEC('ALTER TABLE [dbo].[Uniforms] DROP CONSTRAINT [' + @ConstraintName + ']')
            PRINT '? Old unique constraint removed'
        END
    END
    
    -- Add new unique constraint on (OrganizationId, UniformIdentifier)
    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'UQ_Uniforms_OrgIdentifier')
    BEGIN
        ALTER TABLE [dbo].[Uniforms] ADD CONSTRAINT [UQ_Uniforms_OrgIdentifier] 
            UNIQUE ([OrganizationId], [UniformIdentifier])
        PRINT '? New unique constraint added'
    END
    
    -- Add index on OrganizationId
    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Uniforms_Organization')
    BEGIN
        CREATE NONCLUSTERED INDEX [IX_Uniforms_Organization]
        ON [dbo].[Uniforms] ([OrganizationId])
        PRINT '? Index added'
    END
    
    PRINT '? Uniforms table migration complete'
END
ELSE
BEGIN
    PRINT '? Uniforms table already has OrganizationId'
END
GO

-- =============================================
-- 4. Display Results
-- =============================================
PRINT ''
PRINT '========================================='
PRINT 'Migration Summary'
PRINT '========================================='
PRINT ''

-- Count records
DECLARE @OrgCount INT, @StudentCount INT, @UniformCount INT, @UserOrgCount INT

SELECT @OrgCount = COUNT(*) FROM [dbo].[Organizations]
SELECT @StudentCount = COUNT(*) FROM [dbo].[Students]
SELECT @UniformCount = COUNT(*) FROM [dbo].[Uniforms]
SELECT @UserOrgCount = COUNT(*) FROM [dbo].[UserOrganizations]

PRINT 'Organizations: ' + CAST(@OrgCount AS VARCHAR(10))
PRINT 'Students: ' + CAST(@StudentCount AS VARCHAR(10))
PRINT 'Uniforms: ' + CAST(@UniformCount AS VARCHAR(10))
PRINT 'User-Org Relationships: ' + CAST(@UserOrgCount AS VARCHAR(10))
PRINT ''

-- Show organizations
IF @OrgCount > 0
BEGIN
    PRINT 'Organizations in database:'
    SELECT OrganizationId, OrganizationName, OrganizationCode, 
           CASE WHEN IsActive = 1 THEN 'Active' ELSE 'Inactive' END AS Status
    FROM [dbo].[Organizations]
    ORDER BY OrganizationName
END

PRINT ''
PRINT '========================================='
PRINT 'Migration Complete! ?'
PRINT '========================================='
PRINT ''
PRINT 'Next steps:'
PRINT '  1. Restart the Azure Function'
PRINT '  2. Login to the WinForms app'
PRINT '  3. You should see existing data in the default organization'
PRINT '  4. Create new organizations as needed'
PRINT '========================================='
GO
