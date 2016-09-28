using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using FT.Entities;
using FT.Model;
using FT.IRepository;
using FT.Model.QueryModel;
using FT.Model.ViewModel;
using FT.Utility.Helper;

namespace FT.Repository
{
    public class TradeRecordRepository : BaseRepository<TradeRecord>, ITradeRecordRepository
    {
        public EasyDataGrid<TradeRecordViewModel> GetTradeRecordGrid(TradeRecordQueryModel query)
        {
            var grid = new EasyDataGrid<TradeRecordViewModel>();
            return QueryDb((context) =>
            {
                var predicate = PredicateBuilderUtility.True<TradeRecord>();
                if (query.Type.HasValue && query.Type.Value != 0 && query.Type.Value != -99)
                    predicate = predicate.And(p => p.Type == (TradEnum) query.Type.Value);
                if (query.StartDate.HasValue && query.EndDate.HasValue)
                {
                    var endtime = query.EndDate.Value.AddDays(1);
                    predicate =
                        predicate.And(
                            p => p.CreateDate > query.StartDate && p.CreateDate < endtime);
                }
                predicate = predicate.And(p => p.Status==LockEnum.Nnormal);
                grid.rows =
                    context.TradeRecord.FindBy(predicate, query.Page.Value, query.PageSize.Value, out Total,
                        query.SortField, query.IsDesc).ToArray()
                        .Select(s => new TradeRecordViewModel
                        {
                            CardNum = s.CardNum,
                            Id = s.Id,
                            FromUserName = s.FromUserName ?? s.ToUserName,
                            ToUserName = s.ToUserName,
                            TradeAmount = s.TradeAmount.Value,
                            CreateDate = s.CreateDate.Value,
                            CreateUser = s.UserAccount == null ? "" : s.UserAccount.LoginName,
                            Type = s.Type.ToDescription(query.Language)
                        }).ToList();
                grid.total = Total;
                return grid;
            }, grid);
        }
        
    }
}
