using System.Web.Mvc;

namespace FT.UI.Areas.Web
{
    public class WebAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Web";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Web_default",
                "Web/{controller}/{action}/{id}",
                new { action = "Index", Controller = "Home", id = UrlParameter.Optional },
                new string[] { "FT.UI.Areas.Web.Controllers" }
            );
        }
    }
}