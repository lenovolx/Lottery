using System.Web.Mvc;

namespace FT.UI.Areas.PC
{
    public class PCAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "PC";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "PC_default",
                "PC/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}