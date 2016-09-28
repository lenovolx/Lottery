

namespace FT.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class MatchInfoLog
    {
        public long Id { get; set; }
        public Nullable<int> Version { get; set; }
        public Nullable<long> MatchId { get; set; }
        public string LeagueName { get; set; }
        public long MatchDate { get; set; }
        public int MatchNumber { get; set; }
        public int MatchType { get; set; }
        public long MatchEndTime { get; set; }
        public string HTeamNum { get; set; }
        public string HTeam { get; set; }
        public string CTeamNum { get; set; }
        public string CTeam { get; set; }
        public string BetType { get; set; }
        public string Odds { get; set; }
        public Nullable<long> OddsUpdateTime { get; set; }
        public string LeagueNameEn { get; set; }
        public string HTeamEn { get; set; }
        public string CTeamEn { get; set; }
    }
}
