using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
namespace FT.WinForm
{
    public class Config
    {
        public static string APIKey()
        {
            string key = ConfigurationManager.AppSettings["APIKey"];
            return key;
        }

        public static string ServerAddress()
        {
            string Address = ConfigurationManager.AppSettings["ServerUrl"];
            return Address;
        }
    }
}
