

namespace FT.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class SystemTask
    {
        public System.Guid TaskID { get; set; }
        public string TaskName { get; set; }
        public string CronExpressionString { get; set; }
        public string Assembly { get; set; }
        public string Class { get; set; }
        public Nullable<TaskRunEnum> Status { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<System.DateTime> ModifyOn { get; set; }
        public Nullable<System.DateTime> RecentRunTime { get; set; }
        public Nullable<System.DateTime> LastRunTime { get; set; }
        public string CronRemark { get; set; }
        public string Remark { get; set; }
    }
}
