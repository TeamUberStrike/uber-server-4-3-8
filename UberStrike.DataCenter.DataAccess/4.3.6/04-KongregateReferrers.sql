USE [MvParadisePaintball]
GO

/****** Object:  Table [dbo].[KongregateReferrers]    Script Date: 04/18/2012 12:34:01 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[KongregateReferrers](
	[KongregateReferrerId] [int] IDENTITY(1,1) NOT NULL,
	[KongregateId] [bigint] NOT NULL,
	[CreationDate] [datetime] NULL,k

	[Referrer] [nvarchar](2000) NOT NULL,
	[ReferrerPartnerId] [int] NULL,
	[UserName] [nchar](10) NULL,
	[FirstName] [nvarchar](75) NULL,
	[LastName] [nvarchar](75) NULL,
	[Cmid] [int] NOT NULL,
 CONSTRAINT [PK_KongregateReferrers] PRIMARY KEY CLUSTERED 
(
	[KongregateReferrerId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


