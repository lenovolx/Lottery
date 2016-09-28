using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FT.Model;
using FT.Model.ViewModel;
using FT.Plugin.Cache;
using FT.Utility.Helper;
using Quartz;

namespace FT.Task
{
    public class FTPlayMatchLogJob : BaseJob, IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            Log.Debug("开始转移滚球比赛日志");
            var logs = new List<XBETMatchInfoLog>();
            var list = Cache.GetList<XBETMatchInfoLogViewModel>(CacheKeyCollection.RollBallMatchLog + "_");
            if (list.Any())
            {
                logs.AddRange(from item in list
                    from items in item.Logs
                    select new XBETMatchInfoLog
                    {
                        LeagueName = items.LeagueName,
                        LeagueNameEn = items.LeagueNameEn,
                        MatchDate = items.MatchDate,
                        MatchNumber = items.MatchNumber,
                        MatchType = 6,
                        HTeamNum = items.HTeamNum,
                        HTeam = items.HTeam,
                        HTeamEn = items.HTeamEn,
                        CTeamNum = items.CTeamNum,
                        CTeam = items.CTeam,
                        CTeamEn = items.CTeamEn,
                        BetType = items.BetType,
                        Odds = items.Odds,
                        OddsUpdateTime = items.OddsUpdateTime,
                        CurrentScore = items.CurrentScore,
                        Finished = items.Finished,
                        TimeSec = items.TimeSec,
                        TimeRun = items.TimeRun
                    });
                QueryDb((db) =>
                {
                    db.XBETMatchInfoLog.AddRange(logs);
                    db.SaveChanges();
                    //Cache.Remove(CacheKeyCollection.RollBallMatchLog + "_", 1);
                });
            }
            else
                Log.Debug("无待转移滚球比赛日志");
            Log.Debug("结束转移滚球比赛日志");
        }
    }
}
