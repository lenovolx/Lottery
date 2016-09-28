using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT.Model.ViewModel
{
    /// <summary>
    /// 
    /// </summary>
    public class AmountWaterViewModel : BaseLanguageViewModel
    {
        public long? Id { get; set; }
        public string Type { get; set; }
        public int? TypeCode { get; set; }
        public long? UserId { get; set; }
        public string UserName { get; set; }
        public decimal? Amount { get; set; }
        public string Remark { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
