// IGNORED: UberstrikeItemView
// IGNORED: UberStrikeCommonConfig
// IGNORED: PlayerXPEventViewId
// IGNORED: UberstrikeAppSettings
// IGNORED: UberStrikeCacheKeys
// IGNORED: EsnsBasicStatisticView
// IGNORED: CyworldBasicStatisticView
// IGNORED: UberstrikeItemGearView
// IGNORED: UberstrikeItemFunctionalView
// IGNORED: PromotionContentElementViewModel
// IGNORED: UberstrikeItemQuickUseView
// IGNORED: UberstrikeWeaponModConfigView
// IGNORED: UberstrikeItemWeaponView
// IGNORED: MapInfoView
// IGNORED: FacebookBasicStatisticView
// IGNORED: UberStrikeAccountCompletionResult
// IGNORED: UberstrikeBuyItemResult
// IGNORED: UberStrikeGroupOperationResult
// IGNORED: BIT_FLAGS
// IGNORED: UberstrikeWeaponConfigView
// IGNORED: UberstrikeItemWeaponModView
// IGNORED: UberstrikeFunctionalConfigView
// IGNORED: RankingView
// IGNORED: LastIpView
// IGNORED: UberstrikeGearConfigView
// IGNORED: UberstrikeItemShopView
// IGNORED: UberstrikeSpecialConfigView
// IGNORED: ApplicationVersionViewModel
// IGNORED: UberstrikeItemSpecialView
// IGNORED: TutorialStepView
// IGNORED: MapVersionView
// IGNORED: MapClusterView
// IGNORED: LinkedMemberView
// IGNORED: RoomMetaData
// IGNORED: MapUsageView
// IGNORED: PromotionContentViewModel
using System.IO;

namespace UberStrike.Core.Serialization
{
	public static class UberStrikeItemWeaponViewProxy
	{
		public static void Serialize(Stream stream, UberStrike.Core.Models.Views.UberStrikeItemWeaponView instance)
		{
			int nullMask = 0;
			if(instance != null)
			{
				using (MemoryStream s = new MemoryStream())
                {
					Int32Proxy.Serialize(s, instance.AccuracySpread);
					if(instance.CustomProperties != null) DictionaryProxy<System.String, System.String>.Serialize(s, instance.CustomProperties, StringProxy.Serialize, StringProxy.Serialize);
					else nullMask |= 1 << 0; 
					Int32Proxy.Serialize(s, instance.DamageKnockback);
					Int32Proxy.Serialize(s, instance.DamagePerProjectile);
					if(instance.Description != null) StringProxy.Serialize(s, instance.Description);
					else nullMask |= 1 << 1; 
					Int32Proxy.Serialize(s, instance.ID);
					BooleanProxy.Serialize(s, instance.IsConsumable);
					EnumProxy<UberStrike.Core.Types.UberstrikeItemClass>.Serialize(s, instance.ItemClass);
					Int32Proxy.Serialize(s, instance.LevelLock);
					Int32Proxy.Serialize(s, instance.MaxAmmo);
					Int32Proxy.Serialize(s, instance.MissileBounciness);
					Int32Proxy.Serialize(s, instance.MissileForceImpulse);
					Int32Proxy.Serialize(s, instance.MissileTimeToDetonate);
					if(instance.Name != null) StringProxy.Serialize(s, instance.Name);
					else nullMask |= 1 << 2; 
					if(instance.Prices != null) ListProxy<UberStrike.Core.Models.Views.ItemPrice>.Serialize(s, instance.Prices, ItemPriceProxy.Serialize);
					else nullMask |= 1 << 3; 
					Int32Proxy.Serialize(s, instance.ProjectileSpeed);
					Int32Proxy.Serialize(s, instance.ProjectilesPerShot);
					Int32Proxy.Serialize(s, instance.RateOfFire);
					Int32Proxy.Serialize(s, instance.RecoilKickback);
					Int32Proxy.Serialize(s, instance.RecoilMovement);
					EnumProxy<UberStrike.Core.Types.ItemShopHighlightType>.Serialize(s, instance.ShopHighlightType);
					Int32Proxy.Serialize(s, instance.SplashRadius);
					Int32Proxy.Serialize(s, instance.StartAmmo);

					Int32Proxy.Serialize(stream, ~nullMask);
					s.WriteTo(stream);
				}
			}
			else
			{
				Int32Proxy.Serialize(stream, 0);
			}
		}
		
		public static UberStrike.Core.Models.Views.UberStrikeItemWeaponView Deserialize(Stream bytes)
        {
			int nullMask = Int32Proxy.Deserialize(bytes);
			
			UberStrike.Core.Models.Views.UberStrikeItemWeaponView instance = null;
			if(nullMask != 0)
			{
            	instance = new UberStrike.Core.Models.Views.UberStrikeItemWeaponView();
				instance.AccuracySpread = Int32Proxy.Deserialize(bytes);
				if((nullMask & (1 << 0)) != 0) instance.CustomProperties = DictionaryProxy<System.String, System.String>.Deserialize(bytes, StringProxy.Deserialize, StringProxy.Deserialize);
				instance.DamageKnockback = Int32Proxy.Deserialize(bytes);
				instance.DamagePerProjectile = Int32Proxy.Deserialize(bytes);
				if((nullMask & (1 << 1)) != 0) instance.Description = StringProxy.Deserialize(bytes);
				instance.ID = Int32Proxy.Deserialize(bytes);
				instance.IsConsumable = BooleanProxy.Deserialize(bytes);
				instance.ItemClass = EnumProxy<UberStrike.Core.Types.UberstrikeItemClass>.Deserialize(bytes);
				instance.LevelLock = Int32Proxy.Deserialize(bytes);
				instance.MaxAmmo = Int32Proxy.Deserialize(bytes);
				instance.MissileBounciness = Int32Proxy.Deserialize(bytes);
				instance.MissileForceImpulse = Int32Proxy.Deserialize(bytes);
				instance.MissileTimeToDetonate = Int32Proxy.Deserialize(bytes);
				if((nullMask & (1 << 2)) != 0) instance.Name = StringProxy.Deserialize(bytes);
				if((nullMask & (1 << 3)) != 0) instance.Prices = ListProxy<UberStrike.Core.Models.Views.ItemPrice>.Deserialize(bytes, ItemPriceProxy.Deserialize);
				instance.ProjectileSpeed = Int32Proxy.Deserialize(bytes);
				instance.ProjectilesPerShot = Int32Proxy.Deserialize(bytes);
				instance.RateOfFire = Int32Proxy.Deserialize(bytes);
				instance.RecoilKickback = Int32Proxy.Deserialize(bytes);
				instance.RecoilMovement = Int32Proxy.Deserialize(bytes);
				instance.ShopHighlightType = EnumProxy<UberStrike.Core.Types.ItemShopHighlightType>.Deserialize(bytes);
				instance.SplashRadius = Int32Proxy.Deserialize(bytes);
				instance.StartAmmo = Int32Proxy.Deserialize(bytes);
			}
			
			return instance;
		}
	}
	public static class UberstrikeMemberViewProxy
	{
		public static void Serialize(Stream stream, UberStrike.DataCenter.Common.Entities.UberstrikeMemberView instance)
		{
			int nullMask = 0;
			if(instance != null)
			{
				using (MemoryStream s = new MemoryStream())
                {
					if(instance.PlayerCardView != null) PlayerCardViewProxy.Serialize(s, instance.PlayerCardView);
					else nullMask |= 1 << 0; 
					if(instance.PlayerStatisticsView != null) PlayerStatisticsViewProxy.Serialize(s, instance.PlayerStatisticsView);
					else nullMask |= 1 << 1; 

					Int32Proxy.Serialize(stream, ~nullMask);
					s.WriteTo(stream);
				}
			}
			else
			{
				Int32Proxy.Serialize(stream, 0);
			}
		}
		
		public static UberStrike.DataCenter.Common.Entities.UberstrikeMemberView Deserialize(Stream bytes)
        {
			int nullMask = Int32Proxy.Deserialize(bytes);
			
			UberStrike.DataCenter.Common.Entities.UberstrikeMemberView instance = null;
			if(nullMask != 0)
			{
            	instance = new UberStrike.DataCenter.Common.Entities.UberstrikeMemberView();
				if((nullMask & (1 << 0)) != 0) instance.PlayerCardView = PlayerCardViewProxy.Deserialize(bytes);
				if((nullMask & (1 << 1)) != 0) instance.PlayerStatisticsView = PlayerStatisticsViewProxy.Deserialize(bytes);
			}
			
			return instance;
		}
	}
	public static class PointDepositsViewModelProxy
	{
		public static void Serialize(Stream stream, UberStrike.Core.ViewModel.PointDepositsViewModel instance)
		{
			int nullMask = 0;
			if(instance != null)
			{
				using (MemoryStream s = new MemoryStream())
                {
					if(instance.PointDeposits != null) ListProxy<Cmune.DataCenter.Common.Entities.PointDepositView>.Serialize(s, instance.PointDeposits, PointDepositViewProxy.Serialize);
					else nullMask |= 1 << 0; 
					Int32Proxy.Serialize(s, instance.TotalCount);

					Int32Proxy.Serialize(stream, ~nullMask);
					s.WriteTo(stream);
				}
			}
			else
			{
				Int32Proxy.Serialize(stream, 0);
			}
		}
		
		public static UberStrike.Core.ViewModel.PointDepositsViewModel Deserialize(Stream bytes)
        {
			int nullMask = Int32Proxy.Deserialize(bytes);
			
			UberStrike.Core.ViewModel.PointDepositsViewModel instance = null;
			if(nullMask != 0)
			{
            	instance = new UberStrike.Core.ViewModel.PointDepositsViewModel();
				if((nullMask & (1 << 0)) != 0) instance.PointDeposits = ListProxy<Cmune.DataCenter.Common.Entities.PointDepositView>.Deserialize(bytes, PointDepositViewProxy.Deserialize);
				instance.TotalCount = Int32Proxy.Deserialize(bytes);
			}
			
			return instance;
		}
	}
	public static class PlayerXPEventViewProxy
	{
		public static void Serialize(Stream stream, UberStrike.DataCenter.Common.Entities.PlayerXPEventView instance)
		{
			int nullMask = 0;
			if(instance != null)
			{
				using (MemoryStream s = new MemoryStream())
                {
					if(instance.Name != null) StringProxy.Serialize(s, instance.Name);
					else nullMask |= 1 << 0; 
					Int32Proxy.Serialize(s, instance.PlayerXPEventId);
					DecimalProxy.Serialize(s, instance.XPMultiplier);

					Int32Proxy.Serialize(stream, ~nullMask);
					s.WriteTo(stream);
				}
			}
			else
			{
				Int32Proxy.Serialize(stream, 0);
			}
		}
		
		public static UberStrike.DataCenter.Common.Entities.PlayerXPEventView Deserialize(Stream bytes)
        {
			int nullMask = Int32Proxy.Deserialize(bytes);
			
			UberStrike.DataCenter.Common.Entities.PlayerXPEventView instance = null;
			if(nullMask != 0)
			{
            	instance = new UberStrike.DataCenter.Common.Entities.PlayerXPEventView();
				if((nullMask & (1 << 0)) != 0) instance.Name = StringProxy.Deserialize(bytes);
				instance.PlayerXPEventId = Int32Proxy.Deserialize(bytes);
				instance.XPMultiplier = DecimalProxy.Deserialize(bytes);
			}
			
			return instance;
		}
	}
	public static class CmuneRoomIDProxy
	{
		public static void Serialize(Stream stream, UberStrike.Core.Models.CmuneRoomID instance)
		{
			int nullMask = 0;
			if(instance != null)
			{
				using (MemoryStream s = new MemoryStream())
                {
					Int32Proxy.Serialize(s, instance.RoomNumber);
					if(instance.Server != null) ConnectionAddressProxy.Serialize(s, instance.Server);
					else nullMask |= 1 << 0; 

					Int32Proxy.Serialize(stream, ~nullMask);
					s.WriteTo(stream);
				}
			}
			else
			{
				Int32Proxy.Serialize(stream, 0);
			}
		}
		
