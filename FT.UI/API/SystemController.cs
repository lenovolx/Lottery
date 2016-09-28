using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FT.Model;
using FT.Model.QueryModel;
using FT.Repository;
using FT.Utility.ApiHelper;
using FT.Utility.ExtendException;
using FT.Utility.Helper;
using Newtonsoft.Json.Linq;

namespace FT.UI.API
{
    /// <summary>
    /// 系统相关数据接口
    /// </summary>
    public class SystemController : BaseApiController
    {
        /// <summary>
        /// 管理员登录
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult Login([FromBody] JObject data)
        {
            var ret = ApiFunc(data, "---System---Login接口---", (d, p) =>
            {
                var dic = Sign.GetParameters(data.ToString());
                var userName = dic["username"] + "";
                var password = dic["password"] + "";
                if (string.IsNullOrWhiteSpace(userName))
                    throw new Exception("请传入登录帐号");
                if (string.IsNullOrWhiteSpace(password))
                    throw new Exception("请传入登录密码");
                var managerRepository = new AdminRepository();
                var manager = managerRepository.ManagerLogin(userName, password);
                if (manager != null && manager.ErrCode == 0)
                {
                    return new ApiReturn
                    {
                        data = manager,
                        code = 0
                    };
                }
                else
                    throw new Exception(manager.ErrMsg);
            });
            return Json(ret);
        }

        /// <summary>
        /// 获取充值卡信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult Card([FromBody] JObject data)
        {
            var ret = ApiFunc(data, "---System---Card接口---", (d, p) =>
            {
                var dic = Sign.GetParameters(data.ToString());
                var card = dic["card"] + "";
                var cardRepository = new CardRepository();
                return new ApiReturn
                {
                    data = cardRepository.SingleCard(new CardQueryModel
                    {
                        CardNum = card
                    }),
                    code = 0
                };
            });
            return Json(ret);
        }

        /// <summary>
        /// 轮播图(advtype 参照 PlatEnum)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public IHttpActionResult Adv([FromBody] JObject data)
        {
            var ret = ApiFunc(data, "---System---Adv接口---", (d, p) =>
            {
                var dic = Sign.GetParameters(data.ToString());
                object advtype;
                if (!dic.TryGetValue("advtype", out advtype))
                    advtype = "0";
                var type = (PlatEnum) (int.Parse(advtype + ""));
                return new ApiReturn
                {
                    data = new SiteAdvRepository().GetAdvList(s => s.Type == type&& s.IsShow==YesNoEnum.Yes),
                    code = 0
                };
            });
            return Json(ret);
        }

        /// <summary>
        /// 获取系统字典
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public IHttpActionResult Dict([FromBody] JObject data)
        {
            var ret = ApiFunc(data, "---System---Dict接口---", (d, p) =>
            {
                var dic = Sign.GetParameters(data.ToString());
                object dictnum, lang;
                if (!dic.TryGetValue("dictnum", out dictnum))
                    dictnum = "1";
                if (!dic.TryGetValue("lang", out lang))
                    lang = "cn";
                var dict = int.Parse(dictnum + "");
                return new ApiReturn
                {
                    data =
                        new DictionaryRepository().DictionaryList(s => s.ParentId == dict && s.IsLock == 0, lang + ""),
                    code = 0
                };
            });
            return Json(ret);
        }
    }
}
