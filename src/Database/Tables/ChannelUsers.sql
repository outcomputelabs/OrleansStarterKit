CREATE TABLE [dbo].[ChannelUsers]
(
	[ChannelId] UNIQUEIDENTIFIER NOT NULL,
	[UserId] UNIQUEIDENTIFIER NOT NULL
)
GO

CREATE UNIQUE NONCLUSTERED INDEX [UK_ChannelUsers_ByChannelId]
ON [dbo].[ChannelUsers]
(
	[ChannelId],
	[UserId]
)
GO

CREATE UNIQUE NONCLUSTERED INDEX [UK_ChannelUsers_ByUserId]
ON [dbo].[ChannelUsers]
(
	[UserId],
	[ChannelId]
)
GO