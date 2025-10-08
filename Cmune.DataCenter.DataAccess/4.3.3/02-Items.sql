USE [Cmune]

ALTER TABLE [Cmune].[dbo].[Items] ADD [IsDisable] BIT NOT NULL
CONSTRAINT [DF_Items_IsDisable] DEFAULT (0)
GO

UPDATE [Cmune].[dbo].[Items] SET [IsDisable] = 1 WHERE [ItemId] < 1000