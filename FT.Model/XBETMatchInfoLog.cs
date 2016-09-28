

namespace FT.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class XBETMatchInfoLog
    {
        public long Id { get; set; }
        public string LeagueName { get; set; }
        public string LeagueNameEn { get; set; }
        public long MatchDate { get; set; }
        public int MatchNumber { get; set; }
        public int MatchType { get; set; }
        public long MatchEndTime { get; set; }
        public string HTeamNum { get; set; }
        public string HTeam { get; set; }
        public string HTeamEn { get; set; }
        public string CTeamNum { get; set; }
        public string CTeam { get; set; }
        public string CTeamEn { get; set; }
        public string BetType { get; set; }
        public string Odds { get; set; }
        public Nullable<long> OddsUpdateTime { get; set; }
        public string CurrentScore { get; set; }
        public Nullable<int> TimeSec { get; set; }
        public bool Finished { get; set; }
        public bool TimeRun { get; set; }
    }
}
