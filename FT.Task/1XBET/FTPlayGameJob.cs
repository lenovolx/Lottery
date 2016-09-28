using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using EntityFramework.Extensions;
using FT.Model;
using FT.Model.ViewModel;
using FT.Plugin.Cache;
using FT.Utility.Helper;
using Newtonsoft.Json;
using Quartz;

namespace FT.Task
{
    public class FTPlayGameJob : BaseJob, IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            Log.Debug("1XBET FTPlayGameJob  抓取开始");
            try
            {
                string str = string.Empty;
                var game = HttpHelper.SimpleGet<Root>(XBETUrl, ref str);
                if (game != null)
                {
#if DEBUG
                    Log.Debug(str);
#endif
                    var matchColls =
                        game.Value.Where(s => s.GameTypeId == 1).Select(item => new XBETMatchInfo
                        {
                            MatchId = item.MainGameId,
                            LeagueName = item.Champ,
                            LeagueNameEn = item.ChampEng,
                            HTeam = item.Opp1,
                            HTeamEn = item.Opp1Eng,
                            CTeam = item.Opp2,
                            CTeamEn = item.Opp2Eng,
                            HTeamNum = item.Opp1Id + "",
                            CTeamNum = item.Opp2Id + "",
                            MatchDate =
                                DateTimeHelper.DateTimeToUnixTimestamp(
                                    DateTimeHelper.UnixTimestampToDateTime(item.Start).AddHours(-8), 0),
                            MatchNumber = item.MainGameId,
                            OddsUpdateTime = DateTimeHelper.DateTimeToUnixTimestamp(DateTime.Now, 0),
                            IsMaster = 1,
                            Odds = OddsXml(item.Events),
                            BetType = "6",
                            CreateDate = DateTime.Now,//DateTimeHelper.UnixTimestampToDateTime(item.Start).AddHours(-8),
                            CurrentScore =
                                string.Format("{0}:{1}", item.Scores.FullScore.Sc1, item.Scores.FullScore.Sc2),
                            TimeSec = item.Scores.TimeSec,
                            Finished = item.Finished,
                            TimeRun = item.Scores.TimeRun,
                            XBETMatchResult = new List<XBETMatchResult>
                            {
                                new XBETMatchResult
                                {
                                    MatchId = item.MainGameId,
                                    Result1 =
                                        string.Format("{0}",
                                            item.Scores.PeriodScores.ElementAtOrDefault(0) == null
                                                ? "0:0"
                                                : item.Scores.PeriodScores.ElementAtOrDefault(0).Value.Sc1 + ":" +
                                                  item.Scores.PeriodScores.ElementAtOrDefault(0).Value.Sc2),
                                    Result2 =
                                        string.Format("{0}",
                                            item.Scores.FullScore != null
                                                ? item.Scores.FullScore.Sc1 + ":" + item.Scores.FullScore.Sc2
                                                : "0:0"),
                                    ResultTime = DateTimeHelper.DateTimeToUnixTimestamp(DateTime.Now, 0)
                                }
                            }
                        });
                    if (matchColls.Any())
                    {
                        if (Cache.Get(CacheKeyCollection.RollBallCurrent) != null)
                            Cache.Remove(CacheKeyCollection.RollBallCurrent);
                        Cache.Add(CacheKeyCollection.RollBallCurrent, matchColls, 2);
                        //日志记录
                        var logs = new XBETMatchInfoLogViewModel
                        {
                            Logs = matchColls.Select(s => new XBETMatchInfoLog
                            {
                                LeagueName = s.LeagueName,
                                LeagueNameEn = s.LeagueNameEn,
                                MatchDate = s.MatchDate,
                                MatchNumber = s.MatchNumber,
                                MatchType = 6,
                                HTeamNum = s.HTeamNum,
                                HTeam = s.HTeam,
                                HTeamEn = s.HTeamEn,
                                CTeamNum = s.CTeamNum,
                                CTeam = s.CTeam,
                                CTeamEn = s.CTeamEn,
                                BetType = s.BetType,
                                Odds = s.Odds,
                                OddsUpdateTime = s.OddsUpdateTime,
                                CurrentScore = s.CurrentScore,
                                TimeSec = s.TimeSec,
                                Finished = s.Finished,
                                TimeRun = s.TimeRun
                            }).ToList()
                        };
                        Cache.Add(string.Format("{0}_{1}", CacheKeyCollection.RollBallMatchLog,
                            DateTime.Now.ToString("yyyyMMddHHmmss")), JsonConvert.SerializeObject(logs)
                            , 120);
                        var graspMatchNums = matchColls.Select(s => s.MatchNumber);
                        QueryDb((db) =>
                        {
                            var existsMatch =
                                db.XBETMatchInfo.Where(s => graspMatchNums.Contains(s.MatchNumber)).ToArray();
                            //比赛信息
                            if (existsMatch.Any())
                            {
                                foreach (var item in existsMatch)
                                {
                                    var matchInfo = matchColls.FirstOrDefault(s => s.MatchNumber == item.MatchNumber);
                                    if (matchInfo == null) continue;
                                    var datenow = DateTime.Now;
                                    db.XBETMatchInfo.Where(s => s.MatchNumber == item.MatchNumber)
                                        .Update(u => new XBETMatchInfo
                                        {
                                            OddsUpdateTime = DateTimeHelper.DateTimeToUnixTimestamp(datenow, 0),
                                            Odds = matchInfo.Odds,
                                            UpdateDate = datenow,
                                            CurrentScore = matchInfo.CurrentScore,
                                            TimeSec = matchInfo.TimeSec,
                                            Finished = matchInfo.Finished,
                                            TimeRun = matchInfo.TimeRun
                                        });

                                    db.XBETMatchResult.Where(s => s.MatchId == matchInfo.MatchId)
                                        .Update(u => new XBETMatchResult
                                        {
                                            ResultTime = DateTimeHelper.DateTimeToUnixTimestamp(datenow, 0),
                                            Result1 = matchInfo.XBETMatchResult.ElementAtOrDefault(0).Result1,
                                            Result2 = matchInfo.XBETMatchResult.ElementAtOrDefault(0).Result2
                                        });
                                }
                            }
                            var dbMatchNums = db.XBETMatchInfo.Select(s => s.MatchNumber).ToArray();
                            var dbLeague =
                                db.MatchBlackList.Where(s => s.DataSource == DataSourceEnum.XBET)
                                    .Select(s => s.LeagueName)
                                    .ToArray();
                            var notExistsMatch =
                                matchColls.Where(
                                    s => !dbMatchNums.Contains(s.MatchNumber) && !dbLeague.Contains(s.LeagueName));
                            if (notExistsMatch.Any())
                            {
                                db.XBETMatchInfo.AddRange(notExistsMatch);
                            }
                            db.SaveChanges();
                        });
                    }
                }
                else
                    Log.Debug("当前无比赛数据");
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            finally
            {
                Log.Debug("1XBET FTPlayGameJob  抓取结束");
            }
        }

