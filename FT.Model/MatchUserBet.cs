

namespace FT.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class MatchUserBet
    {
        public MatchUserBet()
        {
            this.MatchUserBetContent = new HashSet<MatchUserBetContent>();
        }
    
        public long BetId { get; set; }
        public decimal BetValue { get; set; }
        public decimal BetBonus { get; set; }
        public long BetTime { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long UserId { get; set; }
        public int BetType { get; set; }
        public OpenEnum IsSettle { get; set; }
        public Nullable<System.DateTime> SettleTime { get; set; }
        public YesNoEnum IsGetPrize { get; set; }
        public Nullable<System.DateTime> GetTime { get; set; }
        public DataSourceEnum BetSource { get; set; }
        public BetStatus Status { get; set; }
        public decimal ValidAmount { get; set; }
    
        public virtual ICollection<MatchUserBetContent> MatchUserBetContent { get; set; }
        public virtual UserAccount UserAccount { get; set; }
    }
}
