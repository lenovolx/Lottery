using System.Collections.Generic;

namespace FT.Plugin.Cache
{
    using System;
    public interface ICache : IStrategy
    {
        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <returns></returns>
        object Get(string key);
        /// <summary>
        /// 获取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">缓存键</param>
        /// <returns></returns>
        T Get<T>(string key) where T : class;
        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="data">缓存数据</param>
        void Insert(string key, object data);
        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="data">缓存数据</param>
        void Insert(string key, object data, int expirtime);
        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="data">缓存数据</param>
        /// <param name="expirtime">缓存时间</param>
        void Insert(string key, object data, DateTime expirtime);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <param name="expirtime"></param>
        /// <returns></returns>
        bool Add(string key, object data, int expirtime);

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="islike"></param>
        void Remove(string key,int islike=0);
        /// <summary>
        /// 设置超时
        /// </summary>
        /// <param name="pattern"></param>
        void TimeOut(string pattern);
        /// <summary>
        /// 根据键前缀获取缓存集合
        /// </summary>
        /// <param name="keypix"></param>
        /// <returns></returns>
        List<T> GetList<T>(string keypix) where T : class;
    }
}
