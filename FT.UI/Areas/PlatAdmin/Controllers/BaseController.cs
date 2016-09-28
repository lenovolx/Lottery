using System;
using System.Collections.Generic;
using System.Configuration;
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
    public class BaseController : Controller
    {
        public static int RoleId { get; set; }
        public AdminViewModel CurrentAdmin
        {
            get
            {
                var info = new AdminViewModel();
                try
                {
                    var cookieAdminId = CookieHelper.GetCookieValue(Constant.PlatAdmin,"/PlatAdmin");
                    if (!string.IsNullOrEmpty(cookieAdminId))
                    {
                        cookieAdminId = EncryptHelper.AesDecrypt(cookieAdminId);
                        long adminId = Convert.ToInt32(cookieAdminId.Replace("FT_PlatAdmin", ""));
                        var model = new AdminRepository().SingleAdmin(s => s.Id == adminId, LanguageAdmin);
                        if (model != null)
                        {
                            info = model;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Info(Request.Url.AbsoluteUri + "验证客户端CookieId异常: \r\n" + ex.Message.ToString());
                }
                return info;
            }
        }
        /// <summary>
        /// 系统语言
        /// </summary>
        public string LanguageAdmin
        {
            get
            {
                var lang= CookieHelper.GetCookieValue(Constant.PlatAdminLanguage, "/PlatAdmin");
                return lang ?? "cn";
            }
        }
    }
}