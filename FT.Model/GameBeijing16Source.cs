

namespace FT.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class GameBeijing16Source
    {
        public long Id { get; set; }
        public string TermNumber { get; set; }
        public string LotteryNumber { get; set; }
        public string FirstAreaNo { get; set; }
        public string NextAreaNo { get; set; }
        public string LastAreaNo { get; set; }
        public Nullable<int> FirstAreaResult { get; set; }
        public Nullable<int> NextAreaResult { get; set; }
        public Nullable<int> LastAreaResult { get; set; }
        public Nullable<System.DateTime> LotteryTime { get; set; }
        public Nullable<bool> IsSynch { get; set; }
    }
}
