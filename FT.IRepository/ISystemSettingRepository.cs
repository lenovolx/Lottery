using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FT.Model;

namespace FT.IRepository
{
    public interface ISystemSettingRepository:IBaseRepository<SystemSetting>
    {
        SystemSetting GetSetting();
        bool EditSetting(SystemSetting systemSetting);
    }
}
