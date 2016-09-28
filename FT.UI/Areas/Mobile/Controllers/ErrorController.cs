using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FT.UI.Areas.Mobile.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Mobile/Error
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Close()
        {
            return View();
        }
    }
}