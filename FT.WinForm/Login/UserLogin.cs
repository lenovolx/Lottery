using System;
using System.IO;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System.Xml;
using System.Windows;
using FT.Utility.Helper;
using FT.Utility.ApiHelper;
using FT.WinForm.Http;
using FT.Model;
using Newtonsoft.Json;
using FT.WinForm.Tools;
namespace FT.WinForm.Login
{
    public class UserLogin
    {
        #region 当前登录用户
        /// <summary>
        /// 当前登录用户
        /// </summary>
        public static UserResponse CurrentUser { get; internal set; }
        #endregion

        #region 登录
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="loginName"></param>
        /// <param name="password"></param>
        /// <param name="isRememberPwd"></param>
        /// <returns></returns>
        public static bool Login(string loginName, string password, bool isRememberPwd)
        {
            bool isOK = false;
            try
            {
                string version = Launch.Element("Version").GetValue(); //系统版本
                HttpParam param = new HttpParam();
                param.Add("username", loginName);
                password = password.StartsWith("ftpassword") ? password.Substring(10, password.Length - 10) : SecureHelper.Md5(password);
                param.Add("password", password);

                ProcessRequest.Process(string.Format("User/Login").HttpPost(param), (ApiReturn res) =>
                {
                    CurrentUser = JsonConvert.DeserializeObject<UserResponse>(res.data + "");
                    //记住用户名
                    //isRememberPwd = false;  //不记住密码
                    LoginCookies.WriteLoginUser(loginName, "ftpassword" + password, isRememberPwd);
                    isOK = true;
                });
            }
            catch (Exception ex)
            {
                Msg.Error(ex.Message);
            }
            return isOK;
        }
        #endregion
        #region 刷新用户余额
        public static void GetBalance()
        {
            try
            {
                string version = Launch.Element("Version").GetValue(); //系统版本
                HttpParam param = new HttpParam();
                ProcessRequest.Process(string.Format("User/Center").HttpPost(param), (ApiReturn res) =>
                {
                    var _user = JsonConvert.DeserializeObject<dynamic>(res.data + "");
                    CurrentUser.User.BalanceAmount = _user.BalanceAmount;
                });
            }
            catch (Exception ex)
            {
                Msg.Error(ex.Message);
            }
        }
        #endregion
        #region 系统版本标识
        /// <summary>
        /// 系统版本标识
        /// </summary>
        public static string EditionFlag
        {
            get
            {
                return "1";
            }
        }
        #endregion

        #region 获得系统文件及版本信息
        private static XElement _launch = null;
        /// <summary>
        /// 系统文件及版本信息
        /// </summary>
        public static XElement Launch
        {
            get
            {
                if (_launch == null)
                {
                    _launch = GetSysVersion();
                }
                return _launch;
            }
            set
            {
                _launch = value;
            }
        }
        /// <summary>
        /// 当前语言
        /// </summary>
        public static string LANG = "cn";
        /// <summary>
        /// 获得系统文件及版本信息
        /// </summary>
        /// <returns></returns>
        private static XElement GetSysVersion()
        {
            XElement xeLaunch = null;
            try
            {
                string launchPath = System.AppDomain.CurrentDomain.BaseDirectory.Remove(System.AppDomain.CurrentDomain.BaseDirectory.Length - 7);

                string launchInfoFileName = "Launch.xml";
                string localPath = launchPath + launchInfoFileName;  //绝对路径

                if (File.Exists(localPath))
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(localPath);
                    xeLaunch = XElement.Parse(xmlDoc.InnerXml);
                }
            }
            finally
            {
                if (xeLaunch == null)
                {
                    xeLaunch = new XElement("Launch");
                }
            }

            return xeLaunch;
        }
        #endregion

    }
}
