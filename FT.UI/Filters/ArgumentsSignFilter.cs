using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using FT.Model;
using FT.Plugin.Cache;
using FT.Utility.ApiHelper;
using FT.Utility.CacheHelper;
using FT.Utility.ExtendException;
using FT.Utility.Helper;
using Newtonsoft.Json;

namespace FT.UI.Filters
{
    /// <summary>
    /// 参数、Token校验器
    /// </summary>
    public class ArgumentsSignFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext context)
        {
            object data = "";
            if (context.ActionArguments.TryGetValue("data", out data))
            {
                var json = data.ToString();
                if (!string.IsNullOrEmpty(json))
                {
                    var dic = Sign.GetParameters(json);
                    if (Sign.Validate(dic))
                    {
                        if (context.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any())
                            return;
                        //Token校验
                        //var userCache = AspNetCache.Get<string>(dic["ticket"] + "");
                        var userCache = Cache.Get<string>(dic["ticket"] + "");
                        Log.Info(userCache);
                        if (string.IsNullOrEmpty(userCache))
                        {
                            context.Response = context.Request.CreateResponse(new ApiReturn
                            {
                                code = 98,
                                errors = HttpErrorEnum.LoginFailed.ToDescription(dic["lang"] + "")
                            });
                        }
                        else
                        {
                            var response = JsonConvert.DeserializeObject<UserResponse>(userCache, new JsonSerializerSettings()
                            {
                                MissingMemberHandling = MissingMemberHandling.Ignore,
                                NullValueHandling = NullValueHandling.Ignore
                            });
                            if (response != null && response.User.Id == long.Parse((dic["userid"] + "")) && response.BeUsed == 0)
                                return;
                            else
                                context.Response = context.Request.CreateResponse(new ApiReturn
                                {
                                    code = 99,
                                    errors = HttpErrorEnum.AccountLoginOther.ToDescription(dic["lang"] + "")
                                });
                        }
                    }
                    else
                        context.Response = context.Request.CreateResponse(new ApiReturn
                        {
                            errors = HttpErrorEnum.SignError.ToDescription(dic["lang"] + "")
                        });
                }
                else
                    context.Response = context.Request.CreateResponse(new ApiReturn
                    {
                        errors = "未配置任何参数"
                    });
            }
            else
                context.Response = context.Request.CreateResponse(new ApiReturn
                {
                    errors = "未配置任何参数"
                });
        }
    }
}