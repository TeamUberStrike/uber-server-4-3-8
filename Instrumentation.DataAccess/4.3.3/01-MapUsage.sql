USE [Instrumentation]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MapUsageStats]') AND type in (N'U'))
DROP TABLE [dbo].[MapUsageStats]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[MapUsageStats](
	[Id] [int] NOT NULL,
	[StatDate] [datetime] NOT NULL,
	[MapId] [int] NOT NULL,
	[GameModeId] [int] NOT NULL,
	[TimeLimit] [int] NOT NULL,
	[PlayerLimit] [int] NOT NULL,
	[PlayersTotal] [int] NOT NULL
) ON [PRIMARY]

GO