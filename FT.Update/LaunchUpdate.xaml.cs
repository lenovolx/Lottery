using FT.Utility.Helper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Linq;

namespace FT.Update
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LaunchUpdate : Window
    {
        //文件路径
        private static string launchPath = System.AppDomain.CurrentDomain.BaseDirectory;

        private static XElement xeLaunch;  //启动信息
        private static string launchInfoFileName = "Launch.xml";
        private static string clientPath = ConfigurationManager.AppSettings["ClientPath"];
        private static string launchPathExt = ConfigurationManager.AppSettings["LaunchPath"];
        private static string serverUrl = ConfigurationManager.AppSettings["ServerUrl"];
        public LaunchUpdate()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(LaunchUpdate_Loaded);
        }

        void LaunchUpdate_Loaded(object sender, RoutedEventArgs e)
        {

            this.TblStatus.Text = "正在检查更新........";
            this.TblFileName.Text = "";

            this.progressBar.IsIndeterminate = true;
            Thread trdDownload = new Thread(new ThreadStart(this.LauchFileInfo));
            trdDownload.Start();
        }

        /// <summary>
        /// 从服务器获取最新版本信息，并检查更新
        /// </summary>
        private void LauchFileInfo()
        {
            string startupUri = ConfigurationManager.AppSettings["StartupUri"];
            try
            {
                string content = HttpHelper.GetResponseResult(serverUrl + "UpdateFile/Launch.xml");
                if (content.Length > 0)
                {
                    XElement xeFileInfo = XElement.Parse(content);
                    if (xeFileInfo == null)  //判断服务器端是否存在客户端程序升级信息，不存在则返回
                        return;

                    string localPath = launchPath + launchInfoFileName;  //绝对路径
                    bool isExistLaunch = File.Exists(localPath);  //启动程序，更新信息文件是否存在
                    bool isHasUpdate = false; //是否有更新
                    List<bool> isUpdateOKList = new List<bool>(); //是否更新成功
                    //客户端更新
                    string version = xeFileInfo.Element("Version").GetValue();
                    foreach (XElement xeServerFile in xeFileInfo.Element("Client").Elements())
                    {
                        //如果更新信息文件不存在，证明第一次则下载，或者不是新版本下载
                        if (!isExistLaunch || !CheckUpdate(xeServerFile, "Client"))
                        {
                            //更新前检查是否有进程存在，有就杀掉进程
                            //第一个更新时检查进程并杀掉
                            if (!isHasUpdate)
                            {
                                #region 检查客户端程序是否存在进程,存在提示是否关闭
                                string exename = System.IO.Path.GetFileNameWithoutExtension(startupUri);
                                Process[] processes = Process.GetProcessesByName(exename);
                                if (processes.Count() > 0 && MessageBox.Show(string.Format("Detection of the program is running,do you want to close it immediately and continue?", exename), "Prompt", MessageBoxButton.YesNo) == MessageBoxResult.No)
                                {
                                    ShutDown();//升级程序退出  
                                    return;
                                }
                                #endregion

                                ClientShutDown();      //杀掉所有客户端进程
                                OtherProgramShutDown();//杀掉其他程序运行的进程
                            }

                            isHasUpdate = true;  //设置有更新  
                            SetShowInfo("New version detected,Start update......", xeServerFile.Value);
                            isUpdateOKList.Add(DownloadingUpdate(xeServerFile, version));  //下载并更新
                        }
                    }

                    if (isHasUpdate)  //如果有更新，则提示更新完成
                    {
                        SetShowInfo("update completed,Starting program......", "");
                    }
                    else
                    {
                        SetShowInfo("Starting program......", "");
                    }

                    #region 保存更新信息结构到本地
                    if (isUpdateOKList.Any(x => x != false))
                    {
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.InnerXml = xeFileInfo.ToString();
                        xmlDoc.Save(localPath);
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                this.Dispatcher.BeginInvoke(new Action(delegate()
                {
                    MessageBox.Show("Update failed:", ex.Message);
                }
                ), DispatcherPriority.SystemIdle);
            }
            finally
            {
                string exename = System.IO.Path.GetFileNameWithoutExtension(startupUri);
                Process[] processes = Process.GetProcessesByName(exename);
                if (processes.Count() > 0 && MessageBox.Show(string.Format("Detection of the program is running,do you want to close it immediately and continue?", exename), "Prompt", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    ClientShutDown();      //杀掉所有客户端进程
                    OtherProgramShutDown();//杀掉其他程序运行的进程
                    if (File.Exists(launchPath + startupUri))
                    {
                        Process.Start(launchPath + startupUri);
                    }
                }
                //关闭自身
                this.ShutDown();
            }
        }

        #region 检查文件是否是新版本,是新版本返回True，不是返回False
        /// <summary>
        /// 检查文件是否是新版本,是新版本返回True，不是返回False
        /// </summary>
        /// <param name="xeFile"></param>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        private bool CheckUpdate(XElement xeServerFile, string nodeName)
        {
            try
            {
                if (xeLaunch == null)
                {
                    string localPath = launchPath + launchInfoFileName;  //绝对路径
                    if (File.Exists(localPath))
                    {
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.Load(localPath);
                        xeLaunch = XElement.Parse(xmlDoc.InnerXml);
                    }

                    //再次检查
                    if (xeLaunch == null || !File.Exists(localPath))
                        return false;
                }
                string filePath = launchPath + "/" + clientPath + "/" + xeServerFile.Attribute("Path").Value; //文件全路径
                foreach (XElement xeLocalFile in xeLaunch.Element(nodeName).Elements())
                {
                    //检查本地文件是否是新版本(找到本地文件，文件存在，并且已经是最新则不用更新)
                    if (xeLocalFile.Value == xeServerFile.Value
                        && File.Exists(filePath)
                        && DateTime.Parse(xeLocalFile.Attribute("UpdateDateTime").Value) >= DateTime.Parse(xeServerFile.Attribute("UpdateDateTime").Value))
                    {
                        return true;
                    }
                }
            }
            catch
            { }

            return false;
        }
        #endregion
        #region 下载更新
        /// <summary>
        /// 下载更新
        /// </summary>
        /// <param name="xeServerFile"></param>
        /// <returns></returns>
        private bool DownloadingUpdate(XElement xeServerFile, string version)
        {
            bool isOK = false;
            try
            {
                string filePath = launchPath + "/" + clientPath + "/" + xeServerFile.Attribute("Path").Value;
                if (!Directory.Exists(System.IO.Path.GetDirectoryName(filePath)))
                {
                    Directory.CreateDirectory(System.IO.Path.GetDirectoryName(filePath));
                }
                Stream stream = HttpHelper.GetStream(serverUrl + "UpdateFile" + "/" + version + "/" + clientPath + "/" + xeServerFile.Attribute("Path").Value, "");
                if (stream == null)  //服务器端不存在更新文件，则不下载
                    return isOK;
                using (FileStream saveStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
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
        #endregion

        #region 设置状态
        /// <summary>
        /// 设置状态
        /// </summary>
        /// <param name="status"></param>
        /// <param name="fileName"></param>
        private void SetShowInfo(string status, string fileName)
        {
            this.TblStatus.Dispatcher.BeginInvoke(new Action(delegate()
            {
                this.TblStatus.Text = status;
            }
             ), DispatcherPriority.SystemIdle);

            this.TblFileName.Dispatcher.BeginInvoke(new Action(delegate()
            {
                this.TblFileName.Text = fileName;
            }
             ), DispatcherPriority.SystemIdle);
        }
        #endregion

        #region 关闭升级程序
        /// <summary>
        /// 关闭升级程序
        /// </summary>
        private void ShutDown()
        {
            this.Dispatcher.BeginInvoke(new Action(delegate()
            {
                App.Current.Shutdown();
            }
             ), DispatcherPriority.SystemIdle);
        }
        #endregion

        #region 杀掉客户端进程
        /// <summary>
        /// 杀掉客户端进程
        /// </summary>
        private void ClientShutDown()
        {
            this.Dispatcher.BeginInvoke(new Action(delegate()
            {
                string exename = System.IO.Path.GetFileNameWithoutExtension(ConfigurationManager.AppSettings["StartupUri"]);
                Process[] processes = Process.GetProcessesByName(exename);
                //杀死进程并继续
                foreach (Process proc in processes)
                {
                    proc.Kill();
                }
            }
             ), DispatcherPriority.SystemIdle);
        }
        #endregion
        #region 杀掉其他程序进程
        /// <summary>
        /// 杀掉其他程序进程
        /// </summary>
        private void OtherProgramShutDown()
        {
            this.Dispatcher.BeginInvoke(new Action(delegate()
            {
                try
                {
                    if (ConfigurationManager.AppSettings["OtherProgramKill"] != null)
                    {
                        string[] exenames = ConfigurationManager.AppSettings["OtherProgramKill"].Split(';');

                        foreach (string exename in exenames)
                        {
                            Process[] processes = Process.GetProcessesByName(exename);
                            //杀死进程并继续
                            foreach (Process proc in processes)
                            {
                                proc.Kill();
                            }
                        }
                    }
                }
                catch { }
            }), DispatcherPriority.SystemIdle);
        }
        #endregion
    }
}
