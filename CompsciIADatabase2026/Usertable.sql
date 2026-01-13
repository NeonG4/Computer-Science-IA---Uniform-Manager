CREATE TABLE [dbo].[Usertable]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [FirstName] NCHAR(10) NOT NULL, 
    [LastName] NCHAR(10) NOT NULL, 
    [Email] CHAR(50) NOT NULL,
    [HashedPassword] CHAR(255) NOT NULL,
    [AssignedClothing] JSON NULL
)