		public static UberStrike.Core.Models.CmuneRoomID Deserialize(Stream bytes)
        {
			int nullMask = Int32Proxy.Deserialize(bytes);
			
			UberStrike.Core.Models.CmuneRoomID instance = null;
			if(nullMask != 0)
			{
            	instance = new UberStrike.Core.Models.CmuneRoomID();
				instance.RoomNumber = Int32Proxy.Deserialize(bytes);
				if((nullMask & (1 << 0)) != 0) instance.Server = ConnectionAddressProxy.Deserialize(bytes);
			}
			
			return instance;
		}
	}
	public static class ItemPriceProxy
	{
		public static void Serialize(Stream stream, UberStrike.Core.Models.Views.ItemPrice instance)
		{
			int nullMask = 0;
			if(instance != null)
			{
				using (MemoryStream s = new MemoryStream())
                {
					Int32Proxy.Serialize(s, instance.Amount);
					EnumProxy<Cmune.DataCenter.Common.Entities.UberStrikeCurrencyType>.Serialize(s, instance.Currency);
					Int32Proxy.Serialize(s, instance.Discount);
					EnumProxy<Cmune.DataCenter.Common.Entities.BuyingDurationType>.Serialize(s, instance.Duration);
					EnumProxy<Cmune.DataCenter.Common.Entities.PackType>.Serialize(s, instance.PackType);
					Int32Proxy.Serialize(s, instance.Price);

					Int32Proxy.Serialize(stream, ~nullMask);
					s.WriteTo(stream);
				}
			}
			else
			{
				Int32Proxy.Serialize(stream, 0);
			}
		}
		
		public static UberStrike.Core.Models.Views.ItemPrice Deserialize(Stream bytes)
        {
			int nullMask = Int32Proxy.Deserialize(bytes);
			
			UberStrike.Core.Models.Views.ItemPrice instance = null;
			if(nullMask != 0)
			{
            	instance = new UberStrike.Core.Models.Views.ItemPrice();
				instance.Amount = Int32Proxy.Deserialize(bytes);
				instance.Currency = EnumProxy<Cmune.DataCenter.Common.Entities.UberStrikeCurrencyType>.Deserialize(bytes);
				instance.Discount = Int32Proxy.Deserialize(bytes);
				instance.Duration = EnumProxy<Cmune.DataCenter.Common.Entities.BuyingDurationType>.Deserialize(bytes);
				instance.PackType = EnumProxy<Cmune.DataCenter.Common.Entities.PackType>.Deserialize(bytes);
				instance.Price = Int32Proxy.Deserialize(bytes);
			}
			
			return instance;
		}
	}
	public static class AccountCompletionResultViewProxy
	{
		public static void Serialize(Stream stream, UberStrike.DataCenter.Common.Entities.AccountCompletionResultView instance)
		{
			int nullMask = 0;
			if(instance != null)
			{
				using (MemoryStream s = new MemoryStream())
                {
					if(instance.ItemsAttributed != null) DictionaryProxy<System.Int32, System.Int32>.Serialize(s, instance.ItemsAttributed, Int32Proxy.Serialize, Int32Proxy.Serialize);
					else nullMask |= 1 << 0; 
					if(instance.NonDuplicateNames != null) ListProxy<System.String>.Serialize(s, instance.NonDuplicateNames, StringProxy.Serialize);
					else nullMask |= 1 << 1; 
					Int32Proxy.Serialize(s, instance.Result);

					Int32Proxy.Serialize(stream, ~nullMask);
					s.WriteTo(stream);
				}
			}
			else
			{
				Int32Proxy.Serialize(stream, 0);
			}
		}
		
		public static UberStrike.DataCenter.Common.Entities.AccountCompletionResultView Deserialize(Stream bytes)
        {
			int nullMask = Int32Proxy.Deserialize(bytes);
			
			UberStrike.DataCenter.Common.Entities.AccountCompletionResultView instance = null;
			if(nullMask != 0)
			{
            	instance = new UberStrike.DataCenter.Common.Entities.AccountCompletionResultView();
				if((nullMask & (1 << 0)) != 0) instance.ItemsAttributed = DictionaryProxy<System.Int32, System.Int32>.Deserialize(bytes, Int32Proxy.Deserialize, Int32Proxy.Deserialize);
				if((nullMask & (1 << 1)) != 0) instance.NonDuplicateNames = ListProxy<System.String>.Deserialize(bytes, StringProxy.Deserialize);
				instance.Result = Int32Proxy.Deserialize(bytes);
			}
			
			return instance;
		}
	}
	public static class ActorInfoProxy
	{
		public static void Serialize(Stream stream, UberStrike.Core.Models.ActorInfo instance)
		{
			int nullMask = 0;
			if(instance != null)
			{
				using (MemoryStream s = new MemoryStream())
                {
					EnumProxy<Cmune.DataCenter.Common.Entities.MemberAccessLevel>.Serialize(s, instance.AccessLevel);
					EnumProxy<Cmune.DataCenter.Common.Entities.ChannelType>.Serialize(s, instance.Channel);
					if(instance.ClanTag != null) StringProxy.Serialize(s, instance.ClanTag);
					else nullMask |= 1 << 0; 
					Int32Proxy.Serialize(s, instance.Cmid);
					if(instance.CurrentRoom != null) CmuneRoomIDProxy.Serialize(s, instance.CurrentRoom);
					else nullMask |= 1 << 1; 
					UInt16Proxy.Serialize(s, instance.Ping);
					if(instance.PlayerName != null) StringProxy.Serialize(s, instance.PlayerName);
					else nullMask |= 1 << 2; 

					Int32Proxy.Serialize(stream, ~nullMask);
					s.WriteTo(stream);
				}
			}
			else
			{
				Int32Proxy.Serialize(stream, 0);
			}
		}
		
		public static UberStrike.Core.Models.ActorInfo Deserialize(Stream bytes)
        {
			int nullMask = Int32Proxy.Deserialize(bytes);
			
			UberStrike.Core.Models.ActorInfo instance = null;
			if(nullMask != 0)
			{
            	instance = new UberStrike.Core.Models.ActorInfo();
				instance.AccessLevel = EnumProxy<Cmune.DataCenter.Common.Entities.MemberAccessLevel>.Deserialize(bytes);
				instance.Channel = EnumProxy<Cmune.DataCenter.Common.Entities.ChannelType>.Deserialize(bytes);
				if((nullMask & (1 << 0)) != 0) instance.ClanTag = StringProxy.Deserialize(bytes);
				instance.Cmid = Int32Proxy.Deserialize(bytes);
				if((nullMask & (1 << 1)) != 0) instance.CurrentRoom = CmuneRoomIDProxy.Deserialize(bytes);
				instance.Ping = UInt16Proxy.Deserialize(bytes);
				if((nullMask & (1 << 2)) != 0) instance.PlayerName = StringProxy.Deserialize(bytes);
			}
			
			return instance;
		}
	}
	public static class CommActorInfoProxy
	{
		public static void Serialize(Stream stream, UberStrike.Core.Models.CommActorInfo instance)
		{
			int nullMask = 0;
			if(instance != null)
			{
				using (MemoryStream s = new MemoryStream())
                {
					EnumProxy<Cmune.DataCenter.Common.Entities.MemberAccessLevel>.Serialize(s, instance.AccessLevel);
					EnumProxy<Cmune.DataCenter.Common.Entities.ChannelType>.Serialize(s, instance.Channel);
					if(instance.ClanTag != null) StringProxy.Serialize(s, instance.ClanTag);
					else nullMask |= 1 << 0; 
					Int32Proxy.Serialize(s, instance.Cmid);
					if(instance.CurrentRoom != null) CmuneRoomIDProxy.Serialize(s, instance.CurrentRoom);
					else nullMask |= 1 << 1; 
					ByteProxy.Serialize(s, instance.ModerationFlag);
					if(instance.ModInformation != null) StringProxy.Serialize(s, instance.ModInformation);
					else nullMask |= 1 << 2; 
					UInt16Proxy.Serialize(s, instance.Ping);
					if(instance.PlayerName != null) StringProxy.Serialize(s, instance.PlayerName);
					else nullMask |= 1 << 3; 

					Int32Proxy.Serialize(stream, ~nullMask);
					s.WriteTo(stream);
				}
			}
			else
			{
				Int32Proxy.Serialize(stream, 0);
			}
		}
		
		public static UberStrike.Core.Models.CommActorInfo Deserialize(Stream bytes)
        {
			int nullMask = Int32Proxy.Deserialize(bytes);
			
			UberStrike.Core.Models.CommActorInfo instance = null;
			if(nullMask != 0)
			{
            	instance = new UberStrike.Core.Models.CommActorInfo();
				instance.AccessLevel = EnumProxy<Cmune.DataCenter.Common.Entities.MemberAccessLevel>.Deserialize(bytes);
				instance.Channel = EnumProxy<Cmune.DataCenter.Common.Entities.ChannelType>.Deserialize(bytes);
				if((nullMask & (1 << 0)) != 0) instance.ClanTag = StringProxy.Deserialize(bytes);
				instance.Cmid = Int32Proxy.Deserialize(bytes);
				if((nullMask & (1 << 1)) != 0) instance.CurrentRoom = CmuneRoomIDProxy.Deserialize(bytes);
				instance.ModerationFlag = ByteProxy.Deserialize(bytes);
				if((nullMask & (1 << 2)) != 0) instance.ModInformation = StringProxy.Deserialize(bytes);
				instance.Ping = UInt16Proxy.Deserialize(bytes);
				if((nullMask & (1 << 3)) != 0) instance.PlayerName = StringProxy.Deserialize(bytes);
			}
			
			return instance;
		}
	}
	public static class MemberAuthenticationViewModelProxy
	{
		public static void Serialize(Stream stream, UberStrike.Core.ViewModel.MemberAuthenticationViewModel instance)
		{
			int nullMask = 0;
			if(instance != null)
			{
				using (MemoryStream s = new MemoryStream())
                {
					EnumProxy<Cmune.DataCenter.Common.Entities.MemberAuthenticationResult>.Serialize(s, instance.MemberAuthenticationResult);
					if(instance.MemberView != null) MemberViewProxy.Serialize(s, instance.MemberView);
					else nullMask |= 1 << 0; 

					Int32Proxy.Serialize(stream, ~nullMask);
					s.WriteTo(stream);
				}
			}
			else
			{
				Int32Proxy.Serialize(stream, 0);
			}
		}
		
		public static UberStrike.Core.ViewModel.MemberAuthenticationViewModel Deserialize(Stream bytes)
        {
			int nullMask = Int32Proxy.Deserialize(bytes);
			
			UberStrike.Core.ViewModel.MemberAuthenticationViewModel instance = null;
			if(nullMask != 0)
			{
            	instance = new UberStrike.Core.ViewModel.MemberAuthenticationViewModel();
				instance.MemberAuthenticationResult = EnumProxy<Cmune.DataCenter.Common.Entities.MemberAuthenticationResult>.Deserialize(bytes);
				if((nullMask & (1 << 0)) != 0) instance.MemberView = MemberViewProxy.Deserialize(bytes);
			}
			
			return instance;
		}
	}
	public static class UberstrikeUserViewModelProxy
	{
		public static void Serialize(Stream stream, UberStrike.Core.ViewModel.UberstrikeUserViewModel instance)
		{
			int nullMask = 0;
			if(instance != null)
			{
				using (MemoryStream s = new MemoryStream())
                {
					if(instance.CmuneMemberView != null) MemberViewProxy.Serialize(s, instance.CmuneMemberView);
					else nullMask |= 1 << 0; 
					if(instance.UberstrikeMemberView != null) UberstrikeMemberViewProxy.Serialize(s, instance.UberstrikeMemberView);
					else nullMask |= 1 << 1; 

					Int32Proxy.Serialize(stream, ~nullMask);
					s.WriteTo(stream);
				}
			}
			else
			{
				Int32Proxy.Serialize(stream, 0);
			}
		}
		
		public static UberStrike.Core.ViewModel.UberstrikeUserViewModel Deserialize(Stream bytes)
        {
			int nullMask = Int32Proxy.Deserialize(bytes);
			
			UberStrike.Core.ViewModel.UberstrikeUserViewModel instance = null;
			if(nullMask != 0)
			{
            	instance = new UberStrike.Core.ViewModel.UberstrikeUserViewModel();
				if((nullMask & (1 << 0)) != 0) instance.CmuneMemberView = MemberViewProxy.Deserialize(bytes);
				if((nullMask & (1 << 1)) != 0) instance.UberstrikeMemberView = UberstrikeMemberViewProxy.Deserialize(bytes);
			}
			
			return instance;
		}
	}
	public static class MapViewProxy
	{
		public static void Serialize(Stream stream, UberStrike.Core.Models.Views.MapView instance)
		{
			int nullMask = 0;
			if(instance != null)
			{
				using (MemoryStream s = new MemoryStream())
                {
					if(instance.Description != null) StringProxy.Serialize(s, instance.Description);
					else nullMask |= 1 << 0; 
					if(instance.DisplayName != null) StringProxy.Serialize(s, instance.DisplayName);
					else nullMask |= 1 << 1; 
					if(instance.FileName != null) StringProxy.Serialize(s, instance.FileName);
					else nullMask |= 1 << 2; 
					BooleanProxy.Serialize(s, instance.IsBlueBox);
					Int32Proxy.Serialize(s, instance.MapId);
					Int32Proxy.Serialize(s, instance.MaxPlayers);
					Int32Proxy.Serialize(s, instance.RecommendedItemId);
					if(instance.SceneName != null) StringProxy.Serialize(s, instance.SceneName);
					else nullMask |= 1 << 3; 
					if(instance.Settings != null) DictionaryProxy<UberStrike.Core.Types.GameModeType, UberStrike.Core.Models.Views.MapSettings>.Serialize(s, instance.Settings, EnumProxy<UberStrike.Core.Types.GameModeType>.Serialize, MapSettingsProxy.Serialize);
					else nullMask |= 1 << 4; 
					Int32Proxy.Serialize(s, instance.SupportedGameModes);

					Int32Proxy.Serialize(stream, ~nullMask);
					s.WriteTo(stream);
				}
			}
			else
			{
				Int32Proxy.Serialize(stream, 0);
			}
		}
		
		public static UberStrike.Core.Models.Views.MapView Deserialize(Stream bytes)
        {
			int nullMask = Int32Proxy.Deserialize(bytes);
			
			UberStrike.Core.Models.Views.MapView instance = null;
			if(nullMask != 0)
			{
            	instance = new UberStrike.Core.Models.Views.MapView();
				if((nullMask & (1 << 0)) != 0) instance.Description = StringProxy.Deserialize(bytes);
				if((nullMask & (1 << 1)) != 0) instance.DisplayName = StringProxy.Deserialize(bytes);
				if((nullMask & (1 << 2)) != 0) instance.FileName = StringProxy.Deserialize(bytes);
				instance.IsBlueBox = BooleanProxy.Deserialize(bytes);
				instance.MapId = Int32Proxy.Deserialize(bytes);
				instance.MaxPlayers = Int32Proxy.Deserialize(bytes);
				instance.RecommendedItemId = Int32Proxy.Deserialize(bytes);
				if((nullMask & (1 << 3)) != 0) instance.SceneName = StringProxy.Deserialize(bytes);
				if((nullMask & (1 << 4)) != 0) instance.Settings = DictionaryProxy<UberStrike.Core.Types.GameModeType, UberStrike.Core.Models.Views.MapSettings>.Deserialize(bytes, EnumProxy<UberStrike.Core.Types.GameModeType>.Deserialize, MapSettingsProxy.Deserialize);
				instance.SupportedGameModes = Int32Proxy.Deserialize(bytes);
			}
			
			return instance;
		}
	}
	public static class WeeklySpecialViewProxy
	{
		public static void Serialize(Stream stream, UberStrike.DataCenter.Common.Entities.WeeklySpecialView instance)
		{
			int nullMask = 0;
			if(instance != null)
			{
				using (MemoryStream s = new MemoryStream())
                {
					if(instance.EndDate != null) DateTimeProxy.Serialize(s, instance.EndDate ?? default(System.DateTime));
					else nullMask |= 1 << 0; 
					Int32Proxy.Serialize(s, instance.Id);
					if(instance.ImageUrl != null) StringProxy.Serialize(s, instance.ImageUrl);
					else nullMask |= 1 << 1; 
					Int32Proxy.Serialize(s, instance.ItemId);
					DateTimeProxy.Serialize(s, instance.StartDate);
					if(instance.Text != null) StringProxy.Serialize(s, instance.Text);
					else nullMask |= 1 << 2; 
					if(instance.Title != null) StringProxy.Serialize(s, instance.Title);
					else nullMask |= 1 << 3; 

					Int32Proxy.Serialize(stream, ~nullMask);
					s.WriteTo(stream);
				}
			}
			else
			{
				Int32Proxy.Serialize(stream, 0);
			}
		}
		
		public static UberStrike.DataCenter.Common.Entities.WeeklySpecialView Deserialize(Stream bytes)
        {
			int nullMask = Int32Proxy.Deserialize(bytes);
			
			UberStrike.DataCenter.Common.Entities.WeeklySpecialView instance = null;
			if(nullMask != 0)
			{
            	instance = new UberStrike.DataCenter.Common.Entities.WeeklySpecialView();
				if((nullMask & (1 << 0)) != 0) instance.EndDate = DateTimeProxy.Deserialize(bytes);
				instance.Id = Int32Proxy.Deserialize(bytes);
				if((nullMask & (1 << 1)) != 0) instance.ImageUrl = StringProxy.Deserialize(bytes);
				instance.ItemId = Int32Proxy.Deserialize(bytes);
				instance.StartDate = DateTimeProxy.Deserialize(bytes);
				if((nullMask & (1 << 2)) != 0) instance.Text = StringProxy.Deserialize(bytes);
				if((nullMask & (1 << 3)) != 0) instance.Title = StringProxy.Deserialize(bytes);
			}
			
			return instance;
		}
	}
	public static class DailyPointsViewProxy
	{
		public static void Serialize(Stream stream, UberStrike.DataCenter.Common.Entities.DailyPointsView instance)
		{
			int nullMask = 0;
			if(instance != null)
			{
				using (MemoryStream s = new MemoryStream())
                {
					Int32Proxy.Serialize(s, instance.Current);
					Int32Proxy.Serialize(s, instance.PointsMax);
					Int32Proxy.Serialize(s, instance.PointsTomorrow);

					Int32Proxy.Serialize(stream, ~nullMask);
					s.WriteTo(stream);
				}
			}
			else
			{
				Int32Proxy.Serialize(stream, 0);
			}
		}
		
		public static UberStrike.DataCenter.Common.Entities.DailyPointsView Deserialize(Stream bytes)
        {
			int nullMask = Int32Proxy.Deserialize(bytes);
			
			UberStrike.DataCenter.Common.Entities.DailyPointsView instance = null;
			if(nullMask != 0)
			{
            	instance = new UberStrike.DataCenter.Common.Entities.DailyPointsView();
				instance.Current = Int32Proxy.Deserialize(bytes);
				instance.PointsMax = Int32Proxy.Deserialize(bytes);
				instance.PointsTomorrow = Int32Proxy.Deserialize(bytes);
			}
			
			return instance;
		}
	}
	public static class ConnectionAddressProxy
	{
		public static void Serialize(Stream stream, UberStrike.Core.Models.ConnectionAddress instance)
		{
			int nullMask = 0;
			if(instance != null)
			{
				using (MemoryStream s = new MemoryStream())
                {
					Int32Proxy.Serialize(s, instance.Ipv4);
					Int16Proxy.Serialize(s, instance.Port);

					Int32Proxy.Serialize(stream, ~nullMask);
					s.WriteTo(stream);
				}
			}
			else
			{
				Int32Proxy.Serialize(stream, 0);
			}
		}
		
		public static UberStrike.Core.Models.ConnectionAddress Deserialize(Stream bytes)
        {
			int nullMask = Int32Proxy.Deserialize(bytes);
			
			UberStrike.Core.Models.ConnectionAddress instance = null;
			if(nullMask != 0)
			{
            	instance = new UberStrike.Core.Models.ConnectionAddress();
				instance.Ipv4 = Int32Proxy.Deserialize(bytes);
				instance.Port = Int16Proxy.Deserialize(bytes);
			}
			
			return instance;
		}
	}
	public static class MemberAuthenticationResultViewProxy
	{
		public static void Serialize(Stream stream, UberStrike.Core.ViewModel.MemberAuthenticationResultView instance)
		{
			int nullMask = 0;
			if(instance != null)
			{
				using (MemoryStream s = new MemoryStream())
                {
					BooleanProxy.Serialize(s, instance.IsAccountComplete);
					BooleanProxy.Serialize(s, instance.IsTutorialComplete);
					if(instance.LuckyDraw != null) LuckyDrawUnityViewProxy.Serialize(s, instance.LuckyDraw);
					else nullMask |= 1 << 0; 
					EnumProxy<Cmune.DataCenter.Common.Entities.MemberAuthenticationResult>.Serialize(s, instance.MemberAuthenticationResult);
					if(instance.MemberView != null) MemberViewProxy.Serialize(s, instance.MemberView);
					else nullMask |= 1 << 1; 
					if(instance.PlayerStatisticsView != null) PlayerStatisticsViewProxy.Serialize(s, instance.PlayerStatisticsView);
					else nullMask |= 1 << 2; 
					DateTimeProxy.Serialize(s, instance.ServerTime);
					if(instance.WeeklySpecial != null) WeeklySpecialViewProxy.Serialize(s, instance.WeeklySpecial);
					else nullMask |= 1 << 3; 

					Int32Proxy.Serialize(stream, ~nullMask);
					s.WriteTo(stream);
				}
			}
			else
			{
				Int32Proxy.Serialize(stream, 0);
			}
		}
		
		public static UberStrike.Core.ViewModel.MemberAuthenticationResultView Deserialize(Stream bytes)
        {
			int nullMask = Int32Proxy.Deserialize(bytes);
			
			UberStrike.Core.ViewModel.MemberAuthenticationResultView instance = null;
			if(nullMask != 0)
			{
            	instance = new UberStrike.Core.ViewModel.MemberAuthenticationResultView();
				instance.IsAccountComplete = BooleanProxy.Deserialize(bytes);
				instance.IsTutorialComplete = BooleanProxy.Deserialize(bytes);
				if((nullMask & (1 << 0)) != 0) instance.LuckyDraw = LuckyDrawUnityViewProxy.Deserialize(bytes);
				instance.MemberAuthenticationResult = EnumProxy<Cmune.DataCenter.Common.Entities.MemberAuthenticationResult>.Deserialize(bytes);
				if((nullMask & (1 << 1)) != 0) instance.MemberView = MemberViewProxy.Deserialize(bytes);
				if((nullMask & (1 << 2)) != 0) instance.PlayerStatisticsView = PlayerStatisticsViewProxy.Deserialize(bytes);
				instance.ServerTime = DateTimeProxy.Deserialize(bytes);
				if((nullMask & (1 << 3)) != 0) instance.WeeklySpecial = WeeklySpecialViewProxy.Deserialize(bytes);
			}
			
			return instance;
		}
	}
	public static class GameApplicationViewProxy
	{
		public static void Serialize(Stream stream, UberStrike.Core.ViewModel.GameApplicationView instance)
		{
			int nullMask = 0;
			if(instance != null)
			{
				using (MemoryStream s = new MemoryStream())
                {
					if(instance.CommServer != null) PhotonViewProxy.Serialize(s, instance.CommServer);
					else nullMask |= 1 << 0; 
					if(instance.EncryptionInitVector != null) StringProxy.Serialize(s, instance.EncryptionInitVector);
					else nullMask |= 1 << 1; 
					if(instance.EncryptionPassPhrase != null) StringProxy.Serialize(s, instance.EncryptionPassPhrase);
					else nullMask |= 1 << 2; 
					if(instance.GameServers != null) ListProxy<Cmune.Core.Models.Views.PhotonView>.Serialize(s, instance.GameServers, PhotonViewProxy.Serialize);
					else nullMask |= 1 << 3; 
					if(instance.SupportUrl != null) StringProxy.Serialize(s, instance.SupportUrl);
					else nullMask |= 1 << 4; 
					if(instance.Version != null) StringProxy.Serialize(s, instance.Version);
					else nullMask |= 1 << 5; 

					Int32Proxy.Serialize(stream, ~nullMask);
					s.WriteTo(stream);
				}
			}
			else
			{
				Int32Proxy.Serialize(stream, 0);
			}
		}
		