        private string OddsXml(List<EventsItem> eventItem)
        {
            if (eventItem != null)
            {
                var xml = new XElement("MatchContent");
                IEnumerable<EventsItem> data;
                if (eventItem.Any(s => s.G == 1)) //胜平负
                {
                    data = eventItem.Where(s => s.G == 1);
                    var H = data.FirstOrDefault(s => s.T == 1) == null ? 0 : data.FirstOrDefault(s => s.T == 1).C;
                    var N = data.FirstOrDefault(s => s.T == 2) == null ? 0 : data.FirstOrDefault(s => s.T == 2).C;
                    var C = data.FirstOrDefault(s => s.T == 3) == null ? 0 : data.FirstOrDefault(s => s.T == 3).C;
                    xml.Add(new XElement("BetInfo",
                        new XElement("BetType", (int)PlayEnum.RollBallWinAll),
                        new XElement("BetKey", "H|N|C"),
                        new XElement("BetIOR", string.Format("{0}|{1}|{2}", H, N, C))));
                }
                if (eventItem.Any(s => s.G == 17)) //大小球
                {
                    data = eventItem.Where(s => s.G == 17);
                    xml.Add(new XElement("BetInfo",
                        new XElement("BetType", (int)PlayEnum.RollBallZiseAll),
                        new XElement("BetKey", string.Format("{0}", eventItem.FirstOrDefault(s => s.G == 17).P)),
                        new XElement("BetIOR",
                            data
                                .OrderBy(s => s.T)
                                .Select(s => s.C.ToString())
                                .Aggregate((c, n) => c + "|" + n))));
                }
                if (eventItem.Any(s => s.G == 2)) //让球
                {
                    data = eventItem.Where(s => s.G == 2);
                    xml.Add(new XElement("BetInfo",
                        new XElement("BetType", (int)PlayEnum.RoleBallHalf),
                        new XElement("BetKey",
                            data.OrderBy(s => s.T)
                                .Select(s => s.P.ToString())
                                .Aggregate(
                                    (c, p) => c == "0"
                                        ? string.Format("{0}|{1}", "H", c)
                                        : (double.Parse(c) < double.Parse(p)
                                            ? string.Format("{0}|{1}", "H",
                                                double.Parse(c) > 0 ? c : ((double.Parse(c)) * -1) + "")
                                            : string.Format("{0}|{1}", "C",
                                                double.Parse(p) > 0 ? p : ((double.Parse(p)) * -1) + "")))),
                        new XElement("BetIOR",
                            data.OrderBy(s => s.T)
                                .Select(s => s.C.ToString())
                                .Aggregate((c, n) => c + "|" + n))));
                }
                return xml.HasElements ? xml.ToString(SaveOptions.DisableFormatting) : string.Empty;
            }
            else
                return string.Empty;
        }
    }
}
