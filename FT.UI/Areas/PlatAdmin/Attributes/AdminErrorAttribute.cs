using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FT.Utility.Helper;

namespace FT.UI.Areas.PlatAdmin.Attributes
{
    /// <summary>
    /// 异常捕捉
    /// </summary>
    public class AdminErrorAttribute : ActionFilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            var message = string.Format("异常页面：{0}\r\n引发异常源：{1}\r\n消息类型：{2}\r\n消息内容：{3}\r\n引发异常的方法：{4}\r\n堆栈信息：{5}"
                , HttpContext.Current.Request.RawUrl
                , filterContext.Exception.Source
                , filterContext.Exception.GetType().Name
                , filterContext.Exception.Message
                , filterContext.Exception.TargetSite,
                filterContext.Exception.StackTrace
                );
            Log.Error(message);
            filterContext.ExceptionHandled = true;
            filterContext.Result = new RedirectResult("/platadmin/error");
        }
    }
}