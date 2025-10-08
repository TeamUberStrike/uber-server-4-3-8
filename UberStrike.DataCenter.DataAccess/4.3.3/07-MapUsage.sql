USE [MvParadisePaintball]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MapUsage]') AND type in (N'U'))
DROP TABLE [dbo].[MapUsage]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[MapUsage](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PlayDate] [datetime] NOT NULL,
	[MapId] [int] NOT NULL,
	[GameModeId] [int] NOT NULL,
	[TimeLimit] [int] NOT NULL,
	[PlayerLimit] [int] NOT NULL,
	[PlayersTotal] [int] NOT NULL,
	[PlayersCompleted] [int] NOT NULL,
 CONSTRAINT [PK_MapUsage] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO