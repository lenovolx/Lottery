using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using FT.Utility.ApiHelper;
using FT.Utility.Helper;
using FT.WinForm.Login;
using System.IO;
namespace FT.WinForm.Http
{
    public static class HttpBase
    {
        public static string APIKey()
        {
            string key = ConfigurationManager.AppSettings["APIKey"];
            return key;
        }

        public static string ServerAddress()
        {
            string Address = ConfigurationManager.AppSettings["ServerUrl"];
            return Address;
        }
        public static string GetAbsoluteUrl(string url)
        {
            string _url = ServerAddress() + url;
            return _url;
        }
        public static string GetZone()
        {
            string zone = ConfigurationManager.AppSettings["Zone"];
            return zone;
        }
        public static ApiReturn HttpPost(this string uri, HttpParam param)
        {
            string tmpuri = uri;
            uri = GetAbsoluteUrl(uri);
            if (!uri.ToLower().Contains("http"))  //用于编码环境下调用验证
                return null;
            if (!param.Keys.Contains("ticket"))
            {
                param.Add("ticket", UserLogin.CurrentUser != null ? UserLogin.CurrentUser.Ticket : "");
            }
            if (!param.Keys.Contains("userid"))
            {
                param.Add("userid", UserLogin.CurrentUser != null ? UserLogin.CurrentUser.User.Id : 0);
            }
            if (!param.Keys.Contains("ip"))
            {
                param.Add("ip", Utils.GetIP());
            }
            if (!param.Keys.Contains("version"))
            {
                param.Add("version", UserLogin.Launch.Element("Version").GetValue());
            }
            if (!param.Keys.Contains("lang"))
            {
                param.Add("lang", UserLogin.LANG);
            }
            if (!param.Keys.Contains("zone"))
            {
                param.Add("zone", GetZone());
            }
            if (!param.Keys.Contains("sign"))
            {
                param.Add("sign", Sign.CreateSign(param));
            }
            ApiReturn r = HttpHelper.Post<ApiReturn>(uri, param, null, 10, null, null, null, true);
            return r;
        }

        public static bool Download(this string url, string savepath)
        {
            bool isOK = false;
            try
            {
                if (!Directory.Exists(System.IO.Path.GetDirectoryName(savepath)))
                {
                    Directory.CreateDirectory(System.IO.Path.GetDirectoryName(savepath));
                }
                Stream stream = HttpHelper.GetStream(url, "");
                if (stream == null)  //服务器端不存在更新文件，则不下载
                    return isOK;
                using (FileStream saveStream = new FileStream(savepath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    byte[] buffer = new byte[4096];
                    int count = 0;
                    while ((count = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        saveStream.Write(buffer, 0, count);
                    }
                }
                isOK = true;
            }
            catch
            {
                isOK = false;
            }
            return isOK;
        }

        public static string HttpGet(this string url)
        {
            try
            {
                return HttpHelper.GetResponseResult(url);
            }
            catch
            {
                return "";
            }
        }
    }
}
