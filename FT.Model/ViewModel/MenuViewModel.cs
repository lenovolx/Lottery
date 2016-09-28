using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT.Model.ViewModel
{
    public class MenuViewModel : BaseViewModel
    {
        public MenuViewModel()
        {
            this.state = "closed";
            this.Icon = "icon-sys-log";
        }
        public int Id { get; set; }
        public int _parentId { get; set; }
        public string Name { get; set; }
        public string NameEn { get; set; }
        public string Url { get; set; }
        public int IsLock { get; set; }
        public int SortNum { get; set; }
        public string state { get; set; }
        public string MenuType { get; set; }
        public string ButtonLink { get; set; }
        public string Icon { get; set; }
    }
    public class MenuButtonViewModel
    {
        public long ButtonId { get; set; }
        public long MenuId { get; set; }
        public string ButtonName { get; set; }
        public string ButtonLink { get; set; }
        public string ButtonIcon { get; set; }
    }

}
