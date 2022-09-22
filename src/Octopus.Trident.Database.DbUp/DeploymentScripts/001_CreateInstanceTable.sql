CREATE TABLE [dbo].[Instance](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OctopusId] [nvarchar](256) NOT NULL,
	[Name] [nvarchar](256) NOT NULL,
	[Url] [nvarchar](1024) NOT NULL,
	[ApiKey] [nvarchar](256) NOT NULL,
 CONSTRAINT [PK_Instance] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
) 

