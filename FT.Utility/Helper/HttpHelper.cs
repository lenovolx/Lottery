namespace FT.Utility.Helper
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Net;
    using System.Text;

    public class HttpHelper
    {
        private const string ContentType = "application/x-www-form-urlencoded";
        private static readonly Encoding DefaultEncoding = Encoding.UTF8;
        private const int TimeOut = 5;
        private static readonly string UserAgent = "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.1 (KHTML, like Gecko) Maxthon/4.1.2.4000 Chrome/26.0.1410.43 Safari/537.1";

        public static HttpWebRequest CreateRequest(string url)
        {
            return (WebRequest.Create(url) as HttpWebRequest);
        }

        private static string FormatData(IEnumerable<KeyValuePair<string, object>> data)
        {
            return new HttpParam(data).Format();
        }

        public static string Get(string url)
        {
            return Get(url, "", 5, null, null, null);
        }

        public static T Get<T>(string url)
        {
            return Get<T>(url, "", 5, null, null, null);
        }

        public static string Get(string url, HttpParam param = null, int timeOut = 5, Encoding encoding = null, CookieContainer cc = null, string refer = null)
        {
            return Get(url, FormatData(param), timeOut, encoding, cc, refer);
        }

        public static T Get<T>(string url, HttpParam param = null, int timeOut = 5, Encoding encoding = null, CookieContainer cc = null, string refer = null)
        {
            return JsonConvert.DeserializeObject<T>(Get(url, param, timeOut, encoding, cc, refer));
        }

        public static string Get(string url, string queryString = null, int timeOut = 5, Encoding encoding = null, CookieContainer cc = null, string refer = null)
        {
            return new StreamReader(GetStream(url, queryString, timeOut, encoding, cc, refer)).ReadToEnd();
        }

        public static T Get<T>(string url, string queryString = null, int timeOut = 5, Encoding encoding = null, CookieContainer cc = null, string refer = null)
        {
            return JsonConvert.DeserializeObject<T>(Get(url, queryString, timeOut, encoding, cc, refer));
        }

        public static Stream GetStream(string url, HttpParam param = null, int timeOut = 5, Encoding encoding = null, CookieContainer cc = null, string refer = null)
        {
            return GetStream(url, FormatData(param), timeOut, encoding, cc, refer);
        }

        public static T SimpleGet<T>(string url,ref string res) where T : new()
        {
            var request = CreateRequest(url);
            request.Method = "GET";
            var stream = request.GetResponse().GetResponseStream();
            if (stream != null)
            {
                var response = new StreamReader(stream).ReadToEnd();
                res = response;
                return JsonConvert.DeserializeObject<T>(response);
            }
            else return new T();
        }


        public static T SimplePost<T>(string url, string param, ref string res) where T : new()
        {
            var request = CreateRequest(url);
            request.Method = "POST";
            request.ContentType = "application/json";
            var data = Encoding.GetEncoding("utf-8").GetBytes(param);
            request.ContentLength = data.Length;
            var stream = request.GetRequestStream();
            stream.Write(data, 0, data.Length);
            stream.Close();
            try
            {
                stream = request.GetResponse().GetResponseStream();
            }
            catch (Exception exception)
            {
                throw exception;
            }
            using (StreamReader sr = new StreamReader(stream, Encoding.GetEncoding("utf-8")))
            {
                var response = sr.ReadToEnd();
                sr.Close();
                res = response;
                return JsonConvert.DeserializeObject<T>(response);
            }
        }

        public static Stream GetStream(string url, string queryString = null, int timeOut = 5, Encoding encoding = null, CookieContainer cc = null, string refer = null)
        {
            url = string.Format("{0}?{1}", url, queryString);
            HttpWebRequest request = CreateRequest(url);
            request.Timeout = timeOut * 0x3e8;
            request.UserAgent = UserAgent;
            request.Referer = refer;
            request.CookieContainer = cc;
            request.Method = "GET";
            return request.GetResponse().GetResponseStream();
        }

        public static HttpStatusCode HeadHttpCode(string url, string data = null, int timeOut = 5, Encoding encoding = null, CookieContainer cc = null, string refer = null)
        {
            try
            {
                HttpWebRequest request = CreateRequest(url);
                request.Timeout = timeOut;
                request.UserAgent = UserAgent;
                request.Method = "HEAD";
                request.Referer = refer;
                request.CookieContainer = cc;
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                if (response != null)
                {
                    return response.StatusCode;
                }
                return HttpStatusCode.ExpectationFailed;
            }
            catch
            {
                return HttpStatusCode.ExpectationFailed;
            }
        }

        public static string Post(string url)
        {
            return Post(url, "", null, 5, null, null, null);
        }

        public static T Post<T>(string url)
        {
            return Post<T>(url, "", null, 5, null, null, null);
        }

        public static string Post(string url, HttpParam param = null, HttpParam getParam = null, int timeOut = 5, Encoding encoding = null, CookieContainer cc = null, string refer = null)
        {
            return Post(string.Format("{0}{1}", url, (getParam == null) ? "" : string.Format("?{0}", getParam.Format())), param.ToJson(), null, timeOut, encoding, cc, refer);
        }

        public static T Post<T>(string url, HttpParam param = null, HttpParam getParam = null, int timeOut = 5, Encoding encoding = null, CookieContainer cc = null, string refer = null)
        {
            return JsonConvert.DeserializeObject<T>(Post(url, param, getParam, timeOut, encoding, cc, refer));
        }

        public static string Post(string url, string param = null, string getParam = null, int timeOut = 5, Encoding encoding = null, CookieContainer cc = null, string refer = null)
        {
            return new StreamReader(PostStream(string.Format("{0}{1}", url, (getParam == null) ? "" : string.Format("?{0}", getParam)), param, timeOut, encoding, cc, refer), encoding ?? DefaultEncoding).ReadToEnd();
        }

        public static T Post<T>(string url, string param = null, string getParam = null, int timeOut = 5, Encoding encoding = null, CookieContainer cc = null, string refer = null)
        {
            return JsonConvert.DeserializeObject<T>(Post(url, param, getParam, timeOut, encoding, cc, refer));
        }
        public static T Post<T>(string url, HttpParam param = null, string getParam = null, int timeOut = 5, Encoding encoding = null, CookieContainer cc = null, string refer = null, bool json = true)
        {
            return JsonConvert.DeserializeObject<T>(new StreamReader(PostJson(url, param.ToJson(), timeOut, encoding, cc, refer), encoding ?? DefaultEncoding).ReadToEnd());
        }
        public static Stream PostStream(string url, string param = null, int timeOut = 5, Encoding encoding = null, CookieContainer cc = null, string refer = null)
        {
            HttpWebRequest request = CreateRequest(url);
            request.Timeout = timeOut * 0x3e8;
            request.UserAgent = UserAgent;
            request.Method = "POST";
            request.Referer = refer;
            request.CookieContainer = cc;
            request.ContentType = "application/x-www-form-urlencoded";
            if (param != null)
            {
                byte[] bytes = (encoding ?? DefaultEncoding).GetBytes(param);
                request.ContentLength = bytes.Length;
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Flush();
                requestStream.Close();
            }
            return request.GetResponse().GetResponseStream();
        }

        public static Stream PostJson(string url, string param = null, int timeOut = 5, Encoding encoding = null, CookieContainer cc = null, string refer = null)
        {
            HttpWebRequest request = CreateRequest(url);
            request.Timeout = timeOut * 0x3e8;
            request.UserAgent = UserAgent;
            request.Method = "POST";
            request.Referer = refer;
            request.CookieContainer = cc;
            request.ContentType = "application/json";
            if (param != null)
            {
                byte[] bytes = (encoding ?? DefaultEncoding).GetBytes(param);
                request.ContentLength = bytes.Length;
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Flush();
                requestStream.Close();
            }
            return request.GetResponse().GetResponseStream();
        }

        public static string Upload(string url, HttpParam formData, string filePath)
        {
            string address = string.Format("{0}?{1}", url, (formData == null) ? "" : formData.Format());
            byte[] bytes = new WebClient().UploadFile(address, "POST", filePath);
            return Encoding.UTF8.GetString(bytes);
        }

        public static string Post_GB2312(string url, string strPost, string contentType)
        {
            string result = "";
            Stream responseStream;
            HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(url);
            objRequest.Method = "POST";
            objRequest.KeepAlive = false;
            objRequest.Accept = "zh-CN,zh;q=0.8";
            objRequest.ContentType = contentType;// "text/xml";//提交xml "application/x-www-form-urlencoded";//提交表单
            byte[] data = Encoding.GetEncoding("gb2312").GetBytes(strPost);
            objRequest.ContentLength = data.Length;
            Stream requestStream = objRequest.GetRequestStream();
            requestStream.Write(data, 0, data.Length);
            requestStream.Close();
            try
            {
                responseStream = objRequest.GetResponse().GetResponseStream();
            }
            catch (Exception exception)
            {
                throw exception;
            }
            using (StreamReader sr = new StreamReader(responseStream, Encoding.GetEncoding("gb2312")))
            {
                result = sr.ReadToEnd();
                sr.Close();
            }

            return result;
        }
        public static string GetResponseResult(string url)
        {
            string result;
            WebRequest req = WebRequest.Create(url);
            using (HttpWebResponse response = (HttpWebResponse)req.GetResponse())
            {
                using (Stream receiveStream = response.GetResponseStream())
                {
                    using (StreamReader readerOfStream = new StreamReader(receiveStream, System.Text.Encoding.UTF8))
                    {
                        result = readerOfStream.ReadToEnd();
                    }
                }
            }
            return result;
        }
    }
 
    public class HTMLHelper
    {
        private static CookieContainer cc;
        public static string OldGetData(string strURL)
        {
            string text1 = "";
            try
            {
                HttpWebRequest HWR = (HttpWebRequest)WebRequest.Create(strURL);
                HWR.UserAgent = "User-Agent: Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/45.0.2414.0 Safari/537.36";
                HWR.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
                HWR.KeepAlive = true;
                HWR.Headers.Add("Accept-Language: zh-CN,zh;q=0.8");
                HWR.Headers.Add("Accept-Encoding: gzip, deflate, sdch");
             
                HWR.CookieContainer = new CookieContainer();
                HttpWebResponse HWResp = (HttpWebResponse)HWR.GetResponse();
                cc = new CookieContainer();
                cc.Add(HWResp.Cookies);
                string strcrook = HWR.CookieContainer.GetCookieHeader(HWR.RequestUri);

                Stream stream1 = HWResp.GetResponseStream();
                stream1 = new GZipStream(stream1, CompressionMode.Decompress);
                StreamReader reader1 = new StreamReader(stream1, Encoding.GetEncoding("utf-8"));
                text1 = reader1.ReadToEnd();
                reader1.Close();
                stream1.Close();
            }
            catch
            {
                text1 = "";
            }

            return text1;
        }

        public static string NewGetData(string strURL)
        {
            string text1 = "";
            string year = DateTime.Now.ToString("yyyy-MM-dd");
            long time = DateTimeHelper.DateTimeToUnixTimestamp(DateTime.Now, 0);//时间戳（现在时间不加4）

            try
            {
                HttpWebRequest HWR = (HttpWebRequest)WebRequest.Create(strURL);
                HWR.UserAgent = "User-Agent: Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/45.0.2414.0 Safari/537.36";
                HWR.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
                HWR.KeepAlive = true;
                HWR.Headers.Add("Accept-Language: zh-CN,zh;q=0.8");
                HWR.Headers.Add("Accept-Encoding: gzip, deflate, sdch");
                HWR.Headers.Add("Cookie:protocolstr=http; _gat_UA-75448111-1=1; _ga=GA1.4.420834271."+time+"; gamePoint_15712982="+year+"%2A0%2A0");

                HttpWebResponse HWResp = (HttpWebResponse)HWR.GetResponse();
 
                Stream stream1 = HWResp.GetResponseStream();
                stream1 = new GZipStream(stream1, CompressionMode.Decompress);
                StreamReader reader1 = new StreamReader(stream1, Encoding.GetEncoding("utf-8"));
                text1 = reader1.ReadToEnd();
                reader1.Close();
                stream1.Close();
            }
            catch
            {
                text1 = "";
            }

            return text1;
        }
 
        /// <summary>
        /// 将json字符串装载到实体
        /// </summary>
        /// <typeparam name="T">实体</typeparam>
        /// <param name="str">json</param>
        /// <returns></returns>
        public static T ToModel<T>(string str)
        {
            return JsonConvert.DeserializeObject<T>(str);
        }
        /// <summary>
        /// GET请求获取网页数据 无GZIP压缩
        /// </summary>
        /// <param name="strUrl"></param>
        /// <returns></returns>
        public static string GetResponse(string strUrl)
        {
            string strResult = string.Empty;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strUrl);
                request.UserAgent =
                    "User-Agent: Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/45.0.2414.0 Safari/537.36";
                request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
                request.KeepAlive = true;
                request.Timeout = 5 * 0x3e8;
                request.Headers.Add("Accept-Language: zh-CN,zh;q=0.8");
                request.Headers.Add("Accept-Encoding: gzip, deflate, sdch");

                request.CookieContainer = new CookieContainer();
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                cc = new CookieContainer();
                cc.Add(response.Cookies);
                string strcrook = request.CookieContainer.GetCookieHeader(request.RequestUri);

                Stream stream = response.GetResponseStream();
                //stream = new GZipStream(stream, CompressionMode.Decompress);
                StreamReader reader = new StreamReader(stream, Encoding.GetEncoding("utf-8"));
                strResult = reader.ReadToEnd();
                reader.Close();
                stream.Close();
            }
            catch (Exception ex)
            {
                throw;
            }
            return strResult;
        }
    }
}