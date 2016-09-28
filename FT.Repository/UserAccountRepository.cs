using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using FT.IRepository;
using FT.Model;
using FT.Utility.CacheHelper;
using FT.Utility.Helper;
using EntityFramework.Extensions;
using FT.Model.ViewModel;
using FT.Entities;
using FT.Model.QueryModel;
using FT.Plugin.Cache;


namespace FT.Repository
{
    public class UserAccountRepository : BaseRepository<UserAccount>, IUserAccountRepository
    {
        public UserResponse UserLogin(string username, string password, bool remember = false, int plat = 0)
        {
            var response = new UserResponse();
            return QueryDb((context) =>
            {
                var user =
                    context.UserAccount.FirstOrDefault(s => s.LoginName.Equals(username));
                if (user != null)
                {
                    if (user.Status == LockEnum.Nnormal)
                    {
                        var infoPass = SecureHelper.Md5(password + user.PasswordSalt);
                        if (infoPass.Equals(user.Password))
                        {
                            var tokenId = Guid.NewGuid().ToString();
                            response.User = new UserViewModel
                            {
                                Id = user.Id,
                                LoginName = user.LoginName,
                                BalanceAmount = user.BalanceAmount,
                                TypeName = (int) user.Type,
                                LevelName = (int) user.Level
                            };
                            response.System = new SystemSettingRepository().GetSetting();
                            response.ErrCode = 0;
                            response.Ticket = string.Format("user{0}_{1}", user.Id,
                                EncryptHelper.AesEncrypt(SecureHelper.Md5(tokenId)));
                            response.DataJson = Newtonsoft.Json.JsonConvert.SerializeObject(response);
                            Cache.TimeOut(string.Format("user{0}", user.Id));
                            if (!remember)
                                Cache.Add(response.Ticket, response.DataJson, response.ExpiresIn);
                            else
                                Cache.Add(response.Ticket, response.DataJson, 24*60*7);
                        }
                        else
                        {
                            response.ErrCode = -2;
                            response.ErrMsg = "账户名密码错误";
                        }
                    }
                    else
                    {
                        response.ErrCode = -1;
                        response.ErrMsg = "帐号被锁定,请联系管理员";
                    }
                }
                else
                {
                    response.ErrCode = -2;
                    response.ErrMsg = "账户名密码错误";
                }
                return response;
            }, response);
        }

