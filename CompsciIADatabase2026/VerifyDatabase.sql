-- =============================================
-- Verify Database is Ready
-- Run this to check if your database is accessible
-- =============================================

USE [master]
GO

PRINT '========================================='
PRINT 'Database Verification'
PRINT '========================================='
PRINT ''

-- Check if database exists
IF EXISTS (SELECT * FROM sys.databases WHERE name = 'DatabasestorageIA')
BEGIN
    PRINT '? Database "DatabasestorageIA" exists'
    
    USE [DatabasestorageIA]
    
    PRINT ''
    PRINT 'Tables in database:'
    SELECT 
        TABLE_NAME as [Table Name],
        (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = t.TABLE_NAME) as [Column Count]
    FROM INFORMATION_SCHEMA.TABLES t
    WHERE TABLE_TYPE = 'BASE TABLE'
    ORDER BY TABLE_NAME
    
    PRINT ''
    PRINT 'Row counts:'
    SELECT 'UserInfo' as [Table], COUNT(*) as [Rows] FROM [dbo].[UserInfo]
    UNION ALL
    SELECT 'Organizations', COUNT(*) FROM [dbo].[Organizations]
    UNION ALL
    SELECT 'UserOrganizations', COUNT(*) FROM [dbo].[UserOrganizations]
    UNION ALL
    SELECT 'Students', COUNT(*) FROM [dbo].[Students]
    UNION ALL
    SELECT 'Uniforms', COUNT(*) FROM [dbo].[Uniforms]
    UNION ALL
    SELECT 'OrganizationJoinRequests', COUNT(*) FROM [dbo].[OrganizationJoinRequests]
    
    PRINT ''
    PRINT '? Database is accessible and ready!'
END
ELSE
BEGIN
    PRINT '? Database "DatabasestorageIA" does NOT exist!'
    PRINT ''
    PRINT 'Available databases:'
    SELECT name FROM sys.databases WHERE name NOT IN ('master', 'tempdb', 'model', 'msdb')
END
GO

PRINT ''
PRINT '========================================='
