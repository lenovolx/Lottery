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
    public class MessageRepository : BaseRepository<Message>, IMessageRepository
    {
        public EasyDataGrid<dynamic> MessageGrid(Model.QueryModel.MessageQueryModel query)
        {
            var grid = new EasyDataGrid<dynamic>();
            return QueryDb((context) =>
            {
                grid.rows =
                    context.Message.FindBy(s => true, query.Page.Value, query.PageSize.Value, out Total, query.SortField,
                        query.IsDesc).ToArray().Select(s => new
                        {
                            Id = s.Id,
                            Title = s.Title,
                            Annexes = s.Annexes,
                            CreateDate = s.CreateDate,
                            CreateUser = s.CreateUser,
                            LastUpdateDate = s.LastUpdateDate,
                            IsDrafts = s.IsDrafts.ToDescription(query.Language),
                            Type = s.Type,
                            IsDraftsNum = (int) s.IsDrafts,
                            CreateUserName = s.CreateUserName,
                            Descriptions = s.Descriptions
                        });
                grid.total = Total;
                return grid;
            }, grid);
        }
    }
}
