

namespace FT.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class MatchUserBetLog
    {
        public long BetId { get; set; }
        public long UserId { get; set; }
        public decimal BetValue { get; set; }
        public int BetType { get; set; }
        public decimal BetBonus { get; set; }
        public long BetTime { get; set; }
        public System.DateTime CreateDate { get; set; }
        public byte IsSettle { get; set; }
        public Nullable<System.DateTime> SettleTime { get; set; }
        public int IsGetPrize { get; set; }
        public Nullable<System.DateTime> GetTime { get; set; }
        public int BetSource { get; set; }
    }
}