		public static UberStrike.Core.ViewModel.GameApplicationView Deserialize(Stream bytes)
        {
			int nullMask = Int32Proxy.Deserialize(bytes);
			
			UberStrike.Core.ViewModel.GameApplicationView instance = null;
			if(nullMask != 0)
			{
            	instance = new UberStrike.Core.ViewModel.GameApplicationView();
				if((nullMask & (1 << 0)) != 0) instance.CommServer = PhotonViewProxy.Deserialize(bytes);
				if((nullMask & (1 << 1)) != 0) instance.EncryptionInitVector = StringProxy.Deserialize(bytes);
				if((nullMask & (1 << 2)) != 0) instance.EncryptionPassPhrase = StringProxy.Deserialize(bytes);
				if((nullMask & (1 << 3)) != 0) instance.GameServers = ListProxy<Cmune.Core.Models.Views.PhotonView>.Deserialize(bytes, PhotonViewProxy.Deserialize);
				if((nullMask & (1 << 4)) != 0) instance.SupportUrl = StringProxy.Deserialize(bytes);
				if((nullMask & (1 << 5)) != 0) instance.Version = StringProxy.Deserialize(bytes);
			}
			
			return instance;
		}
	}
	public static class LoadoutViewProxy
	{
		public static void Serialize(Stream stream, UberStrike.DataCenter.Common.Entities.LoadoutView instance)
		{
			int nullMask = 0;
			if(instance != null)
			{
				using (MemoryStream s = new MemoryStream())
                {
					Int32Proxy.Serialize(s, instance.Backpack);
					Int32Proxy.Serialize(s, instance.Boots);
					Int32Proxy.Serialize(s, instance.Cmid);
					Int32Proxy.Serialize(s, instance.Face);
					Int32Proxy.Serialize(s, instance.FunctionalItem1);
					Int32Proxy.Serialize(s, instance.FunctionalItem2);
					Int32Proxy.Serialize(s, instance.FunctionalItem3);
					Int32Proxy.Serialize(s, instance.Gloves);
					Int32Proxy.Serialize(s, instance.Head);
					Int32Proxy.Serialize(s, instance.LoadoutId);
					Int32Proxy.Serialize(s, instance.LowerBody);
					Int32Proxy.Serialize(s, instance.MeleeWeapon);
					Int32Proxy.Serialize(s, instance.QuickItem1);
					Int32Proxy.Serialize(s, instance.QuickItem2);
					Int32Proxy.Serialize(s, instance.QuickItem3);
					if(instance.SkinColor != null) StringProxy.Serialize(s, instance.SkinColor);
					else nullMask |= 1 << 0; 
					EnumProxy<UberStrike.Core.Types.AvatarType>.Serialize(s, instance.Type);
					Int32Proxy.Serialize(s, instance.UpperBody);
					Int32Proxy.Serialize(s, instance.Weapon1);
					Int32Proxy.Serialize(s, instance.Weapon1Mod1);
					Int32Proxy.Serialize(s, instance.Weapon1Mod2);
					Int32Proxy.Serialize(s, instance.Weapon1Mod3);
					Int32Proxy.Serialize(s, instance.Weapon2);
					Int32Proxy.Serialize(s, instance.Weapon2Mod1);
					Int32Proxy.Serialize(s, instance.Weapon2Mod2);
					Int32Proxy.Serialize(s, instance.Weapon2Mod3);
					Int32Proxy.Serialize(s, instance.Weapon3);
					Int32Proxy.Serialize(s, instance.Weapon3Mod1);
					Int32Proxy.Serialize(s, instance.Weapon3Mod2);
					Int32Proxy.Serialize(s, instance.Weapon3Mod3);
					Int32Proxy.Serialize(s, instance.Webbing);

					Int32Proxy.Serialize(stream, ~nullMask);
					s.WriteTo(stream);
				}
			}
			else
			{
				Int32Proxy.Serialize(stream, 0);
			}
		}
		
		public static UberStrike.DataCenter.Common.Entities.LoadoutView Deserialize(Stream bytes)
        {
			int nullMask = Int32Proxy.Deserialize(bytes);
			
			UberStrike.DataCenter.Common.Entities.LoadoutView instance = null;
			if(nullMask != 0)
			{
            	instance = new UberStrike.DataCenter.Common.Entities.LoadoutView();
				instance.Backpack = Int32Proxy.Deserialize(bytes);
				instance.Boots = Int32Proxy.Deserialize(bytes);
				instance.Cmid = Int32Proxy.Deserialize(bytes);
				instance.Face = Int32Proxy.Deserialize(bytes);
				instance.FunctionalItem1 = Int32Proxy.Deserialize(bytes);
				instance.FunctionalItem2 = Int32Proxy.Deserialize(bytes);
				instance.FunctionalItem3 = Int32Proxy.Deserialize(bytes);
				instance.Gloves = Int32Proxy.Deserialize(bytes);
				instance.Head = Int32Proxy.Deserialize(bytes);
				instance.LoadoutId = Int32Proxy.Deserialize(bytes);
				instance.LowerBody = Int32Proxy.Deserialize(bytes);
				instance.MeleeWeapon = Int32Proxy.Deserialize(bytes);
				instance.QuickItem1 = Int32Proxy.Deserialize(bytes);
				instance.QuickItem2 = Int32Proxy.Deserialize(bytes);
				instance.QuickItem3 = Int32Proxy.Deserialize(bytes);
				if((nullMask & (1 << 0)) != 0) instance.SkinColor = StringProxy.Deserialize(bytes);
				instance.Type = EnumProxy<UberStrike.Core.Types.AvatarType>.Deserialize(bytes);
				instance.UpperBody = Int32Proxy.Deserialize(bytes);
				instance.Weapon1 = Int32Proxy.Deserialize(bytes);
				instance.Weapon1Mod1 = Int32Proxy.Deserialize(bytes);
				instance.Weapon1Mod2 = Int32Proxy.Deserialize(bytes);
				instance.Weapon1Mod3 = Int32Proxy.Deserialize(bytes);
				instance.Weapon2 = Int32Proxy.Deserialize(bytes);
				instance.Weapon2Mod1 = Int32Proxy.Deserialize(bytes);
				instance.Weapon2Mod2 = Int32Proxy.Deserialize(bytes);
				instance.Weapon2Mod3 = Int32Proxy.Deserialize(bytes);
				instance.Weapon3 = Int32Proxy.Deserialize(bytes);
				instance.Weapon3Mod1 = Int32Proxy.Deserialize(bytes);
				instance.Weapon3Mod2 = Int32Proxy.Deserialize(bytes);
				instance.Weapon3Mod3 = Int32Proxy.Deserialize(bytes);
				instance.Webbing = Int32Proxy.Deserialize(bytes);
			}
			
			return instance;
		}
	}
	public static class CurrencyDepositsViewModelProxy
	{
		public static void Serialize(Stream stream, UberStrike.Core.ViewModel.CurrencyDepositsViewModel instance)
		{
			int nullMask = 0;
			if(instance != null)
			{
				using (MemoryStream s = new MemoryStream())
                {
					if(instance.CurrencyDeposits != null) ListProxy<Cmune.DataCenter.Common.Entities.CurrencyDepositView>.Serialize(s, instance.CurrencyDeposits, CurrencyDepositViewProxy.Serialize);
					else nullMask |= 1 << 0; 
					Int32Proxy.Serialize(s, instance.TotalCount);

					Int32Proxy.Serialize(stream, ~nullMask);
					s.WriteTo(stream);
				}
			}
			else
			{
				Int32Proxy.Serialize(stream, 0);
			}
		}
		
		public static UberStrike.Core.ViewModel.CurrencyDepositsViewModel Deserialize(Stream bytes)
        {
			int nullMask = Int32Proxy.Deserialize(bytes);
			
			UberStrike.Core.ViewModel.CurrencyDepositsViewModel instance = null;
			if(nullMask != 0)
			{
            	instance = new UberStrike.Core.ViewModel.CurrencyDepositsViewModel();
				if((nullMask & (1 << 0)) != 0) instance.CurrencyDeposits = ListProxy<Cmune.DataCenter.Common.Entities.CurrencyDepositView>.Deserialize(bytes, CurrencyDepositViewProxy.Deserialize);
				instance.TotalCount = Int32Proxy.Deserialize(bytes);
			}
			
			return instance;
		}
	}
	public static class UberStrikeItemFunctionalViewProxy
	{
		public static void Serialize(Stream stream, UberStrike.Core.Models.Views.UberStrikeItemFunctionalView instance)
		{
			int nullMask = 0;
			if(instance != null)
			{
				using (MemoryStream s = new MemoryStream())
                {
					if(instance.CustomProperties != null) DictionaryProxy<System.String, System.String>.Serialize(s, instance.CustomProperties, StringProxy.Serialize, StringProxy.Serialize);
					else nullMask |= 1 << 0; 
					if(instance.Description != null) StringProxy.Serialize(s, instance.Description);
					else nullMask |= 1 << 1; 
					Int32Proxy.Serialize(s, instance.ID);
					BooleanProxy.Serialize(s, instance.IsConsumable);
					EnumProxy<UberStrike.Core.Types.UberstrikeItemClass>.Serialize(s, instance.ItemClass);
					Int32Proxy.Serialize(s, instance.LevelLock);
					if(instance.Name != null) StringProxy.Serialize(s, instance.Name);
					else nullMask |= 1 << 2; 
					if(instance.Prices != null) ListProxy<UberStrike.Core.Models.Views.ItemPrice>.Serialize(s, instance.Prices, ItemPriceProxy.Serialize);
					else nullMask |= 1 << 3; 
					EnumProxy<UberStrike.Core.Types.ItemShopHighlightType>.Serialize(s, instance.ShopHighlightType);

					Int32Proxy.Serialize(stream, ~nullMask);
					s.WriteTo(stream);
				}
			}
			else
			{
				Int32Proxy.Serialize(stream, 0);
			}
		}
		
