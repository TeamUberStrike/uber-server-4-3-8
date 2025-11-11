// IGNORED: BannedIpView
// IGNORED: MysteryBoxItemView
// IGNORED: BoxTransactionView
// IGNORED: LuckyDrawSetItemView
// IGNORED: CreditPackView
// IGNORED: PrizeElementView
// IGNORED: PhotonsClusterView
// IGNORED: EpinBatchView
// IGNORED: EpinView
// IGNORED: LuckyDrawView
// IGNORED: CommonConfig
// IGNORED: CommonAppSettings
// IGNORED: DatabaseDeployment
// IGNORED: CmuneCacheKeys
// IGNORED: ItemView
// IGNORED: BuyItemResult
// IGNORED: AccountCompletionResult
// IGNORED: GroupOperationResult
// IGNORED: CreditPackItemView
// IGNORED: LuckyDrawSetView
// IGNORED: ConvertEntities
// IGNORED: ApplicationMilestoneView
// IGNORED: MysteryBoxView
// IGNORED: ModerationActionView
using System.IO;

namespace UberStrike.Core.Serialization
{
	public static class LuckyDrawSetUnityViewProxy
	{
		public static void Serialize(Stream stream, Cmune.DataCenter.Common.Entities.LuckyDrawSetUnityView instance)
		{
			int nullMask = 0;
			if(instance != null)
			{
				using (MemoryStream s = new MemoryStream())
                {
					Int32Proxy.Serialize(s, instance.CreditsAttributed);
					BooleanProxy.Serialize(s, instance.ExposeItemsToPlayers);
					Int32Proxy.Serialize(s, instance.Id);
					if(instance.ImageUrl != null) StringProxy.Serialize(s, instance.ImageUrl);
					else nullMask |= 1 << 0; 
					Int32Proxy.Serialize(s, instance.LuckyDrawId);
					if(instance.LuckyDrawSetItems != null) ListProxy<Cmune.DataCenter.Common.Entities.BundleItemView>.Serialize(s, instance.LuckyDrawSetItems, BundleItemViewProxy.Serialize);
					else nullMask |= 1 << 1; 
					Int32Proxy.Serialize(s, instance.PointsAttributed);
					Int32Proxy.Serialize(s, instance.SetWeight);

					Int32Proxy.Serialize(stream, ~nullMask);
					s.WriteTo(stream);
				}
			}
			else
			{
				Int32Proxy.Serialize(stream, 0);
			}
		}
		
