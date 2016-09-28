using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT.Plugin.Cache.Redis
{
    /// <summary>
    /// 
    /// </summary>
    public class RedisConfig
    {
        public RedisConfig()
        {
            this.Prot = 6379;
        }
        public string Host { get; set; }
        public int Prot { get; set; }
    }
}
