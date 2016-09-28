using System;
using FT.Model;
using FT.Utility.Helper;
using Quartz;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using FT.Entities;
using EntityFramework.Extensions;
using System.Linq;
using FT.Utility.CacheHelper;
using System.Configuration;

namespace FT.Task
{
    /// <summary>
    /// 足彩数据抓取Job
    /// </summary>
    [DisallowConcurrentExecution]
    public class NewFTPlayResultJob : BaseJob, IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            AnalyzeHtml();
        }

        public void TestResult(string htmlCode)
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            var matchList = NewAnalyzeData.GetPlayResultData(htmlCode, date);
        }
        public void AnalyzeHtml()
        {
            Log.Debug("FTPlayResultJob  抓取开始");

            var list = new List<MatchInfo>();

            var _FootBall_GQ = new List<BZ_Gamehead>();

            if (string.IsNullOrEmpty(AspNetCache.Get<string>(CacheKeyCollection.LoginJobUid)))
            {
                loginHG();
            }

            if (AspNetCache.Get<string>(CacheKeyCollection.LoginJobUid) != "")
            {
                List<MatchResult> listMatchResult = new List<MatchResult>();

                #region 获取最近2天的比赛结果

                for (var i = 0; i < 7; i++)
                {
                    string date = DateTime.Now.AddDays(-7 + i).ToString("yyyy-MM-dd");//比赛结果日期
                    string url = "";
                    string htmlCode = GetPlayResultHtml(date, ref url);//获得网页脚本
                    Log.Info("地址：" + url + "结果：" + htmlCode, null);//记录日志
                    var matchList = NewAnalyzeData.GetPlayResultData(htmlCode, date);//解析结果
                    listMatchResult.AddRange(matchList);
                }
                #endregion 获取最近2天的比赛结果

                //比赛结果录入数据库

                #region 业务入库实现代码

                List<MatchResult> resultList = new List<MatchResult>();

                if (listMatchResult != null && listMatchResult.Count > 0)
                {

                    List<MatchInfo> allMatchList = new List<MatchInfo>();
                    QueryDb((context) =>
                    {
                        allMatchList = context.MatchInfo.Where(s => s.MatchId != 0).ToList();
                    });

                    //比赛信息集合
                    List<MatchInfo> editMatchList = null;
                    if (allMatchList.Any())
                    {
                        var tempList = allMatchList.FindAll(w => listMatchResult.Select(s => s.MatchNumber).Contains(w.MatchNumber));
                        if (tempList.Any())
                        {
                            editMatchList = tempList.ToList();
                        }
                    }
                    foreach (var result in listMatchResult)
                    {
                        //比赛id转换
                        if (editMatchList != null && editMatchList.Count > 0)
                        {
                            var temp = editMatchList.Find(p => p.MatchNumber == result.MatchNumber);
                            if (temp != null)
                            {
                                long matchId = temp.MatchId;
                                result.MatchId = matchId;
                                resultList.Add(result);
                            }
                        }
                    }
                    listMatchResult = resultList;
                }

                foreach (var item in listMatchResult)
                {
                    List<MatchResult> old_MatchResult = new List<MatchResult>();
                    QueryDb((context) =>
                    {
                        old_MatchResult = context.MatchResult.Where(s => s.MatchId == item.MatchId).ToList();
                    });
                    if (old_MatchResult != null && old_MatchResult.Count > 0)
                    {
                        QueryDb((context) =>
                        {
                            context.MatchResult.Update(w => w.MatchId == item.MatchId, u => new MatchResult
                            {
                                ResultTime = item.ResultTime,
                                Result1 = item.Result1,
                                Result2 = item.Result2
                            });
                        });
                    }
                    else
                    {
                        QueryDb((context) =>
                        {
                            context.MatchResult.Add(item);
                            context.SaveChanges();
                        });
                    }
                }

                #endregion 业务入库实现代码
            }

            Log.Debug("FTPlayResultJob 抓取结束");
        }

        /// <summary>
        /// 登录到网站
        /// </summary>
        private void loginHG()
        {
            string host = ConfigurationManager.AppSettings["LoginURL"];
            string username = ConfigurationManager.AppSettings["UserName"];
            string pwd = ConfigurationManager.AppSettings["PassWord"];

            if (!string.IsNullOrEmpty(host) && !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(pwd))
            {
                string script = HTMLHelper.OldGetData(string.Format("{0}/app/member/new_login.php?username={1}&passwd={2}&langx=zh-cn", host, username, pwd));

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
        private string GetPlayResultHtml(string date,ref string url)
        {
            string uid = AspNetCache.Get<string>(CacheKeyCollection.LoginJobUid);
            string host = ConfigurationManager.AppSettings["NewGetFtRuseltDataURL"];//取赛果的地址

            if (uid == "")
            {
                loginHG();
            }

            if (!string.IsNullOrEmpty(host) && !string.IsNullOrEmpty(uid))
            {
                url = string.Format("{0}/app/member/result/result.php?game_type=FT&list_date={1}&uid={2}&langx=zh-cn", host, date, uid);
                return HTMLHelper.OldGetData(url);
            }

            return "";
        }
    }
}
