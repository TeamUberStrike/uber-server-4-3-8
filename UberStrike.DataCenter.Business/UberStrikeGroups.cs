using System;
using Cmune.DataCenter.Business;
using Cmune.DataCenter.Common.Entities;
using Cmune.DataCenter.DataAccess;
using UberStrike.DataCenter.Common.Entities;
using UberStrike.DataCenter.DataAccess;
using Cmune.DataCenter.Utils;
using System.Collections.Generic;
using Cmune.DataCenter.Common.Utils;
using UberStrike.Core.Types;

namespace UberStrike.DataCenter.Business
{
    public static class UberStrikeGroups
    {
        /// <summary>
        /// Checks whether a member can own a clan
        /// </summary>
        /// <param name="cmid"></param>
        /// <returns>from (UberStrikeGroupOperationResult)</returns>
        public static int CanOwnAClan(int cmid)
        {
            int result = UberStrikeGroupOperationResult.Ok;

            ItemInventory clanLeaderLicence = CmuneMember.GetItemInventory(cmid, UberStrikeCommonConfig.ClanLeaderLicenseId);

            if (clanLeaderLicence == null)
            {
                result = UberStrikeGroupOperationResult.ClanLicenceNotFound;
            }

            if (result.Equals(GroupOperationResult.Ok))
            {
                User user = Users.GetUser(cmid);

                if (user != null)
                {
                    if (user.Level < UberStrikeCommonConfig.ClanLeaderMinLevel)
                    {
                        result = UberStrikeGroupOperationResult.InvalidLevel;
                    }
                }
                else
                {
                    result = UberStrikeGroupOperationResult.MemberNotFound;
                }
            }

            if (result.Equals(GroupOperationResult.Ok))
            {
                int contactsCount = CmuneRelationship.CountContacts(cmid);

                if (contactsCount < UberStrikeCommonConfig.ClanLeaderMinContactsCount)
                {
                    result = UberStrikeGroupOperationResult.InvalidContactsCount;
                }
            }

            return result;
        }

        /// <summary>
        /// Create a new clan
        /// </summary>
        /// <param name="name"></param>
        /// <param name="motto"></param>
        /// <param name="cmid"></param>
        /// <param name="tag"></param>
        /// <param name="locale"></param>
        /// <returns></returns>
        public static ClanCreationReturnView CreateGroup(string name, string motto, int cmid, string tag, string locale)
        {
            ClanCreationReturnView result = new ClanCreationReturnView();
            result.ResultCode = CanOwnAClan(cmid);

            if (result.ResultCode == UberStrikeGroupOperationResult.Ok)
            {
                int clanId = 0;
                result.ResultCode = CmuneGroups.CreateGroup(cmid, name, motto, tag, locale, UberStrikeCommonConfig.ApplicationId, out clanId);

                if (result.ResultCode == UberStrikeGroupOperationResult.Ok)
                {
                    result.ClanView = CmuneGroups.GetGroupView(clanId);
                }
            }

            return result;
        }

        /// <summary>
        /// Transfers the ownership to another member of the clan
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="previousLeaderCmid"></param>
        /// <param name="newLeaderCmid"></param>
        /// <returns></returns>
        public static int TransferOwnership(int groupId, int previousLeaderCmid, int newLeaderCmid)
        {
            int result = UberStrikeGroupOperationResult.Ok;

            int canMemberOwnAClan = CanOwnAClan(newLeaderCmid);

            if (canMemberOwnAClan == UberStrikeGroupOperationResult.Ok)
            {
                result = CmuneGroups.TransferOwnership(groupId, previousLeaderCmid, newLeaderCmid);
            }
            else
            {
                result = canMemberOwnAClan;
            }

            return result;
        }

        #region Cache propagation

        /// <summary>
        /// Propagates a tag name change to all the cache tables
        /// </summary>
        /// <param name="clanTag"></param>
        /// <param name="cmid"></param>
        /// <returns></returns>
        public static bool PropagateClanTag(string clanTag, int cmid)
        {
            using (UberstrikeDataContext paradiseDB = new UberstrikeDataContext())
            {
                bool isPropagated = false;

                int res = paradiseDB.Paradise_Propagate_TagName(clanTag, cmid);

                if (res == CommonConfig.StoredProcedureSuccess)
                {
                    isPropagated = true;
                }
                else
                {
                    CmuneLog.LogUnexpectedReturn(res, "clanTag=" + clanTag + "&cmid=" + cmid.ToString());
                }

                return isPropagated;
            }
        }

        /// <summary>
        /// Propagates a tag name change to all the cache tables
        /// </summary>
        /// <param name="clanTag"></param>
        /// <param name="cmids"></param>
        /// <returns></returns>
        public static bool PropagateClanTag(string clanTag, List<int> cmids)
        {
            using (UberstrikeDataContext paradiseDB = new UberstrikeDataContext())
            {
                bool isPropagated = false;

                if (cmids.Count > 0)
                {
                    string joinedCmid = TextUtilities.Join(", ", cmids);

                    int res = paradiseDB.UberStrike_Propagate_Tags(clanTag, joinedCmid);

                    if (res == CommonConfig.StoredProcedureSuccess)
                    {
                        isPropagated = true;
                    }
                    else
                    {
                        CmuneLog.LogUnexpectedReturn(res, "clanTag=" + clanTag + "&cmids=" + joinedCmid);
                    }
                }

                return isPropagated;
            }
        }

        #endregion Cache propagation

        #region Setters

        /// <summary>
        /// Change the clan tag
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="groupTag"></param>
        /// <param name="locale"></param>
        /// <returns>from (UberStrikeGroupOperationResult)</returns>
        public static int SetGroupTag(int groupId, string groupTag, string locale)
        {
            List<int> cmidsToUpdate;
            int ret = CmuneGroups.SetGroupTag(groupId, groupTag, locale, out cmidsToUpdate);

            if (ret.Equals(GroupOperationResult.Ok))
            {
                PropagateClanTag(groupTag, cmidsToUpdate);
            }

            return ret;
        }

        #endregion Setters
    }
}