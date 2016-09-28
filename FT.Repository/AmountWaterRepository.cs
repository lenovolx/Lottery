using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FT.Entities;
using FT.IRepository;
using FT.Model;
using FT.Model.QueryModel;
using FT.Model.ViewModel;
using FT.Utility.Helper;

namespace FT.Repository
{
    public class AmountWaterRepository : BaseRepository<AmountWater>, IAmountWaterRepository
    {
        public EasyDataGrid<AmountWaterViewModel> AmountWaterGrid(AmountWaterQueryModel query)
        {
            var grid = new EasyDataGrid<AmountWaterViewModel>();
            return QueryDb((context) =>
            {
                var predicate = PredicateBuilderUtility.True<AmountWater>();
                if (query.StartDate.HasValue && query.EndDate.HasValue)
                {
                    var enddate = query.EndDate.Value.AddDays(1);
                    predicate =
                        predicate.And(
                            s => s.CreateDate > query.StartDate.Value && s.CreateDate < enddate);
                }
                if (!string.IsNullOrEmpty(query.UserName))
                    predicate = predicate.And(s => s.UserName.Contains(query.UserName));
                if (query.Type.HasValue && query.Type.Value!=-99)
                    predicate = predicate.And(s => s.Type == (TradEnum) query.Type.Value);
                grid.rows =
                    context.AmountWater.FindBy(predicate, query.Page.Value, query.PageSize.Value, out Total,
                        query.SortField, query.IsDesc).ToArray().Select(item => new AmountWaterViewModel
                        {
                            Id = item.Id,
                            UserName = item.UserName,
                            UserId = item.UserId,
                            Amount = item.Amount,
                            CreateDate = item.CreateDate,
                            Type = item.Type.ToDescription(query.Language),
                            Remark = item.Remark,
                            TypeCode = (int) item.Type
                        });
                grid.total = Total;
                return grid;
            }, grid);
        }
    }
}
