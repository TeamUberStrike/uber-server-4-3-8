USE [Instrumentation]
GO

-- CreditDepositStats

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

-- DailyActivePlayingUsers

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

-- MonthlyActivePlayingUsers

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

-- UserInstallStepTotal

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_UserInstallStepTotal_ChannelId]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[UserInstallStepTotal] DROP CONSTRAINT [DF_UserInstallStepTotal_ChannelId]
END

GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserInstallStepTotal]') AND type in (N'U'))
DROP TABLE [dbo].[UserInstallStepTotal]
GO

-- ReferrerStatsCredits

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_ReferrerStatsCredits_ReferrerPartnerId]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ReferrerStatsCredits] DROP CONSTRAINT [DF_ReferrerStatsCredits_ReferrerPartnerId]
END

GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_ReferrerStatsCredits_CashDeposited]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ReferrerStatsCredits] DROP CONSTRAINT [DF_ReferrerStatsCredits_CashDeposited]
END

GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_ReferrerStatsCredits_TransactionsUnique]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ReferrerStatsCredits] DROP CONSTRAINT [DF_ReferrerStatsCredits_TransactionsUnique]
END

GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_ReferrerStatsCredits_TransactionsTotal]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ReferrerStatsCredits] DROP CONSTRAINT [DF_ReferrerStatsCredits_TransactionsTotal]
END

GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ReferrerStatsCredits]') AND type in (N'U'))
DROP TABLE [dbo].[ReferrerStatsCredits]
GO