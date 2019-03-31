CREATE TABLE [dbo].[Channels]
(
	[Id] UNIQUEIDENTIFIER NOT NULL,
	[Handle] NVARCHAR(100) NOT NULL,
	[Description] NVARCHAR(1000) NOT NULL
)
GO

CREATE UNIQUE NONCLUSTERED INDEX [UK_Channels_Id]
ON [dbo].[Channels]
(
	[Id]
)
GO

CREATE UNIQUE NONCLUSTERED INDEX [UK_Channels_Handle]
ON [dbo].[Channels]
(
	[Handle]
)
GO