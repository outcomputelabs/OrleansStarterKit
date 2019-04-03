CREATE TABLE [dbo].[ChannelUsers]
(
	[ChannelId] UNIQUEIDENTIFIER NOT NULL,
	[UserId] UNIQUEIDENTIFIER NOT NULL
)
GO

CREATE UNIQUE NONCLUSTERED INDEX [UK_ChannelUsers_Index]
ON [dbo].[ChannelUsers]
(
	[ChannelId],
	[UserId]
)
GO
