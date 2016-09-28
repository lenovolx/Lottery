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
using System.Linq.Expressions;
using FT.Utility.ExtendException;


namespace FT.Repository
{
    public class CashRecordRepository : BaseRepository<CashRecord>, ICashRecordRepository
    {
        public TransAccountsEnum UserCashInfo(CashRecord cash, string code)
        {
            var flag = TransAccountsEnum.Failure;
            return QueryDbUseTran((context) =>
            {
                if (cash == null && cash.UserId <= 0) return TransAccountsEnum.CashNullInvalid;
                var user = context.UserAccount.FirstOrDefault(s => s.Id == cash.UserId);
                if (user == null) return TransAccountsEnum.UserInvalid;
                if (user.BalanceAmount < cash.Amount.Value) return TransAccountsEnum.BalanceLack;
                if (string.IsNullOrEmpty(code) ||
                    !user.SafePassword.Equals(SecureHelper.Md5(code + user.PasswordSalt)))
                    return TransAccountsEnum.SafeCodeInvalid;
                context.CashRecord.Add(cash);
                user.BalanceAmount += -cash.Amount.Value;
                //添加金额流水
                context.AmountWater.Add(new AmountWater
                {
                    CreateDate = DateTime.Now,
                    UserId = user.Id,
                    UserName = user.LoginName,
                    Type = TradEnum.Cash,
                    Amount = -cash.Amount.Value,
                    Remark = "提现扣除"
                });
                return TransAccountsEnum.Success;
            }, flag);
        }

        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public EasyDataGrid<CashRecordViewModel> CashRecordGrid(CashRecordQueryModel query)
        {
            var grid = new EasyDataGrid<CashRecordViewModel>();
            return QueryDb((context) =>
            {
                var predicate = PredicateBuilderUtility.True<CashRecord>();
                if (query.Status.HasValue && query.Status.Value != -1)
                    predicate = predicate.And(p => p.Status == (CashAuditEnum)query.Status);
                if (query.StartDate.HasValue && query.EndDate.HasValue)
                {
                    var endtime = query.EndDate.Value.AddDays(1);
                    predicate =
                        predicate.And(
                            p => p.CreateTime > query.StartDate && p.CreateTime < endtime);
                }
                grid.rows =
                  context.CashRecord.FindBy(predicate, query.Page.Value, query.PageSize.Value, out Total, query.SortField, query.IsDesc).ToArray()
                      .Select(s => new CashRecordViewModel
                      {
                          Id = s.Id,
                          UserId = s.UserId,
                          UserName = s.UserAccount.LoginName,
                          BankType = s.BankType,
                          BankCardNum = s.BankCardNum,
                          RealName = s.RealName,
                          BankBranch = s.BankBranch,
                          Amount = s.Amount.Value,
                          CreateTime = s.CreateTime,
                          StatusNum = (int)s.Status,
                          Status = s.Status.ToDescription(query.Language),
                          RejectRemark = s.RejectRemark
                      }).ToList();
                grid.total = Total;

                return grid;
            }, grid);
        }
        //更新
        public bool AuditCashRecord(CashRecord model)
        {
            var flag = false;
            return QueryDbUseTran((context) =>
            {
                var cash = context.CashRecord.FirstOrDefault(s => s.Id == model.Id);
                if (cash != null)
                {
                    cash.Status = model.Status;
                    cash.RejectRemark = model.RejectRemark;
                    cash.AuditorName = model.AuditorName;
                    cash.AuditTime = DateTime.Now;
                    if (model.Status == CashAuditEnum.NoPass)
                    {
                        var user = context.UserAccount.FirstOrDefault(s => s.Id == model.UserId);
                        if (user != null)
                        {
                            user.BalanceAmount = user.BalanceAmount + model.Amount.Value;
                            //添加金额流水
                            context.AmountWater.Add(new AmountWater
                            {
                                CreateDate = DateTime.Now,
                                UserId = user.Id,
                                UserName = user.LoginName,
                                Type = TradEnum.Cash,
                                Amount = model.Amount.Value,
                                Remark = "提现审核不通过,返还提现金额"
                            });
                        }
                        else
                            throw new FtException("用户不存在");
                    }
                    flag = true;
                }
                return flag;
            }, flag);
        }
    }
}
