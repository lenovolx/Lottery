namespace FT.Plugin.Cache.Redis
{
    using System.Collections.Generic;
    using System.Linq;
    using Utility.Helper;
    using System;
    using ServiceStack.Redis;
    public class RedisCache : ICache
    {
        private static RedisConfig RedisConfig
        {
            get
            {
                try
                {
                    return ConfigUtility<RedisConfig>.GetConfig(IoHelper.GetMapPath("/Config/Redis.config"));
                }
                catch
                {
                    return new RedisConfig
                    {
                        Host="127.0.0.1",
                        Prot = 6379
                    };
                }
            }
        }
        public RedisCache()
        {
        }
        public object Get(string key)
        {
            using (var client = new RedisClient(RedisConfig.Host, RedisConfig.Prot))
            {
                return client.Get<object>(key);
            }
        }

        public T Get<T>(string key) where T : class
        {
            using (var client = new RedisClient(RedisConfig.Host, RedisConfig.Prot))
            {
                return client.Get<T>(key);
            }
        }
        public void Insert(string key, object data, int expirtime)
        {
            using (var client = new RedisClient(RedisConfig.Host, RedisConfig.Prot))
            {
                if (client.Get(key) == null)
                    client.Set(key, data, new TimeSpan(0, expirtime, 0));
            }
        }
        public void Insert(string key, object data, DateTime expirtime)
        {
            using (var client = new RedisClient(RedisConfig.Host, RedisConfig.Prot))
            {
                if (client.Get(key) == null)
                    client.Set(key, data, expirtime);
            }
        }
        public void Insert(string key, object data)
        {
            using (var client = new RedisClient(RedisConfig.Host, RedisConfig.Prot))
            {
                if (client.Get(key) == null)
                    client.Set(key, data);
            }
        }
        public bool Add(string key, object data, int expirtime)
        {
            using (var client = new RedisClient(RedisConfig.Host, RedisConfig.Prot))
            {
                return client.Get(key) == null && client.Set(key, data, new TimeSpan(0, expirtime, 0));
            }
        }

        public void Remove(string key, int islike = 0)
        {
            using (var client = new RedisClient(RedisConfig.Host, RedisConfig.Prot))
            {
                if (islike == 0)
                    client.Del(key);
                else
                    client.RemoveByPattern(key + "*");
            }
        }

        public void RemoveAll()
        {
            using (var client = new RedisClient(RedisConfig.Host, RedisConfig.Prot))
            {
                client.FlushAll();
            }
        }

        public void TimeOut(string pattern)
        {
            using (var client = new RedisClient(RedisConfig.Host, RedisConfig.Prot))
            {
                var obj = client.SearchKeys(pattern + "_*");
                if (!obj.Any()) return;
                foreach (var item in obj)
                {
                    if (string.IsNullOrEmpty(item)) continue;
                    var cacheStr = client.Get<string>(item);
                    if (string.IsNullOrEmpty(cacheStr)) continue;
                    var value = (dynamic)Newtonsoft.Json.JsonConvert.DeserializeObject(cacheStr);
                    if (value == null) continue;
                    value.BeUsed = 1;
                    string json = Newtonsoft.Json.JsonConvert.SerializeObject(value) + "";
                    client.Replace(item, json, new TimeSpan(0, 3, 0));
                }
            }
        }
        public List<T> GetList<T>(string keypix) where T : class
        {
            using (var client = new RedisClient(RedisConfig.Host, RedisConfig.Prot))
            {
                var list = new List<T>();
                var obj = client.SearchKeys(keypix + "*");
                if (!obj.Any()) return list;
                foreach (var key in obj)
                {
                    var cacheObj = client.Get<string>(key);
                    if (cacheObj == null) continue;
                    list.Add(Newtonsoft.Json.JsonConvert.DeserializeObject<T>(cacheObj,
                        new Newtonsoft.Json.JsonSerializerSettings()
                        {
                            MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Ignore,
                            NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
                        }));
                    client.Remove(key);
                }
                return list;
            }
        }
    }
}