using System.Web.Mvc;

namespace FT.UI.Areas.PlatAdmin
{
    public class PlatAdminAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "PlatAdmin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "PlatAdmin_default",
                "PlatAdmin/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                new string[] { "FT.UI.Areas.PlatAdmin.Controllers" }
            );
        }
    }
}