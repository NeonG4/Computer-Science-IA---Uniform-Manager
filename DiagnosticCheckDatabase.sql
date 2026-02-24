-- =============================================
-- Diagnostic Script - Check Multi-Org Tables
-- Run this to verify your database is set up correctly
-- =============================================

USE [DatabasestorageIA]
GO

PRINT '========================================='
PRINT 'Database Diagnostic Check'
PRINT '========================================='
PRINT ''

-- Check if database exists
IF DB_ID('DatabasestorageIA') IS NOT NULL
BEGIN
    PRINT '? Database "DatabasestorageIA" exists'
END
ELSE
BEGIN
    PRINT '? Database "DatabasestorageIA" does NOT exist!'
    PRINT '  Action: Create the database first'
END
PRINT ''

-- Check for required tables
PRINT 'Checking for required tables...'
PRINT ''

-- UserInfo
IF OBJECT_ID('dbo.UserInfo', 'U') IS NOT NULL
    PRINT '? UserInfo table exists'
ELSE
    PRINT '? UserInfo table is MISSING!'

-- Organizations
IF OBJECT_ID('dbo.Organizations', 'U') IS NOT NULL
    PRINT '? Organizations table exists'
ELSE
    PRINT '? Organizations table is MISSING! (Required for multi-org feature)'

-- UserOrganizations
IF OBJECT_ID('dbo.UserOrganizations', 'U') IS NOT NULL
    PRINT '? UserOrganizations table exists'
ELSE
    PRINT '? UserOrganizations table is MISSING! (Required for multi-org feature)'

-- Students
IF OBJECT_ID('dbo.Students', 'U') IS NOT NULL
BEGIN
    PRINT '? Students table exists'
    
    -- Check if OrganizationId column exists
    IF COL_LENGTH('dbo.Students', 'OrganizationId') IS NOT NULL
        PRINT '  ? Students.OrganizationId column exists'
    ELSE
        PRINT '  ? Students.OrganizationId column is MISSING! (Need to run migration)'
END
ELSE
    PRINT '? Students table is MISSING!'

-- Uniforms
IF OBJECT_ID('dbo.Uniforms', 'U') IS NOT NULL
BEGIN
    PRINT '? Uniforms table exists'
    
    -- Check if OrganizationId column exists
    IF COL_LENGTH('dbo.Uniforms', 'OrganizationId') IS NOT NULL
        PRINT '  ? Uniforms.OrganizationId column exists'
    ELSE
        PRINT '  ? Uniforms.OrganizationId column is MISSING! (Need to run migration)'
END
ELSE
    PRINT '? Uniforms table is MISSING!'

PRINT ''
PRINT '========================================='
PRINT 'Data Summary'
PRINT '========================================='
PRINT ''

-- Count records in each table
IF OBJECT_ID('dbo.UserInfo', 'U') IS NOT NULL
BEGIN
    DECLARE @UserCount INT
    SELECT @UserCount = COUNT(*) FROM dbo.UserInfo
    PRINT 'UserInfo: ' + CAST(@UserCount AS VARCHAR(10)) + ' users'
END

IF OBJECT_ID('dbo.Organizations', 'U') IS NOT NULL
BEGIN
    DECLARE @OrgCount INT
    SELECT @OrgCount = COUNT(*) FROM dbo.Organizations
    PRINT 'Organizations: ' + CAST(@OrgCount AS VARCHAR(10)) + ' organizations'
END

IF OBJECT_ID('dbo.UserOrganizations', 'U') IS NOT NULL
BEGIN
    DECLARE @UserOrgCount INT
    SELECT @UserOrgCount = COUNT(*) FROM dbo.UserOrganizations
    PRINT 'UserOrganizations: ' + CAST(@UserOrgCount AS VARCHAR(10)) + ' user-org relationships'
END

IF OBJECT_ID('dbo.Students', 'U') IS NOT NULL
BEGIN
    DECLARE @StudentCount INT
    SELECT @StudentCount = COUNT(*) FROM dbo.Students
    PRINT 'Students: ' + CAST(@StudentCount AS VARCHAR(10)) + ' students'
END

IF OBJECT_ID('dbo.Uniforms', 'U') IS NOT NULL
BEGIN
    DECLARE @UniformCount INT
    SELECT @UniformCount = COUNT(*) FROM dbo.Uniforms
    PRINT 'Uniforms: ' + CAST(@UniformCount AS VARCHAR(10)) + ' uniforms'
END

PRINT ''
PRINT '========================================='
PRINT 'Diagnosis Complete'
PRINT '========================================='
PRINT ''

-- Provide recommendations
PRINT 'Recommendations:'
PRINT ''

IF OBJECT_ID('dbo.Organizations', 'U') IS NULL OR OBJECT_ID('dbo.UserOrganizations', 'U') IS NULL
BEGIN
    PRINT '? CRITICAL: Multi-organization tables are missing!'
    PRINT '  ? Run SetupLocalDatabaseWithOrgs.sql to create them'
    PRINT ''
END
ELSE
BEGIN
    DECLARE @OrgExists INT
    SELECT @OrgExists = COUNT(*) FROM dbo.Organizations
    
    IF @OrgExists = 0
    BEGIN
        PRINT '? No organizations exist yet'
        PRINT '  ? Create your first organization through the WinForms app'
        PRINT ''
    END
    ELSE
    BEGIN
        PRINT '? Database appears to be set up correctly!'
        PRINT ''
        PRINT 'Organizations in database:'
        SELECT OrganizationId, OrganizationName, OrganizationCode, IsActive
        FROM dbo.Organizations
        ORDER BY OrganizationName
        PRINT ''
    END
END

IF OBJECT_ID('dbo.Students', 'U') IS NOT NULL AND COL_LENGTH('dbo.Students', 'OrganizationId') IS NULL
BEGIN
    PRINT '? CRITICAL: Students table is missing OrganizationId column!'
    PRINT '  ? Run SetupLocalDatabaseWithOrgs.sql to add it'
    PRINT ''
END

IF OBJECT_ID('dbo.Uniforms', 'U') IS NOT NULL AND COL_LENGTH('dbo.Uniforms', 'OrganizationId') IS NULL
BEGIN
    PRINT '? CRITICAL: Uniforms table is missing OrganizationId column!'
    PRINT '  ? Run SetupLocalDatabaseWithOrgs.sql to add it'
    PRINT ''
END

GO
