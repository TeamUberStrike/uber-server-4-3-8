USE [CmuneMonitoring]

ALTER TABLE [dbo].[ManagedServers] ALTER COLUMN [ServerName] NVARCHAR(50) NULL
GO

ALTER TABLE [dbo].[ManagedServers] ADD [Role] NVARCHAR(250) NULL
GO

ALTER TABLE [dbo].[ManagedServers] DROP COLUMN [ServerIP]
GO

ALTER TABLE [dbo].[ManagedServers] ADD [PublicIP] NVARCHAR(30) NOT NULL
GO

ALTER TABLE [dbo].[ManagedServers] ADD [PrivateIP] NVARCHAR(30) NULL
GO

ALTER TABLE [dbo].[ManagedServers] ADD [ServerIDC] NVARCHAR(50) NOT NULL
GO

ALTER TABLE [dbo].[ManagedServers] ADD [Region] INT NOT NULL
GO

ALTER TABLE [dbo].[ManagedServers] ADD [City] NVARCHAR(100) NULL
GO

ALTER TABLE [dbo].[ManagedServers] ADD [CPUModel] NVARCHAR(50) NOT NULL
GO

ALTER TABLE [dbo].[ManagedServers] ADD [CPUSpeed] DECIMAL(18,2) NOT NULL
GO

ALTER TABLE [dbo].[ManagedServers] ADD [CPUs] INT NOT NULL
GO

ALTER TABLE [dbo].[ManagedServers] ADD [CPUCore] SMALLINT NOT NULL
GO

ALTER TABLE [dbo].[ManagedServers] ADD [DiskSpace] SMALLINT NULL
GO

ALTER TABLE [dbo].[ManagedServers] ADD [RAM] INT NOT NULL
GO

ALTER TABLE [dbo].[ManagedServers] ADD [AllowedBandwidth] INT NULL
GO

ALTER TABLE [dbo].[ManagedServers] ADD [Note] NVARCHAR(500) NULL
GO

ALTER TABLE [dbo].[ManagedServers] ADD [DeploymentTime] DATETIME NULL
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ManagedServerMonitoring_ManagedServers]') AND parent_object_id = OBJECT_ID(N'[dbo].[ManagedServerMonitoring]'))
ALTER TABLE [dbo].[ManagedServerMonitoring] DROP CONSTRAINT [FK_ManagedServerMonitoring_ManagedServers]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ManagedServerMonitoring]') AND type in (N'U'))
DROP TABLE [dbo].[ManagedServerMonitoring]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ManagedServerMonitoring](
	[ManagedServerMonitoringId] [int] IDENTITY(1,1) NOT NULL,
	[CPUUsage] [int] NOT NULL,
	[RamUsage] [int] NOT NULL,
	[ReportTime] [datetime] NOT NULL,
	[BandwidthUsage] [decimal](18, 2) NOT NULL,
	[DiskUsage] [decimal](18, 2) NOT NULL,
	[ManagedServerId] [int] NOT NULL,
 CONSTRAINT [PK_ManagedServerMonitoring] PRIMARY KEY CLUSTERED 
(
	[ManagedServerMonitoringId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[ManagedServerMonitoring]  WITH CHECK ADD  CONSTRAINT [FK_ManagedServerMonitoring_ManagedServers] FOREIGN KEY([ManagedServerId])
REFERENCES [dbo].[ManagedServers] ([ManagedServerID])
GO

ALTER TABLE [dbo].[ManagedServerMonitoring] CHECK CONSTRAINT [FK_ManagedServerMonitoring_ManagedServers]
GO