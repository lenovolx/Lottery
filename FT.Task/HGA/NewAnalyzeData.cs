using FT.Model;
using FT.Utility.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FT.Task
{
    public class NewAnalyzeData
    {
        /// <summary>
        /// 去掉'号，返回数组
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string[] BuildString(string str)
        {
            string[] m = str.Split(',');

            for (int i = 0; i < m.Length; i++)
            {
                m[i] = m[i].TrimStart('\'').TrimEnd('\'');
            }

            return m;
        }

        /// <summary>
        /// 拼接成json对象
        /// </summary>
        /// <param name="strhead"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string BuildJson(string[] strhead, string content)
        {
            string sjson = "";
            string[] aContent = BuildString(content);
            StringBuilder sbjson = new StringBuilder();

            if (strhead.Length <= aContent.Length)//内容长度会大于字段长度
            {
                sbjson.Append("{");
                for (int i = 0; i < strhead.Length; i++)
                {
                    sbjson.Append("'" + strhead[i] + "':'" + aContent[i] + "',");
                }
                sjson = sbjson.ToString().TrimEnd(',') + "}";
            }

            return sjson;
        }

        /// <summary>
        /// 得到标准盘数据
        /// </summary>
        /// <param name="htmlOrScript"></param>
        /// <param name="head"></param>
        /// <param name="RunningBall"></param>
        /// <returns></returns>
        public static string[] GetBZP_data(string htmlOrScript, ref string head, bool RunningBall = false)
        {
            string script = htmlOrScript;

            string headscript = script.Replace("_.GameHead =[", "<TTTTTT>").Replace("];", "<Tend>");
            const string headbigen = @"<TTTTTT>(.*?)<Tend>";
            Regex headst = new Regex(headbigen, RegexOptions.IgnoreCase);
            MatchCollection MCheadst = headst.Matches(headscript);

            string[] dataT = null;
            if (MCheadst.Count > 0)
            {
                head = MCheadst[0].ToString().Replace("<TTTTTT>", "").Replace("<Tend>", "");
            }

            if (RunningBall == false)
            {
                string script2 = script.Replace("_.gm['", "<HHHHHH>'").Replace("']=[", "',").Replace("];", "<Hend>");//替换成正则可以取到的数据

                const string gmbigen = @"<HHHHHH>(.*?)<Hend>";
                Regex ROH = new Regex(gmbigen, RegexOptions.IgnoreCase);
                MatchCollection MCOH = ROH.Matches(script2);

                if (MCOH.Count > 0)
                {
                    string script3 = script.Replace("g(['", "<BBBBBB>'").Replace("]);", "<Bend>");//替换成正则可以取到的数据

                    const string gbigen = @"<BBBBBB>(.*?)<Bend>";
                    Regex Rg = new Regex(gbigen, RegexOptions.IgnoreCase);
                    MatchCollection MCg = Rg.Matches(script3);

                    if (MCOH.Count == MCg.Count)
                    {
                        string[] data = new String[MCg.Count];

                        for (int i = 0; i < MCOH.Count; i++)
                        {
                            int douHaoIndex = MCOH[i].ToString().IndexOf("',");
                            string gid = MCOH[i].ToString().Substring(0, douHaoIndex).Replace("<HHHHHH>'", "");

                            for (int j = 0; j < MCg.Count; j++)
                            {
                                if (MCg[j].ToString().Contains(gid))
                                {
                                    string ss = "'" + MCOH[i].ToString().Replace("<HHHHHH>'", "").Replace("<Hend>", "") + MCg[j].ToString().Replace("<BBBBBB>", "").Replace("<Bend>", "").Replace("'" + gid + "'", "");
                                    data[i] = ss;
                                }
                            }
                        }
                        dataT = data;
                    }
                }
            }
            if (RunningBall == true)
            {
                string script3 = script.Replace("g(['", "<BBBBBB>'").Replace("]);", "<Bend>");//替换成正则可以取到的数据

                const string gbigen = @"<BBBBBB>(.*?)<Bend>";
                Regex Rg = new Regex(gbigen, RegexOptions.IgnoreCase);
                MatchCollection MCg = Rg.Matches(script3);
                string[] data = new String[MCg.Count];

                if (MCg.Count > 0)
                {
                    for (int i = 0; i < MCg.Count; i++)
                    {
                        data[i] = MCg[i].ToString();
                    }
                }
                dataT = data;
            }
            return dataT;
        }

        /// <summary>
        /// 得到标准集合
        /// </summary>
        /// <param name="htmlScript"></param>
        /// <returns></returns>
        public static List<New_BZP_Model> DoStandard(string[] htmlScript)
        {
            List<New_BZP_Model> Listbzp = new List<New_BZP_Model>();
            for (int i = 0; i < htmlScript.Length; i++)
            {
                string head = "";
                string[] pageData_bzp = GetBZP_data(htmlScript[i], ref head);
                string[] pagehead = BuildString(head);

                if (pageData_bzp != null)
                {
                    for (int j = 0; j < pageData_bzp.Length; j++)
                    {
                        string GMjson = BuildJson(pagehead, pageData_bzp[j].ToString());

                        New_BZP_Model bzpModel = FT.Utility.Helper.HTMLHelper.ToModel<New_BZP_Model>(GMjson);

                        Listbzp.Add(bzpModel);
                    }
                }
            }
            return Listbzp;
        }

        /// <summary>
        /// 取数据（其他玩法）
        /// </summary>
        /// <param name="rtype"></param>
        /// <param name="htmlOrScript"></param>
        /// <param name="head"></param>
        /// <returns></returns>
        public static string[] Get_data(string rtype, string htmlOrScript, ref string head)
        {
            string script = htmlOrScript;

            string headscript = script.Replace("_.GameHead =[", "<TTTTTT>").Replace("];", "<Tend>");
            const string headbigen = @"<TTTTTT>(.*?)<Tend>";
            Regex headst = new Regex(headbigen, RegexOptions.IgnoreCase);
            MatchCollection MCheadst = headst.Matches(headscript);

            string[] dataT = null;
            if (MCheadst.Count > 0)
            {
                head = MCheadst[0].ToString().Replace("<TTTTTT>", "").Replace("<Tend>", "");
            }

            string script2 = script.Replace("g([", "<HHHHHH>'").Replace("]);", "<Hend>");//替换成正则可以取到的数据

            const string gmbigen = @"<HHHHHH>(.*?)<Hend>";
            Regex Regex = new Regex(gmbigen, RegexOptions.IgnoreCase);
            MatchCollection MCbd = Regex.Matches(script2);

            if (MCbd.Count > 0)
            {
                string[] data = new String[MCbd.Count];

                for (int i = 0; i < MCbd.Count; i++)
                {
                    data[i] = MCbd[i].ToString().Replace("<HHHHHH>", "").Replace("<Hend>", "");
                }

                dataT = data;
            }

            return dataT;
        }

        /// <summary>
        /// 得到实体集合（其他玩法）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rtype"></param>
        /// <param name="htmlScript"></param>
        /// <returns></returns>
        public static List<T> DoOther<T>(string rtype, string[] htmlScript)
        {
            List<T> List = new List<T>();
            for (int i = 0; i < htmlScript.Length; i++)
            {
                string head = "";
                string[] pagedata = Get_data(rtype, htmlScript[i], ref head);
                string[] pagehead = BuildString(head);

                for (int j = 0; j < pagedata.Length; j++)
                {
                    string GMjson = BuildJson(pagehead, pagedata[j].ToString());
                    T Model = FT.Utility.Helper.HTMLHelper.ToModel<T>(GMjson);

                    List.Add(Model);
                }
            }
            return List;
        }

        //合并之后处理时间
        public static DateTime GetDate(string datetimestr)
        {
            DateTime dt = new DateTime();

            datetimestr = datetimestr.Replace("<br><font color=red>Running Ball</font>", "");
            string datetime = (DateTime.Now.Year.ToString() + datetimestr).Replace("<br>", " ");
            bool isP = datetime.Contains("p");
            DateTime matchdt = Convert.ToDateTime(datetime.Replace("p", ":00").Replace("a", ":00"));

            if (isP && matchdt.Hour >= 1 && matchdt.Hour < 12)//特殊处理
            {
                dt = matchdt.AddHours(12);
            }
            if (isP == false)
            {
                dt = matchdt;
            }
            return dt;
        }

        public static long GetMatchDate(string str, ref DateTime dt)
        {
            //07-28<br>01:00p<br><font color=red>Running Ball</font>06-30<br>12:00p
            bool isAfter = str.Contains("p");//是否为下午时间
            string time = str.Replace("<br>", "").IndexOf(':') == 6 ? "0" + str.Replace("<br>", "").Substring(5, 4) : str.Replace("<br>", "").Substring(5, 5);
            string ftTime = DateTime.Now.Year + "-" + str.Substring(0, 5) + " " + time;

            DateTime ftBeginTime = Convert.ToDateTime(ftTime);

            if (ftBeginTime.Hour >= 1 && ftBeginTime.Hour < 12 && isAfter)
            {
                ftBeginTime = ftBeginTime.AddHours(12);
            }

            dt = ftBeginTime;
            return DateTimeHelper.DateTimeToUnixTimestamp(ftBeginTime); //比赛时间 注意（去<br>）     ConvertDateTimeInt(System.DateTime time)
        }

        //取时间或者半场、全场字段
        public static string GetdateForNewRusult(string datetimestr)
        {
            datetimestr = datetimestr.Replace("<font style=background-color=red>半场</font>", "半场");//滚球取数据要经过这个
            datetimestr = datetimestr.Replace("<font style=background-color=red>全场</font>", "全场");
            return datetimestr;
        }

        //标准盘
        public static List<MatchInfo> BulidXMl_New_BZP(List<New_BZP_Model> bzp_list)
        {
            List<MatchInfo> listMatchInfo = new List<MatchInfo>();

            foreach (var i in bzp_list)
            {
                XElement XMatchContent = new XElement("MatchContent");
                StringBuilder sbBetType = new StringBuilder();
                MatchInfo mi = new MatchInfo();
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

                #region 中文数据处理逻辑

                mi.LeagueName = i.league;//联赛名称
                mi.HTeam = i.team_h;//主队名称
                mi.HTeamNum = i.gnum_h;//主队编号
                mi.CTeam = i.team_c;//客队名称
                mi.CTeamNum = i.gnum_c;//客队编号
                mi.MatchNumber = Convert.ToInt32(i.gid);//比赛编号
                mi.MatchType = 0;

                long eventid = Convert.ToInt64(string.IsNullOrEmpty(i.gidm) != true ? i.gidm.Replace(".", "") : "0");//标准场必须要《此作为关联数据》
                mi.EventId = eventid;

                DateTime dt = DateTime.Now;
                mi.MatchDate = GetMatchDate(i.datetime, ref dt);
                mi.BetType = sbBetType.ToString();
                mi.MatchEndTime = DateTimeHelper.DateTimeToUnixTimestamp(dt.AddMinutes(90));
                mi.CreateDate = DateTime.Now;
                mi.Odds = XMatchContent.ToString(SaveOptions.DisableFormatting);
                mi.OddsUpdateTime = DateTimeHelper.DateTimeToUnixTimestamp(DateTime.Now);

                #endregion 数据处理逻辑

                listMatchInfo.Add(mi);
            }

            return listMatchInfo;
        }

        //半场。全场
        public static List<MatchInfo> BulidXMl_New_BCQC(List<New_BCQC_Model> bcqc_list)
        {
            List<MatchInfo> listMatchInfo = new List<MatchInfo>();

            foreach (var i in bcqc_list)
            {
                XElement XMatchContent = new XElement("MatchContent");
                StringBuilder sbBetType = new StringBuilder();
                MatchInfo mi = new MatchInfo();

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

                #region 中文数据处理逻辑

                mi.LeagueName = i.league;//联赛名称
                mi.HTeam = i.team_h;//主队名称
                mi.HTeamNum = i.gnum_h;//主队编号
                mi.CTeam = i.team_c;//客队名称
                mi.CTeamNum = i.gnum_c;//客队编号
                mi.MatchNumber = Convert.ToInt32(i.gid);//比赛编号
                mi.MatchType = 0;

                DateTime dt = DateTime.Now;
                mi.MatchDate = GetMatchDate(i.datetime, ref dt);
                mi.BetType = sbBetType.ToString();
                mi.MatchEndTime = DateTimeHelper.DateTimeToUnixTimestamp(dt.AddMinutes(90));
                mi.CreateDate = DateTime.Now;
                mi.Odds = XMatchContent.ToString(SaveOptions.DisableFormatting);
                mi.OddsUpdateTime = DateTimeHelper.DateTimeToUnixTimestamp(DateTime.Now);

                #endregion 数据处理逻辑

                listMatchInfo.Add(mi);
            }

            return listMatchInfo;
        }

        //波胆
        public static List<MatchInfo> BulidXMl_New_BD(List<New_BD_Model> bd_list)
        {
            List<MatchInfo> listMatchInfo = new List<MatchInfo>();

            foreach (var i in bd_list)
            {
                XElement XMatchContent = new XElement("MatchContent");
                StringBuilder sbBetType = new StringBuilder();
                MatchInfo mi = new MatchInfo();

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

                #region 中文数据处理逻辑

                mi.LeagueName = i.league;//联赛名称
                mi.HTeam = i.team_h;//主队名称
                mi.HTeamNum = i.gnum_h;//主队编号
                mi.CTeam = i.team_c;//客队名称
                mi.CTeamNum = i.gnum_c;//客队编号
                mi.MatchNumber = Convert.ToInt32(i.gid);//比赛编号
                mi.MatchType = 0;

                DateTime dt = DateTime.Now;

                mi.MatchDate = GetMatchDate(i.datetime, ref  dt);
                mi.BetType = sbBetType.ToString();
                mi.MatchEndTime = DateTimeHelper.DateTimeToUnixTimestamp(dt.AddMinutes(90));
                mi.CreateDate = DateTime.Now;
                mi.Odds = XMatchContent.ToString(SaveOptions.DisableFormatting);
                mi.OddsUpdateTime = DateTimeHelper.DateTimeToUnixTimestamp(DateTime.Now);

                #endregion 数据处理逻辑

                listMatchInfo.Add(mi);
            }

            return listMatchInfo;
        }

        //总入球
        public static List<MatchInfo> BulidXMl_New_ZRQ(List<New_ZRQ_Model> zrq_list)
        {
            List<MatchInfo> listMatchInfo = new List<MatchInfo>();

            foreach (var i in zrq_list)
            {
                XElement XMatchContent = new XElement("MatchContent");
                StringBuilder sbBetType = new StringBuilder();
                MatchInfo mi = new MatchInfo();

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

                #region 中文数据处理逻辑

                mi.LeagueName = i.league;//联赛名称
                mi.HTeam = i.team_h;//主队名称
                mi.HTeamNum = i.gnum_h;//主队编号
                mi.CTeam = i.team_c;//客队名称
                mi.CTeamNum = i.gnum_c;//客队编号
                mi.MatchNumber = Convert.ToInt32(i.gid);//比赛编号
                mi.MatchType = 0;

                DateTime dt = DateTime.Now;
                mi.MatchDate = GetMatchDate(i.datetime, ref dt);

                mi.BetType = sbBetType.ToString();
                mi.MatchEndTime = DateTimeHelper.DateTimeToUnixTimestamp(dt.AddMinutes(90));
                mi.CreateDate = DateTime.Now;
                mi.Odds = XMatchContent.ToString(SaveOptions.DisableFormatting);
                mi.OddsUpdateTime = DateTimeHelper.DateTimeToUnixTimestamp(DateTime.Now);

                #endregion 数据处理逻辑

                listMatchInfo.Add(mi);
            }

            return listMatchInfo;
        }

        //综合过关（待加入）
        public static List<MatchInfo> BulidXMl_New_HZGG(List<New_ZHGG_Model> zhgg_list)
        {
            List<MatchInfo> listMatchInfo = new List<MatchInfo>();

            foreach (var i in zhgg_list)
            {
                XElement XMatchContent = new XElement("MatchContent");
                StringBuilder sbBetType = new StringBuilder();
                MatchInfo mi = new MatchInfo();
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

                #region 中文数据处理逻辑

                mi.LeagueName = i.league;//联赛名称
                mi.HTeam = i.team_h;//主队名称
                mi.HTeamNum = i.gnum_h;//主队编号
                mi.CTeam = i.team_c;//客队名称
                mi.CTeamNum = i.gnum_c;//客队编号
                mi.MatchNumber = Convert.ToInt32(i.gid);//比赛编号
                mi.MatchType = 0;

                long eventid = Convert.ToInt64(string.IsNullOrEmpty(i.gidm) != true ? i.gidm.Replace(".", "") : "0");//标准场必须要《此作为关联数据》
                mi.EventId = eventid;

                DateTime dt = DateTime.Now;
                mi.MatchDate = GetMatchDate(i.datetime, ref dt);
                mi.BetType = sbBetType.ToString();
                mi.MatchEndTime = DateTimeHelper.DateTimeToUnixTimestamp(dt.AddMinutes(90));
                mi.CreateDate = DateTime.Now;
                mi.Odds = XMatchContent.ToString(SaveOptions.DisableFormatting);
                mi.OddsUpdateTime = DateTimeHelper.DateTimeToUnixTimestamp(DateTime.Now);

                #endregion 数据处理逻辑

                listMatchInfo.Add(mi);
            }

            return listMatchInfo;
        }

        #region 比赛结果

        /// <summary>
        /// 获得比赛结果(新网站)
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
                string onetr = "id=\"TR_" + htmltops[i].TrimStart('\'').TrimEnd('\'');//第一个tr
                string twotr = "id=\"TR_1_" + htmltops[i].TrimStart('\'').TrimEnd('\'');//d第二个tr

                MatchResult matchResModel = new MatchResult();//比赛结果实体
                string tdtime = "", tdfull1 = "", tdbg1 = "", tdfull2 = "", tdbg2 = "";

                #region 获得比赛比分结果
                if (script.Contains(onetr) && script.Contains(twotr))
                {
                    string one_tr = script.Substring(script.IndexOf(onetr), 500);
                    if (one_tr.Contains("</tr>"))
                    {
                        one_tr = one_tr.Substring(0, (one_tr.IndexOf("</tr>") + 5));//第一个tr

                        if (i == 6)
                        {
                        }
                        string one_tr_regiexstr = @"<td(.*?)</td>";
                        Regex R_one_tr = new Regex(one_tr_regiexstr, RegexOptions.IgnoreCase);
                        MatchCollection MC_one_tr = R_one_tr.Matches(one_tr);
                        if (MC_one_tr.Count == 3)
                        {
                            string td = MC_one_tr[1].ToString();
                            tdfull1 = JieXiTD(td);

                            td = MC_one_tr[2].ToString();
                            tdbg1 = JieXiTD(td);
                        }
                        if (MC_one_tr.Count == 5)
                        {
                            string td = MC_one_tr[0].ToString();
                            tdtime = JieXiTD(td);

                            td = MC_one_tr[2].ToString();
                            tdfull1 = JieXiTD(td);

                            td = MC_one_tr[3].ToString();
                            tdbg1 = JieXiTD(td);
                        }
                    }

                    string two_tr = script.Substring(script.IndexOf(twotr), 300);//全场判断
                    if (two_tr.Contains("</tr>"))
                    {
                        two_tr = two_tr.Substring(0, (two_tr.IndexOf("</tr>") + 5));

                        string two_tr_regiexstr = @"<td(.*?)</td>";
                        Regex R_two_tr = new Regex(two_tr_regiexstr, RegexOptions.IgnoreCase);
                        MatchCollection MC_two_tr = R_two_tr.Matches(two_tr);
                        if (MC_two_tr.Count >= 2)
                        {
                            string td = MC_two_tr[1].ToString();
                            tdfull2 = JieXiTD(td);

                            if (tdfull2 == "")
                            {
                            }
                            td = MC_two_tr[2].ToString();
                            tdbg2 = JieXiTD(td);
                            if (tdbg2 == "")
                            {
                            }
                        }
                    }
                }

                //拼接比赛结果
                matchResModel.Result1 = GetScore(tdbg1, tdbg2);
                matchResModel.Result2 = GetScore(tdfull1, tdfull2);

                long longs = 0;
                if (long.TryParse(tdtime, out longs))
                {
                    matchResModel.ResultTime = long.Parse(tdtime);
                }

                #endregion

                string[] number = htmltops[i].TrimStart('\'').TrimEnd('\'').Split('_');
                matchResModel.MatchNumber = Convert.ToInt32(number[1]);//比赛号

                matchResultList.Add(matchResModel);
            }
            return matchResultList;
        }

        /// <summary>
        /// 解析TD
        /// </summary>
        /// <param name="td"></param>
        /// <returns></returns>
        private static string JieXiTD(string td)
        {
            string jieguo = "";

            if (td.Contains("acc_result_time"))
            {
                int indextime = td.IndexOf("time") + 6;
                int indextd = td.IndexOf("</td>");
                int lengths = indextd - indextime;

                string tt1 = td.Substring(indextime, lengths);

                bool isAfter = tt1.Contains("p");//是否为下午时间
                string time = tt1.Replace("<br>", "").IndexOf(':') == 6 ? "0" + tt1.Replace("<br>", "").Substring(5, 4) : tt1.Replace("<br>", "").Substring(5, 5);
                string ftTime = DateTime.Now.Year + "-" + tt1.Substring(0, 5) + " " + time;
                DateTime dttime = (isAfter == true ? Convert.ToDateTime(ftTime).AddHours(12) : Convert.ToDateTime(ftTime));

                jieguo = DateTimeHelper.DateTimeToUnixTimestamp(dttime, 4).ToString();//这里不用加4
            }
            if (td.Contains("acc_result_full"))
            {
                string two_tr_regiexstr = @"span(.*?)</span";
                Regex R_two_tr = new Regex(two_tr_regiexstr, RegexOptions.IgnoreCase);
                MatchCollection MC_two_tr = R_two_tr.Matches(td);
                if (MC_two_tr.Count == 1)
                {
                    string one_tr_links1 = MC_two_tr[0].ToString();
                    int one_tr_index1 = one_tr_links1.IndexOf(">");
                    int one_tr_endindex1 = one_tr_links1.Length - one_tr_index1;
                    jieguo = one_tr_links1.Substring(one_tr_index1, one_tr_endindex1).Replace("</span", "").TrimStart('>').TrimStart('>');//最后结果
                }
            }
            if (td.Contains("acc_result_bg"))
            {
                string two_tr_regiexstr = @"span(.*?)</span";
                Regex R_two_tr = new Regex(two_tr_regiexstr, RegexOptions.IgnoreCase);
                MatchCollection MC_two_tr = R_two_tr.Matches(td);
                if (MC_two_tr.Count == 1)
                {
                    string one_tr_links1 = MC_two_tr[0].ToString();
                    int one_tr_index1 = one_tr_links1.IndexOf(">");
                    int one_tr_endindex1 = one_tr_links1.Length - one_tr_index1;
                    jieguo = one_tr_links1.Substring(one_tr_index1, one_tr_endindex1).Replace("</span", "").TrimStart('>').TrimStart('>');//最后结果
                }
            }

            return jieguo;
        }

        /// <summary>
        /// 比分
        /// </summary>
        /// <param name="val1"></param>
        /// <param name="val2"></param>
        /// <returns></returns>
        private static string GetScore(string val1, string val2)
        {
            string score = "";
            int s1, s2;
            if (int.TryParse(val1, out s1) && int.TryParse(val2, out s2))
            {
                score = s1 + ":" + s2;
            }
            else
            {
                score = "取消";
            }

            return score;
        }

        #endregion 比赛结果
    }
}