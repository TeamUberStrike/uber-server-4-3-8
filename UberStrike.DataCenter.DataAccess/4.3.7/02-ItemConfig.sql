USE [MvParadisePaintball]

ALTER TABLE [MvParadisePaintball].[dbo].[ItemFunctionalConfig] ADD CONSTRAINT
[UQ_ItemFunctionalConfig_ItemId] UNIQUE NONCLUSTERED
(
    [ItemId]
)

ALTER TABLE [MvParadisePaintball].[dbo].[ItemGearConfig] ADD CONSTRAINT
[UQ_ItemGearConfig_ItemId] UNIQUE NONCLUSTERED
(
    [ItemId]
)

ALTER TABLE [MvParadisePaintball].[dbo].[ItemQuickUseConfig] ADD CONSTRAINT
[UQ_ItemQuickUseConfig_ItemId] UNIQUE NONCLUSTERED
(
    [ItemId]
)

ALTER TABLE [MvParadisePaintball].[dbo].[ItemSpecialConfig] ADD CONSTRAINT
[UQ_ItemSpecialConfig_ItemId] UNIQUE NONCLUSTERED
(
    [ItemId]
)

ALTER TABLE [MvParadisePaintball].[dbo].[ItemWeaponConfig] ADD CONSTRAINT
[UQ_ItemWeaponConfig_ItemId] UNIQUE NONCLUSTERED
(
    [ItemId]
)

ALTER TABLE [MvParadisePaintball].[dbo].[ItemWeaponModConfig] ADD CONSTRAINT
[UQ_ItemWeaponModConfig_ItemId] UNIQUE NONCLUSTERED
(
    [ItemId]
)