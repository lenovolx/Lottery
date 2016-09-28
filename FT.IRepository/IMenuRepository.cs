using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using FT.Model;
using FT.Model.ViewModel;

namespace FT.IRepository
{
    public interface IMenuRepository:IBaseRepository<Menus>
    {
        EasyDataGrid<MenuViewModel> MenuList(int parentid, string language = "cn");
        bool CreateMenu(Menus model);
        bool UpdateMenu(Menus model);
        List<MenuViewModel> CacheMenu(Expression<Func<Menus, bool>> exp, string language = "cn");
    }
}
