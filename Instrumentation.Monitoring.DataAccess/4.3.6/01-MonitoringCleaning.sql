USE [CmuneMonitoring]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_DatabaseTestHistory_DatabaseTests]') AND parent_object_id = OBJECT_ID(N'[dbo].[DatabaseTestHistory]'))
ALTER TABLE [dbo].[DatabaseTestHistory] DROP CONSTRAINT [FK_DatabaseTestHistory_DatabaseTests]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_DatabaseTestHistory_PollHistory]') AND parent_object_id = OBJECT_ID(N'[dbo].[DatabaseTestHistory]'))
ALTER TABLE [dbo].[DatabaseTestHistory] DROP CONSTRAINT [FK_DatabaseTestHistory_PollHistory]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DatabaseTestHistory]') AND type in (N'U'))
DROP TABLE [dbo].[DatabaseTestHistory]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_DatabaseTests_ManagedServers]') AND parent_object_id = OBJECT_ID(N'[dbo].[DatabaseTests]'))
ALTER TABLE [dbo].[DatabaseTests] DROP CONSTRAINT [FK_DatabaseTests_ManagedServers]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_DatabaseTests_IsDisable]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[DatabaseTests] DROP CONSTRAINT [DF_DatabaseTests_IsDisable]
END

GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DatabaseTests]') AND type in (N'U'))
DROP TABLE [dbo].[DatabaseTests]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_LastPolls_ManagedServers]') AND parent_object_id = OBJECT_ID(N'[dbo].[LastPolls]'))
ALTER TABLE [dbo].[LastPolls] DROP CONSTRAINT [FK_LastPolls_ManagedServers]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_LastPolls_PollHistory]') AND parent_object_id = OBJECT_ID(N'[dbo].[LastPolls]'))
ALTER TABLE [dbo].[LastPolls] DROP CONSTRAINT [FK_LastPolls_PollHistory]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LastPolls]') AND type in (N'U'))
DROP TABLE [dbo].[LastPolls]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PhotonTestHistory_PhotonTests]') AND parent_object_id = OBJECT_ID(N'[dbo].[PhotonTestHistory]'))
ALTER TABLE [dbo].[PhotonTestHistory] DROP CONSTRAINT [FK_PhotonTestHistory_PhotonTests]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PhotonTestHistory_PollHistory]') AND parent_object_id = OBJECT_ID(N'[dbo].[PhotonTestHistory]'))
ALTER TABLE [dbo].[PhotonTestHistory] DROP CONSTRAINT [FK_PhotonTestHistory_PollHistory]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PhotonTestHistory]') AND type in (N'U'))
DROP TABLE [dbo].[PhotonTestHistory]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PhotonTests_ManagedServers]') AND parent_object_id = OBJECT_ID(N'[dbo].[PhotonTests]'))
ALTER TABLE [dbo].[PhotonTests] DROP CONSTRAINT [FK_PhotonTests_ManagedServers]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_PhotonTests_IsDisable]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[PhotonTests] DROP CONSTRAINT [DF_PhotonTests_IsDisable]
END

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PhotonTests]') AND type in (N'U'))
DROP TABLE [dbo].[PhotonTests]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RotationMembers]') AND type in (N'U'))
DROP TABLE [dbo].[RotationMembers]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_WebserviceTestResults_PollHistory]') AND parent_object_id = OBJECT_ID(N'[dbo].[WebserviceTestHistory]'))
ALTER TABLE [dbo].[WebserviceTestHistory] DROP CONSTRAINT [FK_WebserviceTestResults_PollHistory]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_WebserviceTestResults_WebserviceTests]') AND parent_object_id = OBJECT_ID(N'[dbo].[WebserviceTestHistory]'))
ALTER TABLE [dbo].[WebserviceTestHistory] DROP CONSTRAINT [FK_WebserviceTestResults_WebserviceTests]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[WebserviceTestHistory]') AND type in (N'U'))
DROP TABLE [dbo].[WebserviceTestHistory]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_WebserviceTests_ManagedServers]') AND parent_object_id = OBJECT_ID(N'[dbo].[WebserviceTests]'))
ALTER TABLE [dbo].[WebserviceTests] DROP CONSTRAINT [FK_WebserviceTests_ManagedServers]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_WebserviceTests_IsDisable]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[WebserviceTests] DROP CONSTRAINT [DF_WebserviceTests_IsDisable]
END

GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[WebserviceTests]') AND type in (N'U'))
DROP TABLE [dbo].[WebserviceTests]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_WebsiteTestHistory_PollHistory]') AND parent_object_id = OBJECT_ID(N'[dbo].[WebsiteTestHistory]'))
ALTER TABLE [dbo].[WebsiteTestHistory] DROP CONSTRAINT [FK_WebsiteTestHistory_PollHistory]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_WebsiteTestHistory_WebsiteTests]') AND parent_object_id = OBJECT_ID(N'[dbo].[WebsiteTestHistory]'))
ALTER TABLE [dbo].[WebsiteTestHistory] DROP CONSTRAINT [FK_WebsiteTestHistory_WebsiteTests]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[WebsiteTestHistory]') AND type in (N'U'))
DROP TABLE [dbo].[WebsiteTestHistory]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_WebsiteTests_ManagedServers]') AND parent_object_id = OBJECT_ID(N'[dbo].[WebsiteTests]'))
ALTER TABLE [dbo].[WebsiteTests] DROP CONSTRAINT [FK_WebsiteTests_ManagedServers]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_WebsiteTests_IsDisable]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[WebsiteTests] DROP CONSTRAINT [DF_WebsiteTests_IsDisable]
END

GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[WebsiteTests]') AND type in (N'U'))
DROP TABLE [dbo].[WebsiteTests]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PollHistory_ManagedServers]') AND parent_object_id = OBJECT_ID(N'[dbo].[PollHistory]'))
ALTER TABLE [dbo].[PollHistory] DROP CONSTRAINT [FK_PollHistory_ManagedServers]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PollHistory]') AND type in (N'U'))
DROP TABLE [dbo].[PollHistory]
GO