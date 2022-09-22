CREATE TABLE [dbo].[Release](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OctopusId] [nvarchar](256) NOT NULL,
	[ProjectId] [int] NOT NULL,
	[Version] [nvarchar](256) NOT NULL,
	[Created] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_Release] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)
GO

ALTER TABLE [dbo].[Release]  WITH CHECK ADD  CONSTRAINT [FK_Release_Project] FOREIGN KEY([ProjectId])
REFERENCES [dbo].[Project] ([Id])
GO

ALTER TABLE [dbo].[Release] CHECK CONSTRAINT [FK_Release_Project]
GO

