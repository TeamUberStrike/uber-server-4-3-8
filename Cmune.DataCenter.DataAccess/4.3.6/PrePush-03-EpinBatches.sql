USE [Cmune]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[EpinBatches](
	[BatchId] [int] IDENTITY(1,1) NOT NULL,
	[ApplicationId] [int] NOT NULL,
	[EpinProvider] [int] NOT NULL,
	[Amount] [int] NOT NULL,
	[CreditAmount] [int] NOT NULL,
	[BatchDate] [datetime] NOT NULL,
	[IsAdmin] [bit] NOT NULL,
	[IsRetired] [bit] NOT NULL,
 CONSTRAINT [PK_EpinBatches] PRIMARY KEY CLUSTERED 
(
	[BatchId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


