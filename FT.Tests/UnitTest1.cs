using System;
using System.Collections.Generic;
using System.Linq;
using FT.Model;
using FT.Utility.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FT.Entities;
using Quartz.Xml;
using EntityFramework.Extensions;
namespace FT.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void XmlDeserialize()
        {
            var list = new List<BetInfo>
            {
                new BetInfo
                {
                    BetType = "10",
                    BetIOR = "1.07|21.00|10.00",
                    BetKey = "H|C|N"
                },
                new BetInfo
                {
                    BetType = "11",
                    BetIOR = "1.36|12.83|3.50",
                    BetKey = "H|C|N"
                }
            };
            var xmlse = XmlHelper.Serializer(typeof(List<BetInfo>), list);


            var xml = @"<ArrayOfBetInfo><BetInfo><BetType>10</BetType><BetKey>H|C|N</BetKey><BetIOR>1.07|21.00|10.00</BetIOR></BetInfo><BetInfo><BetType>11</BetType><BetKey>H|C|N</BetKey><BetIOR>1.36|12.83|3.50</BetIOR></BetInfo><BetInfo><BetType>12</BetType><BetKey>2.5 / 3</BetKey><BetIOR>1.020|0.880</BetIOR></BetInfo><BetInfo><BetType>13</BetType><BetKey>1</BetKey><BetIOR>0.700|1.180</BetIOR></BetInfo><BetInfo><BetType>14</BetType><BetKey>3.5</BetKey><BetIOR>0.970|0.900</BetIOR></BetInfo><BetInfo><BetType>15</BetType><BetKey>1.5</BetKey><BetIOR>0.870|1.000</BetIOR></BetInfo><BetInfo><BetType>16</BetType><BetKey>O|E</BetKey><BetIOR>1.94|1.93</BetIOR></BetInfo><BetInfo><BetType>50</BetType><BetKey>H|C|N</BetKey><BetIOR>1.07|21.00|10.00</BetIOR></BetInfo><BetInfo><BetType>52</BetType><BetKey>2.5 / 3</BetKey><BetIOR>1.020|0.880</BetIOR></BetInfo><BetInfo><BetType>54</BetType><BetKey>3.5</BetKey><BetIOR>0.970|0.900</BetIOR></BetInfo><BetInfo><BetType>55</BetType><BetKey>1.5</BetKey><BetIOR>0.870|1.000</BetIOR></BetInfo><BetInfo><BetType>56</BetType><BetKey>O|E</BetKey><BetIOR>1.94|1.93</BetIOR></BetInfo></ArrayOfBetInfo>";
            var obj = XmlHelper.Deserialize(typeof(List<BetInfo>), xml);
        }

        [TestMethod]
        public void LamdaAny()
        {
            var listBetContentUpdate = new List<MatchUserBetContent>();
            var listBetUpdate = new List<MatchUserBet>();
            using (var context = new Entity())
            {
                var longlist = new List<long>
                {
                    22460,
                    22461
                };
                var matchinfo = context.MatchInfo.Where(s => longlist.Contains(s.MatchId)).ToList();
                foreach (var item in matchinfo)
                {
                    var dicResult = GetResult(context.MatchResult.FirstOrDefault(s => s.MatchId == item.MatchId));

                    var list =
                        context.MatchUserBet.Include("MatchUserBetContent").Where(
                            s => s.MatchUserBetContent.Select(b => b.MatchID).Any(c => longlist.Contains(c)))
                            .ToList();
                    if (list.Any())
                    {
                        foreach (var userbet in list)
                        {
                            var userbetcontent =
                                userbet.MatchUserBetContent.FirstOrDefault(c => c.MatchID == item.MatchId);
                            if (
                                !listBetContentUpdate.Any(
                                    c =>
                                        userbetcontent != null &&
                                        (c.MatchID == userbetcontent.MatchID && c.BetId == userbetcontent.BetId &&
                                         c.BetType == userbetcontent.BetType)))
                            {
                                if (userbetcontent != null)
                                {
                                    userbetcontent.IsSettle = OpenEnum.Settlemented;
                                    userbetcontent.SettleIor = GetIor(dicResult, userbetcontent);
                                    listBetContentUpdate.Add(userbetcontent);
                                }
                            }
                            if ((userbet.BetType != 5 ||
                                 userbet.MatchUserBetContent.Count(c => c.IsSettle == OpenEnum.Settlementing) != 0) &&
                                userbet.BetType == 5) continue;
                            {
                                userbet.IsSettle = OpenEnum.Settlemented;
                                userbet.SettleTime = DateTime.Now;
                                userbet.BetBonus =
                                    userbet.MatchUserBetContent.Select(
                                        c => c.SettleIor ?? 0).Aggregate((i, j) => i * j) *
                                    userbet.BetValue;
                                listBetUpdate.Add(userbet);
                            }
                        }
                    }

                }

                var listbetid = listBetContentUpdate.Select(w => w.BetId);

                var dic = listBetContentUpdate.Select(s => new
                {
                    BetId = s.BetId,
                    SettleIor = s.SettleIor
                }).ToArray();



                //context.MatchUserBetContent.Where(s => listbetid.Contains(s.BetId))
                //    .Update(u => new MatchUserBetContent
                //    {
                //        IsSettle = OpenEnum.Settlemented,
                //        SettleIor = dic.FirstOrDefault(t => t.BetId == u.BetId).SettleIor
                //    });
                var sql = @"UPDATE MatchUserBetContent SET IsSettle=0,SettleIor=null WHERE BetId=2016072043850798;
UPDATE MatchUserBetContent SET IsSettle=0,SettleIor=null WHERE BetId=2016072002343977;
UPDATE MatchUserBetContent SET IsSettle=0,SettleIor=null WHERE BetId=2016072074532485;";

                Assert.IsTrue(context.Database.ExecuteSqlCommand(sql) > 0);
            }
        }

        [TestMethod]
        public void BulkUpdate()
        {
            Log.Debug("q34123");
        }

        public static int ConvertDateTimeInt(DateTime time, int addHours = 4)
        {
            time = time.AddHours(addHours);//加4小时，
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return Convert.ToInt32((time - startTime).TotalSeconds);
        }
        [TestMethod]
        public void MatchSettle()
        {
            FT.Repository.MatchInfoRepository match = new Repository.MatchInfoRepository();
            match.MatchSettlement(24600, 1);
        }
        [TestMethod]
        public void json2object()
        {
            string a = "{\"code\":0,\"errors\":null,\"message\":\"操作成功\",\"data\":{\"Leagues\":[]},\"total\":0}";
            FT.Utility.ApiHelper.ApiReturn ar = Newtonsoft.Json.JsonConvert.DeserializeObject<FT.Utility.ApiHelper.ApiReturn>(a);
        }

        [TestMethod]
        public void BetIRO()
        {
            var dic = new Dictionary<string, string>
            {
                {"14", "3"}
            };
            var content = new MatchUserBetContent
            {
                BetType = 14,
                BetContent = "O@1.74",
                BetKey = "2.5/3"
            };
            Assert.IsTrue(GetIor(dic, content) == decimal.Parse((1.74 / 2 + 0.5)+""));
        }

        #region Private Method

        /// <summary>
        /// 获取中奖赔率，没中奖的则为0
        /// </summary>
        /// <param name="dicResult"></param>
        /// <param name="mc"></param>
        /// <returns></returns>
        private decimal GetIor(Dictionary<string, string> dicResult, MatchUserBetContent mc)
        {
            decimal ior = 0;
            try
            {
                ior = decimal.Parse(mc.BetContent.Split('@')[1].Trim());
                var key = mc.BetContent.Split('@')[0].Trim();
                #region 让球
                if (mc.BetType == 12 || mc.BetType == 13 || mc.BetType == 63)
                {
                    var strong = mc.BetKey.Split('|')[0].Trim();
                    var ratio = mc.BetKey.Split('|')[1].Trim();
                    decimal ratio1 = 0;
                    decimal ratio2 = 0;
                    string result1 = "";//让球后结果
                    string result2 = "";//让球后结果
                    if (ratio.Split('/').Length > 1)
                    {
                        ratio1 = decimal.Parse((strong == "H" ? "-" : "") + ratio.Split('/')[0].Trim());
                        ratio2 = decimal.Parse((strong == "H" ? "-" : "") + ratio.Split('/')[1].Trim());
                    }
                    else
                    {
                        ratio2 = ratio1 = decimal.Parse((strong == "H" ? "-" : "") + ratio.Trim());
                    }
                    if (ratio1 + decimal.Parse(dicResult[mc.BetType + ""]) == 0)
                    {
                        result1 = "N";//平
                    }
                    else if (ratio1 + decimal.Parse(dicResult[mc.BetType + ""]) > 0)
                    {
                        result1 = "H";//主胜
                    }
                    else
                    {
                        result1 = "C";//客胜
                    }
                    if (ratio2 + decimal.Parse(dicResult[mc.BetType + ""]) == 0)
                    {
                        result2 = "N";//平
                    }
                    else if (ratio2 + decimal.Parse(dicResult[mc.BetType + ""]) > 0)
                    {
                        result2 = "H";//主胜
                    }
                    else
                    {
                        result2 = "C";//客胜
                    }
                    decimal tmp = 0;
                    tmp += result1 == "N" ? 0.5M : result1 == key ? ior / 2 : 0;
                    tmp += result2 == "N" ? 0.5M : result2 == key ? ior / 2 : 0;
                    ior = tmp;
                }
                #endregion
                #region 大小球
                else if (mc.BetType == 14 || mc.BetType == 15 || mc.BetType == 61)
                {
                    var ratio = mc.BetKey.Trim();
                     decimal ratio1 = 0;
                     decimal ratio2 = 0;
                    var result1 = "";//结果12
                    var result2 = "";//结果
                    if (ratio.Split('/').Length > 1)
                    {
                        ratio1 = decimal.Parse(ratio.Split('/')[0].Trim());
                        ratio2 = decimal.Parse(ratio.Split('/')[1].Trim());
                    }
                    else
                    {
                        ratio2 = ratio1 = decimal.Parse(ratio.Trim());
                    }
                    if (ratio1 == decimal.Parse(dicResult[mc.BetType + ""]))
                    {
                        result1 = "N";//平
                    }
                    else if (ratio1 < decimal.Parse(dicResult[mc.BetType + ""]))
                    {
                        result1 = "O";//大球
                    }
                    else
                    {
                        result1 = "U";//小球
                    }
                    if (ratio2 == decimal.Parse(dicResult[mc.BetType + ""]))
                    {
                        result2 = "N";//平
                    }
                    else if (ratio2 < decimal.Parse(dicResult[mc.BetType + ""]))
                    {
                        result2 = "O";//大球
                    }
                    else
                    {
                        result2 = "U";//小球
                    }
                    decimal tmp = 0;
                    tmp += result1 == "N" ? 0.5M : result1 == key ? ior / 2 : 0;
                    tmp += result2 == "N" ? 0.5M : result2 == key ? ior / 2 : 0;
                    ior = tmp;
                }
                #endregion
                else
                {
                    if (key != dicResult[mc.BetType + ""])
                    {
                        ior = 0;
                    }
                }
            }
            catch
            {
                ior = 0;
            }
            return ior;
        }

        /// <summary>
        /// 根据比赛结果，组合出所有的赛果
        /// </summary>
        /// <param name="mr"></param>
        /// <returns></returns>
        private static Dictionary<string, string> GetResult(MatchResult mr, XBETMatchResult xbet = null)
        {
            var dicResult = new Dictionary<string, string>();
            try
            {
                if (xbet == null)
                {
                    var hmh = int.Parse(mr.Result1.Split(':')[0]);
                    var hmc = int.Parse(mr.Result1.Split(':')[1]);
                    var mh = int.Parse(mr.Result2.Split(':')[0]);
                    var mc = int.Parse(mr.Result2.Split(':')[1]);
                    dicResult.Add("10", string.Format("{0}", mh > mc ? "H" : mh == mc ? "N" : "C")); //全场胜负平
                    dicResult.Add("11", string.Format("{0}", hmh > hmc ? "H" : hmh == hmc ? "N" : "C")); //半场胜负平
                    dicResult.Add("12", string.Format("{0}", mh - mc)); //全场分差
                    dicResult.Add("13", string.Format("{0}", hmh - hmc)); //半场分差
                    dicResult.Add("14", string.Format("{0}", mh + mc)); //全场总进球
                    dicResult.Add("15", string.Format("{0}", hmh + hmc)); //半场总进球
                    dicResult.Add("20",
                        string.Format("{0}",
                            (mh + mc) >= 7 ? "7+" : (mh + mc) >= 4 ? "4-6" : (mh + mc) >= 2 ? "2-3" : "0-1"));
                    //全场总进球
                    dicResult.Add("30", string.Format("{0}", (mh > 4 || mc > 4) ? "OVH" : mr.Result2)); //全场比分
                    dicResult.Add("31", string.Format("{0}", (hmh > 3 || hmc > 3) ? "OVH" : mr.Result1)); //半场比分
                    dicResult.Add("40", string.Format("{0}", dicResult["11"] + dicResult["10"])); //半全场
                    dicResult.Add("16", string.Format("{0}", (mh + mc) % 2 == 0 ? "E" : "O")); //全场总进球
                    dicResult.Add("50", string.Format("{0}", mh > mc ? "H" : mh == mc ? "N" : "C")); //全场胜负平
                    dicResult.Add("51", string.Format("{0}", hmh > hmc ? "H" : hmh == hmc ? "N" : "C")); //半场胜负平
                    dicResult.Add("52", string.Format("{0}", mh - mc)); //全场分差
                    dicResult.Add("53", string.Format("{0}", hmh - hmc)); //半场分差
                    dicResult.Add("54", string.Format("{0}", mh + mc)); //全场总进球
                    dicResult.Add("55", string.Format("{0}", hmh + hmc)); //半场总进球
                    dicResult.Add("56", string.Format("{0}", (mh + mc) % 2 == 0 ? "E" : "O")); //全场总进球
                }
                else
                {
                    var mh = int.Parse(xbet.Result2.Split(':')[0]);
                    var mc = int.Parse(xbet.Result2.Split(':')[1]);
                    dicResult.Add("60", string.Format("{0}", mh > mc ? "H" : mh == mc ? "N" : "C")); //滚球全场胜负平
                    dicResult.Add("61", string.Format("{0}", mh + mc)); //滚球全场总进球
                    dicResult.Add("63", string.Format("{0}", mh - mc)); //滚球全场分差
                }
            }
            catch
            {
                // ignored
            }
            return dicResult;
        }

        #endregion
    }

    public class User
    {
        public string name { get; set; }
        public int locks { get; set; }
    }
}
