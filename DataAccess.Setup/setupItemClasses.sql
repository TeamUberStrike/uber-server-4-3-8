SET IDENTITY_INSERT dbo.ItemClass ON;

INSERT INTO dbo.ItemClass (ItemClassId, Name, ItemTypeId) VALUES (1,  'WeaponMelee',         1);
INSERT INTO dbo.ItemClass (ItemClassId, Name, ItemTypeId) VALUES (2,  'WeaponHandgun',       1);
INSERT INTO dbo.ItemClass (ItemClassId, Name, ItemTypeId) VALUES (3,  'WeaponMachinegun',    1);
INSERT INTO dbo.ItemClass (ItemClassId, Name, ItemTypeId) VALUES (4,  'WeaponShotgun',       1);
INSERT INTO dbo.ItemClass (ItemClassId, Name, ItemTypeId) VALUES (5,  'WeaponSniperRifle',   1);
INSERT INTO dbo.ItemClass (ItemClassId, Name, ItemTypeId) VALUES (6,  'WeaponCannon',        1);
INSERT INTO dbo.ItemClass (ItemClassId, Name, ItemTypeId) VALUES (7,  'WeaponSplattergun',   1);
INSERT INTO dbo.ItemClass (ItemClassId, Name, ItemTypeId) VALUES (8,  'WeaponLauncher',      1);
INSERT INTO dbo.ItemClass (ItemClassId, Name, ItemTypeId) VALUES (9,  'WeaponModScope',      2);
INSERT INTO dbo.ItemClass (ItemClassId, Name, ItemTypeId) VALUES (10, 'WeaponModMuzzle',     2);
INSERT INTO dbo.ItemClass (ItemClassId, Name, ItemTypeId) VALUES (11, 'WeaponModWeaponMod',  2);
INSERT INTO dbo.ItemClass (ItemClassId, Name, ItemTypeId) VALUES (12, 'GearBoots',           3);
INSERT INTO dbo.ItemClass (ItemClassId, Name, ItemTypeId) VALUES (13, 'GearHead',            3);
INSERT INTO dbo.ItemClass (ItemClassId, Name, ItemTypeId) VALUES (14, 'GearFace',            3);
INSERT INTO dbo.ItemClass (ItemClassId, Name, ItemTypeId) VALUES (15, 'GearUpperBody',       3);
INSERT INTO dbo.ItemClass (ItemClassId, Name, ItemTypeId) VALUES (16, 'GearLowerBody',       3);
INSERT INTO dbo.ItemClass (ItemClassId, Name, ItemTypeId) VALUES (17, 'GearGloves',          3);
INSERT INTO dbo.ItemClass (ItemClassId, Name, ItemTypeId) VALUES (18, 'QuickUseGeneral',     4);
INSERT INTO dbo.ItemClass (ItemClassId, Name, ItemTypeId) VALUES (19, 'QuickUseGrenade',     4);
INSERT INTO dbo.ItemClass (ItemClassId, Name, ItemTypeId) VALUES (20, 'QuickUseMine',        4);
INSERT INTO dbo.ItemClass (ItemClassId, Name, ItemTypeId) VALUES (21, 'FunctionalGeneral',   5);
INSERT INTO dbo.ItemClass (ItemClassId, Name, ItemTypeId) VALUES (22, 'SpecialGeneral',      6);
INSERT INTO dbo.ItemClass (ItemClassId, Name, ItemTypeId) VALUES (23, 'GearHolo',            3);

SET IDENTITY_INSERT dbo.ItemClass OFF;
