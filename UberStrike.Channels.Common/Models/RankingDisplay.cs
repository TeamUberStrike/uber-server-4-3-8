using System;
using System.Collections.Generic;
using UberStrike.Channels.Utils;
using UberStrike.DataCenter.Common.Entities;

namespace UberStrike.Channels.Common.Models
{
    public class RankingDisplay
    {
        public List<RankingView> Ranking { get; private set; }
        public PaginationModel Pagination { get; private set; }
        public string RankingType { get; private set; }
        public DateTime RankingDate { get; private set; }

        public RankingDisplay(List<RankingView> ranking, PaginationModel pagination, string rankingType, DateTime rankingDate)
        {
            Ranking = ranking;
            Pagination = pagination;
            RankingType = rankingType;
            RankingDate = rankingDate;
        }
    }
}
