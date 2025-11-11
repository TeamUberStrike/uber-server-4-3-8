USE [MvParadisePaintball];
GO

IF OBJECT_ID('[dbo].[Paradise_Propagate_Name]', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[Paradise_Propagate_Name];
GO

CREATE PROCEDURE [dbo].[Paradise_Propagate_Name]
    @Name NVARCHAR(50),
    @Cmid INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @RowsAffected INT = 0;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        IF @Cmid IS NOT NULL AND @Name IS NOT NULL AND LEN(LTRIM(RTRIM(@Name))) > 0
        BEGIN
            -- Update AllTimeTotalRanking table in MvParadisePaintball
            UPDATE [dbo].[AllTimeTotalRanking] 
            SET [Name] = @Name 
            WHERE [CMID] = @Cmid;
            
            SET @RowsAffected = @RowsAffected + @@ROWCOUNT;
            
            -- Update WeeklyTotalRanking table in MvParadisePaintball
            UPDATE [dbo].[WeeklyTotalRanking] 
            SET [Name] = @Name 
            WHERE [CMID] = @Cmid;
            
            SET @RowsAffected = @RowsAffected + @@ROWCOUNT;
            
            -- Update DailyRanking table in MvParadisePaintball
            UPDATE [dbo].[DailyRanking] 
            SET [Name] = @Name 
            WHERE [Cmid] = @Cmid;
            
            SET @RowsAffected = @RowsAffected + @@ROWCOUNT;
            
            -- Update Members table in Cmune database (using TagName field)
            UPDATE [Cmune].[dbo].[Members] 
            SET [TagName] = @Name 
            WHERE [CMID] = @Cmid;
            
            SET @RowsAffected = @RowsAffected + @@ROWCOUNT;
        END
        
        COMMIT TRANSACTION;
        
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
            
        -- Re-raise the error
        THROW;
    END CATCH
    
    RETURN @RowsAffected;
END
GO