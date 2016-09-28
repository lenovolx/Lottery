using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using FT.Model;
using FT.Model.QueryModel;
using FT.Model.ViewModel;

namespace FT.IRepository
{
    public interface IRoleRepository:IBaseRepository<Role>
    {
        dynamic GetRole(Expression<Func<Role, bool>> predi);
        AngularTable<RoleViewModel> GetRoleList(RoleQueryModel query);
        List<MenuViewModel> GetRoleMenu(int roleid, int userid, string language = "cn");
        EasyDataGrid<RoleViewModel> RoleGrid(RoleQueryModel query);
        List<RoleCellViewModel> GetRoleOperateCell(int roleid);
        void SetRoleMenu(IEnumerable<RoleMenu> roleMenu, int roleId);
        List<MenuButtonViewModel> GetRoleButton(long roleId, long menuId, string language = "cn");
        List<int> RoleMenuList(long roleId);
        bool FrozenOrDeleteRole(long roleid, LockEnum state);
    }
}
