using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using FT.Model;
using FT.Model.QueryModel;
using FT.Model.ViewModel;

namespace FT.IRepository
{
    public interface IMatchInfoRepository : IBaseRepository<MatchInfo>
    {
       
        /// <summary>
        /// 按联赛分组后赛事数据(pc clent端使用)
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        LeaMatchViewModel GetLeagueMatch(MatchQueryModel query);
      
        /// <summary>
        /// 获取比赛信息（分页）
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        EasyDataGrid<MatchViewModel> GetMatchList(MatchQueryModel query);

        /// <summary>
        /// 获取比赛联赛分组数据
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        LeagueTotal GetMatchLeague(MatchQueryModel query);
        /// <summary>
        /// 获取XBET联赛数据
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        LeagueTotal GetRbMatchLeague(MatchQueryModel query);

        /// <summary>
        /// 比赛结算
        /// </summary>
        /// <param name="matchId"></param>
        /// <param name="settleUserId"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        bool MatchSettlement(long matchId, int settleUserId, int source = 0);

        /// <summary>
        /// 比赛取消
        /// </summary>
        /// <param name="matchIds"></param>
        /// <param name="cancelUserId"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        bool CancelMatch(IEnumerable<long> matchIds, int cancelUserId,int source=0);
        List<MatchViewModel> GetMatchDetail(MatchQueryModel query);

        List<MatchResultViewModel> GetMatchResult(long matchId, int source = 0);
        bool MatchBlackInfo(IEnumerable<long> matchIds, int operateUserId, int datasource = 0);

        /// <summary>
        /// 获取滚球比赛数据
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        LeaMatchViewModel GetRbLeagueMatch(MatchQueryModel query);
    }
}
