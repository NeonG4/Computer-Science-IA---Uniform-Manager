-- =============================================
-- FORCE FRESH DATABASE SETUP
-- This deletes physical files and recreates everything
-- WARNING: ALL DATA WILL BE LOST!
-- =============================================

USE [master]
GO

PRINT '========================================='
PRINT 'FORCE FRESH DATABASE SETUP'
PRINT 'WARNING: Deleting all data and files!'
PRINT '========================================='
PRINT ''

-- =============================================
-- 1. SET DATABASE TO SINGLE USER AND DROP
-- =============================================
IF EXISTS (SELECT * FROM sys.databases WHERE name = 'DatabasestorageIA')
BEGIN
    PRINT 'Setting database to single user mode...'
    
    ALTER DATABASE [DatabasestorageIA] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    
    PRINT 'Dropping database...'
    DROP DATABASE [DatabasestorageIA];
    
    PRINT '? Database dropped'
END
ELSE
BEGIN
    PRINT '? Database does not exist in SQL Server'
END
GO

-- Wait a moment for files to be released
WAITFOR DELAY '00:00:02'
GO

PRINT 'Creating new database...'
GO

-- =============================================
-- 2. CREATE DATABASE (Force new files)
-- =============================================
CREATE DATABASE [DatabasestorageIA]
ON PRIMARY 
(
    NAME = N'DatabasestorageIA',
    FILENAME = N'C:\Users\1117078\DatabasestorageIA_NEW.mdf',
    SIZE = 8MB,
    FILEGROWTH = 64MB
)
LOG ON 
(
    NAME = N'DatabasestorageIA_log',
    FILENAME = N'C:\Users\1117078\DatabasestorageIA_NEW_log.ldf',
    SIZE = 8MB,
    FILEGROWTH = 64MB
)
GO

PRINT '? New database created with fresh files'
GO

USE [DatabasestorageIA]
GO

PRINT ''
PRINT '========================================='
PRINT 'Creating Tables'
PRINT '========================================='
PRINT ''

-- =============================================
-- 3. Create UserInfo Table
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
    [AccountLevel] INT NOT NULL DEFAULT 2,
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
    [AccountLevel] INT NOT NULL,
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
-- 6. Create Students Table
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
-- 7. Create Uniforms Table
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
    [Status] INT NOT NULL DEFAULT 0,
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
-- 9. Verify Tables
-- =============================================
PRINT ''
PRINT '========================================='
PRINT 'Verifying Database Structure'
PRINT '========================================='
GO

SELECT 
    TABLE_NAME as [Table],
    (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = t.TABLE_NAME) as [Columns]
FROM INFORMATION_SCHEMA.TABLES t
WHERE TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_NAME
GO

PRINT ''
PRINT '========================================='
PRINT 'SUCCESS! Database Ready! ?'
PRINT '========================================='
PRINT ''
PRINT 'Fresh database created at:'
PRINT '  C:\Users\1117078\DatabasestorageIA_NEW.mdf'
PRINT ''
PRINT 'All tables created and empty:'
PRINT '  ? UserInfo'
PRINT '  ? Organizations'
PRINT '  ? UserOrganizations'
PRINT '  ? Students (with OrganizationId)'
PRINT '  ? Uniforms (with OrganizationId)'
PRINT '  ? OrganizationJoinRequests'
PRINT ''
PRINT 'You can now:'
PRINT '  1. Delete old files manually if needed:'
PRINT '     - C:\Users\1117078\DatabasestorageIA.mdf'
PRINT '     - C:\Users\1117078\DatabasestorageIA_log.ldf'
PRINT '  2. Start your Azure Function (F5)'
PRINT '  3. Run your WinForms application'
PRINT '  4. Create account ? Create org ? Add students!'
PRINT '========================================='
GO
