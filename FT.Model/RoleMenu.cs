

namespace FT.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class RoleMenu
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public int MenuId { get; set; }
    
        public virtual Role Role { get; set; }
        public virtual Menus Menus { get; set; }
    }
}
