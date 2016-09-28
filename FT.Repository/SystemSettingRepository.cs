using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FT.Entities;
using FT.IRepository;
using FT.Model;
using FT.Model.ViewModel;
using FT.Plugin.Cache;
using FT.Utility.CacheHelper;

namespace FT.Repository
{
    public class SystemSettingRepository : BaseRepository<SystemSetting>, ISystemSettingRepository
    {
        public SystemSetting GetSetting()
        {
            var setting = new SystemSetting();
            var cache = Cache.Get<SystemSetting>(CacheKeyCollection.SystemSetting);
            if (cache != null)
                setting = cache;
            else
            {
                var setting1 = setting;
                setting = QueryDb((context) =>
                {
                    IEnumerable<SystemSetting> sets = context.SystemSetting.FindAll().ToArray();
                    var properties = setting1.GetType().GetProperties();
                    foreach (var property in properties)
                    {
                        if (property.Name == "Id" || property.Name == "Key" || property.Name == "Value") continue;
                        var temp = sets.FirstOrDefault(item => item.Key == property.Name);
                        if (temp != null)
                            property.SetValue(setting1, Convert.ChangeType(temp.Value, property.PropertyType));
                    }
                    Cache.Insert(CacheKeyCollection.SystemSetting, setting1, 120);
                    return setting1;
                }, setting);
            }
            return setting;
        }

        public bool EditSetting(SystemSetting systemSetting)
        {
            var properties = systemSetting.GetType().GetProperties();
            SystemSetting temp;
            string value;
            object obj;
            return QueryDb((context) =>
            {
                IEnumerable<SystemSetting> siteSettingInfos = context.SystemSetting.FindAll().ToArray();
                foreach (var property in properties)
                {
                    obj = property.GetValue(systemSetting);
                    value = obj != null ? obj.ToString() : "";
                    if (property.Name == "Id" || property.Name == "Key" || property.Name == "Value") continue;
                    temp = siteSettingInfos.FirstOrDefault(item => item.Key == property.Name);
                    if (temp == null)
                        context.SystemSetting.Add(new SystemSetting() { Key = property.Name, Value = value });
                    else
                        temp.Value = value;
                }
                var propertyNames = properties.Select(item => item.Name);
                context.SystemSetting.RemoveRange(siteSettingInfos.Where(item => !propertyNames.Contains(item.Key)));
                context.SaveChanges();
                Cache.Remove(CacheKeyCollection.SystemSetting);
                return true;
            }, false);
        }
    }
}
