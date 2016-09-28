using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FT.Model;
using FT.Model.ViewModel;

namespace FT.IRepository
{
    public interface IMatchUserBetContentRepository : IBaseRepository<MatchUserBetContent>
    {
        List<UserBetDetail> BetDetail(long betid, string language="cn");
    }
}
