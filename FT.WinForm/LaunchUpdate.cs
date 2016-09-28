using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Threading;
using System.Xml.Linq;
using System.IO;
using System.Xml;

namespace FT.WinForm
{
    public class LaunchUpdate
    {
        //文件路径
        private static string launchPath = System.AppDomain.CurrentDomain.BaseDirectory + "..//";
        private static string launchUpdateInfoFileName = "LaunchUpdate.xml";

        private static XElement xeLaunchUpdate;  //升级程序更新信息

        /// <summary>
        /// 从服务器获取最新版本信息，并检查更新
        /// </summary>
        public static void LauchUpdateFileInfo()
        {
            try
            {
                //如果升级程序不存在则返回
                if (!File.Exists(launchPath + "FT.Update.exe"))
                    return;

                //using (HttpResponseMessage res = string.Format("Launch/GetLaunchUpdateInfo").HttpGet())
                //{
                //    XElement xeFinleInfo = res.Content.ReadAsXElement();

                //    if (xeFinleInfo == null)  //判断服务器端是否存在升级程序升级信息，不存在则返回
                //        return;

                //    string localPath = launchPath + launchUpdateInfoFileName;  //绝对路径

                //    bool isExistLaunch = File.Exists(localPath);  //启动程序，更新信息文件是否存在

                //    //升级程序更新
                //    foreach (XElement xeServerFile in xeFinleInfo.Elements())
                //    {
                //        //如果更新信息文件不存在，证明第一次则下载，或者不是新版本下载
                //        if (!isExistLaunch || !CheckUpdate(xeServerFile))
                //        {
                //            LaunchShutDown();                //杀掉所有升级程序进程
                //            //DownloadingUpdate(xeServerFile); //下载并更新
                //        }
                //    }

                //    #region 保存更新信息结构到本地
                //    XmlDocument xmlDoc = new XmlDocument();
                //    xmlDoc.InnerXml = xeFinleInfo.ToString();
                //    xmlDoc.Save(localPath);
                //    #endregion
                //}
            }
            catch
            {

            }
        }

        /// <summary>
        /// 检查文件是否是新版本,是新版本返回True，不是返回False
        /// </summary>
        /// <param name="xeFile"></param>
        /// <returns></returns>
        private static bool CheckUpdate(XElement xeServerFile)
        {
            try
            {
                if (xeLaunchUpdate == null)
                {
                    string localPath = launchPath + launchUpdateInfoFileName;  //绝对路径
                    if (File.Exists(localPath))
                    {
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.Load(localPath);
                        xeLaunchUpdate = XElement.Parse(xmlDoc.InnerXml);
                    }

                    //再次检查
                    if (xeLaunchUpdate == null || !File.Exists(localPath))
                        return false;
                }
                string filePath = launchPath + xeServerFile.Value; //文件全路径
                foreach (XElement xeLocalFile in xeLaunchUpdate.Elements())
                {
                    //检查本地文件是否是新版本
                    if (xeLocalFile.Value == xeServerFile.Value && File.Exists(filePath)
                        && DateTime.Parse(xeLocalFile.Attribute("UpdateDateTime").Value) == DateTime.Parse(xeServerFile.Attribute("UpdateDateTime").Value))
                    {
                        return true;
                    }
                }
            }
            catch
            { }

            return false;
        }
        /// <summary>
        /// 杀掉客户端进程
        /// </summary>
        private static void LaunchShutDown()
        {
            string exename = System.IO.Path.GetFileNameWithoutExtension("FT.Update.exe");
            Process[] processes = Process.GetProcessesByName(exename);
            //杀死进程并继续
            foreach (Process proc in processes)
            {
                proc.Kill();
            }
        }
    }
}
