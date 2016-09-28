using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FT.Model;
using FT.Model.QueryModel;

namespace FT.IRepository
{
    public interface ISystemTaskRepository:IBaseRepository<SystemTask>
    {
        EasyDataGrid<dynamic> TaskGrid(BaseQueryModel query);
    }
}
