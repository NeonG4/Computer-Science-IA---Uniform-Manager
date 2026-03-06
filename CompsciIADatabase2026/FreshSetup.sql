-- =============================================
-- FRESH DATABASE SETUP - DROPS AND RECREATES EVERYTHING
-- WARNING: This will DELETE ALL DATA if the database exists!
-- =============================================

USE [master]
GO

PRINT '========================================='
PRINT 'FRESH DATABASE SETUP'
PRINT 'WARNING: Deleting existing database!'
PRINT '========================================='
PRINT ''

-- =============================================
-- 1. DROP DATABASE IF EXISTS
-- =============================================
IF EXISTS (SELECT * FROM sys.databases WHERE name = 'DatabasestorageIA')
BEGIN
    PRINT 'Dropping existing database...'
    
    -- Set database to single user mode to force close connections
    ALTER DATABASE [DatabasestorageIA] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    
    -- Drop the database
    DROP DATABASE [DatabasestorageIA];
    
    PRINT '? Old database dropped'
END
GO

PRINT 'Creating new database...'
GO

-- =============================================
-- 2. CREATE DATABASE
-- =============================================
CREATE DATABASE [DatabasestorageIA]
GO

PRINT '? Database created: DatabasestorageIA'
GO

USE [DatabasestorageIA]
GO

PRINT ''
PRINT '========================================='
PRINT 'Creating Tables'
PRINT '========================================='
PRINT ''

-- =============================================
-- 3. Create UserInfo Table (Base table)
-- =============================================
PRINT 'Creating UserInfo table...'
GO

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

PRINT '? UserInfo table created'
GO

-- =============================================
-- 4. Create Organizations Table
-- =============================================
PRINT 'Creating Organizations table...'
GO

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

PRINT '? Organizations table created'
GO

-- =============================================
-- 5. Create UserOrganizations Table
-- =============================================
PRINT 'Creating UserOrganizations table...'
GO

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

PRINT '? UserOrganizations table created'
GO

-- =============================================
-- 6. Create Students Table (WITH OrganizationId)
-- =============================================
PRINT 'Creating Students table...'
GO

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

PRINT '? Students table created'
GO

-- =============================================
-- 7. Create Uniforms Table (WITH OrganizationId)
-- =============================================
PRINT 'Creating Uniforms table...'
GO

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

PRINT '? Uniforms table created'
GO

-- =============================================
-- 8. Create OrganizationJoinRequests Table
-- =============================================
PRINT 'Creating OrganizationJoinRequests table...'
GO

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

PRINT '? OrganizationJoinRequests table created'
GO

-- =============================================
-- 9. Display Summary
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
PRINT 'FRESH DATABASE SETUP COMPLETE! ?'
PRINT '========================================='
PRINT ''
PRINT 'All tables created with NO data:'
PRINT '  ? UserInfo (authentication)'
PRINT '  ? Organizations (multi-tenant)'
PRINT '  ? UserOrganizations (membership)'
PRINT '  ? Students (with OrganizationId)'
PRINT '  ? Uniforms (with OrganizationId)'
PRINT '  ? OrganizationJoinRequests (join workflow)'
PRINT ''
PRINT 'Database is completely empty and ready!'
PRINT ''
PRINT 'Next steps:'
PRINT '  1. Start Azure Function: F5'
PRINT '  2. Run WinForms application'
PRINT '  3. Create a new account'
PRINT '  4. Create your organization'
PRINT '  5. Add students and manage uniforms!'
PRINT '========================================='
GO
