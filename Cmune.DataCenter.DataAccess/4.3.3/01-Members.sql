USE [Cmune]

ALTER TABLE [Cmune].[dbo].[Members] ADD [IsAccountComplete] BIT NOT NULL
CONSTRAINT [DF_Members_IsAccountComplete] DEFAULT (1)
GO