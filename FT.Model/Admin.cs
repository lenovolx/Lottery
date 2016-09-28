

namespace FT.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class Admin
    {
        public Admin()
        {
            this.MatchBlackList = new HashSet<MatchBlackList>();
            this.AdminMessage = new HashSet<AdminMessage>();
        }
    
        public string LoginName { get; set; }
        public string Password { get; set; }
        public string PasswordSalt { get; set; }
        public LockEnum Status { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public int RoleId { get; set; }
        public int Id { get; set; }
        public Nullable<long> AgentId { get; set; }
    
        public virtual Role Role { get; set; }
        public virtual UserAccount UserAccount { get; set; }
        public virtual ICollection<MatchBlackList> MatchBlackList { get; set; }
        public virtual ICollection<AdminMessage> AdminMessage { get; set; }
    }
}
