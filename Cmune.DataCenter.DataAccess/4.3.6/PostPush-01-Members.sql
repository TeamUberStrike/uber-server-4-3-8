USE [Cmune]

UPDATE [dbo].[Members] SET [EmailAddressState] = 1 WHERE [isActivated] = 1 AND [Login] IS NOT NULL