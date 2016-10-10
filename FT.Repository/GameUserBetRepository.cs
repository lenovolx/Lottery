using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FT.Model;
using FT.Entities;
using FT.Utility.Helper;
using FT.IRepository;
using EntityFramework.Extensions;

namespace FT.Repository
{
    public class GameUserBetRepository:BaseRepository<GameUserBet>,IGameUserBetRepository
    {
        public BetEnum UserBet(GameUserBet bet, double zone=8)
        {
            return QueryDbUseTran((context) =>
            {
                var ident = BetEnum.Success;
                if (bet != null)
                {
                    if (bet.BetMoney > 0)
                    {
                        var setting = new SystemSettingRepository().GetSetting();
                        if (bet.UserId > 0)
                        {
                            var user = context.UserAccount.FirstOrDefault(s => s.Id == bet.UserId);
                            if (user != null)
                            {
                                var datenowunix = DateTimeHelper.DateTimeToUnixTimestamp(DateTime.Now, -zone);
                                
                                var betDb = context.GameUserBet.FirstOrDefault(s => s.TermNumber == bet.TermNumber);
                                if (user.BalanceAmount > bet.BetMoney)
                                {
                                    if (betDb == null)
                                    {
                                        context.GameUserBet.Add(bet);
                                        user.BalanceAmount += -bet.BetMoney;
                                        //添加金额流水
                                        context.AmountWater.Add(new AmountWater
                                        {
                                            CreateDate = DateTime.Now,
                                            UserId = user.Id,
                                            UserName = user.LoginName,
                                            Type = TradEnum.Bet,
                                            Amount = -bet.BetMoney,
                                            Remark = string.Format("投注,期号{0}扣除余额{1}", bet.TermNumber, bet.BetMoney)
                                        });
                                    }
                                    else
                                    {
                                        decimal moneySum = betDb.BetMoney + bet.GameUserBetContent.Select(s => s.BetMoney).Sum();
                                        betDb.BetMoney = moneySum;
                                        betDb.GameUserBetContent.ToList().AddRange(bet.GameUserBetContent);
                                        if (context.GameUserBet.Where(s => s.TermNumber == bet.TermNumber).Update(s => betDb) > 0)
                                            ident = BetEnum.Success;
                                    }
                                }
                                else
                                {
                                    ident = BetEnum.BalanceNotEnough;
                                }
                            }
                            else
                            {
                                ident = BetEnum.UserNotFind;
                            }
                        }
                        else
                        {
                            ident = BetEnum.UserNotFind;
                        }
                    }
                    else
                    {
                        ident = BetEnum.BetAmountZero;
                    }
                }
                else
                {
                    ident = BetEnum.ObjectNull;
                }
                return ident;
            }, BetEnum.Failure);
        }
    }
}
