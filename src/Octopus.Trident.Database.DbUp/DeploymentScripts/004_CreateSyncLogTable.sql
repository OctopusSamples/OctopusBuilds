CREATE TABLE [dbo].[SyncLog](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SyncId] [int] NOT NULL,
	[Created] [datetime2](7) NOT NULL,
	[Type] [nvarchar](256) NOT NULL,
	[Message] [nvarchar](2048) NOT NULL,
 CONSTRAINT [PK_SyncLog] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)
GO

ALTER TABLE [dbo].[SyncLog]  WITH CHECK ADD  CONSTRAINT [FK_SyncLog_Sync] FOREIGN KEY([SyncId])
REFERENCES [dbo].[Sync] ([Id])
GO

ALTER TABLE [dbo].[SyncLog] CHECK CONSTRAINT [FK_SyncLog_Sync]
GO

