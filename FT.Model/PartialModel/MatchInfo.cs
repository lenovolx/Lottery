using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FT.Utility.Helper;

namespace FT.Model
{
    public partial class MatchInfo
    {
        /// <summary>
        /// 玩法集合输出
        /// </summary>
        public dynamic OddData
        {
            get
            {
                return XmlHelper.Deserialize(typeof(List<BetInfo>), Odds.Replace("MatchContent", "ArrayOfBetInfo"));
            }
        }
    }
    public class BetInfo
    {
        public string BetType { get; set; }
        public string BetKey { get; set; }
        public string BetIOR { get; set; }
    }
}
