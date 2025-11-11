BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.ItemQuickUseConfig ADD
	UsesPerLife int NOT NULL CONSTRAINT DF_ItemQuickUseConfig_UsesPerLife DEFAULT 0,
	UsesPerRound int NOT NULL CONSTRAINT DF_ItemQuickUseConfig_UsesPerRound DEFAULT 0,
	UsesPerGame int NOT NULL CONSTRAINT DF_ItemQuickUseConfig_UsesPerGame DEFAULT 0,
	CoolDownTime int NOT NULL CONSTRAINT DF_ItemQuickUseConfig_CoolDownTime DEFAULT 0,
	WarmUpTime int NOT NULL CONSTRAINT DF_ItemQuickUseConfig_FocusTime DEFAULT 0,
    BehaviourType int NOT NULL CONSTRAINT DF_ItemQuickUseConfig_Logic DEFAULT 0
GO
ALTER TABLE dbo.ItemQuickUseConfig SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.ItemQuickUseConfig', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.ItemQuickUseConfig', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.ItemQuickUseConfig', 'Object', 'CONTROL') as Contr_Per 