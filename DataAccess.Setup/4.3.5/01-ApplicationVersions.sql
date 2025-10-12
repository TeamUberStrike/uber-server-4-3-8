USE [MvParadisePaintball]

BEGIN TRANSACTION

SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO

CREATE TABLE [MvParadisePaintball].[dbo].[ApplicationVersionsTmp]
(
	[ApplicationVersionId] INT NOT NULL IDENTITY (1, 1),
	[Version] [nvarchar](20) NOT NULL,
	[WebPlayerFileName] [nvarchar](250) NULL,
	[Channel] [int] NOT NULL,
	[ModificationDate] [datetime] NOT NULL,
	[IsEnabled] [bit] NOT NULL,
	[WarnPlayer] [bit] NOT NULL,
	[PhotonClusterId] [int] NOT NULL,
 CONSTRAINT [PK_ApplicationVersions] PRIMARY KEY CLUSTERED 
(
	[ApplicationVersionId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET IDENTITY_INSERT [MvParadisePaintball].[dbo].[ApplicationVersionsTmp] ON
GO
IF EXISTS(SELECT * FROM [MvParadisePaintball].[dbo].[ApplicationVersions])
EXEC('INSERT INTO [MvParadisePaintball].[dbo].[ApplicationVersionsTmp] ([ApplicationVersionId], [Version], [WebPlayerFileName], [Channel], [ModificationDate], [IsEnabled], [WarnPlayer], [PhotonClusterId])
SELECT [ApplicationVersionId], [Version], [WebPlayerFileName], [Channel], [ModificationDate], [IsEnabled], [WarnPlayer], [PhotonClusterId] FROM [MvParadisePaintball].[dbo].[ApplicationVersions] WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT [MvParadisePaintball].[dbo].[ApplicationVersionsTmp] OFF
GO
DROP TABLE [MvParadisePaintball].[dbo].[ApplicationVersions]
GO
EXECUTE sp_rename N'dbo.ApplicationVersionsTmp', N'ApplicationVersions', 'OBJECT'
GO
COMMIT

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ApplicationVersions_PhotonsGroups]') AND parent_object_id = OBJECT_ID(N'[dbo].[ApplicationVersions]'))
ALTER TABLE [dbo].[ApplicationVersions] DROP CONSTRAINT [FK_ApplicationVersions_PhotonsGroups]
GO

ALTER TABLE [dbo].[ApplicationVersions]  WITH CHECK ADD  CONSTRAINT [FK_ApplicationVersions_PhotonsGroups] FOREIGN KEY([PhotonClusterId])
REFERENCES [dbo].[PhotonsGroups] ([PhotonsGroupID])
GO

ALTER TABLE [dbo].[ApplicationVersions] CHECK CONSTRAINT [FK_ApplicationVersions_PhotonsGroups]
GO