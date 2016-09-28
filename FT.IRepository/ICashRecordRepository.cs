using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FT.Model;
using FT.Model.ViewModel;
using FT.Model.QueryModel;

namespace FT.IRepository
{
    public interface ICashRecordRepository:IBaseRepository<CashRecord>
    {
        TransAccountsEnum UserCashInfo(CashRecord cash, string code);
        EasyDataGrid<CashRecordViewModel> CashRecordGrid(CashRecordQueryModel query);
        bool AuditCashRecord(CashRecord model);
    }
}
