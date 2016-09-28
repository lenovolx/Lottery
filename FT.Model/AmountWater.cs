

namespace FT.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class AmountWater
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; }
        public decimal Amount { get; set; }
        public TradEnum Type { get; set; }
        public string Remark { get; set; }
        public System.DateTime CreateDate { get; set; }
    
        public virtual UserAccount UserAccount { get; set; }
    }
}
