using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FT.Model;
using FT.Model.QueryModel;
using FT.Model.ViewModel;
using FT.Repository;
using FT.UI.Filters;
using FT.Utility.ApiHelper;
using FT.Utility.CacheHelper;
using FT.Utility.ExtendException;
using FT.Utility.Helper;
using Newtonsoft.Json.Linq;

namespace FT.UI.API
{
    /// <summary>
    /// 比赛相关数据接口
    /// </summary>
    public class MatchController : BaseApiController
    {
        /// <summary>
        /// 获取所有比赛(比赛数据返回)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult List([FromBody] JObject data)
        {
            var api = ApiFunc(data, "---Match---List接口---", (d, p) =>
            {
                var ret = new ApiReturn();
                var dic = Sign.GetParameters(data.ToString());
                object datatype, league, lang, bettype, oddbettype, source,zone;
                if (!dic.TryGetValue("datatype", out datatype))
                    datatype = "xml";
                if (!dic.TryGetValue("league", out league))
                    league = "";
                if (!dic.TryGetValue("lang", out lang))
                    lang = "cn";
                if (!dic.TryGetValue("bettype", out bettype))
                    bettype = 99;
                if (!dic.TryGetValue("oddbettype", out oddbettype))
                    oddbettype = 99;
                if (!dic.TryGetValue("source", out source))
                    source = "0";
                if (!dic.TryGetValue("zone", out zone))
                    zone = "8";
                var query = new MatchQueryModel
                {
                    LeagueName = league + "",
                    BetType = bettype + "",
                    OddBetType = oddbettype + "",
                    DataType = datatype.ToString(),
                    Language = lang + "",
                    DataSource = int.Parse(source + ""),
                    TimeZone = double.Parse(zone + "")
                };
                LeaMatchViewModel match;
                switch (query.DataSource)
                {
                    case 1:
                        match =
                    new MatchInfoRepository().GetRbLeagueMatch(query);
                        break;
                    default:
                        match =
                    new MatchInfoRepository().GetLeagueMatch(query);
                        break;
                }
                ret.data = match;
                ret.code = 0;
                return ret;
            });
            return Json(api);
        }
        /// <summary>
        /// 获取比赛详情
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult Detail([FromBody] JObject data)
        {
            var api = ApiFunc(data, "---Match---Detail接口---", (d, p) =>
            {
                var ret = new ApiReturn();
                var dic = Sign.GetParameters(data.ToString());
                var matchids = dic["matchids"] + "";
                object datatype, league, lang;
                if (!dic.TryGetValue("datatype", out datatype))
                    datatype = "xml";
                if (!dic.TryGetValue("league", out league))
                    league = "";
                if (!dic.TryGetValue("lang", out lang))
                    lang = "cn";
                var query = new MatchQueryModel
                {
                    LeagueName = league + "",
                    DataType = datatype.ToString(),
                    Language = lang + "",
                    MatchIds = matchids
                };
                var match =
                    new MatchInfoRepository().GetMatchDetail(query);
                ret.data = match;
                ret.code = 0;
                return ret;
            });
            return Json(api);
        }
        /// <summary>
        /// 获取比赛结果
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult Result([FromBody] JObject data)
        {
            var ret = new ApiReturn();
            return Json(ret);
        }
        /// <summary>
        /// 获取联赛信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult League([FromBody] JObject data)
        {
            var api = ApiFunc(data, "---Match---League接口---", (d, p) =>
            {
                var ret = new ApiReturn();
                var dic = Sign.GetParameters(data.ToString());
                object lang, source, page, size, zone;
                if (!dic.TryGetValue("lang", out lang))
                    lang = "cn";
                if (!dic.TryGetValue("source", out source))
                    source = "0";
                if (!dic.TryGetValue("page", out page))
                    page = "1";
                if (!dic.TryGetValue("size", out size))
                    size = "5";
                if (!dic.TryGetValue("zone", out zone))
                    zone = "8";
                var query = new MatchQueryModel
                {
                    Language = lang + "",
                    DataSource = int.Parse(source + ""),
                    Page = int.Parse(page + ""),
                    PageSize = int.Parse(size + ""),
                    TimeZone = double.Parse(zone + "")
                };
                LeagueTotal league;
                switch (query.DataSource)
                {
                    case 1:
                        league =
                            new MatchInfoRepository().GetRbMatchLeague(query);
                        break;
                    default:
                        league =
                            new MatchInfoRepository().GetMatchLeague(query);
                        break;
                }
                ret.data = league;
                ret.code = 0;
                return ret;
            });
            return Json(api);
        }

        /// <summary>
        /// 获取滚球比赛(比赛数据返回)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult RbList([FromBody] JObject data)
        {
            var api = ApiFunc(data, "---Match---RbList接口---", (d, p) =>
            {
                var ret = new ApiReturn();
                var dic = Sign.GetParameters(data.ToString());
                object lang;
                if (!dic.TryGetValue("lang", out lang))
                    lang = "cn";
                var query = new MatchQueryModel
                {
                    Language = lang + ""
                };
                ret.data = new MatchInfoRepository().GetRbLeagueMatch(query);
                ret.code = 0;
                return ret;
            });
            return Json(api);
        }
    }
}