		public static UberStrike.Core.Models.Views.UberStrikeItemFunctionalView Deserialize(Stream bytes)
        {
			int nullMask = Int32Proxy.Deserialize(bytes);
			
			UberStrike.Core.Models.Views.UberStrikeItemFunctionalView instance = null;
			if(nullMask != 0)
			{
            	instance = new UberStrike.Core.Models.Views.UberStrikeItemFunctionalView();
				if((nullMask & (1 << 0)) != 0) instance.CustomProperties = DictionaryProxy<System.String, System.String>.Deserialize(bytes, StringProxy.Deserialize, StringProxy.Deserialize);
				if((nullMask & (1 << 1)) != 0) instance.Description = StringProxy.Deserialize(bytes);
				instance.ID = Int32Proxy.Deserialize(bytes);
				instance.IsConsumable = BooleanProxy.Deserialize(bytes);
				instance.ItemClass = EnumProxy<UberStrike.Core.Types.UberstrikeItemClass>.Deserialize(bytes);
				instance.LevelLock = Int32Proxy.Deserialize(bytes);
				if((nullMask & (1 << 2)) != 0) instance.Name = StringProxy.Deserialize(bytes);
				if((nullMask & (1 << 3)) != 0) instance.Prices = ListProxy<UberStrike.Core.Models.Views.ItemPrice>.Deserialize(bytes, ItemPriceProxy.Deserialize);
				instance.ShopHighlightType = EnumProxy<UberStrike.Core.Types.ItemShopHighlightType>.Deserialize(bytes);
			}
			
			return instance;
		}
	}
	public static class AuthenticateApplicationViewProxy
	{
		public static void Serialize(Stream stream, UberStrike.DataCenter.Common.Entities.AuthenticateApplicationView instance)
		{
			int nullMask = 0;
			if(instance != null)
			{
				using (MemoryStream s = new MemoryStream())
                {
					if(instance.CommServer != null) PhotonViewProxy.Serialize(s, instance.CommServer);
					else nullMask |= 1 << 0; 
					if(instance.EncryptionInitVector != null) StringProxy.Serialize(s, instance.EncryptionInitVector);
					else nullMask |= 1 << 1; 
					if(instance.EncryptionPassPhrase != null) StringProxy.Serialize(s, instance.EncryptionPassPhrase);
					else nullMask |= 1 << 2; 
					if(instance.GameServers != null) ListProxy<Cmune.Core.Models.Views.PhotonView>.Serialize(s, instance.GameServers, PhotonViewProxy.Serialize);
					else nullMask |= 1 << 3; 
					BooleanProxy.Serialize(s, instance.IsEnabled);
					BooleanProxy.Serialize(s, instance.WarnPlayer);

					Int32Proxy.Serialize(stream, ~nullMask);
					s.WriteTo(stream);
				}
			}
			else
			{
				Int32Proxy.Serialize(stream, 0);
			}
		}
		
		public static UberStrike.DataCenter.Common.Entities.AuthenticateApplicationView Deserialize(Stream bytes)
        {
			int nullMask = Int32Proxy.Deserialize(bytes);
			
			UberStrike.DataCenter.Common.Entities.AuthenticateApplicationView instance = null;
			if(nullMask != 0)
			{
            	instance = new UberStrike.DataCenter.Common.Entities.AuthenticateApplicationView();
				if((nullMask & (1 << 0)) != 0) instance.CommServer = PhotonViewProxy.Deserialize(bytes);
				if((nullMask & (1 << 1)) != 0) instance.EncryptionInitVector = StringProxy.Deserialize(bytes);
				if((nullMask & (1 << 2)) != 0) instance.EncryptionPassPhrase = StringProxy.Deserialize(bytes);
				if((nullMask & (1 << 3)) != 0) instance.GameServers = ListProxy<Cmune.Core.Models.Views.PhotonView>.Deserialize(bytes, PhotonViewProxy.Deserialize);
				instance.IsEnabled = BooleanProxy.Deserialize(bytes);
				instance.WarnPlayer = BooleanProxy.Deserialize(bytes);
			}
			
			return instance;
		}
	}
	public static class UberstrikeLevelViewModelProxy
	{
		public static void Serialize(Stream stream, UberStrike.Core.ViewModel.UberstrikeLevelViewModel instance)
		{
			int nullMask = 0;
			if(instance != null)
			{
				using (MemoryStream s = new MemoryStream())
                {
					if(instance.Maps != null) ListProxy<UberStrike.Core.Models.Views.MapView>.Serialize(s, instance.Maps, MapViewProxy.Serialize);
					else nullMask |= 1 << 0; 

					Int32Proxy.Serialize(stream, ~nullMask);
					s.WriteTo(stream);
				}
			}
			else
			{
				Int32Proxy.Serialize(stream, 0);
			}
		}
		
		public static UberStrike.Core.ViewModel.UberstrikeLevelViewModel Deserialize(Stream bytes)
        {
			int nullMask = Int32Proxy.Deserialize(bytes);
			
			UberStrike.Core.ViewModel.UberstrikeLevelViewModel instance = null;
			if(nullMask != 0)
			{
            	instance = new UberStrike.Core.ViewModel.UberstrikeLevelViewModel();
				if((nullMask & (1 << 0)) != 0) instance.Maps = ListProxy<UberStrike.Core.Models.Views.MapView>.Deserialize(bytes, MapViewProxy.Deserialize);
			}
			
			return instance;
		}
	}
	public static class UberStrikeItemGearViewProxy
	{
		public static void Serialize(Stream stream, UberStrike.Core.Models.Views.UberStrikeItemGearView instance)
		{
			int nullMask = 0;
			if(instance != null)
			{
				using (MemoryStream s = new MemoryStream())
                {
					Int32Proxy.Serialize(s, instance.ArmorAbsorptionPercent);
					Int32Proxy.Serialize(s, instance.ArmorPoints);
					Int32Proxy.Serialize(s, instance.ArmorWeight);
					if(instance.CustomProperties != null) DictionaryProxy<System.String, System.String>.Serialize(s, instance.CustomProperties, StringProxy.Serialize, StringProxy.Serialize);
					else nullMask |= 1 << 0; 
					if(instance.Description != null) StringProxy.Serialize(s, instance.Description);
					else nullMask |= 1 << 1; 
					Int32Proxy.Serialize(s, instance.ID);
					BooleanProxy.Serialize(s, instance.IsConsumable);
					EnumProxy<UberStrike.Core.Types.UberstrikeItemClass>.Serialize(s, instance.ItemClass);
					Int32Proxy.Serialize(s, instance.LevelLock);
					if(instance.Name != null) StringProxy.Serialize(s, instance.Name);
					else nullMask |= 1 << 2; 
					if(instance.Prices != null) ListProxy<UberStrike.Core.Models.Views.ItemPrice>.Serialize(s, instance.Prices, ItemPriceProxy.Serialize);
					else nullMask |= 1 << 3; 
					EnumProxy<UberStrike.Core.Types.ItemShopHighlightType>.Serialize(s, instance.ShopHighlightType);

					Int32Proxy.Serialize(stream, ~nullMask);
					s.WriteTo(stream);
				}
			}
			else
			{
				Int32Proxy.Serialize(stream, 0);
			}
		}
		
		public static UberStrike.Core.Models.Views.UberStrikeItemGearView Deserialize(Stream bytes)
        {
			int nullMask = Int32Proxy.Deserialize(bytes);
			
			UberStrike.Core.Models.Views.UberStrikeItemGearView instance = null;
			if(nullMask != 0)
			{
            	instance = new UberStrike.Core.Models.Views.UberStrikeItemGearView();
				instance.ArmorAbsorptionPercent = Int32Proxy.Deserialize(bytes);
				instance.ArmorPoints = Int32Proxy.Deserialize(bytes);
				instance.ArmorWeight = Int32Proxy.Deserialize(bytes);
				if((nullMask & (1 << 0)) != 0) instance.CustomProperties = DictionaryProxy<System.String, System.String>.Deserialize(bytes, StringProxy.Deserialize, StringProxy.Deserialize);
				if((nullMask & (1 << 1)) != 0) instance.Description = StringProxy.Deserialize(bytes);
				instance.ID = Int32Proxy.Deserialize(bytes);
				instance.IsConsumable = BooleanProxy.Deserialize(bytes);
				instance.ItemClass = EnumProxy<UberStrike.Core.Types.UberstrikeItemClass>.Deserialize(bytes);
				instance.LevelLock = Int32Proxy.Deserialize(bytes);
				if((nullMask & (1 << 2)) != 0) instance.Name = StringProxy.Deserialize(bytes);
				if((nullMask & (1 << 3)) != 0) instance.Prices = ListProxy<UberStrike.Core.Models.Views.ItemPrice>.Deserialize(bytes, ItemPriceProxy.Deserialize);
				instance.ShopHighlightType = EnumProxy<UberStrike.Core.Types.ItemShopHighlightType>.Deserialize(bytes);
			}
			
			return instance;
		}
	}
	public static class RegisterClientApplicationViewModelProxy
	{
		public static void Serialize(Stream stream, UberStrike.Core.ViewModel.RegisterClientApplicationViewModel instance)
		{
			int nullMask = 0;
			if(instance != null)
			{
				using (MemoryStream s = new MemoryStream())
                {
					if(instance.ItemsAttributed != null) ListProxy<System.Int32>.Serialize(s, instance.ItemsAttributed, Int32Proxy.Serialize);
					else nullMask |= 1 << 0; 
					EnumProxy<Cmune.DataCenter.Common.Entities.ApplicationRegistrationResult>.Serialize(s, instance.Result);

					Int32Proxy.Serialize(stream, ~nullMask);
					s.WriteTo(stream);
				}
			}
			else
			{
				Int32Proxy.Serialize(stream, 0);
			}
		}
		
		public static UberStrike.Core.ViewModel.RegisterClientApplicationViewModel Deserialize(Stream bytes)
        {
			int nullMask = Int32Proxy.Deserialize(bytes);
			
			UberStrike.Core.ViewModel.RegisterClientApplicationViewModel instance = null;
			if(nullMask != 0)
			{
            	instance = new UberStrike.Core.ViewModel.RegisterClientApplicationViewModel();
				if((nullMask & (1 << 0)) != 0) instance.ItemsAttributed = ListProxy<System.Int32>.Deserialize(bytes, Int32Proxy.Deserialize);
				instance.Result = EnumProxy<Cmune.DataCenter.Common.Entities.ApplicationRegistrationResult>.Deserialize(bytes);
			}
			
			return instance;
		}
	}
	public static class LiveFeedViewProxy
	{
		public static void Serialize(Stream stream, UberStrike.DataCenter.Common.Entities.LiveFeedView instance)
		{
			int nullMask = 0;
			if(instance != null)
			{
				using (MemoryStream s = new MemoryStream())
                {
					DateTimeProxy.Serialize(s, instance.Date);
					if(instance.Description != null) StringProxy.Serialize(s, instance.Description);
					else nullMask |= 1 << 0; 
					Int32Proxy.Serialize(s, instance.LivedFeedId);
					Int32Proxy.Serialize(s, instance.Priority);
					if(instance.Url != null) StringProxy.Serialize(s, instance.Url);
					else nullMask |= 1 << 1; 

					Int32Proxy.Serialize(stream, ~nullMask);
					s.WriteTo(stream);
				}
			}
			else
			{
				Int32Proxy.Serialize(stream, 0);
			}
		}
		
		public static UberStrike.DataCenter.Common.Entities.LiveFeedView Deserialize(Stream bytes)
        {
			int nullMask = Int32Proxy.Deserialize(bytes);
			
			UberStrike.DataCenter.Common.Entities.LiveFeedView instance = null;
			if(nullMask != 0)
			{
            	instance = new UberStrike.DataCenter.Common.Entities.LiveFeedView();
				instance.Date = DateTimeProxy.Deserialize(bytes);
				if((nullMask & (1 << 0)) != 0) instance.Description = StringProxy.Deserialize(bytes);
				instance.LivedFeedId = Int32Proxy.Deserialize(bytes);
				instance.Priority = Int32Proxy.Deserialize(bytes);
				if((nullMask & (1 << 1)) != 0) instance.Url = StringProxy.Deserialize(bytes);
			}
			
			return instance;
		}
	}
	public static class MapSettingsProxy
	{
		public static void Serialize(Stream stream, UberStrike.Core.Models.Views.MapSettings instance)
		{
			int nullMask = 0;
			if(instance != null)
			{
				using (MemoryStream s = new MemoryStream())
                {
					Int32Proxy.Serialize(s, instance.KillsCurrent);
					Int32Proxy.Serialize(s, instance.KillsMax);
					Int32Proxy.Serialize(s, instance.KillsMin);
					Int32Proxy.Serialize(s, instance.PlayersCurrent);
					Int32Proxy.Serialize(s, instance.PlayersMax);
					Int32Proxy.Serialize(s, instance.PlayersMin);
					Int32Proxy.Serialize(s, instance.TimeCurrent);
					Int32Proxy.Serialize(s, instance.TimeMax);
					Int32Proxy.Serialize(s, instance.TimeMin);

					Int32Proxy.Serialize(stream, ~nullMask);
					s.WriteTo(stream);
				}
			}
			else
			{
				Int32Proxy.Serialize(stream, 0);
			}
		}
		
