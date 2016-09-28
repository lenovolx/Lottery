

namespace FT.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class RolePrivilege
    {
        public long ID { get; set; }
        public Nullable<int> RoleID { get; set; }
        public Nullable<int> Privilege { get; set; }
    }
}
