using EntityFramework.Extensions;
using FT.Entities;
using FT.Model;
using FT.Utility.CacheHelper;
using FT.Utility.Helper;
using Quartz;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace FT.Task
{
    /// <summary>
    /// 赛事玩法数据采集Job
    /// </summary>
    [DisallowConcurrentExecution]
    public class OldPlayMethodJob : BaseJob, IJob
    {
        private static int tatolpage_no;//总页数
        public void Execute(IJobExecutionContext context)
        {
            AnalyzeHtml();
        }

        public void AnalyzeHtml()
        {
            Log.Debug("PlayMethodJob 抓取开始");
           
            if (string.IsNullOrEmpty(AspNetCache.Get<string>(CacheKeyCollection.LoginJobUid)))
            {
                loginHG();
            }

            QueryDb((context) =>
            {
                var list = new List<MatchInfo>();
                //数据采集 集合转换
                var _FootBall_GQ = new List<BZ_Gamehead>();

                if (AspNetCache.Get<string>(CacheKeyCollection.LoginJobUid) != "")
                {
                    var matchInfoList = new List<MatchInfo>();

                    var ZHCN_listMatchInfo = GetHGDataForList("zh-cn");
                    var ENUS_listMatchInfo = GetHGDataForList("en-us");//先去英文不然取不到数据
 
                    try
                    {
                        var NoTestZHCN_MatchInfo = ZHCN_listMatchInfo.Where(p => !p.LeagueName.Contains("测试") || !p.HTeam.Contains("TEST") || !p.CTeam.Contains("TEST")).GroupBy(c => c.MatchNumber);
 
                        foreach (var matchnum in NoTestZHCN_MatchInfo)
                        {
                            var itemCn = ZHCN_listMatchInfo.FirstOrDefault(c => c.MatchNumber == matchnum.Key);
                            //var itemEn = ENUS_listMatchInfo.FirstOrDefault(c => c.MatchNumber == matchnum.Key && c.CTeamEn != null && c.HTeamEn != null && c.LeagueNameEn != null);

                            #region 合并比赛玩法处理
                            var matchList = ZHCN_listMatchInfo.FindAll(p => p.MatchNumber == itemCn.MatchNumber);

                            if (matchList != null && matchList.Count > 0)
                            {
                                List<BetInfo> listBetInfo = new List<BetInfo>();//玩法xml集合
                                string betType = "";//玩法类型

                                //获得比赛Odds

                                foreach (var match in matchList)
                                {
                                    var betList = (List<BetInfo>)XmlHelper.Deserialize(typeof(List<BetInfo>), match.Odds.Replace("MatchContent", "ArrayOfBetInfo"));
                                    foreach (var _betlist in betList)
                                    {
                                        string btype = _betlist.BetType.Substring(0, 1);

                                        //玩法类型追加
                                        if (!("|" + betType + "|").Contains("|" + btype + "|"))
                                        {
                                            betType += "|" + btype;
                                        }

                                        //判断是否同一个玩法，
                                        var FindbetType = listBetInfo.Find(a => a.BetType == _betlist.BetType && a.BetKey == _betlist.BetKey);
                                        if (FindbetType != null)
                                        {
                                            listBetInfo.Remove(FindbetType);//移除相同的玩法
                                            //FindbetType.BetIOR = _betlist.BetIOR;
                                        }

                                        var temp = listBetInfo.Find(p => p.BetType == _betlist.BetType && p.BetKey == _betlist.BetKey && p.BetIOR == _betlist.BetIOR);
                                        //不存在的玩法录入总的玩法集合
                                        if (temp == null)
                                        {
                                            listBetInfo.Add(_betlist);
                                        }
                                    }

                                    if (match.MinLimit.HasValue && match.MinLimit.Value > 0)
                                    {
                                        itemCn.MinLimit = match.MinLimit;
                                    }
                                    if (match.MaxLimit.HasValue && match.MaxLimit.Value > 0)
                                    {
                                        itemCn.MaxLimit = match.MaxLimit;
                                    }
                                }
                                if (listBetInfo == null || listBetInfo.Count == 0)
                                {
                                    continue;
                                }
                                string Odds = XmlHelper.Serializer(typeof(List<BetInfo>), listBetInfo)
                                    .Replace("<?xml version=\"1.0\"?>", "<MatchContent>")
                                    .Replace("<ArrayOfBetInfo xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">", "")
                                    .Replace("</ArrayOfBetInfo>", "</MatchContent>");

                                itemCn.Odds = Odds.ToString().Replace("\r\n", "").Replace(" ", "");
                                itemCn.BetType = betType.TrimStart('|');

                                //itemCn.HTeamEn = itemEn.HTeamEn;
                                //itemCn.CTeamEn = itemEn.CTeamEn;
                                //itemCn.LeagueNameEn = itemEn.LeagueNameEn;

                                matchInfoList.Add(itemCn);
                            }
                            #endregion
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Debug("合并比赛玩法处理" + ex.Message, ex);
                    }

                    ZHCN_listMatchInfo = matchInfoList.OrderBy(p => p.MatchNumber).ToList(); //集合重新排序到数据库

                    #region 业务入库实现代码
                    try
                    {
                        #region 比赛信息录入日志表

                        List<MatchInfoLog> matchInfoLogList = new List<MatchInfoLog>();
                        DateTime _today = Convert.ToDateTime(DateTime.Now.ToShortDateString());

                        var oldMatchList = context.MatchInfo.Where(s => s.MatchId != 0 && s.CreateDate > _today).ToList();//当天时间判断

                        foreach (var match in oldMatchList)
                        {
                            int version = context.MatchInfoLog.Where(s => s.MatchNumber == match.MatchNumber).ToList().Count + 1;

                            MatchInfoLog matchLog = new MatchInfoLog();
                            matchLog.Version = version;//版本号
                            matchLog.MatchId = match.MatchId;
                            matchLog.LeagueName = match.LeagueName;
                            matchLog.LeagueNameEn = match.LeagueNameEn;//英文
                            matchLog.MatchDate = match.MatchDate;
                            matchLog.MatchNumber = match.MatchNumber;
                            matchLog.MatchType = match.MatchType;
                            matchLog.MatchEndTime = match.MatchEndTime;
                            matchLog.HTeamNum = match.HTeamNum;
                            matchLog.HTeam = match.HTeam;
                            matchLog.HTeamEn = match.HTeamEn;//英文
                            matchLog.CTeamNum = match.CTeamNum;
                            matchLog.CTeam = match.CTeam;
                            matchLog.CTeamEn = match.CTeamEn;//英文
                            matchLog.BetType = match.BetType;
                            matchLog.Odds = match.Odds;
                            matchLog.OddsUpdateTime = match.OddsUpdateTime;

                            matchInfoLogList.Add(matchLog);
                        }
                        context.MatchInfoLog.AddRange(matchInfoLogList);
                        context.SaveChanges();

                        #endregion 比赛信息录入日志表
                    }
                    catch (Exception ex)
                    {
                        Log.Debug("比赛信息录入日志表" + ex.Message, ex);
                    }

                    try
                    {
                        #region 更新原比赛信息，录入不存在的比赛信息

                        var allMatchList = context.MatchInfo.Where(s => s.MatchId != 0).ToList();

                        //需要编辑的比赛信息集合
                        List<MatchInfo> editMatchList = null;
                        if (allMatchList.Any())
                        {
                            editMatchList = allMatchList.FindAll(w => ZHCN_listMatchInfo.Select(s => s.MatchNumber).Contains(w.MatchNumber));
                        }

                        List<MatchInfo> addMatchList = new List<MatchInfo>();
                        if (editMatchList != null)
                        {
                            foreach (var match in ZHCN_listMatchInfo)
                            {
                                var temp = editMatchList.Find(p => p.MatchNumber == match.MatchNumber);
                                if (temp != null)
                                {
                                    long matchId = temp.MatchId;

                                    DateTime updatetime = DateTime.Now;

                                    if (match.EventId == null || match.EventId.ToString() == "0")
                                    {
                                        context.MatchInfo.Update(w => w.MatchId == matchId, u => new MatchInfo
                                        {
                                            MatchDate = match.MatchDate,
                                            BetType = match.BetType,
                                            Odds = match.Odds,
                                            OddsUpdateTime = DateTimeHelper.DateTimeToUnixTimestamp(DateTime.Now, 4),//时间加4
                                            MinLimit = match.MinLimit,
                                            MaxLimit = match.MaxLimit,
                                            UpdateDate = updatetime
                                        });
                                    }
                                    else
                                    {
                                        //更新比赛信息
                                        context.MatchInfo.Update(w => w.MatchId == matchId, u => new MatchInfo
                                        {
                                            MatchDate = match.MatchDate,
                                            EventId = match.EventId,//不等于null || 0 时更新到数据库
                                            BetType = match.BetType,
                                            Odds = match.Odds,
                                            OddsUpdateTime = DateTimeHelper.DateTimeToUnixTimestamp(DateTime.Now, 4),//时间加4
                                            MinLimit = match.MinLimit,
                                            MaxLimit = match.MaxLimit,
                                            IsMaster = match.IsMaster,
                                            UpdateDate = updatetime
                                        });
                                    }
                                }
                                else
                                {
                                    //黑名单表（不存在则向比赛表添加比赛数据）DataSourceEnum.HGA
                                    if (context.MatchBlackList.Where(a => a.LeagueName == match.LeagueName.Trim() && a.IsBan == YesNoEnum.Yes && a.DataSource== DataSourceEnum.HGA).Count() != 1)
                                    {
                                        addMatchList.Add(match);
                                    }
                                }
                            }
                        }
                        else
                        {
                            addMatchList = ZHCN_listMatchInfo;
                        }

                        context.MatchInfo.AddRange(addMatchList);//录入不存在的比赛信息
                        context.SaveChanges();

                        #endregion 更新原比赛信息，录入不存在的比赛信息
                    }
                    catch (Exception ex)
                    {
                        Log.Debug("更新原比赛信息，录入不存在的比赛信息" + ex.Message, ex);
                    }

                    #endregion 业务入库实现代码

                    //英文字段录入ENUS_listMatchInfo

                    foreach (var enus in ENUS_listMatchInfo)
                    {
                        context.MatchInfo.Update(r => r.MatchNumber == enus.MatchNumber, p => new MatchInfo
                        {
                            LeagueNameEn = enus.LeagueName,
                            HTeamEn = enus.HTeam,
                            CTeamEn = enus.CTeam
                        });
                    }
                }
            });
            Log.Debug("PlayMethodJob 抓取结束");
        }

        /// <summary>
        /// 登录到网站
        /// </summary>
        private void loginHG()
        {
            //new  http://66.133.81.209
            //old  http://180.94.224.38

            string host = ConfigurationManager.AppSettings["LoginURL"];
            string username = ConfigurationManager.AppSettings["UserName"];
            string pwd = ConfigurationManager.AppSettings["PassWord"];

            if (!string.IsNullOrEmpty(host) && !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(pwd))
            {
                string result = HTMLHelper.OldGetData(host);
                bool isok = result.Contains("服务中断");
                if (isok) {
                    AspNetCache.Remove(CacheKeyCollection.LoginJobUid);
                    Log.Info("网站服务中断：" + host + "返回数据：" + result, null);
                    return;
                }
 
                string loginUrl = string.Format("{0}/app/member/new_login.php?username={1}&passwd={2}&langx=zh-cn", host, username, pwd);
                string script = HTMLHelper.OldGetData(loginUrl);
                 
                Log.Info("登录地址：" + loginUrl + "返回数据：" + script, null);

                string[] Llinks = script.Split('|');
                if (Llinks.Length >= 4)
                {
                    string uid = Llinks[3];

                    AspNetCache.Remove(CacheKeyCollection.LoginJobUid);
                    AspNetCache.Insert(CacheKeyCollection.LoginJobUid, uid, DateTime.Now.AddHours(2));//缓存时间是2小时
                }
            }
        }

        /// <summary>
        /// 获得网站数据
        /// </summary>
        /// <param name="rType">类型 example:[滚球]</param>
        private string GetHGScript(string rType, string page_no, string langx, ref string url)
        {
            string uid = AspNetCache.Get<string>(CacheKeyCollection.LoginJobUid);
            string host = ConfigurationManager.AppSettings["GetDataURL"].ToString();
            string html = "";

            if (uid == "")
            {
                loginHG();
            }

            if (!string.IsNullOrEmpty(host) && !string.IsNullOrEmpty(uid))
            {
                string s2 = string.Format("{0}/app/member/FT_browse/body_var.php?uid={1}&rtype={2}&langx={3}&mtype=3&page_no={4}&league_id=&hot_game=", host, uid, rType, langx, page_no);

                if (rType == "p3") {
                    //综合过关
                    s2 = string.Format("{0}/app/member/FT_browse/body_var.php?uid={1}&rtype={2}&langx={3}&mtype=3&page_no={4}&league_id=&hot_game=&showgtype=FU&g_date=ALL", host, uid, rType, langx, page_no);
                }
         
                url = s2;
                html = HTMLHelper.OldGetData(s2);

                const string GameHead = @"_.t_page(.*?);";
                Regex GHR = new Regex(GameHead, RegexOptions.IgnoreCase);
                MatchCollection MC = GHR.Matches(html);

                if (MC.Count == 0)//没有取到数据，将重新登录
                {
                    loginHG();
                    html = HTMLHelper.OldGetData(s2);
                }

                GHR = new Regex(GameHead, RegexOptions.IgnoreCase);
                MC = GHR.Matches(html);//从新验证

                if (MC.Count == 1)
                {
                    tatolpage_no = Convert.ToInt32(MC[0].ToString().Replace("_.t_page=", "").Trim(';')) - 1;//获得总页数
                }
            }

            return html;
        }

        public List<MatchInfo> GetHGDataForList(string language)
        {
            string[] rtypes = "r,p3,pd,t,f".Split(',');
            var listMatchInfo = new List<MatchInfo>();
            int page_no = 0;//分页号默认：0
            string url = "";

            foreach (var rtype in rtypes)
            {
                string htmlCode = GetHGScript(rtype, page_no.ToString(), language, ref url);//获得网页脚本
                int t_page_no = Convert.ToInt32(tatolpage_no);
                var listMatch = OldAnalyzeData.SetData(htmlCode, rtype,url);//解析数据

                listMatchInfo.AddRange(listMatch);

                if (page_no < t_page_no)
                {
                    for (int i = 1; i <= t_page_no; i++)
                    {
                        htmlCode = GetHGScript(rtype, i.ToString(), language, ref url);
                        listMatch = OldAnalyzeData.SetData(htmlCode, rtype,url);
                        listMatchInfo.AddRange(listMatch);
                    }
                }
            }

            listMatchInfo = listMatchInfo.OrderBy(p => p.MatchNumber).ToList();
            return listMatchInfo;
        }
    }
}