		public static UberStrike.Core.Models.Views.MapSettings Deserialize(Stream bytes)
        {
			int nullMask = Int32Proxy.Deserialize(bytes);
			
			UberStrike.Core.Models.Views.MapSettings instance = null;
			if(nullMask != 0)
			{
            	instance = new UberStrike.Core.Models.Views.MapSettings();
				instance.KillsCurrent = Int32Proxy.Deserialize(bytes);
				instance.KillsMax = Int32Proxy.Deserialize(bytes);
				instance.KillsMin = Int32Proxy.Deserialize(bytes);
				instance.PlayersCurrent = Int32Proxy.Deserialize(bytes);
				instance.PlayersMax = Int32Proxy.Deserialize(bytes);
				instance.PlayersMin = Int32Proxy.Deserialize(bytes);
				instance.TimeCurrent = Int32Proxy.Deserialize(bytes);
				instance.TimeMax = Int32Proxy.Deserialize(bytes);
				instance.TimeMin = Int32Proxy.Deserialize(bytes);
			}
			
			return instance;
		}
	}
	public static class PlayerCardViewProxy
	{
		public static void Serialize(Stream stream, UberStrike.DataCenter.Common.Entities.PlayerCardView instance)
		{
			int nullMask = 0;
			if(instance != null)
			{
				using (MemoryStream s = new MemoryStream())
                {
					Int32Proxy.Serialize(s, instance.Cmid);
					Int64Proxy.Serialize(s, instance.Hits);
					if(instance.Name != null) StringProxy.Serialize(s, instance.Name);
					else nullMask |= 1 << 0; 
					if(instance.Precision != null) StringProxy.Serialize(s, instance.Precision);
					else nullMask |= 1 << 1; 
					Int32Proxy.Serialize(s, instance.Ranking);
					Int64Proxy.Serialize(s, instance.Shots);
					Int32Proxy.Serialize(s, instance.Splats);
					Int32Proxy.Serialize(s, instance.Splatted);
					if(instance.TagName != null) StringProxy.Serialize(s, instance.TagName);
					else nullMask |= 1 << 2; 

					Int32Proxy.Serialize(stream, ~nullMask);
					s.WriteTo(stream);
				}
			}
			else
			{
				Int32Proxy.Serialize(stream, 0);
			}
		}
		
		public static UberStrike.DataCenter.Common.Entities.PlayerCardView Deserialize(Stream bytes)
        {
			int nullMask = Int32Proxy.Deserialize(bytes);
			
			UberStrike.DataCenter.Common.Entities.PlayerCardView instance = null;
			if(nullMask != 0)
			{
            	instance = new UberStrike.DataCenter.Common.Entities.PlayerCardView();
				instance.Cmid = Int32Proxy.Deserialize(bytes);
				instance.Hits = Int64Proxy.Deserialize(bytes);
				if((nullMask & (1 << 0)) != 0) instance.Name = StringProxy.Deserialize(bytes);
				if((nullMask & (1 << 1)) != 0) instance.Precision = StringProxy.Deserialize(bytes);
				instance.Ranking = Int32Proxy.Deserialize(bytes);
				instance.Shots = Int64Proxy.Deserialize(bytes);
				instance.Splats = Int32Proxy.Deserialize(bytes);
				instance.Splatted = Int32Proxy.Deserialize(bytes);
				if((nullMask & (1 << 2)) != 0) instance.TagName = StringProxy.Deserialize(bytes);
			}
			
			return instance;
		}
	}
	public static class ServerConnectionViewProxy
	{
		public static void Serialize(Stream stream, UberStrike.Core.ViewModel.ServerConnectionView instance)
		{
			int nullMask = 0;
			if(instance != null)
			{
				using (MemoryStream s = new MemoryStream())
                {
					Int32Proxy.Serialize(s, instance.ApiVersion);
					EnumProxy<Cmune.DataCenter.Common.Entities.ChannelType>.Serialize(s, instance.Channel);
					Int32Proxy.Serialize(s, instance.Cmid);

					Int32Proxy.Serialize(stream, ~nullMask);
					s.WriteTo(stream);
				}
			}
			else
			{
				Int32Proxy.Serialize(stream, 0);
			}
		}
		
		public static UberStrike.Core.ViewModel.ServerConnectionView Deserialize(Stream bytes)
        {
			int nullMask = Int32Proxy.Deserialize(bytes);
			
			UberStrike.Core.ViewModel.ServerConnectionView instance = null;
			if(nullMask != 0)
			{
            	instance = new UberStrike.Core.ViewModel.ServerConnectionView();
				instance.ApiVersion = Int32Proxy.Deserialize(bytes);
				instance.Channel = EnumProxy<Cmune.DataCenter.Common.Entities.ChannelType>.Deserialize(bytes);
				instance.Cmid = Int32Proxy.Deserialize(bytes);
			}
			
			return instance;
		}
	}
	public static class PlayerWeaponStatisticsViewProxy
	{
		public static void Serialize(Stream stream, UberStrike.DataCenter.Common.Entities.PlayerWeaponStatisticsView instance)
		{
			int nullMask = 0;
			if(instance != null)
			{
				using (MemoryStream s = new MemoryStream())
                {
					Int32Proxy.Serialize(s, instance.CannonTotalDamageDone);
					Int32Proxy.Serialize(s, instance.CannonTotalShotsFired);
					Int32Proxy.Serialize(s, instance.CannonTotalShotsHit);
					Int32Proxy.Serialize(s, instance.CannonTotalSplats);
					Int32Proxy.Serialize(s, instance.HandgunTotalDamageDone);
					Int32Proxy.Serialize(s, instance.HandgunTotalShotsFired);
					Int32Proxy.Serialize(s, instance.HandgunTotalShotsHit);
					Int32Proxy.Serialize(s, instance.HandgunTotalSplats);
					Int32Proxy.Serialize(s, instance.LauncherTotalDamageDone);
					Int32Proxy.Serialize(s, instance.LauncherTotalShotsFired);
					Int32Proxy.Serialize(s, instance.LauncherTotalShotsHit);
					Int32Proxy.Serialize(s, instance.LauncherTotalSplats);
					Int32Proxy.Serialize(s, instance.MachineGunTotalDamageDone);
					Int32Proxy.Serialize(s, instance.MachineGunTotalShotsFired);
					Int32Proxy.Serialize(s, instance.MachineGunTotalShotsHit);
					Int32Proxy.Serialize(s, instance.MachineGunTotalSplats);
					Int32Proxy.Serialize(s, instance.MeleeTotalDamageDone);
					Int32Proxy.Serialize(s, instance.MeleeTotalShotsFired);
					Int32Proxy.Serialize(s, instance.MeleeTotalShotsHit);
					Int32Proxy.Serialize(s, instance.MeleeTotalSplats);
					Int32Proxy.Serialize(s, instance.ShotgunTotalDamageDone);
					Int32Proxy.Serialize(s, instance.ShotgunTotalShotsFired);
					Int32Proxy.Serialize(s, instance.ShotgunTotalShotsHit);
					Int32Proxy.Serialize(s, instance.ShotgunTotalSplats);
					Int32Proxy.Serialize(s, instance.SniperTotalDamageDone);
					Int32Proxy.Serialize(s, instance.SniperTotalShotsFired);
					Int32Proxy.Serialize(s, instance.SniperTotalShotsHit);
					Int32Proxy.Serialize(s, instance.SniperTotalSplats);
					Int32Proxy.Serialize(s, instance.SplattergunTotalDamageDone);
					Int32Proxy.Serialize(s, instance.SplattergunTotalShotsFired);
					Int32Proxy.Serialize(s, instance.SplattergunTotalShotsHit);
					Int32Proxy.Serialize(s, instance.SplattergunTotalSplats);

					Int32Proxy.Serialize(stream, ~nullMask);
					s.WriteTo(stream);
				}
			}
			else
			{
				Int32Proxy.Serialize(stream, 0);
			}
		}
		
		public static UberStrike.DataCenter.Common.Entities.PlayerWeaponStatisticsView Deserialize(Stream bytes)
        {
			int nullMask = Int32Proxy.Deserialize(bytes);
			
			UberStrike.DataCenter.Common.Entities.PlayerWeaponStatisticsView instance = null;
			if(nullMask != 0)
			{
            	instance = new UberStrike.DataCenter.Common.Entities.PlayerWeaponStatisticsView();
				instance.CannonTotalDamageDone = Int32Proxy.Deserialize(bytes);
				instance.CannonTotalShotsFired = Int32Proxy.Deserialize(bytes);
				instance.CannonTotalShotsHit = Int32Proxy.Deserialize(bytes);
				instance.CannonTotalSplats = Int32Proxy.Deserialize(bytes);
				instance.HandgunTotalDamageDone = Int32Proxy.Deserialize(bytes);
				instance.HandgunTotalShotsFired = Int32Proxy.Deserialize(bytes);
				instance.HandgunTotalShotsHit = Int32Proxy.Deserialize(bytes);
				instance.HandgunTotalSplats = Int32Proxy.Deserialize(bytes);
				instance.LauncherTotalDamageDone = Int32Proxy.Deserialize(bytes);
				instance.LauncherTotalShotsFired = Int32Proxy.Deserialize(bytes);
				instance.LauncherTotalShotsHit = Int32Proxy.Deserialize(bytes);
				instance.LauncherTotalSplats = Int32Proxy.Deserialize(bytes);
				instance.MachineGunTotalDamageDone = Int32Proxy.Deserialize(bytes);
				instance.MachineGunTotalShotsFired = Int32Proxy.Deserialize(bytes);
				instance.MachineGunTotalShotsHit = Int32Proxy.Deserialize(bytes);
				instance.MachineGunTotalSplats = Int32Proxy.Deserialize(bytes);
				instance.MeleeTotalDamageDone = Int32Proxy.Deserialize(bytes);
				instance.MeleeTotalShotsFired = Int32Proxy.Deserialize(bytes);
				instance.MeleeTotalShotsHit = Int32Proxy.Deserialize(bytes);
				instance.MeleeTotalSplats = Int32Proxy.Deserialize(bytes);
				instance.ShotgunTotalDamageDone = Int32Proxy.Deserialize(bytes);
				instance.ShotgunTotalShotsFired = Int32Proxy.Deserialize(bytes);
				instance.ShotgunTotalShotsHit = Int32Proxy.Deserialize(bytes);
				instance.ShotgunTotalSplats = Int32Proxy.Deserialize(bytes);
				instance.SniperTotalDamageDone = Int32Proxy.Deserialize(bytes);
				instance.SniperTotalShotsFired = Int32Proxy.Deserialize(bytes);
				instance.SniperTotalShotsHit = Int32Proxy.Deserialize(bytes);
				instance.SniperTotalSplats = Int32Proxy.Deserialize(bytes);
				instance.SplattergunTotalDamageDone = Int32Proxy.Deserialize(bytes);
				instance.SplattergunTotalShotsFired = Int32Proxy.Deserialize(bytes);
				instance.SplattergunTotalShotsHit = Int32Proxy.Deserialize(bytes);
				instance.SplattergunTotalSplats = Int32Proxy.Deserialize(bytes);
			}
			
			return instance;
		}
	}
	public static class PlayerStatisticsViewProxy
	{
		public static void Serialize(Stream stream, UberStrike.DataCenter.Common.Entities.PlayerStatisticsView instance)
		{
			int nullMask = 0;
			if(instance != null)
			{
				using (MemoryStream s = new MemoryStream())
                {
					Int32Proxy.Serialize(s, instance.Cmid);
					Int32Proxy.Serialize(s, instance.Headshots);
					Int64Proxy.Serialize(s, instance.Hits);
					Int32Proxy.Serialize(s, instance.Level);
					Int32Proxy.Serialize(s, instance.Nutshots);
					if(instance.PersonalRecord != null) PlayerPersonalRecordStatisticsViewProxy.Serialize(s, instance.PersonalRecord);
					else nullMask |= 1 << 0; 
					Int32Proxy.Serialize(s, instance.Points);
					Int64Proxy.Serialize(s, instance.Shots);
					Int32Proxy.Serialize(s, instance.Splats);
					Int32Proxy.Serialize(s, instance.Splatted);
					Int32Proxy.Serialize(s, instance.TimeSpentInGame);
					if(instance.WeaponStatistics != null) PlayerWeaponStatisticsViewProxy.Serialize(s, instance.WeaponStatistics);
					else nullMask |= 1 << 1; 
					Int32Proxy.Serialize(s, instance.Xp);

					Int32Proxy.Serialize(stream, ~nullMask);
					s.WriteTo(stream);
				}
			}
			else
			{
				Int32Proxy.Serialize(stream, 0);
			}
		}
		
