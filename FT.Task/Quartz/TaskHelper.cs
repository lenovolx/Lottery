using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityFramework.Extensions;
using FT.Model;

namespace FT.Task.Quartz
{
    public class TaskHelper : BaseJob
    {
        /// <summary>
        /// 运行指定任务
        /// </summary>
        /// <param name="taskId"></param>
        public static void RunById(string taskId)
        {
            QuartzHelper.RunOnceTask(taskId);
        }

        /// <summary>
        /// 删除指定id任务
        /// </summary>
        /// <param name="taskId">任务id</param>
        public static void DeleteById(string taskId)
        {
            QuartzHelper.DeleteJob(taskId);
        }

        /// <summary>
        /// 更新任务运行状态
        /// </summary>
        /// <param name="taskId">任务id</param>
        /// <param name="status">任务状态</param>
        public static void UpdateTaskStatus(string taskId, int status)
        {
            if (status == 0)
                QuartzHelper.ResumeJob(taskId);
            else
                QuartzHelper.PauseJob(taskId);
        }

        /// <summary>
        /// 更新任务下次运行时间
        /// </summary>
        /// <param name="TaskID">任务id</param>
        /// <param name="LastRunTime">下次运行时间</param>
        public static void UpdateLastRunTime(string TaskID, DateTime LastRunTime)
        {
            if (TaskID == null) throw new ArgumentNullException("TaskID");
            var taskId = Guid.Parse(TaskID);
            QueryDb((context) =>
            {
                context.SystemTask.Where(s => s.TaskID == taskId).Update(u => new SystemTask
                {
                    LastRunTime = LastRunTime
                });
            });
        }

        /// <summary>
        /// 更新任务最近运行时间
        /// </summary>
        /// <param name="TaskID">任务id</param>
        /// <param name="lastRunTime"></param>
        public static void UpdateRecentRunTime(string TaskID, DateTime lastRunTime)
        {
            if (TaskID == null) throw new ArgumentNullException("TaskID");
            var taskId = Guid.Parse(TaskID);
            QueryDb((context) =>
            {
                context.SystemTask.Where(s => s.TaskID == taskId).Update(u => new SystemTask
                {
                    RecentRunTime = DateTime.Now,
                    LastRunTime = lastRunTime
                });
            });
        }
        /// <summary>
        /// 任务更新/保存
        /// </summary>
        /// <param name="value"></param>
        /// <param name="update"></param>
        public static void SaveTask(TaskUtil value, bool update = false)
        {
            var date = QuartzHelper.GetTaskeFireTime(value.CronExpressionString, 1)[0];
            value.LastRunTime = date;
            if (update)
            {
                //添加新任务
                value.TaskID = Guid.NewGuid();
                var flag = QueryDb((context) =>
                {
                    context.SystemTask.Add(new SystemTask
                    {
                        LastRunTime = value.LastRunTime.Value,
                        TaskName = value.TaskName,
                        CronExpressionString = value.CronExpressionString,
                        Assembly = value.Assembly,
                        Class = value.Class,
                        CronRemark = value.CronRemark,
                        Remark = value.Remark
                    });
                    return true;
                }, false);
                if (flag)
                    QuartzHelper.ScheduleJob(value);
            }
            else
            {
                //更新任务
                var flag = QueryDb((context) =>
                {
                    context.SystemTask.Where(s => s.TaskID == value.TaskID).Update(u => new SystemTask
                    {
                        LastRunTime = value.LastRunTime.Value,
                        TaskName = value.TaskName,
                        CronExpressionString = value.CronExpressionString,
                        Assembly = value.Assembly,
                        Class = value.Class,
                        CronRemark = value.CronRemark,
                        Remark = value.Remark
                    });
                    return true;
                }, false);
                if (flag)
                    QuartzHelper.ScheduleJob(value, true);
            }
        }
    }
}
