using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT.Model.QueryModel
{
    public class UserBetQueryModel : BaseQueryModel
    {
        public UserBetQueryModel()
        {
            this.Source = 0;
        }
        public long? UserId { get; set; }
        public string UserName { get; set; }
        public long? BetId { get; set; }
        public int? IsSet { get; set; }
        public long? MatchId { get; set; }
        public int? Source { get; set; }
    }
}
