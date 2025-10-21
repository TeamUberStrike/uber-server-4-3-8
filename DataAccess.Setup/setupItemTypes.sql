SET IDENTITY_INSERT dbo.ItemType ON;

INSERT INTO dbo.ItemType (ItemTypeId, Name) VALUES (1, 'Weapon');
INSERT INTO dbo.ItemType (ItemTypeId, Name) VALUES (2, 'WeaponMod');
INSERT INTO dbo.ItemType (ItemTypeId, Name) VALUES (3, 'Gear');
INSERT INTO dbo.ItemType (ItemTypeId, Name) VALUES (4, 'QuickUse');
INSERT INTO dbo.ItemType (ItemTypeId, Name) VALUES (5, 'Functional');
INSERT INTO dbo.ItemType (ItemTypeId, Name) VALUES (6, 'Special');

SET IDENTITY_INSERT dbo.ItemType OFF;
