using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Linq;
using EntityFramework.Extensions;
using FT.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FT.Model;
using FT.Plugin.Cache;
using FT.Repository;
using FT.Utility.Helper;
using HtmlAgilityPack;
using Newtonsoft.Json;

namespace FT.Tests
{
    [TestClass]
    public class UnitTest2
    {
        public const string XBETUrl =
            "https://1x-bet-bk.com/LiveFeed/BestGamesExt?sportId=&sports=1&champs=&tf=1000000&tz=0&afterDays=0&count=50&cnt=50&lng=cn&cfview=4&page=0&antisports=&mode=4&subGames=&cyberFlag=0&country=90&partner=1";

        [TestMethod]
        public void TestMethod1()
        {
            //using (var context = new Entity())
            //{
            //    UpdateEntity(DbSet<>)
            //}

            var a = new MatchInfoRepository().UpdateSql(w => w.MatchId == 10702, u => new MatchInfo
            {
                SettleTime = DateTime.Now
            });

            Assert.IsTrue(a > 0);
        }

        [TestMethod]
        public void GetEnumDesString()
        {
            string str = string.Empty;
            var game = HttpHelper.SimpleGet<Root>(XBETUrl, ref str);
            if (game != null)
            {
                try
                {
                    var matchColls = game.Value.Select(item => new XBETMatchInfo
                    {
                        LeagueName = item.Champ,
                        LeagueNameEn = item.ChampEng,
                        HTeam = item.Opp1,
                        HTeamEn = item.Opp1Eng,
                        CTeam = item.Opp2,
                        CTeamEn = item.Opp2Eng,
                        HTeamNum = item.Opp1Id + "",
                        CTeamNum = item.Opp2Id + "",
                        MatchDate = item.Start,
                        MatchNumber = item.Id,
                        OddsUpdateTime = DateTimeHelper.DateTimeToUnixTimestamp(DateTime.Now, 0),
                        IsMaster = 1,
                        Odds = OddsXml(item.Events)
                        //BetType = item.Events.Aggregate((c,n)=>new {c.})
                    }).ToList();
                    Log.Info(JsonConvert.SerializeObject(matchColls));
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
            }
            Assert.IsNull(game);
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
                    xml.Add(new XElement("BetInfo",
                        new XElement("BetType", (int) PlayEnum.RollBallWinAll),
                        new XElement("BetKey", "H|C|N"),
                        new XElement("BetIOR",
                            data.OrderBy(s => s.T)
                                .Select(s => s.C.ToString())
                                .Aggregate((c, n) => c + "|" + n))));
                }
                if (eventItem.Any(s => s.G == 2)) //让球
                {
                    data = eventItem.Where(s => s.G == 2);
                    xml.Add(new XElement("BetInfo",
                        new XElement("BetType", (int) PlayEnum.RoleBallHalf),
                        new XElement("BetKey",
                            data.OrderBy(s => s.T)
                                .Select(s => s.P.ToString())
                                .Aggregate(
                                    (c, p) => c == "0"
                                        ? string.Format("{0}|{1}", "H", c)
                                        : (double.Parse(c) < double.Parse(p)
                                            ? string.Format("{0}|{1}", "H", c)
                                            : string.Format("{0}|{1}", "C", p)))),
                        new XElement("BetIOR",
                            data.OrderBy(s => s.T)
                                .Select(s => s.C.ToString())
                                .Aggregate((c, n) => c + "|" + n))));
                }
                if (eventItem.Any(s => s.G == 17)) //大小球
                {
                    data = eventItem.Where(s => s.G == 17);
                    xml.Add(new XElement("BetInfo",
                        new XElement("BetType", (int) PlayEnum.RollBallZiseAll),
                        new XElement("BetKey", string.Format("{0}", eventItem.FirstOrDefault(s => s.G == 17).P)),
                        new XElement("BetIOR",
                            data
                                .OrderBy(s => s.T)
                                .Select(s => s.C.ToString())
                                .Aggregate((c, n) => c + "|" + n))));
                }
                return xml.ToString(SaveOptions.DisableFormatting);
            }
            else
                return string.Empty;
        }

