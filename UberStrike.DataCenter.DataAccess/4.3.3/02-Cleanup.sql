USE [MvParadisePaintball]
GO

DROP TABLE [MvParadisePaintball].[dbo].[Facebook]

DELETE FROM [MvParadisePaintball].[dbo].[Users] WHERE [CMID] = 0 AND UserID NOT IN (
SELECT U.UserID
FROM [MvParadisePaintball].[dbo].[Users] AS U,
		[MvParadisePaintball].[dbo].[Cyworld] AS C
WHERE U.UserID = C.UserId)