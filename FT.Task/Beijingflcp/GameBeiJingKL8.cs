using System.Collections.Generic;
using FT.Model;
using FT.Utility.Helper;

using EntityFramework.Extensions;
using System.Linq;

namespace FT.Task
{
    public class GameBeiJingKL8
    {
        public void InsertBeiJing28Data(List<BeiJingKL8Source> list, Entities.Entity context)
        {
            GameBeijing28Source obj;
            Log.Info("开始写入北京28玩法数据。。。");
            foreach (BeiJingKL8Source item in list)
            {
                obj = new GameBeijing28Source();
                var lotteryArr = item.SortedLotNumber.Split(',');
                var lotteryList = new List<string>(lotteryArr);
                obj.TermNumber = item.TermNumber;
                obj.LotteryNumber = item.LotteryNumber;
                obj.SortNumber = item.SortedLotNumber;
                //第一区号码 求和 尾数 生成
                var tempSum1 = 0;
                var tempStr1 = string.Empty;
                for (int i = 0; i < 6; i++)
                {
                    tempSum1 += lotteryList[i].ToInt();
                    tempStr1 += lotteryList[i] + ",";
                }
                obj.FirstAreaNo = tempStr1.TrimEnd(',');
                obj.FirstAreaResult = tempSum1;
                obj.FirstAreaMantissa = tempSum1.ToString().Substring(tempSum1.ToString().Length - 1).ToInt();
                //第二区号码
                var tempSum2 = 0;
                var tempStr2 = string.Empty;
                for (int i = 6; i < 12; i++)
                {
                    tempSum2 += lotteryList[i].ToInt();
                    tempStr2 += lotteryList[i] + ",";
                }
                obj.NextAreaNo = tempStr2.TrimEnd(',');
                obj.NextAreaResult = tempSum2;
                obj.NextAreaMantissa = tempSum2.ToString().Substring(tempSum2.ToString().Length - 1).ToInt();
                //第三区号码
                var tempSum3 = 0;
                var tempStr3 = string.Empty;
                for (int i = 12; i < 18; i++)
                {
                    tempSum3 += lotteryList[i].ToInt();
                    tempStr3 += lotteryList[i] + ",";
                }
                obj.LastAreaNo = tempStr3.TrimEnd(',');
                obj.LastAreaResult = tempSum3;
                obj.LastAreaMantissa = tempSum3.ToString().Substring(tempSum3.ToString().Length - 1).ToInt();

                obj.IsSync = true;
                obj.LotteryTime = item.LotteryTime;
                obj.SyncTime = System.DateTime.Now;
                if (context.GameBeijing28Source.First(s => s.TermNumber == obj.TermNumber) != null)
                {
                    context.GameBeijing28Source.Where(s => s.TermNumber == obj.TermNumber).Update(s => new GameBeijing28Source
                    {
                        LotteryNumber = obj.LotteryNumber,
                        SortNumber = obj.SortNumber,
                        FirstAreaNo = obj.FirstAreaNo,
                        FirstAreaMantissa = obj.FirstAreaMantissa,
                        FirstAreaResult = obj.FirstAreaResult,
                        NextAreaMantissa = obj.NextAreaMantissa,
                        NextAreaNo = obj.NextAreaNo,
                        NextAreaResult = obj.NextAreaResult,
                        LastAreaMantissa = obj.LastAreaMantissa,
                        LastAreaNo = obj.LastAreaNo,
                        LastAreaResult = obj.LastAreaResult,
                        LotteryTime = obj.LotteryTime,
                        IsSync = true,
                        SyncTime = System.DateTime.Now,
                    });
                }
                else
                {
                    context.GameBeijing28Source.Add(obj);
                }
            }
            Log.Info("结束写入北京28玩法数据。。。");
        }
    }
}