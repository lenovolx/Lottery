using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using FT.Entities;
using FT.Model;
using FT.IRepository;
using FT.Model.QueryModel;
using FT.Model.ViewModel;
using FT.Utility.Helper;
using EntityFramework.Extensions;
using FT.Plugin.Cache;

namespace FT.Repository
{
    public class MatchInfoRepository : BaseRepository<MatchInfo>, IMatchInfoRepository
    {
        #region WebAPI HGA
        /// <summary>
        /// 按联赛分组后赛事数据
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public LeaMatchViewModel GetLeagueMatch(MatchQueryModel query)
        {
            var match = new LeaMatchViewModel();
            return QueryDb((context) =>
            {
                var predicate = PredicateBuilderUtility.True<MatchInfo>();
                var datenowunix = DateTimeHelper.DateTimeToUnixTimestamp(DateTime.Now, -query.TimeZone.Value);
                predicate = predicate.And(s => s.MatchDate >= datenowunix);
                if (!string.IsNullOrEmpty(query.LeagueName))
                {
                    var league = HttpUtility.UrlDecode(query.LeagueName, Encoding.UTF8);
                    predicate = query.Language == "cn"
                        ? predicate.And(s => s.LeagueName.Equals(league))
                        : predicate.And(s => s.LeagueNameEn.Equals(league));
                }
                if (!string.IsNullOrEmpty(query.BetType) && !query.BetType.Equals("99"))
                    predicate = predicate.And(s => ("|" + s.BetType + "|").Contains("|" + query.BetType + "|"));
                if (!string.IsNullOrEmpty(query.OddBetType) && !query.OddBetType.Equals("99"))
                {
                    var oddbettype = string.Format("<BetType>{0}</BetType>", query.OddBetType);
                    predicate =
                        predicate.And(
                            s =>
                                query.OddBetType != "5"
                                    ? s.Odds.Contains(oddbettype)
                                    : s.Odds.Contains("<BetType>50</BetType>") ||
                                      s.Odds.Contains("<BetType>52</BetType>"));
                }
                Expression<Func<MatchInfo, string>> keygroup = s => s.LeagueName;
                if (query.Language != "cn")
                    keygroup = s => s.LeagueNameEn;
                var group = context.MatchInfo.Where(predicate).GroupBy(keygroup, (i, v) => new
                {
                    League = i,
                    Match = v
                }).ToList();
                if (query.DataType.Equals("xml"))
                    #region Odds返回xml格式

                    match.Leagues = group.Select(v => new LeagueMatchViewModel
                    {
                        Language = query.Language,
                        League = v.League,
                        Match = v.Match.Select(s => new MatchViewModel
                        {
                            MatchId = s.MatchId,
                            LeagueName = query.Language == "cn" ? s.LeagueName : s.LeagueNameEn,
                            MatchDate =
                                DateTimeHelper.UnixTimestampToDateTime(s.MatchDate).AddHours(query.TimeZone.Value).ToString("yyyy-MM-dd HH:mm"),
                            MatchNumber = s.MatchNumber,
                            MatchType = s.MatchType,
                            MatchEndTime = s.MatchEndTime,
                            HTeamNum = s.HTeamNum,
                            HTeam = query.Language == "cn" ? s.HTeam : s.HTeamEn,
                            CTeamNum = s.CTeamNum,
                            CTeam = query.Language == "cn" ? s.CTeam : s.CTeamEn,
                            BetType = s.BetType,
                            Odds = s.Odds,
                            CreateDate = s.CreateDate,
                            OddsUpdateTime = s.OddsUpdateTime,
                            EventId = s.EventId,
                            MatchDateWeek = DateTimeHelper.UnixTimestampToDateTime(s.MatchDate).AddHours(query.TimeZone.Value).ToString("MM.dd"),//DateTimeHelper.GetDayOfWeekCN(DateTimeHelper.UnixTimestampToDateTime(s.MatchDate).AddHours(query.TimeZone.Value)),
                            MinLimit = s.MinLimit ?? 0,
                            MaxLimit = s.MaxLimit ?? 0
                        })
                    }).ToList();

                    #endregion
                else
                    match.Leagues = group.Select(v => new LeagueMatchViewModel
                    {
                        Language = query.Language,
                        League = v.League,
                        Match = v.Match.Select(s => new MatchViewModel
                        {
                            MatchId = s.MatchId,
                            LeagueName = query.Language == "cn" ? s.LeagueName : s.LeagueNameEn,
                            MatchDate =
                                DateTimeHelper.UnixTimestampToDateTime(s.MatchDate).AddHours(query.TimeZone.Value).ToString("HH:mm"),
                            MatchNumber = s.MatchNumber,
                            MatchType = s.MatchType,
                            MatchEndTime = s.MatchEndTime,
                            HTeamNum = s.HTeamNum,
                            HTeam = query.Language == "cn" ? s.HTeam : s.HTeamEn,
                            CTeamNum = s.CTeamNum,
                            CTeam = query.Language == "cn" ? s.CTeam : s.CTeamEn,
                            BetType = s.BetType,
                            Odds = s.OddData,
                            CreateDate = s.CreateDate,
                            OddsUpdateTime = s.OddsUpdateTime,
                            EventId = s.EventId,
                            MatchDateWeek = DateTimeHelper.UnixTimestampToDateTime(s.MatchDate).AddHours(query.TimeZone.Value).ToString("MM.dd"),//DateTimeHelper.GetDayOfWeekCN(DateTimeHelper.UnixTimestampToDateTime(s.MatchDate).AddHours(query.TimeZone.Value)),
                            MinLimit = s.MinLimit ?? 0,
                            MaxLimit = s.MaxLimit ?? 0
                        })
                    }).ToList();
                return match;
            }, match);
        }
        /// <summary>
        /// 获取皇冠联赛信息
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public LeagueTotal GetMatchLeague(MatchQueryModel query)
        {
            var leagueT = new LeagueTotal();
            var maxPage = 0;
            return QueryDb((context) =>
            {
                var predicate = PredicateBuilderUtility.True<MatchInfo>();
                predicate = predicate.And(s => s.IsSettle == OpenEnum.Settlementing);
                var datenowunix = DateTimeHelper.DateTimeToUnixTimestamp(DateTime.Now, -query.TimeZone.Value);
                predicate = predicate.And(s => s.MatchDate >= datenowunix);
                Expression<Func<MatchInfo, string>> keygroup = s => s.LeagueName;
                if (query.Language != "cn")
                    keygroup = s => s.LeagueNameEn;
                var league = context.MatchInfo.Where(predicate).GroupBy(keygroup, (i, v) => new GroupLeague
                {
                    League = i,
                    Count = v.Count()
                }).ToList();
                leagueT.Group =
                    league.OrderByDescending(s => s.Count)
                    //.Skip((query.Page.Value - 1) * query.PageSize.Value)
                    //.Take(query.PageSize.Value)
                        .ToList();

                leagueT.Total = league.Sum(s => s.Count);
                maxPage = league.Count / query.PageSize.Value;
                if (league.Count % query.PageSize.Value != 0)
                    maxPage += 1;
                leagueT.NextPage = maxPage > query.Page.Value;
                leagueT.Language = query.Language;
                leagueT.Source = query.DataSource;
                return leagueT;
            }, leagueT);
        }
        #endregion

        #region WebAPI XBET
        /// <summary>
        /// 联赛
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public LeagueTotal GetRbMatchLeague(MatchQueryModel query)
        {
            var leagueT = new LeagueTotal();
            var maxPage = 0;
            var xbMatch = Cache.Get<List<XBETMatchInfo>>(CacheKeyCollection.RollBallCurrent);
            Expression<Func<XBETMatchInfo, string>> keygrouprb = s => s.LeagueName;
            if (query.Language != "cn")
                keygrouprb = s => s.LeagueNameEn;
            if (xbMatch == null || !xbMatch.Any())
            {
                return QueryDb((context) =>
                {
                    var predicateRb = PredicateBuilderUtility.True<XBETMatchInfo>();
                    //var datenowunixrb = DateTimeHelper.DateTimeToUnixTimestamp(DateTime.Now, -query.TimeZone.Value);
                    //predicateRb = predicateRb.And(s => s.MatchDate <= datenowunixrb);
                    predicateRb =
                        predicateRb.And(
                            s =>
                                s.IsSettle == OpenEnum.Settlementing && !s.Finished && s.TimeRun &&
                                !string.IsNullOrEmpty(s.Odds));

                    var leagueRb =
                        context.XBETMatchInfo.Where(predicateRb).GroupBy(keygrouprb, (i, v) => new GroupLeague
                        {
                            League = i,
                            Count = v.Count()
                        }).ToList();
                    leagueT.Group = leagueRb.OrderByDescending(s => s.Count)
                        //.Skip((query.Page.Value - 1) * query.PageSize.Value).Take(query.PageSize.Value)
                        .ToList();
                    leagueT.Total = leagueRb.Sum(s => s.Count);
                    maxPage = leagueRb.Count / query.PageSize.Value;
                    if (leagueRb.Count % query.PageSize.Value != 0)
                        maxPage += 1;
                    leagueT.NextPage = maxPage > query.Page.Value;
                    leagueT.Language = query.Language;
                    leagueT.Source = query.DataSource;
                    return leagueT;
                }, leagueT);
            }
            else
            {
                var leagueRb = xbMatch.AsQueryable().Where(s => !string.IsNullOrEmpty(s.Odds)).GroupBy(keygrouprb, (i, v) => new GroupLeague
                {
                    League = i,
                    Count = v.Count()
                }).ToList();
                leagueT.Group = leagueRb.OrderByDescending(s => s.Count)
                    //.Skip((query.Page.Value - 1) * query.PageSize.Value).Take(query.PageSize.Value)
                        .ToList();
                leagueT.Total = leagueRb.Sum(s => s.Count);
                maxPage = leagueRb.Count / query.PageSize.Value;
                if (leagueRb.Count % query.PageSize.Value != 0)
                    maxPage += 1;
                leagueT.NextPage = maxPage > query.Page.Value;
                leagueT.Language = query.Language;
                leagueT.Source = query.DataSource;
                return leagueT;
            }
        }

        /// <summary>
        /// 联赛比赛
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public LeaMatchViewModel GetRbLeagueMatch(MatchQueryModel query)
        {
            var match = new LeaMatchViewModel();
            var xbMatch = Cache.Get<List<XBETMatchInfo>>(CacheKeyCollection.RollBallCurrent);
            var predicate = PredicateBuilderUtility.True<XBETMatchInfo>();
            predicate =
                predicate.And(
                    s =>
                        s.IsSettle == OpenEnum.Settlementing && !s.Finished && s.TimeRun &&
                        !string.IsNullOrEmpty(s.Odds));
            if (!string.IsNullOrEmpty(query.LeagueName))
            {
                var league = HttpUtility.UrlDecode(query.LeagueName, Encoding.UTF8);
                predicate = query.Language == "cn"
                    ? predicate.And(s => s.LeagueName.Equals(league))
                    : predicate.And(s => s.LeagueNameEn.Equals(league));
            }
            if (!string.IsNullOrEmpty(query.OddBetType) && !query.OddBetType.Equals("99"))
            {
                var oddbettype = string.Format("<BetType>{0}</BetType>", query.OddBetType);
                predicate =
                    predicate.And(
                        s => s.Odds.Contains(oddbettype));
            }
            Expression<Func<XBETMatchInfo, string>> keygroup = s => s.LeagueName;
            if (query.Language != "cn")
                keygroup = s => s.LeagueNameEn;

            #region DataBase Data
            if (xbMatch == null || !xbMatch.Any())
            {
                return QueryDb((context) =>
                {
                    //var datenowunixrb = DateTimeHelper.DateTimeToUnixTimestamp(DateTime.Now, -query.TimeZone.Value);
                    //predicate = predicate.And(s => s.MatchDate <= datenowunixrb);
                    var group = context.XBETMatchInfo.Where(predicate).GroupBy(keygroup, (i, v) => new
                    {
                        League = i,
                        Match = v
                    }).ToList();
                    match.Leagues = group.Select(v => new LeagueMatchViewModel
                    {
                        Language = query.Language,
                        League = v.League,
                        Match = v.Match.Select(s => new MatchViewModel
                        {
                            TimeSec = s.TimeSec,
                            MatchId = s.MatchId,
                            LeagueName = query.Language == "cn" ? s.LeagueName : s.LeagueNameEn,
                            MatchDate = DateTimeHelper.SecondsToMinutes(s.TimeSec, ":"),
                            MatchNumber = s.MatchNumber,
                            MatchType = s.MatchType,
                            MatchEndTime = s.MatchEndTime,
                            HTeamNum = s.HTeamNum,
                            HTeam = query.Language == "cn" ? s.HTeam : s.HTeamEn,
                            CTeamNum = s.CTeamNum,
                            CTeam = query.Language == "cn" ? s.CTeam : s.CTeamEn,
                            BetType = s.BetType,
                            Odds = s.OddData,
                            CreateDate = s.CreateDate,
                            OddsUpdateTime = s.OddsUpdateTime,
                            EventId = s.EventId,
                            MinLimit = s.MinLimit ?? 0,
                            MaxLimit = s.MaxLimit ?? 0,
                            CurrentScore = s.CurrentScore
                        })
                    }).ToList();
                    return match;
                }, match);
            }
            #endregion

            #region Redis Data
            else
            {
                var group = xbMatch.AsQueryable().Where(predicate).GroupBy(keygroup, (i, v) => new
                {
                    League = i,
                    Match = v
                }).ToList();
                match.Leagues = group.Select(v => new LeagueMatchViewModel
                {
                    Language = query.Language,
                    League = v.League,
                    Match = v.Match.Select(s => new MatchViewModel
                    {
                        TimeSec = s.TimeSec,
                        MatchId = s.MatchId,
                        LeagueName = query.Language == "cn" ? s.LeagueName : s.LeagueNameEn,
                        MatchDate = DateTimeHelper.SecondsToMinutes(s.TimeSec, ":"),
                        MatchNumber = s.MatchNumber,
                        MatchType = s.MatchType,
                        MatchEndTime = s.MatchEndTime,
                        HTeamNum = s.HTeamNum,
                        HTeam = query.Language == "cn" ? s.HTeam : s.HTeamEn,
                        CTeamNum = s.CTeamNum,
                        CTeam = query.Language == "cn" ? s.CTeam : s.CTeamEn,
                        BetType = s.BetType,
                        Odds = s.OddData,
                        CreateDate = s.CreateDate,
                        OddsUpdateTime = s.OddsUpdateTime,
                        EventId = s.EventId,
                        MinLimit = s.MinLimit ?? 0,
                        MaxLimit = s.MaxLimit ?? 0,
                        CurrentScore = s.CurrentScore
                    })
                }).ToList();
                return match;
            }
            #endregion
        }

