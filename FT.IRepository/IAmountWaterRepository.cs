using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FT.Model;
using FT.Model.QueryModel;
using FT.Model.ViewModel;

namespace FT.IRepository
{
    public interface IAmountWaterRepository:IBaseRepository<AmountWater>
    {
        EasyDataGrid<AmountWaterViewModel> AmountWaterGrid(AmountWaterQueryModel query);
    }
}
