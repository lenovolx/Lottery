using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FT.Utility.Helper;
using Newtonsoft.Json;

namespace FT.Model
{
    public partial class XBETMatchInfo
    {
        [JsonIgnore]
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
}
