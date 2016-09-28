using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using FT.Model;
using FT.Repository;
using FT.Utility.ApiHelper;
using FT.Utility.CacheHelper;
using FT.Utility.ExtendException;
using FT.Utility.Helper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FT.UI.API
{
    public class BaseApiController : ApiController
    {
        private static readonly object Obj = new object();
        public long GetOrderId()
        {
            lock (Obj)
            {
                var orderId = string.Empty;
                var random = new Random(BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0));
                for (var i = 0; i < 5; i++)
                {
                    var rand = random.Next();
                    var code = (char)('0' + (char)(rand % 10));
                    orderId += code.ToString();
                }
                return long.Parse(DateTime.Now.ToString("yyyyMMddfff") + orderId);
            }
        }
        /// <summary>
        /// API 接口请求
        /// </summary>
        /// <param name="data">参数集合</param>
        /// <param name="p">接口描述</param>
        /// <param name="func"></param>
        /// <returns></returns>
        public virtual ApiReturn ApiFunc(JObject data, string p, Func<JObject, string, ApiReturn> func)
        {
            var ret = new ApiReturn();
            try
            {
                Log.Debug(string.Format("{0}请求参数", p) + data);
                ret = func(data, p);
            }
            catch (FtException ft)
            {
                Log.Error(string.Format("{0}异常", p), ft);
                ret.errors = ft.Message;
            }
            catch (Exception ex)
            {
                Log.Error(string.Format("{0}异常", p), ex);
                ret.errors = ex.Message;
            }
            return ret;
        }
    }
}