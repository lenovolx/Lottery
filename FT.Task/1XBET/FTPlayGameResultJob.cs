using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FT.Model;
using FT.Utility.ApiHelper;
using FT.Utility.Helper;
using Quartz;

namespace FT.Task
{
    /// <summary>
    /// 1XBET 比赛结果
    /// </summary>
    public class FTPlayGameResultJob:BaseJob,IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            var param = new HttpParam {{"Language", "cn"}, {"Params", "[null, null, \"1\", null, null]"}};
            var result = HttpHelper.Post<ResultMatch>(XBETResultUrl, param, null, 10, null, null, null, true);
            throw new NotImplementedException();
        }
    }
}
