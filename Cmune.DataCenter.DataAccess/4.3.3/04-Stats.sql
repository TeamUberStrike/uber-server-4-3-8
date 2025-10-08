USE [Cmune]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_CreditDepositStats_PaymentPartnerId]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[CreditDepositStats] DROP CONSTRAINT [DF_CreditDepositStats_PaymentPartnerId]
END

GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_CreditDepositStats_CashDeposited]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[CreditDepositStats] DROP CONSTRAINT [DF_CreditDepositStats_CashDeposited]
END

GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_CreditDepositStats_TransactionsUnique]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[CreditDepositStats] DROP CONSTRAINT [DF_CreditDepositStats_TransactionsUnique]
END

GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_CreditDepositStats_TransactionsTotal]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[CreditDepositStats] DROP CONSTRAINT [DF_CreditDepositStats_TransactionsTotal]
END

GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CreditDepositStats]') AND type in (N'U'))
DROP TABLE [dbo].[CreditDepositStats]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_DailyActivePlayingUsers_Applications]') AND parent_object_id = OBJECT_ID(N'[dbo].[DailyActivePlayingUsers]'))
ALTER TABLE [dbo].[DailyActivePlayingUsers] DROP CONSTRAINT [FK_DailyActivePlayingUsers_Applications]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_DailyActivePlayingUsers_ApplicationId]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[DailyActivePlayingUsers] DROP CONSTRAINT [DF_DailyActivePlayingUsers_ApplicationId]
END

GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_DailyActivePlayingUsers_ChannelId]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[DailyActivePlayingUsers] DROP CONSTRAINT [DF_DailyActivePlayingUsers_ChannelId]
END

GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_DailyActivePlayingUsers_RefPartnerId]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[DailyActivePlayingUsers] DROP CONSTRAINT [DF_DailyActivePlayingUsers_RefPartnerId]
END

GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DailyActivePlayingUsers]') AND type in (N'U'))
DROP TABLE [dbo].[DailyActivePlayingUsers]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DailyActiveUsers]') AND type in (N'U'))
DROP TABLE [dbo].[DailyActiveUsers]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_MonthlyActivePlayingUsers_Applications]') AND parent_object_id = OBJECT_ID(N'[dbo].[MonthlyActivePlayingUsers]'))
ALTER TABLE [dbo].[MonthlyActivePlayingUsers] DROP CONSTRAINT [FK_MonthlyActivePlayingUsers_Applications]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_MonthlyActivePlayingUsers_ApplicationId]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[MonthlyActivePlayingUsers] DROP CONSTRAINT [DF_MonthlyActivePlayingUsers_ApplicationId]
END

GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_MonthlyActivePlayingUsers_ChannelId]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[MonthlyActivePlayingUsers] DROP CONSTRAINT [DF_MonthlyActivePlayingUsers_ChannelId]
END

GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_MonthlyActivePlayingUsers_RefPartnerId]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[MonthlyActivePlayingUsers] DROP CONSTRAINT [DF_MonthlyActivePlayingUsers_RefPartnerId]
END

GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MonthlyActivePlayingUsers]') AND type in (N'U'))
DROP TABLE [dbo].[MonthlyActivePlayingUsers]
GO