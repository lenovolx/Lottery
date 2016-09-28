using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FT.Plugin.Cache;

namespace FT.UI.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //Cache.Add("asdasd", "123123", 600);
            //return View();
            return RedirectToAction("Index", "m");
        }
        public ActionResult About()
        {
            
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }
    }
}