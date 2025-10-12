USE [MvParadisePaintball]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PromotionContent]') AND type in (N'U'))
DROP TABLE [dbo].[PromotionContent]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PromotionContent](
	[PromotionContentId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NOT NULL,
	[IsPermanent] [bit] NOT NULL,
 CONSTRAINT [PK_PromotionContent] PRIMARY KEY CLUSTERED 
(
	[PromotionContentId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PromotionContentElements]') AND type in (N'U'))
DROP TABLE [dbo].[PromotionContentElements]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PromotionContentElements](
	[PromotionContentElementId] [int] IDENTITY(1,1) NOT NULL,
	[ChannelType] [int] NOT NULL,
	[ChannelLocation] [int] NOT NULL,
	[Filename] [nvarchar](200) NOT NULL,
	[FilenameTitle] [nvarchar](200) NULL,
	[PromotionContentId] [int] NOT NULL,
 CONSTRAINT [PK_PromotionContentElement] PRIMARY KEY CLUSTERED 
(
	[PromotionContentElementId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO