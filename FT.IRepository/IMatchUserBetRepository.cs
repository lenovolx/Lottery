using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FT.Model;
using FT.Model.ViewModel;
using FT.Model.QueryModel;

namespace FT.IRepository
{
    public interface IMatchUserBetRepository : IBaseRepository<MatchUserBet>
    {
        BetEnum UserBet(MatchUserBet bet, double zone = 8);
        List<TradingInfoViewModel> GetUserBetList(TradingInfoQueryModel query);
        EasyDataGrid<UserBetRecordViewModel> GetUserBetList(UserBetQueryModel query);
        EasyDataGrid<UserBetRecordViewModel, dynamic> GetUserBetBonusList(UserBetQueryModel query);
        EasyDataGrid<MatchBetViewModel> MatchBetGrid(UserBetQueryModel query);
        EasyDataGrid<UserBetDetail> MatchBetContent(MatchUserBetContentQueryModel query);
        dynamic UserBetGet(long uid, long betId);
    }
}