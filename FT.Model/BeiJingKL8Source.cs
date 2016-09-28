

namespace FT.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class BeiJingKL8Source
    {
        public long Id { get; set; }
        public string TermNumber { get; set; }
        public string LotteryNumber { get; set; }
        public string SortedLotNumber { get; set; }
        public string SingleNumber { get; set; }
        public Nullable<System.DateTime> LotteryTime { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
    }
}
