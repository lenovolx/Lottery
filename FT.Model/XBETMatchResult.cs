

namespace FT.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class XBETMatchResult
    {
        public long Id { get; set; }
        public long MatchId { get; set; }
        public long ResultTime { get; set; }
        public string Odds { get; set; }
        public string Result1 { get; set; }
        public string Result2 { get; set; }
        public string StartOpen { get; set; }
    
        public virtual XBETMatchInfo XBETMatchInfo { get; set; }
    }
}
