using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT.Model.QueryModel
{
    public class TradingInfoQueryModel : BaseQueryModel
    {
        public long BetId { get; set; }
        public long AccountID { get; set; }
        public decimal BetValue { get; set; }
        public decimal BetBonus { get; set; }
        public long BetTime { get; set; }
        public int? IsOpen { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
