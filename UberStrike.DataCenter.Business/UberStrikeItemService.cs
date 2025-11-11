using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UberStrike.DataCenter.DataAccess;
using UberStrike.DataCenter.Common.Entities;
using UberStrike.Core.Types;

namespace UberStrike.DataCenter.Business
{
    public static class UberStrikeItemService
    {
        public static ItemQuickUseConfigView ToItemQuickUseConfigView(this ItemQuickUseConfig itemQuickUseConfig)
        {
            if (itemQuickUseConfig == null)
                return null;

            var itemQuickUseConfigView = new ItemQuickUseConfigView();

            itemQuickUseConfigView.ItemId = itemQuickUseConfig.ItemId;
            itemQuickUseConfigView.LevelRequired = itemQuickUseConfig.LevelRequired;
            itemQuickUseConfigView.UsesPerGame = itemQuickUseConfig.UsesPerGame;
            itemQuickUseConfigView.UsesPerLife = itemQuickUseConfig.UsesPerLife;
            itemQuickUseConfigView.UsesPerRound = itemQuickUseConfig.UsesPerRound;
            itemQuickUseConfigView.CoolDownTime = itemQuickUseConfig.CoolDownTime;
            itemQuickUseConfigView.WarmUpTime = itemQuickUseConfig.WarmUpTime;
            itemQuickUseConfigView.BehaviourType = (QuickItemLogic)itemQuickUseConfig.BehaviourType;

            return itemQuickUseConfigView;
        }


        public static ItemQuickUseConfig ToItemQuickUseConfig(this ItemQuickUseConfigView itemQuickUseConfigView)
        {
            if (itemQuickUseConfigView == null)
                return null;

            var itemQuickUseConfig = new ItemQuickUseConfig();
            itemQuickUseConfig.CopyFromItemQuickUseConfigView(itemQuickUseConfigView);

            return itemQuickUseConfig;
        }

        public static ItemQuickUseConfig CopyFromItemQuickUseConfigView(this ItemQuickUseConfig itemQuickUseConfig, ItemQuickUseConfigView itemQuickUseConfigView)
        {
            itemQuickUseConfig.UsesPerGame = itemQuickUseConfigView.UsesPerGame;
            itemQuickUseConfig.UsesPerLife = itemQuickUseConfigView.UsesPerLife;
            itemQuickUseConfig.UsesPerRound = itemQuickUseConfigView.UsesPerRound;
            itemQuickUseConfig.LevelRequired = itemQuickUseConfigView.LevelRequired;
            itemQuickUseConfig.ItemId = itemQuickUseConfigView.ItemId;
            itemQuickUseConfig.CoolDownTime = itemQuickUseConfigView.CoolDownTime;
            itemQuickUseConfig.WarmUpTime = itemQuickUseConfigView.WarmUpTime;
            itemQuickUseConfig.BehaviourType = (int)itemQuickUseConfigView.BehaviourType;

            return itemQuickUseConfig;
        }

        public static UberstrikeGearConfigView ToItemGearConfigView(this ItemGearConfig itemGearConfig)
        {
            if (itemGearConfig == null)
                return null;

            var itemGearConfigView = new UberstrikeGearConfigView();

            itemGearConfigView.ArmorAbsorptionPercent = itemGearConfig.ArmorAbsorptionPercent;
            itemGearConfigView.ArmorPoints = itemGearConfig.ArmorPoints;
            itemGearConfigView.ArmorWeight = itemGearConfig.ArmorWeight;
            itemGearConfigView.LevelRequired = itemGearConfig.LevelRequired;
            
            return itemGearConfigView;
        }



        public static UberstrikeFunctionalConfigView ToItemFunctionalConfigView(this ItemFunctionalConfig itemFunctionalConfig)
        {
            if (itemFunctionalConfig == null)
                return null;

            var uberstrikeFunctionalConfigView = new UberstrikeFunctionalConfigView();
            uberstrikeFunctionalConfigView.LevelRequired = itemFunctionalConfig.LevelRequired;
            return uberstrikeFunctionalConfigView;
        }


