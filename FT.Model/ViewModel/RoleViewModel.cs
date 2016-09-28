using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT.Model.ViewModel
{
    public class RoleViewModel : BaseViewModel
    {
        public int Id { get; set; }
        public string RoleName { get; set; }
        public string IsLock { get; set; }
    }
    public class RoleCellViewModel
    {
        public string CellName { get; set; }
        public string OperateName { get; set; }
        public long MenuId { get; set; }
    }

    public class RoleMenuViewModel
    {
        public List<int> MenuId { get; set; }
    }
}
