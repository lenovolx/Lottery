

namespace FT.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class MatchBlackList
    {
        public long Id { get; set; }
        public string LeagueName { get; set; }
        public string LeagueNameEn { get; set; }
        public YesNoEnum IsBan { get; set; }
        public System.DateTime LastOperateDate { get; set; }
        public int OperateUserId { get; set; }
        public DataSourceEnum DataSource { get; set; }
    
        public virtual Admin Admin { get; set; }
    }
}
