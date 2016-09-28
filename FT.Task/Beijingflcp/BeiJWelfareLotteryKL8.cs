using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using FT.Model;
using FT.Utility.Helper;
using Quartz;

namespace FT.Task.Beijingflcp
{
    public class BeiJWelfareLotteryKL8:BaseJob,IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            AnalyzrHtml();
        }

        public void AnalyzrHtml()
        {
            Log.Debug("北京快乐8  开始抓取");
            string siteUrl = ConfigurationManager.AppSettings["BeiJingKL8"];
            siteUrl = "http://www.bwlc.net/bulletin/keno.html";
            string url = siteUrl + "?page=1";
            try
            {
                string html = string.Empty;
                html = HTMLHelper.GetResponse(url);
                string pattern =
                    @"<tr class=.*?\s+<td>(.*?)</td>.*?\s+<td>(.*?)</td>.*?\s+<td>(.*?)</td>.*?\s+<td>(.*?)</td>.*?\s+</tr>";
                List<BeiJingKL8Source> arr = new List<BeiJingKL8Source>();
                Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
                MatchCollection mc = regex.Matches(html);
                foreach (Match match in mc)
                {
                    GroupCollection groupCollection = match.Groups;
                    var sortArr = groupCollection[2].Value.Split(',').AsQueryable().OrderBy(t => t.ToInt());
                    string sortStr = string.Join(",", sortArr);
                    arr.Add(new BeiJingKL8Source()
                    {
                        TermNumber = groupCollection[1].Value,
                        LotteryNumber = groupCollection[2].Value,
                        SingleNumber = groupCollection[3].Value,
                        LotteryTime = groupCollection[4].Value.ToDateTime(),
                        SortedLotNumber = sortStr,
                        CreateTime = DateTime.Now
                    });
                }
                var termNums = arr.Select(t => t.TermNumber);
                QueryDb((context) =>
                {
                    var existsMatch =
                        context.BeiJingKL8Source.Where(t => termNums.Contains(t.TermNumber)).ToArray();
                    var existsNums = existsMatch.Select(t => t.TermNumber).ToArray();
                    var notExistsData = arr.Where(t => !existsNums.Contains(t.TermNumber)).ToList();
                    if (notExistsData.Any())
                    {
                        context.BeiJingKL8Source.AddRange(notExistsData);
                    }
                    else
                        Log.Debug("北京快乐8 未更新新数据");

                    //

                    context.SaveChanges();
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            finally
            {
                Log.Debug("北京快乐8  抓取结束");
            }
        }
    }
}