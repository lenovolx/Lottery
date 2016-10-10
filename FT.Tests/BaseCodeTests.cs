using System;
using System.Text;
using System.Collections.Generic;
using FT.Utility.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FT.Tests
{
    /// <summary>
    /// BaseCodeTests 的摘要说明
    /// </summary>
    [TestClass]
    public class BaseCodeTests
    {
        [TestMethod]
        public void ToStringTest()
        {
            var now = DateTime.Now;
            DateTime testTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, 60, 0);
            Console.Write(testTime.ToString());
        }
    }
}
