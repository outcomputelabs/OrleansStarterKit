CREATE TABLE [dbo].[Messages]
(
	[Id] UNIQUEIDENTIFIER NOT NULL,
	[SenderId] UNIQUEIDENTIFIER NOT NULL,
	[SenderHandle] NVARCHAR(100) NOT NULL,
	[SenderName] NVARCHAR(100) NOT NULL,
	[ReceiverId] UNIQUEIDENTIFIER NOT NULL,
	[Content] NVARCHAR(1000) NOT NULL,
	[Timestamp] DATETIME2(2) NOT NULL
)
GO

CREATE UNIQUE NONCLUSTERED INDEX [UK_Messages_Id]
ON [dbo].[Messages]
(
	[Id]
)
GO

CREATE NONCLUSTERED INDEX [UK_Messages_SenderId]
ON [dbo].[Messages]
(
	[SenderId],
	[Timestamp] DESC
)
GO

CREATE NONCLUSTERED INDEX [UK_Messages_ReceiverId]
ON [dbo].[Messages]
(
	[ReceiverId],
	[Timestamp] DESC
)
GO
