namespace FT.Utility.Helper
{
    using System;
    using System.Collections.Specialized;
    using System.IO;
    using System.IO.Compression;
    using System.Net;
    using System.Net.Cache;
    using System.Security;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;

    public class WebHelper
    {
        private static string[] _browserlist = new string[] { "ie", "chrome", "mozilla", "netscape", "firefox", "opera", "konqueror" };
        private static Regex _metaregex = new Regex("<meta([^<]*)charset=([^<]*)[\"']", RegexOptions.Multiline | RegexOptions.IgnoreCase);
        private static string[] _searchenginelist = new string[] { "baidu", "google", "360", "sogou", "bing", "msn", "sohu", "soso", "sina", "163", "yahoo", "jikeu" };

        public static void DeleteCookie(string name)
        {
            HttpCookie cookie = new HttpCookie(name)
            {
                Expires = DateTime.Now.AddYears(-1)
            };
            HttpContext.Current.Response.AppendCookie(cookie);
        }

        public static string GetBrowserName()
        {
            string browser = HttpContext.Current.Request.Browser.Browser;
            if (string.IsNullOrEmpty(browser))
            {
                return "未知";
            }
            return browser.ToLower();
        }

        public static string GetBrowserType()
        {
            string type = HttpContext.Current.Request.Browser.Type;
            if (string.IsNullOrEmpty(type))
            {
                return "未知";
            }
            return type.ToLower();
        }

        public static string GetBrowserVersion()
        {
            string version = HttpContext.Current.Request.Browser.Version;
            if (string.IsNullOrEmpty(version))
            {
                return "未知";
            }
            return version;
        }

        public static string GetCookie(string name)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[name];
            if (cookie != null)
            {
                return cookie.Value;
            }
            return string.Empty;
        }

