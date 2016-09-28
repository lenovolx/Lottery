using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Caching;
using System.Web.Http;
using FT.Model;
using FT.Model.QueryModel;
using FT.Model.ViewModel;
using Newtonsoft.Json.Linq;
using FT.Utility.ApiHelper;
using FT.Utility.Helper;
using FT.Repository;
using FT.UI.Filters;
using FT.Utility.ExtendException;
using Newtonsoft.Json;

namespace FT.UI.API
{
    /// <summary>
    /// 会员相关数据接口
    /// </summary>
    public class UserController : BaseApiController
    {
        #region User Login Register
        /// <summary>
        /// 会员登录
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public IHttpActionResult Login([FromBody] JObject data)
        {
            var ret = ApiFunc(data, "---User---Login接口---", (d, p) =>
            {
                var dic = Sign.GetParameters(data.ToString());
                var userName = dic["username"] + "";
                var password = dic["password"] + "";
                object plat, rember;
                if (!dic.TryGetValue("plat", out plat))
                    plat = 0;
                if (!dic.TryGetValue("rember", out rember))
                    rember = false;
                if (string.IsNullOrWhiteSpace(userName))
                    throw new FtException("请传入登录帐号");
                if (string.IsNullOrWhiteSpace(password))
                    throw new FtException("请传入登录密码");
                var userRepository = new UserAccountRepository();
                var user = userRepository.UserLogin(userName, password, bool.Parse(rember + ""), int.Parse(plat + ""));
                if (user != null && user.ErrCode == 0)
                    return new ApiReturn
                    {
                        data = user.DataJson,
                        code = 0
                    };
                else
                    return new ApiReturn
                    {
                        errors = user.ErrMsg
                    };
            });
            return Json(ret);
        }

        /// <summary>
        /// 注销登录
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public IHttpActionResult Logout([FromBody] JObject data)
        {
            var ret = ApiFunc(data, "---User---Logout接口---", (d, p) =>
            {
                var dic = Sign.GetParameters(data.ToString());
                object token;
                if (!dic.TryGetValue("ticket", out token))
                    token = "";
                if (string.IsNullOrWhiteSpace(token + ""))
                    throw new FtException("请传入登录帐号");

                var userRepository = new UserAccountRepository();
                userRepository.UserLogout(token + "");
                return new ApiReturn
                {
                    code = 0
                };
            });
            return Json(ret);
        }

        /// <summary>
        /// 会员注册
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult Register([FromBody] JObject data)
        {
            var api = ApiFunc(data, "---User---Register接口---", (d, p) =>
            {
                var ret = new ApiReturn();
                var dic = Sign.GetParameters(data.ToString());
                var userName = dic["username"] + "";
                var password = dic["password"] + "";
                if (string.IsNullOrWhiteSpace(userName))
                    throw new FtException("请传入登录帐号");
                if (string.IsNullOrWhiteSpace(password))
                    throw new FtException("请传入登录密码");
                var userRepository = new UserAccountRepository();
                var user = userRepository.UserRegister(userName, password);
                if (user == RegisterEnum.Success)
                    ret.code = 0;
                else
                    ret.errors = user.ToDescription("cn");
                return ret;
            });
            return Json(api);
        }
        #endregion

        #region User Bet

        /// <summary>
        /// 用户投注
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult Bet([FromBody] JObject data)
        {
            var api = ApiFunc(data, "---User---Bet接口---", (d, p) =>
            {
                var ret = new ApiReturn();
                var dic = Sign.GetParameters(data.ToString());
                var userid = dic["userid"] + "";
                var matchuserbet = dic["matchuserbet"] + "";
                object lang, zone, source;
                if (!dic.TryGetValue("lang", out lang))
                    lang = "cn";
                if (!dic.TryGetValue("zone", out zone))
                    zone = "8";
                if (!dic.TryGetValue("source", out source))
                    source = "0";
                var userbet = JsonConvert.DeserializeObject<MatchUserBet>(matchuserbet,
                    new JsonSerializerSettings()
                    {
                        MissingMemberHandling = MissingMemberHandling.Ignore,
                        NullValueHandling = NullValueHandling.Ignore
                    });
                userbet.UserId = long.Parse(userid);
                userbet.BetId = GetOrderId();
                userbet.CreateDate = DateTime.Now;
                userbet.BetSource = source.ToString() == "1" ? DataSourceEnum.XBET : DataSourceEnum.HGA;
                userbet.BetTime = DateTimeHelper.DateTimeToUnixTimestamp(userbet.CreateDate, 0);
                userbet.MatchUserBetContent = userbet.MatchUserBetContent.Select(s => new MatchUserBetContent
                {
                    BetId = userbet.BetId,
                    MatchID = s.MatchID,
                    BetContent = s.BetContent,
                    BetType = s.BetType,
                    BetKey = s.BetKey,
                    BetScore = s.BetScore
                }).ToArray();
                var flag = new MatchUserBetRepository().UserBet(userbet, double.Parse(zone+""));
                if (flag == BetEnum.Success)
                {
                    ret.code = 0;
                    ret.data = new { BetId = userbet.BetId, CreateDate = userbet.CreateDate };
                }
                else
                {
                    ret.code = 1;
                    ret.errors = flag.ToDescription(lang + "");
                }
                return ret;
            });
            return Json(api);
        }

