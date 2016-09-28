using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
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
    public class RoleRepository : BaseRepository<Role>, IRoleRepository
    {
        public dynamic GetRole(Expression<Func<Role, bool>> predi)
        {
            if (AspNetCache.Get(CacheKeyCollection.SysRoleCache) != null)
                return AspNetCache.Get<dynamic>(CacheKeyCollection.SysRoleCache);
            else
            {
                return QueryDb((context) =>
                {
                    var obj = context.Role.Where(predi).ToList().Select(s => new
                    {
                        RoleName = s.RoleName,
                        Id = s.Id
                    });
                    AspNetCache.Insert(CacheKeyCollection.SysRoleCache, obj, DateTime.Now.AddMinutes(30));
                    return obj;
                });
            }
        }
        public AngularTable<RoleViewModel> GetRoleList(Model.QueryModel.RoleQueryModel query)
        {
            return QueryDb((context) =>
            {
                var table = new AngularTable<RoleViewModel>();

                #region 条件拼接

                var predicate = PredicateBuilderUtility.True<Role>();
                if (!string.IsNullOrEmpty(query.RoleName))
                    predicate = predicate.And(s => s.RoleName.Equals(query.RoleName));
                #endregion

                var data = context.Role.FindBy(predicate, query.Page.Value, query.PageSize.Value, out Total,
                    query.SortField,
                    query.IsDesc).ToList().Select(s => new RoleViewModel
                    {
                        Id = s.Id,
                        RoleName = s.RoleName,
                        IsLock = s.IsLock.ToDescription(query.Language),
                    }).ToList();
                table.total = Total;
                table.data = data;
                return table;
            });
        }
        public List<MenuViewModel> GetRoleMenu(int roleid, int userid,string language="cn")
        {
            var table=new List<MenuViewModel>();
            if (AspNetCache.Get<List<MenuViewModel>>(string.Format(CacheKeyCollection.RoleMenu, roleid)) != null)
                table = AspNetCache.Get<List<MenuViewModel>>(string.Format(CacheKeyCollection.RoleMenu, roleid));
            else
            {
                table= QueryDb((context) =>
                {
                    if (userid == 1)
                    {
                        table =
                            context.Menus.Where(s => s.IsLock == 0 && s.MenuType == MenuEnum.Menu)
                                .ToList()
                                .Select(s => new MenuViewModel
                                {
                                    Id = s.Id,
                                    _parentId = s.ParentId,
                                    Name = language.Equals("cn") ? s.Name : s.NameEn,
                                    Url = s.Url,
                                    Icon = s.Icon
                                }).ToList();
                    }
                    else
                    {
                        table =
                            context.RoleMenu.Where(s => s.RoleId == roleid).ToArray().Select(item => new MenuViewModel()
                            {
                                _parentId = item.Menus.ParentId,
                                Icon = item.Menus.Icon,
                                Id = item.MenuId,
                                Url = item.Menus.Url,
                                Name = language.Equals("cn") ? item.Menus.Name : item.Menus.NameEn
                            }).ToList();
                        AspNetCache.Insert(string.Format(CacheKeyCollection.RoleMenu, roleid), table,
                            DateTime.Now.AddHours(2));
                    }
                    return table;
                }, table);
            }
            return table;
        }
        public EasyDataGrid<RoleViewModel> RoleGrid(Model.QueryModel.RoleQueryModel query)
        {
            return QueryDb((context) =>
            {
                var table = new EasyDataGrid<RoleViewModel>();

                #region 条件拼接
                var predicate = PredicateBuilderUtility.True<Role>();
                predicate = predicate.And(s => s.IsLock != LockEnum.Delete);
                if (!string.IsNullOrEmpty(query.RoleName))
                    predicate = predicate.And(s => s.RoleName.Equals(query.RoleName));
                #endregion
                var menu = GetRoleOperateCell(query.RoleId).Where(s => s.MenuId == query.MenuId).Select(s => s.OperateName);
                var data = context.Role.FindBy(predicate, query.Page.Value, query.PageSize.Value, out Total,
                    query.SortField,
                    query.IsDesc).ToList().Select(s => new RoleViewModel
                    {
                        Id = s.Id,
                        RoleName = query.Language.Equals("cn") ? s.RoleName : s.RoleNameEn,
                        IsLock = s.IsLock.ToDescription(query.Language),
                        Cell = GetRoleCell(s, query.RoleId, query.MenuId, menu)
                    }).ToList();
                table.total = Total;
                table.rows = data;
                return table;
            });
        }
        public List<RoleCellViewModel> GetRoleOperateCell(int roleid)
        {
            var cell = new List<RoleCellViewModel>();
            if (AspNetCache.Get<List<RoleCellViewModel>>(string.Format(CacheKeyCollection.RoleCell, roleid)) != null)
                cell =
                    AspNetCache.Get<List<RoleCellViewModel>>(string.Format(CacheKeyCollection.RoleCell, roleid));
            else
            {
                cell= QueryDb((context) =>
                {

                    cell =
                        context.RoleMenu.FindBy(
                            s => s.RoleId == roleid && s.Menus.IsLock == 0 && s.Menus.MenuType == MenuEnum.Cell)
                            .ToArray()
                            .Select(s => new RoleCellViewModel
                            {
                                CellName = s.Menus.Name,
                                OperateName = s.Menus.ButtonLink,
                                MenuId = s.Menus.ParentId
                            }).ToList();
                    AspNetCache.Insert(string.Format(CacheKeyCollection.RoleCell, roleid), cell,
                        DateTime.Now.AddHours(2));
                    return cell;
                }, cell);
            }
            return cell;
        }
        private string GetRoleCell(Role info, int roleId, int menuId, IEnumerable<string> cell)
        {
            var sb = new StringBuilder();
            if (cell.Any())
            {
                var lockNum = info.IsLock == LockEnum.Nnormal ? 1 : 0;
                var lockText = lockNum == 1 ? "冻结" : "解冻";
                if (cell.Contains("cel_RoleLocked"))
                    sb.AppendFormat("<a href='javascript:void(0)' class=\"good-check\" onclick=\"role.lockRole('" + info.Id + "','" + lockNum + "')\">" + lockText + "</a> ");
                if (cell.Contains("cel_PowerRole") && info.IsLock == LockEnum.Nnormal)
                    sb.AppendFormat(" <a href='javascript:void(0)' onclick='role.getRolePower(" + info.Id + ")' class=\"good-check\">权限设置</a> ");
            }
            return sb.ToString();
        }
        public void SetRoleMenu(IEnumerable<RoleMenu> roleMenu, int roleId)
        {
            QueryDb((context) =>
            {
                var menu = context.RoleMenu.Where(s => s.RoleId == roleId);
                if (menu.Any())
                    context.RoleMenu.RemoveRange(menu);
                context.RoleMenu.AddRange(roleMenu);
                if (roleId > 0)
                {
                    AspNetCache.Remove(string.Format(CacheKeyCollection.RoleMenu, roleId));
                    AspNetCache.Remove(string.Format(CacheKeyCollection.RoleCell, roleId));
                    AspNetCache.Remove(string.Format(CacheKeyCollection.RoleButton, roleId));
                }
                context.SaveChanges();
            });
        }
        public List<MenuButtonViewModel> GetRoleButton(long roleId, long menuId,string language="cn")
        {
            var buttons = new List<MenuButtonViewModel>();
            if (AspNetCache.Get<List<MenuButtonViewModel>>(string.Format(CacheKeyCollection.RoleButton, roleId)) != null)
            {
                var allbutton =
                    AspNetCache.Get<List<MenuButtonViewModel>>(string.Format(CacheKeyCollection.RoleButton, roleId));
                buttons = allbutton.Where(s => s.MenuId == menuId).ToList();
            }
            else
            {
                return QueryDb((context) =>
                {
                    var allbutton =
                        context.RoleMenu.FindBy(
                            s =>
                                s.RoleId == roleId && s.Menus.IsLock == 0 &&
                                s.Menus.MenuType == MenuEnum.Button).ToArray().Select(s => new MenuButtonViewModel()
                                {
                                    ButtonId = s.MenuId,
                                    ButtonLink = s.Menus.ButtonLink,
                                    ButtonIcon = s.Menus.Icon,
                                    ButtonName = language.Equals("cn")?s.Menus.Name:s.Menus.NameEn,
                                    MenuId = s.Menus.ParentId
                                }).ToList();
                    if (allbutton.Any())
                    {
                        buttons =
                            allbutton.Where(s => s.MenuId == menuId)
                                .Select(s => new MenuButtonViewModel()
                                {
                                    ButtonId = s.MenuId,
                                    ButtonLink = s.ButtonLink,
                                    ButtonIcon = s.ButtonIcon,
                                    ButtonName = s.ButtonName,
                                    MenuId = s.MenuId
                                }).ToList();
                    }
                    AspNetCache.Insert(string.Format(CacheKeyCollection.RoleButton, roleId), allbutton,
                        DateTime.Now.AddHours(2));
                    return buttons;
                }, buttons);
            }
            return buttons;
        }
        public List<int> RoleMenuList(long roleId)
        {
            return QueryDb((context) =>
            {
                return
                    context.Role.FirstOrDefault(s => s.Id == roleId)
                        .RoleMenu.ToArray()
                        .Select(item => item.MenuId).ToList();
            });
        }
        public bool FrozenOrDeleteRole(long roleid, LockEnum state)
        {
            return QueryDb((context) =>
            {
                var roleused = context.Admin.Any(s => s.RoleId == roleid && s.Status == LockEnum.Nnormal);
                if (roleused && state != LockEnum.Nnormal) return false;
                else
                    return context.Role.Update(w => w.Id == roleid, u => new Role
                    {
                        IsLock = state
                    }) > 0;
            }, false);
        }
    }
}
