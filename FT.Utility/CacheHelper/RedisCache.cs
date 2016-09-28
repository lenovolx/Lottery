using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT.Utility.CacheHelper
{
    public class RedisCache
    {
        public static object Get(string key)
        {
            throw new NotImplementedException();
        }

        public static T Get<T>(string key) where T : class
        {
            throw new NotImplementedException();
        }

        public static void Insert(string key, object data)
        {
            throw new NotImplementedException();
        }

        public static void Insert(string key, object data, int expirtime)
        {
            throw new NotImplementedException();
        }

        public static void Insert(string key, object data, DateTime expirtime)
        {
            throw new NotImplementedException();
        }

        public static bool Add(string key, object data, int expirtime)
        {
            throw new NotImplementedException();
        }

        public static void Remove(string key)
        {
            throw new NotImplementedException();
        }

        public static void Clear()
        {
            throw new NotImplementedException();
        }
    }
}
