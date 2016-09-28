using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT.Model
{
    public partial class SystemSetting
    {
        /// <summary>
        /// 最大投注额
        /// </summary>
        [NotMapped]
        public decimal MaxBetAmount { get; set; }
        /// <summary>
        /// 最小投注额
        /// </summary>
        [NotMapped]
        public decimal MinBetAmount { get; set; }
        /// <summary>
        /// 危险球进球时效(滚球注单判别用.单位:秒)
        /// </summary>
        [NotMapped]
        public int DangerBall { get; set; }
        /// <summary>
        /// 是否关闭网站
        /// </summary>
        [NotMapped]
        public bool CloseWebSite { get; set; }
    }
}
