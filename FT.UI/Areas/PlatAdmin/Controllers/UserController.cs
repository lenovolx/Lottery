using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FT.Model;
using FT.Model.QueryModel;
using FT.Model.ViewModel;
using FT.Repository;
using FT.UI.Areas.PlatAdmin.Filters;
using FT.Utility.ApiHelper;
using FT.Utility.Helper;
using Newtonsoft.Json.Linq;
using System.Data;


namespace FT.UI.Areas.PlatAdmin.Controllers
{
    /// <summary>
    /// User   
    /// </summary>
    [AdminAuthorize]
    public class UserController : BaseController
    {
        #region View
        [PublicViewBagFilterAttribute()]
        public ActionResult UserInfo()
        {
            return View(CurrentAdmin);
        }
        [PublicViewBagFilterAttribute()]
        public ActionResult CashRecord()
        {
            return View();
        }
        #endregion

        #region Ajax Method
        #region User
        [HttpPost]
        public JsonResult UserGrid(string sort, string order, int page = 1, int rows = 50, string username = "",
            int menuid = 0)
        {
            var query = new UserQueryModel()
            {
                Page = page,
                PageSize = rows,
                IsDesc = order == "desc" ? true : false,
                SortField = sort,
                UserName = username,
                RoleId = CurrentAdmin.RoleId,
                MenuId = menuid,
                AgentId = CurrentAdmin.AgentId,
                AgentLevel = CurrentAdmin.AgentLevel,
                Language = LanguageAdmin
            };
            var datagrid = new UserAccountRepository().GetUserGrid(query);
            return Json(datagrid, JsonRequestBehavior.AllowGet);
        }
        [LogInfo(type: "会员新增")]
        public ActionResult Add(UserAccount model)
        {
            var rrt = new ApiReturn();
            var reg = new UserAccountRepository().AddUser(model);
            if (reg == RegisterEnum.Success)
                rrt.code = 0;
            else
                rrt.errors = reg.ToDescription(LanguageAdmin);
            return Json(rrt, "text/html", JsonRequestBehavior.AllowGet);
        }
        [LogInfo(type: "会员编辑")]
        public ActionResult Edit(UserAccount model)
        {
            var rrt = new ApiReturn();
            var flag = new UserAccountRepository().EditUser(model);
            if (flag==RegisterEnum.Success)
                rrt.code = 0;
            else
                rrt.errors = flag.ToDescription(LanguageAdmin);
            return Json(rrt, "text/html", JsonRequestBehavior.AllowGet);
        }
        [LogInfo(type: "会员删除")]
        public JsonResult Delete(int id)//删除
        {
            var rrt = new ApiReturn();
            if (new UserAccountRepository().FrozenOrDeleteUser(id, LockEnum.Delete))
                rrt.code = 0;
            else
                rrt.errors = "操作失败";
            return Json(rrt, JsonRequestBehavior.AllowGet);
        }
        [LogInfo(type: "会员锁定")]
        public JsonResult SetUserLock(int id, int Lock)
        {
            var rrt = new ApiReturn();
            if (new UserAccountRepository().FrozenOrDeleteUser(id, (LockEnum)Lock))
                rrt.code = 0;
            else
                rrt.errors = "操作失败";
            return Json(rrt, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Cash
        public JsonResult CashGrid(DateTime? sdate,DateTime?edate,int? status, string sort, string order, int page = 1, int rows = 50, int menuid = 0)
        {
            var query = new CashRecordQueryModel()
            {
                Page = page,
                PageSize = rows,
                IsDesc = order == "desc",
                SortField = sort,
                MenuId = menuid,
                StartDate = sdate,
                EndDate = edate,Status = status,Language = LanguageAdmin
            };
            var datagrid = new CashRecordRepository().CashRecordGrid(query);
            return Json(datagrid, JsonRequestBehavior.AllowGet);
        }
        [LogInfo(type: "会员提现审核")]
        public JsonResult EditAduit(CashRecord cr)
        {
            var rrt = new ApiReturn();
            cr.AuditorName = CurrentAdmin.LoginName;
            var aduit = new CashRecordRepository().AuditCashRecord(cr);
            if (aduit)
                rrt.code = 0;
            else
                rrt.errors = "操作失败";
            return Json(rrt, JsonRequestBehavior.AllowGet);

        }
        #endregion
        #endregion

    }
}