-- =============================================
-- Complete Database Setup Script
-- Run this script to create all tables for the Uniform Manager system
-- =============================================

USE [DatabasestorageIA]
GO

PRINT '========================================='
PRINT 'Starting Complete Database Setup'
PRINT '========================================='
GO

-- =============================================
-- 1. Create UserInfo Table (Base table - no dependencies)
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
        [AccountLevel] INT NOT NULL DEFAULT 2, -- 0=Administrator, 1=User, 2=Viewer
        [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
        [LastModified] DATETIME NULL,
        CONSTRAINT [CK_UserInfo_AccountLevel] CHECK ([AccountLevel] >= 0 AND [AccountLevel] <= 2)
    )
    
    CREATE NONCLUSTERED INDEX [IX_UserInfo_AccountLevel]
    ON [dbo].[UserInfo] ([AccountLevel])
    
    PRINT '? UserInfo table created successfully.'
END
ELSE
BEGIN
    PRINT '? UserInfo table already exists.'
END
GO

-- =============================================
-- 2. Create Students Table (Depends on UserInfo)
-- =============================================
PRINT 'Creating Students table...'
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Students' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE [dbo].[Students]
    (
        [StudentId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [StudentIdentifier] NVARCHAR(50) NOT NULL UNIQUE,
        [FirstName] NVARCHAR(100) NOT NULL,
        [LastName] NVARCHAR(100) NOT NULL,
        [Grade] INT NOT NULL,
        [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
        [LastModified] DATETIME NULL,
        [ModifiedBy] INT NULL,
        CONSTRAINT [FK_Students_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [UserInfo]([UserId]),
        CONSTRAINT [CK_Students_Grade] CHECK ([Grade] >= 1 AND [Grade] <= 12)
    )
    
    CREATE NONCLUSTERED INDEX [IX_Students_Grade]
    ON [dbo].[Students] ([Grade])
    
    CREATE NONCLUSTERED INDEX [IX_Students_LastName]
    ON [dbo].[Students] ([LastName])
    
    CREATE NONCLUSTERED INDEX [IX_Students_FullName]
    ON [dbo].[Students] ([FirstName], [LastName])
    
    PRINT '? Students table created successfully.'
END
ELSE
BEGIN
    PRINT '? Students table already exists.'
END
GO

-- =============================================
-- 3. Create Uniforms Table (Depends on UserInfo)
-- =============================================
PRINT 'Creating Uniforms table...'
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Uniforms' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE [dbo].[Uniforms]
    (
        [UniformId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [UniformIdentifier] NVARCHAR(50) NOT NULL UNIQUE,
        [UniformType] INT NOT NULL, -- Maps to UniformClothing enum
        [Size] INT NOT NULL,
        [IsCheckedOut] BIT NOT NULL DEFAULT 0,
        [AssignedStudentId] NVARCHAR(50) NULL,
        [Conditions] NVARCHAR(MAX) NULL, -- JSON array of conditions
        [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
        [LastModified] DATETIME NULL,
        [ModifiedBy] INT NULL,
        CONSTRAINT [FK_Uniforms_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [UserInfo]([UserId])
    )
    
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
    PRINT '? Uniforms table already exists.'
END
GO

-- =============================================
-- 4. Insert Sample Data (Optional)
-- =============================================
PRINT 'Checking for sample data...'
GO

-- Insert sample students if table is empty
IF NOT EXISTS (SELECT * FROM [dbo].[Students])
BEGIN
    PRINT 'Inserting sample students...'
    
    INSERT INTO [dbo].[Students] ([StudentIdentifier], [FirstName], [LastName], [Grade])
    VALUES
        ('S2024001', 'John',        'Smith',     9),
        ('S2024002', 'Emily',       'Johnson',   10),
        ('S2024003', 'Michael',     'Brown',     11),
        ('S2024004', 'Sarah',       'Davis',     12),
        ('S2024005', 'David',       'Wilson',    9),
        ('S2024006', 'Jessica',     'Martinez',  10),
        ('S2024007', 'Daniel',      'Anderson',  11),
        ('S2024008', 'Ashley',      'Taylor',    12),
        ('S2024009', 'Christopher', 'Thomas',    9),
        ('S2024010', 'Amanda',      'Garcia',    10);
    
    PRINT '? Sample students inserted.'
END
ELSE
BEGIN
    PRINT '? Students table already has data.'
END
GO

-- Insert sample uniforms if table is empty
IF NOT EXISTS (SELECT * FROM [dbo].[Uniforms])
BEGIN
    PRINT 'Inserting sample uniforms...'
    INSERT INTO [dbo].[Uniforms] 
        ([UniformIdentifier], [UniformType], [Size], [IsCheckedOut], [Conditions])
    VALUES
        ('CC-001', 0, 42, 0, '[]'),
        ('CC-002', 0, 44, 0, '[]'),
        ('DM-001', 1, 40, 0, '[]'),
        ('HAT-001', 2, 7, 0, '[]'),
        ('MC-001', 3, 38, 0, '[]'),
        ('MC-002', 3, 40, 0, '[]'),
        ('MS-001', 4, 32, 0, '[]'),
        ('SOCK-001', 5, 10, 0, '[]'),
        ('PANTS-001', 6, 34, 0, '[]'),
        ('PANTS-002', 6, 36, 0, '[]')
    
    PRINT '? Sample uniforms inserted.'
END
ELSE
BEGIN
    PRINT '? Uniforms table already has data.'
END
GO

-- =============================================
-- 5. Display Summary
-- =============================================
PRINT ''
PRINT '========================================='
PRINT 'Database Setup Summary'
PRINT '========================================='
GO

SELECT 
    'UserInfo' as [Table],
    COUNT(*) as [Row Count]
FROM [dbo].[UserInfo]
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
PRINT 'Setup Complete! ?'
PRINT '========================================='
PRINT ''
PRINT 'Tables created:'
PRINT '  • UserInfo (base table for authentication)'
PRINT '  • Students (student records)'
PRINT '  • Uniforms (uniform inventory)'
PRINT ''
PRINT 'You can now:'
PRINT '  1. Start the Azure Function (F5)'
PRINT '  2. Create user accounts via API'
PRINT '  3. Manage students and uniforms'
PRINT '========================================='
GO
