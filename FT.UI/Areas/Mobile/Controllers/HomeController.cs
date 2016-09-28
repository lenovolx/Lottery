using FT.UI.Areas.Mobile.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FT.UI.Areas.Mobile.Filters;

namespace FT.UI.Areas.Mobile.Controllers
{
    [Error]
    [UserAuthorize]
    public class HomeController : BaseController
    {
        // GET: Mobile/Home
        public ActionResult Index()
        {
            return View();
        }
    }
}