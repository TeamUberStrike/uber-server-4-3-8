USE [MvParadisePaintball]
GO

DELETE FROM [MvParadisePaintball].[dbo].[FacebookReferrers] WHERE [FacebookID] IN (
SELECT R.FacebookID
FROM [MvParadisePaintball].[dbo].[FacebookReferrers] AS R,
		[MvParadisePaintball].[dbo].[Facebook] AS F,
		[MvParadisePaintball].[dbo].[Users] AS U
WHERE R.FacebookID = F.FacebookID AND F.UserID = U.UserID AND U.[CMID] = 0)

DELETE FROM [MvParadisePaintball].[dbo].[MySpaceReferrers] WHERE [MySpaceID] IN (
SELECT R.MySpaceID
FROM [MvParadisePaintball].[dbo].[MySpaceReferrers] AS R,
		[MvParadisePaintball].[dbo].[MySpace] AS M,
		[MvParadisePaintball].[dbo].[Users] AS U
WHERE R.MySpaceID = M.MySpaceID AND M.UserID = U.UserID AND U.[CMID] = 0)

DELETE FROM [MvParadisePaintball].[dbo].[MySpace] WHERE [MySpaceID] IN (
SELECT M.MySpaceID
FROM [MvParadisePaintball].[dbo].[MySpace] AS M,
		[MvParadisePaintball].[dbo].[Users] AS U
WHERE M.UserID = U.UserID AND U.Cmid = 0)

DROP TABLE [MvParadisePaintball].[dbo].[CyworldReferrers]

DELETE FROM [MvParadisePaintball].[dbo].[PortalReferrers] WHERE UserID NOT IN (
SELECT U.UserID
	FROM [MvParadisePaintball].[dbo].[PortalReferrers] AS R,
		[MvParadisePaintball].[dbo].[Users] AS U
WHERE R.UserID = U.UserID
)

ALTER TABLE [MvParadisePaintball].[dbo].[PortalReferrers] ADD [Cmid] INT NOT NULL
CONSTRAINT [DF_PortalReferrers_Cmid] DEFAULT (0)
GO

UPDATE [MvParadisePaintball].[dbo].[PortalReferrers] SET [Cmid] = U.[CMID]
FROM [MvParadisePaintball].[dbo].[PortalReferrers] AS R,
	[MvParadisePaintball].[dbo].[Users] AS U
WHERE R.UserID = U.UserID

ALTER TABLE [MvParadisePaintball].[dbo].[PortalReferrers] DROP COLUMN UserID

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AppSettings]') AND type in (N'U'))
DROP TABLE [dbo].[AppSettings]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DailyTotalRanking]') AND type in (N'U'))
DROP TABLE [dbo].[DailyTotalRanking]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Games]') AND type in (N'U'))
DROP TABLE [dbo].[Games]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_ReferrerStats_InstallCount]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ReferrerStats] DROP CONSTRAINT [DF_ReferrerStats_InstallCount]
END

GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_ReferrerStats_MemberCount]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ReferrerStats] DROP CONSTRAINT [DF_ReferrerStats_MemberCount]
END

GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_ReferrerStats_ReferrerPartnerId]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ReferrerStats] DROP CONSTRAINT [DF_ReferrerStats_ReferrerPartnerId]
END

GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_ReferrerStats_ChannelID]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ReferrerStats] DROP CONSTRAINT [DF_ReferrerStats_ChannelID]
END

GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_ReferrerStats_CreditsDeposited]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ReferrerStats] DROP CONSTRAINT [DF_ReferrerStats_CreditsDeposited]
END

GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_ReferrerStats_Splats]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ReferrerStats] DROP CONSTRAINT [DF_ReferrerStats_Splats]
END

GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ReferrerStats]') AND type in (N'U'))
DROP TABLE [dbo].[ReferrerStats]
GO

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

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[StepTracking]') AND type in (N'U'))
DROP TABLE [dbo].[StepTracking]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_UserInstallStepTotal_ChannelId]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[UserInstallStepTotal] DROP CONSTRAINT [DF_UserInstallStepTotal_ChannelId]
END

GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserInstallStepTotal]') AND type in (N'U'))
DROP TABLE [dbo].[UserInstallStepTotal]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_UsersStepsTracking_Users]') AND parent_object_id = OBJECT_ID(N'[dbo].[UsersStepsTracking]'))
ALTER TABLE [dbo].[UsersStepsTracking] DROP CONSTRAINT [FK_UsersStepsTracking_Users]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_UsersStepsTracking_Users1]') AND parent_object_id = OBJECT_ID(N'[dbo].[UsersStepsTracking]'))
ALTER TABLE [dbo].[UsersStepsTracking] DROP CONSTRAINT [FK_UsersStepsTracking_Users1]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UsersStepsTracking]') AND type in (N'U'))
DROP TABLE [dbo].[UsersStepsTracking]
GO