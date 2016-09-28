using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FT.IRepository;
using FT.Model;
using FT.Model.ViewModel;

namespace FT.Repository
{
    public class MatchUserBetContentRepository : BaseRepository<MatchUserBetContent>, IMatchUserBetContentRepository
    {
        public List<UserBetDetail> BetDetail(long betid, string language = "cn")
        {
            var list = new List<UserBetDetail>();
            return QueryDb((context) =>
            {
                list = context.MatchUserBetContent.Where(s => s.BetId == betid).Select(item => new UserBetDetail
                {
                    BetType = item.BetType,
                    LeagueName =
                        language == "cn" ? item.MatchInfo.LeagueName : item.MatchInfo.LeagueNameEn,
                    HTeam = language == "cn" ? item.MatchInfo.HTeam : item.MatchInfo.HTeamEn,
                    CTeam = language == "cn" ? item.MatchInfo.CTeam : item.MatchInfo.CTeamEn,
                    BetContent = item.BetContent,
                    BetKey = item.BetKey,
                    MatchId = item.MatchID,
                    Language = language
                }).ToList();
                return list;
            }, list);
        }
    }
}