        public RegisterEnum UserRegister(string username, string password, PlatEnum plat = PlatEnum.Mobile, bool passmd5 = true)
        {
            return QueryDb((context) =>
            {
                var user = context.UserAccount.FirstOrDefault(s => s.LoginName.Equals(username));
                if (user != null)
                    return RegisterEnum.Existing;
                else
                {
                    var salt = Guid.NewGuid().ToString();
                    context.UserAccount.Add(new UserAccount
                    {
                        LoginName = username,
                        PasswordSalt = salt,
                        Password =
                            passmd5
                                ? SecureHelper.Md5(password + salt)
                                : SecureHelper.Md5(SecureHelper.Md5(password) + salt),
                        PlatFrom = plat,
                        BalanceAmount = 0,
                        Type = UserTypeEnum.Ordinary,
                        Level = LevelEnum.None,
                        CreateDate = DateTime.Now
                    });
                    if (context.SaveChanges() > 0)
                        return RegisterEnum.Success;
                    else
                        return RegisterEnum.Failure;
                }
            });
        }
        public EasyDataGrid<UserViewModel> GetUserGrid(UserQueryModel query)
        {
            return QueryDb((context) =>
            {
                var table = new EasyDataGrid<UserViewModel>();

                #region 条件拼接

                var @where = PredicateBuilderUtility.True<UserAccount>();
                @where = @where.And(s => s.Status != LockEnum.Delete);
                if (!string.IsNullOrEmpty(query.UserName))
                    where = where.And(s => s.LoginName.Contains(query.UserName));
                if (query.StartDate.HasValue && query.EndDate.HasValue)
                    where = where.And(s => s.CreateDate > query.StartDate && s.CreateDate < query.EndDate);
                if (query.AgentId.HasValue && query.AgentId.Value != 0)
                    where = where.And(s => ("|" + s.ParentPath + "|").Contains("|" + query.AgentId.Value + "|"));
                if (query.AgentLevel.HasValue && query.AgentLevel.Value != 0)
                {
                    var level = query.AgentLevel.Value < 3
                        ? (LevelEnum) (query.AgentLevel.Value + 1)
                        : LevelEnum.ThreeAgent;
                    where = where.And(s => s.Level == level);
                }

                #endregion

                var data = context.UserAccount.FindBy(where, query.Page.Value, query.PageSize.Value, out Total,
                    query.SortField,
                    query.IsDesc).ToList().Select(s => new UserViewModel
                    {
                        Id = s.Id,
                        LoginName = s.LoginName,
                        BalanceAmount = s.BalanceAmount,
                        Status = (int)s.Status,
                        Phone = s.Phone,
                        email = s.email,
                        CreateDate = s.CreateDate,
                        TypeName = (int)s.Type,
                        Type = s.Type.ToDescription(query.Language),
                        Level = s.Level.ToDescription(query.Language),
                        LevelName = (int)s.Level,
                        StatusName = s.Status.ToDescription(query.Language),
                        ParentPath=s.ParentPath,
                        CreditLimit=s.CreditLimit,
                        ReturnRate = s.ReturnRate
                    }).ToList();
                table.total = Total;
                table.rows = data;
                return table;
            });
        }
        private string GetRoleCell(UserAccount info,IEnumerable<string> cell)
        {
            var sb = new StringBuilder();
            if (cell.Any())
            {
                var lockNum = info.Status == LockEnum.Nnormal ? 1 : 0;
                var lockText = lockNum == 1 ? "冻结" : "解冻";
                if (cell.Contains("cel_LockUser"))
                    sb.AppendFormat("<a href='javascript:void(0)' class=\"good-check\" onclick=\"user.editStaus('" + info.Id + "','" + lockNum + "')\">" + lockText + "</a> ");
            }
            return sb.ToString();
        }
        public EditEnum ModifyPass(long userid, string password, string newpassword, int passType = 0)
        {
            return QueryDb((context) =>
            {
                var user = context.UserAccount.FirstOrDefault(s => s.Id == userid);
                if (user != null)
                {
                    if (user.Status == 0)
                    {
                        var salt = user.PasswordSalt;
                        if (passType == 3)
                        {
                            var oldpass = SecureHelper.Md5(password + salt);
                            if (user.Password.Equals(oldpass))
                            {
                                if (context.UserAccount.Update(w => w.Id == userid, u => new UserAccount
                                {
                                    Password = SecureHelper.Md5(newpassword + salt)
                                }) > 0)
                                    return EditEnum.Success;
                                else
                                    return EditEnum.Failure;
                            }
                            else
                                return EditEnum.OriginalPassError;
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(password))
                            {
                                var oldpass = SecureHelper.Md5(password + salt);
                                if (user.SafePassword.Equals(oldpass))
                                {
                                    if (context.UserAccount.Update(w => w.Id == userid, u => new UserAccount
                                    {
                                        SafePassword = SecureHelper.Md5(newpassword + salt)
                                    }) > 0)
                                        return EditEnum.Success;
                                    else
                                        return EditEnum.Failure;
                                }
                                else
                                    return EditEnum.OriginalPassError;
                            }
                            else
                            {
                                if (context.UserAccount.Update(w => w.Id == userid, u => new UserAccount
                                {
                                    SafePassword = SecureHelper.Md5(newpassword + salt)
                                }) > 0)
                                    return EditEnum.Success;
                                else
                                    return EditEnum.Failure;
                            }
                        }
                    }
                    else
                        return EditEnum.IsLock;
                }
                else
                    return EditEnum.AccountNotFind;
            }, EditEnum.Failure);
        }
        public TransAccountsEnum UserTrad(TradEnum type, long formuser, string touser = "", decimal amount = 0, string card = "", string pass = "",string code="")
        {
            return QueryDbUseTran((context) =>
            {
                var status = TransAccountsEnum.Success;
                if (type == TradEnum.Transfer)
                {                    
                    var fuser = context.UserAccount.FirstOrDefault(s => s.Id == formuser);
                    if (fuser != null && fuser.Status == LockEnum.Nnormal)
                    {
                        if (!string.IsNullOrEmpty(code) &&
                            fuser.SafePassword.Equals(SecureHelper.Md5(code + fuser.PasswordSalt)))
                        {
                            if (fuser.BalanceAmount >= amount)
                            {
                                touser = HttpUtility.UrlDecode(touser);
                                var tuser = context.UserAccount.FirstOrDefault(s => s.LoginName.Equals(touser));
                                if (tuser != null && tuser.Status == LockEnum.Nnormal)
                                {
                                    if (context.UserAccount.Update(s => s.Id == formuser, w => new UserAccount
                                    {
                                        BalanceAmount = fuser.BalanceAmount - amount
                                    }) > 0)
                                    {
                                        if (context.UserAccount.Update(s => s.Id == tuser.Id, w => new UserAccount
                                        {
                                            BalanceAmount = tuser.BalanceAmount + amount
                                        }) > 0)
                                        {
                                            context.TradeRecord.Add(new TradeRecord
                                            {
                                                Id = GetOrderId(),
                                                FromId = fuser.Id,
                                                FromUserName = fuser.LoginName,
                                                ToId = tuser.Id,
                                                ToUserName = tuser.LoginName,
                                                TradeAmount = amount,
                                                CreateDate = DateTime.Now,
                                                CreateUserId = formuser,
                                                Type = TradEnum.Transfer
                                            });
                                            //添加金额流水
                                            context.AmountWater.AddRange(new List<AmountWater>
                                            {
                                                new AmountWater
                                                {
                                                    CreateDate = DateTime.Now,
                                                    UserId = fuser.Id,
                                                    UserName = fuser.LoginName,
                                                    Type = TradEnum.Transfer,
                                                    Amount = -amount,
                                                    Remark =
                                                        string.Format("{0} 转出金额 {2} 至 {1}", fuser.LoginName,
                                                            tuser.LoginName, amount)
                                                },
                                                new AmountWater
                                                {
                                                    CreateDate = DateTime.Now,
                                                    UserId = tuser.Id,
                                                    UserName = tuser.LoginName,
                                                    Type = TradEnum.Transfer,
                                                    Amount = amount,
                                                    Remark =
                                                        string.Format("{0} 收到 {1} 转入金额 {2}", tuser.LoginName,
                                                            fuser.LoginName, amount)
                                                }
                                            });
                                            status = TransAccountsEnum.Success;
                                        }
                                    }
                                }
                                else
                                    status = TransAccountsEnum.UserInvalid;
                            }
                            else
                                status = TransAccountsEnum.BalanceLack;
                        }
                        else status = TransAccountsEnum.SafeCodeInvalid;
                    }
                    else
                        status = TransAccountsEnum.UserInvalid;
                }
                else
                {
                    var userid = long.Parse(touser);
                    var infouser = context.UserAccount.FirstOrDefault(s => s.Id == userid);
                    if (infouser != null && infouser.Status == LockEnum.Nnormal)
                    {
                        var cardinfo =
                            context.Card.FirstOrDefault(s => s.CardNum.Equals(card) && s.Status==0);
                        if (cardinfo != null && cardinfo.IsUsed == 0)
                        {
                            amount = cardinfo.CardGroup.Amount;
                            if (context.UserAccount.Update(s => s.Id == infouser.Id, w => new UserAccount
                            {
                                BalanceAmount = infouser.BalanceAmount + amount
                            }) > 0)
                            {
                                cardinfo.IsUsed = YesNoEnum.Yes;
                                cardinfo.UseDate=DateTime.Now;
                                context.TradeRecord.Add(new TradeRecord
                                {
                                    Id = GetOrderId(),
                                    FromId = 0,
                                    ToId = infouser.Id,
                                    ToUserName = infouser.LoginName,
                                    TradeAmount = amount,
                                    CreateDate = DateTime.Now,
                                    CreateUserId = infouser.Id,
                                    Type = TradEnum.Recharge,
                                    CardNum = card
                                });
                                //添加金额流水
                                context.AmountWater.Add(new AmountWater
                                {
                                    CreateDate = DateTime.Now,
                                    UserId = infouser.Id,
                                    UserName = infouser.LoginName,
                                    Type = TradEnum.Recharge,
                                    Amount = amount,
                                    Remark = string.Format("{0}充值{2} 充值卡号{1}", infouser.LoginName, card, amount)
                                });
                                status = TransAccountsEnum.Success;
                            }
                        }
                        else 
                            status = TransAccountsEnum.CardInvalid;
                    }
                    else
                        status = TransAccountsEnum.UserInvalid;
                }
                return status;
            }, TransAccountsEnum.Failure);
        }
        public List<UserAccountHistoryViewModel> GetUserTradHistory(UserBetQueryModel query)
        {
            var view = new List<UserAccountHistoryViewModel>();
            return QueryDb((context) =>
            {
                var predicate = PredicateBuilderUtility.True<MatchUserBet>();
                predicate = predicate.And(s => s.IsSettle == OpenEnum.Settlemented);
                if (query.StartDate.HasValue && query.EndDate.HasValue && (query.StartDate.Value <= query.EndDate.Value))
                    predicate = predicate.And(s => true);
                else
                {
                    query.EndDate = DateTime.Today;
                    query.StartDate = query.EndDate.Value.AddDays(-6);
                }
                if (query.UserId.HasValue)
                    predicate = predicate.And(s => s.UserId == query.UserId.Value);
                var startUnix = DateTimeHelper.DateTimeToUnixTimestamp(query.StartDate.Value, 0);
                var endUnix =
                    DateTimeHelper.DateTimeToUnixTimestamp(
                        query.EndDate.Value.AddHours(23).AddMinutes(59).AddSeconds(59), 0);
                predicate = predicate.And(s => s.BetTime > startUnix && s.BetTime < endUnix);

                var bet = context.MatchUserBet.Where(predicate).ToArray().Select(s => new
                {
                    BetValue = s.BetValue,
                    BetBonus = s.BetBonus,
                    BetTime = DateTimeHelper.UnixTimestampToDateTime(s.BetTime).ToString("yyyy-MM-dd")
                }).ToList().GroupBy(g => g.BetTime, (i, v) => new
                {
                    Times = i,
                    Data = v
                }).ToArray();
                var dateY = TableY(6, query.EndDate);
                return (from time in dateY
                    join data in bet on time.Dates equals data.Times into joindata
                    from result in joindata.DefaultIfEmpty()
                    select new
                    {
                        DateTimes = time.Dates,
                        DateWeek = time.Weeks,
                        BetValue = result == null ? 0 : result.Data.Sum(s => s.BetValue),
                        BetBonus = result == null ? 0 : result.Data.Sum(s => s.BetBonus)
                    }).Select(s => new UserAccountHistoryViewModel
                    {
                        BetAmount = s.BetValue,
                        DateTimes = s.DateTimes,
                        DateWeek = s.DateWeek,
                        WinOrLose = Effamount(s.BetValue, s.BetBonus),
                        EffectiveAmount = Math.Floor(s.BetValue)
                    }).ToList();
            }, view);
        }
        public FootGrid<UserAccountHistoryViewModel, TotalTradViewModel> GetTradHistory(UserBetQueryModel query)
        {
            var grid = new FootGrid<UserAccountHistoryViewModel, TotalTradViewModel>();
            return QueryDb((context) =>
            {
                var predicate = PredicateBuilderUtility.True<MatchUserBet>();
                predicate = predicate.And(s => s.IsSettle == OpenEnum.Settlemented);
                if (query.StartDate.HasValue && query.EndDate.HasValue && (query.StartDate.Value <= query.EndDate.Value))
                    predicate = predicate.And(s => true);
                else
                {
                    query.EndDate = DateTime.Today;
                    query.StartDate = query.EndDate.Value.AddDays(-6);
                }
                if (query.UserId.HasValue)
                    predicate = predicate.And(s => s.UserId == query.UserId.Value);
                var startUnix = DateTimeHelper.DateTimeToUnixTimestamp(query.StartDate.Value, 0);
                var endUnix =
                    DateTimeHelper.DateTimeToUnixTimestamp(
                        query.EndDate.Value.AddDays(1));
                predicate = predicate.And(s => s.BetTime > startUnix && s.BetTime < endUnix);

                var bet = context.MatchUserBet.Where(predicate).ToArray().Select(s => new
                {
                    BetValue = s.BetValue,
                    BetBonus = s.BetBonus,
                    BetTime = DateTimeHelper.UnixTimestampToDateTime(s.BetTime).ToString("yyyy-MM-dd")
                }).ToList().GroupBy(g => g.BetTime, (i, v) => new
                {
                    Times = i,
                    Data = v
                }).ToArray();
                var dateY = TableY(6, query.EndDate);
                grid.Grid = (from time in dateY
                    join data in bet on time.Dates equals data.Times into joindata
                    from result in joindata.DefaultIfEmpty()
                    select new
                    {
                        DateTimes = time.Dates,
                        DateWeek = time.Weeks,
                        BetValue = result == null ? 0 : result.Data.Sum(s => s.BetValue),
                        BetBonus = result == null ? 0 : result.Data.Sum(s => s.BetBonus)
                    }).Select(s => new UserAccountHistoryViewModel
                    {
                        BetAmount = s.BetValue,
                        DateTimes = s.DateTimes,
                        DateWeek = s.DateWeek,
                        WinOrLose = Effamount(s.BetValue, s.BetBonus),
                        EffectiveAmount =
                            Effamount(s.BetValue, s.BetBonus) > 0 ? Math.Floor(Effamount(s.BetValue, s.BetBonus)) : 0
                    }).ToList();

                grid.Foot = new TotalTradViewModel
                {
                    SumBetAmount = grid.Grid.Sum(s => s.BetAmount),
                    SumEffectiveAmount = grid.Grid.Sum(s => s.EffectiveAmount),
                    SumWinOrLose = grid.Grid.Sum(s => s.WinOrLose)
                };
                return grid;
            }, grid);
        }
        private static IEnumerable<DayYaxis> TableY(int days, DateTime? endtime)
        {
            var dayY = new List<DayYaxis>();
            for (var i = days; i >= 0; i--)
            {
                var dateY = endtime.Value.AddDays(0 - i);
                dayY.Add(new DayYaxis
                {
                    Dates = dateY.ToString("yyyy-MM-dd"),
                    Amount = 0,
                    Weeks = DateTimeHelper.GetDayOfWeekCN(dateY)
                });
            }
            return dayY;
        }
        private static decimal Effamount(decimal betValue, decimal betBonus)
        {
            return betBonus - betValue;
        }

