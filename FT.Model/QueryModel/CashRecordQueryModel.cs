using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT.Model.QueryModel
{
    public class CashRecordQueryModel:BaseQueryModel
    {
        public string RealName { get; set; }
        public int? Status { get; set; }
    }
}
