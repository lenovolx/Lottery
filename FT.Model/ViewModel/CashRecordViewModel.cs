using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT.Model.ViewModel
{
    public class CashRecordViewModel : BaseViewModel
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string BankType { get; set; }
        public string BankCardNum { get; set; }
        public string RealName { get; set; }
        public string BankBranch { get; set; }
        public decimal Amount { get; set; }
        public int StatusNum { get; set; }
        public string Status { get; set; }

        public string RejectRemark { get; set; }
        public DateTime CreateTime { get; set; }

        public string UserCashAccount
        {
            get { return string.Format("{0}({1})", BankType, BankCardNum); }
        }
    }
}
