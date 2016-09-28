using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using FT.Utility.Helper;
using System.Windows;
namespace FT.WinForm
{
    public class BetInfoHelper
    {
        public static XElement GetBetContent(XElement xeMatch, XElement xeBetInfo)
        {
            //string betText ="";
            XElement betContent = new XElement("UserBetContent");
            betContent.SetElementValue("MatchDate", xeMatch.Element("MatchDate").GetValue());
            betContent.SetElementValue("LeagueName", xeMatch.Element("LeagueName").GetValue());
            betContent.SetElementValue("MatchID", xeMatch.Element("MatchId").GetValue(0));
            betContent.SetElementValue("MatchNumber", xeMatch.Element("MatchNumber").GetValue());
            betContent.SetElementValue("BetKey", xeBetInfo.Element("BetKey").GetValue());
            betContent.SetElementValue("BetType", xeBetInfo.Element("BetType").GetValue(0));
            betContent.SetElementValue("BetTypeName", Application.Current.FindResource("Enum.BetType." + xeBetInfo.Element("BetType").GetValue(1)));
            betContent.SetElementValue("HTeam", xeMatch.Element("HTeam").GetValue());
            betContent.SetElementValue("CTeam", xeMatch.Element("CTeam").GetValue());
            betContent.SetElementValue("VS", "VS");
            betContent.SetElementValue("MinLimit", xeBetInfo.Element("BetType").GetValue().StartsWith("5") ? xeMatch.Element("MinLimit").GetValue(1) : 1);
            betContent.SetElementValue("MaxLimit", xeBetInfo.Element("BetType").GetValue().StartsWith("5") ? xeMatch.Element("MaxLimit").GetValue(10) : 1);
            return betContent;
        }
        public static string Fix(string value)
        {
            decimal tmp = 0;
            if (decimal.TryParse(value, out tmp))
            {

            }
            return tmp.ToString("F2");
        }

        #region 全场独赢
        /// <summary>
        /// 全场独赢-胜赔率
        /// </summary>
        /// <param name="xe"></param>
        /// <returns></returns>
        public static XElement GetIOR_MH(XElement xe)
        {
            string ior = xe.Element("BetIOR").GetValue();
            return new XElement("IOR_MH", Fix(ior.Split('|')[0]));
        }
        /// <summary>
        /// 全场独赢-负赔率
        /// </summary>
        /// <param name="xe"></param>
        /// <returns></returns>
        public static XElement GetIOR_MC(XElement xe)
        {
            string ior = xe.Element("BetIOR").GetValue();
            return new XElement("IOR_MC", Fix(ior.Split('|')[1]));
        }
        /// <summary>
        /// 全场独赢-平赔率
        /// </summary>
        /// <param name="xe"></param>
        /// <returns></returns>
        public static XElement GetIOR_MN(XElement xe)
        {
            string ior = xe.Element("BetIOR").GetValue();
            return new XElement("IOR_MN", Fix(ior.Split('|')[2]));
        }
        #endregion
        #region 半场独赢
        /// <summary>
        /// 半场独赢-胜赔率
        /// </summary>
        /// <param name="xe"></param>
        /// <returns></returns>
        public static XElement GetIOR_HMH(XElement xe)
        {
            string ior = xe.Element("BetIOR").GetValue();
            return new XElement("IOR_HMH", Fix(ior.Split('|')[0]));
        }
        /// <summary>
        /// 半场独赢-负赔率
        /// </summary>
        /// <param name="xe"></param>
        /// <returns></returns>
        public static XElement GetIOR_HMC(XElement xe)
        {
            string ior = xe.Element("BetIOR").GetValue();
            return new XElement("IOR_HMC", Fix(ior.Split('|')[1]));
        }
        /// <summary>
        /// 半场独赢-平赔率
        /// </summary>
        /// <param name="xe"></param>
        /// <returns></returns>
        public static XElement GetIOR_HMN(XElement xe)
        {
            string ior = xe.Element("BetIOR").GetValue();
            return new XElement("IOR_HMN", Fix(ior.Split('|')[2]));
        }
        #endregion
        #region 全场让球
        /// <summary>
        /// 全场让球-胜赔率
        /// </summary>
        /// <param name="xe"></param>
        /// <returns></returns>
        public static XElement GetIOR_RH(XElement xe)
        {
            string ior = xe.Element("BetIOR").GetValue();
            return new XElement("IOR_RH", Fix(ior.Split('|')[0]));
        }
        /// <summary>
        /// 全场让球-负赔率
        /// </summary>
        /// <param name="xe"></param>
        /// <returns></returns>
        public static XElement GetIOR_RC(XElement xe)
        {
            string ior = xe.Element("BetIOR").GetValue();
            return new XElement("IOR_RC", Fix(ior.Split('|')[1]));
        }
        /// <summary>
        /// 全场让球数
        /// </summary>
        /// <param name="xe"></param>
        /// <returns></returns>
        public static XElement GetRatio(XElement xe)
        {
            string ior = xe.Element("BetKey").GetValue();
            return new XElement("IOR_Ratio", ior.Split('|')[1]);
        }

