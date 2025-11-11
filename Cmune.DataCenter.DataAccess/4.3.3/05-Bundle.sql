USE [Cmune]

ALTER TABLE [dbo].[Bundle] ADD [Category] INTEGER NOT NULL
CONSTRAINT [DF_Bundle_Category] DEFAULT (0)
GO

ALTER TABLE [dbo].[Bundle] ADD [PromotionTag] NVARCHAR(100) NOT NULL
CONSTRAINT [DF_Bundle_PromotionTag] DEFAULT ('')
GO

ALTER TABLE [dbo].[Bundle] ADD [UniqueId] NVARCHAR(100) NOT NULL
CONSTRAINT [DF_Bundle_UniqueId] DEFAULT ('')
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_BundlesAvailability_Bundle]') AND parent_object_id = OBJECT_ID(N'[dbo].[BundlesAvailability]'))
ALTER TABLE [dbo].[BundlesAvailability] DROP CONSTRAINT [FK_BundlesAvailability_Bundle]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BundlesAvailability]') AND type in (N'U'))
DROP TABLE [dbo].[BundlesAvailability]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[BundlesAvailability](
	[BundleId] [int] NOT NULL,
	[Channel] [int] NOT NULL,
 CONSTRAINT [PK_BundlesAvailability] PRIMARY KEY CLUSTERED 
(
	[BundleId] ASC,
	[Channel] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[BundlesAvailability]  WITH CHECK ADD  CONSTRAINT [FK_BundlesAvailability_Bundle] FOREIGN KEY([BundleId])
REFERENCES [dbo].[Bundle] ([Id])
GO

ALTER TABLE [dbo].[BundlesAvailability] CHECK CONSTRAINT [FK_BundlesAvailability_Bundle]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_MasReceipts_CreditDeposit]') AND parent_object_id = OBJECT_ID(N'[dbo].[MasReceipts]'))
ALTER TABLE [dbo].[MasReceipts] DROP CONSTRAINT [FK_MasReceipts_CreditDeposit]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MasReceipts]') AND type in (N'U'))
DROP TABLE [dbo].[MasReceipts]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[MasReceipts](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Receipt] [nvarchar](max) NOT NULL,
	[TransactionId] [int] NOT NULL,
 CONSTRAINT [PK_MasReceipts] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[MasReceipts]  WITH CHECK ADD  CONSTRAINT [FK_MasReceipts_CreditDeposit] FOREIGN KEY([TransactionId])
REFERENCES [dbo].[CreditDeposit] ([id])
GO

ALTER TABLE [dbo].[MasReceipts] CHECK CONSTRAINT [FK_MasReceipts_CreditDeposit]
GO