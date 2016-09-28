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
    public class MatchController : BaseController
    {
        [PublicViewBagFilterAttribute()]
        public ActionResult MatchInfo()
        {
            return View();
        }
        [PublicViewBagFilterAttribute()]
        public ActionResult SettleRecord()
        {
            return View();
        }
        [PublicViewBagFilterAttribute()]
        public ActionResult LeagueBlackList()
        {
            return View();
        }
        #region Ajax Method

        public JsonResult MatchGrid(string leagueName, DateTime? sdate, DateTime? edate,string team, int? isOpen, int? isSet, string sort, string order, int page = 1, int rows = 50, int menuid = 0,int datasource=0)
        {
            var query = new MatchQueryModel()
            {
                Page = page,
                PageSize = rows,
                IsDesc = order == "desc",
                SortField = sort,
                LeagueName = leagueName,
                StartDate = sdate,
                EndDate = edate.Value.AddDays(1),
                IsSettle = isSet,
                IsOpen = isOpen,
                RoleId = CurrentAdmin.RoleId,
                MenuId = menuid,
                Language = LanguageAdmin,
                DataSource = datasource,
                Team = team
            };
            var datagrid = new MatchInfoRepository().GetMatchList(query);
            return Json(datagrid, JsonRequestBehavior.AllowGet);
        }

        public JsonResult BetMatchGrid(DateTime? sdate, DateTime? edate, string username, int? isSet, string sort, long matchid, long? betid,
            string order, int page = 1, int rows = 20, int source=0)
        {
            var grid = new MatchUserBetRepository().MatchBetGrid(new UserBetQueryModel
            {
                Page = page,
                PageSize = rows,
                IsDesc = order == "desc",
                SortField = sort,
                MatchId = matchid,
                Language = LanguageAdmin,
                UserName = username,
                BetId = betid,
                Source = source
            });
            return Json(grid, JsonRequestBehavior.AllowGet);
        }
        public JsonResult BlackLeague(string leaguename, int? isBan, string sort,string order, int page = 1, int rows = 20)
        {
            var grid = new MatchBlackListRepository().BlackLeagueGrid(new BlackLeagueQueryModel
            {
                Page = page,
                PageSize = rows,
                IsDesc = order == "desc",
                SortField = sort,
                Language = LanguageAdmin,
                LeagueName = leaguename,
                IsBan = isBan
            });
            return Json(grid, JsonRequestBehavior.AllowGet);
        }
        public JsonResult BetDetail(long? betid)
        {
            var grid = new MatchUserBetRepository().MatchBetContent(new MatchUserBetContentQueryModel
            {
                IsDesc = true,
                SortField = "BetType",
                BetId = betid
            });
            return Json(grid, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [LogInfo(type: "比赛结算")]
        public JsonResult BatchSettleMatch(long matchid, int source=0)
        {
            var api = new ApiReturn();
            var flag = new MatchInfoRepository().MatchSettlement(matchid, CurrentAdmin.Id.Value, source);
            if (flag)
                api.code = 0;
            else
                api.errors = RegisterEnum.Failure.ToDescription(LanguageAdmin);
            return Json(api, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [LogInfo(type: "联赛黑名单(加入)")]
        public JsonResult MatchBlackInfo(string matchids, int datasource=0)
        {
            var api = new ApiReturn();
            var flag =
                new MatchInfoRepository().MatchBlackInfo(matchids.Split(',')
                    .Select(long.Parse), CurrentAdmin.Id.Value, datasource);
            if (flag)
                api.code = 0;
            else
                api.errors = RegisterEnum.Failure.ToDescription(LanguageAdmin);
            return Json(api, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [LogInfo(type:"联赛黑名单(加入、移除)")]
        public JsonResult LeagueRemoveBlack(string matchids,int type=0)
        {
            var api = new ApiReturn();
            var flag =
                new MatchBlackListRepository().SetLeagueBan((YesNoEnum)type, matchids.Split(',')
                    .Select(long.Parse), CurrentAdmin.Id.Value);
            if (flag)
                api.code = 0;
            else
                api.errors = RegisterEnum.Failure.ToDescription(LanguageAdmin);
            return Json(api, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [LogInfo(type: "比赛取消")]
        public JsonResult BatchCancelMatch(string matchids, int source = 0)
        {
            var api = new ApiReturn();
            var flag =
                new MatchInfoRepository().CancelMatch(matchids.Split(',')
                    .Select(long.Parse), CurrentAdmin.Id.Value, source);
            if (flag)
                api.code = 0;
            else
                api.errors = RegisterEnum.Failure.ToDescription(LanguageAdmin);
            return Json(api, JsonRequestBehavior.AllowGet);
        }
        public JsonResult MatchResult(long matchId, int source=0)
        {
            return Json(new MatchInfoRepository().GetMatchResult(matchId, source), JsonRequestBehavior.AllowGet);
        }
        #endregion

    }
}