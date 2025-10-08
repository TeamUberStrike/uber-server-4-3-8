USE [MvParadisePaintball]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[WeeklySpecials]') AND type in (N'U'))
DROP TABLE [dbo].[WeeklySpecials]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[WeeklySpecials](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PopupTitle] [nvarchar](100) NOT NULL,
	[PopupText] [nvarchar](500) NOT NULL,
	[ImageUrl] [nvarchar](250) NOT NULL,
	[ItemId] [int] NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NULL,
 CONSTRAINT [PK_WeeklySpecials] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO