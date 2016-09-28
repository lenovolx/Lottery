using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT.Model.QueryModel
{
    public class MatchQueryModel : BaseQueryModel
    {
        public MatchQueryModel()
        {
            this.LeagueName = string.Empty;
            this.DataType = "xml";
            this.BetType = "1";
            this.DataSource = 0;
        }
        /// <summary>
        /// 联赛名称
        /// </summary>
        public string LeagueName { get; set; }
        public string HTeam { get; set; }
        public long MatchDate { get; set; }
        public string CTeam { get; set; }
        /// <summary>
        /// 是否结算
        /// </summary>
        public int? IsSettle { get; set; }
        /// <summary>
        /// 是否开奖
        /// </summary>
        public int? IsOpen { get; set; }
        /// <summary>
        /// 比赛玩法
        /// </summary>
        public string BetType { get; set; }
        /// <summary>
        /// 玩法节点（mobile端无标准盘概念）
        /// </summary>
        public string OddBetType { get; set; }
        /// <summary>
        /// 数据类型
        /// </summary>
        public string DataType { get; set; }
        /// <summary>
        /// 比赛编号集合
        /// </summary>
        public string MatchIds { get; set; }
        /// <summary>
        /// 赛事数据来源
        /// </summary>
        public int DataSource { get; set; }
        public string Team { get; set; }
    }
}
