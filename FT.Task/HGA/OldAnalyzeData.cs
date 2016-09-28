using FT.Model;
using FT.Utility.Helper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using FT.Utility;

namespace FT.Task
{
    public class OldAnalyzeData
    {
        #region 比赛信息

        /// <summary>
        /// 生成数据到集合并解析成xml格式
        /// </summary>
        /// <param name="html">脚本文件</param>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static List<MatchInfo> SetData(string html, string rtype, string url)
        {
            Log.Info("地址："+url+"  内容： " + html, null);//将html 脚本写到日志文件

            const string GameHead = @"_.GameHead(.*?)];";
            Regex GHR = new Regex(GameHead, RegexOptions.IgnoreCase);
            MatchCollection MC = GHR.Matches(html);
            List<BZ_Gamehead> list = new List<BZ_Gamehead>();
            if (MC.Count > 0)
            {
                string Headlink = MC[0].ToString().Replace("_.GameHead =[", "").Trim(';').Trim(']');

                if (rtype == "p3")
                {
                    Headlink += ",'hot'";
                }
                string[] GMList = Headlink.Split(',');

                for (int i = 0; i < GMList.Length; i++)
                {
                    string strToModel = GMList[i].TrimStart('\'').TrimEnd('\'');
                    GMList[i] = strToModel;
                }

                string _html = html.Replace("g([", "_XXX1XXXXXXbegin").Replace("]);", "_XXX1XXXXXXend");

                const string GameFT = @"_XXX1XXXXXXbegin(.*?)_XXX1XXXXXXend";
                Regex r = new Regex(GameFT, RegexOptions.IgnoreCase);
                MatchCollection m = r.Matches(_html);

                for (int i = 0; i < m.Count; i++)
                {
                    string link = m[i].ToString().Replace("_XXX1XXXXXXbegin", "").Replace("_XXX1XXXXXXend", "");
                    //link = link.Trim(';').Trim(')');

                    string[] JContent = link.Split(',');

                    StringBuilder _GamejsonStr = new StringBuilder("{");

                    if (GMList.Length > JContent.Length)
                    {
                        for (int j = 0; j < JContent.Length; j++)
                        {
                            JContent[j] = "\"" + JContent[j].TrimStart('\'').TrimEnd('\'') + "\"";
                            JContent[j] = "\"" + GMList[j] + "\":" + JContent[j];
                            _GamejsonStr.Append(JContent[j] + ",");
                        }
                    }
                    else
                    {
                        int gmLength=GMList.Length;
 
                        for (int j = 0; j <gmLength; j++)
                        {
                            JContent[j] = "\"" + JContent[j].TrimStart('\'').TrimEnd('\'') + "\"";
                            JContent[j] = "\"" + GMList[j] + "\":" + JContent[j];

                            if (rtype == "r")
                            {
                                JContent[35] = "\"" + GMList[35] + "\":" + JContent[39];//eventid
                                JContent[36] = "\"" + GMList[36] + "\":" + JContent[40];//hot{ismater}
                            }
                            _GamejsonStr.Append(JContent[j] + ",");
                        }
                    }

                    string GamejsonStr = _GamejsonStr.ToString().Trim('"').Trim(',') + "}";

                    BZ_Gamehead model22 = FT.Utility.Helper.HTMLHelper.ToModel<BZ_Gamehead>(GamejsonStr);

                    list.Add(model22);
                }
            }

            return BulidXMl2(list, rtype);
        }

