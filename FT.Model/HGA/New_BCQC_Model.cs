using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT.Model
{
    //新网站半场全场实体
    public class New_BCQC_Model //: New_BZP_Model
    {
        public string gid { get; set; }
        public string datetime { get; set; }
        public string league { get; set; }
        public string gnum_h { get; set; }
        public string gnum_c { get; set; }
        public string team_h { get; set; }
        public string team_c { get; set; }
        public string strong { get; set; }
        public string ior_FHH { get; set; }
        public string ior_FHN { get; set; }
        public string ior_FHC { get; set; }
        public string ior_FNH { get; set; }
        public string ior_FNN { get; set; }
        public string ior_FNC { get; set; }
        public string ior_FCH { get; set; }
        public string ior_FCN { get; set; }
        public string ior_FCC { get; set; }
    }
}
