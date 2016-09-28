

namespace FT.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class UserMessage
    {
        public long Id { get; set; }
        public long MessageId { get; set; }
        public long UserId { get; set; }
        public int IsRead { get; set; }
        public Nullable<System.DateTime> ReadDate { get; set; }
    }
}
