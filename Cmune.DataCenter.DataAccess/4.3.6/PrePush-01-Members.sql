USE [Cmune]

ALTER TABLE [dbo].[Members] ADD [EmailAddressState] TINYINT NOT NULL
CONSTRAINT [DF_Members_EmailAddressState] DEFAULT (0)
GO

ALTER TABLE [dbo].[Members] ADD [MarketingSubscriptionState] TINYINT NOT NULL
CONSTRAINT [DF_Members_MarketingSubscriptionState] DEFAULT (1)
GO