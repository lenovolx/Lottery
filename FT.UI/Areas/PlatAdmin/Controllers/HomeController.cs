using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FT.UI.Areas.PlatAdmin.Filters;

namespace FT.UI.Areas.PlatAdmin.Controllers
{
    [AdminAuthorize]
    public class HomeController : BaseController
    {
        [PublicViewBagFilter]
        public ActionResult Index()
        {
            return View();
        }
    }
}