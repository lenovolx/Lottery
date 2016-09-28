

namespace FT.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class BeiJingKL8Archive
    {
        public long Id { get; set; }
        public long OriginalId { get; set; }
        public string TermNumber { get; set; }
        public string LotteryNumber { get; set; }
        public string SortedNumber { get; set; }
        public string SingleNumber { get; set; }
        public Nullable<System.DateTime> LotteryTime { get; set; }
        public Nullable<System.DateTime> ArchiveTime { get; set; }
    }
}
