-- Migration Script: Add AccountLevel to existing UserInfo table
-- Run this script on your existing database to add the AccountLevel column

USE [DatabasestorageIA]
GO

-- Step 1: Add AccountLevel column if it doesn't exist
IF NOT EXISTS (
    SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'UserInfo' 
    AND TABLE_SCHEMA = 'dbo'
    AND COLUMN_NAME = 'AccountLevel'
)
BEGIN
    ALTER TABLE [dbo].[UserInfo]
    ADD [AccountLevel] INT NOT NULL DEFAULT 2 -- Default to Viewer (2)
    
    PRINT 'AccountLevel column added successfully.'
    
    -- Add check constraint
    ALTER TABLE [dbo].[UserInfo]
    ADD CONSTRAINT [CK_UserInfo_AccountLevel] CHECK ([AccountLevel] >= 0 AND [AccountLevel] <= 2)
    
    PRINT 'Check constraint added.'
END
ELSE
BEGIN
    PRINT 'AccountLevel column already exists.'
END
GO

-- Step 2: Add Username column if it doesn't exist
IF NOT EXISTS (
    SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'UserInfo' 
    AND TABLE_SCHEMA = 'dbo'
    AND COLUMN_NAME = 'Username'
)
BEGIN
    -- Add as nullable first
    ALTER TABLE [dbo].[UserInfo]
    ADD [Username] NVARCHAR(50) NULL
    
    PRINT 'Username column added.'
    
    -- Populate with default values (email prefix)
    UPDATE [dbo].[UserInfo]
    SET [Username] = LEFT([Email], CHARINDEX('@', [Email]) - 1)
    WHERE [Username] IS NULL
    
    -- Make it NOT NULL
    ALTER TABLE [dbo].[UserInfo]
    ALTER COLUMN [Username] NVARCHAR(50) NOT NULL
    
    -- Add unique constraint
    ALTER TABLE [dbo].[UserInfo]
    ADD CONSTRAINT [UQ_UserInfo_Username] UNIQUE ([Username])
    
    PRINT 'Username column configured with unique constraint.'
END
ELSE
BEGIN
    PRINT 'Username column already exists.'
END
GO

-- Step 3: Update existing users to have appropriate account levels
-- You can customize this based on your needs
-- For example, make the first user an admin:

-- DECLARE @FirstUserId INT
-- SELECT TOP 1 @FirstUserId = UserId FROM [dbo].[UserInfo] ORDER BY UserId
-- UPDATE [dbo].[UserInfo] SET [AccountLevel] = 0 WHERE UserId = @FirstUserId
-- PRINT 'First user promoted to Administrator.'

-- Step 4: Create index for better performance
IF NOT EXISTS (
    SELECT * FROM sys.indexes 
    WHERE name = 'IX_UserInfo_AccountLevel' 
    AND object_id = OBJECT_ID('[dbo].[UserInfo]')
)
BEGIN
    CREATE NONCLUSTERED INDEX [IX_UserInfo_AccountLevel]
    ON [dbo].[UserInfo] ([AccountLevel])
    
    PRINT 'Index created on AccountLevel column.'
END
ELSE
BEGIN
    PRINT 'Index already exists on AccountLevel.'
END
GO

-- Step 5: Display summary
SELECT 
    'UserInfo Table Summary' as [Info],
    COUNT(*) as [TotalUsers],
    SUM(CASE WHEN AccountLevel = 0 THEN 1 ELSE 0 END) as [Administrators],
    SUM(CASE WHEN AccountLevel = 1 THEN 1 ELSE 0 END) as [Users],
    SUM(CASE WHEN AccountLevel = 2 THEN 1 ELSE 0 END) as [Viewers]
FROM [dbo].[UserInfo]
GO

PRINT 'Migration completed successfully!'
