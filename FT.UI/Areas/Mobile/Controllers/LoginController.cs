using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FT.Repository;
using FT.UI.Areas.Mobile.Attributes;
using FT.Utility.CacheHelper;
using FT.Utility.Helper;
namespace FT.UI.Areas.Mobile.Controllers
{
    [Error]
    public class LoginController : BaseController
    {
        // GET: Mobile/Login
        public ActionResult Index()
        {
            //var langue = CookieHelper.GetCookieValue(Constant.MobileLanguage, "/m") ?? "cn";
            var timeZone = new DictionaryRepository().DictionaryXY(s => s.ParentId == 1 && s.IsLock == 0);
            return View(timeZone);
        }
    }
}