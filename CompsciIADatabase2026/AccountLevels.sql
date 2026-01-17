-- Account Levels Lookup Table (Optional Reference Table)
CREATE TABLE [dbo].[AccountLevels]
(
    [LevelId] INT NOT NULL PRIMARY KEY,
    [LevelName] NVARCHAR(20) NOT NULL,
    [Description] NVARCHAR(100) NULL
)
GO

-- Insert account level definitions
INSERT INTO [dbo].[AccountLevels] ([LevelId], [LevelName], [Description])
VALUES 
    (0, 'Administrator', 'Full access to all features and settings'),
    (1, 'User', 'Standard user with limited administrative rights'),
    (2, 'Viewer', 'Read-only access to the system')
GO
