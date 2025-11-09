USE [MvParadisePaintball];
GO

-- 1. First create the PhotonsGroup
INSERT INTO dbo.PhotonsGroups (Name, Description, CreationDate, ModificationDate)
VALUES ('LocalhostGroup', 'Localhost Photon Group', GETDATE(), GETDATE());
GO
