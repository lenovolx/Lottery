using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using FT.Utility.Helper;

namespace FT.Model.ViewModel
{
    /// <summary>
    /// 比赛
    /// </summary>
    public class MatchViewModel
    {
        public MatchViewModel()
        {
            this.Finished = false;
            this.TimeSec = 0;
        }
        public long MatchId { get; set; }
        public string LeagueName { get; set; }
        public string MatchDate { get; set; }
        public int MatchNumber { get; set; }
        public int MatchType { get; set; }
        public long MatchEndTime { get; set; }
        public string HTeamNum { get; set; }
        public string HTeam { get; set; }
        public string CTeamNum { get; set; }
        public string CTeam { get; set; }
        public string BetType { get; set; }
        public DateTime? CreateDate { get; set; }
        public dynamic Odds { get; set; }
        public long OddsUpdateTime { get; set; }
        public long? EventId { get; set; }
        public string MatchDateWeek { get; set; }
        /// <summary>
        /// 是否结算
        /// </summary>
        public string IsSettleName { get; set; }
        /// <summary>
        /// 是否开奖
        /// </summary>
        public string IsOpenName { get; set; }
        /// <summary>
        /// 是否结算
        /// </summary>
        public int IsSettle { get; set; }
        /// <summary>
        /// 是否开奖
        /// </summary>
        public int IsOpen { get; set; }
        /// <summary>
        /// 最小投注(综合过关可用)
        /// </summary>
        public int? MinLimit { get; set; }
        /// <summary>
        /// 最大投注(综合过关可用)
        /// </summary>
        public int? MaxLimit { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool Finished { get; set; }
        public int TimeSec { get; set; }
        /// <summary>
        /// 当前比分
        /// </summary>
        public string CurrentScore { get; set; }

        public string TeamVs { get; set; }
    }

    public class XBETMatchInfoLogViewModel
    {
        public List<XBETMatchInfoLog> Logs { get; set; } 
    }
}
