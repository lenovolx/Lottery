

namespace FT.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class Role
    {
        public Role()
        {
            this.RoleMenu = new HashSet<RoleMenu>();
            this.Admin = new HashSet<Admin>();
        }
    
        public string RoleName { get; set; }
        public LockEnum IsLock { get; set; }
        public int Id { get; set; }
        public string RoleNameEn { get; set; }
    
        public virtual ICollection<RoleMenu> RoleMenu { get; set; }
        public virtual ICollection<Admin> Admin { get; set; }
    }
}
