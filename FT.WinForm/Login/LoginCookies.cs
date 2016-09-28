using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using FT.Utility.Helper;
namespace FT.WinForm.Login
{
    public class LoginCookies
    {
        private static string filePath = "ClientCookies.xml";

        public static List<XElement> GetLoginUser()
        {
            List<XElement> userLoginList = new List<XElement>();
            if (File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + filePath))
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(System.AppDomain.CurrentDomain.BaseDirectory + filePath);
                XElement xe = XElement.Parse(xmlDoc.InnerXml);
                if (xe != null && xe.Element("LoginUser") != null)
                    userLoginList = xe.Element("LoginUser").Elements().ToList();
            }
            return userLoginList;
        }


        public static XElement GetXeCookies()
        {
            if (File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + filePath))
            {
                return XElement.Load(System.AppDomain.CurrentDomain.BaseDirectory + filePath);
            }
            return new XElement("Cookies", new XElement("LoginUser"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loginName"></param>
        /// <param name="password"></param>
        /// <param name="isRememberPwd"></param>
        public static void WriteLoginUser(string loginName, string password, bool isRememberPwd)
        {
            XElement xeCookies = GetXeCookies();
            XElement xeLoginUser = xeCookies.Element("LoginUser");
            XElement xeCurrentUser = xeLoginUser.Elements().SingleOrDefault(x => x.Element("LoginName").GetValue() == loginName);
            if (xeCurrentUser == null)
            {
                xeCurrentUser = new XElement("User", new XElement("LoginName", loginName), new XElement("Password", isRememberPwd ? password : "")
                    , new XElement("LastLoginDate", DateTime.Now), new XElement("IsRememberPwd", isRememberPwd));
                xeLoginUser.Add(xeCurrentUser);
            }
            else
            {
                xeCurrentUser.SetElementValue("LastLoginDate", DateTime.Now);
                xeCurrentUser.SetElementValue("Password", isRememberPwd ? password : "");
                xeCurrentUser.SetElementValue("IsRememberPwd", isRememberPwd);
            }

            //刷新排序
            XElement[] xeUserArray = xeLoginUser.Elements().OrderByDescending(x => x.Element("LastLoginDate").GetValue<DateTime>(DateTime.MinValue)).ToArray();
            xeLoginUser.Elements().Remove();
            xeLoginUser.Add(xeUserArray);
            
            xeCookies.Save(System.AppDomain.CurrentDomain.BaseDirectory + filePath);
        }

        /// <summary>
        /// 删除Cokies
        /// </summary>
        public static void DeleteClientCookies()
        {
            try
            {
                if (File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + filePath))
                {
                    File.Delete(System.AppDomain.CurrentDomain.BaseDirectory + filePath);
                }
            }
            catch { }
        }
    }
}
