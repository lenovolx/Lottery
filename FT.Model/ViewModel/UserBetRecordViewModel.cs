using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FT.Utility.Helper;

namespace FT.Model.ViewModel
{
    public class UserBetRecordViewModel
    {
        public UserBetRecordViewModel()
        {
            Detail = new List<UserBetDetail>();
            BetStatus = "未结算";
        }
        /// <summary>
        /// 订单编号
        /// </summary>
        public long OrderId { get; set; }
        /// <summary>
        /// 下单时间
        /// </summary>
        public string OrderDate { get; set; }
        /// <summary>
        /// 投注金额
        /// </summary>
        public decimal BetValue { get; set; }
        /// <summary>
        /// 输赢金额
        /// </summary>
        public decimal BetBonus { get; set; }
        public int IsSettle { get; set; }
        /// <summary>
        /// 可赢金额(赔率阶积x下注金额)
        /// </summary>
        public decimal? AchieveAmount
        {
            get
            {
                return Detail.Any()
                    ? Detail.Select(s => s.BetIor).Aggregate(
                        (a, b) => a*b)*BetValue - BetValue
                    : 0;
            }
        }
        public int? BetType { get; set; }
        /// <summary>
        /// 注单状态
        /// </summary>
        public string BetStatus { get; set; }
        /// <summary>
        /// 投注详细
        /// </summary>
        public IEnumerable<UserBetDetail> Detail { get; set; }
    }

    public class UserBetDetail : BaseLanguageViewModel
    {
        /// <summary>
        /// 联赛名称
        /// </summary>
        public string LeagueName { get; set; }
        /// <summary>
        /// 对决球队(主队前客队后)
        /// </summary>
        public string TeamVs
        {
            get
            {
                string description;
                if (BetKey.IndexOf("|", StringComparison.Ordinal) > 0 && (BetType == 12 || BetType == 14 || BetType == 63 || BetType == 61 || BetType == 52 || BetType == 53))
                {
                    var keys = BetKey.Split('|');
                    description = keys[0].ToUpper().Equals("h") ? string.Format("{0}{2} VS {1}{3}", HTeam, CTeam, " " + keys[1], "") : string.Format("{0}{2} VS {3}{1}", HTeam, CTeam, "", keys[1] + " ");
                }
                else
                    description = string.Format("{0} VS {1}", HTeam, CTeam);

                return description;
            }
        }

        public long MatchId { get; set; }
        public int BetType { get; set; }

