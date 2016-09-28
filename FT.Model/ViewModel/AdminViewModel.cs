using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT.Model.ViewModel
{
    public class AdminViewModel : BaseViewModel
    {
        public AdminViewModel()
        {
            this.CreditLimit = 0;
        }
        public int? Id { get; set; }
        public string LoginName { get; set; }
        public string Status { get; set; }
        public DateTime? CreateDate { get; set; }
        public string RoleName { get; set; }
        public int RoleId { get; set; }
        public long? AgentId { get; set; }
        public int AgentLevel { get; set; }
        public string ParentPath { get; set; }
        public decimal? CreditLimit { get; set; }
    }
}