		public static UberStrike.DataCenter.Common.Entities.PlayerStatisticsView Deserialize(Stream bytes)
        {
			int nullMask = Int32Proxy.Deserialize(bytes);
			
			UberStrike.DataCenter.Common.Entities.PlayerStatisticsView instance = null;
			if(nullMask != 0)
			{
            	instance = new UberStrike.DataCenter.Common.Entities.PlayerStatisticsView();
				instance.Cmid = Int32Proxy.Deserialize(bytes);
				instance.Headshots = Int32Proxy.Deserialize(bytes);
				instance.Hits = Int64Proxy.Deserialize(bytes);
				instance.Level = Int32Proxy.Deserialize(bytes);
				instance.Nutshots = Int32Proxy.Deserialize(bytes);
				if((nullMask & (1 << 0)) != 0) instance.PersonalRecord = PlayerPersonalRecordStatisticsViewProxy.Deserialize(bytes);
				instance.Points = Int32Proxy.Deserialize(bytes);
				instance.Shots = Int64Proxy.Deserialize(bytes);
				instance.Splats = Int32Proxy.Deserialize(bytes);
				instance.Splatted = Int32Proxy.Deserialize(bytes);
				instance.TimeSpentInGame = Int32Proxy.Deserialize(bytes);
				if((nullMask & (1 << 1)) != 0) instance.WeaponStatistics = PlayerWeaponStatisticsViewProxy.Deserialize(bytes);
				instance.Xp = Int32Proxy.Deserialize(bytes);
			}
			
			return instance;
		}
	}
	public static class MatchViewProxy
	{
		public static void Serialize(Stream stream, UberStrike.DataCenter.Common.Entities.MatchView instance)
		{
			int nullMask = 0;
			if(instance != null)
			{
				using (MemoryStream s = new MemoryStream())
                {
					EnumProxy<UberStrike.Core.Types.GameModeType>.Serialize(s, instance.GameModeId);
					Int32Proxy.Serialize(s, instance.MapId);
					if(instance.PlayersCompleted != null) ListProxy<UberStrike.DataCenter.Common.Entities.PlayerStatisticsView>.Serialize(s, instance.PlayersCompleted, PlayerStatisticsViewProxy.Serialize);
					else nullMask |= 1 << 0; 
					Int32Proxy.Serialize(s, instance.PlayersLimit);
					if(instance.PlayersNonCompleted != null) ListProxy<UberStrike.DataCenter.Common.Entities.PlayerStatisticsView>.Serialize(s, instance.PlayersNonCompleted, PlayerStatisticsViewProxy.Serialize);
					else nullMask |= 1 << 1; 
					Int32Proxy.Serialize(s, instance.TimeLimit);

					Int32Proxy.Serialize(stream, ~nullMask);
					s.WriteTo(stream);
				}
			}
			else
			{
				Int32Proxy.Serialize(stream, 0);
			}
		}
		
		public static UberStrike.DataCenter.Common.Entities.MatchView Deserialize(Stream bytes)
        {
			int nullMask = Int32Proxy.Deserialize(bytes);
			
			UberStrike.DataCenter.Common.Entities.MatchView instance = null;
			if(nullMask != 0)
			{
            	instance = new UberStrike.DataCenter.Common.Entities.MatchView();
				instance.GameModeId = EnumProxy<UberStrike.Core.Types.GameModeType>.Deserialize(bytes);
				instance.MapId = Int32Proxy.Deserialize(bytes);
				if((nullMask & (1 << 0)) != 0) instance.PlayersCompleted = ListProxy<UberStrike.DataCenter.Common.Entities.PlayerStatisticsView>.Deserialize(bytes, PlayerStatisticsViewProxy.Deserialize);
				instance.PlayersLimit = Int32Proxy.Deserialize(bytes);
				if((nullMask & (1 << 1)) != 0) instance.PlayersNonCompleted = ListProxy<UberStrike.DataCenter.Common.Entities.PlayerStatisticsView>.Deserialize(bytes, PlayerStatisticsViewProxy.Deserialize);
				instance.TimeLimit = Int32Proxy.Deserialize(bytes);
			}
			
			return instance;
		}
	}
	public static class ItemQuickUseConfigViewProxy
	{
		public static void Serialize(Stream stream, UberStrike.DataCenter.Common.Entities.ItemQuickUseConfigView instance)
		{
			int nullMask = 0;
			if(instance != null)
			{
				using (MemoryStream s = new MemoryStream())
                {
					EnumProxy<UberStrike.Core.Types.QuickItemLogic>.Serialize(s, instance.BehaviourType);
					Int32Proxy.Serialize(s, instance.CoolDownTime);
					Int32Proxy.Serialize(s, instance.ItemId);
					Int32Proxy.Serialize(s, instance.LevelRequired);
					Int32Proxy.Serialize(s, instance.UsesPerGame);
					Int32Proxy.Serialize(s, instance.UsesPerLife);
					Int32Proxy.Serialize(s, instance.UsesPerRound);
					Int32Proxy.Serialize(s, instance.WarmUpTime);

					Int32Proxy.Serialize(stream, ~nullMask);
					s.WriteTo(stream);
				}
			}
			else
			{
				Int32Proxy.Serialize(stream, 0);
			}
		}
		
		public static UberStrike.DataCenter.Common.Entities.ItemQuickUseConfigView Deserialize(Stream bytes)
        {
			int nullMask = Int32Proxy.Deserialize(bytes);
			
			UberStrike.DataCenter.Common.Entities.ItemQuickUseConfigView instance = null;
			if(nullMask != 0)
			{
            	instance = new UberStrike.DataCenter.Common.Entities.ItemQuickUseConfigView();
				instance.BehaviourType = EnumProxy<UberStrike.Core.Types.QuickItemLogic>.Deserialize(bytes);
				instance.CoolDownTime = Int32Proxy.Deserialize(bytes);
				instance.ItemId = Int32Proxy.Deserialize(bytes);
				instance.LevelRequired = Int32Proxy.Deserialize(bytes);
				instance.UsesPerGame = Int32Proxy.Deserialize(bytes);
				instance.UsesPerLife = Int32Proxy.Deserialize(bytes);
				instance.UsesPerRound = Int32Proxy.Deserialize(bytes);
				instance.WarmUpTime = Int32Proxy.Deserialize(bytes);
			}
			
			return instance;
		}
	}
	public static class ClanInvitationAnswerViewModelProxy
	{
		public static void Serialize(Stream stream, UberStrike.Core.ViewModel.ClanInvitationAnswerViewModel instance)
		{
			int nullMask = 0;
			if(instance != null)
			{
				using (MemoryStream s = new MemoryStream())
                {
					Int32Proxy.Serialize(s, instance.GroupInvitationId);
					BooleanProxy.Serialize(s, instance.IsInvitationAccepted);
					Int32Proxy.Serialize(s, instance.ReturnValue);

					Int32Proxy.Serialize(stream, ~nullMask);
					s.WriteTo(stream);
				}
			}
			else
			{
				Int32Proxy.Serialize(stream, 0);
			}
		}
		
		public static UberStrike.Core.ViewModel.ClanInvitationAnswerViewModel Deserialize(Stream bytes)
        {
			int nullMask = Int32Proxy.Deserialize(bytes);
			
			UberStrike.Core.ViewModel.ClanInvitationAnswerViewModel instance = null;
			if(nullMask != 0)
			{
            	instance = new UberStrike.Core.ViewModel.ClanInvitationAnswerViewModel();
				instance.GroupInvitationId = Int32Proxy.Deserialize(bytes);
				instance.IsInvitationAccepted = BooleanProxy.Deserialize(bytes);
				instance.ReturnValue = Int32Proxy.Deserialize(bytes);
			}
			
			return instance;
		}
	}
	public static class ItemTransactionsViewModelProxy
	{
		public static void Serialize(Stream stream, UberStrike.Core.ViewModel.ItemTransactionsViewModel instance)
		{
			int nullMask = 0;
			if(instance != null)
			{
				using (MemoryStream s = new MemoryStream())
                {
					if(instance.ItemTransactions != null) ListProxy<Cmune.DataCenter.Common.Entities.ItemTransactionView>.Serialize(s, instance.ItemTransactions, ItemTransactionViewProxy.Serialize);
					else nullMask |= 1 << 0; 
					Int32Proxy.Serialize(s, instance.TotalCount);

					Int32Proxy.Serialize(stream, ~nullMask);
					s.WriteTo(stream);
				}
			}
			else
			{
				Int32Proxy.Serialize(stream, 0);
			}
		}
		
		public static UberStrike.Core.ViewModel.ItemTransactionsViewModel Deserialize(Stream bytes)
        {
			int nullMask = Int32Proxy.Deserialize(bytes);
			
			UberStrike.Core.ViewModel.ItemTransactionsViewModel instance = null;
			if(nullMask != 0)
			{
            	instance = new UberStrike.Core.ViewModel.ItemTransactionsViewModel();
				if((nullMask & (1 << 0)) != 0) instance.ItemTransactions = ListProxy<Cmune.DataCenter.Common.Entities.ItemTransactionView>.Deserialize(bytes, ItemTransactionViewProxy.Deserialize);
				instance.TotalCount = Int32Proxy.Deserialize(bytes);
			}
			
			return instance;
		}
	}
	public static class PlaySpanHashesViewModelProxy
	{
		public static void Serialize(Stream stream, UberStrike.Core.ViewModel.PlaySpanHashesViewModel instance)
		{
			int nullMask = 0;
			if(instance != null)
			{
				using (MemoryStream s = new MemoryStream())
                {
					if(instance.Hashes != null) DictionaryProxy<System.Decimal, System.String>.Serialize(s, instance.Hashes, DecimalProxy.Serialize, StringProxy.Serialize);
					else nullMask |= 1 << 0; 
					if(instance.MerchTrans != null) StringProxy.Serialize(s, instance.MerchTrans);
					else nullMask |= 1 << 1; 

					Int32Proxy.Serialize(stream, ~nullMask);
					s.WriteTo(stream);
				}
			}
			else
			{
				Int32Proxy.Serialize(stream, 0);
			}
		}
		
