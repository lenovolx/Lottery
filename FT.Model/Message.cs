

namespace FT.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class Message
    {
        public Message()
        {
            this.AdminMessage = new HashSet<AdminMessage>();
        }
    
        public long Id { get; set; }
        public int Type { get; set; }
        public string Title { get; set; }
        public string Annexes { get; set; }
        public string Descriptions { get; set; }
        public System.DateTime CreateDate { get; set; }
        public Nullable<System.DateTime> LastUpdateDate { get; set; }
        public int CreateUser { get; set; }
        public int SendTo { get; set; }
        public YesNoEnum IsDrafts { get; set; }
        public string CreateUserName { get; set; }
    
        public virtual ICollection<AdminMessage> AdminMessage { get; set; }
    }
}