        #endregion

        public EasyDataGrid<MatchViewModel> GetMatchList(MatchQueryModel query)
        {
            var grid = new EasyDataGrid<MatchViewModel>();
            return QueryDb((context) =>
            {
                if (query.DataSource == 0)
                {
                    var predicate = PredicateBuilderUtility.True<MatchInfo>();
                    predicate = predicate.And(s => s.IsMaster == 1);
                    switch (query.Language)
                    {
                        case "en":
                        case "pt":
                            if (!string.IsNullOrEmpty(query.LeagueName))
                                predicate = predicate.And(p => p.LeagueNameEn.Contains(query.LeagueName));
                            if (!string.IsNullOrEmpty(query.Team))
                                predicate = predicate.And(p => p.HTeamEn.Contains(query.Team) || p.CTeamEn.Contains(query.Team));
                            break;
                        default:
                            if (!string.IsNullOrEmpty(query.LeagueName))
                                predicate = predicate.And(p => p.LeagueName.Contains(query.LeagueName));
                            if (!string.IsNullOrEmpty(query.Team))
                                predicate = predicate.And(p => p.HTeam.Contains(query.Team) || p.CTeam.Contains(query.Team));
                            break;
                    }
                    if (query.IsSettle.HasValue)
                        predicate = predicate.And(p => p.IsSettle == (OpenEnum)query.IsSettle);
                    if (query.StartDate.HasValue && query.EndDate.HasValue)
                    {
                        var stimeunix = DateTimeHelper.DateTimeToUnixTimestamp(query.StartDate.Value, 0 - TimeZonePlat);
                        var etimeunix = DateTimeHelper.DateTimeToUnixTimestamp(query.EndDate.Value, 0 - TimeZonePlat);
                        predicate = predicate.And(s => s.MatchDate >= stimeunix && s.MatchDate < etimeunix);
                    }
                    if (query.IsOpen.HasValue)
                    {
                        if (query.IsOpen.Value == 1)
                            predicate = predicate.And(p => p.MatchResult.Select(s => s.MatchId).Contains(p.MatchId));
                        else
                            predicate = predicate.And(p => !p.MatchResult.Select(s => s.MatchId).Contains(p.MatchId));
                    }
                    grid.rows =
                        context.MatchInfo.FindBy(predicate, query.Page.Value, query.PageSize.Value, out Total,
                            query.SortField, query.IsDesc).ToArray()
                            .Select(s => new MatchViewModel
                            {
                                MatchId = s.MatchId,
                                LeagueName = query.Language.Equals("cn") ? s.LeagueName : s.LeagueNameEn,
                                MatchDate =
                                    DateTimeHelper.UnixTimestampToDateTime(s.MatchDate)
                                        .AddHours(TimeZonePlat)
                                        .ToString("yyyy-MM-dd HH:mm"),
                                MatchNumber = s.MatchNumber,
                                MatchType = s.MatchType,
                                HTeamNum = s.HTeamNum,
                                HTeam = query.Language.Equals("cn") ? s.HTeam : s.HTeamEn,
                                CTeamNum = s.CTeamNum,
                                CTeam = query.Language.Equals("cn") ? s.CTeam : s.CTeamEn,
                                BetType = s.BetType,
                                CreateDate = s.CreateDate,
                                IsSettleName = s.IsSettle.ToDescription(query.Language),
                                IsOpenName = ((YesNoEnum)(s.MatchResult.Any() ? 1 : 0)).ToDescription(query.Language),
                                IsSettle = (int)s.IsSettle,
                                IsOpen = (int)s.MatchResult.Count,
                                UpdateDate = s.UpdateDate,
                                TeamVs =
                                    string.Format("{0} VS {1}", query.Language.Equals("cn") ? s.HTeam : s.HTeamEn,
                                        query.Language.Equals("cn") ? s.CTeam : s.CTeamEn)
                            }).ToList();
                }
                else
                {
                    var predicate = PredicateBuilderUtility.True<XBETMatchInfo>();
                    predicate = predicate.And(s => s.IsMaster == 1);
                    if (!string.IsNullOrEmpty(query.LeagueName))
                        predicate = predicate.And(p => p.LeagueName.Contains(query.LeagueName));
                    if (!string.IsNullOrEmpty(query.HTeam))
                        predicate = predicate.And(p => p.HTeam.Contains(query.HTeam));
                    if (!string.IsNullOrEmpty(query.CTeam))
                        predicate = predicate.And(p => p.CTeam.Contains(query.CTeam));
                    if (query.IsSettle.HasValue)
                        predicate = predicate.And(p => p.IsSettle == (OpenEnum)query.IsSettle);
                    if (query.StartDate.HasValue && query.EndDate.HasValue)
                    {
                        var stimeunix = DateTimeHelper.DateTimeToUnixTimestamp(query.StartDate.Value, 0 - TimeZonePlat);
                        var etimeunix = DateTimeHelper.DateTimeToUnixTimestamp(query.EndDate.Value, 0 - TimeZonePlat);
                        predicate = predicate.And(s => s.MatchDate >= stimeunix && s.MatchDate < etimeunix);
                    }
                    if (query.IsOpen.HasValue)
                    {
                        if (query.IsOpen.Value == 1)
                            predicate = predicate.And(p => p.XBETMatchResult.Select(s => s.MatchId).Contains(p.MatchId));
                        else
                            predicate = predicate.And(p => !p.XBETMatchResult.Select(s => s.MatchId).Contains(p.MatchId));
                    }
                    grid.rows =
                        context.XBETMatchInfo.FindBy(predicate, query.Page.Value, query.PageSize.Value, out Total,
                            query.SortField, query.IsDesc).ToArray()
                            .Select(s => new MatchViewModel
                            {
                                MatchId = s.MatchId,
                                LeagueName = query.Language.Equals("cn") ? s.LeagueName : s.LeagueNameEn,
                                MatchDate =
                                    DateTimeHelper.UnixTimestampToDateTime(s.MatchDate).AddHours(TimeZonePlat)
                                        .ToString("yyyy-MM-dd HH:mm"),
                                MatchNumber = s.MatchNumber,
                                MatchType = s.MatchType,
                                HTeamNum = s.HTeamNum,
                                HTeam = query.Language.Equals("cn") ? s.HTeam : s.HTeamEn,
                                CTeamNum = s.CTeamNum,
                                CTeam = query.Language.Equals("cn") ? s.CTeam : s.CTeamEn,
                                BetType = s.BetType,
                                CreateDate = s.CreateDate,
                                IsSettleName = s.IsSettle.ToDescription(query.Language),
                                IsOpenName =
                                    ((YesNoEnum)(s.XBETMatchResult.Any() ? 1 : 0)).ToDescription(query.Language),
                                IsSettle = (int)s.IsSettle,
                                IsOpen = (int)s.XBETMatchResult.Count,
                                UpdateDate = s.UpdateDate,
                                TimeSec = s.TimeSec,
                                Finished = s.Finished,
                                TeamVs =
                                    string.Format("{0} VS {1}", query.Language.Equals("cn") ? s.HTeam : s.HTeamEn,
                                        query.Language.Equals("cn") ? s.CTeam : s.CTeamEn)
                            }).ToList();
                }
                grid.total = Total;
                return grid;
            }, grid);
        }
        /// <summary>
        /// 比赛结算
        /// </summary>
        /// <returns></returns>
        public bool MatchSettlement(long matchId, int settleUserId, int source = 0)
        {
            var sb = new StringBuilder();
            try
            {
                var listBet = new List<MatchUserBet>();
                var listBetUpdate = new List<MatchUserBet>();
                var listBetContentUpdate = new List<MatchUserBetContent>();
                var userAmount = new List<UserBetAmountModel>();

                switch (source)
                {
                    case 1:
                        #region 滚球
                        var siteSetting = new SystemSettingRepository().GetSetting();
                        var listMatchRb = new List<XBETMatchInfo>();
                        if (!QueryDb((context) =>
                        {
                            listMatchRb = context.XBETMatchInfo.Include("XBETMatchResult").Where(c => c.MatchId == matchId).ToList();
                            return listMatchRb.Any();
                        }, false))
                        {
                            return false;
                        }
                        foreach (var item in listMatchRb.Where(item => item.IsSettle == OpenEnum.Settlementing))
                        {
                            var matchResult = GetResult(null, item.XBETMatchResult.FirstOrDefault());
                            var tempBet = new List<MatchUserBet>();
                            QueryDb((context) =>
                            {
                                tempBet = context.MatchUserBet.Include("MatchUserBetContent").Include("UserAccount")
                                    .Where(s => s.MatchUserBetContent.Select(c => c.MatchID).Contains(item.MatchId))
                                    .ToList();
                            });
                            if (tempBet.Any())
                            {
                                listBet.AddRange(tempBet);
                                foreach (var userbet in tempBet)
                                {
                                    var userbetcontent =
                                        userbet.MatchUserBetContent.FirstOrDefault(c => c.MatchID == item.MatchId);
                                    if (
                                        !listBetContentUpdate.Any(
                                            c =>
                                                userbetcontent != null &&
                                                (c.MatchID == userbetcontent.MatchID && c.BetId == userbetcontent.BetId &&
                                                 c.BetType == userbetcontent.BetType)))
                                    {
                                        if (userbetcontent != null)
                                        {
                                            userbetcontent.IsSettle = OpenEnum.Settlemented;
                                            userbetcontent.SettleIor = GetIor(matchResult, userbetcontent);
                                            listBetContentUpdate.Add(userbetcontent);
                                        }
                                    }
                                    //如果是综合过关，必须是全部结算完了，否则跳过
                                    if ((userbet.BetType != 5 ||
                                         userbet.MatchUserBetContent.Count(c => c.IsSettle == OpenEnum.Settlementing) != 0) &&
                                        userbet.BetType == 5) continue;
                                    {
                                        userbet.IsSettle = OpenEnum.Settlemented;
                                        userbet.SettleTime = DateTime.Now;
                                        userbet.BetBonus =
                                            userbet.MatchUserBetContent.Select(
                                                c => c.SettleIor ?? 0).Aggregate((i, j) => i * j) *
                                            userbet.BetValue;
                                        listBetUpdate.Add(userbet);
                                    }
                                }
                            }
                            //执行数据库操作
                            sb.AppendFormat(
                                "UPDATE XBETMatchInfo SET IsSettle={0},SettleTime='{1}',SettleUserId={2} WHERE MatchId={3};",
                                1, DateTime.Now, settleUserId, item.MatchId);
                        }
                        foreach (var item in listBetUpdate)
                        {
                            sb.AppendFormat("UPDATE MatchUserBet SET IsSettle={0},SettleTime='{1}',BetBonus={2} WHERE BetId={3};", 1, item.SettleTime, item.BetBonus, item.BetId);
                            userAmount.Add(new UserBetAmountModel
                            {
                                BetId = item.BetId,
                                UserId = item.UserId,
                                UserName = item.UserAccount.LoginName,
                                Amount = (item.BetBonus - item.BetValue) > 0 ? item.BetBonus - item.BetValue : 0
                            });
                        }
                        foreach (var item in listBetContentUpdate)
                        {
                            sb.AppendFormat("UPDATE MatchUserBetContent SET IsSettle={0},SettleIor={1} WHERE Id={2};",
                                1, item.SettleIor, item.Id);
                        }
                        //用户余额修改，资金流水明细记录
                        if (userAmount.Any())
                        {
                            foreach (var item in userAmount.Where(item => item.Amount > 0))
                            {
                                sb.AppendFormat(
                                    "INSERT INTO AmountWater(UserId,UserName,Amount,Type,Remark,CreateDate) VALUES({0},'{1}',{2},{3},'{4}','{5}');",
                                    item.UserId, item.UserName, item.Amount, (int)TradEnum.MatchLottery,
                                    string.Format("比赛结算 注单{0}中奖 金额{1} 返还奖额至余额", item.BetId, item.Amount), DateTime.Now);
                            }
                            var group = userAmount.Where(a => a.Amount > 0).GroupBy(s => s.UserId, (i, v) => new UserBetAmountGroupModel
                            {
                                UserId = i,
                                Group = v.ToList()
                            }).ToArray();
                            foreach (var items in group)
                            {
                                sb.AppendFormat("UPDATE UserAccount SET BalanceAmount=BalanceAmount+{0} WHERE Id={1};",
                                    items.Group.Sum(s => s.Amount.Value), items.UserId);
                            }
                        }
                        #endregion

                        break;
                    default:
                        #region 标准玩法
                        var listMatch = new List<MatchInfo>();
                        #region  查询本场次相关的所有比赛
                        if (!QueryDb((context) =>
                        {
                            //先查当前场次，如果同名比赛有多场的，则全部取出来，一起结算
                            listMatch = context.MatchInfo.Include("MatchResult").Where(c => c.MatchId == matchId).ToList();
                            if (listMatch.Count > 0 && listMatch[0].EventId > 0)
                            {
                                var eventId = listMatch[0].EventId;
                                if (eventId != null)
                                {
                                    var eventid = eventId.Value;
                                    listMatch = context.MatchInfo.Include("MatchResult").Where(c => c.EventId == eventid).ToList();
                                }
                            }
                            return true;
                        }, false))
                        {
                            return false;
                        }
                        #endregion
                        var match = listMatch.FirstOrDefault(c => c.IsSettle == OpenEnum.Settlementing && c.MatchResult.FirstOrDefault() != null && c.MatchResult.FirstOrDefault().Result1 != null);

                        //如果本场比赛没结果，或者没有投注，则直接标记为已结算
                        if (match == null || match.MatchResult.FirstOrDefault() == null)
                            return false;
                        var mr = match.MatchResult.FirstOrDefault();
                        if (mr == null)
                            return false;
                        var issettle = OpenEnum.Settlemented;
                        if (mr.Result1.Contains("取消"))
                        {
                            issettle = OpenEnum.Canceled;
                        }
                        //结算单场比赛
                        var dicResult = GetResult(mr);

                        foreach (var mc in listMatch)
                        {
                            if (mc.IsSettle > 0)
                            {
                                return true;
                            }
                            var mc1 = mc;
                            QueryDb((context) =>
                            {
                                listBet =
                                    context.MatchUserBet.Include("MatchUserBetContent").Include("UserAccount")
                                        .Where(s => s.MatchUserBetContent.Select(c => c.MatchID).Contains(mc1.MatchId))
                                        .ToList();

                            });
                            //如果没有投注，就直接更改结算标示
                            if (listBet != null && listBet.Count > 0)
                            {
                                foreach (var userbet in listBet)
                                {
                                    var userbetcontent =
                                        userbet.MatchUserBetContent.FirstOrDefault(c => c.MatchID == mc1.MatchId);
                                    if (
                                        !listBetContentUpdate.Any(
                                            c =>
                                                userbetcontent != null &&
                                                (c.MatchID == userbetcontent.MatchID && c.BetId == userbetcontent.BetId &&
                                                 c.BetType == userbetcontent.BetType)))
                                    {
                                        if (userbetcontent != null)
                                        {
                                            userbetcontent.IsSettle = issettle;
                                            userbetcontent.SettleIor = GetIor(dicResult, userbetcontent);
                                            listBetContentUpdate.Add(userbetcontent);
                                        }
                                    }
                                    //如果是综合过关，必须是全部结算完了，否则跳过
                                    if ((userbet.BetType != 5 ||
                                         userbet.MatchUserBetContent.Count(c => c.IsSettle == OpenEnum.Settlementing) != 0) &&
                                        userbet.BetType == 5) continue;
                                    {
                                        userbet.IsSettle = OpenEnum.Settlemented;
                                        userbet.SettleTime = DateTime.Now;
                                        userbet.BetBonus =
                                            userbet.MatchUserBetContent.Select(
                                                c => c.SettleIor ?? 0).Aggregate((i, j) => i * j) *
                                            userbet.BetValue;
                                        listBetUpdate.Add(userbet);
                                    }
                                }
                            }
                            //结算本赛
                            mc1.IsSettle = OpenEnum.Settlemented;
                            mc1.SettleTime = DateTime.Now;
                            mc1.SettleUserId = settleUserId;

                            //执行数据库操作
                            sb.AppendFormat(
                                "UPDATE MatchInfo SET IsSettle={0},SettleTime='{1}',SettleUserId={2} WHERE MatchId={3};",
                                1, mc1.SettleTime, mc1.SettleUserId, mc1.MatchId);
                        }
                        foreach (var item in listBetUpdate)
                        {
                            sb.AppendFormat("UPDATE MatchUserBet SET IsSettle={0},SettleTime='{1}',BetBonus={2} WHERE BetId={3};", 1, item.SettleTime, item.BetBonus, item.BetId);
                            userAmount.Add(new UserBetAmountModel
                            {
                                BetId = item.BetId,
                                UserId = item.UserId,
                                UserName = item.UserAccount.LoginName,
                                Amount = (item.BetBonus - item.BetValue) > 0 ? item.BetBonus - item.BetValue : 0
                            });
                        }
                        foreach (var item in listBetContentUpdate)
                        {
                            sb.AppendFormat("UPDATE MatchUserBetContent SET IsSettle={0},SettleIor={1} WHERE Id={2};",
                                1, item.SettleIor, item.Id);
                        }
                        //用户余额修改，资金流水明细记录
                        if (userAmount.Any())
                        {
                            foreach (var item in userAmount.Where(item => item.Amount > 0))
                            {
                                sb.AppendFormat(
                                    "INSERT INTO AmountWater(UserId,UserName,Amount,Type,Remark,CreateDate) VALUES({0},'{1}',{2},{3},'{4}','{5}');",
                                    item.UserId, item.UserName, item.Amount, (int)TradEnum.MatchLottery,
                                    string.Format("比赛结算 注单{0}中奖 金额{1} 返还奖额至余额", item.BetId, item.Amount), DateTime.Now);
                            }
                            var group = userAmount.Where(a => a.Amount > 0).GroupBy(s => s.UserId, (i, v) => new UserBetAmountGroupModel
                            {
                                UserId = i,
                                Group = v.ToList()
                            }).ToArray();
                            foreach (var items in group)
                            {
                                sb.AppendFormat("UPDATE UserAccount SET BalanceAmount=BalanceAmount+{0} WHERE Id={1};",
                                    items.Group.Sum(s => s.Amount.Value), items.UserId);
                            }
                        }
                        #endregion
                        break;
                }

                Log.Debug(sb.ToString());
                //return false;
                return QueryDb((context) => context.Database.ExecuteSqlCommand(TransactionalBehavior.EnsureTransaction, sb.ToString()) > 0, false);
            }
            catch (Exception ex)
            {
                Log.Error("结算失败" + ex); ;
                return false;
            }
        }
        #region Private Method

