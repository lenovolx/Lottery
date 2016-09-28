using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FT.Model.ViewModel
{
    public class TradeRecordViewModel : BaseViewModel
    {
        public long Id { get; set; }
        public string FromUserName { get; set; }
        public string ToUserName { get; set; }
        public string Type { get; set; }
        public decimal TradeAmount { get; set; }
        public string CardNum { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateUser { get; set; }
    }
}
