USE [Cmune]

ALTER TABLE [dbo].[Members] DROP [DF_Members_isActivated]
GO

ALTER TABLE [dbo].[Members] DROP COLUMN [isActivated]
GO