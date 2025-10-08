USE [MvParadisePaintball]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TutorialSteps]') AND type in (N'U'))
DROP TABLE [dbo].[TutorialSteps]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[TutorialSteps](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Cmid] [int] NOT NULL,
	[StepId] [int] NOT NULL,
	[StepDateTime] [datetime] NOT NULL,
 CONSTRAINT [PK_TutorialSteps] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO