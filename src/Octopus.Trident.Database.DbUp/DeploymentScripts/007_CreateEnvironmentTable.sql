CREATE TABLE [dbo].[Environment](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OctopusId] [nvarchar](256) NOT NULL,
	[SpaceId] [int] NOT NULL,
	[Name] [nvarchar](256) NOT NULL,
 CONSTRAINT [PK_Environment] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)
GO

ALTER TABLE [dbo].[Environment]  WITH CHECK ADD  CONSTRAINT [FK_Environment_Space] FOREIGN KEY([SpaceId])
REFERENCES [dbo].[Space] ([Id])
GO

ALTER TABLE [dbo].[Environment] CHECK CONSTRAINT [FK_Environment_Space]
GO

