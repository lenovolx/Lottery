using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FT.Repository;
using FT.Utility.CacheHelper;
using FT.Utility.Helper;

namespace FT.UI.Areas.PlatAdmin.Filters
{
    /// <summary>
    /// 公共ViewBag输出
    /// </summary>
    public class PublicViewBagFilterAttribute : FilterAttribute, IResultFilter
    {
        /// <summary>
        /// 登录用户角色编号
        /// </summary>
        public int RoleId
        {
            get
            {
                return
                    IntTryParse(
                        EncryptHelper.AesDecrypt(CookieHelper.GetCookieValue(Constant.PlatAdminRole, "/PlatAdmin"))
                            .Replace("FT_PlatAdmin", ""));
            }
        }
        /// <summary>
        /// 系统语言
        /// </summary>
        public string PlatLanguage
        {
            get { return CookieHelper.GetCookieValue(Constant.PlatAdminLanguage, "/PlatAdmin"); }
        }
        public PublicViewBagFilterAttribute() { }
        public void OnResultExecuted(ResultExecutedContext filterContext)
        {

        }
        public void OnResultExecuting(ResultExecutingContext filterContext)
        {
            var menuid = LongTryParse(filterContext.RequestContext.HttpContext.Request.Params["MenuId"]);
            filterContext.Controller.ViewBag.MenuId = menuid;
            filterContext.Controller.ViewBag.RoleButtons = new RoleRepository().GetRoleButton(RoleId, menuid, PlatLanguage);
        }

        #region Private Method
        private static long LongTryParse(string str)
        {
            long id = 0;
            long.TryParse(str, out id);
            return id;
        }
        private static int IntTryParse(string str)
        {
            int id = 0;
            int.TryParse(str, out id);
            return id;
        }
        #endregion
    }
}