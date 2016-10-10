using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FT.Task;

namespace FT.Tests.BeijingKL8
{
    /// <summary>
    /// BeijingKL8DataCollectionTest 的摘要说明
    /// </summary>
    [TestClass]
    public class BeijingKL8DataCollectionTest
    {
        public BeijingKL8DataCollectionTest()
        {
            //
            //TODO:  在此处添加构造函数逻辑
            //
        }
        [TestMethod]
        public void TestMethod1()
        {
            BeiJWelfareLotteryKL8 kl8Job = new BeiJWelfareLotteryKL8();
            kl8Job.AnalyzrHtml();
        }

        [TestMethod]
        public void GameBeijingKL8Base_InsertBJ28BaseData()
        {
            GameBeijingKL8Base kl8 = new GameBeijingKL8Base();
            kl8.InsertBJ28BaseData();
        }
    }
}