        /// <summary>
        /// 交易状况
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult BetRecord([FromBody] JObject data)
        {
            var api = ApiFunc(data, "---User---BetRecord接口---", (d, p) =>
            {
                var ret = new ApiReturn();
                var dic = Sign.GetParameters(data.ToString());
                var userid = dic["userid"] + "";
                var page = dic["page"] + "";
                var size = dic["size"] + "";
                object sdate, edate, lang, iset;
                if (!dic.TryGetValue("edate", out edate))
                    edate = DateTime.Today.ToString("yyyy-MM-dd");
                if (!dic.TryGetValue("sdate", out sdate))
                    sdate = DateTime.Today.AddDays(-7).ToString("yyyy-MM-dd");
                if (!dic.TryGetValue("lang", out lang))
                    lang = "cn";
                if (!dic.TryGetValue("iset", out iset))
                    iset = "0";
                var record = new MatchUserBetRepository().GetUserBetBonusList(new UserBetQueryModel
                {
                    UserId = long.Parse(userid),
                    Page = int.Parse(page),
                    PageSize = int.Parse(size),
                    StartDate = DateTime.Parse(sdate + ""),
                    EndDate = DateTime.Parse(edate + ""),
                    Language = lang + "",
                    IsSet = int.Parse(iset + "")
                });
                ret.code = 0;
                ret.data = record;
                ret.total = record.total;
                return ret;
            });
            return Json(api);
        }

        /// <summary>
        /// 用户投注
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult ContentDetail([FromBody] JObject data)
        {
            var api = ApiFunc(data, "---User---ContentDetail接口---", (d, p) =>
            {
                var ret = new ApiReturn();
                var dic = Sign.GetParameters(data.ToString());
                var betid = dic["betid"] + "";
                object lang;
                if (!dic.TryGetValue("lang", out lang))
                    lang = "cn";
                var grid = new MatchUserBetContentRepository().BetDetail(long.Parse(betid), lang + "");
                if (grid.Any())
                {
                    ret.code = 0;
                    ret.data = grid;
                }
                else
                    ret.code = 1;
                return ret;
            });
            return Json(api);
        }

        #endregion

        #region User Bank

        /// <summary>
        /// 获取提现账户
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult UserBank([FromBody]JObject data)
        {
            var api = ApiFunc(data, "---User---UserBank接口---", (d, p) =>
            {
                var ret = new ApiReturn();
                var dic = Sign.GetParameters(data.ToString());
                var userid = dic["userid"] + "";
                var userBank = new UserBankRepository();
                var uid = long.Parse(userid);
                ret.data = userBank.GetList(s => s.UserId == uid).Select(s => new
                {
                    Id = s.Id,
                    UserId = s.UserId,
                    RealName = s.RealName,
                    BankType = s.BankType,
                    BankCardNum = s.BankCardNum,
                    BankBranch = s.BankBranch
                }).OrderByDescending(s => s.Id).ToList();
                ret.code = 0;
                return ret;
            });
            return Json(api);
        }

        /// <summary>
        /// 添加提现账户
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult AddUserBank([FromBody] JObject data)
        {
            var api = ApiFunc(data, "---User---AddUserBank接口---", (d, p) =>
            {
                var ret = new ApiReturn();
                var dic = Sign.GetParameters(data.ToString());
                var userid = dic["userid"] + "";
                var realname = dic["realname"] + "";
                var banktype = dic["banktype"] + "";
                var bankcardnum = dic["bankcardnum"] + "";
                var bankbranch = dic["bankbranch"] + "";
                var userBank = new UserBankRepository();
                var flag = userBank.Insert(new UserBankInfo
                {
                    UserId = long.Parse(userid),
                    RealName = HttpUtility.UrlDecode(realname),
                    BankType = HttpUtility.UrlDecode(banktype),
                    BankCardNum = bankcardnum,
                    BankBranch = HttpUtility.UrlDecode(bankbranch)
                });
                ret.code = flag ? 0 : 1;
                return ret;
            });
            return Json(api);
        }