        public RegisterEnum AddUser(UserAccount model)
        {
            var flag = RegisterEnum.Failure;
            return QueryDbUseTran((context) =>
            {
                if (model != null)
                {
                    var existsuser = context.UserAccount.Any(s => s.LoginName.Equals(model.LoginName));
                    if (!existsuser)
                    {
                        var salt = Guid.NewGuid().ToString();
                        var pass = SecureHelper.Md5(SecureHelper.Md5(model.Password) + salt);
                        model.PasswordSalt = salt;
                        model.Password = pass;
                        model.SafePassword = pass;
                        model.BalanceAmount = 0;
                        model.CreateDate = DateTime.Now;
                        var user = context.UserAccount.Add(model);
                        context.SaveChanges();
                        if (model.Type == UserTypeEnum.Agent)
                        {
                            context.Admin.Add(new Admin
                            {
                                LoginName = string.Format("{0}_{1}", "agent", user.LoginName),
                                Password = user.Password,
                                PasswordSalt = user.PasswordSalt,
                                AgentId = user.Id,
                                CreateDate = DateTime.Now
                            });
                        }
                        flag = RegisterEnum.Success;
                    }
                    else
                        flag = RegisterEnum.Existing;
                }
                return flag;
            }, flag);
        }

        public bool FrozenOrDeleteUser(long userid, LockEnum state)
        {
            return QueryDb((context) =>
            {
                return context.UserAccount.Where(w => w.Id == userid).Update(u => new UserAccount
                {
                    Status = state
                }) > 0;
            }, false);
        }

        public RegisterEnum EditUser(UserAccount model)
        {
            var flag = RegisterEnum.Failure;
            return QueryDb((context) =>
            {
                var exists = context.UserAccount.Any(s => s.Id != model.Id && s.LoginName.Equals(model.LoginName));
                if (!exists)
                {
                    if (context.UserAccount.Where(s => s.Id == model.Id).Update(w => new UserAccount
                    {
                        LoginName = model.LoginName,
                        email = model.email,
                        Phone = model.Phone,
                        CreditLimit = model.CreditLimit,
                        ReturnRate = model.ReturnRate
                    }) > 0) 
                        flag = RegisterEnum.Success;
                }
                else
                    flag = RegisterEnum.Existing;
                return flag;
            }, flag);
        }

        public void UserLogout(string token)
        {
            Cache.Remove(token);
        }
    }
}
