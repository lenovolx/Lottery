

namespace FT.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class MatchUserBetContentLog
    {
        public long Id { get; set; }
        public long BetId { get; set; }
        public long MatchID { get; set; }
        public int BetType { get; set; }
        public string BetContent { get; set; }
        public string BetKey { get; set; }
        public Nullable<int> IsSettle { get; set; }
        public Nullable<decimal> SettleIor { get; set; }
        public string BetScore { get; set; }
    }
}
