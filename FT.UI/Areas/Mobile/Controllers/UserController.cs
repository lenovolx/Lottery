using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FT.UI.Areas.Mobile.Attributes;
using FT.UI.Areas.Mobile.Filters;

namespace FT.UI.Areas.Mobile.Controllers
{
    [Error]
    [UserAuthorize]
    public class UserController : BaseController
    {
        // GET: Mobile/User
        #region view
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Deal()
        {
            return View();
        }
        public ActionResult History()
        {
            return View();
        }
        public ActionResult Bank()
        {
            return View();
        }
        public ActionResult HistoryDetail()
        {
            return View();
        }
        #endregion
    }
}