

namespace FT.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class TradeRecord
    {
        public long Id { get; set; }
        public Nullable<long> FromId { get; set; }
        public long ToId { get; set; }
        public TradEnum Type { get; set; }
        public Nullable<decimal> TradeAmount { get; set; }
        public string CardNum { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<long> CreateUserId { get; set; }
        public string FromUserName { get; set; }
        public string ToUserName { get; set; }
        public LockEnum Status { get; set; }
    
        public virtual UserAccount UserAccount { get; set; }
    }
}
