-- =============================================
-- Complete Database Setup with Organizations Support
-- This creates the database and all tables
-- =============================================

-- =============================================
-- 1. CREATE DATABASE
-- =============================================
USE [master]
GO

IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'DatabasestorageIA')
BEGIN
    CREATE DATABASE [DatabasestorageIA]
    PRINT '? Database created: DatabasestorageIA'
END
ELSE
BEGIN
    PRINT '? Database already exists: DatabasestorageIA'
END
GO

USE [DatabasestorageIA]
GO

PRINT '========================================='
PRINT 'Starting Complete Database Setup'
PRINT '========================================='
PRINT ''

-- =============================================
-- 2. Create UserInfo Table (Base table - no dependencies)
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
-- 3. Create Organizations Table
-- =============================================
PRINT 'Creating Organizations table...'
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Organizations' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE [dbo].[Organizations]
    (
        [OrganizationId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [OrganizationName] NVARCHAR(200) NOT NULL,
        [OrganizationCode] NVARCHAR(50) NOT NULL UNIQUE,
        [Description] NVARCHAR(500) NULL,
        [IsActive] BIT NOT NULL DEFAULT 1,
        [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
        [CreatedBy] INT NULL,
        CONSTRAINT [FK_Organizations_CreatedBy] 
            FOREIGN KEY ([CreatedBy]) REFERENCES [UserInfo]([UserId])
    )
    
    CREATE NONCLUSTERED INDEX [IX_Organizations_Code]
    ON [dbo].[Organizations] ([OrganizationCode])
    
    PRINT '? Organizations table created successfully.'
END
ELSE
BEGIN
    PRINT '? Organizations table already exists.'
END
GO

-- =============================================
-- 4. Create UserOrganizations Table (Many-to-Many)
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
        [AccountLevel] INT NOT NULL, -- 0=Admin, 1=User, 2=Viewer
        [JoinedDate] DATETIME NOT NULL DEFAULT GETDATE(),
        [IsActive] BIT NOT NULL DEFAULT 1,
        CONSTRAINT [FK_UserOrganizations_User] 
            FOREIGN KEY ([UserId]) REFERENCES [UserInfo]([UserId]),
        CONSTRAINT [FK_UserOrganizations_Organization] 
            FOREIGN KEY ([OrganizationId]) REFERENCES [Organizations]([OrganizationId]) 
            ON DELETE CASCADE,
        CONSTRAINT [CK_UserOrganizations_AccountLevel] 
            CHECK ([AccountLevel] >= 0 AND [AccountLevel] <= 2)
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
-- 5. Create Students Table (WITH OrganizationId)
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
        CONSTRAINT [FK_Students_Organization] 
            FOREIGN KEY ([OrganizationId]) REFERENCES [Organizations]([OrganizationId]) 
            ON DELETE CASCADE,
        CONSTRAINT [FK_Students_ModifiedBy] 
            FOREIGN KEY ([ModifiedBy]) REFERENCES [UserInfo]([UserId]),
        CONSTRAINT [CK_Students_Grade] 
            CHECK ([Grade] >= 1 AND [Grade] <= 12),
        CONSTRAINT [UQ_Students_Identifier_Organization] 
            UNIQUE ([StudentIdentifier], [OrganizationId])
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
    PRINT '? Students table already exists.'
END
GO

-- =============================================
-- 6. Create Uniforms Table (WITH OrganizationId)
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
        [UniformType] INT NOT NULL, -- Maps to UniformClothing enum
        [Size] INT NOT NULL,
        [IsCheckedOut] BIT NOT NULL DEFAULT 0,
        [AssignedStudentId] NVARCHAR(50) NULL,
        [Conditions] NVARCHAR(MAX) NULL, -- JSON array of conditions
        [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
        [LastModified] DATETIME NULL,
        [ModifiedBy] INT NULL,
        CONSTRAINT [FK_Uniforms_Organization] 
            FOREIGN KEY ([OrganizationId]) REFERENCES [Organizations]([OrganizationId]) 
            ON DELETE CASCADE,
        CONSTRAINT [FK_Uniforms_ModifiedBy] 
            FOREIGN KEY ([ModifiedBy]) REFERENCES [UserInfo]([UserId]),
        CONSTRAINT [UQ_Uniforms_Identifier_Organization] 
            UNIQUE ([UniformIdentifier], [OrganizationId])
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
    PRINT '? Uniforms table already exists.'
END
GO

-- =============================================
-- 7. Create OrganizationJoinRequests Table
-- =============================================
PRINT 'Creating OrganizationJoinRequests table...'
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'OrganizationJoinRequests' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE [dbo].[OrganizationJoinRequests]
    (
        [RequestId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [OrganizationId] INT NOT NULL,
        [UserId] INT NOT NULL,
        [RequestedAccountLevel] INT NOT NULL,
        [Status] INT NOT NULL DEFAULT 0, -- 0=Pending, 1=Approved, 2=Rejected
        [RequestMessage] NVARCHAR(500) NULL,
        [RequestedDate] DATETIME NOT NULL DEFAULT GETDATE(),
        [ReviewedBy] INT NULL,
        [ReviewedDate] DATETIME NULL,
        [ReviewNotes] NVARCHAR(500) NULL,
        CONSTRAINT [FK_JoinRequests_Organization] 
            FOREIGN KEY ([OrganizationId]) REFERENCES [Organizations]([OrganizationId]) 
            ON DELETE CASCADE,
        CONSTRAINT [FK_JoinRequests_User] 
            FOREIGN KEY ([UserId]) REFERENCES [UserInfo]([UserId]),
        CONSTRAINT [FK_JoinRequests_ReviewedBy] 
            FOREIGN KEY ([ReviewedBy]) REFERENCES [UserInfo]([UserId]),
        CONSTRAINT [CK_JoinRequests_Status] 
            CHECK ([Status] >= 0 AND [Status] <= 2),
        CONSTRAINT [CK_JoinRequests_AccountLevel] 
            CHECK ([RequestedAccountLevel] >= 0 AND [RequestedAccountLevel] <= 2)
    )
    
    CREATE NONCLUSTERED INDEX [IX_JoinRequests_Organization_Status]
    ON [dbo].[OrganizationJoinRequests] ([OrganizationId], [Status])
    
    CREATE UNIQUE NONCLUSTERED INDEX [UQ_JoinRequests_User_Org_Pending]
    ON [dbo].[OrganizationJoinRequests] ([UserId], [OrganizationId])
    WHERE [Status] = 0
    
    PRINT '? OrganizationJoinRequests table created successfully.'
END
ELSE
BEGIN
    PRINT '? OrganizationJoinRequests table already exists.'
END
GO

-- =============================================
-- 8. Display Summary
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
    'Organizations' as [Table],
    COUNT(*) as [Row Count]
FROM [dbo].[Organizations]
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
UNION ALL
SELECT 
    'OrganizationJoinRequests' as [Table],
    COUNT(*) as [Row Count]
FROM [dbo].[OrganizationJoinRequests]
ORDER BY [Table]
GO

PRINT ''
PRINT '========================================='
PRINT 'Setup Complete! ?'
PRINT '========================================='
PRINT ''
PRINT 'Tables created:'
PRINT '  ? UserInfo (authentication)'
PRINT '  ? Organizations (multi-tenant)'
PRINT '  ? UserOrganizations (user-org membership)'
PRINT '  ? Students (WITH OrganizationId)'
PRINT '  ? Uniforms (WITH OrganizationId)'
PRINT '  ? OrganizationJoinRequests (join workflow)'
PRINT ''
PRINT 'Next steps:'
PRINT '  1. Start the Azure Function (F5)'
PRINT '  2. Run your WinForms application'
PRINT '  3. Create an account'
PRINT '  4. Create an organization'
PRINT '  5. Add students and uniforms!'
PRINT '========================================='
GO
