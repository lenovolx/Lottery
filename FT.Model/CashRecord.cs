

namespace FT.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class CashRecord
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string AuditorName { get; set; }
        public string BankType { get; set; }
        public string BankCardNum { get; set; }
        public string RealName { get; set; }
        public string BankBranch { get; set; }
        public CashAuditEnum Status { get; set; }
        public string RejectRemark { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public System.DateTime CreateTime { get; set; }
        public Nullable<System.DateTime> AuditTime { get; set; }
    
        public virtual UserAccount UserAccount { get; set; }
    }
}
