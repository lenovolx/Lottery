using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FT.Model;
using FT.Repository;

namespace FT.UI.Areas.Mobile.Controllers
{
    public class BaseController : Controller
    {
        /// <summary>
        /// 系统配置
        /// </summary>
        public SystemSetting SystemSetting
        {
            get { return new SystemSettingRepository().GetSetting(); }
        }
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (SystemSetting.CloseWebSite)
            {
                filterContext.Result = new RedirectToRouteResult("Mobile_default",
                   new System.Web.Routing.RouteValueDictionary{
                   {"controller", "Error"},
                   {"action", "Close"}
                   });
            }
            base.OnActionExecuting(filterContext); 
        }
    }
}