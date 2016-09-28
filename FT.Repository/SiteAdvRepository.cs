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
    public class SiteAdvRepository : BaseRepository<SiteAdv>, ISiteAdvRepository
    {
        public EasyDataGrid<dynamic> AdvGrid(Model.QueryModel.BaseQueryModel query)
        {
            var grid = new EasyDataGrid<dynamic>();
            return QueryDb((context) =>
            {
                grid.rows =
                    context.SiteAdv.FindBy(s => true, query.Page.Value, query.PageSize.Value, out Total, query.SortField,
                        query.IsDesc).ToArray().Select(s => new
                        {
                            Id = s.Id,
                            AdvTitle = s.AdvTitle,
                            AdvImg = s.AdvImg,
                            Type = s.Type.ToDescription(query.Language),
                            IsShow = s.IsShow.ToDescription(query.Language),
                            TypeNum = (int) s.Type,
                            IsShowNum = (int) s.IsShow,Sort=s.Sort
                        });
                grid.total = Total;
                return grid;
            }, grid);
        }
        public IEnumerable<dynamic> GetAdvList(System.Linq.Expressions.Expression<Func<SiteAdv, bool>> pride)
        {
            IEnumerable<dynamic> list;
            return QueryDb((context) =>
            {
                list =
                    context.SiteAdv.FindBy(pride, 1, 5, out Total, "Sort", true).ToArray().Select(s => new
                    {
                        Title = s.AdvTitle,
                        Img = s.AdvImg
                    }).ToList();
                return list;
            }, new List<dynamic>());
        }
    }
}
