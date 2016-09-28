

namespace FT.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class AdminMessage
    {
        public long Id { get; set; }
        public long MessageId { get; set; }
        public int AdminId { get; set; }
        public YesNoEnum IsRead { get; set; }
        public Nullable<System.DateTime> ReadDate { get; set; }
    
        public virtual Message Message { get; set; }
        public virtual Admin Admin { get; set; }
    }
}
