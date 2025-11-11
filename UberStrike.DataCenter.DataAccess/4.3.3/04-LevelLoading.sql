USE [MvParadisePaintball]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Maps]') AND type in (N'U'))
DROP TABLE [dbo].[Maps]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Maps](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MapId] [int] NOT NULL,
	[AppVersion] [nvarchar](20) NOT NULL,
	[DisplayName] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](500) NOT NULL,
	[SceneName] [nvarchar](50) NOT NULL,
	[InUse] [bit] NOT NULL,
	[IsBlueBox] [bit] NOT NULL,
 CONSTRAINT [PK_Maps] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MapVersions]') AND type in (N'U'))
DROP TABLE [dbo].[MapVersions]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[MapVersions](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MapId] [int] NOT NULL,
	[AppVersion] [nvarchar](20) NOT NULL,
	[FileName] [nvarchar](100) NOT NULL,
	[LastUpdatedDate] [datetime] NOT NULL,
 CONSTRAINT [PK_MapVersions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO