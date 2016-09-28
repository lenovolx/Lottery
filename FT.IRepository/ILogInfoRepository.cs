using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FT.Model;
using FT.Model.QueryModel;

namespace FT.IRepository
{
    public interface ILogInfoRepository:IBaseRepository<LogInfo>
    {
        EasyDataGrid<dynamic> LogGrid(LogInfoQueryModel query);
    }
}
