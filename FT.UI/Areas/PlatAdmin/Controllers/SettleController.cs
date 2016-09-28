using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FT.Model.ViewModel;
using FT.Repository;
using FT.Model.QueryModel;
using FT.UI.Areas.PlatAdmin.Filters;

namespace FT.UI.Areas.PlatAdmin.Controllers
{
    [AdminAuthorize]
    public class SettleController : BaseController
    {
        [PublicViewBagFilterAttribute()]
        public ActionResult SettleInfo()
        {
            return View();
        }
        public JsonResult GetData(string sort, string order, int page = 1, int rows = 50, string username = "",
            int menuid = 0, int roleid = 0)
        {
            var query = new TradingInfoQueryModel()
            {
                Page = page,
                PageSize = rows,
                IsDesc = order == "desc" ? true : false,
                SortField = sort

            };

            var datagrid = new MatchUserBetRepository().GetUserBetList(query);
            return Json(datagrid, JsonRequestBehavior.AllowGet);
        }
    }
}