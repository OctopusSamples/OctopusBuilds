CREATE TABLE [dbo].[Deployment](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OctopusId] [nvarchar](256) NOT NULL,
	[ReleaseId] [int] NOT NULL,
	[EnvironmentId] [int] NOT NULL,
	[TenantId] [int] NULL,
	[QueueTime] [datetime2](7) NOT NULL,
	[StartTime] [datetime2](7) NULL,
	[CompletedTime] [datetime2](7) NULL,
	[DeploymentState] [nvarchar](256) NOT NULL,
 CONSTRAINT [PK_Deployment] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)
GO

ALTER TABLE [dbo].[Deployment]  WITH CHECK ADD  CONSTRAINT [FK_Deployment_Environment] FOREIGN KEY([EnvironmentId])
REFERENCES [dbo].[Environment] ([Id])
GO

ALTER TABLE [dbo].[Deployment] CHECK CONSTRAINT [FK_Deployment_Environment]
GO

ALTER TABLE [dbo].[Deployment]  WITH CHECK ADD  CONSTRAINT [FK_Deployment_Release] FOREIGN KEY([ReleaseId])
REFERENCES [dbo].[Release] ([Id])
GO

ALTER TABLE [dbo].[Deployment] CHECK CONSTRAINT [FK_Deployment_Release]
GO

ALTER TABLE [dbo].[Deployment]  WITH CHECK ADD  CONSTRAINT [FK_Deployment_Tenant] FOREIGN KEY([TenantId])
REFERENCES [dbo].[Tenant] ([Id])
GO

ALTER TABLE [dbo].[Deployment] CHECK CONSTRAINT [FK_Deployment_Tenant]
GO