        public static UberstrikeWeaponConfigView ToItemWeaponConfigView(this ItemWeaponConfig itemWeaponConfig)
        {
            if (itemWeaponConfig == null)
                return null;

            var uberstrikeWeaponConfigView = new UberstrikeWeaponConfigView();

            uberstrikeWeaponConfigView.DamageKnockback = itemWeaponConfig.DamageKnockback;
            uberstrikeWeaponConfigView.DamagePerProjectile = itemWeaponConfig.DamagePerProjectile;
            uberstrikeWeaponConfigView.AccuracySpread = itemWeaponConfig.AccuracySpread;
            uberstrikeWeaponConfigView.RecoilKickback = itemWeaponConfig.RecoilKickback;
            uberstrikeWeaponConfigView.StartAmmo = itemWeaponConfig.StartAmmo;
            uberstrikeWeaponConfigView.MaxAmmo = itemWeaponConfig.MaxAmmo;
            uberstrikeWeaponConfigView.MissileTimeToDetonate = itemWeaponConfig.MissileTimeToDetonate;
            uberstrikeWeaponConfigView.MissileForceImpulse = itemWeaponConfig.MissileForceImpulse;
            uberstrikeWeaponConfigView.MissileBounciness = itemWeaponConfig.MissileBounciness;
            uberstrikeWeaponConfigView.SplashRadius = itemWeaponConfig.SplashRadius;
            uberstrikeWeaponConfigView.ProjectilesPerShot = itemWeaponConfig.ProjectilesPerShot;
            uberstrikeWeaponConfigView.ProjectileSpeed = itemWeaponConfig.ProjectileSpeed;
            uberstrikeWeaponConfigView.RateOfFire = itemWeaponConfig.RateOfFire;
            uberstrikeWeaponConfigView.RecoilMovement = itemWeaponConfig.RecoilMovement;
            uberstrikeWeaponConfigView.LevelRequired = itemWeaponConfig.LevelRequired;


            return uberstrikeWeaponConfigView;
        }


        public static UberstrikeSpecialConfigView ToItemSpecialConfigView(this ItemSpecialConfig itemSpecialConfig)
        {
            if (itemSpecialConfig == null)
                return null;
            var uberstrikeSpecialConfigView = new UberstrikeSpecialConfigView();
            uberstrikeSpecialConfigView.LevelRequired = itemSpecialConfig.LevelRequired;
            return uberstrikeSpecialConfigView;
        }

        public static UberstrikeWeaponModConfigView ToItemWeaponModConfigView(this ItemWeaponModConfig itemWeaponModConfig)
        {
            if (itemWeaponModConfig == null)
                return null;
            var uberstrikeWeaponModConfigView = new UberstrikeWeaponModConfigView();
            uberstrikeWeaponModConfigView.LevelRequired = itemWeaponModConfig.LevelRequired;
            return uberstrikeWeaponModConfigView;
        }

        public static ItemQuickUseConfigView ToItemQuickUseConfigViewOrInitialObject(this ItemQuickUseConfig itemQuickUseConfig)
        {
            var config = itemQuickUseConfig.ToItemQuickUseConfigView();
            if (config == null)
                return new ItemQuickUseConfigView();
            return config;
        }

        public static UberstrikeGearConfigView ToItemGearConfigViewOrInitialObject(this ItemGearConfig itemGearConfig)
        {
            var config = itemGearConfig.ToItemGearConfigView();
            if (config == null)
                return new UberstrikeGearConfigView();
            return config;
        }

        public static UberstrikeFunctionalConfigView ToItemFunctionalConfigViewOrInitialObject(this ItemFunctionalConfig itemFunctionalConfig)
        {
            var config = itemFunctionalConfig.ToItemFunctionalConfigView();
            if (config == null)
                return new UberstrikeFunctionalConfigView();
            return config;
        }

        public static UberstrikeWeaponConfigView ToItemWeaponConfigViewOrInitialObject(this ItemWeaponConfig itemWeaponConfig)
        {
            var config = itemWeaponConfig.ToItemWeaponConfigView();
            if (config == null)
                return new UberstrikeWeaponConfigView();
            return config;
        }

        public static UberstrikeSpecialConfigView ToItemSpecialConfigViewOrInitialObject(this ItemSpecialConfig itemSpecialConfig)
        {
            var config = itemSpecialConfig.ToItemSpecialConfigView();
            if (config == null)
                return new UberstrikeSpecialConfigView();
            return config;
        }


        public static UberstrikeWeaponModConfigView ToItemWeaponModConfigViewOrInitialObject(this ItemWeaponModConfig itemWeaponModConfig)
        {
            var config = itemWeaponModConfig.ToItemWeaponModConfigView();
            if (config == null)
                return new UberstrikeWeaponModConfigView();
            return config;
        }
    }
}
