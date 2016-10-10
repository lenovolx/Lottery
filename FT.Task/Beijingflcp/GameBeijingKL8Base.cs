using System;
using System.Linq;
using Quartz;
using System.Configuration;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using FT.Model;
using FT.Utility.Helper;

namespace FT.Task
{
    [DisallowConcurrentExecution]
    public class GameBeijingKL8Base:BaseJob,IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            InsertBJ28BaseData();
        }

        public void InsertBJ28BaseData()
        {
            DateTime now = DateTime.Now;
            DateTime startTime = new DateTime(now.Year, now.Month, now.Day, 9, 5, 0);
            DateTime endTime = new DateTime(now.Year, now.Month, now.Day, 23, 55, 0);
            
            DateTime firstTime = new DateTime();
            if (now > endTime)
            {
                return;
            }
            if (now < startTime)
            {
                firstTime = startTime;
            }
            else
            {
                //int[] minuteArr = { 60, 55, 50, 45, 40, 35, 30, 25, 20, 15, 10, 5 };
                int[] minuteArr = { 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60 };
                for (int i = 0; i < minuteArr.Length; i++)
                {
                    if (now.Minute < minuteArr[i])
                    {
                        var tempHour = now.Hour;
                        var tempMinute = minuteArr[i];
                        if (minuteArr[i] == 60)
                        {
                            tempHour = tempHour + 1;
                            tempMinute = 0;
                        }
                        firstTime = new DateTime(now.Year, now.Month, now.Day, tempHour, tempMinute, 0);
                        break;
                    }
                }
            }
            //获取最新期号
            string siteUrl = ConfigurationManager.AppSettings["BeiJingKL8"];
            siteUrl = "http://www.bwlc.net/bulletin/keno.html";
            string url = siteUrl;
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
            var max = arr.Max(s => s.TermNumber.ToInt());
            List<GameBeijing28Source> bj28 = new List<GameBeijing28Source>();
            DateTime lotTime = firstTime;
            do
            {
                bj28.Add(new GameBeijing28Source
                {
                    TermNumber = (max + 1)+"",
                    LotteryTime = lotTime,
                });
                lotTime = lotTime.AddMinutes(5);
                max++;
            } while (lotTime <= endTime);
            var termNums = bj28.Select(s => s.TermNumber);
            QueryDb((Db) =>
            {
                var existsMatch =
                        Db.GameBeijing28Source.Where(t => termNums.Contains(t.TermNumber)).ToArray();
                var existsNums = existsMatch.Select(t => t.TermNumber).ToArray();
                var notExistsData = bj28.Where(t => !existsNums.Contains(t.TermNumber)).ToList();
                if (notExistsData.Any())
                {
                    Db.GameBeijing28Source.AddRange(notExistsData);
                }
                Db.SaveChanges();
            });
        }
    }
}
