-- =============================================
-- Add OrganizationId to Students Table
-- Migration Script - Run this to update existing Students table
-- =============================================

USE [DatabasestorageIA]
GO

PRINT '========================================='
PRINT 'Updating Students Table for Organizations'
PRINT '========================================='
PRINT ''

-- =============================================
-- Add OrganizationId column if it doesn't exist
-- =============================================
PRINT 'Checking Students table structure...'
GO

IF NOT EXISTS (
    SELECT * FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'[dbo].[Students]') 
    AND name = 'OrganizationId'
)
BEGIN
    PRINT 'Adding OrganizationId column to Students table...'
    
    -- Add the column (nullable first to avoid issues with existing data)
    ALTER TABLE [dbo].[Students]
    ADD [OrganizationId] INT NULL
    
    PRINT '? OrganizationId column added'
    
    -- Update existing rows to use first organization if any exist
    -- (This assigns existing students to the first organization found)
    IF EXISTS (SELECT * FROM [dbo].[Organizations])
    BEGIN
        DECLARE @FirstOrgId INT
        SELECT TOP 1 @FirstOrgId = OrganizationId 
        FROM [dbo].[Organizations] 
        ORDER BY OrganizationId
        
        UPDATE [dbo].[Students]
        SET OrganizationId = @FirstOrgId
        WHERE OrganizationId IS NULL
        
        PRINT '? Existing students assigned to organization ' + CAST(@FirstOrgId AS NVARCHAR(10))
    END
    
    -- Now make it NOT NULL
    ALTER TABLE [dbo].[Students]
    ALTER COLUMN [OrganizationId] INT NOT NULL
    
    PRINT '? OrganizationId set to NOT NULL'
    
    -- Add foreign key constraint
    ALTER TABLE [dbo].[Students]
    ADD CONSTRAINT [FK_Students_Organization] 
        FOREIGN KEY ([OrganizationId]) 
        REFERENCES [Organizations]([OrganizationId]) 
        ON DELETE CASCADE
    
    PRINT '? Foreign key constraint added'
    
    -- Add index for performance
    CREATE NONCLUSTERED INDEX [IX_Students_OrganizationId]
    ON [dbo].[Students] ([OrganizationId])
    
    PRINT '? Index created on OrganizationId'
END
ELSE
BEGIN
    PRINT '? OrganizationId column already exists'
END
GO

-- =============================================
-- Display Summary
-- =============================================
PRINT ''
PRINT '========================================='
PRINT 'Students Table Structure'
PRINT '========================================='
GO

SELECT 
    c.name AS [Column Name],
    t.name AS [Data Type],
    c.max_length AS [Max Length],
    c.is_nullable AS [Nullable]
FROM sys.columns c
INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
WHERE c.object_id = OBJECT_ID(N'[dbo].[Students]')
ORDER BY c.column_id
GO

PRINT ''
PRINT '========================================='
PRINT 'Migration Complete! ?'
PRINT '========================================='
PRINT ''
PRINT 'Students table now supports:'
PRINT '  • Multiple organizations'
PRINT '  • Proper foreign key relationships'
PRINT '  • Cascading deletes'
PRINT ''
PRINT 'You can now add students to any organization!'
PRINT '========================================='
GO
