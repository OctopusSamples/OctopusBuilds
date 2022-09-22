CREATE TABLE [dbo].[Space](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OctopusId] [nvarchar](256) NOT NULL,
	[InstanceId] [int] NOT NULL,
	[Name] [nvarchar](256) NOT NULL,
 CONSTRAINT [PK_Space] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)
GO

ALTER TABLE [dbo].[Space]  WITH CHECK ADD  CONSTRAINT [FK_Space_Instance] FOREIGN KEY([InstanceId])
REFERENCES [dbo].[Instance] ([Id])
GO

ALTER TABLE [dbo].[Space] CHECK CONSTRAINT [FK_Space_Instance]
GO

