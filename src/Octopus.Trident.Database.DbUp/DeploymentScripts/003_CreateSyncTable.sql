CREATE TABLE [dbo].[Sync](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[InstanceId] [int] NOT NULL,
	[State] [nvarchar](256) NOT NULL,
	[Name] [nvarchar](1024) NOT NULL,
	[Created] [datetime2](7) NOT NULL,
	[Started] [datetime2](7) NULL,
	[Completed] [datetime2](7) NULL,
	[SearchStateDate] [datetime2](7) NULL,
	[RetryAttempts] [int] NULL,
 CONSTRAINT [PK_Sync] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)
GO

ALTER TABLE [dbo].[Sync]  WITH CHECK ADD  CONSTRAINT [FK_Sync_Instance] FOREIGN KEY([InstanceId])
REFERENCES [dbo].[Instance] ([Id])
GO

ALTER TABLE [dbo].[Sync] CHECK CONSTRAINT [FK_Sync_Instance]
GO

