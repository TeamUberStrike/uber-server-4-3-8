USE [MvParadisePaintball]

ALTER TABLE [dbo].[MapVersions] ADD [MapType] INT NOT NULL
CONSTRAINT [DF_MapVersions_MapType] DEFAULT (1)
GO

ALTER TABLE [dbo].[MapVersions] DROP [DF_MapVersions_MapType]
GO