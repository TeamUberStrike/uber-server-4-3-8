public static class WebServiceDescriptor
{
	public static string Xml =@"<services>
  <service name=""ApplicationWebService"" endpoint=""UberStrike.DataCenter.WebService.CWS.ApplicationWebServiceContract.svc"" contract=""IApplicationWebServiceContract"">
    <operations>
      <operation name=""RegisterClientApplication"" returnType=""UberStrike.Core.ViewModel.RegisterClientApplicationViewModel"">
        <parameters>
          <parameter name=""cmuneId"" type=""int"" />
          <parameter name=""hashCode"" type=""string"" />
          <parameter name=""channel"" type=""Cmune.DataCenter.Common.Entities.ChannelType"" />
          <parameter name=""applicationId"" type=""int"" />
        </parameters>
      </operation>
      <operation name=""GetPhotonServers"" returnType=""System.Collections.Generic.List&lt;Cmune.Core.Models.Views.PhotonView&gt;"">
        <parameters>
          <parameter name=""applicationView"" type=""Cmune.DataCenter.Common.Entities.ApplicationView"" />
        </parameters>
      </operation>
      <operation name=""GetMyIP"" returnType=""string"">
        <parameters />
      </operation>
      <operation name=""AuthenticateApplication"" returnType=""UberStrike.DataCenter.Common.Entities.AuthenticateApplicationView"">
        <parameters>
          <parameter name=""version"" type=""string"" />
          <parameter name=""channel"" type=""Cmune.DataCenter.Common.Entities.ChannelType"" />
          <parameter name=""publicKey"" type=""string"" />
        </parameters>
      </operation>
      <operation name=""RecordException"" returnType=""void"">
        <parameters>
          <parameter name=""cmid"" type=""int"" />
          <parameter name=""buildType"" type=""Cmune.DataCenter.Common.Entities.BuildType"" />
          <parameter name=""channelType"" type=""Cmune.DataCenter.Common.Entities.ChannelType"" />
          <parameter name=""buildNumber"" type=""string"" />
          <parameter name=""logString"" type=""string"" />
          <parameter name=""stackTrace"" type=""string"" />
          <parameter name=""exceptionData"" type=""string"" />
        </parameters>
      </operation>
      <operation name=""RecordExceptionUnencrypted"" returnType=""void"">
        <parameters>
          <parameter name=""buildType"" type=""Cmune.DataCenter.Common.Entities.BuildType"" />
          <parameter name=""channelType"" type=""Cmune.DataCenter.Common.Entities.ChannelType"" />
          <parameter name=""buildNumber"" type=""string"" />
          <parameter name=""errorType"" type=""string"" />
          <parameter name=""errorMessage"" type=""string"" />
        </parameters>
      </operation>
      <operation name=""RecordTutorialStep"" returnType=""void"">
        <parameters>
          <parameter name=""cmid"" type=""int"" />
          <parameter name=""step"" type=""UberStrike.Core.Types.TutorialStepType"" />
        </parameters>
      </operation>
      <operation name=""ReportBug"" returnType=""bool"">
        <parameters>
          <parameter name=""bugView"" type=""Cmune.DataCenter.Common.Entities.BugView"" />
        </parameters>
      </operation>
      <operation name=""GetLiveFeed"" returnType=""System.Collections.Generic.List&lt;UberStrike.DataCenter.Common.Entities.LiveFeedView&gt;"">
        <parameters />
      </operation>
      <operation name=""GetMaps"" returnType=""System.Collections.Generic.List&lt;UberStrike.Core.Models.Views.MapView&gt;"">
        <parameters>
          <parameter name=""appVersion"" type=""string"" />
          <parameter name=""locale"" type=""UberStrike.Core.Types.LocaleType"" />
          <parameter name=""mapType"" type=""UberStrike.Core.Types.MapType"" />
        </parameters>
      </operation>
      <operation name=""SetLevelVersion"" returnType=""void"">
        <parameters>
          <parameter name=""id"" type=""int"" />
          <parameter name=""version"" type=""int"" />
          <parameter name=""md5Hash"" type=""string"" />
        </parameters>
      </operation>
      <operation name=""GetPhotonServerName"" returnType=""string"">
        <parameters>
          <parameter name=""applicationVersion"" type=""string"" />
          <parameter name=""ipAddress"" type=""string"" />
          <parameter name=""port"" type=""int"" />
        </parameters>
      </operation>
      <operation name=""IsAlive"" returnType=""string"">
        <parameters />
      </operation>
    </operations>
  </service>
  <service name=""AuthenticationWebService"" endpoint=""UberStrike.DataCenter.WebService.CWS.AuthenticationWebServiceContract.svc"" contract=""IAuthenticationWebServiceContract"">
    <operations>
      <operation name=""CreateUser"" returnType=""Cmune.DataCenter.Common.Entities.MemberRegistrationResult"">
        <parameters>
          <parameter name=""emailAddress"" type=""string"" />
          <parameter name=""password"" type=""string"" />
          <parameter name=""channel"" type=""Cmune.DataCenter.Common.Entities.ChannelType"" />
          <parameter name=""locale"" type=""string"" />
          <parameter name=""machineId"" type=""string"" />
        </parameters>
      </operation>
      <operation name=""CompleteAccount"" returnType=""UberStrike.DataCenter.Common.Entities.AccountCompletionResultView"">
        <parameters>
          <parameter name=""cmid"" type=""int"" />
          <parameter name=""name"" type=""string"" />
          <parameter name=""channel"" type=""Cmune.DataCenter.Common.Entities.ChannelType"" />
          <parameter name=""locale"" type=""string"" />
          <parameter name=""machineId"" type=""string"" />
        </parameters>
      </operation>
      <operation name=""UncompleteAccount"" returnType=""bool"">
        <parameters>
          <parameter name=""cmid"" type=""int"" />
        </parameters>
      </operation>
      <operation name=""LoginMemberEmail"" returnType=""UberStrike.Core.ViewModel.MemberAuthenticationResultView"">
        <parameters>
          <parameter name=""email"" type=""string"" />
          <parameter name=""password"" type=""string"" />
          <parameter name=""channelType"" type=""Cmune.DataCenter.Common.Entities.ChannelType"" />
          <parameter name=""machineId"" type=""string"" />
        </parameters>
      </operation>
      <operation name=""LoginMemberCookie"" returnType=""UberStrike.Core.ViewModel.MemberAuthenticationResultView"">
        <parameters>
          <parameter name=""cmid"" type=""int"" />
          <parameter name=""expirationTime"" type=""System.DateTime"" />
          <parameter name=""encryptedContent"" type=""string"" />
          <parameter name=""hash"" type=""string"" />
          <parameter name=""channelType"" type=""Cmune.DataCenter.Common.Entities.ChannelType"" />
          <parameter name=""machineId"" type=""string"" />
        </parameters>
      </operation>
    </operations>
  </service>
  <service name=""ClanWebService"" endpoint=""UberStrike.DataCenter.WebService.CWS.ClanWebServiceContract.svc"" contract=""IClanWebServiceContract"">
    <operations>
      <operation name=""IsMemberPartOfGroup"" returnType=""bool"">
        <parameters>
          <parameter name=""cmuneId"" type=""int"" />
          <parameter name=""groupId"" type=""int"" />
        </parameters>
      </operation>
      <operation name=""IsMemberPartOfAnyGroup"" returnType=""bool"">
        <parameters>
          <parameter name=""cmuneId"" type=""int"" />
          <parameter name=""applicationId"" type=""int"" />
        </parameters>
      </operation>
      <operation name=""GetClan"" returnType=""Cmune.DataCenter.Common.Entities.ClanView"">
        <parameters>
          <parameter name=""groupId"" type=""int"" />
        </parameters>
      </operation>
      <operation name=""UpdateMemberPosition"" returnType=""int"">
        <parameters>
          <parameter name=""updateMemberPositionData"" type=""Cmune.DataCenter.Common.Entities.MemberPositionUpdateView"" />
        </parameters>
      </operation>
      <operation name=""InviteMemberToJoinAGroup"" returnType=""int"">
        <parameters>
          <parameter name=""clanId"" type=""int"" />
          <parameter name=""inviterCmid"" type=""int"" />
          <parameter name=""inviteeCmid"" type=""int"" />
          <parameter name=""message"" type=""string"" />
        </parameters>
      </operation>
      <operation name=""AcceptClanInvitation"" returnType=""Cmune.DataCenter.Common.Entities.ClanRequestAcceptView"">
        <parameters>
          <parameter name=""clanInvitationId"" type=""int"" />
          <parameter name=""cmid"" type=""int"" />
        </parameters>
      </operation>
      <operation name=""DeclineClanInvitation"" returnType=""Cmune.DataCenter.Common.Entities.ClanRequestDeclineView"">
        <parameters>
          <parameter name=""clanInvitationId"" type=""int"" />
          <parameter name=""cmid"" type=""int"" />
        </parameters>
      </operation>
      <operation name=""KickMemberFromClan"" returnType=""int"">
        <parameters>
          <parameter name=""groupId"" type=""int"" />
          <parameter name=""cmidTakingAction"" type=""int"" />
          <parameter name=""cmidToKick"" type=""int"" />
        </parameters>
      </operation>
      <operation name=""DisbandGroup"" returnType=""int"">
        <parameters>
          <parameter name=""groupId"" type=""int"" />
          <parameter name=""cmidTakingAction"" type=""int"" />
        </parameters>
      </operation>
      <operation name=""LeaveAClan"" returnType=""int"">
        <parameters>
          <parameter name=""groupId"" type=""int"" />
          <parameter name=""cmid"" type=""int"" />
        </parameters>
      </operation>
      <operation name=""GetMyClanId"" returnType=""int"">
        <parameters>
          <parameter name=""cmid"" type=""int"" />
          <parameter name=""applicationId"" type=""int"" />
        </parameters>
      </operation>
      <operation name=""CancelInvitation"" returnType=""int"">
        <parameters>
          <parameter name=""groupInvitationId"" type=""int"" />
          <parameter name=""cmid"" type=""int"" />
        </parameters>
      </operation>
      <operation name=""CancelRequest"" returnType=""int"">
        <parameters>
          <parameter name=""groupRequestId"" type=""int"" />
        </parameters>
      </operation>
      <operation name=""GetAllGroupInvitations"" returnType=""System.Collections.Generic.List&lt;Cmune.DataCenter.Common.Entities.GroupInvitationView&gt;"">
        <parameters>
          <parameter name=""inviteeCmid"" type=""int"" />
          <parameter name=""applicationId"" type=""int"" />
        </parameters>
      </operation>
      <operation name=""GetPendingGroupInvitations"" returnType=""System.Collections.Generic.List&lt;Cmune.DataCenter.Common.Entities.GroupInvitationView&gt;"">
        <parameters>
          <parameter name=""groupId"" type=""int"" />
        </parameters>
      </operation>
      <operation name=""CreateClan"" returnType=""Cmune.DataCenter.Common.Entities.ClanCreationReturnView"">
        <parameters>
          <parameter name=""createClanData"" type=""Cmune.DataCenter.Common.Entities.GroupCreationView"" />
        </parameters>
      </operation>
      <operation name=""TransferOwnership"" returnType=""int"">
        <parameters>
          <parameter name=""groupId"" type=""int"" />
          <parameter name=""previousLeaderCmid"" type=""int"" />
          <parameter name=""newLeaderCmid"" type=""int"" />
        </parameters>
      </operation>
      <operation name=""CanOwnAClan"" returnType=""int"">
        <parameters>
          <parameter name=""cmid"" type=""int"" />
        </parameters>
      </operation>
      <operation name=""test"" returnType=""int"">
        <parameters />
      </operation>
    </operations>
  </service>
  <service name=""ModerationWebService"" endpoint=""UberStrike.DataCenter.WebService.CWS.ModerationWebServiceContract.svc"" contract=""IModerationWebServiceContract"">
    <operations>
      <operation name=""BanPermanently"" returnType=""bool"">
        <parameters>
          <parameter name=""sourceCmid"" type=""int"" />
          <parameter name=""targetCmid"" type=""int"" />
          <parameter name=""applicationId"" type=""int"" />
          <parameter name=""ip"" type=""string"" />
        </parameters>
      </operation>
    </operations>
  </service>
  <service name=""PrivateMessageWebService"" endpoint=""UberStrike.DataCenter.WebService.CWS.PrivateMessageWebServiceContract.svc"" contract=""IPrivateMessageWebServiceContract"">
    <operations>
      <operation name=""GetAllMessageThreadsForUser"" returnType=""System.Collections.Generic.List&lt;Cmune.DataCenter.Common.Entities.MessageThreadView&gt;"">
        <parameters>
          <parameter name=""cmid"" type=""int"" />
        </parameters>
      </operation>
      <operation name=""GetAllMessageThreadsForUser"" returnType=""System.Collections.Generic.List&lt;Cmune.DataCenter.Common.Entities.MessageThreadView&gt;"">
        <parameters>
          <parameter name=""cmid"" type=""int"" />
          <parameter name=""pageNumber"" type=""int"" />
        </parameters>
      </operation>
      <operation name=""GetThreadMessages"" returnType=""System.Collections.Generic.List&lt;Cmune.DataCenter.Common.Entities.PrivateMessageView&gt;"">
        <parameters>
          <parameter name=""threadViewerCmid"" type=""int"" />
          <parameter name=""otherCmid"" type=""int"" />
          <parameter name=""pageNumber"" type=""int"" />
        </parameters>
      </operation>
      <operation name=""SendMessage"" returnType=""Cmune.DataCenter.Common.Entities.PrivateMessageView"">
        <parameters>
          <parameter name=""senderCmid"" type=""int"" />
          <parameter name=""receiverCmid"" type=""int"" />
          <parameter name=""content"" type=""string"" />
        </parameters>
      </operation>
      <operation name=""GetMessageWithId"" returnType=""Cmune.DataCenter.Common.Entities.PrivateMessageView"">
        <parameters>
          <parameter name=""messageId"" type=""int"" />
          <parameter name=""requesterCmid"" type=""int"" />
        </parameters>
      </operation>
      <operation name=""MarkThreadAsRead"" returnType=""void"">
        <parameters>
          <parameter name=""threadViewerCmid"" type=""int"" />
          <parameter name=""otherCmid"" type=""int"" />
        </parameters>
      </operation>
      <operation name=""DeleteThread"" returnType=""void"">
        <parameters>
          <parameter name=""threadViewerCmid"" type=""int"" />
          <parameter name=""otherCmid"" type=""int"" />
        </parameters>
      </operation>
    </operations>
  </service>
  <service name=""RelationshipWebService"" endpoint=""UberStrike.DataCenter.WebService.CWS.RelationshipWebServiceContract.svc"" contract=""IRelationshipWebServiceContract"">
    <operations>
      <operation name=""SendContactRequest"" returnType=""int"">
        <parameters>
          <parameter name=""initiatorCmid"" type=""int"" />
          <parameter name=""receiverCmid"" type=""int"" />
          <parameter name=""message"" type=""string"" />
        </parameters>
      </operation>
      <operation name=""GetContactRequests"" returnType=""System.Collections.Generic.List&lt;Cmune.DataCenter.Common.Entities.ContactRequestView&gt;"">
        <parameters>
          <parameter name=""cmid"" type=""int"" />
        </parameters>
      </operation>
      <operation name=""AcceptContactRequest"" returnType=""Cmune.DataCenter.Common.Entities.ContactRequestAcceptView"">
        <parameters>
          <parameter name=""contactRequestId"" type=""int"" />
          <parameter name=""cmid"" type=""int"" />
          <parameter name=""applicationId"" type=""int"" />
        </parameters>
      </operation>
      <operation name=""DeclineContactRequest"" returnType=""Cmune.DataCenter.Common.Entities.ContactRequestDeclineView"">
        <parameters>
          <parameter name=""contactRequestId"" type=""int"" />
          <parameter name=""cmid"" type=""int"" />
        </parameters>
      </operation>
      <operation name=""DeleteContact"" returnType=""Cmune.DataCenter.Common.Entities.MemberOperationResult"">
        <parameters>
          <parameter name=""cmid"" type=""int"" />
          <parameter name=""contactCmid"" type=""int"" />
        </parameters>
      </operation>
      <operation name=""MoveContactToGroup"" returnType=""Cmune.DataCenter.Common.Entities.MemberOperationResult"">
        <parameters>
          <parameter name=""cmid"" type=""int"" />
          <parameter name=""contactCmid"" type=""int"" />
          <parameter name=""previousGroupId"" type=""int"" />
          <parameter name=""newGroupId"" type=""int"" />
        </parameters>
      </operation>
      <operation name=""GetContactsByGroups"" returnType=""System.Collections.Generic.List&lt;Cmune.DataCenter.Common.Entities.ContactGroupView&gt;"">
        <parameters>
          <parameter name=""cmid"" type=""int"" />
          <parameter name=""applicationId"" type=""int"" />
        </parameters>
      </operation>
    </operations>
  </service>
  <service name=""ShopWebService"" endpoint=""UberStrike.DataCenter.WebService.CWS.ShopWebServiceContract.svc"" contract=""IShopWebServiceContract"">
    <operations>
      <operation name=""GetShop"" returnType=""UberStrike.Core.Models.Views.UberStrikeItemShopClientView"">
        <parameters>
          <parameter name=""applicationVersion"" type=""string"" />
        </parameters>
      </operation>
      <operation name=""BuyItem"" returnType=""int"">
        <parameters>
          <parameter name=""itemId"" type=""int"" />
          <parameter name=""buyerCmid"" type=""int"" />
          <parameter name=""currencyType"" type=""Cmune.DataCenter.Common.Entities.UberStrikeCurrencyType"" />
          <parameter name=""durationType"" type=""Cmune.DataCenter.Common.Entities.BuyingDurationType"" />
          <parameter name=""itemType"" type=""UberStrike.Core.Types.UberstrikeItemType"" />
          <parameter name=""marketLocation"" type=""Cmune.DataCenter.Common.Entities.BuyingLocationType"" />
          <parameter name=""recommendationType"" type=""Cmune.DataCenter.Common.Entities.BuyingRecommendationType"" />
        </parameters>
      </operation>
      <operation name=""BuyPack"" returnType=""int"">
        <parameters>
          <parameter name=""itemId"" type=""int"" />
          <parameter name=""buyerCmid"" type=""int"" />
          <parameter name=""packType"" type=""Cmune.DataCenter.Common.Entities.PackType"" />
          <parameter name=""currencyType"" type=""Cmune.DataCenter.Common.Entities.UberStrikeCurrencyType"" />
          <parameter name=""itemType"" type=""UberStrike.Core.Types.UberstrikeItemType"" />
          <parameter name=""marketLocation"" type=""Cmune.DataCenter.Common.Entities.BuyingLocationType"" />
          <parameter name=""recommendationType"" type=""Cmune.DataCenter.Common.Entities.BuyingRecommendationType"" />
        </parameters>
      </operation>
      <operation name=""GetBundles"" returnType=""System.Collections.Generic.List&lt;Cmune.DataCenter.Common.Entities.BundleView&gt;"">
        <parameters>
          <parameter name=""channel"" type=""Cmune.DataCenter.Common.Entities.ChannelType"" />
        </parameters>
      </operation>
      <operation name=""BuyMasBundle"" returnType=""bool"">
        <parameters>
          <parameter name=""cmid"" type=""int"" />
          <parameter name=""bundleId"" type=""int"" />
          <parameter name=""hashedReceipt"" type=""string"" />
          <parameter name=""applicationId"" type=""int"" />
        </parameters>
      </operation>
      <operation name=""BuyiPadBundle"" returnType=""bool"">
        <parameters>
          <parameter name=""cmid"" type=""int"" />
          <parameter name=""bundleId"" type=""int"" />
          <parameter name=""hashedReceipt"" type=""string"" />
          <parameter name=""applicationId"" type=""int"" />
        </parameters>
      </operation>
      <operation name=""BuyiPhoneBundle"" returnType=""bool"">
        <parameters>
          <parameter name=""cmid"" type=""int"" />
          <parameter name=""bundleId"" type=""int"" />
          <parameter name=""hashedReceipt"" type=""string"" />
          <parameter name=""applicationId"" type=""int"" />
        </parameters>
      </operation>
      <operation name=""UseConsumableItem"" returnType=""bool"">
        <parameters>
          <parameter name=""cmid"" type=""int"" />
          <parameter name=""itemId"" type=""int"" />
        </parameters>
      </operation>
      <operation name=""GetAllMysteryBoxs"" returnType=""System.Collections.Generic.List&lt;Cmune.DataCenter.Common.Entities.MysteryBoxUnityView&gt;"">
        <parameters />
      </operation>
      <operation name=""GetAllMysteryBoxs"" returnType=""System.Collections.Generic.List&lt;Cmune.DataCenter.Common.Entities.MysteryBoxUnityView&gt;"">
        <parameters>
          <parameter name=""bundleCategoryType"" type=""Cmune.DataCenter.Common.Entities.BundleCategoryType"" />
        </parameters>
      </operation>
      <operation name=""GetMysteryBox"" returnType=""Cmune.DataCenter.Common.Entities.MysteryBoxUnityView"">
        <parameters>
          <parameter name=""mysteryBoxId"" type=""int"" />
        </parameters>
      </operation>
      <operation name=""RollMysteryBox"" returnType=""System.Collections.Generic.List&lt;Cmune.DataCenter.Common.Entities.MysteryBoxWonItemUnityView&gt;"">
        <parameters>
          <parameter name=""cmid"" type=""int"" />
          <parameter name=""mysteryBoxId"" type=""int"" />
          <parameter name=""channel"" type=""Cmune.DataCenter.Common.Entities.ChannelType"" />
        </parameters>
      </operation>
      <operation name=""GetAllLuckyDraws"" returnType=""System.Collections.Generic.List&lt;Cmune.DataCenter.Common.Entities.LuckyDrawUnityView&gt;"">
        <parameters />
      </operation>
      <operation name=""GetAllLuckyDraws"" returnType=""System.Collections.Generic.List&lt;Cmune.DataCenter.Common.Entities.LuckyDrawUnityView&gt;"">
        <parameters>
          <parameter name=""bundleCategoryType"" type=""Cmune.DataCenter.Common.Entities.BundleCategoryType"" />
        </parameters>
      </operation>
      <operation name=""GetLuckyDraw"" returnType=""Cmune.DataCenter.Common.Entities.LuckyDrawUnityView"">
        <parameters>
          <parameter name=""luckyDrawId"" type=""int"" />
        </parameters>
      </operation>
      <operation name=""RollLuckyDraw"" returnType=""int"">
        <parameters>
          <parameter name=""cmid"" type=""int"" />
          <parameter name=""luckDrawId"" type=""int"" />
          <parameter name=""channel"" type=""Cmune.DataCenter.Common.Entities.ChannelType"" />
        </parameters>
      </operation>
    </operations>
  </service>
  <service name=""UserWebService"" endpoint=""UberStrike.DataCenter.WebService.CWS.UserWebServiceContract.svc"" contract=""IUserWebServiceContract"">
    <operations>
      <operation name=""ChangeMemberName"" returnType=""Cmune.DataCenter.Common.Entities.MemberOperationResult"">
        <parameters>
          <parameter name=""cmid"" type=""int"" />
          <parameter name=""name"" type=""string"" />
          <parameter name=""locale"" type=""string"" />
          <parameter name=""machineId"" type=""string"" />
        </parameters>
      </operation>
      <operation name=""IsDuplicateMemberName"" returnType=""bool"">
        <parameters>
          <parameter name=""username"" type=""string"" />
        </parameters>
      </operation>
      <operation name=""GenerateNonDuplicatedMemberNames"" returnType=""System.Collections.Generic.List&lt;string&gt;"">
        <parameters>
          <parameter name=""username"" type=""string"" />
        </parameters>
      </operation>
      <operation name=""GetPublicProfile"" returnType=""Cmune.DataCenter.Common.Entities.PublicProfileView"">
        <parameters>
          <parameter name=""cmid"" type=""int"" />
          <parameter name=""applicationId"" type=""int"" />
        </parameters>
      </operation>
      <operation name=""GetMemberWallet"" returnType=""Cmune.DataCenter.Common.Entities.MemberWalletView"">
        <parameters>
          <parameter name=""cmid"" type=""int"" />
        </parameters>
      </operation>
      <operation name=""GetInventory"" returnType=""System.Collections.Generic.List&lt;Cmune.DataCenter.Common.Entities.ItemInventoryView&gt;"">
        <parameters>
          <parameter name=""cmid"" type=""int"" />
        </parameters>
      </operation>
      <operation name=""ReportMember"" returnType=""bool"">
        <parameters>
          <parameter name=""memberReport"" type=""Cmune.DataCenter.Common.Entities.MemberReportView"" />
        </parameters>
      </operation>
      <operation name=""FindMembers"" returnType=""System.Collections.Generic.Dictionary&lt;int,string&gt;"">
        <parameters>
          <parameter name=""name"" type=""string"" />
          <parameter name=""maxResults"" type=""int"" />
        </parameters>
      </operation>
      <operation name=""GetCurrencyDeposits"" returnType=""UberStrike.Core.ViewModel.CurrencyDepositsViewModel"">
        <parameters>
          <parameter name=""cmid"" type=""int"" />
          <parameter name=""pageIndex"" type=""int"" />
          <parameter name=""elementPerPage"" type=""int"" />
        </parameters>
      </operation>
      <operation name=""GetItemTransactions"" returnType=""UberStrike.Core.ViewModel.ItemTransactionsViewModel"">
        <parameters>
          <parameter name=""cmid"" type=""int"" />
          <parameter name=""pageIndex"" type=""int"" />
          <parameter name=""elementPerPage"" type=""int"" />
        </parameters>
      </operation>
      <operation name=""GetPointsDeposits"" returnType=""UberStrike.Core.ViewModel.PointDepositsViewModel"">
        <parameters>
          <parameter name=""cmid"" type=""int"" />
          <parameter name=""pageIndex"" type=""int"" />
          <parameter name=""elementPerPage"" type=""int"" />
        </parameters>
      </operation>
      <operation name=""GetUserAndTopStats"" returnType=""System.Collections.Generic.List&lt;UberStrike.DataCenter.Common.Entities.PlayerCardView&gt;"">
        <parameters>
          <parameter name=""cmid"" type=""int"" />
        </parameters>
      </operation>
      <operation name=""GetRealTimeStatistics"" returnType=""UberStrike.DataCenter.Common.Entities.PlayerCardView"">
        <parameters>
          <parameter name=""cmid"" type=""int"" />
        </parameters>
      </operation>
      <operation name=""SetScore"" returnType=""void"">
        <parameters>
          <parameter name=""scoringView"" type=""UberStrike.DataCenter.Common.Entities.MatchView"" />
        </parameters>
      </operation>
      <operation name=""GetMember"" returnType=""UberStrike.Core.ViewModel.UberstrikeUserViewModel"">
        <parameters>
          <parameter name=""cmid"" type=""int"" />
        </parameters>
      </operation>
      <operation name=""GetStatistics"" returnType=""UberStrike.DataCenter.Common.Entities.PlayerCardView"">
        <parameters>
          <parameter name=""cmid"" type=""int"" />
        </parameters>
      </operation>
      <operation name=""GetLoadout"" returnType=""UberStrike.DataCenter.Common.Entities.LoadoutView"">
        <parameters>
          <parameter name=""cmid"" type=""int"" />
        </parameters>
      </operation>
      <operation name=""SetLoadout"" returnType=""Cmune.DataCenter.Common.Entities.MemberOperationResult"">
        <parameters>
          <parameter name=""loadoutView"" type=""UberStrike.DataCenter.Common.Entities.LoadoutView"" />
        </parameters>
      </operation>
      <operation name=""GetXPEventsView"" returnType=""System.Collections.Generic.Dictionary&lt;int,UberStrike.DataCenter.Common.Entities.PlayerXPEventView&gt;"">
        <parameters />
      </operation>
      <operation name=""GetLevelCapsView"" returnType=""System.Collections.Generic.List&lt;UberStrike.DataCenter.Common.Entities.PlayerLevelCapView&gt;"">
        <parameters />
      </operation>
    </operations>
  </service>
</services>";
}