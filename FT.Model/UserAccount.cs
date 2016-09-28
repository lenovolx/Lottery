

namespace FT.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class UserAccount
    {
        public UserAccount()
        {
            this.MatchUserBet = new HashSet<MatchUserBet>();
            this.UserBankInfo = new HashSet<UserBankInfo>();
            this.CashRecord = new HashSet<CashRecord>();
            this.Admin = new HashSet<Admin>();
            this.TradeRecord = new HashSet<TradeRecord>();
            this.AmountWater = new HashSet<AmountWater>();
        }
    
        public long Id { get; set; }
        public string LoginName { get; set; }
        public string Password { get; set; }
        public string PasswordSalt { get; set; }
        public decimal BalanceAmount { get; set; }
        public LockEnum Status { get; set; }
        public string Phone { get; set; }
        public string email { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public UserTypeEnum Type { get; set; }
        public LevelEnum Level { get; set; }
        public PlatEnum PlatFrom { get; set; }
        public string ParentPath { get; set; }
        public decimal CreditLimit { get; set; }
        public string SafePassword { get; set; }
        public int ReturnRate { get; set; }
    
        public virtual ICollection<MatchUserBet> MatchUserBet { get; set; }
        public virtual ICollection<UserBankInfo> UserBankInfo { get; set; }
        public virtual ICollection<CashRecord> CashRecord { get; set; }
        public virtual ICollection<Admin> Admin { get; set; }
        public virtual ICollection<TradeRecord> TradeRecord { get; set; }
        public virtual ICollection<AmountWater> AmountWater { get; set; }
    }
}
