-- =============================================
-- ULTIMATE RESET - New Database Name
-- This uses a completely different database name
-- to avoid all file conflicts
-- =============================================

USE [master]
GO

PRINT '========================================='
PRINT 'Creating Fresh Database with New Name'
PRINT '========================================='
PRINT ''

-- Drop old database if exists
IF EXISTS (SELECT * FROM sys.databases WHERE name = 'UniformManagerDB')
BEGIN
    ALTER DATABASE [UniformManagerDB] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [UniformManagerDB];
    PRINT '? Dropped old UniformManagerDB'
END

-- Drop legacy database if exists
IF EXISTS (SELECT * FROM sys.databases WHERE name = 'DatabasestorageIA')
BEGIN
    ALTER DATABASE [DatabasestorageIA] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [DatabasestorageIA];
    PRINT '? Dropped old DatabasestorageIA'
END
GO

WAITFOR DELAY '00:00:02'
GO

PRINT 'Creating new database...'
GO

-- Create with new name
CREATE DATABASE [UniformManagerDB]
GO

PRINT '? UniformManagerDB created'
GO

USE [UniformManagerDB]
GO

-- UserInfo
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
CREATE NONCLUSTERED INDEX [IX_UserInfo_AccountLevel] ON [dbo].[UserInfo] ([AccountLevel])
PRINT '? UserInfo'

-- Organizations
CREATE TABLE [dbo].[Organizations]
(
    [OrganizationId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [OrganizationName] NVARCHAR(200) NOT NULL,
    [OrganizationCode] NVARCHAR(50) NOT NULL UNIQUE,
    [Description] NVARCHAR(500) NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [CreatedBy] INT NULL,
    CONSTRAINT [FK_Organizations_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [UserInfo]([UserId])
)
CREATE NONCLUSTERED INDEX [IX_Organizations_Code] ON [dbo].[Organizations] ([OrganizationCode])
PRINT '? Organizations'

-- UserOrganizations
CREATE TABLE [dbo].[UserOrganizations]
(
    [UserOrganizationId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [UserId] INT NOT NULL,
    [OrganizationId] INT NOT NULL,
    [AccountLevel] INT NOT NULL,
    [JoinedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [IsActive] BIT NOT NULL DEFAULT 1,
    CONSTRAINT [FK_UserOrganizations_User] FOREIGN KEY ([UserId]) REFERENCES [UserInfo]([UserId]),
    CONSTRAINT [FK_UserOrganizations_Organization] FOREIGN KEY ([OrganizationId]) REFERENCES [Organizations]([OrganizationId]) ON DELETE CASCADE,
    CONSTRAINT [CK_UserOrganizations_AccountLevel] CHECK ([AccountLevel] >= 0 AND [AccountLevel] <= 2)
)
CREATE NONCLUSTERED INDEX [IX_UserOrganizations_User] ON [dbo].[UserOrganizations] ([UserId])
CREATE NONCLUSTERED INDEX [IX_UserOrganizations_Organization] ON [dbo].[UserOrganizations] ([OrganizationId])
PRINT '? UserOrganizations'

-- Students
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
    CONSTRAINT [UQ_Students_Identifier_Organization] UNIQUE ([StudentIdentifier], [OrganizationId])
)
CREATE NONCLUSTERED INDEX [IX_Students_Organization] ON [dbo].[Students] ([OrganizationId])
PRINT '? Students'

-- Uniforms
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
    CONSTRAINT [UQ_Uniforms_Identifier_Organization] UNIQUE ([UniformIdentifier], [OrganizationId])
)
CREATE NONCLUSTERED INDEX [IX_Uniforms_Organization] ON [dbo].[Uniforms] ([OrganizationId])
PRINT '? Uniforms'

-- OrganizationJoinRequests
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
    CONSTRAINT [FK_JoinRequests_Organization] FOREIGN KEY ([OrganizationId]) REFERENCES [Organizations]([OrganizationId]) ON DELETE CASCADE,
    CONSTRAINT [FK_JoinRequests_User] FOREIGN KEY ([UserId]) REFERENCES [UserInfo]([UserId]),
    CONSTRAINT [FK_JoinRequests_ReviewedBy] FOREIGN KEY ([ReviewedBy]) REFERENCES [UserInfo]([UserId]),
    CONSTRAINT [CK_JoinRequests_Status] CHECK ([Status] >= 0 AND [Status] <= 2),
    CONSTRAINT [CK_JoinRequests_AccountLevel] CHECK ([RequestedAccountLevel] >= 0 AND [RequestedAccountLevel] <= 2)
)
PRINT '? OrganizationJoinRequests'

PRINT ''
PRINT '========================================='
PRINT 'SUCCESS! ???'
PRINT '========================================='
PRINT ''
PRINT 'Database: UniformManagerDB (NEW NAME!)'
PRINT 'All tables created and empty'
PRINT ''
PRINT 'IMPORTANT: Update local.settings.json to:'
PRINT '  "SqlConnection": "Server=(localdb)\\MSSQLLocalDB;Database=UniformManagerDB;Integrated Security=true;"'
PRINT ''
PRINT 'Then restart Azure Function!'
PRINT '========================================='
GO
