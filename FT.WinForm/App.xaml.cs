using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace FT.WinForm
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public App()
        {

        }
        #region 捕获未经处理的异常

        /// <summary>
        /// 绑定异常捕获事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            //非窗体线程异常
            AppDomain.CurrentDomain.UnhandledException +=
                         new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            //窗体线程异常
            this.DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(Application_DispatcherUnhandledException);

        }
        /// <summary>
        /// 异常处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            try
            {
                //MessageBox.Show("程序发生非窗体线程异常，正在尝试继续运行！详细异常信息：\r\n" + ex.Message, "程序异常",
                //                MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch
            {
                MessageBox.Show("程序发生异常，应用程序将退出！详细异常信息：\r\n" + ex.Message, "程序异常",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                this.Shutdown(); //程序已崩溃，此操作基本上已经无效了
            }
        }

        /// <summary>
        /// 捕获未经处理的异常(主用户界面(UI)线程)
        /// 特别说明：DispatcherUnhandledException由运行在主UI线程的代码未处理异常引发.
        /// 如果在主用户界面(UI)线程 (有自己的 Dispatcher的线程) 或用户界面内的辅助线程 (不是Dispatcher的线程未处理)，则不被转发到主 UI 线程,不会引发DispatcherUnhandledException 。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            //当从 COM 方法调用返回无法识别的 HRESULT 时引发的异常
            //var comException = e.Exception as System.Runtime.InteropServices.COMException;
            //if (comException != null && comException.ErrorCode == -2147221040)
            //{
            //    //针对剪切板异常，捕获"OpenClipboard 失败 (异常来自 HRESULT:0x800401D0 (CLIPBRD_E_CANT_OPEN))"
            //    //设置 Handled为 true,跳过异常。
            //    e.Handled = true;
            //    MessageBox.Show("剪切板异常,请稍后再试，或尝试用鼠标右键进行复制！");
            //}
            //else
            //{
            //    try
            //    {
            //        Exception ex = e.Exception;
            //        if (ex.GetType() != typeof(ResourceReferenceKeyNotFoundException))
            //            MessageBox.Show("程序发生窗体线程异常，正在尝试继续运行！详细异常信息：\r\n" + ex.Message, "Error",
            //                            MessageBoxButton.OK, MessageBoxImage.Error);
            //        e.Handled = true;
            //    }
            //    catch
            //    {
            //        MessageBox.Show("程序发生严重的窗体线程异常，应用程序将退出！");
            //        this.Shutdown(); //程序已崩溃，此操作基本上已经无效了
            //    }

            //}

        }
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            //Environment.Exit(0);
        }
        #endregion

    }
}
