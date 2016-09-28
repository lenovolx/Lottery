using System.Web;
using System.Web.Mvc;
using FT.Repository;
using FT.Utility.ApiHelper;
using FT.Utility.CacheHelper;

namespace FT.UI.Areas.PlatAdmin.Filters
{
    /// <summary>
    /// 登录校验
    /// </summary>
    public class AdminAuthorizeAttribute : AuthorizeAttribute
    {
        public string ControllerName { get; private set; }
        public string ActionName { get; private set; }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var cookieOperate = new CookieOperate();
            var adminId = cookieOperate.AdminId;
            var roleId = cookieOperate.AdminRoleId;
            if (adminId <= 0 && roleId<=0) return base.AuthorizeCore(httpContext);
            var adminUser = new AdminRepository().Find(s => s.Id == adminId);
            return adminUser != null || base.AuthorizeCore(httpContext);
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            ControllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            ActionName = filterContext.ActionDescriptor.ActionName;
            base.OnAuthorization(filterContext);
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                filterContext.Result = new JsonResult
                {
                    Data = new ApiReturn
                    {
                        code = 98,
                        errors = "登录超时"
                    },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };

                //filterContext.Result = new JavaScriptResult
                //{
                //    Script = "top.location.href = '/Login/Index'"
                //};
            }
            else
            {
                filterContext.Result = new RedirectToRouteResult("PlatAdmin_default",
                    new System.Web.Routing.RouteValueDictionary{
                   {"controller", "Login"},
                   {"action", "Index"}
                   });
            }
        }
    }
}