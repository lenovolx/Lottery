using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using EntityFramework.Extensions;
using FT.Entities;
using FT.Model;
using FT.IRepository;
using FT.Model.QueryModel;
using FT.Model.ViewModel;
using FT.Utility.Helper;

namespace FT.Repository
{
    public class MatchUserBetRepository : BaseRepository<MatchUserBet>, IMatchUserBetRepository
    {
        public BetEnum UserBet(MatchUserBet bet,double zone=8)
        {
            return QueryDbUseTran((context) =>
            {
                var ident = BetEnum.Success;
                if (bet != null)
                {
                    if (bet.BetValue > 0)
                    {
                        var setting = new SystemSettingRepository().GetSetting();
                        if (bet.BetValue >= setting.MinBetAmount && bet.BetValue <= setting.MaxBetAmount)
                        {
                            if (bet.UserId > 0)
                            {
                                var user = context.UserAccount.FirstOrDefault(s => s.Id == bet.UserId);
                                if (user != null)
                                {
                                    var datenowunix = DateTimeHelper.DateTimeToUnixTimestamp(DateTime.Now, -zone);
                                    var betmatchids = bet.MatchUserBetContent.Select(b => b.MatchID);
                                    if (bet.BetSource == DataSourceEnum.HGA)
                                    {
                                        var match =
                                            context.MatchInfo.Where(
                                                s =>
                                                    betmatchids.Contains(s.MatchId) &&
                                                    s.MatchDate >= datenowunix);
                                        if (match.Any() &&
                                            match.Count() == betmatchids.Count())
                                        {

                                            if (user.BalanceAmount >= bet.BetValue)
                                            {
                                                bet.BetType =
                                                    int.Parse(
                                                        bet.MatchUserBetContent.FirstOrDefault()
                                                            .BetType.ToString()
                                                            .Substring(0, 1));
                                                context.MatchUserBet.Add(bet);
                                                user.BalanceAmount += -bet.BetValue;
                                                //添加金额流水
                                                context.AmountWater.Add(new AmountWater
                                                {
                                                    CreateDate = DateTime.Now,
                                                    UserId = user.Id,
                                                    UserName = user.LoginName,
                                                    Type = TradEnum.Bet,
                                                    Amount = -bet.BetValue,
                                                    Remark = string.Format("投注,注单{0}扣除余额{1}", bet.BetId, bet.BetValue)
                                                });
                                            }
                                            else
                                                ident = BetEnum.BalanceNotEnough;
                                        }
                                        else
                                            ident = BetEnum.BetOverTime; 
                                    }
                                    else
                                    {
                                        var match =
                                            context.XBETMatchInfo.Where(
                                                s =>
                                                    betmatchids.Contains(s.MatchId));
                                        if (match.Any() &&
                                            match.Count() == betmatchids.Count())
                                        {

                                            if (user.BalanceAmount >= bet.BetValue)
                                            {
                                                bet.BetType =
                                                    int.Parse(
                                                        bet.MatchUserBetContent.FirstOrDefault()
                                                            .BetType.ToString()
                                                            .Substring(0, 1));
                                                context.MatchUserBet.Add(bet);
                                                user.BalanceAmount += -bet.BetValue;
                                                //添加金额流水
                                                context.AmountWater.Add(new AmountWater
                                                {
                                                    CreateDate = DateTime.Now,
                                                    UserId = user.Id,
                                                    UserName = user.LoginName,
                                                    Type = TradEnum.Bet,
                                                    Amount = -bet.BetValue,
                                                    Remark = string.Format("投注,注单{0}扣除余额{1}", bet.BetId, bet.BetValue)
                                                });
                                            }
                                            else
                                                ident = BetEnum.BalanceNotEnough;
                                        }
                                        else
                                            ident = BetEnum.BetOverTime; 
                                    }
                                }
                                else
                                    ident = BetEnum.UserNotFind;
                            }
                        }
                        else
                            ident = BetEnum.BetAmountOverRange;
                    }
                    else
                        ident = BetEnum.BetAmountZero;
                }
                else
                    ident = BetEnum.ObjectNull;
                return ident;
            }, BetEnum.Failure);
        }
        public List<TradingInfoViewModel> GetUserBetList(TradingInfoQueryModel query)
        {
            var userBet = new List<TradingInfoViewModel>();
            return QueryDb((context) =>
            {
                Expression<Func<MatchInfo, bool>> predicate = p => p.MatchId > 0;
                if (!string.IsNullOrEmpty(query.AccountID.ToString()))
                    predicate = predicate.And(s => s.LeagueName.Equals(query.AccountID.ToString()));
                userBet =
                    context.MatchUserBet.OrderBy(a => a.CreateDate).Select(s => new TradingInfoViewModel
                        {
                            BetId = s.BetId,
                            UserId = s.UserId,
                            BetValue = s.BetValue,
                            BetBonus = s.BetBonus,
                            BetTime = s.BetTime,
                            CreateDate = s.CreateDate
                        }).ToList();

                return userBet;
            }, userBet);
        }
        public EasyDataGrid<UserBetRecordViewModel> GetUserBetList(UserBetQueryModel query)
        {
            var grid = new EasyDataGrid<UserBetRecordViewModel>();
            return QueryDb((context) =>
            {
                var predicate = PredicateBuilderUtility.True<MatchUserBet>();
                //predicate = predicate.And(s => s.IsSettle == OpenEnum.Settlementing);
                if (query.UserId.HasValue)
                    predicate = predicate.And(s => s.UserId == query.UserId.Value);
                //var startUnix = DateTimeHelper.DateTimeToUnixTimestamp(query.StartDate.Value, 0);
                //var endUnix =
                //    DateTimeHelper.DateTimeToUnixTimestamp(
                //        query.EndDate.Value.AddHours(23).AddMinutes(59).AddSeconds(59), 0);
                //predicate = predicate.And(s => s.BetTime > startUnix && s.BetTime < endUnix);
                if (query.IsSet.HasValue)
                {
                    var setenum = (OpenEnum)query.IsSet.Value;
                    predicate = predicate.And(s => s.IsSettle == setenum);
                }
                grid.rows =
                    context.MatchUserBet.OrderByDescending(s => s.BetTime)
                        .FindBy(predicate, query.Page.Value, query.PageSize.Value, out Total)
                        .ToArray()
                        .Select(s => new UserBetRecordViewModel
                        {
                            BetValue = s.BetValue,
                            BetType = s.BetType,
                            BetBonus = s.BetBonus,
                            Detail = s.MatchUserBetContent.Select(item => new UserBetDetail
                            {
                                BetType = item.BetType,
                                LeagueName =
                                    query.Language == "cn" ? item.MatchInfo.LeagueName : item.MatchInfo.LeagueNameEn,
                                HTeam = query.Language == "cn" ? item.MatchInfo.HTeam : item.MatchInfo.HTeamEn,
                                CTeam = query.Language == "cn" ? item.MatchInfo.CTeam : item.MatchInfo.CTeamEn,
                                BetContent = item.BetContent,
                                BetKey = item.BetKey,
                                MatchId = item.MatchID,
                                Language = query.Language
                            }).ToList(),
                            OrderId = s.BetId,
                            OrderDate = DateTimeHelper.UnixTimestampToDateTime(s.BetTime).ToString("MM-dd"),
                            IsSettle = (int)s.IsSettle
                        }).ToList();
                grid.total = Total;
                return grid;
            }, grid);
        }
        public EasyDataGrid<UserBetRecordViewModel, dynamic> GetUserBetBonusList(UserBetQueryModel query)
        {
            var grid = new EasyDataGrid<UserBetRecordViewModel, dynamic>();
            return QueryDb((context) =>
            {
                var predicate = PredicateBuilderUtility.True<MatchUserBet>();
                if (query.UserId.HasValue)
                    predicate = predicate.And(s => s.UserId == query.UserId.Value);
                if (query.StartDate.HasValue && query.EndDate.HasValue)
                {
                    var startUnix = DateTimeHelper.DateTimeToUnixTimestamp(query.StartDate.Value, 0);
                    var endUnix = DateTimeHelper.DateTimeToUnixTimestamp(query.EndDate.Value.AddDays(1));
                    predicate = predicate.And(s => s.BetTime > startUnix && s.BetTime < endUnix);
                }
                if (query.IsSet.HasValue)
                {
                    var setenum = (OpenEnum) query.IsSet.Value;

                    if (setenum == OpenEnum.Settlementing)
                        predicate = predicate.And(s => s.IsSettle == setenum);
                    else
                        predicate = predicate.And(s => s.IsSettle == OpenEnum.Settlemented || s.IsSettle == OpenEnum.Canceled);
                }
                grid.rows =
                    context.MatchUserBet.OrderByDescending(s => s.BetTime)
                        .FindBy(predicate, query.Page.Value, query.PageSize.Value, out Total)
                        .ToArray()
                        .Select(s => new UserBetRecordViewModel
                        {
                            BetValue = s.BetValue,
                            BetType = s.BetType,
                            BetBonus = s.BetBonus > 0 ? s.BetBonus - s.BetValue : 0,
                            Detail = s.MatchUserBetContent.Select(item => new UserBetDetail
                            {
                                BetType = item.BetType,
                                LeagueName = item.MatchInfo != null
                                    ? (query.Language == "cn" ? item.MatchInfo.LeagueName : item.MatchInfo.LeagueNameEn)
                                    : (query.Language == "cn"
                                        ? context.XBETMatchInfo.FirstOrDefault(r => r.MatchId == item.MatchID)
                                            .LeagueName
                                        : context.XBETMatchInfo.FirstOrDefault(r => r.MatchId == item.MatchID)
                                            .LeagueNameEn),
                                HTeam = item.MatchInfo != null
                                    ? (query.Language == "cn" ? item.MatchInfo.HTeam : item.MatchInfo.HTeamEn)
                                    : (query.Language == "cn"
                                        ? context.XBETMatchInfo.FirstOrDefault(r => r.MatchId == item.MatchID).HTeam
                                        : context.XBETMatchInfo.FirstOrDefault(r => r.MatchId == item.MatchID).HTeamEn),
                                CTeam = item.MatchInfo != null
                                    ? (query.Language == "cn" ? item.MatchInfo.CTeam : item.MatchInfo.CTeamEn)
                                    : (query.Language == "cn"
                                        ? context.XBETMatchInfo.FirstOrDefault(r => r.MatchId == item.MatchID).CTeam
                                        : context.XBETMatchInfo.FirstOrDefault(r => r.MatchId == item.MatchID).CTeamEn),
                                BetContent = item.BetContent,
                                BetKey = item.BetKey,
                                MatchId = item.MatchID,
                                Language = query.Language,
                                BetTypeName = ((PlayEnum) item.BetType).ToDescription(query.Language)
                            }).ToList(),
                            OrderId = s.BetId,
                            OrderDate = DateTimeHelper.UnixTimestampToDateTime(s.BetTime).ToString("MM-dd"),
                            IsSettle = (int) s.IsSettle,
                            BetStatus = s.IsSettle.ToDescription(query.Language)
                        }).ToList();
                grid.total = Total;
                if (query.IsSet == 1)
                    grid.foot = new
                    {
                        SumBetValue = grid.rows.Sum(s => s.BetValue),
                        SumBetBonus = grid.rows.Sum(s => s.BetBonus) - grid.rows.Sum(s => s.BetValue)
                    };
                return grid;
            }, grid);
        }
        private static string Effamount(decimal betValue, decimal betBonus)
        {

            if (betBonus == 0) return "输";
            else if (betBonus > betValue) return "赢";
            else if (betBonus > 0 && betBonus < betValue) return "输部分";
            else if (betValue == betBonus) return "和";
            else return "";
        }
        public EasyDataGrid<MatchBetViewModel> MatchBetGrid(UserBetQueryModel query)
        {
            var grid = new EasyDataGrid<MatchBetViewModel>();
            return QueryDb((context) =>
            {
                var predicate = PredicateBuilderUtility.True<MatchUserBet>();
                if (query.MatchId.HasValue)
                {
                    switch (query.Source)
                    {
                        case 1:
                            predicate =
                                    predicate.And(
                                        s => s.MatchUserBetContent.Select(c => c.MatchID).Contains(query.MatchId.Value));
                            break;
                        default:
                            var matchInfo = context.MatchInfo.FirstOrDefault(s => s.MatchId == query.MatchId);
                            if (matchInfo != null && (matchInfo.EventId == null || matchInfo.EventId == 0))
                                predicate =
                                    predicate.And(
                                        s => s.MatchUserBetContent.Select(c => c.MatchID).Contains(query.MatchId.Value));
                            else
                            {
                                var matchIds =
                                    context.MatchInfo.Where(s => s.EventId == matchInfo.EventId)
                                        .Select(s => s.MatchId)
                                        .ToArray();
                                predicate =
                                    predicate.And(
                                        s => s.MatchUserBetContent.Any(c => matchIds.Contains(c.MatchID)));
                            }
                            break;
                    }
                }
                if (!string.IsNullOrEmpty(query.UserName))
                    predicate = predicate.And(s => s.UserAccount.LoginName.Contains(query.UserName));
                if (query.BetId.HasValue)
                    predicate = predicate.And(s => s.BetId == query.BetId.Value);
                grid.rows =
                    context.MatchUserBet.FindBy(predicate, query.Page.Value, query.PageSize.Value, out Total,
                        query.SortField, query.IsDesc)
                        .ToArray()
                        .Select(s => new MatchBetViewModel
                        {
                            UserName = s.UserAccount.LoginName,
                            BetValue = s.BetValue,
                            BetBonus = s.BetBonus,
                            AchieveAmount =
                                s.MatchUserBetContent.Any()
                                    ? s.MatchUserBetContent.Sum(a => decimal.Parse(a.BetContent.Split('@')[1]))*
                                      s.BetValue - s.BetValue
                                    : 0,
                            OrderId = s.BetId,
                            OrderDate = DateTimeHelper.UnixTimestampToDateTime(s.BetTime).ToString("MM-dd"),
                            BetStatus = s.IsSettle.ToDescription(query.Language)
                        }).ToList();
                grid.total = Total;
                return grid;
            }, grid);
        }
        public EasyDataGrid<UserBetDetail> MatchBetContent(MatchUserBetContentQueryModel query)
        {
            var grid = new EasyDataGrid<UserBetDetail>();
            return QueryDb((context) =>
            {
                var predicate = PredicateBuilderUtility.True<MatchUserBetContent>();
                if (query.BetId.HasValue)
                    predicate = predicate.And(s => s.BetId == query.BetId.Value);
                grid.rows =
                    context.MatchUserBetContent.FindBy(predicate, query.Page.Value, query.PageSize.Value, out Total,
                        query.SortField, query.IsDesc).ToArray()
                        .Select(item => new UserBetDetail
                        {
                            BetType = item.BetType,
                            LeagueName = item.MatchInfo != null
                                    ? (query.Language == "cn" ? item.MatchInfo.LeagueName : item.MatchInfo.LeagueNameEn)
                                    : (query.Language == "cn"
                                        ? context.XBETMatchInfo.FirstOrDefault(r => r.MatchId == item.MatchID).LeagueName
                                        : context.XBETMatchInfo.FirstOrDefault(r => r.MatchId == item.MatchID).LeagueNameEn),
                            HTeam = item.MatchInfo != null
                                    ? (query.Language == "cn" ? item.MatchInfo.HTeam : item.MatchInfo.HTeamEn)
                                    : (query.Language == "cn"
                                        ? context.XBETMatchInfo.FirstOrDefault(r => r.MatchId == item.MatchID).HTeam
                                        : context.XBETMatchInfo.FirstOrDefault(r => r.MatchId == item.MatchID).HTeamEn),
                            CTeam = item.MatchInfo != null
                                    ? (query.Language == "cn" ? item.MatchInfo.CTeam : item.MatchInfo.CTeamEn)
                                    : (query.Language == "cn"
                                        ? context.XBETMatchInfo.FirstOrDefault(r => r.MatchId == item.MatchID).CTeam
                                        : context.XBETMatchInfo.FirstOrDefault(r => r.MatchId == item.MatchID).CTeamEn),
                            BetContent = item.BetContent,
                            BetKey = item.BetKey,
                            MatchId = item.MatchID,
                            Language = query.Language,
                            BetTypeName = ((PlayEnum)item.BetType).ToDescription(query.Language)
                        }).ToList();
                return grid;
            }, grid);
        }
        public dynamic UserBetGet(long uid, long betId)
        {
            dynamic dynObject = new ExpandoObject();
            dynObject.BetBonus = 0;
            dynObject.Flag = PrizeEnum.Failure;
            return QueryDb((context) =>
            {
                var userbet =
                    context.MatchUserBet.FirstOrDefault(s => s.BetId == betId);
                if (userbet == null)
                {
                    dynObject.Flag = PrizeEnum.BetDoesNotExists;
                    return dynObject;
                }
                if (userbet.IsSettle != OpenEnum.Settlemented)
                {
                    dynObject.Flag = PrizeEnum.NoLottery;
                    return dynObject;
                }
                if (userbet.IsGetPrize == YesNoEnum.Yes)
                {
                    dynObject.Flag = PrizeEnum.Prized;
                    return dynObject;
                }
                if (userbet.UserId != uid)
                {
                    dynObject.Flag = PrizeEnum.BetIsNotBelongUser;
                    return dynObject;
                }
                if (userbet.BetBonus == 0)
                {
                    dynObject.Flag = PrizeEnum.NotWinning;
                    return dynObject;
                }
                dynObject.Flag = context.MatchUserBet.Update(w => w.BetId == betId && w.UserId == uid,
                    u => new MatchUserBet
                    {
                        IsGetPrize = YesNoEnum.Yes,
                        GetTime = DateTime.Now
                    }) > 0
                    ? PrizeEnum.Success
                    : PrizeEnum.Failure;
                dynObject.BetBonus = userbet.BetBonus;
                return dynObject;
            }, new
            {
                BetBonus = 0,
                Flag = PrizeEnum.Failure
            });
        }
    }
}
