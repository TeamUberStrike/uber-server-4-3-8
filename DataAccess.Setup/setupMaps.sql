USE [MvParadisePaintball];
GO

INSERT INTO [dbo].[Maps] 
    ([MapId], [AppVersion], [DisplayName], [Description], [SceneName], [InUse], [IsBlueBox])
VALUES 
    (1, '4.3.8', 'Monkey Island', '', 'LevelMonkeyIsland', 1, 0),
    (2, '4.3.8', 'Lost Paradise 2', '', 'LevelLostParadise2', 1, 0),
    (3, '4.3.8', 'The Warehouse', '', 'LevelTheWarehouse', 1, 0),
    (4, '4.3.8', 'Temple Of The Raven', '', 'LevelTempleOfTheRaven', 1, 0),
    (5, '4.3.8', 'Fort Winter', '', 'LevelFortWinter', 1, 0),
    (6, '4.3.8', 'Gideons Tower', '', 'LevelGideonsTower', 1, 0),
    (7, '4.3.8', 'Sky Garden', '', 'LevelSkyGarden', 1, 0),
    (8, '4.3.8', 'Cuber Strike', '', 'LevelCuberStrike', 1, 0),
    (10, '4.3.8', 'Spaceport Alpha', '', 'LevelSpaceportAlpha', 1, 0);
GO
