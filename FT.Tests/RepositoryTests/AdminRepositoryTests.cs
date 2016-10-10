using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FT.Repository;

namespace FT.Tests.RepositoryTests
{
    /// <summary>
    /// AdminRepositoryTests 的摘要说明
    /// </summary>
    [TestClass]
    public class AdminRepositoryTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            AdminRepository admin = new AdminRepository();
            string username = "admin";
            string password = "888888";
            admin.ManagerLogin(username, password);
        }
    }
}
