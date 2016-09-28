using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using EntityFramework.Extensions;
using FT.Entities;
using FT.IRepository;
using FT.Model;
using FT.Model.ViewModel;
using FT.Utility.CacheHelper;
using FT.Utility.Helper;
namespace FT.Repository
{
    public class AdminRepository : BaseRepository<Admin>, IAdminRepository
    {
        public ManagerResponse ManagerLogin(string username, string password, bool remember = false)
        {
            var response = new ManagerResponse();
            return QueryDb((context) =>
            {
                var manager =
                    context.Admin.FirstOrDefault(s => s.LoginName.Equals(username));
                if (manager != null)
                {
                    if (manager.Status == LockEnum.Nnormal)
                    {
                        var infoPass = SecureHelper.Md5(SecureHelper.Md5(password) + manager.PasswordSalt);
                        if (infoPass.Equals(manager.Password))
                        {
                            var tokenId = Guid.NewGuid().ToString();
                            response.Admin = manager;
                            response.ErrCode = 0;
                            response.Ticket = EncryptHelper.AesEncrypt(SecureHelper.Md5(tokenId));
                            //response.DataJson = Newtonsoft.Json.JsonConvert.SerializeObject(response);
                            if (!remember)
                                AspNetCache.Insert(response.Ticket, response.Admin, response.ExpiresIn);
                            else
                                AspNetCache.Insert(response.Ticket, response.Admin, 7 * 60 * 60);
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
        public AngularTable<AdminViewModel> GetAdmin(Model.QueryModel.AdminQueryModel query)
        {
            var table = new AngularTable<AdminViewModel>();

            return QueryDb((context) =>
            {
                #region 条件拼接

                var predicate = PredicateBuilderUtility.True<Admin>();
                if (query.RoleId.HasValue)
                    predicate = predicate.And(s => s.RoleId == query.RoleId.Value);
                if (!string.IsNullOrEmpty(query.UserName))
                    predicate = predicate.And(s => s.LoginName.Equals(query.UserName));
                if (query.StartDate.HasValue && query.EndDate.HasValue)
                    predicate = predicate.And(s => s.CreateDate > query.StartDate && s.CreateDate < query.EndDate);

                #endregion

                var data = context.Admin.FindBy(predicate, query.Page.Value, query.PageSize.Value, out Total,
                    query.SortField,
                    query.IsDesc).ToList().Select(s => new AdminViewModel
                    {
                        Id = s.Id,
                        LoginName = s.LoginName,
                        Status = s.Status.ToDescription(query.Language),
                        CreateDate = s.CreateDate,
                        RoleName = s.Role != null ? s.Role.RoleName : string.Empty
                    }).ToList();
                table.total = Total;
                table.data = data;
                return table;
            }, table);
        }
        public EasyDataGrid<AdminViewModel> GetAdminGrid(Model.QueryModel.AdminQueryModel query)
        {
            var table = new EasyDataGrid<AdminViewModel>();
            return QueryDb((context) =>
            {
                #region 条件拼接
                var predicate = PredicateBuilderUtility.True<Admin>();
                predicate = predicate.And(s => s.Status != LockEnum.Delete);
                //if (query.RoleId.HasValue)
                //    predicate = predicate.And(s => s.RoleId == query.RoleId.Value);
                if (!string.IsNullOrEmpty(query.UserName))
                    predicate = predicate.And(s => s.LoginName.Equals(query.UserName));
                if (query.StartDate.HasValue && query.EndDate.HasValue)
                    predicate = predicate.And(s => s.CreateDate > query.StartDate && s.CreateDate < query.EndDate);
                #endregion
                var menu = new RoleRepository().GetRoleOperateCell(query.RoleId.Value).Where(s => s.MenuId == query.MenuId).Select(s => s.OperateName);
                var data = context.Admin.FindBy(predicate, query.Page.Value, query.PageSize.Value, out Total,
                    query.SortField,
                    query.IsDesc).ToList().Select(s => new AdminViewModel
                    {
                        Id = s.Id,
                        LoginName = s.LoginName,
                        Status = s.Status.ToDescription(query.Language),
                        CreateDate = s.CreateDate,
                        RoleName =
                            s.Role != null
                                ? (query.Language.Equals("cn") ? s.Role.RoleName : s.Role.RoleNameEn)
                                : string.Empty,
                        RoleId = s.RoleId,
                        Cell = GetRoleCell(s, menu)
                    }).ToList();
                table.total = Total;
                table.rows = data;
                return table;
            }, table);
        }
        private string GetRoleCell(Admin info, IEnumerable<string> menu)
        {
            var sb = new StringBuilder();
            if (menu.Any())
            {
                var lockNum = info.Status == LockEnum.Nnormal ? 1 : 0;
                var lockText = lockNum == 1 ? "冻结" : "解冻";
                if (menu.Contains("cel_FrzAdmin"))
                    sb.AppendFormat(
                        "<a href='javascript:void(0)' class=\"good-check\" onclick=\"admin.editStaus('{0}','{1}')\">{2}</a> ",
                        info.Id, lockNum, lockText);
                if (menu.Contains("cel_ResetPass") && info.Status==LockEnum.Nnormal)
                    sb.AppendFormat(
                        "<a href='javascript:void(0)' class=\"good-check\" onclick=\"admin.resetPass('{0}')\">重置密码</a> ",
                        info.Id);
            }
            return sb.ToString();
        }
        public AdminViewModel SingleAdmin(Expression<Func<Admin, bool>> pried,string language="cn")
        {
            var admin = new AdminViewModel();
            return QueryDb((context) =>
            {
                var manager = context.Admin.FirstOrDefault(pried);
                if (manager != null)
                    admin = new AdminViewModel
                    {
                        AgentId = manager.AgentId,
                        AgentLevel = manager.UserAccount == null ? 0 : (int) manager.UserAccount.Level,
                        Id = manager.Id,
                        LoginName = manager.LoginName,
                        Status = manager.Status.ToDescription(language),
                        RoleId = manager.RoleId,
                        RoleName = manager.Role.RoleName,
                        ParentPath = manager.UserAccount == null ? string.Empty : manager.UserAccount.ParentPath,
                        CreditLimit = manager.UserAccount == null ? 0 : manager.UserAccount.CreditLimit
                    };
                return admin;
            }, admin);
        }
        public EditEnum ModifyPass(int userid, string password, string newpassword,bool useoldpass=true)
        {
            return QueryDb((context) =>
            {
                var user = context.Admin.FirstOrDefault(s => s.Id == userid);
                if (user != null)
                {
                    if (user.Status == 0)
                    {
                        var salt = user.PasswordSalt;
                        if (useoldpass)
                        {
                            var oldpass = SecureHelper.Md5(SecureHelper.Md5(password) + salt);
                            if (user.Password.Equals(oldpass))
                            {
                                if (context.Admin.Update(w => w.Id == userid, u => new Admin
                                {
                                    Password = SecureHelper.Md5(SecureHelper.Md5(newpassword) + salt)
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
                            if (context.Admin.Update(w => w.Id == userid, u => new Admin
                            {
                                Password = SecureHelper.Md5(SecureHelper.Md5(newpassword) + salt)
                            }) > 0)
                                return EditEnum.Success;
                            else
                                return EditEnum.Failure;
                        }
                    }
                    else
                        return EditEnum.IsLock;
                }
                else
                    return EditEnum.AccountNotFind;
            }, EditEnum.Failure);
        }
        public bool FrozenOrDeleteAdmin(int adminid, LockEnum state)
        {
            return QueryDb((context) =>
            {
                return context.Admin.Update(w => w.Id == adminid, u => new Admin
                {
                    Status = state
                }) > 0;
            }, false);
        }
        public bool ModifyAdmin(Admin admin)
        {
            return QueryDb((context) =>
            {
                var exists = context.Admin.Any(s => s.Id != admin.Id && s.LoginName.Equals(admin.LoginName));
                if (!exists)
                {
                    return context.Admin.Where(s => s.Id == admin.Id).Update(w => new Admin
                    {
                        LoginName = admin.LoginName
                    }) > 0;
                }
                return false;
            }, false);
        }
    }
}