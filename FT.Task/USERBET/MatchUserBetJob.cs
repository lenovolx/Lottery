using EntityFramework.Extensions;
using FT.Entities;
using FT.Model;
using FT.Utility.CacheHelper;
using FT.Utility.Helper;
using Quartz;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace FT.Task.USERBET
{
    /// <summary>
    /// 足彩数据抓取Job
    /// </summary>
    [DisallowConcurrentExecution]
    public class MatchUserBetJob : BaseJob, IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            BackUpData();
        }

        public void BackUpData()
        {
            Log.Debug("MatchUserBetJob  备份开始");
            QueryDb((context) =>
            {
                DateTime dtnow = DateTime.Now.AddDays(-7);
                var MatchUserBetList = context.MatchUserBet.Where(a => a.CreateDate <= dtnow ).OrderBy(a => a.CreateDate).ToList();

                foreach (var mub_list in MatchUserBetList)
                {
                    long BetId = mub_list.BetId;
                    var mubc_list = context.MatchUserBetContent.Where(a => a.BetId == BetId).ToList();

                    try
                    {
                        if (mubc_list.Count >= 1)
                        {
                            List<MatchUserBetContentLog> mubclist = Getmubc_list(mubc_list);
                            context.MatchUserBetContentLog.AddRange(mubclist);
                            context.MatchUserBetContent.RemoveRange(mubc_list);

                            List<MatchUserBetLog> mlist = Getmub_list(mub_list);
                            context.MatchUserBetLog.AddRange(mlist);
                            context.MatchUserBet.Remove(mub_list);
                        }
                        else {
                            List<MatchUserBetLog> mlist = Getmub_list(mub_list);
                            context.MatchUserBetLog.AddRange(mlist);
                            context.MatchUserBet.Remove(mub_list);
                        }
                        context.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        Log.Info(e.Message, null);
                    }
                }
            });
            Log.Debug("MatchUserBetJob  备份结束");
        }

        private List<MatchUserBetLog> Getmub_list(MatchUserBet mub_list)
        {
            List<MatchUserBetLog> list_mublog = new List<MatchUserBetLog>();
            MatchUserBetLog Mubl_model = new MatchUserBetLog();
            Mubl_model.BetId = mub_list.BetId;
            Mubl_model.UserId = mub_list.UserId;
            Mubl_model.BetValue = mub_list.BetValue;
            Mubl_model.BetType = mub_list.BetType;
            Mubl_model.BetBonus = mub_list.BetBonus;
            Mubl_model.BetTime = mub_list.BetTime;
            Mubl_model.CreateDate = mub_list.CreateDate;
            Mubl_model.IsSettle = (byte)mub_list.IsSettle;
            Mubl_model.SettleTime = mub_list.SettleTime;
            Mubl_model.IsGetPrize = (int)mub_list.IsGetPrize;
            Mubl_model.GetTime = mub_list.GetTime;
            Mubl_model.BetSource = (int)mub_list.BetSource;
            list_mublog.Add(Mubl_model);
            return list_mublog;
        }

        private List<MatchUserBetContentLog> Getmubc_list(List<MatchUserBetContent> list)
        {
            List<MatchUserBetContentLog> list_mubclog = new List<MatchUserBetContentLog>();
            foreach (var item in list)
            {
                MatchUserBetContentLog Mublc_model = new MatchUserBetContentLog();
                Mublc_model.BetId = item.BetId;
                Mublc_model.MatchID = item.MatchID;
                Mublc_model.BetType = item.BetType;
                Mublc_model.BetContent = item.BetContent;
                Mublc_model.BetKey = item.BetKey;
                Mublc_model.IsSettle = (int?)item.IsSettle;
                Mublc_model.SettleIor = item.SettleIor;
                Mublc_model.BetScore = item.BetScore;

                list_mubclog.Add(Mublc_model);
            }
            return list_mubclog;
        }
    }
}
