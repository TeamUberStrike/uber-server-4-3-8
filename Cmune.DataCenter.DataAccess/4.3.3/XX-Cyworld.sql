DECLARE @FacebookId BIGINT;
DECLARE @FacebookCmid INT;
DECLARE @CyworldId INT;
DECLARE @CyworldCmid INT;
DECLARE @EsnsId INT;
DECLARE @CyworldUserId INT;
DECLARE @CyworldReason NVARCHAR(1000);
DECLARE @FacebookReason NVARCHAR(1000);

-- TO SET --
SET @CyworldId = 53437679
SET @FacebookId = 100001593423727
-- TO SET --

SELECT @CyworldCmid = [CMID] FROM [Cmune].[dbo].[ESNSIdentities] WHERE [Handle] = @CyworldId AND [Type] = 8
SELECT @EsnsId = [ESNSID], @FacebookCmid = [CMID] FROM [Cmune].[dbo].[ESNSIdentities] WHERE [Handle] = @FacebookId AND [Type] = 6
SELECT @CyworldUserId = [UserID] FROM [MvParadisePaintball].[dbo].[Users] WHERE [CMID] = @CyworldCmid

SELECT @CyworldId As CyworldId
SELECT @CyworldUserId As CyworldUserId
SELECT @CyworldCmid AS CyworldCmid
SELECT @EsnsId As EsnsId
SELECT @FacebookCmid As FacebookCmid

SET @CyworldReason = 'New Facebook of ' + CAST(@FacebookCmid AS NVARCHAR(15))
SET @FacebookReason = 'Old Facebook of ' + CAST(@CyworldCmid AS NVARCHAR(15))

SELECT @CyworldReason As CyworldReason
SELECT @FacebookReason As FacebookReason

IF @EsnsId > 0
BEGIN
	SELECT 'Existing FacebookId'
	
	UPDATE [Cmune].[dbo].[ESNSIdentities] SET [CMID] = @CyworldCmid WHERE [ESNSID] = @EsnsId
	
	INSERT INTO [Cmune].[dbo].[ModerationActions]
           ([ActionType]
           ,[SourceCmid]
           ,[SourceIp]
           ,[TargetCmid]
           ,[ActionDate]
           ,[ApplicationId]
           ,[Reason])
     VALUES
           (5
           ,955
           ,3416710415
           ,@CyworldCmid
           ,GETDATE()
           ,1
           ,@CyworldReason)
    
    INSERT INTO [Cmune].[dbo].[ModerationActions]
           ([ActionType]
           ,[SourceCmid]
           ,[SourceIp]
           ,[TargetCmid]
           ,[ActionDate]
           ,[ApplicationId]
           ,[Reason])
     VALUES
           (5
           ,955
           ,3416710415
           ,@FacebookCmid
           ,GETDATE()
           ,1
           ,@FacebookReason)
END
ELSE
BEGIN
	SELECT 'Non Existing FacebookId'
	
	INSERT INTO [Cmune].[dbo].[ESNSIdentities]
           ([CMID]
           ,[LastSyncDate]
           ,[AutoAccept]
           ,[Handle]
           ,[Type]
           ,[IsVerified])
     VALUES
           (@CyworldCmid
           ,GETDATE()
           ,0
           ,@FacebookId
           ,6
           ,1)
    
    INSERT INTO [Cmune].[dbo].[ModerationActions]
           ([ActionType]
           ,[SourceCmid]
           ,[SourceIp]
           ,[TargetCmid]
           ,[ActionDate]
           ,[ApplicationId]
           ,[Reason])
     VALUES
           (5
           ,955
           ,3416710415
           ,@CyworldCmid
           ,GETDATE()
           ,1
           ,'Added Facebook Id')
END