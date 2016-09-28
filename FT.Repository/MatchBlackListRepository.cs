using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityFramework.Extensions;
using FT.Entities;
using FT.IRepository;
using FT.Model;
using FT.Utility.Helper;

namespace FT.Repository
{
    public class MatchBlackListRepository : BaseRepository<MatchBlackList>, IMatchBlackListRepository
    {
        public EasyDataGrid<dynamic> BlackLeagueGrid(Model.QueryModel.BlackLeagueQueryModel query)
        {
            var grid = new EasyDataGrid<dynamic>();
            return QueryDb((context) =>
            {
                var predicate = PredicateBuilderUtility.True<MatchBlackList>();
                if (!string.IsNullOrEmpty(query.LeagueName))
                    predicate = query.Language.Equals("cn")
                        ? predicate.And(s => s.LeagueName.Contains(query.LeagueName))
                        : predicate.And(s => s.LeagueNameEn.Contains(query.LeagueName));
                if (query.IsBan.HasValue)
                    predicate = predicate.And(s => s.IsBan == (YesNoEnum) query.IsBan);
                grid.rows =
                    context.MatchBlackList.FindBy(predicate, query.Page.Value, query.PageSize.Value, out Total,
                        query.SortField, query.IsDesc).ToArray().Select(s => new
                        {
                            Id = s.Id,
                            LeagueName = query.Language.Equals("cn") ? s.LeagueName : s.LeagueNameEn,
                            IsBanNum = (int) s.IsBan,
                            IsBan = s.IsBan.ToDescription(query.Language),
                            LastOperateDate = s.LastOperateDate,
                            OperateUserId = s.Admin.LoginName,
                            DataSource = s.DataSource.ToDescription(query.Language),
                            DataSourceNum = (int) s.DataSource
                        }).ToList();
                grid.total = Total;
                return grid;
            }, grid);
        }
        public bool SetLeagueBan(YesNoEnum iaban, IEnumerable<long> ids,int operateUser)
        {
            return QueryDb((context) =>
            {
                return context.MatchBlackList.Where(s => ids.Contains(s.Id)).Update(w => new MatchBlackList
                {
                    LastOperateDate = DateTime.Now,
                    IsBan = iaban,
                    OperateUserId = operateUser
                }) > 0;
            }, false);
        }
    }
}
