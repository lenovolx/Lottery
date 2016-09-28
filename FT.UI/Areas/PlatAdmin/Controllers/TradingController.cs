using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FT.Model;
using FT.UI.Areas.PlatAdmin.Filters;
using FT.Model.QueryModel;
using FT.Repository;
using FT.Utility.ApiHelper;
using FT.Utility.Helper;

namespace FT.UI.Areas.PlatAdmin.Controllers
{
    [AdminAuthorize]
    public class TradingController : BaseController
    {
        [PublicViewBagFilterAttribute()]
        public ActionResult TradingInfo()
        {
            return View();
        }
        [PublicViewBagFilterAttribute()]
        public ActionResult FinacialDetail()
        {
            return View();
        }
        //转账、充值
        public JsonResult TradGrid(int? type, DateTime? sdate, DateTime? edate, int menuid, string sort, string order, int page = 1, int rows = 50)
        {
            var query = new TradeRecordQueryModel()
            {
                Page = page,
                PageSize = rows,
                IsDesc = order == "desc",
                SortField = sort,
                Type = type,
                StartDate = sdate,
                EndDate = edate,
                MenuId = menuid,
                RoleId = CurrentAdmin.RoleId,Language = LanguageAdmin
            };
            var datagrid = new TradeRecordRepository().GetTradeRecordGrid(query);
            return Json(datagrid, JsonRequestBehavior.AllowGet);
        }
        //财务流水
        public JsonResult AmountWaterGrid(int? type, DateTime? sdate, DateTime? edate,string username, int menuid, string sort, string order, int page = 1, int rows = 50)
        {
            var query = new AmountWaterQueryModel()
            {
                Page = page,
                PageSize = rows,
                IsDesc = order == "desc",
                SortField = sort,
                Type = type,
                StartDate = sdate,
                EndDate = edate,
                MenuId = menuid,
                RoleId = CurrentAdmin.RoleId,
                Language = LanguageAdmin,
                UserName = username
            };
            var datagrid = new AmountWaterRepository().AmountWaterGrid(query);
            return Json(datagrid, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [LogInfo(type:"删除充值转账")]
        public JsonResult Delete(string ids)
        {
            var rrt = new ApiReturn();
            var stringIds = ids.Split(',').Select(long.Parse);
            if (new TradeRecordRepository().Update(s => stringIds.Contains(s.Id),u=>new TradeRecord
            {
                 Status = LockEnum.Delete
            }))
                rrt.code = 0;
            else
                rrt.errors = RegisterEnum.Failure.ToDescription(LanguageAdmin);
            return Json(rrt, JsonRequestBehavior.AllowGet);
        }
    }
}