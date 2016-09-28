using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT.Model
{
    public class AngularTable<T> where T : class
    {
        public AngularTable()
        {
            this.total = 0;
            this.data = new List<T>();
        }
        public long? total { get; set; }
        public List<T> data { get; set; }
    }
    /// <summary>
    /// 底部带有统计
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TU"></typeparam>
    public class FootGrid<T, TU>
        where T : class
        where TU : class
    {
        public FootGrid()
        {
            this.Grid=new List<T>();
        }
        /// <summary>
        /// 列表数据
        /// </summary>
        public IEnumerable<T> Grid { get; set; }
        /// <summary>
        /// 底部统计数据
        /// </summary>
        public TU Foot { get; set; }
    }
}
