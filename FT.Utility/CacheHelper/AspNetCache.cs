using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;

namespace FT.Utility.CacheHelper
{
    public class AspNetCache
    {
        public static void Clear()
        {
            var enumerator = HttpRuntime.Cache.GetEnumerator();
            while (enumerator.MoveNext())
            {
                HttpRuntime.Cache.Remove(enumerator.Key.ToString());
            }
        }

        public static object Get(string key)
        {
            return HttpRuntime.Cache.Get(key);
        }

        public static T Get<T>(string key) where T : class
        {
            return (T) HttpRuntime.Cache.Get(key);
        }

        public static void Insert(string key, object value)
        {
            if (HttpRuntime.Cache.Get(key) != null)
                HttpRuntime.Cache.Remove(key);
            HttpRuntime.Cache.Insert(key, value);
        }

        public static void Insert(string key, object data, int expirtime)
        {
            HttpRuntime.Cache.Insert(key, data, null, DateTime.Now.AddSeconds((double) expirtime),
                Cache.NoSlidingExpiration, CacheItemPriority.High, null);
        }

        public static void Insert(string key, object data, DateTime expirtime)
        {
            HttpRuntime.Cache.Insert(key, data, null, expirtime, Cache.NoSlidingExpiration, CacheItemPriority.High, null);
        }

        public static bool Add(string key, object data, int expirtime)
        {
            var flag = true;
            HttpRuntime.Cache.Insert(key, data, null, DateTime.Now.AddSeconds((double) expirtime),
                Cache.NoSlidingExpiration, CacheItemPriority.High, null);
            if (HttpRuntime.Cache.Get(key) == null)
                flag = false;
            return flag;
        }

        public static void Remove(string key)
        {
            HttpRuntime.Cache.Remove(key);
        }

        public static void ClearContains(string key)
        {
            var enumerator = HttpRuntime.Cache.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Key.ToString().Contains(key))
                {
                    HttpRuntime.Cache.Remove(enumerator.Key.ToString());
                }
            }
        }

        public static void TimeOutCache(string keyFix)
        {
            var enumerator = HttpRuntime.Cache.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Key.ToString().Contains(keyFix))
                {
                    var key = enumerator.Key.ToString();
                    var user =
                        (dynamic) Newtonsoft.Json.JsonConvert.DeserializeObject(HttpRuntime.Cache.Get(key).ToString());
                    user.BeUsed = 1;
                    HttpRuntime.Cache.Remove(key);
                    HttpRuntime.Cache.Insert(key, Newtonsoft.Json.JsonConvert.SerializeObject(user), null,
                        DateTime.Now.AddMinutes(3), Cache.NoSlidingExpiration, CacheItemPriority.High, null);
                }
            }
        }
    }
}
