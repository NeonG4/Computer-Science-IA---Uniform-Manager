-- =============================================
-- Local Database Setup Script with Organizations
-- Uniform Manager System - Multi-Organization Support
-- =============================================

USE [master]
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'DatabasestorageIA')
BEGIN
    PRINT 'Creating DatabasestorageIA database...'
    CREATE DATABASE [DatabasestorageIA]
    PRINT '? Database created successfully.'
END
ELSE
BEGIN
    PRINT '? Database DatabasestorageIA already exists.'
END
GO

USE [DatabasestorageIA]
GO

PRINT '========================================='
PRINT 'Starting Multi-Organization Database Setup'
PRINT '========================================='
GO

-- =============================================
-- 1. Create Organizations Table
-- =============================================
PRINT 'Creating Organizations table...'
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Organizations' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE [dbo].[Organizations]
    (
        [OrganizationId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [OrganizationName] NVARCHAR(100) NOT NULL,
        [OrganizationCode] NVARCHAR(50) NOT NULL UNIQUE,
        [Description] NVARCHAR(500) NULL,
        [IsActive] BIT NOT NULL DEFAULT 1,
        [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
        [CreatedBy] INT NULL
    )
    
    CREATE NONCLUSTERED INDEX [IX_Organizations_IsActive]
    ON [dbo].[Organizations] ([IsActive])
    
    PRINT '? Organizations table created successfully.'
END
ELSE
BEGIN
    PRINT '? Organizations table already exists.'
END
GO

-- =============================================
-- 2. Create UserInfo Table
-- =============================================
PRINT 'Creating UserInfo table...'
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'UserInfo' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE [dbo].[UserInfo]
    (
        [UserId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Username] NVARCHAR(50) NOT NULL UNIQUE,
        [FirstName] NVARCHAR(50) NOT NULL,
        [LastName] NVARCHAR(50) NOT NULL,
        [Email] NVARCHAR(100) NOT NULL UNIQUE,
        [HashedPassword] NVARCHAR(255) NOT NULL,
        [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
        [LastModified] DATETIME NULL
    )
    
    PRINT '? UserInfo table created successfully.'
END
ELSE
BEGIN
    PRINT '? UserInfo table already exists.'
END
GO

-- =============================================
-- 3. Create UserOrganizations Junction Table
-- =============================================
PRINT 'Creating UserOrganizations table...'
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'UserOrganizations' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE [dbo].[UserOrganizations]
    (
        [UserOrganizationId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [UserId] INT NOT NULL,
        [OrganizationId] INT NOT NULL,
        [AccountLevel] INT NOT NULL DEFAULT 2, -- 0=Admin, 1=User, 2=Viewer
        [JoinedDate] DATETIME NOT NULL DEFAULT GETDATE(),
        [IsActive] BIT NOT NULL DEFAULT 1,
        CONSTRAINT [FK_UserOrganizations_User] FOREIGN KEY ([UserId]) REFERENCES [UserInfo]([UserId]) ON DELETE CASCADE,
        CONSTRAINT [FK_UserOrganizations_Organization] FOREIGN KEY ([OrganizationId]) REFERENCES [Organizations]([OrganizationId]) ON DELETE CASCADE,
        CONSTRAINT [CK_UserOrganizations_AccountLevel] CHECK ([AccountLevel] >= 0 AND [AccountLevel] <= 2),
        CONSTRAINT [UQ_UserOrganizations] UNIQUE ([UserId], [OrganizationId])
    )
    
    CREATE NONCLUSTERED INDEX [IX_UserOrganizations_User]
    ON [dbo].[UserOrganizations] ([UserId])
    
    CREATE NONCLUSTERED INDEX [IX_UserOrganizations_Organization]
    ON [dbo].[UserOrganizations] ([OrganizationId])
    
    PRINT '? UserOrganizations table created successfully.'
END
ELSE
BEGIN
    PRINT '? UserOrganizations table already exists.'
END
GO

-- =============================================
-- 4. Create/Update Students Table
-- =============================================
PRINT 'Creating Students table...'
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Students' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE [dbo].[Students]
    (
        [StudentId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [OrganizationId] INT NOT NULL,
        [StudentIdentifier] NVARCHAR(50) NOT NULL,
        [FirstName] NVARCHAR(100) NOT NULL,
        [LastName] NVARCHAR(100) NOT NULL,
        [Grade] INT NOT NULL,
        [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
        [LastModified] DATETIME NULL,
        [ModifiedBy] INT NULL,
        CONSTRAINT [FK_Students_Organization] FOREIGN KEY ([OrganizationId]) REFERENCES [Organizations]([OrganizationId]) ON DELETE CASCADE,
        CONSTRAINT [FK_Students_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [UserInfo]([UserId]),
        CONSTRAINT [CK_Students_Grade] CHECK ([Grade] >= 1 AND [Grade] <= 12),
        CONSTRAINT [UQ_Students_OrgIdentifier] UNIQUE ([OrganizationId], [StudentIdentifier])
    )
    
    CREATE NONCLUSTERED INDEX [IX_Students_Organization]
    ON [dbo].[Students] ([OrganizationId])
    
    CREATE NONCLUSTERED INDEX [IX_Students_Grade]
    ON [dbo].[Students] ([Grade])
    
    CREATE NONCLUSTERED INDEX [IX_Students_LastName]
    ON [dbo].[Students] ([LastName])
    
    PRINT '? Students table created successfully.'
END
ELSE
BEGIN
    -- Check if OrganizationId column exists, if not add it
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Students') AND name = 'OrganizationId')
    BEGIN
        PRINT 'Adding OrganizationId to existing Students table...'
        
        -- Add the column (allow NULL temporarily)
        ALTER TABLE [dbo].[Students] ADD [OrganizationId] INT NULL
        
        -- Create a default organization if none exists
        IF NOT EXISTS (SELECT * FROM [dbo].[Organizations])
        BEGIN
            INSERT INTO [dbo].[Organizations] ([OrganizationName], [OrganizationCode], [Description])
            VALUES ('Default Organization', 'DEFAULT', 'Migrated data from single-org system')
        END
        
        -- Update existing records to default organization
        DECLARE @DefaultOrgId INT
        SELECT @DefaultOrgId = MIN([OrganizationId]) FROM [dbo].[Organizations]
        UPDATE [dbo].[Students] SET [OrganizationId] = @DefaultOrgId WHERE [OrganizationId] IS NULL
        
        -- Make column NOT NULL
        ALTER TABLE [dbo].[Students] ALTER COLUMN [OrganizationId] INT NOT NULL
        
        -- Add foreign key
        ALTER TABLE [dbo].[Students] ADD CONSTRAINT [FK_Students_Organization] 
            FOREIGN KEY ([OrganizationId]) REFERENCES [Organizations]([OrganizationId]) ON DELETE CASCADE
        
        -- Add unique constraint
        IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'UQ_Students_OrgIdentifier')
        BEGIN
            ALTER TABLE [dbo].[Students] ADD CONSTRAINT [UQ_Students_OrgIdentifier] 
                UNIQUE ([OrganizationId], [StudentIdentifier])
        END
        
        PRINT '? Students table updated with organization support.'
    END
    ELSE
    BEGIN
        PRINT '? Students table already has organization support.'
    END
END
GO

-- =============================================
-- 5. Create/Update Uniforms Table
-- =============================================
PRINT 'Creating Uniforms table...'
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Uniforms' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE [dbo].[Uniforms]
    (
        [UniformId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [OrganizationId] INT NOT NULL,
        [UniformIdentifier] NVARCHAR(50) NOT NULL,
        [UniformType] INT NOT NULL,
        [Size] INT NOT NULL,
        [IsCheckedOut] BIT NOT NULL DEFAULT 0,
        [AssignedStudentId] NVARCHAR(50) NULL,
        [Conditions] NVARCHAR(MAX) NULL,
        [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
        [LastModified] DATETIME NULL,
        [ModifiedBy] INT NULL,
        CONSTRAINT [FK_Uniforms_Organization] FOREIGN KEY ([OrganizationId]) REFERENCES [Organizations]([OrganizationId]) ON DELETE CASCADE,
        CONSTRAINT [FK_Uniforms_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [UserInfo]([UserId]),
        CONSTRAINT [UQ_Uniforms_OrgIdentifier] UNIQUE ([OrganizationId], [UniformIdentifier])
    )
    
    CREATE NONCLUSTERED INDEX [IX_Uniforms_Organization]
    ON [dbo].[Uniforms] ([OrganizationId])
    
    CREATE NONCLUSTERED INDEX [IX_Uniforms_Type]
    ON [dbo].[Uniforms] ([UniformType])
    
    CREATE NONCLUSTERED INDEX [IX_Uniforms_IsCheckedOut]
    ON [dbo].[Uniforms] ([IsCheckedOut])
    
    CREATE NONCLUSTERED INDEX [IX_Uniforms_AssignedStudent]
    ON [dbo].[Uniforms] ([AssignedStudentId])
    WHERE [AssignedStudentId] IS NOT NULL
    
    PRINT '? Uniforms table created successfully.'
END
ELSE
BEGIN
    -- Check if OrganizationId column exists, if not add it
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Uniforms') AND name = 'OrganizationId')
    BEGIN
        PRINT 'Adding OrganizationId to existing Uniforms table...'
        
        -- Add the column (allow NULL temporarily)
        ALTER TABLE [dbo].[Uniforms] ADD [OrganizationId] INT NULL
        
        -- Update existing records to default organization
        DECLARE @DefaultOrgId INT
        SELECT @DefaultOrgId = MIN([OrganizationId]) FROM [dbo].[Organizations]
        UPDATE [dbo].[Uniforms] SET [OrganizationId] = @DefaultOrgId WHERE [OrganizationId] IS NULL
        
        -- Make column NOT NULL
        ALTER TABLE [dbo].[Uniforms] ALTER COLUMN [OrganizationId] INT NOT NULL
        
        -- Add foreign key
        ALTER TABLE [dbo].[Uniforms] ADD CONSTRAINT [FK_Uniforms_Organization] 
            FOREIGN KEY ([OrganizationId]) REFERENCES [Organizations]([OrganizationId]) ON DELETE CASCADE
        
        -- Add unique constraint
        IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'UQ_Uniforms_OrgIdentifier')
        BEGIN
            ALTER TABLE [dbo].[Uniforms] ADD CONSTRAINT [UQ_Uniforms_OrgIdentifier] 
                UNIQUE ([OrganizationId], [UniformIdentifier])
        END
        
        PRINT '? Uniforms table updated with organization support.'
    END
    ELSE
    BEGIN
        PRINT '? Uniforms table already has organization support.'
    END
END
GO

-- =============================================
-- 6. Insert Sample Data
-- =============================================
PRINT 'Checking for sample data...'
GO

-- Create sample organizations
IF NOT EXISTS (SELECT * FROM [dbo].[Organizations])
BEGIN
    PRINT 'Inserting sample organizations...'
    
    INSERT INTO [dbo].[Organizations] ([OrganizationName], [OrganizationCode], [Description])
    VALUES
        ('Lincoln High School', 'LHS', 'Main campus uniform management'),
        ('Washington Middle School', 'WMS', 'Middle school campus'),
        ('Jefferson Elementary', 'JES', 'Elementary school campus');
    
    PRINT '? Sample organizations inserted.'
END
ELSE
BEGIN
    PRINT '? Organizations table already has data.'
END
GO

-- Insert sample students per organization
DECLARE @OrgId INT
SELECT @OrgId = MIN([OrganizationId]) FROM [dbo].[Organizations]

IF NOT EXISTS (SELECT * FROM [dbo].[Students] WHERE [OrganizationId] = @OrgId)
BEGIN
    PRINT 'Inserting sample students for first organization...'
    
    INSERT INTO [dbo].[Students] ([OrganizationId], [StudentIdentifier], [FirstName], [LastName], [Grade])
    VALUES
        (@OrgId, 'S2024001', 'John', 'Smith', 9),
        (@OrgId, 'S2024002', 'Emily', 'Johnson', 10),
        (@OrgId, 'S2024003', 'Michael', 'Brown', 11),
        (@OrgId, 'S2024004', 'Sarah', 'Davis', 12),
        (@OrgId, 'S2024005', 'David', 'Wilson', 9);
    
    PRINT '? Sample students inserted.'
END
ELSE
BEGIN
    PRINT '? Students table already has data for this organization.'
END
GO

-- Insert sample uniforms per organization
DECLARE @OrgId INT
SELECT @OrgId = MIN([OrganizationId]) FROM [dbo].[Organizations]

IF NOT EXISTS (SELECT * FROM [dbo].[Uniforms] WHERE [OrganizationId] = @OrgId)
BEGIN
    PRINT 'Inserting sample uniforms for first organization...'
    
    INSERT INTO [dbo].[Uniforms] ([OrganizationId], [UniformIdentifier], [UniformType], [Size], [IsCheckedOut], [Conditions])
    VALUES
        (@OrgId, 'CC-001', 0, 42, 0, '[]'),
        (@OrgId, 'CC-002', 0, 44, 0, '[]'),
        (@OrgId, 'DM-001', 1, 40, 0, '[]'),
        (@OrgId, 'HAT-001', 2, 7, 0, '[]'),
        (@OrgId, 'MC-001', 3, 38, 0, '[]');
    
    PRINT '? Sample uniforms inserted.'
END
ELSE
BEGIN
    PRINT '? Uniforms table already has data for this organization.'
END
GO

-- =============================================
-- 7. Display Summary
-- =============================================
PRINT ''
PRINT '========================================='
PRINT 'Database Setup Summary'
PRINT '========================================='
GO

SELECT 
    'Organizations' as [Table],
    COUNT(*) as [Row Count]
FROM [dbo].[Organizations]
UNION ALL
SELECT 
    'UserInfo' as [Table],
    COUNT(*) as [Row Count]
FROM [dbo].[UserInfo]
UNION ALL
SELECT 
    'UserOrganizations' as [Table],
    COUNT(*) as [Row Count]
FROM [dbo].[UserOrganizations]
UNION ALL
SELECT 
    'Students' as [Table],
    COUNT(*) as [Row Count]
FROM [dbo].[Students]
UNION ALL
SELECT 
    'Uniforms' as [Table],
    COUNT(*) as [Row Count]
FROM [dbo].[Uniforms]
ORDER BY [Table]
GO

PRINT ''
PRINT '========================================='
PRINT 'Multi-Organization Setup Complete! ?'
PRINT '========================================='
PRINT ''
PRINT 'Database: DatabasestorageIA'
PRINT 'Server: Local SQL Server Instance'
PRINT ''
PRINT 'Tables created:'
PRINT '  • Organizations (organization definitions)'
PRINT '  • UserInfo (user accounts)'
PRINT '  • UserOrganizations (user roles per org)'
PRINT '  • Students (student records per org)'
PRINT '  • Uniforms (uniform inventory per org)'
PRINT ''
PRINT 'Features:'
PRINT '  ? Multi-organization support'
PRINT '  ? Per-organization user roles'
PRINT '  ? Data isolation between orgs'
PRINT '  ? User can be admin in one org, viewer in another'
PRINT ''
PRINT 'Next steps:'
PRINT '  1. Update local.settings.json with connection string'
PRINT '  2. Start the Azure Function (F5)'
PRINT '  3. Create your first user account'
PRINT '  4. Create your first organization'
PRINT '  5. Launch the desktop application'
PRINT '========================================='
GO
