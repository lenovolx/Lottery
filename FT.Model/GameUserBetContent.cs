

namespace FT.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class GameUserBetContent
    {
        public long Id { get; set; }
        public long BetId { get; set; }
        public string TermNumber { get; set; }
        public string BetContent { get; set; }
        public Nullable<byte> BetType { get; set; }
        public Nullable<decimal> CurrentOdds { get; set; }
        public decimal BetMoney { get; set; }
        public Nullable<bool> IsGetPrize { get; set; }
        public Nullable<decimal> WinningMoney { get; set; }
        public Nullable<int> BetRate { get; set; }
        public Nullable<long> BetTime { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
    
        public virtual GameUserBet GameUserBet { get; set; }
    }
}
