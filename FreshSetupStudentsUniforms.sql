-- =============================================
-- Fresh Setup - Drop and Recreate Students/Uniforms with Org Support
-- WARNING: This will delete all existing students and uniforms data
-- Run only if you don't need to keep existing data
-- =============================================

USE [DatabasestorageIA]
GO

PRINT '========================================='
PRINT 'Fresh Setup - Students and Uniforms'
PRINT '========================================='
PRINT ''
PRINT 'WARNING: This will delete all existing data!'
PRINT ''

-- =============================================
-- 1. Drop existing Students table
-- =============================================
PRINT 'Dropping Students table...'
GO

IF OBJECT_ID('dbo.Students', 'U') IS NOT NULL
BEGIN
    DROP TABLE [dbo].[Students]
    PRINT '? Students table dropped'
END
ELSE
BEGIN
    PRINT '? Students table does not exist'
END
GO

-- =============================================
-- 2. Drop existing Uniforms table
-- =============================================
PRINT 'Dropping Uniforms table...'
GO

IF OBJECT_ID('dbo.Uniforms', 'U') IS NOT NULL
BEGIN
    DROP TABLE [dbo].[Uniforms]
    PRINT '? Uniforms table dropped'
END
ELSE
BEGIN
    PRINT '? Uniforms table does not exist'
END
GO

-- =============================================
-- 3. Create Students table with OrganizationId
-- =============================================
PRINT ''
PRINT 'Creating Students table with organization support...'
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
    CONSTRAINT [CK_Students_Grade] CHECK ([Grade] >= 1 AND [Grade] <= 12),
    CONSTRAINT [UQ_Students_OrgIdentifier] UNIQUE ([OrganizationId], [StudentIdentifier])
)

CREATE NONCLUSTERED INDEX [IX_Students_Organization]
ON [dbo].[Students] ([OrganizationId])

CREATE NONCLUSTERED INDEX [IX_Students_Grade]
ON [dbo].[Students] ([Grade])

CREATE NONCLUSTERED INDEX [IX_Students_LastName]
ON [dbo].[Students] ([LastName])

PRINT '? Students table created successfully'
GO

-- =============================================
-- 4. Create Uniforms table with OrganizationId
-- =============================================
PRINT 'Creating Uniforms table with organization support...'
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
    CONSTRAINT [FK_Uniforms_Organization] FOREIGN KEY ([OrganizationId]) 
        REFERENCES [Organizations]([OrganizationId]) ON DELETE CASCADE,
    CONSTRAINT [FK_Uniforms_ModifiedBy] FOREIGN KEY ([ModifiedBy]) 
        REFERENCES [UserInfo]([UserId]),
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

PRINT '? Uniforms table created successfully'
GO

-- =============================================
-- 5. Display Summary
-- =============================================
PRINT ''
PRINT '========================================='
PRINT 'Fresh Setup Complete! ?'
PRINT '========================================='
PRINT ''
PRINT 'Tables created:'
PRINT '  ? Students (with OrganizationId)'
PRINT '  ? Uniforms (with OrganizationId)'
PRINT ''
PRINT 'Next steps:'
PRINT '  1. Restart the Azure Function'
PRINT '  2. Login to WinForms app'
PRINT '  3. Create your first organization'
PRINT '  4. Add students and uniforms to it'
PRINT '========================================='
GO
