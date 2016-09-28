using FT.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FT.Model.QueryModel;

namespace FT.IRepository
{
    public interface ICardGroupRepository : IBaseRepository<CardGroup>
    {
        EasyDataGrid<dynamic> CardList(CardQueryModel query);
        bool AddCardGroup(CardGroup cg);
    }
}
