CREATE TABLE [dbo].[Messages]
(
	[Id] UNIQUEIDENTIFIER NOT NULL,
	[PublisherId] UNIQUEIDENTIFIER NOT NULL,
	[PublisherHandle] NVARCHAR(100) NOT NULL,
	[PublisherName] NVARCHAR(100) NOT NULL,
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

CREATE NONCLUSTERED INDEX [UK_Messages_PublisherId]
ON [dbo].[Messages]
(
	[PublisherId],
	[Timestamp]
)
GO