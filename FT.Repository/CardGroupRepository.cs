using FT.IRepository;
using FT.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FT.Entities;
using FT.Utility.Helper;

namespace FT.Repository
{
    public class CardGroupRepository : BaseRepository<CardGroup>, ICardGroupRepository
    {
        public bool AddCardGroup(CardGroup cg)
        {
            return QueryDb((context) =>
            {
                cg.CreateDate = DateTime.Now;
                context.CardGroup.Add(cg);
                return context.SaveChanges() > 0;
            });
        }
        public EasyDataGrid<dynamic> CardList(Model.QueryModel.CardQueryModel query)
        {
            var grid = new EasyDataGrid<dynamic>();
            return QueryDb((context) =>
            {
                var @where = PredicateBuilderUtility.True<CardGroup>();
                grid.rows =
                    context.CardGroup.FindBy(where, query.Page.Value, query.PageSize.Value, out Total, query.SortField,
                        query.IsDesc).ToArray().Select(s => new
                        {
                            GroupId = s.GroupId,
                            GroupName = s.GroupName,
                            Amount = s.Amount,
                            Number = s.Number,
                            CreateDate = s.CreateDate
                        });
                grid.total = Total;
                return grid;
            }, grid);
        }
    }
}