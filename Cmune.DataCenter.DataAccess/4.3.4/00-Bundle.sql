USE [Cmune]
GO

ALTER TABLE [dbo].[Bundle] ADD [IsDefault] BIT NOT NULL
CONSTRAINT [DF_Bundle_IsDefault] DEFAULT (0)
GO