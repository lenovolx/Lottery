
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;

namespace FT.Plugin.Cache
{
    using Autofac;
    using Autofac.Builder;
    using Autofac.Configuration;
    using System;
    public static class Cache
    {
        private static ICache _cache = null;
        private static readonly object _cacheLocker = new object();
        static Cache()
        {
            Load();
        }
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <returns></returns>
        public static object Get(string key)
        {

            return string.IsNullOrWhiteSpace(key) ? null : _cache.Get(key);
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <returns></returns>
        public static T Get<T>(string key) where T : class
        {
            return string.IsNullOrWhiteSpace(key) ? null : _cache.Get<T>(key);
        }
        /// <summary>
        /// 根据键前缀获取缓存集合
        /// </summary>
        /// <param name="keypix">键前缀</param>
        /// <returns></returns>
        public static List<T> GetList<T>(string keypix) where T : class
        {
            return string.IsNullOrWhiteSpace(keypix) ? null : _cache.GetList<T>(keypix);
        }
        /// <summary>
        /// 插入缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="data">值</param>
        public static void Insert(string key, object data)
        {
            if (!string.IsNullOrWhiteSpace(key) && (data != null))
            {
                lock (_cacheLocker)
                {
                    _cache.Insert(key, data);
                }
            }
        }
        /// <summary>
        /// 插入缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="data">缓存数据</param>
        /// <param name="expirtime">缓存时间[参数单位:分钟]</param>
        public static void Insert(string key, object data, int expirtime)
        {
            if (!string.IsNullOrWhiteSpace(key) && (data != null))
            {
                lock (_cacheLocker)
                {
                    _cache.Insert(key, data, expirtime);
                }
            }
        }
        /// <summary>
        /// 插入缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="data">缓存数据</param>
        /// <param name="expirtime">缓存时间</param>
        public static void Insert(string key, object data, DateTime expirtime)
        {
            if (!string.IsNullOrWhiteSpace(key) && (data != null))
            {
                lock (_cacheLocker)
                {
                    _cache.Insert(key, data, expirtime);
                }
            }
        }
        /// <summary>
        /// 插入缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="data">缓存数据</param>
        /// <param name="expirtime">缓存时间(单位：分钟)</param>
        /// <returns>bool</returns>
        public static bool Add(string key, object data, int expirtime)
        {
            if (!string.IsNullOrWhiteSpace(key) && (data != null))
            {
                lock (_cacheLocker)
                {
                    return _cache.Add(key, data, expirtime);
                }
            }
            else
                return false;
        }

        /// <summary>
        /// 设置超时
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="T"></param>
        public static void TimeOut(string pattern)
        {
            _cache.TimeOut(pattern);
        }

        private static void Load()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<ICache>();
            builder.RegisterModule(new ConfigurationSettingsReader("autofac"));
            IContainer context = null;
            try
            {
                context = builder.Build(ContainerBuildOptions.None);
                _cache = context.Resolve<ICache>();
            }
            catch (Exception exception)
            {
                throw new Exception("注册缓存服务异常", exception);
            }
            finally
            {
                if (context != null)
                {
                    context.Dispose();
                }
            }
        }

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="islike"></param>
        public static void Remove(string key,int islike=0)
        {
            if (string.IsNullOrWhiteSpace(key)) return;
            _cache.Remove(key, islike);
        }
    }
}

