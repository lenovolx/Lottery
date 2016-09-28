

namespace FT.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class MatchUserBetContent
    {
        public long Id { get; set; }
        public long BetId { get; set; }
        public long MatchID { get; set; }
        public int BetType { get; set; }
        public string BetContent { get; set; }
        public string BetKey { get; set; }
        public Nullable<OpenEnum> IsSettle { get; set; }
        public Nullable<decimal> SettleIor { get; set; }
        public string BetScore { get; set; }
    
        public virtual MatchUserBet MatchUserBet { get; set; }
        public virtual MatchInfo MatchInfo { get; set; }
    }
}
