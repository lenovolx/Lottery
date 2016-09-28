using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityFramework.Extensions;
using FT.Entities;
using FT.IRepository;
using FT.Model;
using FT.Model.ViewModel;
using FT.Utility.CacheHelper;
using FT.Utility.Helper;

namespace FT.Repository
{
    public class MenuRepository : BaseRepository<Menus>, IMenuRepository
    {
        public EasyDataGrid<MenuViewModel> MenuList(int parentid,string language="cn")
        {
            var tree = new EasyDataGrid<MenuViewModel>();
            return QueryDb((context) =>
            {
                var menu = context.Menus.FindBy(s => s.IsLock == 0 && s.ParentId == parentid).ToArray();
                if (menu.Any())
                {
                    tree.rows = menu.Select(item => new MenuViewModel()
                    {
                        _parentId = item.ParentId,
                        Id = item.Id,
                        IsLock = (int)item.IsLock,
                        Name = item.Name,
                        NameEn=item.NameEn,
                        SortNum = item.SortNum,
                        Url = item.ParentId == 0 ? "" : item.Url,
                        ButtonLink = item.ButtonLink,
                        MenuType = item.MenuType.ToDescription(language),
                        Icon = item.Icon
                    });
                    tree.total = menu.Length;
                }
                else
                    tree.rows = new List<MenuViewModel>();
                return tree;
            }, tree);
        }
        public bool CreateMenu(Menus model)
        {
            return QueryDb((context) =>
            {
                context.Menus.Add(model);
                var flag = context.SaveChanges() > 0;
                if (flag)
                    AspNetCache.Remove(CacheKeyCollection.MneuCache);
                return flag;
            },false);
        }
        public bool UpdateMenu(Menus model)
        {
            return QueryDb((context) =>
            {
                var flag = context.Menus.Where(s => s.Id == model.Id).Update(w => new Menus
                {
                    Name = model.Name,
                    SortNum = model.SortNum,
                    ButtonLink = model.ButtonLink,
                    Url = model.Url,
                    MenuType = model.MenuType,
                    Icon = model.Icon,
                    NameEn = model.NameEn
                }) > 0;
                if (flag)
                    AspNetCache.Remove(CacheKeyCollection.MneuCache);
                return flag;
            }, false);
        }
        public List<MenuViewModel> CacheMenu(System.Linq.Expressions.Expression<Func<Menus, bool>> exp,string language="cn")
        {
            var meunList = new List<MenuViewModel>();
            if (AspNetCache.Get<List<MenuViewModel>>(CacheKeyCollection.MneuCache) != null)
                meunList = AspNetCache.Get<List<MenuViewModel>>(CacheKeyCollection.MneuCache);
            else
            {
                meunList= QueryDb((context) =>
                {
                    var menu = context.Menus.FindBy(exp).ToArray();
                    if (menu.Any())
                    {
                        meunList = menu.Select(item => new MenuViewModel()
                        {
                            _parentId = item.ParentId,
                            Id = item.Id,
                            IsLock = (int) item.IsLock,
                            Name = language.Equals("cn") ? item.Name : item.NameEn,
                            SortNum = item.SortNum,
                            Url = item.ParentId == 0 ? "" : item.Url,
                            MenuType = item.MenuType.ToDescription(language),
                            Icon = item.Icon
                        }).ToList();
                    }
                    AspNetCache.Insert(CacheKeyCollection.MneuCache, meunList, DateTime.Now.AddHours(2));
                    return meunList;
                }, meunList);
            }
            return meunList;
        }
    }
}
