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
    public class SystemTaskRepository : BaseRepository<SystemTask>, ISystemTaskRepository
    {
        public EasyDataGrid<dynamic> TaskGrid(Model.QueryModel.BaseQueryModel query)
        {
            var grid = new EasyDataGrid<dynamic>();
            return QueryDb((context) =>
            {
                var predicate = PredicateBuilderUtility.True<SystemTask>();
                grid.rows = context.SystemTask.FindBy(predicate, query.Page.Value, query.PageSize.Value, out Total,
                    query.SortField,
                    query.IsDesc).ToArray().Select(s => new
                    {
                        TaskID = s.TaskID,
                        TaskName = s.TaskName,
                        CronRemark = s.CronRemark,
                        Status = (int)s.Status.Value,
                        StatusText = s.Status.Value.ToDescription(query.Language),
                        RecentRunTime = s.RecentRunTime,
                        LastRunTime = s.LastRunTime,
                        CreatedOn = s.CreatedOn,
                        Remark = s.Remark,
                        Assembly=s.Assembly,
                        CronExpressionString=s.CronExpressionString,
                        Class=s.Class,
                        Cell = ""
                    });
                grid.total = Total;
                return grid;
            }, grid);
        }
    }
}