        /// <summary>
        /// 全场让球方  H|C
        /// </summary>
        /// <param name="xe"></param>
        /// <returns>H|C</returns>
        public static XElement GetStrong(XElement xe)
        {
            string ior = xe.Element("BetKey").GetValue();
            return new XElement("IOR_Strong", ior.Split('|')[0]);
        }
        #endregion
        #region 半场让球
        /// <summary>
        /// 半场让球-胜赔率
        /// </summary>
        /// <param name="xe"></param>
        /// <returns></returns>
        public static XElement GetIOR_HRH(XElement xe)
        {
            string ior = xe.Element("BetIOR").GetValue();
            return new XElement("IOR_HRH", Fix(ior.Split('|')[0]));
        }
        /// <summary>
        /// 半场让球-负赔率
        /// </summary>
        /// <param name="xe"></param>
        /// <returns></returns>
        public static XElement GetIOR_HRC(XElement xe)
        {
            string ior = xe.Element("BetIOR").GetValue();
            return new XElement("IOR_HRC", Fix(ior.Split('|')[1]));
        }
        /// <summary>
        /// 半场让球数
        /// </summary>
        /// <param name="xe"></param>
        /// <returns></returns>
        public static XElement GetHRatio(XElement xe)
        {
            string ior = xe.Element("BetKey").GetValue();
            return new XElement("HRatio", ior.Split('|')[1]);
        }

        /// <summary>
        /// 半场让球方  H|C
        /// </summary>
        /// <param name="xe"></param>
        /// <returns>H|C</returns>
        public static XElement GetHStrong(XElement xe)
        {
            string ior = xe.Element("BetKey").GetValue();
            return new XElement("HStrong", ior.Split('|')[0]);
        }
        #endregion
        #region 全场大小球
        /// <summary>
        /// 全场大球数
        /// </summary>
        /// <param name="xe"></param>
        /// <returns></returns>
        public static XElement GetRatio_O(XElement xe)
        {
            string ratio_o = xe.Element("BetKey").GetValue();
            return new XElement("Ratio_O", ratio_o);
        }
        /// <summary>
        /// 全场大球赔率
        /// </summary>
        /// <param name="xe"></param>
        /// <returns></returns>
        public static XElement GetIOR_OUO(XElement xe)
        {
            string ior = xe.Element("BetIOR").GetValue();
            return new XElement("IOR_OUO", Fix(ior.Split('|')[0]));
        }
        /// <summary>
        /// 全场小球数
        /// </summary>
        /// <param name="xe"></param>
        /// <returns></returns>
        public static XElement GetRatio_U(XElement xe)
        {
            string ratio_u = xe.Element("BetKey").GetValue();
            return new XElement("Ratio_U", ratio_u);
        }
        /// <summary>
        /// 全场小球赔率
        /// </summary>
        /// <param name="xe"></param>
        /// <returns></returns>
        public static XElement GetIOR_OUU(XElement xe)
        {
            string ior = xe.Element("BetIOR").GetValue();
            return new XElement("IOR_OUU", Fix(ior.Split('|')[1]));
        }
        #endregion
        #region 半场大小球
        /// <summary>
        /// 半场大球数
        /// </summary>
        /// <param name="xe"></param>
        /// <returns></returns>
        public static XElement GetHRatio_O(XElement xe)
        {
            string ratio_o = xe.Element("BetKey").GetValue();
            return new XElement("HRatio_O", ratio_o);
        }
        /// <summary>
        /// 半场大球赔率
        /// </summary>
        /// <param name="xe"></param>
        /// <returns></returns>
        public static XElement GetIOR_HOUO(XElement xe)
        {
            string ior = xe.Element("BetIOR").GetValue();
            return new XElement("IOR_HOUO", Fix(ior.Split('|')[0]));
        }
        /// <summary>
        /// 半场小球数
        /// </summary>
        /// <param name="xe"></param>
        /// <returns></returns>
        public static XElement GetHRatio_U(XElement xe)
        {
            string ratio_u = xe.Element("BetKey").GetValue();
            return new XElement("HRatio_U", ratio_u);
        }
        /// <summary>
        /// 半场小球赔率
        /// </summary>
        /// <param name="xe"></param>
        /// <returns></returns>
        public static XElement GetIOR_HOUU(XElement xe)
        {
            string ior = xe.Element("BetIOR").GetValue();
            return new XElement("IOR_HOUU", Fix(ior.Split('|')[1]));
        }
        #endregion
        #region 单双
        /// <summary>
        /// 全场单赔率
        /// </summary>
        /// <param name="xe"></param>
        /// <returns></returns>
        public static XElement GetIOR_EOO(XElement xe)
        {
            string ior = xe.Element("BetIOR").GetValue();
            return new XElement("IOR_EOO", Fix(ior.Split('|')[0]));
        }
        /// <summary>
        /// 全场双赔率
        /// </summary>
        /// <param name="xe"></param>
        /// <returns></returns>
        public static XElement GetIOR_EOE(XElement xe)
        {
            string ior = xe.Element("BetIOR").GetValue();
            return new XElement("IOR_EOE", Fix(ior.Split('|')[1]));
        }
        #endregion
        #region 波胆比分及赔率字典
        public static Dictionary<string, string> GetIOR_Score(XElement xe)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            string[] scorearr = xe.Element("BetKey").GetValue().Split('|');
            string[] iorarr = xe.Element("BetIOR").GetValue().Split('|');
            for (int i = 0; i < scorearr.Length; i++)
            {
                dic.Add(scorearr[i], iorarr[i]);
            }
            return dic;
        }
        #endregion

