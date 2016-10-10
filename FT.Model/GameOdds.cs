

namespace FT.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class GameOdds
    {
        public long Id { get; set; }
        public string GameCategory { get; set; }
        public Nullable<byte> GameNumber { get; set; }
        public Nullable<decimal> CurrentOdds { get; set; }
        public Nullable<decimal> OldOdds { get; set; }
        public Nullable<decimal> StandardOdds { get; set; }
        public Nullable<long> UpdateTime { get; set; }
    }
}
