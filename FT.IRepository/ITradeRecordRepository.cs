using FT.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FT.Model.ViewModel;

namespace FT.IRepository
{
    public interface ITradeRecordRepository : IBaseRepository<TradeRecord>
    {
        EasyDataGrid<TradeRecordViewModel> GetTradeRecordGrid(Model.QueryModel.TradeRecordQueryModel query);
    }
}