        [TestMethod]
        public void XBETGAME()
        {
            FT.Task.FTPlayMatchLogJob GAME = new FT.Task.FTPlayMatchLogJob();
            //GAME.ExecInitLog();
        }

        [TestMethod]
        public void GetRedisConfig()
        {
            string str = string.Empty;
            var result = HttpHelper.SimplePost<ResultMatch>("https://1x-bet-bk.com/getTranslate/ViewGameResults",
                "{\"Language\":\"cn\"}{\"Params\":[null, null, \"1\", null, null]}", ref str);
        }

        [TestMethod]
        public void GetTimeZone()
        {
            var language = "en";
            var listzone = ParsePageByArea(language);
            var changeZone = listzone.Select(s => new SystemDictionary
            {
                ParentId = 1,
                DictionaryValue = s.Zone,
                DictionaryName = string.Format("{0}{1}", s.ZoneTitle, s.ZoneText.Trim())
            });
            var notexists = new List<SystemDictionary>();
            using (var context = new Entity())
            {
                var existsZone = context.SystemDictionary.Where(s => s.ParentId == 1).ToArray();
                if (existsZone.Any())
                {
                    foreach (var item in existsZone)
                    {
                        foreach (var items in changeZone)
                        {
                            if (item.ParentId == items.ParentId && item.DictionaryValue == items.DictionaryValue)
                            {
                                switch (language)
                                {
                                    case "en":
                                        context.SystemDictionary.Where(s => s.Id == item.Id).Update(u => new SystemDictionary
                                        {
                                            DictionaryNameEn = items.DictionaryName
                                        });
                                        break;
                                    case "pt":
                                        context.SystemDictionary.Where(s => s.Id == item.Id).Update(u => new SystemDictionary
                                        {
                                            DictionaryNamePt = items.DictionaryName
                                        });
                                        break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    notexists = changeZone.ToList();
                }
                if (notexists.Any())
                {
                    context.SystemDictionary.AddRange(changeZone);
                    context.SaveChanges();
                }
            }
        }

        private static string GetWebClient(string url)
        {
            string strHTML = "";
            WebClient myWebClient = new WebClient();
            Stream myStream = myWebClient.OpenRead(url);
            StreamReader sr = new StreamReader(myStream, Encoding.UTF8); //注意编码
            strHTML = sr.ReadToEnd();
            myStream.Close();
            return strHTML;
        }

        private static IEnumerable<TimeZoneCity> ParsePageByArea(String cityCode)
        {
            var zone = new List<TimeZoneCity>();
            //更加链接格式和省份代码构造URL
            String url = String.Format("https://1x-bet-bk.com/{0}/", cityCode);
            //下载网页源代码 
            var docText = GetWebClient(url);
            //加载源代码，获取文档对象
            var doc = new HtmlDocument();
            doc.LoadHtml(docText);
            //更加xpath获取总的对象，如果不为空，就继续选择dl标签
            var res =
                doc.DocumentNode.SelectSingleNode(
                    @"/html[1]/body[1]/div[1]/div[1]/div[1]/noindex[1]/div[1]/div[1]/div[1]/ul[1]");
            if (res != null)
            {
                var list = res.SelectNodes(@"li"); //选择标签数组
                if (list.Count < 1) return new List<TimeZoneCity>();
                zone.AddRange(list.Select(item => new TimeZoneCity
                {
                    ZoneTitle = item.SelectSingleNode(@"span").InnerText,
                    Zone = double.Parse(item.Attributes["data-value"].Value),
                    ZoneText =
                        item.Elements("#text").ElementAtOrDefault(1) == null
                            ? item.Elements("#text").ElementAtOrDefault(0).InnerText
                            : item.Elements("#text").ElementAtOrDefault(1).InnerText
                }));
            }
            return zone;
        }
    }

    class TimeZoneCity
    {
        public double Zone { get; set; }
        public string ZoneTitle { get; set; }
        public string ZoneText { get; set; }
    }
}