        public static string GetCookie(string name, string key)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[name];
            if ((cookie != null) && cookie.HasKeys)
            {
                string str = cookie[key];
                if (str != null)
                {
                    return str;
                }
            }
            return string.Empty;
        }

        public static int GetFormInt(string key)
        {
            return GetFormInt(key, 0);
        }

        public static int GetFormInt(string key, int defaultValue)
        {
            return TypeHelper.StringToInt(HttpContext.Current.Request.Form[key], defaultValue);
        }

        public static string GetFormString(string key)
        {
            return GetFormString(key, "");
        }

        public static string GetFormString(string key, string defaultValue)
        {
            string str = HttpContext.Current.Request.Form[key];
            if (!string.IsNullOrWhiteSpace(str))
            {
                return str;
            }
            return defaultValue;
        }

        public static string GetHost()
        {
            return HttpContext.Current.Request.Url.Host;
        }

        public static string GetIP()
        {
            string str = string.Empty;
            if (HttpContext.Current.Request.ServerVariables["HTTP_VIA"] != null)
            {
                str = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
            }
            else
            {
                str = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString();
            }
            if (!(!string.IsNullOrEmpty(str) && ValidateHelper.IsIP(str)))
            {
                str = "127.0.0.1";
            }
            return str;
        }

        public static string GetOSName()
        {
            string platform = HttpContext.Current.Request.Browser.Platform;
            if (string.IsNullOrEmpty(platform))
            {
                return "未知";
            }
            return platform;
        }

        public static string GetOSType()
        {
            string str = "未知";
            string userAgent = HttpContext.Current.Request.UserAgent;
            if (userAgent.Contains("NT 6.1"))
            {
                return "Windows 7";
            }
            if (userAgent.Contains("NT 5.1"))
            {
                return "Windows XP";
            }
            if (userAgent.Contains("NT 6.2"))
            {
                return "Windows 8";
            }
            if (userAgent.Contains("android"))
            {
                return "Android";
            }
            if (userAgent.Contains("iphone"))
            {
                return "IPhone";
            }
            if (userAgent.Contains("Mac"))
            {
                return "Mac";
            }
            if (userAgent.Contains("NT 6.0"))
            {
                return "Windows Vista";
            }
            if (userAgent.Contains("NT 5.2"))
            {
                return "Windows 2003";
            }
            if (userAgent.Contains("NT 5.0"))
            {
                return "Windows 2000";
            }
            if (userAgent.Contains("98"))
            {
                return "Windows 98";
            }
            if (userAgent.Contains("95"))
            {
                return "Windows 95";
            }
            if (userAgent.Contains("Me"))
            {
                return "Windows Me";
            }
            if (userAgent.Contains("NT 4"))
            {
                return "Windows NT4";
            }
            if (userAgent.Contains("Unix"))
            {
                return "UNIX";
            }
            if (userAgent.Contains("Linux"))
            {
                return "Linux";
            }
            if (userAgent.Contains("SunOS"))
            {
                str = "SunOS";
            }
            return str;
        }

        public static NameValueCollection GetParmList(string data)
        {
            NameValueCollection values = new NameValueCollection(StringComparer.OrdinalIgnoreCase);
            if (!string.IsNullOrEmpty(data))
            {
                int length = data.Length;
                for (int i = 0; i < length; i++)
                {
                    string str;
                    string str2;
                    int startIndex = i;
                    int num4 = -1;
                    while (i < length)
                    {
                        char ch = data[i];
                        if (ch == '=')
                        {
                            if (num4 < 0)
                            {
                                num4 = i;
                            }
                        }
                        else if (ch == '&')
                        {
                            break;
                        }
                        i++;
                    }
                    if (num4 >= 0)
                    {
                        str = data.Substring(startIndex, num4 - startIndex);
                        str2 = data.Substring(num4 + 1, (i - num4) - 1);
                    }
                    else
                    {
                        str = data.Substring(startIndex, i - startIndex);
                        str2 = string.Empty;
                    }
                    values[str] = str2;
                    if ((i == (length - 1)) && (data[i] == '&'))
                    {
                        values[str] = string.Empty;
                    }
                }
            }
            return values;
        }

        public static int GetQueryInt(string key)
        {
            return GetQueryInt(key, 0);
        }

        public static int GetQueryInt(string key, int defaultValue)
        {
            return TypeHelper.StringToInt(HttpContext.Current.Request.QueryString[key], defaultValue);
        }

        public static string GetQueryString(string key)
        {
            return GetQueryString(key, "");
        }

        public static string GetQueryString(string key, string defaultValue)
        {
            string str = HttpContext.Current.Request.QueryString[key];
            if (!string.IsNullOrWhiteSpace(str))
            {
                return str;
            }
            return defaultValue;
        }

        public static string GetRawUrl()
        {
            return HttpContext.Current.Request.RawUrl;
        }

        public static string GetRequestData(string url, string postData)
        {
            return GetRequestData(url, "post", postData);
        }

        public static string GetRequestData(string url, string method, string postData)
        {
            return GetRequestData(url, method, postData, Encoding.UTF8);
        }

        public static string GetRequestData(string url, string method, string postData, Encoding encoding)
        {
            return GetRequestData(url, method, postData, encoding, 0x4e20);
        }

        public static string GetRequestData(string url, string method, string postData, Encoding encoding, int timeout)
        {
            string str3;
            if (!(url.Contains("http://") || url.Contains("https://")))
            {
                url = "http://" + url;
            }
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = method.Trim().ToLower();
            request.Timeout = timeout;
            request.AllowAutoRedirect = true;
            request.ContentType = "text/html";
            request.Accept = "text/html, application/xhtml+xml, */*,zh-CN";
            request.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)";
            request.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
            try
            {
                if (!(string.IsNullOrEmpty(postData) || !(request.Method == "post")))
                {
                    byte[] bytes = encoding.GetBytes(postData);
                    request.ContentLength = bytes.Length;
                    request.GetRequestStream().Write(bytes, 0, bytes.Length);
                }
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    StreamReader reader2;
                    if (encoding == null)
                    {
                        MemoryStream destination = new MemoryStream();
                        if ((response.ContentEncoding != null) && response.ContentEncoding.Equals("gzip", StringComparison.InvariantCultureIgnoreCase))
                        {
                            new GZipStream(response.GetResponseStream(), CompressionMode.Decompress).CopyTo(destination, 0x2800);
                        }
                        else
                        {
                            response.GetResponseStream().CopyTo(destination, 0x2800);
                        }
                        byte[] buffer2 = destination.ToArray();
                        string input = Encoding.Default.GetString(buffer2, 0, buffer2.Length);
                        System.Text.RegularExpressions.Match match = _metaregex.Match(input);
                        string str2 = (match.Groups.Count > 2) ? match.Groups[2].Value : string.Empty;
                        str2 = str2.Replace("\"", string.Empty).Replace("'", string.Empty).Replace(";", string.Empty);
                        if (str2.Length > 0)
                        {
                            encoding = Encoding.GetEncoding(str2.ToLower().Replace("iso-8859-1", "gbk"));
                        }
                        else if (response.CharacterSet.ToLower().Trim() == "iso-8859-1")
                        {
                            encoding = Encoding.GetEncoding("gbk");
                        }
                        else if (string.IsNullOrEmpty(response.CharacterSet.Trim()))
                        {
                            encoding = Encoding.UTF8;
                        }
                        else
                        {
                            encoding = Encoding.GetEncoding(response.CharacterSet);
                        }
                        return encoding.GetString(buffer2);
                    }
                    StreamReader reader = null;
                    if ((response.ContentEncoding != null) && response.ContentEncoding.Equals("gzip", StringComparison.InvariantCultureIgnoreCase))
                    {
                        using (reader2 = reader = new StreamReader(new GZipStream(response.GetResponseStream(), CompressionMode.Decompress), encoding))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                    reader2 = reader = new StreamReader(response.GetResponseStream(), encoding);
                    try
                    {
                        return reader.ReadToEnd();
                    }
                    catch
                    {
                        return "close";
                    }
                    finally
                    {
                        if (reader2 != null)
                        {
                            reader2.Dispose();
                        }
                    }
                }
            }
            catch
            {
                str3 = "error";
            }
            return str3;
        }

        public static AspNetHostingPermissionLevel GetTrustLevel()
        {
            AspNetHostingPermissionLevel none = AspNetHostingPermissionLevel.None;
            AspNetHostingPermissionLevel[] levelArray = new AspNetHostingPermissionLevel[] { AspNetHostingPermissionLevel.Unrestricted, AspNetHostingPermissionLevel.High, AspNetHostingPermissionLevel.Medium, AspNetHostingPermissionLevel.Low, AspNetHostingPermissionLevel.Minimal };
            foreach (AspNetHostingPermissionLevel level2 in levelArray)
            {
                try
                {
                    new AspNetHostingPermission(level2).Demand();
                    return level2;
                }
                catch (SecurityException)
                {
                }
            }
            return none;
        }

        public static string GetUrl()
        {
            return HttpContext.Current.Request.Url.ToString();
        }

        public static string GetUrlReferrer()
        {
            Uri urlReferrer = HttpContext.Current.Request.UrlReferrer;
            if (urlReferrer == null)
            {
                return string.Empty;
            }
            return urlReferrer.ToString();
        }

        public static string HtmlDecode(string s)
        {
            return HttpUtility.HtmlDecode(s);
        }

        public static string HtmlEncode(string s)
        {
            return HttpUtility.HtmlEncode(s);
        }

        public static bool IsAjax()
        {
            return (HttpContext.Current.Request.Headers["X-Requested-With"] == "XMLHttpRequest");
        }

        public static bool IsBrowser()
        {
            string browserName = GetBrowserName();
            foreach (string str2 in _browserlist)
            {
                if (browserName.Contains(str2))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsCrawler()
        {
            bool crawler = HttpContext.Current.Request.Browser.Crawler;
            if (!crawler)
            {
                string urlReferrer = GetUrlReferrer();
                if (urlReferrer.Length > 0)
                {
                    foreach (string str2 in _searchenginelist)
                    {
                        if (urlReferrer.Contains(str2))
                        {
                            return true;
                        }
                    }
                }
            }
            return crawler;
        }

        public static bool IsGet()
        {
            return (HttpContext.Current.Request.HttpMethod == "GET");
        }

        public static bool IsMobile()
        {
            if (HttpContext.Current.Request.Browser.IsMobileDevice)
            {
                return true;
            }
            bool result = false;
            return (bool.TryParse(HttpContext.Current.Request.Browser["IsTablet"], out result) && result);
        }

        public static bool IsPost()
        {
            return (HttpContext.Current.Request.HttpMethod == "POST");
        }

        public static void SetCookie(string name, string value)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[name];
            if (cookie != null)
            {
                cookie.Value = value;
            }
            else
            {
                cookie = new HttpCookie(name, value);
            }
            HttpContext.Current.Response.AppendCookie(cookie);
        }

        public static void SetCookie(string name, string value, DateTime dt)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[name];
            if (cookie == null)
            {
                cookie = new HttpCookie(name);
            }
            cookie.Value = value;
            cookie.Expires = dt;
            HttpContext.Current.Response.AppendCookie(cookie);
        }

        public static void SetCookie(string name, string key, string value)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[name];
            if (cookie == null)
            {
                cookie = new HttpCookie(name);
            }
            cookie[key] = value;
            HttpContext.Current.Response.AppendCookie(cookie);
        }

        public static void SetCookie(string name, string key, string value, DateTime dt)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[name];
            if (cookie == null)
            {
                cookie = new HttpCookie(name);
            }
            cookie[key] = value;
            cookie.Expires = dt;
            HttpContext.Current.Response.AppendCookie(cookie);
        }

        public static string UrlDecode(string s)
        {
            return HttpUtility.UrlDecode(s);
        }

        public static string UrlEncode(string s)
        {
            return HttpUtility.UrlEncode(s);
        }
    }
}