        /// <summary>
        /// 获取中奖赔率，没中奖的则为0
        /// </summary>
        /// <param name="dicResult"></param>
        /// <param name="mc"></param>
        /// <returns></returns>
        private decimal GetIor(Dictionary<string, string> dicResult, MatchUserBetContent mc)
        {
            decimal ior = 0;
            try
            {
                ior = decimal.Parse(mc.BetContent.Split('@')[1].Trim());
                var key = mc.BetContent.Split('@')[0].Trim();
                #region 让球
                if (mc.BetType == 12 || mc.BetType == 13 || mc.BetType == 63)
                {
                    var strong = mc.BetKey.Split('|')[0].Trim();
                    var ratio = mc.BetKey.Split('|')[1].Trim();
                    decimal ratio1 = 0;
                    decimal ratio2 = 0;
                    string result1 = "";//让球后结果
                    string result2 = "";//让球后结果
                    if (ratio.Split('/').Length > 1)
                    {
                        ratio1 = decimal.Parse((strong == "H" ? "-" : "") + ratio.Split('/')[0].Trim());
                        ratio2 = decimal.Parse((strong == "H" ? "-" : "") + ratio.Split('/')[1].Trim());
                    }
                    else
                    {
                        ratio2 = ratio1 = decimal.Parse((strong == "H" ? "-" : "") + ratio.Trim());
                    }
                    if (ratio1 + decimal.Parse(dicResult[mc.BetType + ""]) == 0)
                    {
                        result1 = "N";//平
                    }
                    else if (ratio1 + decimal.Parse(dicResult[mc.BetType + ""]) > 0)
                    {
                        result1 = "H";//主胜
                    }
                    else
                    {
                        result1 = "C";//客胜
                    }
                    if (ratio2 + decimal.Parse(dicResult[mc.BetType + ""]) == 0)
                    {
                        result2 = "N";//平
                    }
                    else if (ratio2 + decimal.Parse(dicResult[mc.BetType + ""]) > 0)
                    {
                        result2 = "H";//主胜
                    }
                    else
                    {
                        result2 = "C";//客胜
                    }
                    decimal tmp = 0;
                    tmp += result1 == "N" ? 0.5M : result1 == key ? ior / 2 : 0;
                    tmp += result2 == "N" ? 0.5M : result2 == key ? ior / 2 : 0;
                    ior = tmp;
                }
                #endregion
                #region 大小球
                else if (mc.BetType == 14 || mc.BetType == 15 || mc.BetType == 61)
                {
                    var ratio = mc.BetKey.Trim();
                    const decimal ratio1 = 0;
                    const decimal ratio2 = 0;
                    var result1 = "";//结果12
                    var result2 = "";//结果

                    if (ratio1 == decimal.Parse(dicResult[mc.BetType + ""]))
                    {
                        result1 = "N";//平
                    }
                    else if (ratio1 < decimal.Parse(dicResult[mc.BetType + ""]))
                    {
                        result1 = "O";//大球
                    }
                    else
                    {
                        result1 = "U";//小球
                    }
                    if (ratio2 == decimal.Parse(dicResult[mc.BetType + ""]))
                    {
                        result2 = "N";//平
                    }
                    else if (ratio2 < decimal.Parse(dicResult[mc.BetType + ""]))
                    {
                        result2 = "O";//大球
                    }
                    else
                    {
                        result2 = "U";//小球
                    }
                    decimal tmp = 0;
                    tmp += result1 == "N" ? 0.5M : result1 == key ? ior / 2 : 0;
                    tmp += result2 == "N" ? 0.5M : result2 == key ? ior / 2 : 0;
                    ior = tmp;
                }
                #endregion
                else
                {
                    if (key != dicResult[mc.BetType + ""])
                    {
                        ior = 0;
                    }
                }
            }
            catch
            {
                ior = 0;
            }
            return ior;
        }

