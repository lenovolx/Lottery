using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT.Model
{
    public partial class MatchResult
    {
        /// <summary>
        /// 比赛编号（扩展）
        /// </summary>
        public int MatchNumber { get; set; }
    }
}
