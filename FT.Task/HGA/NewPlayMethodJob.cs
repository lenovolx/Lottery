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
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FT.Task
{
    /// <summary>
    /// 新网站赛事玩法数据采集Job
    /// </summary>
    [DisallowConcurrentExecution]
    public class NewPlayMethodJob : BaseJob, IJob
    {
        private static int tatolpage_no;//总页数
        public void Execute(IJobExecutionContext context)
        {
            NewAnalyzeHtml();
        }

        public void NewAnalyzeHtml()
        {
            Log.Debug("NewPlayMethodJob 抓取开始");
            LoginNewHG();//登录到新网站

            List<MatchInfo> MatchInfos = allMatchInfo();//所有的数据集合，包括英文字段

            var _list = MergeMi(MatchInfos);//合并相同MatchNum的玩法

            //处理数据集合
            QueryDb((context) =>
            {
                foreach (var mis in _list)
                {
                    //查询数据库数据是否存在，存在则更新，不存在保存一条数据
                    var oldMatchInfo = context.MatchInfo.Where(a => a.MatchNumber == mis.MatchNumber).ToList();

                    if (oldMatchInfo != null && oldMatchInfo.Count > 0)
                    {
                        DateTime updatetime = DateTime.Now;

                        context.MatchInfo.Update(w => w.MatchId == oldMatchInfo[0].MatchId, u => new MatchInfo
                        {
                            MatchDate = oldMatchInfo[0].MatchDate,
                            EventId = oldMatchInfo[0].EventId,//不等于null || 0 时更新到数据库
                            BetType = oldMatchInfo[0].BetType,
                            Odds = oldMatchInfo[0].Odds,
                            OddsUpdateTime = DateTimeHelper.DateTimeToUnixTimestamp(DateTime.Now, 4),//时间加4
                            MinLimit = oldMatchInfo[0].MinLimit,
                            MaxLimit = oldMatchInfo[0].MaxLimit,
                            UpdateDate = updatetime
                        });
                    }
                    else
                    {
                        context.MatchInfo.Add(mis);
                    }
                }
            });

            Log.Debug("NewPlayMethodJob 抓取结束");
        }

        private List<MatchInfo> allMatchInfo()
        {
            List<MatchInfo> AllMatchInfoList = new List<MatchInfo>();

            string[] pageScripts = GetPageScript("r", "zh-cn");//获得同一个球类型的所有数据，包括分页
            List<New_BZP_Model> zhcn_rlist = NewAnalyzeData.DoStandard(pageScripts);//【中文】标准盘和滚球在同一个实体
            pageScripts = GetPageScript("r", "en-us");
            List<New_BZP_Model> enus_rlist = NewAnalyzeData.DoStandard(pageScripts);//【英文】标准盘和滚球在同一个实体

            pageScripts = GetPageScript("t", "zh-cn");
            List<New_ZRQ_Model> zhcn_tlist = NewAnalyzeData.DoOther<New_ZRQ_Model>("t", pageScripts);
            pageScripts = GetPageScript("t", "en-us");
            List<New_ZRQ_Model> enus_tlist = NewAnalyzeData.DoOther<New_ZRQ_Model>("t", pageScripts);

            pageScripts = GetPageScript("pd", "zh-cn");
            List<New_BD_Model> zhcn_bdlist = NewAnalyzeData.DoOther<New_BD_Model>("pd", pageScripts);
            pageScripts = GetPageScript("pd", "en-us");
            List<New_BD_Model> enus_bdlist = NewAnalyzeData.DoOther<New_BD_Model>("pd", pageScripts);

            pageScripts = GetPageScript("f", "zh-cn");
            List<New_BCQC_Model> zhcn_flist = NewAnalyzeData.DoOther<New_BCQC_Model>("f", pageScripts);
            pageScripts = GetPageScript("f", "en-us");
            List<New_BCQC_Model> enus_flist = NewAnalyzeData.DoOther<New_BCQC_Model>("f", pageScripts);

            pageScripts = GetPageScript("p3", "zh-cn");
            List<New_ZHGG_Model> zhcn_p3list = NewAnalyzeData.DoOther<New_ZHGG_Model>("p3", pageScripts);
            pageScripts = GetPageScript("p3", "en-us");
            List<New_ZHGG_Model> enus_p3list = NewAnalyzeData.DoOther<New_ZHGG_Model>("p3", pageScripts);

            List<MatchInfo> new_zhcn_bzp = NewAnalyzeData.BulidXMl_New_BZP(zhcn_rlist);
            List<MatchInfo> new_enus_bzp = NewAnalyzeData.BulidXMl_New_BZP(enus_rlist);
            AllMatchInfoList.AddRange(DoMi(new_zhcn_bzp, new_enus_bzp));

            List<MatchInfo> new_zhcn_zrq = NewAnalyzeData.BulidXMl_New_ZRQ(zhcn_tlist);
            List<MatchInfo> new_enus_zrq = NewAnalyzeData.BulidXMl_New_ZRQ(enus_tlist);
            AllMatchInfoList.AddRange(DoMi(new_zhcn_zrq, new_enus_zrq));

            List<MatchInfo> new_zhcn_bd = NewAnalyzeData.BulidXMl_New_BD(zhcn_bdlist);
            List<MatchInfo> new_enus_bd = NewAnalyzeData.BulidXMl_New_BD(enus_bdlist);
            AllMatchInfoList.AddRange(DoMi(new_zhcn_bd, new_enus_bd));

            List<MatchInfo> new_zhcn_bcqc = NewAnalyzeData.BulidXMl_New_BCQC(zhcn_flist);
            List<MatchInfo> new_enus_bcqc = NewAnalyzeData.BulidXMl_New_BCQC(enus_flist);
            AllMatchInfoList.AddRange(DoMi(new_zhcn_bcqc, new_enus_bcqc));

            List<MatchInfo> new_zhcn_zhgg = NewAnalyzeData.BulidXMl_New_HZGG(zhcn_p3list);
            List<MatchInfo> new_enus_zhgg = NewAnalyzeData.BulidXMl_New_HZGG(enus_p3list);
            AllMatchInfoList.AddRange(DoMi(new_zhcn_zhgg, new_enus_zhgg,true));

            return AllMatchInfoList;
        }

        //取页面脚本数据，包括分页以后的页面集合
        private string[] GetPageScript(string rtype, string language)
        {
            //先取第一个数据，然后取页面最大分页号
            int page_no = 0;//分页号默认：0
            string htmlCode = GetHGScript(rtype, page_no, language);//获得网页脚本
            int t_page_no = tatolpage_no;
            string[] pageScripts = new String[t_page_no + 1];//:TODO

            if (page_no < t_page_no)
            {
                for (int i = 1; i <= t_page_no; i++)
                {
                    htmlCode = GetHGScript(rtype, i, language);
                    pageScripts[i] = htmlCode;
                }
            }
            else
            {
                pageScripts[0] = htmlCode;
            }

            return pageScripts;
        }

        /// 登录到网站
        private void LoginNewHG()
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

        // 获得网站数据(rType:类型，page_no：页码，langx：语言)
        private string GetHGScript(string rType, int page_no, string langx = "zh-cn")
        {
            string uid = AspNetCache.Get<string>(CacheKeyCollection.LoginJobUid);
            string host = ConfigurationManager.AppSettings["NewGetDataURL"].ToString();

            string html = "";

            if (uid == "")
            {
                LoginNewHG();//获得页面脚本时，过期登录
            }

            if (!string.IsNullOrEmpty(host) && !string.IsNullOrEmpty(uid))
            {
                string s2 = string.Format("{0}/app/member/FT_browse/body_var.php?uid={1}&rtype={2}&langx={3}&mtype=3&page_no={4}&league_id=&hot_game=", host, uid, rType, langx, page_no.ToString());

                if (rType == "p3") {
                    s2 = string.Format("{0}/app/member/FT_browse/body_var.php?uid={1}&rtype={2}&langx={3}&mtype=3&page_no={4}&league_id=&hot_game=&showgtype=FU&g_date=ALL&isie11=%27N%27", host, uid, rType, langx, page_no.ToString());
                }


                html = HTMLHelper.NewGetData(s2);

                const string GameHead = @"_.t_page(.*?);";//_.t_page=1;
                Regex GHR = new Regex(GameHead, RegexOptions.IgnoreCase);
                MatchCollection MC = GHR.Matches(html);

                if (MC.Count == 0)//没有取到数据，将重新登录
                {
                    LoginNewHG();
                    html = HTMLHelper.NewGetData(s2);
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

        //去掉<MatchContent></MatchContent>
        private string Rodds(string odds)
        {
            return odds.Replace("<MatchContent>", "").Replace("</MatchContent>", "");
        }

        private List<MatchInfo> DoMi(List<MatchInfo> new_zhcn, List<MatchInfo> new_enus,bool isp3=false)
        {
            List<MatchInfo> listmi = new List<MatchInfo>();
            for (int i = 0; i < new_zhcn.Count; i++)
            {
                MatchInfo mi_new = new MatchInfo();
                int gid = new_zhcn[i].MatchNumber;
                for (int j = 0; j < new_enus.Count; j++)
                {
                    if (new_enus[j].MatchNumber == gid)
                    {
                        mi_new.LeagueNameEn = new_enus[j].LeagueName;
                        mi_new.HTeamEn = new_enus[j].HTeam;
                        mi_new.CTeamEn = new_enus[j].CTeam;
                    }
                }

                mi_new.LeagueName = new_zhcn[i].LeagueName;
                mi_new.MatchDate = new_zhcn[i].MatchDate;
                mi_new.MatchNumber = new_zhcn[i].MatchNumber;
                mi_new.EventId = new_zhcn[i].EventId;
                mi_new.MatchType = new_zhcn[i].MatchType;
                mi_new.MatchEndTime = new_zhcn[i].MatchEndTime;
                mi_new.HTeamNum = new_zhcn[i].HTeamNum;
                mi_new.HTeam = new_zhcn[i].HTeam;
                mi_new.CTeamNum = new_zhcn[i].CTeamNum;
                mi_new.CTeam = new_zhcn[i].CTeam;
                mi_new.BetType = new_zhcn[i].BetType;
                mi_new.CreateDate = new_zhcn[i].CreateDate;
                mi_new.OddsUpdateTime = new_zhcn[i].OddsUpdateTime;//:TODO  +4(在后面的代码加4)
                mi_new.Odds = Rodds(new_zhcn[i].Odds);

                if (isp3) {
                    mi_new.MaxLimit = new_zhcn[i].MaxLimit;
                    mi_new.MinLimit = new_zhcn[i].MinLimit;
                }

                listmi.Add(mi_new);
            }
            return listmi;
        }

        private List<MatchInfo> MergeMi(List<MatchInfo> MatchInfos)
        {
            List<MatchInfo> matchInfoList = new List<MatchInfo>();
            var miGroup = MatchInfos.Where(p => !p.LeagueName.Contains("测试") || !p.HTeam.Contains("TEST") || !p.CTeam.Contains("TEST")).GroupBy(a => a.MatchNumber).ToList();

            try
            {
                foreach (var matchnum in miGroup)
                {
                    var mat_FOD = MatchInfos.FirstOrDefault(c => c.MatchNumber == matchnum.Key);
                    var mat_Find = MatchInfos.FindAll(c => c.MatchNumber == matchnum.Key);

                    string bettype = "";
                    string odds = "";
                    long? eventid = null;

                    foreach (var mis in mat_Find)
                    {
                        if (mis.EventId > 0 && mis.EventId.HasValue && eventid==null)
                        {
                            eventid = mis.EventId;
                        }

                        bettype += mis.BetType;
                        odds += mis.Odds;

                        if (mis.MinLimit.HasValue && mis.MinLimit.Value > 0)
                        {
                            mat_FOD.MinLimit = mis.MinLimit;
                        }
                        if (mis.MaxLimit.HasValue && mis.MaxLimit.Value > 0)
                        {
                            mat_FOD.MaxLimit = mis.MaxLimit;
                        }
                    }

                    mat_FOD.BetType = bettype.TrimEnd('|');
                    if (eventid == null) {
                        mat_FOD.EventId = 0;
                    }

                    odds = "<MatchContent>" + odds + "</MatchContent>";
                    mat_FOD.Odds = odds;

                    //黑名单查询
                    QueryDb((context) =>
                    {
                         if (context.MatchBlackList.Where(a => a.LeagueName == mat_FOD.LeagueName.Trim() && a.IsBan == YesNoEnum.Yes).Count() != 1)
                         {
                             matchInfoList.Add(mat_FOD);
                         }
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Debug("合并比赛玩法处理" + ex.Message, ex);
            }

            return matchInfoList;
        }
    }
}
