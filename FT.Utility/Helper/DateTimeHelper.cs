using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT.Utility.Helper
{
    public class DateTimeHelper
    {
        #region Method

        /// <summary>
        /// 哪天
        /// </summary>
        /// <param name="days">7天前:-7 7天后:+7</param>
        /// <param name="dateTime">日期时间</param>
        /// <returns>返回值</returns>
        public static string GetTheDay(int? days, DateTime dateTime)
        {
            int day = days ?? 0;
            return dateTime.AddDays(day).ToShortDateString();
        }

        /// <summary>
        /// 周日
        /// </summary>
        /// <param name="weeks">上周-1 下周+1 本周0</param>
        /// <param name="dateTime">日期时间</param>
        /// <returns>返回值</returns>
        public static string GetSunday(int? weeks, DateTime dateTime)
        {
            int week = weeks ?? 0;
            return dateTime.AddDays(Convert.ToDouble((0 - Convert.ToInt16(dateTime.DayOfWeek))) + 7 * week).ToShortDateString();
        }

        /// <summary>
        /// 周六
        /// </summary>
        /// <param name="weeks">上周-1 下周+1 本周0</param>
        /// <param name="dateTime">日期时间</param>
        /// <returns>返回值</returns>
        public static string GetSaturday(int? weeks, DateTime dateTime)
        {
            int week = weeks ?? 0;
            return dateTime.AddDays(Convert.ToDouble((6 - Convert.ToInt16(dateTime.DayOfWeek))) + 7 * week).ToShortDateString();
        }

        /// <summary>
        /// 月第一天
        /// </summary>
        /// <param name="months">上月-1 本月0 下月1</param>
        /// <param name="dateTime">日期时间</param>
        /// <returns>返回值</returns>
        public static string GetFirstDayOfMonth(int? months, DateTime dateTime)
        {
            int month = months ?? 0;
            return DateTime.Parse(DateTime.Now.ToString("yyyy-MM-01")).AddMonths(month).ToShortDateString();
        }

        /// <summary>
        /// 月最后一天
        /// </summary>
        /// <param name="months">上月0 本月1 下月2</param>
        /// <param name="dateTime">日期时间</param>
        /// <returns>返回值</returns>
        public static string GetLastDayOfMonth(int? months, DateTime dateTime)
        {
            int month = months ?? 0;
            return DateTime.Parse(dateTime.ToString("yyyy-MM-01")).AddMonths(month).AddDays(-1).ToShortDateString();
        }

        /// <summary>
        /// 年度第一天
        /// </summary>
        /// <param name="dateTime">日期时间</param>
        /// <returns>返回值</returns>
        public static string GetFirstDayOfYear(DateTime dateTime)
        {
            int year = Convert.ToInt32(dateTime.Year);
            return DateTime.Parse(dateTime.ToString("yyyy-01-01")).AddYears(year).ToShortDateString();
        }

        /// <summary>
        /// 年度最后一天
        /// </summary>
        /// <param name="dateTime">日期时间</param>
        /// <returns>返回值</returns>
        public static string GetLastDayOfYear(DateTime dateTime)
        {
            int year = Convert.ToInt32(dateTime.Year);
            return DateTime.Parse(dateTime.ToString("yyyy-01-01")).AddYears(year).AddDays(-1).ToShortDateString();
        }

        /// <summary>
        /// 季度第一天
        /// </summary>
        /// <param name="quarters">上季度-1 下季度+1</param>
        /// <param name="dateTime">日期时间</param>
        /// <returns>返回值</returns>
        public static string GetFirstDayOfQuarter(int? quarters, DateTime dateTime)
        {
            int quarter = quarters ?? 0;
            return dateTime.AddMonths(quarter * 3 - ((dateTime.Month - 1) % 3)).ToString("yyyy-MM-01");
        }

        /// <summary>
        /// 季度最后一天
        /// </summary>
        /// <param name="quarters">上季度0 本季度1 下季度2</param>
        /// <param name="dateTime">日期时间</param>
        /// <returns>返回值</returns>
        public static string GetLastDayOfQuarter(int? quarters, DateTime dateTime)
        {
            int quarter = quarters ?? 0;
            return
                DateTime.Parse(dateTime.AddMonths(quarter * 3 - ((dateTime.Month - 1) % 3)).ToString("yyyy-MM-01")).AddDays(-1).
                    ToShortDateString();
        }

        /// <summary>
        /// 中文星期
        /// </summary>
        /// <param name="dateTime">日期时间</param>
        /// <param name="language"></param>
        /// <returns>返回值</returns>
        public static string GetDayOfWeekCN(DateTime dateTime, string language = "cn")
        {
            var day = new[] { "周日", "周一", "周二", "周三", "周四", "周五", "周六" };
            return day[Convert.ToInt16(dateTime.DayOfWeek)];
        }

        /// <summary>
        /// 获取星期数字形式,周一开始
        /// </summary>
        /// <param name="dateTime">日期时间</param>
        /// <returns>返回值</returns>
        public int GetDayOfWeekNum(DateTime dateTime)
        {
            int day = (Convert.ToInt16(dateTime.DayOfWeek) == 0) ? 7 : Convert.ToInt16(dateTime.DayOfWeek);
            return day;
        }

        /// <summary> 
        /// 取指定日期是一年中的第几周 
        /// </summary> 
        /// <param name="dtime">日期时间</param> 
        /// <returns>数字 一年中的第几周</returns> 
        public static int GetWeekofyear(DateTime dtime)
        {
            return CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(dtime, CalendarWeekRule.FirstDay,
                                                                     DayOfWeek.Sunday);
        }

        /// <summary> 
        /// 取指定日期是一月中的第几天 
        /// </summary> 
        /// <param name="dtime">日期时间</param> 
        /// <returns>数字 一月中的第几天</returns> 
        public static int GetDayofmonth(DateTime dtime)
        {
            return CultureInfo.CurrentCulture.Calendar.GetDayOfMonth(dtime);
        }

        /// <summary>
        /// 日期转换成unix时间戳
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="hours">累加小时</param>
        /// <returns></returns>
        public static long DateTimeToUnixTimestamp(DateTime dateTime, double hours = 4)
        {
            dateTime = dateTime.AddHours(hours);
            var start = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            return Convert.ToInt64((dateTime - start).TotalSeconds);
        }
        /// <summary>
        /// unix时间戳转换成日期
        /// </summary>
        /// <param name="timestamp">时间戳（秒）</param>
        /// <returns></returns>
        public static DateTime UnixTimestampToDateTime(long timestamp)
        {
            var start = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return start.AddSeconds(timestamp);
        }

        public static string SecondsToMinutes(int seconds, string splitChar)
        {
            var str = "{0}{1}{2}";
            if (seconds == 0) 
                str = string.Format(str, "00", splitChar, "00");
            else
            {
                var ts = new TimeSpan(0, 0, seconds);
                str = string.Format(str, (ts.Hours * 60 + ts.Minutes).ToString().PadLeft(2,'0'), splitChar, ts.Seconds.ToString().PadLeft(2, '0'));
            }
            return str;
        }
        #endregion
    }
}