		public static Cmune.DataCenter.Common.Entities.LuckyDrawSetUnityView Deserialize(Stream bytes)
        {
			int nullMask = Int32Proxy.Deserialize(bytes);
			
			Cmune.DataCenter.Common.Entities.LuckyDrawSetUnityView instance = null;
			if(nullMask != 0)
			{
            	instance = new Cmune.DataCenter.Common.Entities.LuckyDrawSetUnityView();
				instance.CreditsAttributed = Int32Proxy.Deserialize(bytes);
				instance.ExposeItemsToPlayers = BooleanProxy.Deserialize(bytes);
				instance.Id = Int32Proxy.Deserialize(bytes);
				if((nullMask & (1 << 0)) != 0) instance.ImageUrl = StringProxy.Deserialize(bytes);
				instance.LuckyDrawId = Int32Proxy.Deserialize(bytes);
				if((nullMask & (1 << 1)) != 0) instance.LuckyDrawSetItems = ListProxy<Cmune.DataCenter.Common.Entities.BundleItemView>.Deserialize(bytes, BundleItemViewProxy.Deserialize);
				instance.PointsAttributed = Int32Proxy.Deserialize(bytes);
				instance.SetWeight = Int32Proxy.Deserialize(bytes);
			}
			
			return instance;
		}
	}
	public static class PublicProfileViewProxy
	{
		public static void Serialize(Stream stream, Cmune.DataCenter.Common.Entities.PublicProfileView instance)
		{
			int nullMask = 0;
			if(instance != null)
			{
				using (MemoryStream s = new MemoryStream())
                {
					EnumProxy<Cmune.DataCenter.Common.Entities.MemberAccessLevel>.Serialize(s, instance.AccessLevel);
					Int32Proxy.Serialize(s, instance.Cmid);
					EnumProxy<Cmune.DataCenter.Common.Entities.EmailAddressStatus>.Serialize(s, instance.EmailAddressStatus);
					if(instance.GroupTag != null) StringProxy.Serialize(s, instance.GroupTag);
					else nullMask |= 1 << 0; 
					BooleanProxy.Serialize(s, instance.IsChatDisabled);
					DateTimeProxy.Serialize(s, instance.LastLoginDate);
					if(instance.Name != null) StringProxy.Serialize(s, instance.Name);
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
		
		public static Cmune.DataCenter.Common.Entities.PublicProfileView Deserialize(Stream bytes)
        {
			int nullMask = Int32Proxy.Deserialize(bytes);
			
			Cmune.DataCenter.Common.Entities.PublicProfileView instance = null;
			if(nullMask != 0)
			{
            	instance = new Cmune.DataCenter.Common.Entities.PublicProfileView();
				instance.AccessLevel = EnumProxy<Cmune.DataCenter.Common.Entities.MemberAccessLevel>.Deserialize(bytes);
				instance.Cmid = Int32Proxy.Deserialize(bytes);
				instance.EmailAddressStatus = EnumProxy<Cmune.DataCenter.Common.Entities.EmailAddressStatus>.Deserialize(bytes);
				if((nullMask & (1 << 0)) != 0) instance.GroupTag = StringProxy.Deserialize(bytes);
				instance.IsChatDisabled = BooleanProxy.Deserialize(bytes);
				instance.LastLoginDate = DateTimeProxy.Deserialize(bytes);
				if((nullMask & (1 << 1)) != 0) instance.Name = StringProxy.Deserialize(bytes);
			}
			
			return instance;
		}
	}
	public static class PackageViewProxy
	{
		public static void Serialize(Stream stream, Cmune.DataCenter.Common.Entities.PackageView instance)
		{
			int nullMask = 0;
			if(instance != null)
			{
				using (MemoryStream s = new MemoryStream())
                {
					Int32Proxy.Serialize(s, instance.Bonus);
					if(instance.Items != null) ListProxy<System.Int32>.Serialize(s, instance.Items, Int32Proxy.Serialize);
					else nullMask |= 1 << 0; 
					if(instance.Name != null) StringProxy.Serialize(s, instance.Name);
					else nullMask |= 1 << 1; 
					DecimalProxy.Serialize(s, instance.Price);

					Int32Proxy.Serialize(stream, ~nullMask);
					s.WriteTo(stream);
				}
			}
			else
			{
				Int32Proxy.Serialize(stream, 0);
			}
		}
		
		public static Cmune.DataCenter.Common.Entities.PackageView Deserialize(Stream bytes)
        {
			int nullMask = Int32Proxy.Deserialize(bytes);
			
			Cmune.DataCenter.Common.Entities.PackageView instance = null;
			if(nullMask != 0)
			{
            	instance = new Cmune.DataCenter.Common.Entities.PackageView();
				instance.Bonus = Int32Proxy.Deserialize(bytes);
				if((nullMask & (1 << 0)) != 0) instance.Items = ListProxy<System.Int32>.Deserialize(bytes, Int32Proxy.Deserialize);
				if((nullMask & (1 << 1)) != 0) instance.Name = StringProxy.Deserialize(bytes);
				instance.Price = DecimalProxy.Deserialize(bytes);
			}
			
			return instance;
		}
	}
	public static class PrivateMessageViewProxy
	{
		public static void Serialize(Stream stream, Cmune.DataCenter.Common.Entities.PrivateMessageView instance)
		{
			int nullMask = 0;
			if(instance != null)
			{
				using (MemoryStream s = new MemoryStream())
                {
					if(instance.ContentText != null) StringProxy.Serialize(s, instance.ContentText);
					else nullMask |= 1 << 0; 
					DateTimeProxy.Serialize(s, instance.DateSent);
					Int32Proxy.Serialize(s, instance.FromCmid);
					if(instance.FromName != null) StringProxy.Serialize(s, instance.FromName);
					else nullMask |= 1 << 1; 
					BooleanProxy.Serialize(s, instance.HasAttachment);
					BooleanProxy.Serialize(s, instance.IsDeletedByReceiver);
					BooleanProxy.Serialize(s, instance.IsDeletedBySender);
					BooleanProxy.Serialize(s, instance.IsRead);
					Int32Proxy.Serialize(s, instance.PrivateMessageId);
					Int32Proxy.Serialize(s, instance.ToCmid);

					Int32Proxy.Serialize(stream, ~nullMask);
					s.WriteTo(stream);
				}
			}
			else
			{
				Int32Proxy.Serialize(stream, 0);
			}
		}
		
		public static Cmune.DataCenter.Common.Entities.PrivateMessageView Deserialize(Stream bytes)
        {
			int nullMask = Int32Proxy.Deserialize(bytes);
			
			Cmune.DataCenter.Common.Entities.PrivateMessageView instance = null;
			if(nullMask != 0)
			{
            	instance = new Cmune.DataCenter.Common.Entities.PrivateMessageView();
				if((nullMask & (1 << 0)) != 0) instance.ContentText = StringProxy.Deserialize(bytes);
				instance.DateSent = DateTimeProxy.Deserialize(bytes);
				instance.FromCmid = Int32Proxy.Deserialize(bytes);
				if((nullMask & (1 << 1)) != 0) instance.FromName = StringProxy.Deserialize(bytes);
				instance.HasAttachment = BooleanProxy.Deserialize(bytes);
				instance.IsDeletedByReceiver = BooleanProxy.Deserialize(bytes);
				instance.IsDeletedBySender = BooleanProxy.Deserialize(bytes);
				instance.IsRead = BooleanProxy.Deserialize(bytes);
				instance.PrivateMessageId = Int32Proxy.Deserialize(bytes);
				instance.ToCmid = Int32Proxy.Deserialize(bytes);
			}
			
			return instance;
		}
	}
	public static class PhotonViewProxy
	{
		public static void Serialize(Stream stream, Cmune.Core.Models.Views.PhotonView instance)
		{
			int nullMask = 0;
			if(instance != null)
			{
				using (MemoryStream s = new MemoryStream())
                {
					if(instance.IP != null) StringProxy.Serialize(s, instance.IP);
					else nullMask |= 1 << 0; 
					Int32Proxy.Serialize(s, instance.MinLatency);
					if(instance.Name != null) StringProxy.Serialize(s, instance.Name);
					else nullMask |= 1 << 1; 
					Int32Proxy.Serialize(s, instance.PhotonId);
					Int32Proxy.Serialize(s, instance.Port);
					EnumProxy<Cmune.DataCenter.Common.Entities.RegionType>.Serialize(s, instance.Region);
					EnumProxy<Cmune.DataCenter.Common.Entities.PhotonUsageType>.Serialize(s, instance.UsageType);

					Int32Proxy.Serialize(stream, ~nullMask);
					s.WriteTo(stream);
				}
			}
			else
			{
				Int32Proxy.Serialize(stream, 0);
			}
		}
		
		public static Cmune.Core.Models.Views.PhotonView Deserialize(Stream bytes)
        {
			int nullMask = Int32Proxy.Deserialize(bytes);
			
			Cmune.Core.Models.Views.PhotonView instance = null;
			if(nullMask != 0)
			{
            	instance = new Cmune.Core.Models.Views.PhotonView();
				if((nullMask & (1 << 0)) != 0) instance.IP = StringProxy.Deserialize(bytes);
				instance.MinLatency = Int32Proxy.Deserialize(bytes);
				if((nullMask & (1 << 1)) != 0) instance.Name = StringProxy.Deserialize(bytes);
				instance.PhotonId = Int32Proxy.Deserialize(bytes);
				instance.Port = Int32Proxy.Deserialize(bytes);
				instance.Region = EnumProxy<Cmune.DataCenter.Common.Entities.RegionType>.Deserialize(bytes);
				instance.UsageType = EnumProxy<Cmune.DataCenter.Common.Entities.PhotonUsageType>.Deserialize(bytes);
			}
			
			return instance;
		}
	}
	public static class MysteryBoxWonItemUnityViewProxy
	{
		public static void Serialize(Stream stream, Cmune.DataCenter.Common.Entities.MysteryBoxWonItemUnityView instance)
		{
			int nullMask = 0;
			if(instance != null)
			{
				using (MemoryStream s = new MemoryStream())
                {
					Int32Proxy.Serialize(s, instance.CreditWon);
					Int32Proxy.Serialize(s, instance.ItemIdWon);
					Int32Proxy.Serialize(s, instance.PointWon);

					Int32Proxy.Serialize(stream, ~nullMask);
					s.WriteTo(stream);
				}
			}
			else
			{
				Int32Proxy.Serialize(stream, 0);
			}
		}
		
		public static Cmune.DataCenter.Common.Entities.MysteryBoxWonItemUnityView Deserialize(Stream bytes)
        {
			int nullMask = Int32Proxy.Deserialize(bytes);
			
			Cmune.DataCenter.Common.Entities.MysteryBoxWonItemUnityView instance = null;
			if(nullMask != 0)
			{
            	instance = new Cmune.DataCenter.Common.Entities.MysteryBoxWonItemUnityView();
				instance.CreditWon = Int32Proxy.Deserialize(bytes);
				instance.ItemIdWon = Int32Proxy.Deserialize(bytes);
				instance.PointWon = Int32Proxy.Deserialize(bytes);
			}
			
			return instance;
		}
	}
	public static class MemberViewProxy
	{
		public static void Serialize(Stream stream, Cmune.DataCenter.Common.Entities.MemberView instance)
		{
			int nullMask = 0;
			if(instance != null)
			{
				using (MemoryStream s = new MemoryStream())
                {
					if(instance.MemberItems != null) ListProxy<System.Int32>.Serialize(s, instance.MemberItems, Int32Proxy.Serialize);
					else nullMask |= 1 << 0; 
					if(instance.MemberWallet != null) MemberWalletViewProxy.Serialize(s, instance.MemberWallet);
					else nullMask |= 1 << 1; 
					if(instance.PublicProfile != null) PublicProfileViewProxy.Serialize(s, instance.PublicProfile);
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
		
		public static Cmune.DataCenter.Common.Entities.MemberView Deserialize(Stream bytes)
        {
			int nullMask = Int32Proxy.Deserialize(bytes);
			
			Cmune.DataCenter.Common.Entities.MemberView instance = null;
			if(nullMask != 0)
			{
            	instance = new Cmune.DataCenter.Common.Entities.MemberView();
				if((nullMask & (1 << 0)) != 0) instance.MemberItems = ListProxy<System.Int32>.Deserialize(bytes, Int32Proxy.Deserialize);
				if((nullMask & (1 << 1)) != 0) instance.MemberWallet = MemberWalletViewProxy.Deserialize(bytes);
				if((nullMask & (1 << 2)) != 0) instance.PublicProfile = PublicProfileViewProxy.Deserialize(bytes);
			}
			
			return instance;
		}
	}
	public static class GroupCreationViewProxy
	{
		public static void Serialize(Stream stream, Cmune.DataCenter.Common.Entities.GroupCreationView instance)
		{
			int nullMask = 0;
			if(instance != null)
			{
				using (MemoryStream s = new MemoryStream())
                {
					if(instance.Address != null) StringProxy.Serialize(s, instance.Address);
					else nullMask |= 1 << 0; 
					Int32Proxy.Serialize(s, instance.ApplicationId);
					Int32Proxy.Serialize(s, instance.Cmid);
					if(instance.Description != null) StringProxy.Serialize(s, instance.Description);
					else nullMask |= 1 << 1; 
					BooleanProxy.Serialize(s, instance.HasPicture);
					if(instance.Locale != null) StringProxy.Serialize(s, instance.Locale);
					else nullMask |= 1 << 2; 
					if(instance.Motto != null) StringProxy.Serialize(s, instance.Motto);
					else nullMask |= 1 << 3; 
					if(instance.Name != null) StringProxy.Serialize(s, instance.Name);
					else nullMask |= 1 << 4; 
					if(instance.Tag != null) StringProxy.Serialize(s, instance.Tag);
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
		
		public static Cmune.DataCenter.Common.Entities.GroupCreationView Deserialize(Stream bytes)
        {
			int nullMask = Int32Proxy.Deserialize(bytes);
			
			Cmune.DataCenter.Common.Entities.GroupCreationView instance = null;
			if(nullMask != 0)
			{
            	instance = new Cmune.DataCenter.Common.Entities.GroupCreationView();
				if((nullMask & (1 << 0)) != 0) instance.Address = StringProxy.Deserialize(bytes);
				instance.ApplicationId = Int32Proxy.Deserialize(bytes);
				instance.Cmid = Int32Proxy.Deserialize(bytes);
				if((nullMask & (1 << 1)) != 0) instance.Description = StringProxy.Deserialize(bytes);
				instance.HasPicture = BooleanProxy.Deserialize(bytes);
				if((nullMask & (1 << 2)) != 0) instance.Locale = StringProxy.Deserialize(bytes);
				if((nullMask & (1 << 3)) != 0) instance.Motto = StringProxy.Deserialize(bytes);
				if((nullMask & (1 << 4)) != 0) instance.Name = StringProxy.Deserialize(bytes);
				if((nullMask & (1 << 5)) != 0) instance.Tag = StringProxy.Deserialize(bytes);
			}
			
			return instance;
		}
	}
	public static class ClanRequestAcceptViewProxy
	{
		public static void Serialize(Stream stream, Cmune.DataCenter.Common.Entities.ClanRequestAcceptView instance)
		{
			int nullMask = 0;
			if(instance != null)
			{
				using (MemoryStream s = new MemoryStream())
                {
					Int32Proxy.Serialize(s, instance.ActionResult);
					Int32Proxy.Serialize(s, instance.ClanRequestId);
					if(instance.ClanView != null) ClanViewProxy.Serialize(s, instance.ClanView);
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
		
		public static Cmune.DataCenter.Common.Entities.ClanRequestAcceptView Deserialize(Stream bytes)
        {
			int nullMask = Int32Proxy.Deserialize(bytes);
			
			Cmune.DataCenter.Common.Entities.ClanRequestAcceptView instance = null;
			if(nullMask != 0)
			{
            	instance = new Cmune.DataCenter.Common.Entities.ClanRequestAcceptView();
				instance.ActionResult = Int32Proxy.Deserialize(bytes);
				instance.ClanRequestId = Int32Proxy.Deserialize(bytes);
				if((nullMask & (1 << 0)) != 0) instance.ClanView = ClanViewProxy.Deserialize(bytes);
			}
			
			return instance;
		}
	}
	public static class BundleViewProxy
	{
		public static void Serialize(Stream stream, Cmune.DataCenter.Common.Entities.BundleView instance)
		{
			int nullMask = 0;
			if(instance != null)
			{
				using (MemoryStream s = new MemoryStream())
                {
					Int32Proxy.Serialize(s, instance.ApplicationId);
					if(instance.Availability != null) ListProxy<Cmune.DataCenter.Common.Entities.ChannelType>.Serialize(s, instance.Availability, EnumProxy<Cmune.DataCenter.Common.Entities.ChannelType>.Serialize);
					else nullMask |= 1 << 0; 
					if(instance.BundleItemViews != null) ListProxy<Cmune.DataCenter.Common.Entities.BundleItemView>.Serialize(s, instance.BundleItemViews, BundleItemViewProxy.Serialize);
					else nullMask |= 1 << 1; 
					EnumProxy<Cmune.DataCenter.Common.Entities.BundleCategoryType>.Serialize(s, instance.Category);
					Int32Proxy.Serialize(s, instance.Credits);
					if(instance.Description != null) StringProxy.Serialize(s, instance.Description);
					else nullMask |= 1 << 2; 
					if(instance.IconUrl != null) StringProxy.Serialize(s, instance.IconUrl);
					else nullMask |= 1 << 3; 
					Int32Proxy.Serialize(s, instance.Id);
					if(instance.ImageUrl != null) StringProxy.Serialize(s, instance.ImageUrl);
					else nullMask |= 1 << 4; 
					if(instance.IosAppStoreUniqueId != null) StringProxy.Serialize(s, instance.IosAppStoreUniqueId);
					else nullMask |= 1 << 5; 
					BooleanProxy.Serialize(s, instance.IsDefault);
					BooleanProxy.Serialize(s, instance.IsOnSale);
					BooleanProxy.Serialize(s, instance.IsPromoted);
					if(instance.MacAppStoreUniqueId != null) StringProxy.Serialize(s, instance.MacAppStoreUniqueId);
					else nullMask |= 1 << 6; 
					if(instance.Name != null) StringProxy.Serialize(s, instance.Name);
					else nullMask |= 1 << 7; 
					Int32Proxy.Serialize(s, instance.Points);
					if(instance.PromotionTag != null) StringProxy.Serialize(s, instance.PromotionTag);
					else nullMask |= 1 << 8; 
					DecimalProxy.Serialize(s, instance.USDPrice);
					DecimalProxy.Serialize(s, instance.USDPromoPrice);

					Int32Proxy.Serialize(stream, ~nullMask);
					s.WriteTo(stream);
				}
			}
			else
			{
				Int32Proxy.Serialize(stream, 0);
			}
		}
		
		public static Cmune.DataCenter.Common.Entities.BundleView Deserialize(Stream bytes)
        {
			int nullMask = Int32Proxy.Deserialize(bytes);
			
			Cmune.DataCenter.Common.Entities.BundleView instance = null;
			if(nullMask != 0)
			{
            	instance = new Cmune.DataCenter.Common.Entities.BundleView();
				instance.ApplicationId = Int32Proxy.Deserialize(bytes);
				if((nullMask & (1 << 0)) != 0) instance.Availability = ListProxy<Cmune.DataCenter.Common.Entities.ChannelType>.Deserialize(bytes, EnumProxy<Cmune.DataCenter.Common.Entities.ChannelType>.Deserialize);
				if((nullMask & (1 << 1)) != 0) instance.BundleItemViews = ListProxy<Cmune.DataCenter.Common.Entities.BundleItemView>.Deserialize(bytes, BundleItemViewProxy.Deserialize);
				instance.Category = EnumProxy<Cmune.DataCenter.Common.Entities.BundleCategoryType>.Deserialize(bytes);
				instance.Credits = Int32Proxy.Deserialize(bytes);
				if((nullMask & (1 << 2)) != 0) instance.Description = StringProxy.Deserialize(bytes);
				if((nullMask & (1 << 3)) != 0) instance.IconUrl = StringProxy.Deserialize(bytes);
				instance.Id = Int32Proxy.Deserialize(bytes);
				if((nullMask & (1 << 4)) != 0) instance.ImageUrl = StringProxy.Deserialize(bytes);
				if((nullMask & (1 << 5)) != 0) instance.IosAppStoreUniqueId = StringProxy.Deserialize(bytes);
				instance.IsDefault = BooleanProxy.Deserialize(bytes);
				instance.IsOnSale = BooleanProxy.Deserialize(bytes);
				instance.IsPromoted = BooleanProxy.Deserialize(bytes);
				if((nullMask & (1 << 6)) != 0) instance.MacAppStoreUniqueId = StringProxy.Deserialize(bytes);
				if((nullMask & (1 << 7)) != 0) instance.Name = StringProxy.Deserialize(bytes);
				instance.Points = Int32Proxy.Deserialize(bytes);
				if((nullMask & (1 << 8)) != 0) instance.PromotionTag = StringProxy.Deserialize(bytes);
				instance.USDPrice = DecimalProxy.Deserialize(bytes);
				instance.USDPromoPrice = DecimalProxy.Deserialize(bytes);
			}
			
			return instance;
		}
	}
	public static class ItemTransactionViewProxy
	{
		public static void Serialize(Stream stream, Cmune.DataCenter.Common.Entities.ItemTransactionView instance)
		{
			int nullMask = 0;
			if(instance != null)
			{
				using (MemoryStream s = new MemoryStream())
                {
					Int32Proxy.Serialize(s, instance.Cmid);
					Int32Proxy.Serialize(s, instance.Credits);
					EnumProxy<Cmune.DataCenter.Common.Entities.BuyingDurationType>.Serialize(s, instance.Duration);
					BooleanProxy.Serialize(s, instance.IsAdminAction);
					Int32Proxy.Serialize(s, instance.ItemId);
					Int32Proxy.Serialize(s, instance.Points);
					DateTimeProxy.Serialize(s, instance.WithdrawalDate);
					Int32Proxy.Serialize(s, instance.WithdrawalId);

					Int32Proxy.Serialize(stream, ~nullMask);
					s.WriteTo(stream);
				}
			}
			else
			{
				Int32Proxy.Serialize(stream, 0);
			}
		}
		
		public static Cmune.DataCenter.Common.Entities.ItemTransactionView Deserialize(Stream bytes)
        {
			int nullMask = Int32Proxy.Deserialize(bytes);
			
			Cmune.DataCenter.Common.Entities.ItemTransactionView instance = null;
			if(nullMask != 0)
			{
            	instance = new Cmune.DataCenter.Common.Entities.ItemTransactionView();
				instance.Cmid = Int32Proxy.Deserialize(bytes);
				instance.Credits = Int32Proxy.Deserialize(bytes);
				instance.Duration = EnumProxy<Cmune.DataCenter.Common.Entities.BuyingDurationType>.Deserialize(bytes);
				instance.IsAdminAction = BooleanProxy.Deserialize(bytes);
				instance.ItemId = Int32Proxy.Deserialize(bytes);
				instance.Points = Int32Proxy.Deserialize(bytes);
				instance.WithdrawalDate = DateTimeProxy.Deserialize(bytes);
				instance.WithdrawalId = Int32Proxy.Deserialize(bytes);
			}
			
			return instance;
		}
	}
	public static class GroupInvitationViewProxy
	{
		public static void Serialize(Stream stream, Cmune.DataCenter.Common.Entities.GroupInvitationView instance)
		{
			int nullMask = 0;
			if(instance != null)
			{
				using (MemoryStream s = new MemoryStream())
                {
					Int32Proxy.Serialize(s, instance.GroupId);
					Int32Proxy.Serialize(s, instance.GroupInvitationId);
					if(instance.GroupName != null) StringProxy.Serialize(s, instance.GroupName);
					else nullMask |= 1 << 0; 
					if(instance.GroupTag != null) StringProxy.Serialize(s, instance.GroupTag);
					else nullMask |= 1 << 1; 
					Int32Proxy.Serialize(s, instance.InviteeCmid);
					if(instance.InviteeName != null) StringProxy.Serialize(s, instance.InviteeName);
					else nullMask |= 1 << 2; 
					Int32Proxy.Serialize(s, instance.InviterCmid);
					if(instance.InviterName != null) StringProxy.Serialize(s, instance.InviterName);
					else nullMask |= 1 << 3; 
					if(instance.Message != null) StringProxy.Serialize(s, instance.Message);
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
		
		public static Cmune.DataCenter.Common.Entities.GroupInvitationView Deserialize(Stream bytes)
        {
			int nullMask = Int32Proxy.Deserialize(bytes);
			
			Cmune.DataCenter.Common.Entities.GroupInvitationView instance = null;
			if(nullMask != 0)
			{
            	instance = new Cmune.DataCenter.Common.Entities.GroupInvitationView();
				instance.GroupId = Int32Proxy.Deserialize(bytes);
				instance.GroupInvitationId = Int32Proxy.Deserialize(bytes);
				if((nullMask & (1 << 0)) != 0) instance.GroupName = StringProxy.Deserialize(bytes);
				if((nullMask & (1 << 1)) != 0) instance.GroupTag = StringProxy.Deserialize(bytes);
				instance.InviteeCmid = Int32Proxy.Deserialize(bytes);
				if((nullMask & (1 << 2)) != 0) instance.InviteeName = StringProxy.Deserialize(bytes);
				instance.InviterCmid = Int32Proxy.Deserialize(bytes);
				if((nullMask & (1 << 3)) != 0) instance.InviterName = StringProxy.Deserialize(bytes);
				if((nullMask & (1 << 4)) != 0) instance.Message = StringProxy.Deserialize(bytes);
			}
			
			return instance;
		}
	}
	public static class CurrencyDepositViewProxy
	{
		public static void Serialize(Stream stream, Cmune.DataCenter.Common.Entities.CurrencyDepositView instance)
		{
			int nullMask = 0;
			if(instance != null)
			{
				using (MemoryStream s = new MemoryStream())
                {
					Int32Proxy.Serialize(s, instance.ApplicationId);
					if(instance.BundleId != null) Int32Proxy.Serialize(s, instance.BundleId ?? default(System.Int32));
					else nullMask |= 1 << 0; 
					if(instance.BundleName != null) StringProxy.Serialize(s, instance.BundleName);
					else nullMask |= 1 << 1; 
					DecimalProxy.Serialize(s, instance.Cash);
					EnumProxy<Cmune.DataCenter.Common.Entities.ChannelType>.Serialize(s, instance.ChannelId);
					Int32Proxy.Serialize(s, instance.Cmid);
					Int32Proxy.Serialize(s, instance.Credits);
					Int32Proxy.Serialize(s, instance.CreditsDepositId);
					if(instance.CurrencyLabel != null) StringProxy.Serialize(s, instance.CurrencyLabel);
					else nullMask |= 1 << 2; 
					DateTimeProxy.Serialize(s, instance.DepositDate);
					BooleanProxy.Serialize(s, instance.IsAdminAction);
					EnumProxy<Cmune.DataCenter.Common.Entities.PaymentProviderType>.Serialize(s, instance.PaymentProviderId);
					Int32Proxy.Serialize(s, instance.Points);
					if(instance.TransactionKey != null) StringProxy.Serialize(s, instance.TransactionKey);
					else nullMask |= 1 << 3; 
					DecimalProxy.Serialize(s, instance.UsdAmount);

					Int32Proxy.Serialize(stream, ~nullMask);
					s.WriteTo(stream);
				}
			}
			else
			{
				Int32Proxy.Serialize(stream, 0);
			}
		}
		
		public static Cmune.DataCenter.Common.Entities.CurrencyDepositView Deserialize(Stream bytes)
        {
			int nullMask = Int32Proxy.Deserialize(bytes);
			
			Cmune.DataCenter.Common.Entities.CurrencyDepositView instance = null;
			if(nullMask != 0)
			{
            	instance = new Cmune.DataCenter.Common.Entities.CurrencyDepositView();
				instance.ApplicationId = Int32Proxy.Deserialize(bytes);
				if((nullMask & (1 << 0)) != 0) instance.BundleId = Int32Proxy.Deserialize(bytes);
				if((nullMask & (1 << 1)) != 0) instance.BundleName = StringProxy.Deserialize(bytes);
				instance.Cash = DecimalProxy.Deserialize(bytes);
				instance.ChannelId = EnumProxy<Cmune.DataCenter.Common.Entities.ChannelType>.Deserialize(bytes);
				instance.Cmid = Int32Proxy.Deserialize(bytes);
				instance.Credits = Int32Proxy.Deserialize(bytes);
				instance.CreditsDepositId = Int32Proxy.Deserialize(bytes);
				if((nullMask & (1 << 2)) != 0) instance.CurrencyLabel = StringProxy.Deserialize(bytes);
				instance.DepositDate = DateTimeProxy.Deserialize(bytes);
				instance.IsAdminAction = BooleanProxy.Deserialize(bytes);
				instance.PaymentProviderId = EnumProxy<Cmune.DataCenter.Common.Entities.PaymentProviderType>.Deserialize(bytes);
				instance.Points = Int32Proxy.Deserialize(bytes);
				if((nullMask & (1 << 3)) != 0) instance.TransactionKey = StringProxy.Deserialize(bytes);
				instance.UsdAmount = DecimalProxy.Deserialize(bytes);
			}
			
			return instance;
		}
	}
	public static class BasicClanViewProxy
	{
		public static void Serialize(Stream stream, Cmune.DataCenter.Common.Entities.BasicClanView instance)
		{
			int nullMask = 0;
			if(instance != null)
			{
				using (MemoryStream s = new MemoryStream())
                {
					if(instance.Address != null) StringProxy.Serialize(s, instance.Address);
					else nullMask |= 1 << 0; 
					Int32Proxy.Serialize(s, instance.ApplicationId);
					EnumProxy<Cmune.DataCenter.Common.Entities.GroupColor>.Serialize(s, instance.ColorStyle);
					if(instance.Description != null) StringProxy.Serialize(s, instance.Description);
					else nullMask |= 1 << 1; 
					EnumProxy<Cmune.DataCenter.Common.Entities.GroupFontStyle>.Serialize(s, instance.FontStyle);
					DateTimeProxy.Serialize(s, instance.FoundingDate);
					Int32Proxy.Serialize(s, instance.GroupId);
					DateTimeProxy.Serialize(s, instance.LastUpdated);
					Int32Proxy.Serialize(s, instance.MembersCount);
					Int32Proxy.Serialize(s, instance.MembersLimit);
					if(instance.Motto != null) StringProxy.Serialize(s, instance.Motto);
					else nullMask |= 1 << 2; 
					if(instance.Name != null) StringProxy.Serialize(s, instance.Name);
					else nullMask |= 1 << 3; 
					Int32Proxy.Serialize(s, instance.OwnerCmid);
					if(instance.OwnerName != null) StringProxy.Serialize(s, instance.OwnerName);
					else nullMask |= 1 << 4; 
					if(instance.Picture != null) StringProxy.Serialize(s, instance.Picture);
					else nullMask |= 1 << 5; 
					if(instance.Tag != null) StringProxy.Serialize(s, instance.Tag);
					else nullMask |= 1 << 6; 
					EnumProxy<Cmune.DataCenter.Common.Entities.GroupType>.Serialize(s, instance.Type);

					Int32Proxy.Serialize(stream, ~nullMask);
					s.WriteTo(stream);
				}
			}
			else
			{
				Int32Proxy.Serialize(stream, 0);
			}
		}
		
		public static Cmune.DataCenter.Common.Entities.BasicClanView Deserialize(Stream bytes)
        {
			int nullMask = Int32Proxy.Deserialize(bytes);
			
			Cmune.DataCenter.Common.Entities.BasicClanView instance = null;
			if(nullMask != 0)
			{
            	instance = new Cmune.DataCenter.Common.Entities.BasicClanView();
				if((nullMask & (1 << 0)) != 0) instance.Address = StringProxy.Deserialize(bytes);
				instance.ApplicationId = Int32Proxy.Deserialize(bytes);
				instance.ColorStyle = EnumProxy<Cmune.DataCenter.Common.Entities.GroupColor>.Deserialize(bytes);
				if((nullMask & (1 << 1)) != 0) instance.Description = StringProxy.Deserialize(bytes);
				instance.FontStyle = EnumProxy<Cmune.DataCenter.Common.Entities.GroupFontStyle>.Deserialize(bytes);
				instance.FoundingDate = DateTimeProxy.Deserialize(bytes);
				instance.GroupId = Int32Proxy.Deserialize(bytes);
				instance.LastUpdated = DateTimeProxy.Deserialize(bytes);
				instance.MembersCount = Int32Proxy.Deserialize(bytes);
				instance.MembersLimit = Int32Proxy.Deserialize(bytes);
				if((nullMask & (1 << 2)) != 0) instance.Motto = StringProxy.Deserialize(bytes);
				if((nullMask & (1 << 3)) != 0) instance.Name = StringProxy.Deserialize(bytes);
				instance.OwnerCmid = Int32Proxy.Deserialize(bytes);
				if((nullMask & (1 << 4)) != 0) instance.OwnerName = StringProxy.Deserialize(bytes);
				if((nullMask & (1 << 5)) != 0) instance.Picture = StringProxy.Deserialize(bytes);
				if((nullMask & (1 << 6)) != 0) instance.Tag = StringProxy.Deserialize(bytes);
				instance.Type = EnumProxy<Cmune.DataCenter.Common.Entities.GroupType>.Deserialize(bytes);
			}
			
			return instance;
		}
	}
	public static class ClanViewProxy
	{
		public static void Serialize(Stream stream, Cmune.DataCenter.Common.Entities.ClanView instance)
		{
			int nullMask = 0;
			if(instance != null)
			{
				using (MemoryStream s = new MemoryStream())
                {
					if(instance.Address != null) StringProxy.Serialize(s, instance.Address);
					else nullMask |= 1 << 0; 
					Int32Proxy.Serialize(s, instance.ApplicationId);
					EnumProxy<Cmune.DataCenter.Common.Entities.GroupColor>.Serialize(s, instance.ColorStyle);
					if(instance.Description != null) StringProxy.Serialize(s, instance.Description);
					else nullMask |= 1 << 1; 
					EnumProxy<Cmune.DataCenter.Common.Entities.GroupFontStyle>.Serialize(s, instance.FontStyle);
					DateTimeProxy.Serialize(s, instance.FoundingDate);
					Int32Proxy.Serialize(s, instance.GroupId);
					DateTimeProxy.Serialize(s, instance.LastUpdated);
					if(instance.Members != null) ListProxy<Cmune.DataCenter.Common.Entities.ClanMemberView>.Serialize(s, instance.Members, ClanMemberViewProxy.Serialize);
					else nullMask |= 1 << 2; 
					Int32Proxy.Serialize(s, instance.MembersCount);
					Int32Proxy.Serialize(s, instance.MembersLimit);
					if(instance.Motto != null) StringProxy.Serialize(s, instance.Motto);
					else nullMask |= 1 << 3; 
					if(instance.Name != null) StringProxy.Serialize(s, instance.Name);
					else nullMask |= 1 << 4; 
					Int32Proxy.Serialize(s, instance.OwnerCmid);
					if(instance.OwnerName != null) StringProxy.Serialize(s, instance.OwnerName);
					else nullMask |= 1 << 5; 
					if(instance.Picture != null) StringProxy.Serialize(s, instance.Picture);
					else nullMask |= 1 << 6; 
					if(instance.Tag != null) StringProxy.Serialize(s, instance.Tag);
					else nullMask |= 1 << 7; 
					EnumProxy<Cmune.DataCenter.Common.Entities.GroupType>.Serialize(s, instance.Type);

					Int32Proxy.Serialize(stream, ~nullMask);
					s.WriteTo(stream);
				}
			}
			else
			{
				Int32Proxy.Serialize(stream, 0);
			}
		}
		
		public static Cmune.DataCenter.Common.Entities.ClanView Deserialize(Stream bytes)
        {
			int nullMask = Int32Proxy.Deserialize(bytes);
			
			Cmune.DataCenter.Common.Entities.ClanView instance = null;
			if(nullMask != 0)
			{
            	instance = new Cmune.DataCenter.Common.Entities.ClanView();
				if((nullMask & (1 << 0)) != 0) instance.Address = StringProxy.Deserialize(bytes);
				instance.ApplicationId = Int32Proxy.Deserialize(bytes);
				instance.ColorStyle = EnumProxy<Cmune.DataCenter.Common.Entities.GroupColor>.Deserialize(bytes);
				if((nullMask & (1 << 1)) != 0) instance.Description = StringProxy.Deserialize(bytes);
				instance.FontStyle = EnumProxy<Cmune.DataCenter.Common.Entities.GroupFontStyle>.Deserialize(bytes);
				instance.FoundingDate = DateTimeProxy.Deserialize(bytes);
				instance.GroupId = Int32Proxy.Deserialize(bytes);
				instance.LastUpdated = DateTimeProxy.Deserialize(bytes);
				if((nullMask & (1 << 2)) != 0) instance.Members = ListProxy<Cmune.DataCenter.Common.Entities.ClanMemberView>.Deserialize(bytes, ClanMemberViewProxy.Deserialize);
				instance.MembersCount = Int32Proxy.Deserialize(bytes);
				instance.MembersLimit = Int32Proxy.Deserialize(bytes);
				if((nullMask & (1 << 3)) != 0) instance.Motto = StringProxy.Deserialize(bytes);
				if((nullMask & (1 << 4)) != 0) instance.Name = StringProxy.Deserialize(bytes);
				instance.OwnerCmid = Int32Proxy.Deserialize(bytes);
				if((nullMask & (1 << 5)) != 0) instance.OwnerName = StringProxy.Deserialize(bytes);
				if((nullMask & (1 << 6)) != 0) instance.Picture = StringProxy.Deserialize(bytes);
				if((nullMask & (1 << 7)) != 0) instance.Tag = StringProxy.Deserialize(bytes);
				instance.Type = EnumProxy<Cmune.DataCenter.Common.Entities.GroupType>.Deserialize(bytes);
			}
			
			return instance;
		}
	}
	public static class ContactRequestViewProxy
	{
		public static void Serialize(Stream stream, Cmune.DataCenter.Common.Entities.ContactRequestView instance)
		{
			int nullMask = 0;
			if(instance != null)
			{
				using (MemoryStream s = new MemoryStream())
                {
					Int32Proxy.Serialize(s, instance.InitiatorCmid);
					if(instance.InitiatorMessage != null) StringProxy.Serialize(s, instance.InitiatorMessage);
					else nullMask |= 1 << 0; 
					if(instance.InitiatorName != null) StringProxy.Serialize(s, instance.InitiatorName);
					else nullMask |= 1 << 1; 
					Int32Proxy.Serialize(s, instance.ReceiverCmid);
					Int32Proxy.Serialize(s, instance.RequestId);
					DateTimeProxy.Serialize(s, instance.SentDate);
					EnumProxy<Cmune.DataCenter.Common.Entities.ContactRequestStatus>.Serialize(s, instance.Status);

					Int32Proxy.Serialize(stream, ~nullMask);
					s.WriteTo(stream);
				}
			}
			else
			{
				Int32Proxy.Serialize(stream, 0);
			}
		}
		
		public static Cmune.DataCenter.Common.Entities.ContactRequestView Deserialize(Stream bytes)
        {
			int nullMask = Int32Proxy.Deserialize(bytes);
			
			Cmune.DataCenter.Common.Entities.ContactRequestView instance = null;
			if(nullMask != 0)
			{
            	instance = new Cmune.DataCenter.Common.Entities.ContactRequestView();
				instance.InitiatorCmid = Int32Proxy.Deserialize(bytes);
				if((nullMask & (1 << 0)) != 0) instance.InitiatorMessage = StringProxy.Deserialize(bytes);
				if((nullMask & (1 << 1)) != 0) instance.InitiatorName = StringProxy.Deserialize(bytes);
				instance.ReceiverCmid = Int32Proxy.Deserialize(bytes);
				instance.RequestId = Int32Proxy.Deserialize(bytes);
				instance.SentDate = DateTimeProxy.Deserialize(bytes);
				instance.Status = EnumProxy<Cmune.DataCenter.Common.Entities.ContactRequestStatus>.Deserialize(bytes);
			}
			
			return instance;
		}
	}
	public static class ClanRequestDeclineViewProxy
	{
		public static void Serialize(Stream stream, Cmune.DataCenter.Common.Entities.ClanRequestDeclineView instance)
		{
			int nullMask = 0;
			if(instance != null)
			{
				using (MemoryStream s = new MemoryStream())
                {
					Int32Proxy.Serialize(s, instance.ActionResult);
					Int32Proxy.Serialize(s, instance.ClanRequestId);

					Int32Proxy.Serialize(stream, ~nullMask);
					s.WriteTo(stream);
				}
			}
			else
			{
				Int32Proxy.Serialize(stream, 0);
			}
		}
		
		public static Cmune.DataCenter.Common.Entities.ClanRequestDeclineView Deserialize(Stream bytes)
        {
			int nullMask = Int32Proxy.Deserialize(bytes);
			
			Cmune.DataCenter.Common.Entities.ClanRequestDeclineView instance = null;
			if(nullMask != 0)
			{
            	instance = new Cmune.DataCenter.Common.Entities.ClanRequestDeclineView();
				instance.ActionResult = Int32Proxy.Deserialize(bytes);
				instance.ClanRequestId = Int32Proxy.Deserialize(bytes);
			}
			
			return instance;
		}
	}
	public static class ContactRequestAcceptViewProxy
	{
		public static void Serialize(Stream stream, Cmune.DataCenter.Common.Entities.ContactRequestAcceptView instance)
		{
			int nullMask = 0;
			if(instance != null)
			{
				using (MemoryStream s = new MemoryStream())
                {
					Int32Proxy.Serialize(s, instance.ActionResult);
					if(instance.Contact != null) PublicProfileViewProxy.Serialize(s, instance.Contact);
					else nullMask |= 1 << 0; 
					Int32Proxy.Serialize(s, instance.RequestId);

					Int32Proxy.Serialize(stream, ~nullMask);
					s.WriteTo(stream);
				}
			}
			else
			{
				Int32Proxy.Serialize(stream, 0);
			}
		}
		
		public static Cmune.DataCenter.Common.Entities.ContactRequestAcceptView Deserialize(Stream bytes)
        {
			int nullMask = Int32Proxy.Deserialize(bytes);
			
			Cmune.DataCenter.Common.Entities.ContactRequestAcceptView instance = null;
			if(nullMask != 0)
			{
            	instance = new Cmune.DataCenter.Common.Entities.ContactRequestAcceptView();
				instance.ActionResult = Int32Proxy.Deserialize(bytes);
				if((nullMask & (1 << 0)) != 0) instance.Contact = PublicProfileViewProxy.Deserialize(bytes);
				instance.RequestId = Int32Proxy.Deserialize(bytes);
			}
			
			return instance;
		}
	}
	public static class PointDepositViewProxy
	{
		public static void Serialize(Stream stream, Cmune.DataCenter.Common.Entities.PointDepositView instance)
		{
			int nullMask = 0;
			if(instance != null)
			{
				using (MemoryStream s = new MemoryStream())
                {
					Int32Proxy.Serialize(s, instance.Cmid);
					DateTimeProxy.Serialize(s, instance.DepositDate);
					EnumProxy<Cmune.DataCenter.Common.Entities.PointsDepositType>.Serialize(s, instance.DepositType);
					BooleanProxy.Serialize(s, instance.IsAdminAction);
					Int32Proxy.Serialize(s, instance.PointDepositId);
					Int32Proxy.Serialize(s, instance.Points);

					Int32Proxy.Serialize(stream, ~nullMask);
					s.WriteTo(stream);
				}
			}
			else
			{
				Int32Proxy.Serialize(stream, 0);
			}
		}
		
		public static Cmune.DataCenter.Common.Entities.PointDepositView Deserialize(Stream bytes)
        {
			int nullMask = Int32Proxy.Deserialize(bytes);
			
			Cmune.DataCenter.Common.Entities.PointDepositView instance = null;
			if(nullMask != 0)
			{
            	instance = new Cmune.DataCenter.Common.Entities.PointDepositView();
				instance.Cmid = Int32Proxy.Deserialize(bytes);
				instance.DepositDate = DateTimeProxy.Deserialize(bytes);
				instance.DepositType = EnumProxy<Cmune.DataCenter.Common.Entities.PointsDepositType>.Deserialize(bytes);
				instance.IsAdminAction = BooleanProxy.Deserialize(bytes);
				instance.PointDepositId = Int32Proxy.Deserialize(bytes);
				instance.Points = Int32Proxy.Deserialize(bytes);
			}
			
			return instance;
		}
	}
	public static class MemberWalletViewProxy
	{
		public static void Serialize(Stream stream, Cmune.DataCenter.Common.Entities.MemberWalletView instance)
		{
			int nullMask = 0;
			if(instance != null)
			{
				using (MemoryStream s = new MemoryStream())
                {
					Int32Proxy.Serialize(s, instance.Cmid);
					Int32Proxy.Serialize(s, instance.Credits);
					DateTimeProxy.Serialize(s, instance.CreditsExpiration);
					Int32Proxy.Serialize(s, instance.Points);
					DateTimeProxy.Serialize(s, instance.PointsExpiration);

					Int32Proxy.Serialize(stream, ~nullMask);
					s.WriteTo(stream);
				}
			}
			else
			{
				Int32Proxy.Serialize(stream, 0);
			}
		}
		
		public static Cmune.DataCenter.Common.Entities.MemberWalletView Deserialize(Stream bytes)
        {
			int nullMask = Int32Proxy.Deserialize(bytes);
			
			Cmune.DataCenter.Common.Entities.MemberWalletView instance = null;
			if(nullMask != 0)
			{
            	instance = new Cmune.DataCenter.Common.Entities.MemberWalletView();
				instance.Cmid = Int32Proxy.Deserialize(bytes);
				instance.Credits = Int32Proxy.Deserialize(bytes);
				instance.CreditsExpiration = DateTimeProxy.Deserialize(bytes);
				instance.Points = Int32Proxy.Deserialize(bytes);
				instance.PointsExpiration = DateTimeProxy.Deserialize(bytes);
			}
			
			return instance;
		}
	}
	public static class MessageThreadViewProxy
	{
		public static void Serialize(Stream stream, Cmune.DataCenter.Common.Entities.MessageThreadView instance)
		{
			int nullMask = 0;
			if(instance != null)
			{
				using (MemoryStream s = new MemoryStream())
                {
					BooleanProxy.Serialize(s, instance.HasNewMessages);
					if(instance.LastMessagePreview != null) StringProxy.Serialize(s, instance.LastMessagePreview);
					else nullMask |= 1 << 0; 
					DateTimeProxy.Serialize(s, instance.LastUpdate);
					Int32Proxy.Serialize(s, instance.MessageCount);
					Int32Proxy.Serialize(s, instance.ThreadId);
					if(instance.ThreadName != null) StringProxy.Serialize(s, instance.ThreadName);
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
		
		public static Cmune.DataCenter.Common.Entities.MessageThreadView Deserialize(Stream bytes)
        {
			int nullMask = Int32Proxy.Deserialize(bytes);
			
			Cmune.DataCenter.Common.Entities.MessageThreadView instance = null;
			if(nullMask != 0)
			{
            	instance = new Cmune.DataCenter.Common.Entities.MessageThreadView();
				instance.HasNewMessages = BooleanProxy.Deserialize(bytes);
				if((nullMask & (1 << 0)) != 0) instance.LastMessagePreview = StringProxy.Deserialize(bytes);
				instance.LastUpdate = DateTimeProxy.Deserialize(bytes);
				instance.MessageCount = Int32Proxy.Deserialize(bytes);
				instance.ThreadId = Int32Proxy.Deserialize(bytes);
				if((nullMask & (1 << 1)) != 0) instance.ThreadName = StringProxy.Deserialize(bytes);
			}
			
			return instance;
		}
	}
	public static class BundleItemViewProxy
	{
		public static void Serialize(Stream stream, Cmune.DataCenter.Common.Entities.BundleItemView instance)
		{
			int nullMask = 0;
			if(instance != null)
			{
				using (MemoryStream s = new MemoryStream())
                {
					Int32Proxy.Serialize(s, instance.Amount);
					Int32Proxy.Serialize(s, instance.BundleId);
					EnumProxy<Cmune.DataCenter.Common.Entities.BuyingDurationType>.Serialize(s, instance.Duration);
					Int32Proxy.Serialize(s, instance.ItemId);

					Int32Proxy.Serialize(stream, ~nullMask);
					s.WriteTo(stream);
				}
			}
			else
			{
				Int32Proxy.Serialize(stream, 0);
			}
		}
		
		public static Cmune.DataCenter.Common.Entities.BundleItemView Deserialize(Stream bytes)
        {
			int nullMask = Int32Proxy.Deserialize(bytes);
			
			Cmune.DataCenter.Common.Entities.BundleItemView instance = null;
			if(nullMask != 0)
			{
            	instance = new Cmune.DataCenter.Common.Entities.BundleItemView();
				instance.Amount = Int32Proxy.Deserialize(bytes);
				instance.BundleId = Int32Proxy.Deserialize(bytes);
				instance.Duration = EnumProxy<Cmune.DataCenter.Common.Entities.BuyingDurationType>.Deserialize(bytes);
				instance.ItemId = Int32Proxy.Deserialize(bytes);
			}
			
			return instance;
		}
	}
	public static class ClanCreationReturnViewProxy
	{
		public static void Serialize(Stream stream, Cmune.DataCenter.Common.Entities.ClanCreationReturnView instance)
		{
			int nullMask = 0;
			if(instance != null)
			{
				using (MemoryStream s = new MemoryStream())
                {
					if(instance.ClanView != null) ClanViewProxy.Serialize(s, instance.ClanView);
					else nullMask |= 1 << 0; 
					Int32Proxy.Serialize(s, instance.ResultCode);

					Int32Proxy.Serialize(stream, ~nullMask);
					s.WriteTo(stream);
				}
			}
			else
			{
				Int32Proxy.Serialize(stream, 0);
			}
		}
		
		public static Cmune.DataCenter.Common.Entities.ClanCreationReturnView Deserialize(Stream bytes)
        {
			int nullMask = Int32Proxy.Deserialize(bytes);
			
			Cmune.DataCenter.Common.Entities.ClanCreationReturnView instance = null;
			if(nullMask != 0)
			{
            	instance = new Cmune.DataCenter.Common.Entities.ClanCreationReturnView();
				if((nullMask & (1 << 0)) != 0) instance.ClanView = ClanViewProxy.Deserialize(bytes);
				instance.ResultCode = Int32Proxy.Deserialize(bytes);
			}
			
			return instance;
		}
	}
	public static class MemberPositionUpdateViewProxy
	{
		public static void Serialize(Stream stream, Cmune.DataCenter.Common.Entities.MemberPositionUpdateView instance)
		{
			int nullMask = 0;
			if(instance != null)
			{
				using (MemoryStream s = new MemoryStream())
                {
					Int32Proxy.Serialize(s, instance.CmidMakingAction);
					Int32Proxy.Serialize(s, instance.GroupId);
					Int32Proxy.Serialize(s, instance.MemberCmid);
					EnumProxy<Cmune.DataCenter.Common.Entities.GroupPosition>.Serialize(s, instance.Position);

					Int32Proxy.Serialize(stream, ~nullMask);
					s.WriteTo(stream);
				}
			}
			else
			{
				Int32Proxy.Serialize(stream, 0);
			}
		}
		
		public static Cmune.DataCenter.Common.Entities.MemberPositionUpdateView Deserialize(Stream bytes)
        {
			int nullMask = Int32Proxy.Deserialize(bytes);
			
			Cmune.DataCenter.Common.Entities.MemberPositionUpdateView instance = null;
			if(nullMask != 0)
			{
            	instance = new Cmune.DataCenter.Common.Entities.MemberPositionUpdateView();
				instance.CmidMakingAction = Int32Proxy.Deserialize(bytes);
				instance.GroupId = Int32Proxy.Deserialize(bytes);
				instance.MemberCmid = Int32Proxy.Deserialize(bytes);
				instance.Position = EnumProxy<Cmune.DataCenter.Common.Entities.GroupPosition>.Deserialize(bytes);
			}
			
			return instance;
		}
	}
	public static class LuckyDrawUnityViewProxy
	{
		public static void Serialize(Stream stream, Cmune.DataCenter.Common.Entities.LuckyDrawUnityView instance)
		{
			int nullMask = 0;
			if(instance != null)
			{
				using (MemoryStream s = new MemoryStream())
                {
					EnumProxy<Cmune.DataCenter.Common.Entities.BundleCategoryType>.Serialize(s, instance.Category);
					if(instance.Description != null) StringProxy.Serialize(s, instance.Description);
					else nullMask |= 1 << 0; 
					if(instance.IconUrl != null) StringProxy.Serialize(s, instance.IconUrl);
					else nullMask |= 1 << 1; 
					Int32Proxy.Serialize(s, instance.Id);
					BooleanProxy.Serialize(s, instance.IsAvailableInShop);
					if(instance.LuckyDrawSets != null) ListProxy<Cmune.DataCenter.Common.Entities.LuckyDrawSetUnityView>.Serialize(s, instance.LuckyDrawSets, LuckyDrawSetUnityViewProxy.Serialize);
					else nullMask |= 1 << 2; 
					if(instance.Name != null) StringProxy.Serialize(s, instance.Name);
					else nullMask |= 1 << 3; 
					Int32Proxy.Serialize(s, instance.Price);
					EnumProxy<Cmune.DataCenter.Common.Entities.UberStrikeCurrencyType>.Serialize(s, instance.UberStrikeCurrencyType);

					Int32Proxy.Serialize(stream, ~nullMask);
					s.WriteTo(stream);
				}
			}
			else
			{
				Int32Proxy.Serialize(stream, 0);
			}
		}
		
		public static Cmune.DataCenter.Common.Entities.LuckyDrawUnityView Deserialize(Stream bytes)
        {
			int nullMask = Int32Proxy.Deserialize(bytes);
			
			Cmune.DataCenter.Common.Entities.LuckyDrawUnityView instance = null;
			if(nullMask != 0)
			{
            	instance = new Cmune.DataCenter.Common.Entities.LuckyDrawUnityView();
				instance.Category = EnumProxy<Cmune.DataCenter.Common.Entities.BundleCategoryType>.Deserialize(bytes);
				if((nullMask & (1 << 0)) != 0) instance.Description = StringProxy.Deserialize(bytes);
				if((nullMask & (1 << 1)) != 0) instance.IconUrl = StringProxy.Deserialize(bytes);
				instance.Id = Int32Proxy.Deserialize(bytes);
				instance.IsAvailableInShop = BooleanProxy.Deserialize(bytes);
				if((nullMask & (1 << 2)) != 0) instance.LuckyDrawSets = ListProxy<Cmune.DataCenter.Common.Entities.LuckyDrawSetUnityView>.Deserialize(bytes, LuckyDrawSetUnityViewProxy.Deserialize);
				if((nullMask & (1 << 3)) != 0) instance.Name = StringProxy.Deserialize(bytes);
				instance.Price = Int32Proxy.Deserialize(bytes);
				instance.UberStrikeCurrencyType = EnumProxy<Cmune.DataCenter.Common.Entities.UberStrikeCurrencyType>.Deserialize(bytes);
			}
			
			return instance;
		}
	}
	public static class ContactRequestDeclineViewProxy
	{
		public static void Serialize(Stream stream, Cmune.DataCenter.Common.Entities.ContactRequestDeclineView instance)
		{
			int nullMask = 0;
			if(instance != null)
			{
				using (MemoryStream s = new MemoryStream())
                {
					Int32Proxy.Serialize(s, instance.ActionResult);
					Int32Proxy.Serialize(s, instance.RequestId);

					Int32Proxy.Serialize(stream, ~nullMask);
					s.WriteTo(stream);
				}
			}
			else
			{
				Int32Proxy.Serialize(stream, 0);
			}
		}
		
		public static Cmune.DataCenter.Common.Entities.ContactRequestDeclineView Deserialize(Stream bytes)
        {
			int nullMask = Int32Proxy.Deserialize(bytes);
			
			Cmune.DataCenter.Common.Entities.ContactRequestDeclineView instance = null;
			if(nullMask != 0)
			{
            	instance = new Cmune.DataCenter.Common.Entities.ContactRequestDeclineView();
				instance.ActionResult = Int32Proxy.Deserialize(bytes);
				instance.RequestId = Int32Proxy.Deserialize(bytes);
			}
			
			return instance;
		}
	}
	public static class ClanMemberViewProxy
	{
		public static void Serialize(Stream stream, Cmune.DataCenter.Common.Entities.ClanMemberView instance)
		{
			int nullMask = 0;
			if(instance != null)
			{
				using (MemoryStream s = new MemoryStream())
                {
					Int32Proxy.Serialize(s, instance.Cmid);
					DateTimeProxy.Serialize(s, instance.JoiningDate);
					DateTimeProxy.Serialize(s, instance.Lastlogin);
					if(instance.Name != null) StringProxy.Serialize(s, instance.Name);
					else nullMask |= 1 << 0; 
					EnumProxy<Cmune.DataCenter.Common.Entities.GroupPosition>.Serialize(s, instance.Position);

					Int32Proxy.Serialize(stream, ~nullMask);
					s.WriteTo(stream);
				}
			}
			else
			{
				Int32Proxy.Serialize(stream, 0);
			}
		}
		
		public static Cmune.DataCenter.Common.Entities.ClanMemberView Deserialize(Stream bytes)
        {
			int nullMask = Int32Proxy.Deserialize(bytes);
			
			Cmune.DataCenter.Common.Entities.ClanMemberView instance = null;
			if(nullMask != 0)
			{
            	instance = new Cmune.DataCenter.Common.Entities.ClanMemberView();
				instance.Cmid = Int32Proxy.Deserialize(bytes);
				instance.JoiningDate = DateTimeProxy.Deserialize(bytes);
				instance.Lastlogin = DateTimeProxy.Deserialize(bytes);
				if((nullMask & (1 << 0)) != 0) instance.Name = StringProxy.Deserialize(bytes);
				instance.Position = EnumProxy<Cmune.DataCenter.Common.Entities.GroupPosition>.Deserialize(bytes);
			}
			
			return instance;
		}
	}
	public static class ApplicationViewProxy
	{
		public static void Serialize(Stream stream, Cmune.DataCenter.Common.Entities.ApplicationView instance)
		{
			int nullMask = 0;
			if(instance != null)
			{
				using (MemoryStream s = new MemoryStream())
                {
					Int32Proxy.Serialize(s, instance.ApplicationVersionId);
					EnumProxy<Cmune.DataCenter.Common.Entities.BuildType>.Serialize(s, instance.Build);
					EnumProxy<Cmune.DataCenter.Common.Entities.ChannelType>.Serialize(s, instance.Channel);
					if(instance.ExpirationDate != null) DateTimeProxy.Serialize(s, instance.ExpirationDate ?? default(System.DateTime));
					else nullMask |= 1 << 0; 
					if(instance.FileName != null) StringProxy.Serialize(s, instance.FileName);
					else nullMask |= 1 << 1; 
					BooleanProxy.Serialize(s, instance.IsCurrent);
					Int32Proxy.Serialize(s, instance.PhotonGroupId);
					if(instance.PhotonGroupName != null) StringProxy.Serialize(s, instance.PhotonGroupName);
					else nullMask |= 1 << 2; 
					DateTimeProxy.Serialize(s, instance.ReleaseDate);
					Int32Proxy.Serialize(s, instance.RemainingTime);
					if(instance.Servers != null) ListProxy<Cmune.Core.Models.Views.PhotonView>.Serialize(s, instance.Servers, PhotonViewProxy.Serialize);
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
		
		public static Cmune.DataCenter.Common.Entities.ApplicationView Deserialize(Stream bytes)
        {
			int nullMask = Int32Proxy.Deserialize(bytes);
			
			Cmune.DataCenter.Common.Entities.ApplicationView instance = null;
			if(nullMask != 0)
			{
            	instance = new Cmune.DataCenter.Common.Entities.ApplicationView();
				instance.ApplicationVersionId = Int32Proxy.Deserialize(bytes);
				instance.Build = EnumProxy<Cmune.DataCenter.Common.Entities.BuildType>.Deserialize(bytes);
				instance.Channel = EnumProxy<Cmune.DataCenter.Common.Entities.ChannelType>.Deserialize(bytes);
				if((nullMask & (1 << 0)) != 0) instance.ExpirationDate = DateTimeProxy.Deserialize(bytes);
				if((nullMask & (1 << 1)) != 0) instance.FileName = StringProxy.Deserialize(bytes);
				instance.IsCurrent = BooleanProxy.Deserialize(bytes);
				instance.PhotonGroupId = Int32Proxy.Deserialize(bytes);
				if((nullMask & (1 << 2)) != 0) instance.PhotonGroupName = StringProxy.Deserialize(bytes);
				instance.ReleaseDate = DateTimeProxy.Deserialize(bytes);
				instance.RemainingTime = Int32Proxy.Deserialize(bytes);
				if((nullMask & (1 << 3)) != 0) instance.Servers = ListProxy<Cmune.Core.Models.Views.PhotonView>.Deserialize(bytes, PhotonViewProxy.Deserialize);
				if((nullMask & (1 << 4)) != 0) instance.SupportUrl = StringProxy.Deserialize(bytes);
				if((nullMask & (1 << 5)) != 0) instance.Version = StringProxy.Deserialize(bytes);
			}
			
			return instance;
		}
	}
	public static class MysteryBoxUnityViewProxy
	{
		public static void Serialize(Stream stream, Cmune.DataCenter.Common.Entities.MysteryBoxUnityView instance)
		{
			int nullMask = 0;
			if(instance != null)
			{
				using (MemoryStream s = new MemoryStream())
                {
					EnumProxy<Cmune.DataCenter.Common.Entities.BundleCategoryType>.Serialize(s, instance.Category);
					Int32Proxy.Serialize(s, instance.CreditsAttributed);
					Int32Proxy.Serialize(s, instance.CreditsAttributedWeight);
					if(instance.Description != null) StringProxy.Serialize(s, instance.Description);
					else nullMask |= 1 << 0; 
					BooleanProxy.Serialize(s, instance.ExposeItemsToPlayers);
					if(instance.IconUrl != null) StringProxy.Serialize(s, instance.IconUrl);
					else nullMask |= 1 << 1; 
					Int32Proxy.Serialize(s, instance.Id);
					if(instance.ImageUrl != null) StringProxy.Serialize(s, instance.ImageUrl);
					else nullMask |= 1 << 2; 
					BooleanProxy.Serialize(s, instance.IsAvailableInShop);
					Int32Proxy.Serialize(s, instance.ItemsAttributed);
					if(instance.MysteryBoxItems != null) ListProxy<Cmune.DataCenter.Common.Entities.BundleItemView>.Serialize(s, instance.MysteryBoxItems, BundleItemViewProxy.Serialize);
					else nullMask |= 1 << 3; 
					if(instance.Name != null) StringProxy.Serialize(s, instance.Name);
					else nullMask |= 1 << 4; 
					Int32Proxy.Serialize(s, instance.PointsAttributed);
					Int32Proxy.Serialize(s, instance.PointsAttributedWeight);
					Int32Proxy.Serialize(s, instance.Price);
					EnumProxy<Cmune.DataCenter.Common.Entities.UberStrikeCurrencyType>.Serialize(s, instance.UberStrikeCurrencyType);

					Int32Proxy.Serialize(stream, ~nullMask);
					s.WriteTo(stream);
				}
			}
			else
			{
				Int32Proxy.Serialize(stream, 0);
			}
		}
		
		public static Cmune.DataCenter.Common.Entities.MysteryBoxUnityView Deserialize(Stream bytes)
        {
			int nullMask = Int32Proxy.Deserialize(bytes);
			
			Cmune.DataCenter.Common.Entities.MysteryBoxUnityView instance = null;
			if(nullMask != 0)
			{
            	instance = new Cmune.DataCenter.Common.Entities.MysteryBoxUnityView();
				instance.Category = EnumProxy<Cmune.DataCenter.Common.Entities.BundleCategoryType>.Deserialize(bytes);
				instance.CreditsAttributed = Int32Proxy.Deserialize(bytes);
				instance.CreditsAttributedWeight = Int32Proxy.Deserialize(bytes);
				if((nullMask & (1 << 0)) != 0) instance.Description = StringProxy.Deserialize(bytes);
				instance.ExposeItemsToPlayers = BooleanProxy.Deserialize(bytes);
				if((nullMask & (1 << 1)) != 0) instance.IconUrl = StringProxy.Deserialize(bytes);
				instance.Id = Int32Proxy.Deserialize(bytes);
				if((nullMask & (1 << 2)) != 0) instance.ImageUrl = StringProxy.Deserialize(bytes);
				instance.IsAvailableInShop = BooleanProxy.Deserialize(bytes);
				instance.ItemsAttributed = Int32Proxy.Deserialize(bytes);
				if((nullMask & (1 << 3)) != 0) instance.MysteryBoxItems = ListProxy<Cmune.DataCenter.Common.Entities.BundleItemView>.Deserialize(bytes, BundleItemViewProxy.Deserialize);
				if((nullMask & (1 << 4)) != 0) instance.Name = StringProxy.Deserialize(bytes);
				instance.PointsAttributed = Int32Proxy.Deserialize(bytes);
				instance.PointsAttributedWeight = Int32Proxy.Deserialize(bytes);
				instance.Price = Int32Proxy.Deserialize(bytes);
				instance.UberStrikeCurrencyType = EnumProxy<Cmune.DataCenter.Common.Entities.UberStrikeCurrencyType>.Deserialize(bytes);
			}
			
			return instance;
		}
	}
	public static class MemberReportViewProxy
	{
		public static void Serialize(Stream stream, Cmune.DataCenter.Common.Entities.MemberReportView instance)
		{
			int nullMask = 0;
			if(instance != null)
			{
				using (MemoryStream s = new MemoryStream())
                {
					Int32Proxy.Serialize(s, instance.ApplicationId);
					if(instance.Context != null) StringProxy.Serialize(s, instance.Context);
					else nullMask |= 1 << 0; 
					if(instance.IP != null) StringProxy.Serialize(s, instance.IP);
					else nullMask |= 1 << 1; 
					if(instance.Reason != null) StringProxy.Serialize(s, instance.Reason);
					else nullMask |= 1 << 2; 
					EnumProxy<Cmune.DataCenter.Common.Entities.MemberReportType>.Serialize(s, instance.ReportType);
					Int32Proxy.Serialize(s, instance.SourceCmid);
					Int32Proxy.Serialize(s, instance.TargetCmid);

					Int32Proxy.Serialize(stream, ~nullMask);
					s.WriteTo(stream);
				}
			}
			else
			{
				Int32Proxy.Serialize(stream, 0);
			}
		}
		
		public static Cmune.DataCenter.Common.Entities.MemberReportView Deserialize(Stream bytes)
        {
			int nullMask = Int32Proxy.Deserialize(bytes);
			
			Cmune.DataCenter.Common.Entities.MemberReportView instance = null;
			if(nullMask != 0)
			{
            	instance = new Cmune.DataCenter.Common.Entities.MemberReportView();
				instance.ApplicationId = Int32Proxy.Deserialize(bytes);
				if((nullMask & (1 << 0)) != 0) instance.Context = StringProxy.Deserialize(bytes);
				if((nullMask & (1 << 1)) != 0) instance.IP = StringProxy.Deserialize(bytes);
				if((nullMask & (1 << 2)) != 0) instance.Reason = StringProxy.Deserialize(bytes);
				instance.ReportType = EnumProxy<Cmune.DataCenter.Common.Entities.MemberReportType>.Deserialize(bytes);
				instance.SourceCmid = Int32Proxy.Deserialize(bytes);
				instance.TargetCmid = Int32Proxy.Deserialize(bytes);
			}
			
			return instance;
		}
	}
	public static class ItemInventoryViewProxy
	{
		public static void Serialize(Stream stream, Cmune.DataCenter.Common.Entities.ItemInventoryView instance)
		{
			int nullMask = 0;
			if(instance != null)
			{
				using (MemoryStream s = new MemoryStream())
                {
					Int32Proxy.Serialize(s, instance.AmountRemaining);
					Int32Proxy.Serialize(s, instance.Cmid);
					if(instance.ExpirationDate != null) DateTimeProxy.Serialize(s, instance.ExpirationDate ?? default(System.DateTime));
					else nullMask |= 1 << 0; 
					Int32Proxy.Serialize(s, instance.ItemId);

					Int32Proxy.Serialize(stream, ~nullMask);
					s.WriteTo(stream);
				}
			}
			else
			{
				Int32Proxy.Serialize(stream, 0);
			}
		}
		
		public static Cmune.DataCenter.Common.Entities.ItemInventoryView Deserialize(Stream bytes)
        {
			int nullMask = Int32Proxy.Deserialize(bytes);
			
			Cmune.DataCenter.Common.Entities.ItemInventoryView instance = null;
			if(nullMask != 0)
			{
            	instance = new Cmune.DataCenter.Common.Entities.ItemInventoryView();
				instance.AmountRemaining = Int32Proxy.Deserialize(bytes);
				instance.Cmid = Int32Proxy.Deserialize(bytes);
				if((nullMask & (1 << 0)) != 0) instance.ExpirationDate = DateTimeProxy.Deserialize(bytes);
				instance.ItemId = Int32Proxy.Deserialize(bytes);
			}
			
			return instance;
		}
	}
	public static class ContactGroupViewProxy
	{
		public static void Serialize(Stream stream, Cmune.DataCenter.Common.Entities.ContactGroupView instance)
		{
			int nullMask = 0;
			if(instance != null)
			{
				using (MemoryStream s = new MemoryStream())
                {
					if(instance.Contacts != null) ListProxy<Cmune.DataCenter.Common.Entities.PublicProfileView>.Serialize(s, instance.Contacts, PublicProfileViewProxy.Serialize);
					else nullMask |= 1 << 0; 
					Int32Proxy.Serialize(s, instance.GroupId);
					if(instance.GroupName != null) StringProxy.Serialize(s, instance.GroupName);
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
		
		public static Cmune.DataCenter.Common.Entities.ContactGroupView Deserialize(Stream bytes)
        {
			int nullMask = Int32Proxy.Deserialize(bytes);
			
			Cmune.DataCenter.Common.Entities.ContactGroupView instance = null;
			if(nullMask != 0)
			{
            	instance = new Cmune.DataCenter.Common.Entities.ContactGroupView();
				if((nullMask & (1 << 0)) != 0) instance.Contacts = ListProxy<Cmune.DataCenter.Common.Entities.PublicProfileView>.Deserialize(bytes, PublicProfileViewProxy.Deserialize);
				instance.GroupId = Int32Proxy.Deserialize(bytes);
				if((nullMask & (1 << 1)) != 0) instance.GroupName = StringProxy.Deserialize(bytes);
			}
			
			return instance;
		}
	}
	public static class CheckApplicationVersionViewProxy
	{
		public static void Serialize(Stream stream, Cmune.DataCenter.Common.Entities.CheckApplicationVersionView instance)
		{
			int nullMask = 0;
			if(instance != null)
			{
				using (MemoryStream s = new MemoryStream())
                {
					if(instance.ClientVersion != null) ApplicationViewProxy.Serialize(s, instance.ClientVersion);
					else nullMask |= 1 << 0; 
					if(instance.CurrentVersion != null) ApplicationViewProxy.Serialize(s, instance.CurrentVersion);
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
		
		public static Cmune.DataCenter.Common.Entities.CheckApplicationVersionView Deserialize(Stream bytes)
        {
			int nullMask = Int32Proxy.Deserialize(bytes);
			
			Cmune.DataCenter.Common.Entities.CheckApplicationVersionView instance = null;
			if(nullMask != 0)
			{
            	instance = new Cmune.DataCenter.Common.Entities.CheckApplicationVersionView();
				if((nullMask & (1 << 0)) != 0) instance.ClientVersion = ApplicationViewProxy.Deserialize(bytes);
				if((nullMask & (1 << 1)) != 0) instance.CurrentVersion = ApplicationViewProxy.Deserialize(bytes);
			}
			
			return instance;
		}
	}
	public static class BugViewProxy
	{
		public static void Serialize(Stream stream, Cmune.DataCenter.Common.Entities.BugView instance)
		{
			int nullMask = 0;
			if(instance != null)
			{
				using (MemoryStream s = new MemoryStream())
                {
					if(instance.Content != null) StringProxy.Serialize(s, instance.Content);
					else nullMask |= 1 << 0; 
					if(instance.Subject != null) StringProxy.Serialize(s, instance.Subject);
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
		
		public static Cmune.DataCenter.Common.Entities.BugView Deserialize(Stream bytes)
        {
			int nullMask = Int32Proxy.Deserialize(bytes);
			
			Cmune.DataCenter.Common.Entities.BugView instance = null;
			if(nullMask != 0)
			{
            	instance = new Cmune.DataCenter.Common.Entities.BugView();
				if((nullMask & (1 << 0)) != 0) instance.Content = StringProxy.Deserialize(bytes);
				if((nullMask & (1 << 1)) != 0) instance.Subject = StringProxy.Deserialize(bytes);
			}
			
			return instance;
		}
	}
}
