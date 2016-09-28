using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using FT.Model;
using FT.Model.ViewModel;

namespace FT.IRepository
{
    public interface IDictionaryRepository : IBaseRepository<SystemDictionary>
    {
        EasyDataGrid<dynamic> DictionaryGrid(int parentid, string language = "cn");
        List<ComBoxModel> DictionaryList(Expression<Func<SystemDictionary, bool>> pride, string language = "cn");
        TimeZoneViewModel DictionaryXY(Expression<Func<SystemDictionary, bool>> pride);
    }
}
