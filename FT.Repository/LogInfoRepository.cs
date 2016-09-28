using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FT.Entities;
using FT.IRepository;
using FT.Model;
using FT.Utility.Helper;

namespace FT.Repository
{
    public class LogInfoRepository : BaseRepository<LogInfo>, ILogInfoRepository
    {
        public EasyDataGrid<dynamic> LogGrid(Model.QueryModel.LogInfoQueryModel query)
        {
            var grid = new EasyDataGrid<dynamic>();
            return QueryDb((context) =>
            {
                var @where = PredicateBuilderUtility.True<LogInfo>();
                if (!string.IsNullOrEmpty(query.LogUser))
                    where = where.And(s => s.LogUser.Equals(query.LogUser));
                if (query.StartDate.HasValue && query.EndDate.HasValue)
                {
                    var endDate = query.EndDate.Value.AddDays(1);
                    where = where.And(s => s.LogTime >= query.StartDate && s.LogTime < endDate);
                }
                grid.rows =
                    context.LogInfo.FindBy(where, query.Page.Value, query.PageSize.Value, out Total, query.SortField,
                        query.IsDesc).ToArray().Select(s => new
                        {
                            LogTime = s.LogTime,
                            LogUser = s.LogUser,
                            LogUserIp = s.LogUserIp,
                            LogController = s.LogController,
                            LogAction = s.LogAction,
                            LogOperate = s.LogOperate,
                            Id = s.Id,LogType=s.LogType
                        });
                grid.total = Total;
                return grid;
            }, grid);
        }
    }
}