		public static UberStrike.Core.ViewModel.PlaySpanHashesViewModel Deserialize(Stream bytes)
        {
			int nullMask = Int32Proxy.Deserialize(bytes);
			
			UberStrike.Core.ViewModel.PlaySpanHashesViewModel instance = null;
			if(nullMask != 0)
			{
            	instance = new UberStrike.Core.ViewModel.PlaySpanHashesViewModel();
				if((nullMask & (1 << 0)) != 0) instance.Hashes = DictionaryProxy<System.Decimal, System.String>.Deserialize(bytes, DecimalProxy.Deserialize, StringProxy.Deserialize);
				if((nullMask & (1 << 1)) != 0) instance.MerchTrans = StringProxy.Deserialize(bytes);
			}
			
			return instance;
		}
	}
	public static class UberStrikeItemQuickViewProxy
	{
		public static void Serialize(Stream stream, UberStrike.Core.Models.Views.UberStrikeItemQuickView instance)
		{
			int nullMask = 0;
			if(instance != null)
			{
				using (MemoryStream s = new MemoryStream())
                {
					EnumProxy<UberStrike.Core.Types.QuickItemLogic>.Serialize(s, instance.BehaviourType);
					Int32Proxy.Serialize(s, instance.CoolDownTime);
					if(instance.CustomProperties != null) DictionaryProxy<System.String, System.String>.Serialize(s, instance.CustomProperties, StringProxy.Serialize, StringProxy.Serialize);
					else nullMask |= 1 << 0; 
					if(instance.Description != null) StringProxy.Serialize(s, instance.Description);
					else nullMask |= 1 << 1; 
					Int32Proxy.Serialize(s, instance.ID);
					BooleanProxy.Serialize(s, instance.IsConsumable);
					EnumProxy<UberStrike.Core.Types.UberstrikeItemClass>.Serialize(s, instance.ItemClass);
					Int32Proxy.Serialize(s, instance.LevelLock);
					Int32Proxy.Serialize(s, instance.MaxOwnableAmount);
					if(instance.Name != null) StringProxy.Serialize(s, instance.Name);
					else nullMask |= 1 << 2; 
					if(instance.Prices != null) ListProxy<UberStrike.Core.Models.Views.ItemPrice>.Serialize(s, instance.Prices, ItemPriceProxy.Serialize);
					else nullMask |= 1 << 3; 
					EnumProxy<UberStrike.Core.Types.ItemShopHighlightType>.Serialize(s, instance.ShopHighlightType);
					Int32Proxy.Serialize(s, instance.UsesPerGame);
					Int32Proxy.Serialize(s, instance.UsesPerLife);
					Int32Proxy.Serialize(s, instance.UsesPerRound);
					Int32Proxy.Serialize(s, instance.WarmUpTime);

					Int32Proxy.Serialize(stream, ~nullMask);
					s.WriteTo(stream);
				}
			}
			else
			{
				Int32Proxy.Serialize(stream, 0);
			}
		}
		
		public static UberStrike.Core.Models.Views.UberStrikeItemQuickView Deserialize(Stream bytes)
        {
			int nullMask = Int32Proxy.Deserialize(bytes);
			
			UberStrike.Core.Models.Views.UberStrikeItemQuickView instance = null;
			if(nullMask != 0)
			{
            	instance = new UberStrike.Core.Models.Views.UberStrikeItemQuickView();
				instance.BehaviourType = EnumProxy<UberStrike.Core.Types.QuickItemLogic>.Deserialize(bytes);
				instance.CoolDownTime = Int32Proxy.Deserialize(bytes);
				if((nullMask & (1 << 0)) != 0) instance.CustomProperties = DictionaryProxy<System.String, System.String>.Deserialize(bytes, StringProxy.Deserialize, StringProxy.Deserialize);
				if((nullMask & (1 << 1)) != 0) instance.Description = StringProxy.Deserialize(bytes);
				instance.ID = Int32Proxy.Deserialize(bytes);
				instance.IsConsumable = BooleanProxy.Deserialize(bytes);
				instance.ItemClass = EnumProxy<UberStrike.Core.Types.UberstrikeItemClass>.Deserialize(bytes);
				instance.LevelLock = Int32Proxy.Deserialize(bytes);
				instance.MaxOwnableAmount = Int32Proxy.Deserialize(bytes);
				if((nullMask & (1 << 2)) != 0) instance.Name = StringProxy.Deserialize(bytes);
				if((nullMask & (1 << 3)) != 0) instance.Prices = ListProxy<UberStrike.Core.Models.Views.ItemPrice>.Deserialize(bytes, ItemPriceProxy.Deserialize);
				instance.ShopHighlightType = EnumProxy<UberStrike.Core.Types.ItemShopHighlightType>.Deserialize(bytes);
				instance.UsesPerGame = Int32Proxy.Deserialize(bytes);
				instance.UsesPerLife = Int32Proxy.Deserialize(bytes);
				instance.UsesPerRound = Int32Proxy.Deserialize(bytes);
				instance.WarmUpTime = Int32Proxy.Deserialize(bytes);
			}
			
			return instance;
		}
	}
	public static class PlayerLevelCapViewProxy
	{
		public static void Serialize(Stream stream, UberStrike.DataCenter.Common.Entities.PlayerLevelCapView instance)
		{
			int nullMask = 0;
			if(instance != null)
			{
				using (MemoryStream s = new MemoryStream())
                {
					Int32Proxy.Serialize(s, instance.Level);
					Int32Proxy.Serialize(s, instance.PlayerLevelCapId);
					Int32Proxy.Serialize(s, instance.XPRequired);

					Int32Proxy.Serialize(stream, ~nullMask);
					s.WriteTo(stream);
				}
			}
			else
			{
				Int32Proxy.Serialize(stream, 0);
			}
		}
		
		public static UberStrike.DataCenter.Common.Entities.PlayerLevelCapView Deserialize(Stream bytes)
        {
			int nullMask = Int32Proxy.Deserialize(bytes);
			
			UberStrike.DataCenter.Common.Entities.PlayerLevelCapView instance = null;
			if(nullMask != 0)
			{
            	instance = new UberStrike.DataCenter.Common.Entities.PlayerLevelCapView();
				instance.Level = Int32Proxy.Deserialize(bytes);
				instance.PlayerLevelCapId = Int32Proxy.Deserialize(bytes);
				instance.XPRequired = Int32Proxy.Deserialize(bytes);
			}
			
			return instance;
		}
	}
	public static class UberStrikeItemShopClientViewProxy
	{
		public static void Serialize(Stream stream, UberStrike.Core.Models.Views.UberStrikeItemShopClientView instance)
		{
			int nullMask = 0;
			if(instance != null)
			{
				using (MemoryStream s = new MemoryStream())
                {
					if(instance.FunctionalItems != null) ListProxy<UberStrike.Core.Models.Views.UberStrikeItemFunctionalView>.Serialize(s, instance.FunctionalItems, UberStrikeItemFunctionalViewProxy.Serialize);
					else nullMask |= 1 << 0; 
					if(instance.GearItems != null) ListProxy<UberStrike.Core.Models.Views.UberStrikeItemGearView>.Serialize(s, instance.GearItems, UberStrikeItemGearViewProxy.Serialize);
					else nullMask |= 1 << 1; 
					if(instance.ItemsRecommendationPerMap != null) DictionaryProxy<System.Int32, System.Int32>.Serialize(s, instance.ItemsRecommendationPerMap, Int32Proxy.Serialize, Int32Proxy.Serialize);
					else nullMask |= 1 << 2; 
					if(instance.QuickItems != null) ListProxy<UberStrike.Core.Models.Views.UberStrikeItemQuickView>.Serialize(s, instance.QuickItems, UberStrikeItemQuickViewProxy.Serialize);
					else nullMask |= 1 << 3; 
					if(instance.WeaponItems != null) ListProxy<UberStrike.Core.Models.Views.UberStrikeItemWeaponView>.Serialize(s, instance.WeaponItems, UberStrikeItemWeaponViewProxy.Serialize);
					else nullMask |= 1 << 4; 

					Int32Proxy.Serialize(stream, ~nullMask);
					s.WriteTo(stream);
				}
			}
			else
			{
				Int32Proxy.Serialize(stream, 0);
			}
		}
		
		public static UberStrike.Core.Models.Views.UberStrikeItemShopClientView Deserialize(Stream bytes)
        {
			int nullMask = Int32Proxy.Deserialize(bytes);
			
			UberStrike.Core.Models.Views.UberStrikeItemShopClientView instance = null;
			if(nullMask != 0)
			{
            	instance = new UberStrike.Core.Models.Views.UberStrikeItemShopClientView();
				if((nullMask & (1 << 0)) != 0) instance.FunctionalItems = ListProxy<UberStrike.Core.Models.Views.UberStrikeItemFunctionalView>.Deserialize(bytes, UberStrikeItemFunctionalViewProxy.Deserialize);
				if((nullMask & (1 << 1)) != 0) instance.GearItems = ListProxy<UberStrike.Core.Models.Views.UberStrikeItemGearView>.Deserialize(bytes, UberStrikeItemGearViewProxy.Deserialize);
				if((nullMask & (1 << 2)) != 0) instance.ItemsRecommendationPerMap = DictionaryProxy<System.Int32, System.Int32>.Deserialize(bytes, Int32Proxy.Deserialize, Int32Proxy.Deserialize);
				if((nullMask & (1 << 3)) != 0) instance.QuickItems = ListProxy<UberStrike.Core.Models.Views.UberStrikeItemQuickView>.Deserialize(bytes, UberStrikeItemQuickViewProxy.Deserialize);
				if((nullMask & (1 << 4)) != 0) instance.WeaponItems = ListProxy<UberStrike.Core.Models.Views.UberStrikeItemWeaponView>.Deserialize(bytes, UberStrikeItemWeaponViewProxy.Deserialize);
			}
			
			return instance;
		}
	}
	public static class PlayerPersonalRecordStatisticsViewProxy
	{
		public static void Serialize(Stream stream, UberStrike.DataCenter.Common.Entities.PlayerPersonalRecordStatisticsView instance)
		{
			int nullMask = 0;
			if(instance != null)
			{
				using (MemoryStream s = new MemoryStream())
                {
					Int32Proxy.Serialize(s, instance.MostArmorPickedUp);
					Int32Proxy.Serialize(s, instance.MostCannonSplats);
					Int32Proxy.Serialize(s, instance.MostConsecutiveSnipes);
					Int32Proxy.Serialize(s, instance.MostDamageDealt);
					Int32Proxy.Serialize(s, instance.MostDamageReceived);
					Int32Proxy.Serialize(s, instance.MostHandgunSplats);
					Int32Proxy.Serialize(s, instance.MostHeadshots);
					Int32Proxy.Serialize(s, instance.MostHealthPickedUp);
					Int32Proxy.Serialize(s, instance.MostLauncherSplats);
					Int32Proxy.Serialize(s, instance.MostMachinegunSplats);
					Int32Proxy.Serialize(s, instance.MostMeleeSplats);
					Int32Proxy.Serialize(s, instance.MostNutshots);
					Int32Proxy.Serialize(s, instance.MostShotgunSplats);
					Int32Proxy.Serialize(s, instance.MostSniperSplats);
					Int32Proxy.Serialize(s, instance.MostSplats);
					Int32Proxy.Serialize(s, instance.MostSplattergunSplats);
					Int32Proxy.Serialize(s, instance.MostXPEarned);

					Int32Proxy.Serialize(stream, ~nullMask);
					s.WriteTo(stream);
				}
			}
			else
			{
				Int32Proxy.Serialize(stream, 0);
			}
		}
		
		public static UberStrike.DataCenter.Common.Entities.PlayerPersonalRecordStatisticsView Deserialize(Stream bytes)
        {
			int nullMask = Int32Proxy.Deserialize(bytes);
			
			UberStrike.DataCenter.Common.Entities.PlayerPersonalRecordStatisticsView instance = null;
			if(nullMask != 0)
			{
            	instance = new UberStrike.DataCenter.Common.Entities.PlayerPersonalRecordStatisticsView();
				instance.MostArmorPickedUp = Int32Proxy.Deserialize(bytes);
				instance.MostCannonSplats = Int32Proxy.Deserialize(bytes);
				instance.MostConsecutiveSnipes = Int32Proxy.Deserialize(bytes);
				instance.MostDamageDealt = Int32Proxy.Deserialize(bytes);
				instance.MostDamageReceived = Int32Proxy.Deserialize(bytes);
				instance.MostHandgunSplats = Int32Proxy.Deserialize(bytes);
				instance.MostHeadshots = Int32Proxy.Deserialize(bytes);
				instance.MostHealthPickedUp = Int32Proxy.Deserialize(bytes);
				instance.MostLauncherSplats = Int32Proxy.Deserialize(bytes);
				instance.MostMachinegunSplats = Int32Proxy.Deserialize(bytes);
				instance.MostMeleeSplats = Int32Proxy.Deserialize(bytes);
				instance.MostNutshots = Int32Proxy.Deserialize(bytes);
				instance.MostShotgunSplats = Int32Proxy.Deserialize(bytes);
				instance.MostSniperSplats = Int32Proxy.Deserialize(bytes);
				instance.MostSplats = Int32Proxy.Deserialize(bytes);
				instance.MostSplattergunSplats = Int32Proxy.Deserialize(bytes);
				instance.MostXPEarned = Int32Proxy.Deserialize(bytes);
			}
			
			return instance;
		}
	}
}
