using System.Web;
using System.Web.Mvc;
using FT.Model;
using FT.Repository;
using FT.Utility.ApiHelper;
using FT.Utility.CacheHelper;

namespace FT.UI.Areas.Mobile.Filters
{
    public class UserAuthorizeAttribute : AuthorizeAttribute
    {
        public string ControllerName { get; private set; }
        public string ActionName { get; private set; }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var cookieOperate = new CookieOperate(); 
            var userId = cookieOperate.MobileUserId;
            if (userId <= 0)
                return base.AuthorizeCore(httpContext);
            var user = new UserAccountRepository().Find(s => s.Id == userId && s.Status==LockEnum.Nnormal);
            return user != null || base.AuthorizeCore(httpContext);
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
                        code = 99,
                        errors = "登录超时"
                    },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            else
            {
                filterContext.Result = new RedirectToRouteResult("Mobile_default",
                    new System.Web.Routing.RouteValueDictionary{
                   {"controller", "Login"},
                   {"action", "Index"}
                   });
            }
        }
    }
}