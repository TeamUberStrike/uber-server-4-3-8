USE [Cmune]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LuckyDraws]') AND type in (N'U'))
DROP TABLE [dbo].[LuckyDraws]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[LuckyDraws](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](max) NOT NULL,
	[PriceInPoints] [int] NOT NULL,
	[PriceInCredits] [int] NOT NULL,
	[IconUrl] [nvarchar](max) NOT NULL,
	[Category] [int] NOT NULL,
	[IsAvailableInShop] [bit] NOT NULL,
 CONSTRAINT [PK_LuckyDraws] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_LuckyDrawSets_LuckyDraws]') AND parent_object_id = OBJECT_ID(N'[dbo].[LuckyDrawSets]'))
ALTER TABLE [dbo].[LuckyDrawSets] DROP CONSTRAINT [FK_LuckyDrawSets_LuckyDraws]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LuckyDrawSets]') AND type in (N'U'))
DROP TABLE [dbo].[LuckyDrawSets]
GO

CREATE TABLE [dbo].[LuckyDrawSets](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SetWeight] [int] NOT NULL,
	[CreditsAttributed] [int] NOT NULL,
	[PointsAttributed] [int] NOT NULL,
	[ImageUrl] [nvarchar](max) NOT NULL,
	[ExposeItemsToPlayers] [bit] NOT NULL,
	[LuckyDrawId] [int] NOT NULL,
 CONSTRAINT [PK_LuckyDrawSets] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[LuckyDrawSets]  WITH CHECK ADD  CONSTRAINT [FK_LuckyDrawSets_LuckyDraws] FOREIGN KEY([LuckyDrawId])
REFERENCES [dbo].[LuckyDraws] ([Id])
GO

ALTER TABLE [dbo].[LuckyDrawSets] CHECK CONSTRAINT [FK_LuckyDrawSets_LuckyDraws]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_LuckyDrawSetItems_Items]') AND parent_object_id = OBJECT_ID(N'[dbo].[LuckyDrawSetItems]'))
ALTER TABLE [dbo].[LuckyDrawSetItems] DROP CONSTRAINT [FK_LuckyDrawSetItems_Items]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_LuckyDrawSetItems_LuckyDrawSets]') AND parent_object_id = OBJECT_ID(N'[dbo].[LuckyDrawSetItems]'))
ALTER TABLE [dbo].[LuckyDrawSetItems] DROP CONSTRAINT [FK_LuckyDrawSetItems_LuckyDrawSets]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LuckyDrawSetItems]') AND type in (N'U'))
DROP TABLE [dbo].[LuckyDrawSetItems]
GO

CREATE TABLE [dbo].[LuckyDrawSetItems](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ItemId] [int] NOT NULL,
	[DurationType] [int] NOT NULL,
	[LuckyDrawSetId] [int] NOT NULL,
 CONSTRAINT [PK_LuckyDrawSetItems] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[LuckyDrawSetItems]  WITH CHECK ADD  CONSTRAINT [FK_LuckyDrawSetItems_Items] FOREIGN KEY([ItemId])
REFERENCES [dbo].[Items] ([ItemId])
GO

ALTER TABLE [dbo].[LuckyDrawSetItems] CHECK CONSTRAINT [FK_LuckyDrawSetItems_Items]
GO

ALTER TABLE [dbo].[LuckyDrawSetItems]  WITH CHECK ADD  CONSTRAINT [FK_LuckyDrawSetItems_LuckyDrawSets] FOREIGN KEY([LuckyDrawSetId])
REFERENCES [dbo].[LuckyDrawSets] ([Id])
GO

ALTER TABLE [dbo].[LuckyDrawSetItems] CHECK CONSTRAINT [FK_LuckyDrawSetItems_LuckyDrawSets]
GO