using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FT.Entities;
using FT.IRepository;
using FT.Model;
using FT.Model.ViewModel;
using FT.Plugin.Cache;
using FT.Utility.CacheHelper;
using FT.Utility.Helper;

namespace FT.Repository
{
    public class DictionaryRepository : BaseRepository<SystemDictionary>, IDictionaryRepository
    {
        public EasyDataGrid<dynamic> DictionaryGrid(int parentid, string language = "cn")
        {
            var tree = new EasyDataGrid<dynamic>();
            return QueryDb((context) =>
            {
                var menu = context.SystemDictionary.FindBy(s => s.IsLock !=LockEnum.Delete && s.ParentId == parentid).ToArray();
                if (menu.Any())
                {
                    tree.rows = menu.Select(item => new
                    {
                        _parentId = item.ParentId,
                        Id = item.Id,
                        IsLock = (int)item.IsLock,
                        Value = item.DictionaryValue,
                        Name =
                            language == "cn"
                                ? item.DictionaryName
                                : (language == "en" ? item.DictionaryNameEn : item.DictionaryNamePt),
                        DictionaryName = item.DictionaryName,
                        DictionaryNameEn = item.DictionaryNameEn,
                        DictionaryNamePt = item.DictionaryNamePt,
                        DictionaryValue=item.DictionaryValue,
                        Sort = item.Sort,
                        state = "closed",
                        Lock = item.IsLock.ToDescription(language)
                    });
                    tree.total = menu.Length;
                }
                else
                    tree.rows = new List<MenuViewModel>();
                return tree;
            }, tree);
        }
        public List<ComBoxModel> DictionaryList(System.Linq.Expressions.Expression<Func<SystemDictionary, bool>> pride, string language = "cn")
        {
            var tree = new List<ComBoxModel>();
            return QueryDb((context) =>
            {
                return context.SystemDictionary.Where(pride).Select(s => new ComBoxModel
                {
                    Id = s.Id + "",
                    Name =
                        language == "cn"
                            ? s.DictionaryName
                            : (language == "en" ? s.DictionaryNameEn : s.DictionaryNamePt)

                }).ToList();
            }, tree);
        }
        public TimeZoneViewModel DictionaryXY(System.Linq.Expressions.Expression<Func<SystemDictionary, bool>> pride)
        {
            var tree = new TimeZoneViewModel();
            var cache = Cache.Get<TimeZoneViewModel>(CacheKeyCollection.SystemTimeZone);
            if (cache != null)
                tree = cache;
            return QueryDb((context) =>
            {
                var dict = context.SystemDictionary.Where(pride).OrderBy(s => s.Id).ToArray();
                if (dict.Any())
                {
                    tree.PointValue = dict.Select(s => s.DictionaryValue).ToList();
                    tree.en = dict.Select(s => s.DictionaryNameEn).ToList();
                    tree.pt = dict.Select(s => s.DictionaryNamePt).ToList();
                    tree.cn = dict.Select(s => s.DictionaryName).ToList();
                    Cache.Add(CacheKeyCollection.SystemTimeZone, tree, 120);
                }
                return tree;
            }, tree);
        }
    }
}
