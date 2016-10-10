

namespace FT.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class GameUserBet
    {
        public GameUserBet()
        {
            this.GameUserBetContent = new HashSet<GameUserBetContent>();
        }
    
        public long Id { get; set; }
        public long UserId { get; set; }
        public string TermNumber { get; set; }
        public string GameCategory { get; set; }
        public string GameNumber { get; set; }
        public decimal BetMoney { get; set; }
        public Nullable<bool> IsDraw { get; set; }
        public Nullable<bool> IsGetPrize { get; set; }
        public Nullable<long> BetTime { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public Nullable<decimal> WinningMoney { get; set; }
    
        public virtual ICollection<GameUserBetContent> GameUserBetContent { get; set; }
        public virtual UserAccount UserAccount { get; set; }
    }
}
