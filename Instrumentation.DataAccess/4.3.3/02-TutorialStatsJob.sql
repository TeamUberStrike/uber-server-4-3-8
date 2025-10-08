DECLARE @todayDate DATETIME
DECLARE @yesterdayDate DATETIME
SET @todayDate = CAST(CONVERT(VARCHAR(10), GETDATE(), 101) AS DATETIME)
SET @yesterdayDate = DATEADD(day, -1, @todayDate)

INSERT INTO [Instrumentation].[dbo].[MapUsageStats]
([StatDate], [MapId], [GameModeId], [TimeLimit], [PlayerLimit], [PlayersTotal])
SELECT @yesterdayDate, [MapId], [GameModeId], [TimeLimit], [PlayerLimit], SUM([PlayersTotal])
FROM [MvParadisePaintball].[dbo].[MapUsage]
WHERE [PlayDate] >= @yesterdayDate AND [PlayDate] < @todayDate
GROUP BY [MapId], [GameModeId], [TimeLimit], [PlayerLimit]