

namespace FT.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class Card
    {
        public long Id { get; set; }
        public string CardNum { get; set; }
        public string CardPwd { get; set; }
        public Nullable<long> GroupID { get; set; }
        public Nullable<System.DateTime> UseDate { get; set; }
        public YesNoEnum IsUsed { get; set; }
        public Nullable<byte> Status { get; set; }
    
        public virtual CardGroup CardGroup { get; set; }
    }
}
