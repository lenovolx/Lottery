

namespace FT.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class GameOddsLog
    {
        public long OddsId { get; set; }
        public string GameCategory { get; set; }
        public Nullable<byte> GameNumber { get; set; }
        public Nullable<decimal> Odds { get; set; }
        public Nullable<long> CreateTime { get; set; }
    }
}
