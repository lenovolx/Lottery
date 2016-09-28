using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FT.Entities;
using FT.IRepository;
using FT.Model;
using FT.Model.QueryModel;
using FT.Utility.Helper;

namespace FT.Repository
{
    public class CardRepository : BaseRepository<Card>, ICardRepository
    {
        public dynamic GetCardList(Model.QueryModel.CardQueryModel query)
        {
            throw new NotImplementedException();
        }
        public dynamic SingleCard(Model.QueryModel.CardQueryModel query)
        {
            return QueryDb((context) =>
            {
                var predicate = PredicateBuilderUtility.True<Card>();
                if (!string.IsNullOrEmpty(query.CardNum))
                    predicate = predicate.And(s => s.CardNum.Equals(query.CardNum));
                var card = context.Card.FirstOrDefault(predicate);
                return new
                {
                    Id = card.Id,
                    CardNum = card.CardNum,
                    GroupID = card.GroupID,
                    UseDate = card.UseDate,
                    IsUsed = (int)card.IsUsed,
                    Status = card.Status,
                    Amount = card.CardGroup.Amount
                };
            });
        }
 

        private static readonly object Obj = new object();
        private long BuildCardNum()
        {
            lock (Obj)
            {
                var orderId = string.Empty;
                string dat = DateTime.Now.ToString("hmmssfff");
                var random = new Random(BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0));
                int randomlength = 16 - dat.Length;//确保是16位

                for (var i = 0; i < randomlength; i++)
                {
                    var rand = random.Next();
                    var code = (char)('0' + (char)(rand % 10));
                    orderId += code.ToString();
                }
                return long.Parse(dat + orderId);
            }
        }

        public bool AddCard(int amount, int number, long groupId)
        {
            List<Card> listcard = new List<Card>();
            for (int i = 0; i < number; i++)
            {
                string pwd = "";
                var random = new Random(BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0));
                for (var j = 0; j < 6; j++)
                {
                    var rand = random.Next();
                    var code = (char)('0' + (char)(rand % 10));
                    pwd += code.ToString();
                }
                var card = new Card
                {
                    CardNum = BuildCardNum().ToString(),
                    CardPwd = pwd,
                    GroupID = groupId,
                    IsUsed = 0,
                    Status = 0
                };
                listcard.Add(card);
            }
            return QueryDb((context) =>
            {
                context.Card.AddRange(listcard);
                return context.SaveChanges() > 0;
            });
        }
        public EasyDataGrid<dynamic> CardList(CardQueryModel query)
        {
            var grid = new EasyDataGrid<dynamic>();
            return QueryDb((context) =>
            {
                var @where = PredicateBuilderUtility.True<Card>();
                @where = @where.And(s => s.GroupID == query.GroupId.Value);
                grid.rows =
                    context.Card.FindBy(where, query.Page.Value, query.PageSize.Value, out Total, query.SortField,
                        query.IsDesc).ToArray().Select(s => new
                        {
                            CardNum = s.CardNum,
                            CardPwd = s.CardPwd,
                            GroupID = s.GroupID,
                            UseDate = s.UseDate,
                            IsUsed = s.IsUsed.ToDescription(query.Language),
                            Status = s.Status,
                            Id = s.Id
                        });
                grid.total = Total;
                return grid;
            }, grid);
        }
    }
}
