USE [MvParadisePaintball]
GO

EXEC sp_rename '[dbo].[ApplicationVersions].[ApplicationVersionId]', 'ApplicationVersionID', 'COLUMN'
GO

EXEC sp_rename '[dbo].[ApplicationVersions].[WebPlayerFileName]', 'FileName', 'COLUMN'
GO

ALTER TABLE [MvParadisePaintball].[dbo].[ApplicationVersions] ALTER COLUMN [FileName] NVARCHAR(50) NULL
GO

ALTER TABLE [dbo].[ApplicationVersions] ADD [Build] INT NOT NULL
CONSTRAINT [DF_ApplicationVersions_Build] DEFAULT (0)
GO

ALTER TABLE [dbo].[ApplicationVersions] DROP [DF_ApplicationVersions_Build]
GO

EXEC sp_rename '[dbo].[ApplicationVersions].[ModificationDate]', 'ReleaseDate', 'COLUMN'
GO

ALTER TABLE [dbo].[ApplicationVersions] ADD [ExpirationDate] DATETIME NULL
CONSTRAINT [DF_ApplicationVersions_ExpirationDate] DEFAULT (NULL)
GO

ALTER TABLE [dbo].[ApplicationVersions] DROP [DF_ApplicationVersions_ExpirationDate]
GO

EXEC sp_rename '[dbo].[ApplicationVersions].[IsEnabled]', 'IsCurrent', 'COLUMN'
GO

ALTER TABLE [dbo].[ApplicationVersions] ADD [SupportUrl] NVARCHAR(250) NOT NULL
CONSTRAINT [DF_ApplicationVersions_SupportUrl] DEFAULT ('http://uberstrike.com/')
GO

ALTER TABLE [dbo].[ApplicationVersions] DROP [DF_ApplicationVersions_SupportUrl]
GO

ALTER TABLE [dbo].[ApplicationVersions] DROP COLUMN WarnPlayer

EXEC sp_rename '[dbo].[ApplicationVersions].[PhotonClusterId]', 'PhotonsGroupID', 'COLUMN'
GO