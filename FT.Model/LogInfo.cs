

namespace FT.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class LogInfo
    {
        public string Id { get; set; }
        public System.DateTime LogTime { get; set; }
        public string LogUser { get; set; }
        public string LogUserIp { get; set; }
        public string LogController { get; set; }
        public string LogAction { get; set; }
        public string LogOperate { get; set; }
        public string LogType { get; set; }
    }
}
