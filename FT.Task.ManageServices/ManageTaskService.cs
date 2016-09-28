using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FT.Task.Quartz;

namespace FT.Task.ManageServices
{
    public partial class ManageTaskService : ServiceBase
    {
        public ManageTaskService()
        {
            InitializeComponent();
        }
        protected override void OnStart(string[] args)
        {
            var att = Assembly.GetExecutingAssembly().GetCustomAttribute<DebuggableAttribute>();
            if (att.IsJITTrackingEnabled)
                Thread.Sleep(10000);
            QuartzHelper.InitScheduler();
            QuartzHelper.StartScheduler();
        }
        protected override void OnStop()
        {
            QuartzHelper.StopSchedule();
            Environment.Exit(0);
        }
    }
}
