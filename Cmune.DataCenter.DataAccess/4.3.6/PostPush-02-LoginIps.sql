USE [Cmune]

DECLARE @Target int
SET @Target = 2000
DECLARE @Count int
SET @Count = @Target
DECLARE @InitialId int
SET @InitialId = 0

ALTER TABLE [dbo].[LoginIps] ADD [Channel] INT NULL DEFAULT (0)

WHILE @Count <> 0 BEGIN

	UPDATE [dbo].[LoginIps] SET [Channel] = 0 WHERE [LoginIpId] > @InitialId AND [LoginIpId] <= @InitialId + @Target
	
	SET @InitialId = @InitialId + @Target
	SELECT @Count = @@ROWCOUNT
	WAITFOR DELAY '000:00:00.200'

END

SET @Count = @Target
SET @InitialId = 0

ALTER TABLE [dbo].[LoginIps] ADD [MachineId] NVARCHAR(50) NULL DEFAULT ('')

WHILE @Count <> 0 BEGIN

	UPDATE [dbo].[LoginIps] SET [MachineId] = '' WHERE [LoginIpId] > @InitialId AND [LoginIpId] <= @InitialId + @Target
	
	SET @InitialId = @InitialId + @Target
	SELECT @Count = @@ROWCOUNT
	WAITFOR DELAY '000:00:00.200'

END