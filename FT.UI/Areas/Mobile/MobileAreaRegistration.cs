using System.Web.Mvc;

namespace FT.UI.Areas.Mobile
{
    public class MobileAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Mobile";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Mobile_default",
                "m/{controller}/{action}/{id}",
                new { action = "Index",controller = "Home",id = UrlParameter.Optional }
            );
        }
    }
}