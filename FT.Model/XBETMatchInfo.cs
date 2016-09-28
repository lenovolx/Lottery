

namespace FT.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class XBETMatchInfo
    {
        public XBETMatchInfo()
        {
            this.XBETMatchResult = new HashSet<XBETMatchResult>();
        }
    
        public long MatchId { get; set; }
        public string LeagueName { get; set; }
        public string LeagueNameEn { get; set; }
        public long MatchDate { get; set; }
        public int MatchNumber { get; set; }
        public Nullable<long> EventId { get; set; }
        public int MatchType { get; set; }
        public long MatchEndTime { get; set; }
        public string HTeamNum { get; set; }
        public string HTeam { get; set; }
        public string HTeamEn { get; set; }
        public string CTeamNum { get; set; }
        public string CTeam { get; set; }
        public string CTeamEn { get; set; }
        public string BetType { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string Odds { get; set; }
        public long OddsUpdateTime { get; set; }
        public OpenEnum IsSettle { get; set; }
        public Nullable<System.DateTime> SettleTime { get; set; }
        public Nullable<long> SettleUserId { get; set; }
        public Nullable<int> MinLimit { get; set; }
        public Nullable<int> MaxLimit { get; set; }
        public int IsMaster { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public string CurrentScore { get; set; }
        public int TimeSec { get; set; }
        public bool Finished { get; set; }
        public bool TimeRun { get; set; }
    
        public virtual ICollection<XBETMatchResult> XBETMatchResult { get; set; }
    }
}
