using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT.Model
{
    /// <summary>
    /// EasyUI DataGrid 数据绑定模型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EasyDataGrid<T> where T : class
    {
        public EasyDataGrid()
        {
            this.total = 0;
            this.rows=new List<T>();
        }
        /// <summary>
        /// 总数
        /// </summary>
        public int total { get; set; }
        /// <summary>
        /// 数据集合
        /// </summary>
        public IEnumerable<T> rows { get; set; }
    }

    public class EasyDataGrid<T,TU> where T : class where TU:class 
    {
        public EasyDataGrid()
        {
            this.total = 0;
        }
        /// <summary>
        /// 总数
        /// </summary>
        public int total { get; set; }
        /// <summary>
        /// 数据集合
        /// </summary>
        public IEnumerable<T> rows { get; set; }
        /// <summary>
        /// 底部信息
        /// </summary>
        public TU foot { get; set; }
    }
}
