using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT.Model
{
    public class ResultMatch
    {
        /// <summary>
        /// 
        /// </summary>
        public List<string> Columns { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<List<object>> Data { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Error { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Success { get; set; }
    }
}
