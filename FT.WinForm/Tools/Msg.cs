using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows.Threading;
using System.Windows.Media;

namespace FT.WinForm.Tools
{
    public class Msg
    {
        //private static MsgWin MsgWin;
        //static System.Timers.Timer timer = new System.Timers.Timer();

        /// <summary>
        /// 信息提示
        /// </summary>
        /// <param name="msg"></param>        
        public static void Success(string msg)
        {
            App.Current.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)delegate()
            {
                MsgWin MsgWin = new MsgWin();
                MsgWin.tbMsg.Text = msg;
                MsgWin.gridLeft.Background = new SolidColorBrush((Color)System.Windows.Media.ColorConverter.ConvertFromString("#5eba11"));
                MsgWin.gridMsg.Background = new SolidColorBrush((Color)System.Windows.Media.ColorConverter.ConvertFromString("#f1f9e9"));
                MsgWin.Topmost = true;
                MsgWin.ShowDialog();
            });
            //timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            //timer.Interval = 3000;    // 1秒 = 1000毫秒
            //timer.Start();
        }

        /// <summary>
        /// 一般信息提示
        /// </summary>
        /// <param name="msg"></param>        
        public static void Info(string msg)
        {
            App.Current.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)delegate()
            {
                MsgWin MsgWin = new MsgWin();
                MsgWin.tbMsg.Text = msg;
                MsgWin.gridLeft.Background = new SolidColorBrush((Color)System.Windows.Media.ColorConverter.ConvertFromString("#31708f"));
                MsgWin.gridMsg.Background = new SolidColorBrush((Color)System.Windows.Media.ColorConverter.ConvertFromString("#d9edf7"));
                MsgWin.Topmost = true;
                MsgWin.ShowDialog();
            });
            //timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            //timer.Interval = 3000;    // 1秒 = 1000毫秒
            //timer.Start();
        }

        /// <summary>
        /// 警告信息提示
        /// </summary>
        /// <param name="msg"></param>        
        public static void Warning(string msg)
        {
            App.Current.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)delegate()
            {
                MsgWin MsgWin = new MsgWin();
                MsgWin.tbMsg.Text = msg;
                MsgWin.gridLeft.Background = new SolidColorBrush((Color)System.Windows.Media.ColorConverter.ConvertFromString("#8a6d3b"));
                MsgWin.gridMsg.Background = new SolidColorBrush((Color)System.Windows.Media.ColorConverter.ConvertFromString("#fcf8e3"));
                MsgWin.Topmost = true;
                MsgWin.ShowDialog();
            });
            //timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            //timer.Interval = 3000;    // 1秒 = 1000毫秒
            //timer.Start();
        }

        /// <summary>
        /// 错误信息提示
        /// </summary>
        /// <param name="msg"></param>        
        public static void Error(string msg)
        {
            App.Current.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)delegate()
            {
                MsgWin MsgWin = new MsgWin();
                MsgWin.tbMsg.Text = msg;
                MsgWin.gridLeft.Background = new SolidColorBrush((Color)System.Windows.Media.ColorConverter.ConvertFromString("#a94442"));
                MsgWin.gridMsg.Background = new SolidColorBrush((Color)System.Windows.Media.ColorConverter.ConvertFromString("#f2dede"));
                MsgWin.Topmost = true;
                MsgWin.ShowDialog();
            });
            //timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            //timer.Interval = 3000;    // 1秒 = 1000毫秒
            //timer.Start();
        }

        /// <summary>
        /// Timer的Elapsed事件处理程序
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        //private static void OnTimedEvent(object source, ElapsedEventArgs e)
        //{
        //    timer.Elapsed -= new ElapsedEventHandler(OnTimedEvent);
        //    timer.Stop();
        //    MsgWin.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)delegate()
        //    {
        //        MsgWin.Hide();
        //    });
        //}

    }
}
