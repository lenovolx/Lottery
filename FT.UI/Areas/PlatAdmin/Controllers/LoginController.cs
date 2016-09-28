using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FT.Model;
using FT.Model.ViewModel;
using FT.Repository;
using FT.Utility.CacheHelper;
using FT.Utility.Helper;

namespace FT.UI.Areas.PlatAdmin.Controllers
{
    public class LoginController : BaseController
    {
        private readonly CookieOperate _cookies = new CookieOperate();
        public ActionResult Index(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model != null)
                {
                    var user = new AdminRepository().ManagerLogin(model.UserName,model.Password);
                    if (user.Admin != null)
                    {
                        if (user.Admin.Status == LockEnum.Nnormal)
                        {
                            _cookies.AdminId = user.Admin.Id;
                            _cookies.AdminRoleId = user.Admin.RoleId;
                            _cookies.AdminName = user.Admin.LoginName;
                            return RedirectToAction("Index", "");
                        }
                        else
                        {
                            ModelState.AddModelError("", "账户被锁定,请联系管理员");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "错误的用户名或密码");
                    }
                }
            }
            return View(model);
        }

        [HttpPost]
        public JsonResult ClearRole()
        {
            AspNetCache.ClearContains("_Role_");
            return Json(null);
        }
        public ActionResult Off()
        {
            _cookies.Clear();
            return RedirectToAction("Index", "Login");
        }
    }
}