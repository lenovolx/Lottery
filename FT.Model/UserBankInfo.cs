

namespace FT.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class UserBankInfo
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string RealName { get; set; }
        public string BankType { get; set; }
        public string BankCardNum { get; set; }
        public string BankBranch { get; set; }
    
        public virtual UserAccount UserAccount { get; set; }
    }
}
