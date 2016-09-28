using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT.Model.QueryModel
{
    /// <summary>
    /// 订单查询条件
    /// </summary>
    public class OrderQueryModel : BaseQueryModel
    {
        public long? OrderId { get; set; }
    }
}
