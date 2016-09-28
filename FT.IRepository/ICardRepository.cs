using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FT.Model;
using FT.Model.QueryModel;

namespace FT.IRepository
{
    public interface ICardRepository : IBaseRepository<Card>
    {
        dynamic GetCardList(CardQueryModel query);
        dynamic SingleCard(CardQueryModel query);
        bool AddCard(int amount, int number, long groupId);
        EasyDataGrid<dynamic> CardList(CardQueryModel query);
    }
}