        /// <summary>
        /// 根据比赛结果，组合出所有的赛果
        /// </summary>
        /// <param name="mr"></param>
        /// <returns></returns>
        private static Dictionary<string, string> GetResult(MatchResult mr, XBETMatchResult xbet = null)
        {
            var dicResult = new Dictionary<string, string>();
            try
            {
                if (xbet == null)
                {
                    var hmh = int.Parse(mr.Result1.Split(':')[0]);
                    var hmc = int.Parse(mr.Result1.Split(':')[1]);
                    var mh = int.Parse(mr.Result2.Split(':')[0]);
                    var mc = int.Parse(mr.Result2.Split(':')[1]);
                    dicResult.Add("10", string.Format("{0}", mh > mc ? "H" : mh == mc ? "N" : "C")); //全场胜负平
                    dicResult.Add("11", string.Format("{0}", hmh > hmc ? "H" : hmh == hmc ? "N" : "C")); //半场胜负平
                    dicResult.Add("12", string.Format("{0}", mh - mc)); //全场分差
                    dicResult.Add("13", string.Format("{0}", hmh - hmc)); //半场分差
                    dicResult.Add("14", string.Format("{0}", mh + mc)); //全场总进球
                    dicResult.Add("15", string.Format("{0}", hmh + hmc)); //半场总进球
                    dicResult.Add("20",
                        string.Format("{0}",
                            (mh + mc) >= 7 ? "7+" : (mh + mc) >= 4 ? "4-6" : (mh + mc) >= 2 ? "2-3" : "0-1"));
                    //全场总进球
                    dicResult.Add("30", string.Format("{0}", (mh > 4 || mc > 4) ? "OVH" : mr.Result2)); //全场比分
                    dicResult.Add("31", string.Format("{0}", (hmh > 3 || hmc > 3) ? "OVH" : mr.Result1)); //半场比分
                    dicResult.Add("40", string.Format("{0}", dicResult["11"] + dicResult["10"])); //半全场
                    dicResult.Add("16", string.Format("{0}", (mh + mc) % 2 == 0 ? "E" : "O")); //全场总进球
                    dicResult.Add("50", string.Format("{0}", mh > mc ? "H" : mh == mc ? "N" : "C")); //全场胜负平
                    dicResult.Add("51", string.Format("{0}", hmh > hmc ? "H" : hmh == hmc ? "N" : "C")); //半场胜负平
                    dicResult.Add("52", string.Format("{0}", mh - mc)); //全场分差
                    dicResult.Add("53", string.Format("{0}", hmh - hmc)); //半场分差
                    dicResult.Add("54", string.Format("{0}", mh + mc)); //全场总进球
                    dicResult.Add("55", string.Format("{0}", hmh + hmc)); //半场总进球
                    dicResult.Add("56", string.Format("{0}", (mh + mc) % 2 == 0 ? "E" : "O")); //全场总进球
                }
                else
                {
                    var mh = int.Parse(xbet.Result2.Split(':')[0]);
                    var mc = int.Parse(xbet.Result2.Split(':')[1]);
                    dicResult.Add("60", string.Format("{0}", mh > mc ? "H" : mh == mc ? "N" : "C")); //滚球全场胜负平
                    dicResult.Add("61", string.Format("{0}", mh + mc)); //滚球全场总进球
                    dicResult.Add("63", string.Format("{0}", mh - mc)); //滚球全场分差
                }
            }
            catch
            {
                // ignored
            }
            return dicResult;
        }

