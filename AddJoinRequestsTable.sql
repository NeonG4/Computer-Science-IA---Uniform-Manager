-- =============================================
-- Add Organization Join Requests Table
-- Allows users to request to join an org, pending admin approval
-- =============================================

USE [DatabasestorageIA]
GO

PRINT '========================================='
PRINT 'Adding Organization Join Requests Feature'
PRINT '========================================='
PRINT ''

-- =============================================
-- Create OrganizationJoinRequests Table
-- =============================================
PRINT 'Creating OrganizationJoinRequests table...'
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'OrganizationJoinRequests' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
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
    
    -- Index for finding pending requests by organization
    CREATE NONCLUSTERED INDEX [IX_JoinRequests_Organization_Status]
    ON [dbo].[OrganizationJoinRequests] ([OrganizationId], [Status])
    
    -- Index for finding requests by user
    CREATE NONCLUSTERED INDEX [IX_JoinRequests_User]
    ON [dbo].[OrganizationJoinRequests] ([UserId])
    
    -- Unique constraint: one pending request per user per org
    CREATE UNIQUE NONCLUSTERED INDEX [UQ_JoinRequests_User_Org_Pending]
    ON [dbo].[OrganizationJoinRequests] ([UserId], [OrganizationId])
    WHERE [Status] = 0 -- Only pending requests must be unique
    
    PRINT '? OrganizationJoinRequests table created successfully'
END
ELSE
BEGIN
    PRINT '? OrganizationJoinRequests table already exists'
END
GO

PRINT ''
PRINT '========================================='
PRINT 'Setup Complete! ?'
PRINT '========================================='
PRINT ''
PRINT 'New table created:'
PRINT '  ? OrganizationJoinRequests'
PRINT ''
PRINT 'Features:'
PRINT '  • Users can request to join organizations'
PRINT '  • Requests are pending until approved by admin'
PRINT '  • One pending request per user per organization'
PRINT '  • Admins can approve or reject requests'
PRINT '  • Track who reviewed and when'
PRINT '========================================='
GO
