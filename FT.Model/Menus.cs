

namespace FT.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class Menus
    {
        public Menus()
        {
            this.RoleMenu = new HashSet<RoleMenu>();
        }
    
        public int Id { get; set; }
        public int ParentId { get; set; }
        public string Name { get; set; }
        public byte IsLock { get; set; }
        public byte SortNum { get; set; }
        public string Url { get; set; }
        public string ButtonLink { get; set; }
        public MenuEnum MenuType { get; set; }
        public string Icon { get; set; }
        public string NameEn { get; set; }
    
        public virtual ICollection<RoleMenu> RoleMenu { get; set; }
    }
}
