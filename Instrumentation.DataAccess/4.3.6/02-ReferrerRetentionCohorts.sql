USE [Instrumentation]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ReferrerRetentionCohorts]') AND type in (N'U'))
DROP TABLE [dbo].[ReferrerRetentionCohorts]
GO