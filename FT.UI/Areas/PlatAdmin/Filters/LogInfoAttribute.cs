using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using FT.Model;
using FT.Model.ViewModel;
using FT.Repository;
using FT.Utility.CacheHelper;
using FT.Utility.Helper;
using Newtonsoft.Json;
namespace FT.UI.Areas.PlatAdmin.Filters
{
    /// <summary>
    /// 数据操作 日志记录
    /// </summary>
    public class LogInfoAttribute : ActionFilterAttribute
    {
        private readonly string _type;
        public LogInfoAttribute(string type)
        {
            _type = type;
        }

        public string AdminName
        {
            get
            {
                var result = string.Empty;
                var cookieAdminName = CookieHelper.GetCookieValue(Constant.PlatAdminName, "/PlatAdmin");
                if (!string.IsNullOrEmpty(cookieAdminName))
                {
                    result = EncryptHelper.AesDecrypt(cookieAdminName).Replace("FT_PlatAdmin", "");
                }
                return result;
            }
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.ActionDescriptor.IsDefined(typeof (AllowAnonymousAttribute), inherit: true) ||
                filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof (AllowAnonymousAttribute),
                    inherit: true)) return;
            try
            {
                new LogInfoRepository().Insert(new LogInfo
                {
                    Id = Guid.NewGuid().ToString(),
                    LogTime = DateTime.Now,
                    LogUser = AdminName,
                    LogType = _type,
                    LogUserIp = GetIp(),
                    LogController = filterContext.RouteData.Values["controller"].ToString(),
                    LogAction = filterContext.RouteData.Values["action"].ToString(),
                    LogOperate = GetParmValue(filterContext)
                });
            }
            catch
            {
                // ignored
            }
        }

        private static string GetIp()
        {
            var ip = string.Empty;
            if (!string.IsNullOrEmpty(HttpContext.Current.Request.ServerVariables["HTTP_VIA"]))
                ip = Convert.ToString(HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"]);
            if (string.IsNullOrEmpty(ip))
                ip = Convert.ToString(HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]);
            return ip;
        }
        private static string GetParmValue(ActionExecutingContext filterContext)
        {
            var pv = "";
            var parametersCount = filterContext.ActionParameters.Count;
            if (parametersCount <= 0) return pv;
            var keys = filterContext.ActionParameters.Keys;
            foreach (var key in keys)
            {
                var value = filterContext.ActionParameters[key];
                if (null == value)
                    continue;
                if (value.GetType().IsClass && value.GetType().Name != "String")
                {
                    var objClass = value;
                    var infos = objClass.GetType().GetProperties();
                    pv = infos.Where(info => info.CanRead).Aggregate(pv, (current, info) => current + (info.Name + "=" + info.GetValue(objClass, null) + ","));
                }
                else
                {
                    pv += key + "=" + value + ",";
                }
            }
            return pv;
        }
    }
}