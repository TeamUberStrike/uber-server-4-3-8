USE [MvParadisePaintball]
GO

ALTER TABLE [MvParadisePaintball].[dbo].[FacebookReferrers] ADD [FirstName] NVARCHAR(75) NOT NULL
CONSTRAINT [DF_FacebookReferrers_FirstName] DEFAULT ('')
GO

ALTER TABLE [MvParadisePaintball].[dbo].[FacebookReferrers] ADD [LastName] NVARCHAR(75) NOT NULL
CONSTRAINT [DF_FacebookReferrers_LastName] DEFAULT ('')
GO

ALTER TABLE [MvParadisePaintball].[dbo].[FacebookReferrers] ADD [Cmid] INT NOT NULL
CONSTRAINT [DF_FacebookReferrers_Cmid] DEFAULT (0)
GO

UPDATE [MvParadisePaintball].[dbo].[FacebookReferrers] SET [Cmid] = [U].CMID
FROM [MvParadisePaintball].[dbo].[FacebookReferrers] AS R,
		[MvParadisePaintball].[dbo].[Facebook] AS F,
		[MvParadisePaintball].[dbo].[Users] AS U
WHERE R.FacebookID = F.FacebookID AND F.UserID = U.UserID