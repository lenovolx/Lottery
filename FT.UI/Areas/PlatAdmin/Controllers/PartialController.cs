using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FT.Model.ViewModel;
using FT.Repository;
using FT.Model.QueryModel;

namespace FT.UI.Areas.PlatAdmin.Controllers
{
    public class PartialController : BaseController
    {
        public ActionResult LeftMenu()
        {
            var model = new RoleRepository().GetRoleMenu(CurrentAdmin.RoleId,CurrentAdmin.Id.Value,LanguageAdmin);
            return PartialView("~/Areas/PlatAdmin/Views/Shared/_LeftMenu.cshtml", model);
        }
        [HttpGet]
        public JsonResult AdminInformation()
        {
            return Json(CurrentAdmin, JsonRequestBehavior.AllowGet);
        }
    }
}
