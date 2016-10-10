

namespace FT.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class GameCategory
    {
        public long Id { get; set; }
        public string GameName { get; set; }
        public string GamenCode { get; set; }
        public string DataSource { get; set; }
        public string GameRule { get; set; }
    }
}
