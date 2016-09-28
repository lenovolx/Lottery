using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT.Model.ViewModel
{
    /// <summary>
    /// 联赛分组比赛
    /// </summary>
    public class LeagueMatchViewModel : BaseLanguageViewModel
    {
        /// <summary>
        /// 联赛名称
        /// </summary>
        public string League { get; set; }
        /// <summary>
        /// 比赛
        /// </summary>
        public IEnumerable<MatchViewModel> Match { get; set; }
    }
    /// <summary>
    /// 比赛数据
    /// </summary>
    public class LeaMatchViewModel
    {
        public List<LeagueMatchViewModel> Leagues { get; set; }
    }
    /// <summary>
    /// 联赛分组总计
    /// </summary>
    public class LeagueTotal:BaseLanguageViewModel
    {
        public LeagueTotal()
        {
            this.Total = 0;
            this.Source = 0;
            this.NextPage = false;
        }
        public int Total { get; set; }
        public int Source { get; set; }
        public bool NextPage { get; set; }
        public List<GroupLeague> Group { get; set; }
    }

    public class GroupLeague
    {
        public GroupLeague()
        {
            this.Count = 0;
        }
        public string League { get; set; }
        public int Count { get; set; }
    }
}
