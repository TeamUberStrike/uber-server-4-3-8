using Cmune.DataCenter.Common.Entities;
using Cmune.DataCenter.Common.Utils;

namespace Cmune.DataCenter.Utils
{
    public static class ValidationUtils
    {
        /// <summary>
        /// Test whether a member name is valid (respecting our own rules + external validation for swear words)
        /// </summary>
        /// <param name="memberName"></param>
        /// <param name="locale"></param>
        /// <returns>Ok, InvalidName or OffensiveName</returns>
        public static MemberOperationResult IsMemberNameFullyValid(string memberName, string locale)
        {
            MemberOperationResult ret = MemberOperationResult.Ok;

            if (!ValidationUtilities.IsValidMemberName(memberName, locale))
            {
                ret = MemberOperationResult.InvalidName;
            }

          
            return ret;
        }

        /// <summary>
        /// Test whether a group name is valid (respecting our own rules + external validation for swear words)
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="locale"></param>
        /// <returns>Ok, InvalidName or OffensiveName (from GroupOperationResult)</returns>
        public static int IsGroupNameFullyValid(string groupName, string locale)
        {
            int ret = GroupOperationResult.Ok;

            if (!ValidationUtilities.IsValidClanName(groupName, locale))
            {
                ret = GroupOperationResult.InvalidName;
            }

            return ret;
        }

        /// <summary>
        /// Test whether a group tag is valid (respecting our own rules + external validation for swear words)
        /// </summary>
        /// <param name="groupTag"></param>
        /// <param name="locale"></param>
        /// <returns>Ok, InvalidTag or OffensiveTag (from GroupOperationResult)</returns>
        public static int IsGroupTagFullyValid(string groupTag, string locale)
        {
            int ret = GroupOperationResult.Ok;

            if (!ValidationUtilities.IsValidClanTag(groupTag, locale))
            {
                ret = GroupOperationResult.InvalidTag;
            }

            return ret;
        }

        /// <summary>
        /// Test whether a group motto is valid (respecting our own rules + external validation for swear words)
        /// </summary>
        /// <param name="groupMotto"></param>
        /// <returns>Ok, InvalidMotto or OffensiveMotto from GroupOperationResult</returns>
        public static int IsGroupMottoFullyValid(string groupMotto)
        {
            int ret = GroupOperationResult.Ok;

            if (!ValidationUtilities.IsValidClanMotto(groupMotto))
            {
                ret = GroupOperationResult.InvalidMotto;
            }

            return ret;
        }

        /// <summary>
        /// Test whether a group description is valid (respecting our own rules + external validation for swear words)
        /// </summary>
        /// <param name="groupDescription"></param>
        /// <returns>Ok, InvalidMotto or OffensiveMotto (from GroupOperationResult)</returns>
        public static int IsGroupDescriptionFullyValid(string groupDescription)
        {
            int ret = GroupOperationResult.Ok;

            if (!ValidationUtilities.IsValidClanDesciption(groupDescription))
            {
                ret = GroupOperationResult.InvalidDescription;
            }

            return ret;
        }
    }
}