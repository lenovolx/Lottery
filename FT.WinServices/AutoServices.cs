using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;
using Quartz.Simpl;
using Quartz.Xml;
using FT.Utility.Helper;
namespace FT.WinServices
{
    public partial class AutoServices : ServiceBase
    {
        public AutoServices()
        {
            InitializeComponent();
        }
        private IScheduler sched;
        protected override void OnStart(string[] args)
        {
            Log.Debug("FT.Auto.Services 开始运行.....");
            XMLSchedulingDataProcessor processor = new XMLSchedulingDataProcessor(new SimpleTypeLoadHelper());
            ISchedulerFactory factory = new StdSchedulerFactory();
            sched = factory.GetScheduler();
            processor.ProcessFileAndScheduleJobs(IoHelper.GetMapPath("/quartz_jobs.xml"), sched);
            sched.Start();
        }
        protected override void OnStop()
        {
            Log.Debug("FT.Auto.Services 停止运行.....");
        }
    }
}
