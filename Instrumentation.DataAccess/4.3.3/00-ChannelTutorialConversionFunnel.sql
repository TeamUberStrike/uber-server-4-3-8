USE [Instrumentation]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ChannelTutorialConversionFunnel]') AND type in (N'U'))
DROP TABLE [dbo].[ChannelTutorialConversionFunnel]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ChannelTutorialConversionFunnel](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[StepDate] [datetime] NOT NULL,
	[StepId] [int] NOT NULL,
	[StepCount] [int] NOT NULL,
	[Region] [int] NOT NULL,
	[ChannelId] [int] NOT NULL,
 CONSTRAINT [PK_ChannelTutorialConversionFunnel] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO