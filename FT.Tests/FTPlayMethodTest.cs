using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using FT.Task.USERBET;

namespace FT.Tests
{
    [TestClass]
    public class FTPlayMethodTest
    {
        [TestMethod]
        public void TestOldMatch()
        {
            FT.Task.OldPlayMethodJob MatchInfoTask = new Task.OldPlayMethodJob();
            MatchInfoTask.AnalyzeHtml();//比赛

            FT.Task.OldFTPlayResultJob MatchResultTask = new Task.OldFTPlayResultJob();
            MatchResultTask.AnalyzeHtml();//赛果
        }


        //新网站数据抓取
        [TestMethod]
        public void TestNewHG()
        {
            FT.Task.NewPlayMethodJob newMatchIfnoTast = new Task.NewPlayMethodJob();
            newMatchIfnoTast.NewAnalyzeHtml();
        }

        [TestMethod]
        public void TestNewHGRusult() {

            string htmlcode = "";
            using (System.IO.StreamReader sr = new System.IO.StreamReader(@"E:\\n1.sql", UTF8Encoding.Default))
            {
                string str;
                while ((str = sr.ReadLine()) != null)
                {
                    htmlcode += str;
                }
            }

            FT.Task.NewFTPlayResultJob NRusultJob = new Task.NewFTPlayResultJob();
            NRusultJob.TestResult(htmlcode);
        }

        [TestMethod]
        public void TestMatchBetBackUpdate()
        {
            MatchUserBetJob mu = new MatchUserBetJob();
            mu.BackUpData();
        }
    }
}
