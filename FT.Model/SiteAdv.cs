

namespace FT.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class SiteAdv
    {
        public int Id { get; set; }
        public string AdvImg { get; set; }
        public string AdvTitle { get; set; }
        public PlatEnum Type { get; set; }
        public YesNoEnum IsShow { get; set; }
        public int Sort { get; set; }
    }
}