        /// <summary>
        /// 删除提现账户
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult DelUserBank([FromBody]JObject data)
        {
            var api = ApiFunc(data, "---User---DelUserBank接口---", (d, p) =>
            {
                var ret = new ApiReturn();
                var dic = Sign.GetParameters(data.ToString());
                var userid = long.Parse(dic["userid"] + "");
                var bankid = long.Parse(dic["bankid"] + "");
                var flag = new UserBankRepository().Delete(s => s.Id == bankid && s.UserId == userid);
                ret.code = flag ? 0 : 1;
                return ret;
            });
            return Json(api);
        }

        [HttpPost]
        public IHttpActionResult EditUserBank([FromBody]JObject data)
        {
            var api = ApiFunc(data, "---User---EditUserBank接口---", (d, p) =>
            {
                var ret = new ApiReturn();
                var dic = Sign.GetParameters(data.ToString());
                var userid = dic["userid"] + "";
                var bankid = dic["bankid"] + "";
                var realname = dic["realname"] + "";
                var banktype = dic["banktype"] + "";
                var bankcardnum = dic["bankcardnum"] + "";
                var bankbranch = dic["bankbranch"] + "";
                var uid = long.Parse(userid); var bid = long.Parse(bankid);
                var userBank = new UserBankRepository();
                var flag = userBank.Update(s => s.UserId == uid && s.Id == bid, f => (new UserBankInfo
                {
                    RealName = HttpUtility.UrlDecode(realname),
                    BankType = HttpUtility.UrlDecode(banktype),
                    BankCardNum = bankcardnum,
                    BankBranch = HttpUtility.UrlDecode(bankbranch)
                }));
                ret.code = flag ? 0 : 1;
                return ret;
            });
            return Json(api);
        }

        [HttpPost]
        public IHttpActionResult SingleUserBank([FromBody]JObject data)
        {
            var api = ApiFunc(data, "---User---SingleUserBank接口---", (d, p) =>
            {
                var ret = new ApiReturn();
                var dic = Sign.GetParameters(data.ToString());
                var bankid = dic["bankid"] + "";
                var bid = long.Parse(bankid);
                var userBank = new UserBankRepository().Find(s => s.Id == bid);
                ret.data = new
                {
                    Id = userBank.Id,
                    BankBranch = userBank.BankBranch,
                    BankCardNum = userBank.BankCardNum,
                    BankType = userBank.BankType,
                    RealName = userBank.RealName,
                    UserId = userBank.UserId
                };
                ret.code = 0;
                return ret;
            });
            return Json(api);
        }

        #endregion

        #region User Center
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult Center([FromBody] JObject data)
        {
            var api = ApiFunc(data, "---User---Center接口---", (d, p) =>
            {
                var ret = new ApiReturn();
                var dic = Sign.GetParameters(data.ToString());
                var userid = dic["userid"] + "";
                var uid = long.Parse(userid);
                var user = new UserAccountRepository().Find(s => s.Id == uid);
                if (user != null)
                {
                    ret.data = new
                    {
                        Id = user.Id,
                        BalanceAmount = user.BalanceAmount,
                        LoginName = user.LoginName,
                        HasSafePass=!string.IsNullOrEmpty(user.SafePassword)
                    };
                    ret.code = 0;
                }
                return ret;
            });
            return Json(api);
        }
        /// <summary>
        /// 转账,充值
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult Trade([FromBody] JObject data)
        {
            var api = ApiFunc(data, "---User---Trade接口---", (d, p) =>
            {
                var ret = new ApiReturn();
                var dic = Sign.GetParameters(data.ToString());
                var type = dic["type"] + "";
                object fuser, tuser, card, amount, lang;
                if (!dic.TryGetValue("fuser", out fuser))
                    fuser = 0;
                if (!dic.TryGetValue("tuser", out tuser))
                    tuser = "";
                if (!dic.TryGetValue("card", out card))
                    card = "";
                if (!dic.TryGetValue("amount", out amount))//交易金额(充值取卡面金额)
                    amount = 0;
                if (!dic.TryGetValue("lang", out lang))
                    lang = "cn";
                var userRepository = new UserAccountRepository();
                TransAccountsEnum trad;
                if (type.Equals("1"))
                {
                    var code = dic["code"] + "";
                    if (decimal.Parse(amount + "") <= 0)
                        throw new FtException("请输入转账金额");
                    if (string.IsNullOrEmpty(tuser + ""))
                        throw new FtException("请输入对方账户");
                    trad = userRepository.UserTrad((TradEnum) int.Parse(type), long.Parse(fuser + ""), tuser + "",
                        decimal.Parse(amount + ""), "", "", code + "");
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(card + ""))
                        throw new FtException("请输入充值卡号");
                    trad = userRepository.UserTrad((TradEnum)int.Parse(type), 0, tuser + "", 0, card + "", "");
                }
                if (trad == TransAccountsEnum.Success)
                    ret.code = 0;
                else
                    ret.errors = trad.ToDescription(lang + "");
                return ret;
            });
            return Json(api);
        }

