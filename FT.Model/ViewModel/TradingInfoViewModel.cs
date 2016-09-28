using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT.Model.ViewModel
{
    /// <summary>
    /// 交易信息
    /// </summary>
    public class TradingInfoViewModel : BaseViewModel
    {
        public long BetId { get; set; }
        public long UserId { get; set; }
        public decimal  BetValue { get; set; }
        public decimal  BetBonus { get; set; }
        public long BetTime { get; set; }
        public string IsOpen { get; set; }
        public DateTime CreateDate { get; set; }
    }
}