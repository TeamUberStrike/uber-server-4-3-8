USE [Cmune]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_CreditPacks_OnSale]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[CreditPacks] DROP CONSTRAINT [DF_CreditPacks_OnSale]
END

GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CreditPacks]') AND type in (N'U'))
DROP TABLE [dbo].[CreditPacks]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CreditPackItems_CreditPacks]') AND parent_object_id = OBJECT_ID(N'[dbo].[CreditPackItems]'))
ALTER TABLE [dbo].[CreditPackItems] DROP CONSTRAINT [FK_CreditPackItems_CreditPacks]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CreditPackItems_Items]') AND parent_object_id = OBJECT_ID(N'[dbo].[CreditPackItems]'))
ALTER TABLE [dbo].[CreditPackItems] DROP CONSTRAINT [FK_CreditPackItems_Items]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CreditPackItems]') AND type in (N'U'))
DROP TABLE [dbo].[CreditPackItems]
GO