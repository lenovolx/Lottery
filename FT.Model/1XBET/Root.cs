using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT.Model
{
    public class EventsItem
    {
        /// <summary>
        /// 
        /// </summary>
        public bool B { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double C { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CV { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int G { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double P { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PV { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Pl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int T { get; set; }
    }

    public class FullScore
    {
        /// <summary>
        /// 
        /// </summary>
        public int Sc1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Sc2 { get; set; }
    }

    public class Value
    {
        /// <summary>
        /// 
        /// </summary>
        public int Sc1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Sc2 { get; set; }
    }

    public class PeriodScoresItem
    {
        /// <summary>
        /// 
        /// </summary>
        public int Key { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Value Value { get; set; }
    }

    public class StatisticItem
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
    public class Scores
    {
        /// <summary>
        /// 
        /// </summary>
        public string __type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CourseOfPlay { get; set; }
        /// <summary>
        /// 2 半场
        /// </summary>
        public string CurrPeriodStr { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int CurrentPeriod { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public FullScore FullScore { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<PeriodScoresItem> PeriodScores { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<StatisticItem> Statistic { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Info { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Podacha { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SubScore { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool TimeDirection { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool TimeRun { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int TimeSec { get; set; }
    }

    public class ValueItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string DopInfo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<EventsItem> Events { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int EventsCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool Finished { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string GameType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int GameTypeId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string GameVid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int GameVidId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int IsTourn { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int LigaId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string NameGame { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Num { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Period { get; set; }
        /// <summary>
        /// 半场
        /// </summary>
        public string PeriodName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Risk { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int SportId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string AnimCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string AnimParam { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Before { get; set; }
        /// <summary>
        /// RFS 杯 U18（上下半场各 40 分钟）
        /// </summary>
        public string Champ { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ChampEng { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ChampRus { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int ClosingSoon { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Cont { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string GameDescr { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string MainEvents { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int MainGameId { get; set; }
        /// <summary>
        /// 比赛地点：坦波夫
        /// </summary>
        public string MatchInfo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Opp1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Opp1Eng { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Opp1Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Opp1Rus { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Opp2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Opp2Eng { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Opp2Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Opp2Rus { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Scores Scores { get; set; }
        /// <summary>
        /// 足球
        /// </summary>
        public string SportName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SportNameEng { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SportNameRus { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Start { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SubGames { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string TvChannel { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string VA { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string VI { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int ZonePlay { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string curPeriodAsia { get; set; }
    }

    public class Root
    {
        /// <summary>
        /// 
        /// </summary>
        public string Error { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int ErrorCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Guid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<ValueItem> Value { get; set; }
    }
}