        /// <summary>
        /// 用户提现
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult Cash([FromBody] JObject data)
        {
            var api = ApiFunc(data, "---User---Cash接口---", (d, p) =>
            {
                var ret = new ApiReturn();
                var dic = Sign.GetParameters(data.ToString());
                object lang;
                var userid = dic["userid"] + "";
                var realname = dic["realname"] + "";
                var bankcardnum = dic["bankcardnum"] + "";
                var amount = dic["amount"] + "";
                var code = dic["code"] + "";
                if (!dic.TryGetValue("lang", out lang))
                    lang = "cn";
                object banktype, bankbranch;
                if (!dic.TryGetValue("banktype", out banktype))
                    banktype = "";
                if (!dic.TryGetValue("bankbranch", out bankbranch))
                    bankbranch = "";
                var cash = new CashRecordRepository().UserCashInfo(new CashRecord
                {
                    UserId = long.Parse(userid),
                    BankType = HttpUtility.UrlDecode(banktype + ""),
                    BankCardNum = bankcardnum + "",
                    RealName = HttpUtility.UrlDecode(realname),
                    BankBranch = HttpUtility.UrlDecode(bankbranch + ""),
                    Amount = decimal.Parse(amount),
                    CreateTime = DateTime.Now
                }, code);
                ret.code = cash == TransAccountsEnum.Success ? 0 : 1;
                ret.errors = cash.ToDescription(lang + "");
                return ret;
            });
            return Json(api);
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult EditPass([FromBody] JObject data)
        {
            var api = ApiFunc(data, "---User---EditPass接口---", (d, p) =>
            {
                var ret = new ApiReturn();
                var dic = Sign.GetParameters(data.ToString());
                var userid = dic["userid"] + "";
                var npwd = dic["npwd"] + ""; //:TODO
                var pwd = dic["pwd"] + ""; //:TODO
                object lang, type;
                if (!dic.TryGetValue("lang", out lang))
                    lang = "cn";
                if (!dic.TryGetValue("type", out type))
                    type = "3";
                if (string.IsNullOrWhiteSpace(userid))
                    throw new FtException("登录信息失效");
                if (string.IsNullOrWhiteSpace(pwd + ""))
                    throw new FtException("请传入旧密码");
                if (string.IsNullOrWhiteSpace(npwd))
                    throw new FtException("请传入新密码");
                var userRepository = new UserAccountRepository();
                var user = userRepository.ModifyPass(long.Parse(userid), pwd, npwd, int.Parse(type + ""));
                if (user == EditEnum.Success)
                    ret.code = 0;
                else
                    ret.errors = user.ToDescription(lang + "");
                return ret;
            });
            return Json(api);
        }

        /// <summary>
        /// 账户历史
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult History([FromBody] JObject data)
        {
            var api = ApiFunc(data, "---User---History接口---", (d, p) =>
            {
                var ret = new ApiReturn();
                var dic = Sign.GetParameters(data.ToString());
                var userid = dic["userid"] + "";
                object sdate, edate;
                if (!dic.TryGetValue("edate", out edate))
                    edate = DateTime.Today;
                if (!dic.TryGetValue("sdate", out sdate))
                    sdate = DateTime.Today.AddDays(-6);
                var history = new UserAccountRepository().GetTradHistory(new UserBetQueryModel
                {
                    UserId = long.Parse(userid),
                    StartDate = DateTime.Parse(sdate + ""),
                    EndDate = DateTime.Parse(edate + "")
                });
                ret.data = history;
                ret.code = 0;
                return ret;
            });
            return Json(api);
        }
        #endregion

        /// <summary>
        /// 兑奖
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult UserBetGet([FromBody] JObject data)
        {
            var api = ApiFunc(data, "---User---UserBetGet接口---", (d, p) =>
            {
                var ret = new ApiReturn();
                var dic = Sign.GetParameters(data.ToString());
                var userid = dic["userid"] + "";
                var betid = dic["betid"] + "";
                object lang;
                if (!dic.TryGetValue("lang", out lang))
                    lang = "cn";
                var prize = new MatchUserBetRepository().UserBetGet(long.Parse(userid), long.Parse(betid));
                if (prize.Flag == PrizeEnum.Success)
                {
                    ret.code = 0;
                    ret.data = new { BetBonus = prize.BetBonus };
                }
                else
                {
                    ret.code = (int)prize.Flag;
                    ret.errors = ((PrizeEnum)prize.Flag).ToDescription(lang + "");
                }
                return ret;
            });
            return Json(api);
        }
    }
}
