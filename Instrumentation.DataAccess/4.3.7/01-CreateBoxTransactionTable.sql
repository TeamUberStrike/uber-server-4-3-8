USE [Cmune]
GO

/****** Object:  Table [dbo].[BoxTransactions]    Script Date: 05/31/2012 18:53:00 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[BoxTransactions](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BoxType] [int] NOT NULL,
	[BoxId] [int] NOT NULL,
	[Cmid] [int] NOT NULL,
	[TransactionDate] [datetime] NOT NULL,
	[IsAdmin] [bit] NOT NULL,
	[CreditPrice] [int] NOT NULL,
	[PointPrice] [int] NOT NULL,
	[TotalCreditsAttributed] [int] NOT NULL,
	[TotalPointsAttributed] [int] NOT NULL,
	[Category] [int] NOT NULL,
 CONSTRAINT [PK_BoxTransactions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


