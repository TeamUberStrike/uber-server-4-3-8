using System;

namespace UberStrike.Channels.Common.Models
{
    public class UserProfileViewModel
    {
        public UserProfileViewModel()
        {

        }

        public int GlobalRank { get; set; }
        public DateTime JoinedDateTime { get; set; }
        public UserLoadoutModel Loadout { get; set; }
        public PersonalBestsModel PersonalBestsPerLife { get; set; }
        public string UserName { get; set; }
        public string ClanTag { get; set; }
        /// <summary>
        /// If the user is a UberStrike user he will have a loadout and personnal records, if he's a Paradise Paintball user he won't
        /// </summary>
        public bool IsUberStrikeUser { get; set; }
        public int Level { get; set; }
        public int Xp { get; set; }
        public AllTimeStatsModel AllTimesStats { get; set; }
        public WeaponsStatsModel WeaponsStats { get; set; }
    }
}