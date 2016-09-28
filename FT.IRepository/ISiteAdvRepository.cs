using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using FT.Model;
using FT.Model.QueryModel;

namespace FT.IRepository
{
    public interface ISiteAdvRepository:IBaseRepository<SiteAdv>
    {
        EasyDataGrid<dynamic> AdvGrid(BaseQueryModel query);
        IEnumerable<dynamic> GetAdvList(Expression<Func<SiteAdv, bool>> pride);
    }
}
