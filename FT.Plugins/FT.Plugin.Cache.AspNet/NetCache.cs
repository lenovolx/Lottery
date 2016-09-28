using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;

namespace FT.Plugin.Cache.AspNet
{
    public class NetCache : ICache
    {
        private int _timeout = 60;
        private readonly System.Web.Caching.Cache _cache = HttpRuntime.Cache;
        private static object _cacheLocker = new object();
        private const int DefaultTmeout = 60;
        public void Clear()
        {
            var enumerator = this._cache.GetEnumerator();
            while (enumerator.MoveNext())
            {
                this._cache.Remove(enumerator.Key.ToString());
            }
        }
        public object Get(string key)
        {
            return this._cache.Get(key);
        }
        public T Get<T>(string key) where T : class
        {
            return (T)this._cache.Get(key);
        }
        public void Insert(string key, object value)
        {
            if (this._cache.Get(key) != null)
                this._cache.Remove(key);
            this._cache.Insert(key, value);
        }
        public void Insert(string key, object data, int expirtime)
        {
            this._cache.Insert(key, data, null, DateTime.Now.AddSeconds((double)expirtime), System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.High, null);
        }
        public void Insert(string key, object data, DateTime expirtime)
        {
            this._cache.Insert(key, data, null, expirtime, System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.High, null);
        }
        public bool Add(string key, object data, int expirtime)
        {
            var flag = true;
            this._cache.Insert(key, data, null, DateTime.Now.AddSeconds((double)expirtime), System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.High, null);
            if (this._cache.Get(key) == null)
                flag = false;
            return flag;
        }

        public void Remove(string key, int islike = 0)
        {
            if (islike == 0)
                this._cache.Remove(key);
            else
            {
                var enumerator = this._cache.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    var ckey = enumerator.Key.ToString();
                    if (!ckey.StartsWith(key)) continue;
                    _cache.Remove(ckey);
                }
            }
        }

        public void TimeOut(string pattern)
        {
            var enumerator = HttpRuntime.Cache.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (!enumerator.Key.ToString().Contains(pattern)) continue;
                var key = enumerator.Key.ToString();
                var cacheObj = HttpRuntime.Cache.Get(key);
                if (cacheObj==null) continue;
                var value =
                    (dynamic)Newtonsoft.Json.JsonConvert.DeserializeObject(cacheObj.ToString());
                if (value==null) continue;
                value.BeUsed = 1;
                HttpRuntime.Cache.Remove(key);
                HttpRuntime.Cache.Insert(key, value, null,
                    DateTime.Now.AddMinutes(3), System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.High,
                    null);
            }
        }
        public List<T> GetList<T>(string keypix) where T : class
        {
            var list = new List<T>();
            var enumerator = this._cache.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var key = enumerator.Key.ToString();
                if (!key.StartsWith(keypix)) continue;
                list.Add(Newtonsoft.Json.JsonConvert.DeserializeObject<T>(_cache.Get(key).ToString(),
                    new Newtonsoft.Json.JsonSerializerSettings()
                    {
                        MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Ignore,
                        NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
                    }));
                _cache.Remove(key);
            }
            return list;
        }
    }
}