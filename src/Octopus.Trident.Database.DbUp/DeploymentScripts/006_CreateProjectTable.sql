CREATE TABLE [dbo].[Project](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OctopusId] [nvarchar](256) NOT NULL,
	[SpaceId] [int] NOT NULL,
	[Name] [nvarchar](256) NOT NULL,
 CONSTRAINT [PK_Project] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)
GO

ALTER TABLE [dbo].[Project]  WITH CHECK ADD  CONSTRAINT [FK_Project_Space] FOREIGN KEY([SpaceId])
REFERENCES [dbo].[Space] ([Id])
GO

ALTER TABLE [dbo].[Project] CHECK CONSTRAINT [FK_Project_Space]
GO

