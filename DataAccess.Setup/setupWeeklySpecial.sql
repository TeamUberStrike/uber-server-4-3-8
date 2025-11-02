/****** Requires the image to be uploaded on the website ******/
USE [MvParadisePaintball];
GO

INSERT INTO [dbo].[WeeklySpecials] 
    ([PopupTitle], [PopupText], [ImageUrl], [ItemId], [StartDate], [EndDate])
VALUES 
    ('Weekly Special Ninja Holo',
     'Grab yours now!',
     'http://uberforever.eu/weeklyspecial/WeeklySpecial_NinjaHolo.jpg',
     1310,
     '2025-11-01',
     '2025-11-08');
GO
