-- =============================================
-- Complete Database Reset Script
-- Drops all tables and recreates them
-- WARNING: This will delete ALL data
-- =============================================

USE [DATABASESTORAGEIA]
GO

PRINT '========================================='
PRINT 'DATABASE RESET - Dropping All Tables'
PRINT '========================================='
PRINT ''
PRINT 'WARNING: This will delete ALL data!'
PRINT 'Press Ctrl+C to cancel or wait 3 seconds...'
PRINT ''

-- Wait 3 seconds before proceeding
WAITFOR DELAY '00:00:03'
GO

PRINT 'Proceeding with database reset...'
PRINT ''

-- =============================================
-- Drop tables in correct order (respecting foreign keys)
-- =============================================

PRINT 'Dropping tables...'
GO

-- Drop dependent tables first
IF OBJECT_ID('dbo.OrganizationJoinRequests', 'U') IS NOT NULL
BEGIN
    DROP TABLE [dbo].[OrganizationJoinRequests]
    PRINT '? Dropped OrganizationJoinRequests table'
END

IF OBJECT_ID('dbo.UserOrganizations', 'U') IS NOT NULL
BEGIN
    DROP TABLE [dbo].[UserOrganizations]
    PRINT '? Dropped UserOrganizations table'
END

IF OBJECT_ID('dbo.Uniforms', 'U') IS NOT NULL
BEGIN
    DROP TABLE [dbo].[Uniforms]
    PRINT '? Dropped Uniforms table'
END

IF OBJECT_ID('dbo.Students', 'U') IS NOT NULL
BEGIN
    DROP TABLE [dbo].[Students]
    PRINT '? Dropped Students table'
END

IF OBJECT_ID('dbo.Organizations', 'U') IS NOT NULL
BEGIN
    DROP TABLE [dbo].[Organizations]
    PRINT '? Dropped Organizations table'
END

IF OBJECT_ID('dbo.UserInfo', 'U') IS NOT NULL
BEGIN
    DROP TABLE [dbo].[UserInfo]
    PRINT '? Dropped UserInfo table'
END

PRINT ''
PRINT '========================================='
PRINT 'Creating Fresh Tables'
PRINT '========================================='
PRINT ''

-- =============================================
-- Create UserInfo Table (Base user accounts)
-- =============================================

PRINT 'Creating UserInfo table...'
GO

