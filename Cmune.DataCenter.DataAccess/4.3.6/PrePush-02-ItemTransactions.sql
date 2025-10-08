USE [Cmune]

ALTER TABLE [dbo].[ItemTransactions] ADD [MarketLocation] INTEGER NOT NULL
CONSTRAINT [DF_ItemTransactions_MarketLocation] DEFAULT (1)
GO

ALTER TABLE [dbo].[ItemTransactions] ADD [RecommendationType] INTEGER NOT NULL
CONSTRAINT [DF_ItemTransactions_RecommendationType] DEFAULT (0)
GO