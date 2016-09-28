using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace FT.Utility.Helper
{
    public class CookieHelper
    {

        #region 获取Cookie

        /// <summary> 
        /// 获得Cookie的值 
        /// </summary> 
        /// <param name="cookieName"></param>
        /// <param name="path"></param>
        /// <returns></returns> 
        public static string GetCookieValue(string cookieName,string path="/")
        {
            return GetCookieValue(cookieName, null, path);
        }

        /// <summary> 
        /// 获得Cookie的值 
        /// </summary> 
        /// <param name="cookieName"></param> 
        /// <param name="key"></param>
        /// <param name="path"></param>
        /// <returns></returns> 
        public static string GetCookieValue(string cookieName, string key, string path = "/")
        {
            var request = HttpContext.Current.Request;
            return GetCookieValue(request.Cookies[cookieName], key);
        }

        /// <summary> 
        /// 获得Cookie的子键值 
        /// </summary> 
        /// <param name="cookie"></param> 
        /// <param name="key"></param>
        /// <param name="path"></param>
        /// <returns></returns> 
        public static string GetCookieValue(HttpCookie cookie, string key, string path = "/")
        {
            if (cookie == null) return "";
            cookie.Path = path;
            if (!string.IsNullOrEmpty(key) && cookie.HasKeys)
                return cookie.Values[key];
            else
                return cookie.Value;
        }

        /// <summary> 
        /// 获得Cookie 
        /// </summary> 
        /// <param name="cookieName"></param> 
        /// <returns></returns> 
        public static HttpCookie GetCookie(string cookieName)
        {
            var request = HttpContext.Current.Request;
            return request.Cookies[cookieName];
        }

        #endregion

        #region 删除Cookie

        /// <summary> 
        /// 删除Cookie 
        /// </summary> 
        /// <param name="cookieName"></param> 
        public static void RemoveCookie(string cookieName)
        {
            RemoveCookie(cookieName, null);
        }

        /// <summary> 
        /// 删除Cookie的子键 
        /// </summary> 
        /// <param name="cookieName"></param> 
        /// <param name="key"></param> 
        public static void RemoveCookie(string cookieName, string key)
        {
            var response = HttpContext.Current.Response;
            var cookie = response.Cookies[cookieName];
            if (cookie == null) return;
            if (!string.IsNullOrEmpty(key) && cookie.HasKeys)
                cookie.Values.Remove(key);
            else
                response.Cookies.Remove(cookieName);
        }

        #endregion

        #region 设置/修改Cookie

        /// <summary> 
        /// 设置Cookie子键的值 
        /// </summary> 
        /// <param name="cookieName"></param> 
        /// <param name="key"></param> 
        /// <param name="value"></param> 
        public static void SetCookie(string cookieName, string key, string value)
        {
            SetCookie(cookieName, key, value, null);
        }

        /// <summary> 
        /// 设置Cookie值 
        /// </summary> 
        /// <param name="key"></param> 
        /// <param name="value"></param> 
        public static void SetCookie(string key, string value)
        {
            SetCookie(key, null, value, null);
        }

        /// <summary> 
        /// 设置Cookie值和过期时间 
        /// </summary> 
        /// <param name="key"></param> 
        /// <param name="value"></param> 
        /// <param name="expires"></param>
        /// <param name="path">路径</param> 
        public static void SetCookie(string key, string value, DateTime expires,string path="/")
        {
            SetCookie(key, null, value, expires, path);
        }

        /// <summary> 
        /// 设置Cookie过期时间 
        /// </summary> 
        /// <param name="cookieName"></param> 
        /// <param name="expires"></param> 
        public static void SetCookie(string cookieName, DateTime expires)
        {
            SetCookie(cookieName, null, null, expires);
        }

        /// <summary> 
        /// 设置Cookie 
        /// </summary> 
        /// <param name="cookieName"></param> 
        /// <param name="key"></param> 
        /// <param name="value"></param> 
        /// <param name="expires"></param>
        /// <param name="path"></param> 
        public static void SetCookie(string cookieName, string key, string value, DateTime? expires,string path="/")
        {
            var response = HttpContext.Current.Response;
            var cookie = response.Cookies[cookieName];
            if (cookie == null) return;
            cookie.Path = path;
            if (!string.IsNullOrEmpty(key) && cookie.HasKeys)
                cookie.Values.Set(key, value);
            else
                if (!string.IsNullOrEmpty(value))
                    cookie.Value = value;
            if (expires != null)
                cookie.Expires = expires.Value;
            response.SetCookie(cookie);
        }
        #endregion
    }
}