        /// <summary>
        /// XElement组装XML （在用）
        /// </summary>
        /// <param name="list">数据集合</param>
        /// <param name="rtype">类型</param>
        /// <returns></returns>
        public static List<MatchInfo> BulidXMl2(List<BZ_Gamehead> list, string rtype)
        {
            List<MatchInfo> listMatchInfo = new List<MatchInfo>();

            foreach (var i in list)
            {
                XElement XMatchContent = new XElement("MatchContent");
                StringBuilder sbBetType = new StringBuilder();
                MatchInfo mi = new MatchInfo();

                if (rtype == "r")
                {
                    sbBetType.Append("1|");
                    #region 独赢玩法，全场ior_MH，ior_MC，ior_MN
                    if (!string.IsNullOrEmpty(i.ior_MH) && !string.IsNullOrEmpty(i.ior_MC) && !string.IsNullOrEmpty(i.ior_MN))
                    {
                        XElement XBetInfo = new XElement("BetInfo",
                             new XElement("BetType", (int)PlayEnum.OnlyWinAll),
                             new XElement("BetKey", "H|C|N"),
                             new XElement("BetIOR", i.ior_MH + "|" + i.ior_MC + "|" + i.ior_MN));
                        XMatchContent.Add(XBetInfo);
                    }

                    #endregion 独赢玩法，全场ior_MH，ior_MC，ior_MN

                    #region 独赢玩法，半场ior_HMH,ior_HMC,ior_HMN

                    if (!string.IsNullOrEmpty(i.ior_HMH) && !string.IsNullOrEmpty(i.ior_HMC) && !string.IsNullOrEmpty(i.ior_HMN))
                    {
                        XElement XBetInfo = new XElement("BetInfo",
                            new XElement("BetType", (int)PlayEnum.OnlyWinHalf),
                            new XElement("BetKey", "H|C|N"),
                            new XElement("BetIOR", i.ior_HMH + "|" + i.ior_HMC + "|" + i.ior_HMN));
                        XMatchContent.Add(XBetInfo);
                    }

                    #endregion 独赢玩法，半场ior_HMH,ior_HMC,ior_HMN

                    #region 让球玩法，全场ratio	ior_RH，ior_RC

                    if (!string.IsNullOrEmpty(i.ratio) && !string.IsNullOrEmpty(i.ior_RH) && !string.IsNullOrEmpty(i.ior_RC))
                    {
                        string ior = (Convert.ToDouble(i.ior_RH) + 1) + "|" + (Convert.ToDouble(i.ior_RC) + 1);//赔率+1

                        XElement XBetInfo = new XElement("BetInfo",
                            new XElement("BetType", (int)PlayEnum.LetBallAll),
                            new XElement("BetKey", i.strong + "|" + i.ratio),
                            new XElement("BetIOR", ior));
                        XMatchContent.Add(XBetInfo);
                    }

                    #endregion 让球玩法，全场ratio	ior_RH，ior_RC

                    #region 让球玩法，半场hstrong，hratio,ior_HRH,ior_HRC

                    if (!string.IsNullOrEmpty(i.hratio) && !string.IsNullOrEmpty(i.ior_HRH) && !string.IsNullOrEmpty(i.ior_HRC))
                    {
                        string ior = (Convert.ToDouble(i.ior_HRH) + 1) + "|" + (Convert.ToDouble(i.ior_HRC) + 1);//赔率+1

                        XElement XBetInfo = new XElement("BetInfo",
                            new XElement("BetType", (int)PlayEnum.LetBallHalf),
                            new XElement("BetKey", i.strong + "|" + i.hratio),
                            new XElement("BetIOR", ior));
                        XMatchContent.Add(XBetInfo);
                    }

                    #endregion 让球玩法，半场hstrong，hratio,ior_HRH,ior_HRC

                    #region 大小球玩法，全场ratio_o,ratio_u,ior_OUH,ior_OUC（大球，小球，小球赔率，大球赔率）

                    if (!string.IsNullOrEmpty(i.ratio_o) && !string.IsNullOrEmpty(i.ratio_u) && !string.IsNullOrEmpty(i.ior_OUH) && !string.IsNullOrEmpty(i.ior_OUC))
                    {
                        string ratio = "";
                        if (i.ratio_o.TrimStart('O') == i.ratio_u.TrimStart('U'))
                        {
                            ratio = i.ratio_o.TrimStart('O');
                        }

                        string ior = (Convert.ToDouble(i.ior_OUC) + 1) + "|" + (Convert.ToDouble(i.ior_OUH) + 1);//赔率+1

                        XElement XBetInfo = new XElement("BetInfo",
                            new XElement("BetType", (int)PlayEnum.BallZiseAll),
                            new XElement("BetKey", ratio),
                            new XElement("BetIOR", ior));
                        XMatchContent.Add(XBetInfo);
                    }

                    #endregion 大小球玩法，全场ratio_o,ratio_u,ior_OUH,ior_OUC（大球，小球，小球赔率，大球赔率）

                    #region 大小球玩法，半场 hratio_o，hratio_u，ior_HOUH，ior_HOUC（大球头，小球头，小球头赔率，大球头赔率）


                    if (!string.IsNullOrEmpty(i.hratio_o) && !string.IsNullOrEmpty(i.hratio_u) && !string.IsNullOrEmpty(i.ior_HOUH) && !string.IsNullOrEmpty(i.ior_HOUC))
                    {
                        string ratio = "";
                        if (i.hratio_o.TrimStart('O') == i.hratio_u.TrimStart('U'))
                        {
                            ratio = i.hratio_o.TrimStart('O');
                        }
                        string ior = (Convert.ToDouble(i.ior_HOUC) + 1) + "|" + (Convert.ToDouble(i.ior_HOUH) + 1);//赔率+1

                        XElement XBetInfo = new XElement("BetInfo",
                            new XElement("BetType", (int)PlayEnum.BallZiseHalf),
                            new XElement("BetKey", ratio),
                            new XElement("BetIOR", ior));
                        XMatchContent.Add(XBetInfo);
                    }

                    #endregion 大小球玩法，半场 hratio_o，hratio_u，ior_HOUH，ior_HOUC（大球头，小球头，小球头赔率，大球头赔率）

                    #region 单双玩法，全场 str_odd	str_even，ior_EOO，ior_EOE(单，双，单数赔率，双数赔率)

                    if (!string.IsNullOrEmpty(i.str_odd) && !string.IsNullOrEmpty(i.str_even) && !string.IsNullOrEmpty(i.ior_EOO) && !string.IsNullOrEmpty(i.ior_EOE))
                    {
                        XElement XBetInfo = new XElement("BetInfo",
                            new XElement("BetType", (int)PlayEnum.FirstsdAll),
                            new XElement("BetKey", "O|E"),
                            new XElement("BetIOR", i.ior_EOO + "|" + i.ior_EOE));
                        XMatchContent.Add(XBetInfo);
                    }

                    #endregion 单双玩法，全场 str_odd	str_even，ior_EOO，ior_EOE(单，双，单数赔率，双数赔率)
                }
                else if (rtype == "t")
                {
                    sbBetType.Append("2|");
                    #region 总入球玩法，全场
                    if (!string.IsNullOrEmpty(i.ior_T01) && !string.IsNullOrEmpty(i.ior_T23) && !string.IsNullOrEmpty(i.ior_T46) && !string.IsNullOrEmpty(i.ior_OVER))
                    {
                        XElement XBetInfo = new XElement("BetInfo",
                            new XElement("BetType", (int)PlayEnum.TotalBallAll),
                            new XElement("BetKey", "0-1|2-3|4-6|7+"),
                            new XElement("BetIOR", i.ior_T01 + "|" + i.ior_T23 + "|" + i.ior_T46 + "|" + i.ior_OVER));
                        XMatchContent.Add(XBetInfo);
                    }
                    #endregion 总入球玩法，全场
                }
                else if (rtype == "pd")
                {
                    sbBetType.Append("3|");
                    #region 波胆玩法 ，全场

                    if (!string.IsNullOrEmpty(i.ior_H1C0) && !string.IsNullOrEmpty(i.ior_H2C0) && !string.IsNullOrEmpty(i.ior_H2C1) && !string.IsNullOrEmpty(i.ior_H3C0) &&
                        !string.IsNullOrEmpty(i.ior_H3C1) && !string.IsNullOrEmpty(i.ior_H3C2) && !string.IsNullOrEmpty(i.ior_H4C0) && !string.IsNullOrEmpty(i.ior_H4C1) &&
                        !string.IsNullOrEmpty(i.ior_H4C2) && !string.IsNullOrEmpty(i.ior_H4C3) && !string.IsNullOrEmpty(i.ior_H0C0) && !string.IsNullOrEmpty(i.ior_H1C1) &&
                        !string.IsNullOrEmpty(i.ior_H2C2) && !string.IsNullOrEmpty(i.ior_H3C3) && !string.IsNullOrEmpty(i.ior_H4C4) && !string.IsNullOrEmpty(i.ior_OVH) &&
                        !string.IsNullOrEmpty(i.ior_H0C1) && !string.IsNullOrEmpty(i.ior_H0C2) && !string.IsNullOrEmpty(i.ior_H1C2) && !string.IsNullOrEmpty(i.ior_H0C3) &&
                        !string.IsNullOrEmpty(i.ior_H1C3) && !string.IsNullOrEmpty(i.ior_H2C3) && !string.IsNullOrEmpty(i.ior_H0C4) && !string.IsNullOrEmpty(i.ior_H1C4) &&
                        !string.IsNullOrEmpty(i.ior_H2C4) && !string.IsNullOrEmpty(i.ior_H3C4))
                    {
                        XElement XBetInfo = new XElement("BetInfo",
                            new XElement("BetType", (int)PlayEnum.CorrectScoreAll),
                            new XElement("BetKey", "1:0|2:0|2:1|3:0|3:1|3:2|4:0|4:1|4:2|4:3|0:0|1:1|2:2|3:3|4:4|OVH|0:1|0:2|1:2|0:3|1:3|2:3|0:4|1:4|2:4|3:4"),
                            new XElement("BetIOR", i.ior_H1C0 + "|" + i.ior_H2C0 + "|" + i.ior_H2C1 + "|" + i.ior_H3C0 + "|" + i.ior_H3C1 + "|" + i.ior_H3C2 + "|" + i.ior_H4C0 + "|" + i.ior_H4C1 + "|" + i.ior_H4C2 + "|" + i.ior_H4C3 + "|" + i.ior_H0C0 + "|" + i.ior_H1C1 + "|" + i.ior_H2C2 + "|" +
                            i.ior_H3C3 + "|" + i.ior_H4C4 + "|" + i.ior_OVH + "|" + i.ior_H0C1 + "|" + i.ior_H0C2 + "|" + i.ior_H1C2 + "|" + i.ior_H0C3 + "|" + i.ior_H1C3 + "|" + i.ior_H2C3 + "|" + i.ior_H0C4 + "|" + i.ior_H1C4 + "|" + i.ior_H2C4 + "|" + i.ior_H3C4));
                        XMatchContent.Add(XBetInfo);
                    }

                    #endregion 波胆玩法 ，全场
                }
                else if (rtype == "f")
                {
                    sbBetType.Append("4|");
                    #region 半全场

                    if (!string.IsNullOrEmpty(i.ior_FHH) && !string.IsNullOrEmpty(i.ior_FHN) && !string.IsNullOrEmpty(i.ior_FHC) && !string.IsNullOrEmpty(i.ior_FNH) &&
                        !string.IsNullOrEmpty(i.ior_FNN) && !string.IsNullOrEmpty(i.ior_FNC) && !string.IsNullOrEmpty(i.ior_FCH) && !string.IsNullOrEmpty(i.ior_FCN) &&
                        !string.IsNullOrEmpty(i.ior_FCC))
                    {
                        XElement XBetInfo = new XElement("BetInfo",
                              new XElement("BetType", (int)PlayEnum.HalfFull),
                              new XElement("BetKey", "HH|HN|HC|NH|NN|NC|CH|CN|CC"),
                              new XElement("BetIOR", i.ior_FHH + "|" + i.ior_FHN + "|" + i.ior_FHC + "|" + i.ior_FNH + "|" + i.ior_FNN + "|" + i.ior_FNC + "|" + i.ior_FCH + "|" + i.ior_FCN + "|" + i.ior_FCC));
                        XMatchContent.Add(XBetInfo);
                    }

                    #endregion 半全场
                }

                else if (rtype == "p3")
                {
                    sbBetType.Append("5|");
                    #region 综合独赢玩法，全场

                    if (!string.IsNullOrEmpty(i.ior_MH) && !string.IsNullOrEmpty(i.ior_MC) && !string.IsNullOrEmpty(i.ior_MN))
                    {
                        XElement XBetInfo = new XElement("BetInfo",
                            new XElement("BetType", (int)PlayEnum.ComOnlyWinAll),
                            new XElement("BetKey", "H|C|N"),
                            new XElement("BetIOR", i.ior_MH + "|" + i.ior_MC + "|" + i.ior_MN));
                        XMatchContent.Add(XBetInfo);
                    }

                    #endregion 综合独赢玩法，全场

                    #region 综合独赢玩法，半场

                    if (!string.IsNullOrEmpty(i.ior_HMH) && !string.IsNullOrEmpty(i.ior_HMC) && !string.IsNullOrEmpty(i.ior_HMN))
                    {
                        XElement XBetInfo = new XElement("BetInfo",
                            new XElement("BetType", (int)PlayEnum.ComOnlyWinHalf),
                            new XElement("BetKey", "H|C|N"),
                            new XElement("BetIOR", i.ior_HMH + "|" + i.ior_HMC + "|" + i.ior_HMN));
                        XMatchContent.Add(XBetInfo);
                    }

                    #endregion 综合独赢玩法，半场

                    #region 综合让球玩法，全场

                    if (!string.IsNullOrEmpty(i.ratio) && !string.IsNullOrEmpty(i.ior_PRH) && !string.IsNullOrEmpty(i.ior_PRC))
                    {
                        XElement XBetInfo = new XElement("BetInfo",
                            new XElement("BetType", (int)PlayEnum.ComLetBallAll),
                            new XElement("BetKey", i.strong + "|" + i.ratio),
                                new XElement("BetIOR", i.ior_PRH + "|" + i.ior_PRC));
                        XMatchContent.Add(XBetInfo);
                    }

                    #endregion 综合让球玩法，全场

                    #region 综合让球玩法，半场

                    if (!string.IsNullOrEmpty(i.hratio) && !string.IsNullOrEmpty(i.ior_HPRH) && !string.IsNullOrEmpty(i.ior_HPRC))
                    {
                        XElement XBetInfo = new XElement("BetInfo",
                            new XElement("BetType", (int)PlayEnum.ComLetBallHalf),
                                new XElement("BetKey", i.hstrong + "|" + i.hratio),
                            new XElement("BetIOR", i.ior_HPRH + "|" + i.ior_HPRC));
                        XMatchContent.Add(XBetInfo);
                    }

                    #endregion 综合让球玩法，半场

                    #region 综合大小球玩法，全场

                    if (!string.IsNullOrEmpty(i.ratio_o) && !string.IsNullOrEmpty(i.ratio_u) && !string.IsNullOrEmpty(i.ior_POUH) && !string.IsNullOrEmpty(i.ior_POUC))
                    {
                        string ratio = "";
                        if (i.ratio_o.TrimStart('O') == i.ratio_u.TrimStart('U'))
                        {
                            ratio = i.ratio_o.TrimStart('O');
                        }

                        XElement XBetInfo = new XElement("BetInfo",
                            new XElement("BetType", (int)PlayEnum.ComBallZiseAll),
                            new XElement("BetKey", ratio),
                                new XElement("BetIOR", i.ior_POUH + "|" + i.ior_POUC));
                        XMatchContent.Add(XBetInfo);
                    }

                    #endregion 综合大小球玩法，全场

                    #region 综合大小球玩法，半场 hratio_o，hratio_u，ior_HOUH，ior_HOUC（大球头，小球头，小球头赔率，大球头赔率）

                    if (!string.IsNullOrEmpty(i.hratio_o) && !string.IsNullOrEmpty(i.hratio_u) && !string.IsNullOrEmpty(i.ior_HPOUH) && !string.IsNullOrEmpty(i.ior_HPOUC))
                    {
                        string ratio = "";

                        if (i.hratio_o.TrimStart('O') == i.hratio_u.TrimStart('U'))
                        {
                            ratio = i.hratio_o.TrimStart('O');
                        }

                        XElement XBetInfo = new XElement("BetInfo",
                            new XElement("BetType", (int)PlayEnum.ComBallZiseHalf),
                            new XElement("BetKey", ratio),
                                new XElement("BetIOR", i.ior_HPOUH + "|" + i.ior_HPOUC));
                        XMatchContent.Add(XBetInfo);
                    }

                    #endregion 综合大小球玩法，半场 hratio_o，hratio_u，ior_HOUH，ior_HOUC（大球头，小球头，小球头赔率，大球头赔率）

                    #region 综合单双玩法，全场 str_odd	str_even，ior_EOO，ior_EOE(单，双，单数赔率，双数赔率)

                    if (!string.IsNullOrEmpty(i.ior_PEOO) && !string.IsNullOrEmpty(i.ior_PEOE))
                    {
                        XElement XBetInfo = new XElement("BetInfo",
                            new XElement("BetType", (int)PlayEnum.ComFirstsdAll),
                            new XElement("BetKey", "O|E"),
                                new XElement("BetIOR", i.ior_PEOO + "|" + i.ior_PEOE));
                        XMatchContent.Add(XBetInfo);
                    }

                    #endregion 综合单双玩法，全场 str_odd	str_even，ior_EOO，ior_EOE(单，双，单数赔率，双数赔率)
                }

                #region 数据处理逻辑

                string LeagueName = mi.LeagueName = i.league;//联赛名称
                string HTeam = mi.HTeam = i.team_h;//主队名称
                string HTeamNum = mi.HTeamNum = i.gnum_h;//主队编号
                string CTeam = mi.CTeam = i.team_c;//客队名称
                string CTeamNum = mi.CTeamNum = i.gnum_c;//客队编号
                int MatchNumber = mi.MatchNumber = Convert.ToInt32(i.gid);//比赛编号
                int MatchType = mi.MatchType = 0;

                long eventid = 0;
                if (rtype == "r")
                {
                    eventid = Convert.ToInt64(!string.IsNullOrEmpty(i.eventid) ? i.eventid.Replace(".", "") : "0");//标准场必须要《此作为关联数据》
                    if (i.hot == "Y")//||eventid==0
                    {
                        mi.IsMaster = 1;
                    }
                }
                if (rtype == "p3")
                {
                    eventid = Convert.ToInt64(string.IsNullOrEmpty(i.gidm) != true ? i.gidm.Replace(".", "") : "0");//标准场必须要《此作为关联数据》
                    if (i.hot == "Y")
                    {
                        mi.IsMaster = 1;
                    }
                    mi.MinLimit = Convert.ToInt32(i.par_minlimit);
                    mi.MaxLimit = Convert.ToInt32(i.par_maxlimit);
                }
 
                //07-28<br>01:00p<br><font color=red>Running Ball</font>06-30<br>12:00p
                bool isAfter = i.datetime.Contains("p");//是否为下午时间
                string time = i.datetime.Replace("<br>", "").IndexOf(':') == 6 ? "0" + i.datetime.Replace("<br>", "").Substring(5, 4) : i.datetime.Replace("<br>", "").Substring(5, 5);
                string ftTime = DateTime.Now.Year + "-" + i.datetime.Substring(0, 5) + " " + time;

                DateTime ftBeginTime = Convert.ToDateTime(ftTime);
         
                if (ftBeginTime.Hour >= 1 && ftBeginTime.Hour < 12 && isAfter)
                {
                    ftBeginTime = ftBeginTime.AddHours(12);
                }
               
                mi.MatchDate = DateTimeHelper.DateTimeToUnixTimestamp(ftBeginTime); //比赛时间 注意（去<br>）     ConvertDateTimeInt(System.DateTime time)

                mi.BetType = sbBetType.ToString().TrimEnd('|');
                mi.MatchEndTime = DateTimeHelper.DateTimeToUnixTimestamp(ftBeginTime.AddMinutes(90));
                mi.CreateDate = DateTime.Now;

                mi.Odds = XMatchContent.ToString(SaveOptions.DisableFormatting);
                mi.OddsUpdateTime = DateTimeHelper.DateTimeToUnixTimestamp(DateTime.Now, 0);
                mi.EventId = eventid;
               
                #endregion 数据处理逻辑

                listMatchInfo.Add(mi);
            }

            return listMatchInfo;
        }