        #endregion
        public List<MatchViewModel> GetMatchDetail(MatchQueryModel query)
        {
            var match = new List<MatchViewModel>();
            return QueryDb((context) =>
            {
                if (!string.IsNullOrWhiteSpace(query.MatchIds))
                {
                    var matchid = query.MatchIds.Split(',').Select(long.Parse);

                    var group =
                        context.MatchInfo.Where(s => matchid.Contains(s.MatchId))
                            .ToArray();
                    if (query.DataType.Equals("xml"))

                        #region Odds返回xml格式

                        match = group.Select(s => new MatchViewModel
                        {
                            MatchId = s.MatchId,
                            LeagueName = query.Language == "cn" ? s.LeagueName : s.LeagueNameEn,
                            MatchDate =
                                DateTimeHelper.UnixTimestampToDateTime(s.MatchDate).AddHours(query.TimeZone.Value).ToString("HH:mm"),
                            MatchNumber = s.MatchNumber,
                            MatchType = s.MatchType,
                            MatchEndTime = s.MatchEndTime,
                            HTeamNum = s.HTeamNum,
                            HTeam = query.Language == "cn" ? s.HTeam : s.HTeamEn,
                            CTeamNum = s.CTeamNum,
                            CTeam = query.Language == "cn" ? s.CTeam : s.CTeamEn,
                            BetType = s.BetType,
                            Odds = s.Odds,
                            CreateDate = s.CreateDate,
                            OddsUpdateTime = s.OddsUpdateTime,
                            EventId = s.EventId,
                            MatchDateWeek =
                                DateTimeHelper.GetDayOfWeekCN(
                                    DateTimeHelper.UnixTimestampToDateTime(s.MatchDate).AddHours(query.TimeZone.Value)),
                            MinLimit = s.MinLimit ?? 0,
                            MaxLimit = s.MaxLimit ?? 0
                        }).ToList();

                        #endregion

                    else
                        match = group.Select(s => new MatchViewModel
                        {
                            MatchId = s.MatchId,
                            LeagueName = query.Language == "cn" ? s.LeagueName : s.LeagueNameEn,
                            MatchDate =
                                DateTimeHelper.UnixTimestampToDateTime(s.MatchDate).AddHours(query.TimeZone.Value).ToString("HH:mm"),
                            MatchNumber = s.MatchNumber,
                            MatchType = s.MatchType,
                            MatchEndTime = s.MatchEndTime,
                            HTeamNum = s.HTeamNum,
                            HTeam = query.Language == "cn" ? s.HTeam : s.HTeamEn,
                            CTeamNum = s.CTeamNum,
                            CTeam = query.Language == "cn" ? s.CTeam : s.CTeamEn,
                            BetType = s.BetType,
                            Odds = s.OddData,
                            CreateDate = s.CreateDate,
                            OddsUpdateTime = s.OddsUpdateTime,
                            EventId = s.EventId,
                            MatchDateWeek =
                                DateTimeHelper.GetDayOfWeekCN(
                                    DateTimeHelper.UnixTimestampToDateTime(s.MatchDate).AddHours(query.TimeZone.Value)),
                            MinLimit = s.MinLimit ?? 0,
                            MaxLimit = s.MaxLimit ?? 0
                        }).ToList();

                }
                return match;
            }, match);
        }
        public List<MatchResultViewModel> GetMatchResult(long matchId, int source = 0)
        {
            var grid = new List<MatchResultViewModel>();
            return QueryDb((context) =>
            {
                switch (source)
                {
                    case 1:
                        grid = context.XBETMatchResult.Where(s => s.MatchId == matchId).ToArray().Select(s => new MatchResultViewModel
                        {
                            Result1 = s.Result1,
                            Result2 = s.Result2
                        }).ToList();
                        break;
                    default:
                        grid = context.MatchResult.Where(s => s.MatchId == matchId).ToArray().Select(s => new MatchResultViewModel
                        {
                            Result1 = s.Result1,
                            Result2 = s.Result2
                        }).ToList();
                        break;
                }
                return grid;
            }, grid);
        }
        /// <summary>
        /// 取消比赛
        /// </summary>
        /// <param name="matchIds"></param>
        /// <param name="cancelUserId"></param>
        /// <returns></returns>
        public bool CancelMatch(IEnumerable<long> matchIds, int cancelUserId, int source = 0)
        {
            var sb = new StringBuilder();
            var listBet = new List<MatchUserBet>();
            try
            {
                switch (source)
                {
                    case 1:
                        #region 滚球
                        var listMatchR = new List<XBETMatchInfo>();
                        if (!QueryDb((context) =>
                        {
                            listMatchR =
                                context.XBETMatchInfo.Include("XBETMatchResult")
                                    .Where(
                                        c =>
                                            matchIds.Contains(c.MatchId) &&
                                            !c.XBETMatchResult.Select(s => s.MatchId).Any(s => matchIds.Contains(s)))
                                    .ToList();
                            if (listMatchR.Count > 0)
                            {
                                matchIds = listMatchR.Select(c => c.MatchId).ToList();
                                listBet =
                                    context.MatchUserBet.Include("MatchUserBetContent").Include("UserAccount")
                                        .Where(s => s.MatchUserBetContent.Select(c => c.MatchID).Any(b => matchIds.Contains(b)))
                                        .ToList();
                            }
                            return true;
                        }, false))
                            return false;
                        var timeR = DateTime.Now;
                        foreach (var item in listMatchR)
                        {
                            if (item.IsMaster == 1)
                            {
                                sb.AppendFormat(
                                    "INSERT INTO MatchResult(MatchId,ResultTime,Result1,Result2) VALUES ({0},{1},'取消','取消');",
                                    item.MatchId, DateTimeHelper.DateTimeToUnixTimestamp(timeR));
                            }
                            sb.AppendFormat(
                                    "UPDATE MatchInfo SET IsSettle={0},SettleTime='{1}',SettleUserId={2} WHERE MatchId={3};",
                                    (int)OpenEnum.Canceled, timeR, cancelUserId, item.MatchId);
                        }
                        var userAmountR = new List<UserBetAmountModel>();
                        foreach (var item in listBet)
                        {
                            sb.AppendFormat("UPDATE MatchUserBet SET IsSettle={0},SettleTime='{1}',BetBonus={2} WHERE BetId={3};", (int)OpenEnum.Canceled, timeR, item.BetValue, item.BetId);
                            sb.AppendFormat("INSERT INTO AmountWater(UserId,UserName,Amount,Type,Remark,CreateDate) VALUES({0},'{1}',{2},{3},'{4}','{5}');",
                                item.UserId, item.UserAccount.LoginName, item.BetValue, (int)TradEnum.MatchCancel, string.Format("比赛取消 注单 {0} 本金 {1} 退还", item.BetId, item.BetValue), timeR);
                            userAmountR.Add(new UserBetAmountModel
                            {
                                UserId = item.UserId,
                                Amount = item.BetValue
                            });
                            foreach (var items in item.MatchUserBetContent)
                            {
                                sb.AppendFormat("UPDATE MatchUserBetContent SET IsSettle={0},SettleIor={1} WHERE Id={2};",
                                    (int)OpenEnum.Canceled, decimal.Parse(items.BetContent.Split('@')[1]), items.Id);
                            }
                        }
                        if (userAmountR.Any())
                        {
                            var group = userAmountR.Where(a => a.Amount > 0).GroupBy(s => s.UserId, (i, v) => new UserBetAmountGroupModel
                            {
                                UserId = i,
                                Group = v.ToList()
                            }).ToArray();
                            foreach (var items in group)
                            {
                                sb.AppendFormat("UPDATE UserAccount SET BalanceAmount=BalanceAmount+{0} WHERE Id={1};",
                                items.Group.Sum(s => s.Amount.Value), items.UserId);
                            }
                        }
                        #endregion
                        break;
                    default:
                        #region 标准玩法
                        var listMatch = new List<MatchInfo>();
                        if (!QueryDb((context) =>
                {
                    listMatch =
                        context.MatchInfo.Include("MatchResult")
                            .Where(
                                c =>
                                    matchIds.Contains(c.MatchId) &&
                                    !c.MatchResult.Select(s => s.MatchId).Any(s => matchIds.Contains(s)))
                            .ToList();
                    if (listMatch.Count > 0 && listMatch[0].EventId > 0)
                    {
                        var eventId = listMatch[0].EventId;
                        if (eventId != null)
                        {
                            var eventid = eventId.Value;
                            listMatch = context.MatchInfo.Include("MatchResult").Where(c => c.EventId == eventid).ToList();
                        }
                        matchIds = listMatch.Select(c => c.MatchId).ToList();
                        listBet =
                            context.MatchUserBet.Include("MatchUserBetContent").Include("UserAccount")
                                .Where(s => s.MatchUserBetContent.Select(c => c.MatchID).Any(b => matchIds.Contains(b)))
                                .ToList();
                    }
                    return true;
                }, false))
                            return false;
                        var time = DateTime.Now;
                        foreach (var item in listMatch)
                        {
                            if (item.IsMaster == 1)
                            {
                                sb.AppendFormat(
                                    "INSERT INTO MatchResult(MatchId,ResultTime,Result1,Result2) VALUES ({0},{1},'取消','取消');",
                                    item.MatchId, DateTimeHelper.DateTimeToUnixTimestamp(time));
                            }
                            sb.AppendFormat(
                                    "UPDATE MatchInfo SET IsSettle={0},SettleTime='{1}',SettleUserId={2} WHERE MatchId={3};",
                                    (int)OpenEnum.Canceled, time, cancelUserId, item.MatchId);
                        }
                        var userAmount = new List<UserBetAmountModel>();
                        foreach (var item in listBet)
                        {
                            sb.AppendFormat("UPDATE MatchUserBet SET IsSettle={0},SettleTime='{1}',BetBonus={2} WHERE BetId={3};", (int)OpenEnum.Canceled, time, item.BetValue, item.BetId);
                            sb.AppendFormat("INSERT INTO AmountWater(UserId,UserName,Amount,Type,Remark,CreateDate) VALUES({0},'{1}',{2},{3},'{4}','{5}');",
                                item.UserId, item.UserAccount.LoginName, item.BetValue, (int)TradEnum.MatchCancel, string.Format("比赛取消 注单 {0} 本金 {1} 退还", item.BetId, item.BetValue), time);
                            userAmount.Add(new UserBetAmountModel
                            {
                                UserId = item.UserId,
                                Amount = item.BetValue
                            });
                            foreach (var items in item.MatchUserBetContent)
                            {
                                sb.AppendFormat("UPDATE MatchUserBetContent SET IsSettle={0},SettleIor={1} WHERE Id={2};",
                                    (int)OpenEnum.Canceled, decimal.Parse(items.BetContent.Split('@')[1]), items.Id);
                            }
                        }
                        if (userAmount.Any())
                        {
                            var group = userAmount.Where(a => a.Amount > 0).GroupBy(s => s.UserId, (i, v) => new UserBetAmountGroupModel
                            {
                                UserId = i,
                                Group = v.ToList()
                            }).ToArray();
                            foreach (var items in group)
                            {
                                sb.AppendFormat("UPDATE UserAccount SET BalanceAmount=BalanceAmount+{0} WHERE Id={1};",
                                items.Group.Sum(s => s.Amount.Value), items.UserId);
                            }
                        }
                        #endregion
                        break;
                }
                Log.Debug(sb.ToString());
                //return false;
                return QueryDb((context) => context.Database.ExecuteSqlCommand(TransactionalBehavior.EnsureTransaction, sb.ToString()) > 0, false);
            }
            catch (Exception ex)
            {
                Log.Error("取消失败" + ex); ;
                return false;
            }
        }
        public bool MatchBlackInfo(IEnumerable<long> matchIds, int operateUserId, int datasource = 0)
        {
            return QueryDb((context) =>
            {
                var groupLeague = datasource == 0
                    ? context.MatchInfo.Where(s => matchIds.Contains(s.MatchId))
                        .GroupBy(a => new { a.LeagueName, a.LeagueNameEn })
                        .Select(a => a.Key)
                        .ToList()
                    : context.XBETMatchInfo.Where(s => matchIds.Contains(s.MatchId))
                        .GroupBy(a => new { a.LeagueName, a.LeagueNameEn })
                        .Select(a => a.Key)
                        .ToList();
                if (groupLeague.Any())
                {
                    var blackLeague = context.MatchBlackList.Select(s => new
                    {
                        s.LeagueName,
                        s.LeagueNameEn
                    }).ToList();
                    var exceptLeague = groupLeague.Except(blackLeague);
                    context.MatchBlackList.AddRange(exceptLeague.Select(s => new MatchBlackList
                    {
                        IsBan = YesNoEnum.Yes,
                        LeagueName = s.LeagueName,
                        LeagueNameEn = s.LeagueNameEn,
                        OperateUserId = operateUserId,
                        LastOperateDate = DateTime.Now,
                        DataSource = (DataSourceEnum)datasource
                    }));
                    context.SaveChanges();
                }
                return true;
            }, false);
        }
    }
}