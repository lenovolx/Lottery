using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml.Linq;
using System.Configuration;
using FT.Utility.ApiHelper;
using FT.WinForm.Tools;
using System.Windows.Threading;
using System.Reflection;
using System.Diagnostics;
namespace FT.WinForm.Http
{

    /// <summary>
    /// 请求处理
    /// </summary>
    public class ProcessRequest
    {

        /// <summary>
        /// 获取当前菜单名称
        /// </summary>
        public static Func<string> GetMenuName;

        public static int ERR_HANDLING = 0; //0: 默认，弹出窗口； 1:不弹出窗口，记录在日志里

        /// <summary>
        /// 
        /// </summary>
        /// <param name="res"></param>
        /// <param name="processDelegate"></param>
        /// <param name="hasRoot">返回列表数据时需要做处理适配</param>
        public static void Process(ApiReturn res, Action<ApiReturn> processDelegate, bool hasRoot = true)
        {
            //null是请求失败，或者超时的，或者url不对的
            if (res != null)
            {
                if (res.code == 0)
                {
                    if (!hasRoot)
                    {
                        res.data = "{\"data\":" + res.data + "}";
                    }
                    processDelegate(res);
                }
                else if (res.code == 1)// 服务端已经捕捉的异常，有友好提示语的，直接显示
                {
                    App.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                    {
                        Msg.Info(res.message);
                    }));
                    return;
                }
                else if (res.code == 98 || res.code == 99)
                {
                    App.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                    {
                        MainWindow.isExitBtn = true;
                        MessageBox.Show(res.message);
                        Environment.Exit(0);
                    }));
                    return;
                }
                else//其他出错的情况
                {
                    App.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                    {
                        Msg.Error(res.message);
                    }));
                    return;
                }
            }
            else
            {
                App.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                {
                    Msg.Error(res.message);
                }));
                return;
            }
        }
    }
}
