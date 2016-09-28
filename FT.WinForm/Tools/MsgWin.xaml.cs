using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace FT.WinForm.Tools
{
    /// <summary>
    /// TwiMsgWin.xaml 的交互逻辑
    /// </summary>
    public partial class MsgWin : Window
    {
        public MsgWin()
        {
            InitializeComponent();
            this.ShowInTaskbar = false;
            //this.Left = SystemParameters.WorkArea.Width - this.Width;
            //this.Top = SystemParameters.WorkArea.Height - this.Height; 
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = 3000;    // 1秒 = 1000毫秒
            aTimer.Start();
        }
        private System.Timers.Timer aTimer = new System.Timers.Timer();
        /// <summary>
        /// Timer的Elapsed事件处理程序
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            aTimer.Elapsed -= new ElapsedEventHandler(OnTimedEvent);
            aTimer.Stop();
            this.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)delegate()
            {
                this.Close();
            });
        }
    }
}