        #region 半全场及赔率字典
        public static Dictionary<string, string> GetIOR_HalfFinal(XElement xe)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            string[] halffinalarr = xe.Element("BetKey").GetValue().Split('|');
            string[] iorarr = xe.Element("BetIOR").GetValue().Split('|');
            for (int i = 0; i < halffinalarr.Length; i++)
            {
                dic.Add(halffinalarr[i], iorarr[i]);
            }
            return dic;
        }
        #endregion
        #region 总入球及赔率字典
        public static Dictionary<string, string> GetIOR_Total(XElement xe)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            string[] totalarr = xe.Element("BetKey").GetValue().Split('|');
            string[] iorarr = xe.Element("BetIOR").GetValue().Split('|');
            for (int i = 0; i < totalarr.Length; i++)
            {
                dic.Add(totalarr[i], iorarr[i]);
            }
            return dic;
        }
        #endregion
        #region 通用赔率字典
        public static Dictionary<string, string> GetIOR_Com(XElement xe)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            string[] totalarr = xe.Element("BetKey").GetValue().Split('|');
            string[] iorarr = xe.Element("BetIOR").GetValue().Split('|');
            for (int i = 0; i < totalarr.Length; i++)
            {
                dic.Add(totalarr[i], iorarr[i]);
            }
            return dic;
        }
        #endregion
        #region 通用获取比赛全部赔率字典
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xe">matcninfo</param>
        /// <returns></returns>
        public static Dictionary<string, string> GetIOR_All(XElement xeMarchInfo)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            foreach (XElement xe in xeMarchInfo.Elements("BetInfo"))
            {
                if (new int[] { 12, 13, 52, 53 }.Contains(xe.Element("BetType").GetValue(0)))
                {
                    string[] keyarr = xe.Element("BetKey").GetValue().Split('|');
                    string[] iorarr = xe.Element("BetIOR").GetValue().Split('|');
                    dic.Add(xe.Element("BetType").GetValue() + "." + "H", Fix(iorarr[0]));
                    dic.Add(xe.Element("BetType").GetValue() + "." + "C", Fix(iorarr[1]));
                }
                else if (new int[] { 14, 15, 54, 55 }.Contains(xe.Element("BetType").GetValue(0)))
                {
                    string[] keyarr = xe.Element("BetKey").GetValue().Split('|');
                    string[] iorarr = xe.Element("BetIOR").GetValue().Split('|');
                    dic.Add(xe.Element("BetType").GetValue() + "." + "O", Fix(iorarr[0]));
                    dic.Add(xe.Element("BetType").GetValue() + "." + "U", Fix(iorarr[1]));
                }
                else
                {
                    string[] keyarr = xe.Element("BetKey").GetValue().Split('|');
                    string[] iorarr = xe.Element("BetIOR").GetValue().Split('|');
                    for (int i = 0; i < keyarr.Length; i++)
                    {
                        dic.Add(xe.Element("BetType").GetValue() + "." + keyarr[i], Fix(iorarr[i]));
                    }
                }
            }
            return dic;
        }
        #endregion
    }
}
