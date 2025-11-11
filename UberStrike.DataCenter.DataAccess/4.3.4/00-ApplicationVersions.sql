USE [MvParadisePaintball]
GO

EXEC sp_rename '[dbo].[ApplicationVersions].[ApplicationVersionID]', 'ApplicationVersionId', 'COLUMN'
GO

EXEC sp_rename '[dbo].[ApplicationVersions].[FileName]', 'WebPlayerFileName', 'COLUMN'
GO

ALTER TABLE [MvParadisePaintball].[dbo].[ApplicationVersions] ALTER COLUMN [WebPlayerFileName] NVARCHAR(250) NULL
GO

ALTER TABLE [dbo].[ApplicationVersions] DROP COLUMN Build

EXEC sp_rename '[dbo].[ApplicationVersions].[ReleaseDate]', 'ModificationDate', 'COLUMN'
GO

ALTER TABLE [dbo].[ApplicationVersions] DROP COLUMN ExpirationDate

EXEC sp_rename '[dbo].[ApplicationVersions].[IsCurrent]', 'IsEnabled', 'COLUMN'
GO

ALTER TABLE [dbo].[ApplicationVersions] DROP COLUMN SupportUrl

ALTER TABLE [dbo].[ApplicationVersions] ADD [WarnPlayer] BIT NOT NULL
CONSTRAINT [DF_ApplicationVersions_WarnPlayer] DEFAULT (0)
GO

ALTER TABLE [dbo].[ApplicationVersions] DROP [DF_ApplicationVersions_WarnPlayer]
GO

EXEC sp_rename '[dbo].[ApplicationVersions].[PhotonsGroupID]', 'PhotonClusterId', 'COLUMN'
GO