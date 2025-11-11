USE [Cmune]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MysteryBoxes]') AND type in (N'U'))
DROP TABLE [dbo].[MysteryBoxes]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[MysteryBoxes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](max) NOT NULL,
	[PriceInPoints] [int] NOT NULL,
	[PriceInCredits] [int] NOT NULL,
	[IconUrl] [nvarchar](max) NOT NULL,
	[Category] [int] NOT NULL,
	[IsAvailableInShop] [bit] NOT NULL,
	[ItemsAttributed] [int] NOT NULL,
	[ImageUrl] [nvarchar](max) NOT NULL,
	[ExposeItemsToPlayers] [bit] NOT NULL,
	[PointsAttributed] [int] NOT NULL,
	[PointsAttributedWeight] [int] NOT NULL,
	[CreditsAttributed] [int] NOT NULL,
	[CreditsAttributedWeight] [int] NOT NULL,
 CONSTRAINT [PK_MysteryBoxes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_MysteryBoxItems_Items]') AND parent_object_id = OBJECT_ID(N'[dbo].[MysteryBoxItems]'))
ALTER TABLE [dbo].[MysteryBoxItems] DROP CONSTRAINT [FK_MysteryBoxItems_Items]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_MysteryBoxItems_MysteryBoxes]') AND parent_object_id = OBJECT_ID(N'[dbo].[MysteryBoxItems]'))
ALTER TABLE [dbo].[MysteryBoxItems] DROP CONSTRAINT [FK_MysteryBoxItems_MysteryBoxes]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MysteryBoxItems]') AND type in (N'U'))
DROP TABLE [dbo].[MysteryBoxItems]
GO

CREATE TABLE [dbo].[MysteryBoxItems](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ItemId] [int] NOT NULL,
	[DurationType] [int] NOT NULL,
	[ItemWeight] [int] NOT NULL,
	[MysteryBoxId] [int] NOT NULL,
 CONSTRAINT [PK_MysteryBoxItems] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[MysteryBoxItems]  WITH CHECK ADD  CONSTRAINT [FK_MysteryBoxItems_Items] FOREIGN KEY([ItemId])
REFERENCES [dbo].[Items] ([ItemId])
GO

ALTER TABLE [dbo].[MysteryBoxItems] CHECK CONSTRAINT [FK_MysteryBoxItems_Items]
GO

ALTER TABLE [dbo].[MysteryBoxItems]  WITH CHECK ADD  CONSTRAINT [FK_MysteryBoxItems_MysteryBoxes] FOREIGN KEY([MysteryBoxId])
REFERENCES [dbo].[MysteryBoxes] ([Id])
GO

ALTER TABLE [dbo].[MysteryBoxItems] CHECK CONSTRAINT [FK_MysteryBoxItems_MysteryBoxes]
GO