CREATE TABLE [dbo].[UserInfo]
(
    [UserId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [Username] NVARCHAR(50) NOT NULL UNIQUE,
    [FirstName] NVARCHAR(100) NOT NULL,
    [LastName] NVARCHAR(100) NOT NULL,
    [Email] NVARCHAR(255) NOT NULL UNIQUE,
    [HashedPassword] NVARCHAR(255) NOT NULL,
    [AccountLevel] INT NOT NULL DEFAULT 2, -- 0=Admin, 1=User, 2=Viewer (global level)
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [LastModified] DATETIME NULL,
    CONSTRAINT [CK_UserInfo_AccountLevel] CHECK ([AccountLevel] >= 0 AND [AccountLevel] <= 2)
)
GO

-- Indexes for UserInfo
CREATE NONCLUSTERED INDEX [IX_UserInfo_Email] ON [dbo].[UserInfo] ([Email])
CREATE NONCLUSTERED INDEX [IX_UserInfo_Username] ON [dbo].[UserInfo] ([Username])
GO

PRINT '? UserInfo table created'
PRINT ''

-- =============================================
-- Create Organizations Table
-- =============================================

PRINT 'Creating Organizations table...'
GO

CREATE TABLE [dbo].[Organizations]
(
    [OrganizationId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [OrganizationName] NVARCHAR(200) NOT NULL,
    [OrganizationCode] NVARCHAR(50) NOT NULL UNIQUE,
    [Description] NVARCHAR(500) NULL,
    [CreatedBy] INT NOT NULL,
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [IsActive] BIT NOT NULL DEFAULT 1,
    [LastModified] DATETIME NULL,
    CONSTRAINT [FK_Organizations_CreatedBy] FOREIGN KEY ([CreatedBy]) 
        REFERENCES [UserInfo]([UserId])
)
GO

-- Indexes for Organizations
CREATE NONCLUSTERED INDEX [IX_Organizations_Code] ON [dbo].[Organizations] ([OrganizationCode])
CREATE NONCLUSTERED INDEX [IX_Organizations_Active] ON [dbo].[Organizations] ([IsActive])
GO

PRINT '? Organizations table created'
PRINT ''

-- =============================================
-- Create UserOrganizations Table (Many-to-Many with roles)
-- =============================================

PRINT 'Creating UserOrganizations table...'
GO

CREATE TABLE [dbo].[UserOrganizations]
(
    [UserOrganizationId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [UserId] INT NOT NULL,
    [OrganizationId] INT NOT NULL,
    [AccountLevel] INT NOT NULL DEFAULT 2, -- 0=Org Admin, 1=Org User, 2=Org Viewer
    [JoinedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [IsActive] BIT NOT NULL DEFAULT 1,
    [LastModified] DATETIME NULL,
    CONSTRAINT [FK_UserOrganizations_User] FOREIGN KEY ([UserId]) 
        REFERENCES [UserInfo]([UserId]) ON DELETE CASCADE,
    CONSTRAINT [FK_UserOrganizations_Organization] FOREIGN KEY ([OrganizationId]) 
        REFERENCES [Organizations]([OrganizationId]) ON DELETE CASCADE,
    CONSTRAINT [CK_UserOrganizations_AccountLevel] CHECK ([AccountLevel] >= 0 AND [AccountLevel] <= 2),
    CONSTRAINT [UQ_UserOrganizations_User_Org] UNIQUE ([UserId], [OrganizationId])
)
GO

-- Indexes for UserOrganizations
CREATE NONCLUSTERED INDEX [IX_UserOrganizations_User] ON [dbo].[UserOrganizations] ([UserId])
CREATE NONCLUSTERED INDEX [IX_UserOrganizations_Org] ON [dbo].[UserOrganizations] ([OrganizationId])
CREATE NONCLUSTERED INDEX [IX_UserOrganizations_Active] ON [dbo].[UserOrganizations] ([IsActive])
GO

PRINT '? UserOrganizations table created'
PRINT ''

-- =============================================
-- Create Students Table
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
    CONSTRAINT [FK_Students_Organization] FOREIGN KEY ([OrganizationId]) 
        REFERENCES [Organizations]([OrganizationId]) ON DELETE CASCADE,
    CONSTRAINT [FK_Students_ModifiedBy] FOREIGN KEY ([ModifiedBy]) 
        REFERENCES [UserInfo]([UserId]),
    CONSTRAINT [UQ_Students_Identifier_Org] UNIQUE ([OrganizationId], [StudentIdentifier]),
    CONSTRAINT [CK_Students_Grade] CHECK ([Grade] >= 0 AND [Grade] <= 12)
)
GO

-- Indexes for Students
CREATE NONCLUSTERED INDEX [IX_Students_Organization] ON [dbo].[Students] ([OrganizationId])
CREATE NONCLUSTERED INDEX [IX_Students_Identifier] ON [dbo].[Students] ([StudentIdentifier])
CREATE NONCLUSTERED INDEX [IX_Students_Name] ON [dbo].[Students] ([LastName], [FirstName])
CREATE NONCLUSTERED INDEX [IX_Students_Grade] ON [dbo].[Students] ([Grade])
GO

PRINT '? Students table created'
PRINT ''

-- =============================================
-- Create Uniforms Table
-- =============================================

PRINT 'Creating Uniforms table...'
GO

CREATE TABLE [dbo].[Uniforms]
(
    [UniformId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [OrganizationId] INT NOT NULL,
    [UniformIdentifier] NVARCHAR(50) NOT NULL,
    [UniformType] INT NOT NULL, -- 0=ConcertCoat, 1=DrumMajorCoat, 2=Hat, 3=MarchingCoat, 4=MarchingShorts, 5=MarchingSocks, 6=Pants
    [Size] NVARCHAR(50) NOT NULL,
    [IsCheckedOut] BIT NOT NULL DEFAULT 0,
    [AssignedStudentId] NVARCHAR(50) NULL,
    [Conditions] NVARCHAR(MAX) NULL, -- Comma-separated list of condition enum values
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [LastModified] DATETIME NULL,
    [ModifiedBy] INT NULL,
    CONSTRAINT [FK_Uniforms_Organization] FOREIGN KEY ([OrganizationId]) 
        REFERENCES [Organizations]([OrganizationId]) ON DELETE CASCADE,
    CONSTRAINT [FK_Uniforms_ModifiedBy] FOREIGN KEY ([ModifiedBy]) 
        REFERENCES [UserInfo]([UserId]),
    CONSTRAINT [UQ_Uniforms_Identifier_Org] UNIQUE ([OrganizationId], [UniformIdentifier]),
    CONSTRAINT [CK_Uniforms_Type] CHECK ([UniformType] >= 0 AND [UniformType] <= 6)
)
GO

-- Indexes for Uniforms
CREATE NONCLUSTERED INDEX [IX_Uniforms_Organization] ON [dbo].[Uniforms] ([OrganizationId])
CREATE NONCLUSTERED INDEX [IX_Uniforms_Identifier] ON [dbo].[Uniforms] ([UniformIdentifier])
CREATE NONCLUSTERED INDEX [IX_Uniforms_CheckedOut] ON [dbo].[Uniforms] ([IsCheckedOut])
CREATE NONCLUSTERED INDEX [IX_Uniforms_Type] ON [dbo].[Uniforms] ([UniformType])
GO

PRINT '? Uniforms table created'
PRINT ''

-- =============================================
-- Create OrganizationJoinRequests Table
-- =============================================

PRINT 'Creating OrganizationJoinRequests table...'
GO

CREATE TABLE [dbo].[OrganizationJoinRequests]
(
    [RequestId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [OrganizationId] INT NOT NULL,
    [UserId] INT NOT NULL,
    [RequestedAccountLevel] INT NOT NULL, -- 0=Admin, 1=User, 2=Viewer
    [Status] INT NOT NULL DEFAULT 0, -- 0=Pending, 1=Approved, 2=Rejected
    [RequestMessage] NVARCHAR(500) NULL,
    [RequestedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [ReviewedBy] INT NULL, -- Admin who approved/rejected
    [ReviewedDate] DATETIME NULL,
    [ReviewNotes] NVARCHAR(500) NULL,
    CONSTRAINT [FK_JoinRequests_Organization] FOREIGN KEY ([OrganizationId]) 
        REFERENCES [Organizations]([OrganizationId]) ON DELETE CASCADE,
    CONSTRAINT [FK_JoinRequests_User] FOREIGN KEY ([UserId]) 
        REFERENCES [UserInfo]([UserId]),
    CONSTRAINT [FK_JoinRequests_ReviewedBy] FOREIGN KEY ([ReviewedBy]) 
        REFERENCES [UserInfo]([UserId]),
    CONSTRAINT [CK_JoinRequests_Status] CHECK ([Status] >= 0 AND [Status] <= 2),
    CONSTRAINT [CK_JoinRequests_AccountLevel] CHECK ([RequestedAccountLevel] >= 0 AND [RequestedAccountLevel] <= 2)
)
GO

-- Indexes for OrganizationJoinRequests
CREATE NONCLUSTERED INDEX [IX_JoinRequests_Organization_Status] 
    ON [dbo].[OrganizationJoinRequests] ([OrganizationId], [Status])
CREATE NONCLUSTERED INDEX [IX_JoinRequests_User] 
    ON [dbo].[OrganizationJoinRequests] ([UserId])
CREATE UNIQUE NONCLUSTERED INDEX [UQ_JoinRequests_User_Org_Pending]
    ON [dbo].[OrganizationJoinRequests] ([UserId], [OrganizationId])
    WHERE [Status] = 0 -- Only pending requests must be unique
GO

PRINT '? OrganizationJoinRequests table created'
PRINT ''

-- =============================================
-- Insert Sample/Test Data (Optional)
-- =============================================

PRINT '========================================='
PRINT 'Database Schema Created Successfully!'
PRINT '========================================='
PRINT ''
PRINT 'Tables created:'
PRINT '  1. ? UserInfo (User accounts)'
PRINT '  2. ? Organizations (Schools/Groups)'
PRINT '  3. ? UserOrganizations (User-Org membership with roles)'
PRINT '  4. ? Students (Organization members)'
PRINT '  5. ? Uniforms (Uniform inventory)'
PRINT '  6. ? OrganizationJoinRequests (Join requests pending approval)'
PRINT ''
PRINT 'Account Levels:'
PRINT '  0 = Administrator (Full access)'
PRINT '  1 = User (Read/Write)'
PRINT '  2 = Viewer (Read-only)'
PRINT ''
PRINT 'Uniform Types:'
PRINT '  0=ConcertCoat, 1=DrumMajorCoat, 2=Hat, 3=MarchingCoat'
PRINT '  4=MarchingShorts, 5=MarchingSocks, 6=Pants'
PRINT ''
PRINT 'Next Steps:'
PRINT '  1. Run your Azure Function API'
PRINT '  2. Create a user account via CreateAccount endpoint'
PRINT '  3. Create organizations and add data'
PRINT ''
PRINT '========================================='
PRINT 'Ready to use! ?'
PRINT '========================================='
GO
