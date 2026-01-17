-- UserInfo Table with Account Levels
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
GO

CREATE NONCLUSTERED INDEX [IX_UserInfo_AccountLevel]
ON [dbo].[UserInfo] ([AccountLevel])
GO