        #endregion 比赛信息

        #region 比赛结果
        /// <summary>
        /// 获得比赛结果
        /// </summary>
        /// <param name="html">比赛脚本</param>
        /// <param name="date">时间</param>
        /// <returns></returns>
        public static List<MatchResult> GetPlayResultData(string html, string date)
        {
            string script = html;

            const string MacthInfo = @"var myleg = new Array(.*?);";
            Regex GHR = new Regex(MacthInfo, RegexOptions.IgnoreCase);
            MatchCollection MC = GHR.Matches(script);
            string link = "", htmltop = "";

            if (MC.Count > 0)
            {
                link = MC[0].ToString();
                htmltop = link.Replace("var myleg = new Array('',", "").Replace(");", "");
            }

            string[] htmltops = htmltop.Split(',');

            script = script.Replace(link, "");//去掉Array,用在后面

            List<MatchResult> matchResultList = new List<MatchResult>();

            for (int i = 0; i < htmltops.Length; i++)
            {
                string hr_tr = "<tr id=\"TR_1_" + htmltops[i].TrimStart('\'').TrimEnd('\'');//半场
                string full_tr = "<tr id=\"TR_2_" + htmltops[i].TrimStart('\'').TrimEnd('\'');//全场
 
                MatchResult matchResModel = new MatchResult();//比赛结果实体

                #region 获得比赛比分结果

                if (script.Contains(hr_tr) && script.Contains(full_tr))
                {
                    string one_tr = script.Substring(script.IndexOf(hr_tr), 300);//半场判断
                    if (one_tr.Contains("</tr>"))
                    {
                        one_tr = one_tr.Substring(0, (one_tr.IndexOf("</tr>") + 5));

                        string one_tr_regiexstr = @"span(.*?)</span";
                        Regex R_one_tr = new Regex(one_tr_regiexstr, RegexOptions.IgnoreCase);
                        MatchCollection MC_one_tr = R_one_tr.Matches(one_tr);
                        if (MC_one_tr.Count >= 2)
                        {
                            string one_tr_links1 = MC_one_tr[0].ToString();
                            int one_tr_index1 = one_tr_links1.IndexOf(">");
                            int one_tr_endindex1 = one_tr_links1.Length - one_tr_index1;
                            string one_tr_value1 = one_tr_links1.Substring(one_tr_index1, one_tr_endindex1).Replace("</span", "").TrimStart('>');//最后结果

                            string one_tr_links2 = MC_one_tr[1].ToString();
                            int one_tr_index2 = one_tr_links2.IndexOf(">");
                            int one_tr_endindex2 = one_tr_links2.Length - one_tr_index2;
                            string one_tr_value2 = one_tr_links2.Substring(one_tr_index2, one_tr_endindex2).Replace("</span", "").TrimStart('>');

                            string Hr_MatchRusult = "";
                            int s = 0;
                            if (int.TryParse(one_tr_value1, out   s) && int.TryParse(one_tr_value2, out   s))
                            {
                                Hr_MatchRusult = one_tr_value1 + ":" + one_tr_value2;//半场比赛结果
                            }
                            else
                            {
                                Hr_MatchRusult = "取消";//半场比赛结果
                            }

                            //装载到model
                            matchResModel.Result1 = Hr_MatchRusult;
                        }
                    }

                    string two_tr = script.Substring(script.IndexOf(full_tr), 300);//全场判断
                    if (two_tr.Contains("</tr>"))
                    {
                        two_tr = two_tr.Substring(0, (two_tr.IndexOf("</tr>") + 5));

                        string two_tr_regiexstr = @"span(.*?)</span";
                        Regex R_two_tr = new Regex(two_tr_regiexstr, RegexOptions.IgnoreCase);
                        MatchCollection MC_two_tr = R_two_tr.Matches(two_tr);
                        if (MC_two_tr.Count >= 2)
                        {
                            string one_tr_links1 = MC_two_tr[0].ToString();
                            int one_tr_index1 = one_tr_links1.IndexOf(">");
                            int one_tr_endindex1 = one_tr_links1.Length - one_tr_index1;
                            string one_tr_value1 = one_tr_links1.Substring(one_tr_index1, one_tr_endindex1).Replace("</span", "").TrimStart('>').TrimStart('>');//最后结果

                            string one_tr_links2 = MC_two_tr[1].ToString();
                            int one_tr_index2 = one_tr_links2.IndexOf(">");
                            int one_tr_endindex2 = one_tr_links2.Length - one_tr_index2;
                            string one_tr_value2 = one_tr_links2.Substring(one_tr_index2, one_tr_endindex2).Replace("</span", "").TrimStart('>').TrimStart('>');

                            string Full_MatchRusult = "";
                            int s = 0;
                            if (int.TryParse(one_tr_value1, out   s) && int.TryParse(one_tr_value2, out   s))
                            {
                                Full_MatchRusult = one_tr_value1 + ":" + one_tr_value2;//全场比赛结果
                            }
                            else
                            {
                                Full_MatchRusult = "取消";//全场比赛结果
                            }

                            //装载到model
                            matchResModel.Result2 = Full_MatchRusult;
                        }
                    }
                }

                #endregion

                //获得比赛时间
                string macth_T_TR = "TR_" + htmltops[i].TrimStart('\'').TrimEnd('\'');
                if (script.Contains(macth_T_TR))
                {
                    string gettime = script.Substring(script.IndexOf(macth_T_TR), 100);//半场判断
                    int indextime = gettime.IndexOf("time") + 6;
                    int indextd = gettime.IndexOf("</td>");
                    int lengths = indextd - indextime;

                    string tt1 = gettime.Substring(indextime, lengths);

                    bool isAfter = tt1.Contains("p");//是否为下午时间
                    string time = tt1.Replace("<br>", "").IndexOf(':') == 6 ? "0" + tt1.Replace("<br>", "").Substring(5, 4) : tt1.Replace("<br>", "").Substring(5, 5);
                    string ftTime = DateTime.Now.Year + "-" + tt1.Substring(0, 5) + " " + time;
                    DateTime dttime = Convert.ToDateTime(ftTime);
                    if (dttime.Hour >= 1 && dttime.Hour < 12 && isAfter)
                    {
                        dttime = dttime.AddHours(12);
                    }

                    matchResModel.ResultTime = DateTimeHelper.DateTimeToUnixTimestamp(dttime, 4);//这里不用加4
                }

                string[] number = htmltops[i].TrimStart('\'').TrimEnd('\'').Split('_');
                matchResModel.MatchNumber = Convert.ToInt32(number[1]);//比赛号

                matchResultList.Add(matchResModel);
            }
            return matchResultList;
        }
        #endregion 比赛结果
    }
}