USE [MvParadisePaintball];
GO

-- Variant #1: UsageType = 1 (All), Port = 5055
INSERT INTO dbo.PhotonServers (IP, Name, Region, PhotonsGroupID, MinLatency, UsageType, Port)
VALUES ('127.0.0.1', 'HaZard''s Server', 1, 1, 500, 6, 5055);
GO

-- Variant #2: UsageType = 6 (CommServer), Port = 5155
INSERT INTO dbo.PhotonServers (IP, Name, Region, PhotonsGroupID, MinLatency, UsageType, Port)
VALUES ('127.0.0.1', 'HaZard''s Server', 1, 1, 500, 1, 5155);
GO