        /// <summary>
        /// 投注描述
        /// </summary>
        public string BetDescription
        {
            get
            {
                var description = string.Empty;
                var betKey = BetContent.Split('@');
                switch (BetType)
                {
                    case 50:
                    case 51:
                    case 60:
                    case 10:
                        if (betKey[0].ToUpper().Equals("H"))
                            description = string.Format("{0} @ {1}", HTeam, betKey[1]);
                        else if (betKey[0].ToUpper().Equals("C"))
                            description = string.Format("{0} @ {1}", CTeam, betKey[1]);
                        else
                            description = string.Format("{0} @ {1}", Language.Equals("cn") ? "和局 " : "Draw ", betKey[1]);
                        break;
                    case 52:
                    case 53:
                    case 63:
                    case 12:
                        if (BetKey.IndexOf("|", StringComparison.Ordinal) > 0)
                        {
                            var keys = BetKey.Split('|');
                            if (keys[0].ToUpper().Equals("H"))
                                description = string.Format("{0} @ {1}", HTeam, betKey[1]);
                            else
                                description = string.Format("{0} @ {1}", CTeam, betKey[1]);
                        }
                        break;
                    case 55:
                    case 54:
                    case 61:
                    case 14:
                        if (betKey[0].ToUpper().Equals("O"))
                            description = string.Format("{0}{2} @ {1}", Language.Equals("cn") ? "大 " : "On ", betKey[1],BetKey);
                        else
                            description = string.Format("{0}{2} @ {1} ", Language.Equals("cn") ? "小 " : "Under ", betKey[1], BetKey);
                        break;
                    case 56:
                    case 16:
                        if (betKey[0].ToUpper().Equals("O"))
                            description = string.Format("{0} @ {1}", Language.Equals("cn") ? "单 " : "Odd ", betKey[1]);
                        else
                            description = string.Format("{0} @ {1}", Language.Equals("cn") ? "双 " : "Even ", betKey[1]);
                        break;
                    case 40:
                        if (betKey[0].ToUpper().Equals("HH"))
                            description = string.Format("{0}/{1}{2}", HTeam, HTeam, " @ " + betKey[1]);
                        else if (betKey[0].ToUpper().Equals("HC"))
                            description = string.Format("{0}/{1}{2}", HTeam, CTeam, " @ " + betKey[1]);
                        else if (betKey[0].ToUpper().Equals("HN"))
                            description = string.Format("{0}/{1}{2}", HTeam, Language.Equals("cn") ? "和局 " : "Draw ",
                                " @ " + betKey[1]);
                        else if (betKey[0].ToUpper().Equals("CC"))
                            description = string.Format("{0}/{1}{2}", CTeam, CTeam, " @ " + betKey[1]);
                        else if (betKey[0].ToUpper().Equals("CH"))
                            description = string.Format("{0}/{1}{2}", CTeam, HTeam, " @ " + betKey[1]);
                        else if (betKey[0].ToUpper().Equals("CN"))
                            description = string.Format("{0}/{1}{2}", HTeam, Language.Equals("cn") ? "和局 " : "Draw ",
                                " @ " + betKey[1]);
                        else if (betKey[0].ToUpper().Equals("NN"))
                            description = string.Format("{0}/{1}{2}", Language.Equals("cn") ? "和局 " : "Draw ",
                                Language.Equals("cn") ? "和局" : "Draw", " @ " + betKey[1]);
                        else if (betKey[0].ToUpper().Equals("NH"))
                            description = string.Format("{0}/{1}{2}", Language.Equals("cn") ? "和局 " : "Draw ", HTeam,
                                " @ " + betKey[1]);
                        else
                            description = string.Format("{0}/{1}{2}", Language.Equals("cn") ? "和局 " : "Draw ", CTeam,
                                " @ " + betKey[1]);
                        break;
                    default:
                        description = string.Format("{0} @ {1}", betKey[0], betKey[1]);
                        break;
                }
                return description;
            }
        }

        public string BetContent { get; set; }
        public string BetKey { get; set; }
        public string HTeam { get; set; }
        public string CTeam { get; set; }
        /// <summary>
        /// 赔率
        /// </summary>
        public decimal BetIor
        {
            get { return decimal.Parse(BetContent.Split('@')[1]); }
        }
        /// <summary>
        /// 投注玩法名称
        /// </summary>
        public string BetTypeName { get; set; }
    }

    public class MatchBetViewModel
    {
        public long OrderId { get; set; }
        public string UserName { get; set; }
        /// <summary>
        /// 输赢金额
        /// </summary>
        public decimal BetBonus { get; set; }
        /// <summary>
        /// 投注金额
        /// </summary>
        public decimal BetValue { get; set; }

        public decimal? AchieveAmount { get; set; }
        /// <summary>
        /// 注单状态
        /// </summary>
        public string BetStatus { get; set; }
        /// <summary>
        /// 下单时间
        /// </summary>
        public string OrderDate { get; set; }
    }

    public class UserBetAmountGroupModel
    {
        public long? UserId { get; set; }
        public List<UserBetAmountModel> Group { get; set; }
    }
    public class UserBetAmountModel
    {
        public long? BetId { get; set; }
        public long? UserId { get; set; }
        public string UserName { get; set; }
        public decimal? Amount { get; set; }
    }
}