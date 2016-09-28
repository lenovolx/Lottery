using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT.Model.ViewModel
{
    public class UserAccountHistoryViewModel
    {
        public UserAccountHistoryViewModel()
        {
            BetAmount = 0;
            EffectiveAmount = 0;
            WinOrLose = 0;
        }

        public string DateTimes { get; set; }
        public string DateWeek { get; set; }
        /// <summary>
        /// 投注金额
        /// </summary>
        public decimal? BetAmount { get; set; }
        /// <summary>
        /// 有效金额
        /// </summary>
        public decimal? EffectiveAmount { get; set; }
        /// <summary>
        /// 输赢(输,金额显示负数)
        /// </summary>
        public decimal? WinOrLose { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class DayYaxis
    {
        public string Dates { get; set; }
        public string Weeks { get; set; }
        public decimal? Amount { get; set; }
    }
    /// <summary>
    /// 底部统计
    /// </summary>
    public class TotalTradViewModel
    {
        public TotalTradViewModel()
        {
            SumBetAmount = 0;
            SumEffectiveAmount = 0;
            SumWinOrLose = 0;
        }
        public decimal? SumBetAmount { get; set; }
        public decimal? SumEffectiveAmount { get; set; }
        public decimal? SumWinOrLose { get; set; }
    }
}
