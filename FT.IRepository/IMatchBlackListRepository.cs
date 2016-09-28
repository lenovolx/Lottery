using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FT.Model;
using FT.Model.QueryModel;

namespace FT.IRepository
{
    public interface IMatchBlackListRepository:IBaseRepository<MatchBlackList>
    {
        EasyDataGrid<dynamic> BlackLeagueGrid(BlackLeagueQueryModel query);
        bool SetLeagueBan(YesNoEnum iaban, IEnumerable<long> ids,int operateUser);
    }
}
