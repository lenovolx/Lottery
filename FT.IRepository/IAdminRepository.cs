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
    public interface IAdminRepository:IBaseRepository<Admin>
    {
        ManagerResponse ManagerLogin(string username, string password, bool remember = false);
        AngularTable<AdminViewModel> GetAdmin(AdminQueryModel query);
        EasyDataGrid<AdminViewModel> GetAdminGrid(AdminQueryModel query);
        AdminViewModel SingleAdmin(Expression<Func<Admin, bool>> pried, string language = "cn");
        EditEnum ModifyPass(int userid, string password, string newpassword, bool useoldpass = true);
        bool FrozenOrDeleteAdmin(int adminid, LockEnum state);
        bool ModifyAdmin(Admin admin);
    }
}
