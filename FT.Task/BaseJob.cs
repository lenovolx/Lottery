using FT.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT.Task
{
    public class BaseJob : FtContext
    {
        /// <summary>
        /// Job数据抓取地址
        /// </summary>
        public const string ApiUrl = "";
        /// <summary>
        /// 1xbet滚球数据接口
        /// </summary>
        public const string XBETUrl =
            "https://1x-bet-bk.com/LiveFeed/BestGamesExt?sportId=&sports=1&champs=&tf=1000000&tz=0&afterDays=0&count=100&cnt=100&lng=cn&cfview=4&page=0&antisports=&mode=4&subGames=&cyberFlag=0&country=90&partner=1";
        /// <summary>
        /// 1xbet滚球比赛结果接口
        /// </summary>
        public const string XBETResultUrl = "https://1x-bet-bk.com/getTranslate/ViewGameResults";
    }
}
