using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FT.Utility.Helper;

namespace FT.Utility.CacheHelper
{
    public class CookieOperate
    {
        public bool Clear()
        {
            HttpContext.Current.Response.Clear();
            CookieHelper.SetCookie(Constant.PlatAdmin,"",DateTime.Now.AddHours(-2),"/PlatAdmin");
            CookieHelper.SetCookie(Constant.PlatAdminRole, "", DateTime.Now.AddHours(-2), "/PlatAdmin");
            return true;
        }

        #region PlatAdmin
        /// <summary>
        /// 获取当前Admin编号
        /// </summary>
        public int AdminId
        {
            get
            {
                var result = 0;
                try
                {
                    var cookieAdminId = CookieHelper.GetCookieValue(Constant.PlatAdmin,"/PlatAdmin");
                    if (!string.IsNullOrEmpty(cookieAdminId))
                    {
                        cookieAdminId = EncryptHelper.AesDecrypt(cookieAdminId);
                        result = Convert.ToInt32(cookieAdminId.Replace("FT_PlatAdmin", ""));
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(HttpContext.Current.Request.Url.AbsoluteUri + "验证客户端AdminId异常: \r\n" + ex.Message.ToString());
                }
                return result;
            }
            set
            {
                try
                {
                    CookieHelper.SetCookie(Constant.PlatAdmin, EncryptHelper.AesEncrypt("FT_PlatAdmin" + value), DateTime.Now.AddDays(7),"/PlatAdmin");
                }
                catch (Exception ex)
                {
                    Log.Error(HttpContext.Current.Request.Url.AbsoluteUri + "写入客户端AdminId异常: \r\n" + ex.Message);
                }
            }
        }
        public string AdminName
        {
            get
            {
                var result = "";
                try
                {
                    var cookieAdminName = CookieHelper.GetCookieValue(Constant.PlatAdminName, "/PlatAdmin");
                    if (!string.IsNullOrEmpty(cookieAdminName))
                    {
                        result = EncryptHelper.AesDecrypt(cookieAdminName).Replace("FT_PlatAdmin", "");
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(HttpContext.Current.Request.Url.AbsoluteUri + "验证客户端AdminName异常: \r\n" + ex.Message.ToString());
                }
                return result;
            }
            set
            {
                try
                {
                    CookieHelper.SetCookie(Constant.PlatAdminName, EncryptHelper.AesEncrypt("FT_PlatAdmin" + value), DateTime.Now.AddDays(7), "/PlatAdmin");
                }
                catch (Exception ex)
                {
                    Log.Error(HttpContext.Current.Request.Url.AbsoluteUri + "写入客户端AdminName异常: \r\n" + ex.Message);
                }
            }
        }
        /// <summary>
        /// 管理员角色
        /// </summary>
        public int AdminRoleId
        {
            get
            {
                var result = 0;
                try
                {
                    var cookieAdminRoleId = CookieHelper.GetCookieValue(Constant.PlatAdminRole, "/PlatAdmin");
                    if (!string.IsNullOrEmpty(cookieAdminRoleId))
                    {
                        cookieAdminRoleId = EncryptHelper.AesDecrypt(cookieAdminRoleId);
                        result = Convert.ToInt32(cookieAdminRoleId.Replace("FT_PlatAdmin", ""));
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(HttpContext.Current.Request.Url.AbsoluteUri + "验证客户端AdminRoleId异常: \r\n" + ex.Message);
                }
                return result;
            }
            set
            {
                try
                {
                    CookieHelper.SetCookie(Constant.PlatAdminRole, EncryptHelper.AesEncrypt("FT_PlatAdmin" + value), DateTime.Now.AddDays(7),"/PlatAdmin");
                }
                catch (Exception ex)
                {
                    Log.Error(HttpContext.Current.Request.Url.AbsoluteUri + "写入客户端AdminRoleId异常: \r\n" + ex.Message);
                }
            }
        }
        #endregion

        #region Mobile
        /// <summary>
        /// 获取当前MobileUserId编号
        /// </summary>
        public long MobileUserId
        {
            get
            {
                long result = 0;
                try
                {
                    var cookiemobileUserId = CookieHelper.GetCookieValue(Constant.Mobile,"/m");
                    if (!string.IsNullOrEmpty(cookiemobileUserId))
                    {
                        result = Convert.ToInt64(cookiemobileUserId);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(HttpContext.Current.Request.Url.AbsoluteUri + "验证客户端CookieId异常: \r\n" + ex.Message);
                }
                return result;
            }
            set
            {
                try
                {
                    CookieHelper.SetCookie(Constant.Mobile, value + "", DateTime.Now.AddDays(7),"/m");
                }
                catch (Exception ex)
                {
                    Log.Error(HttpContext.Current.Request.Url.AbsoluteUri + "写入客户端CookieId异常: \r\n" + ex.Message);
                }
            }
        }
        #endregion
    }
}