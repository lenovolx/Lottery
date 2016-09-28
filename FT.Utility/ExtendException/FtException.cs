using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace FT.Utility.ExtendException
{
    /// <summary>
    /// FT 自定义异常错误处理类
    /// </summary>
    public class FtException : Exception
    {
        public FtException() { }
        /// <summary>
        /// YB.Mall 自定义异常错误处理类
        /// </summary>
        /// <param name="message">错误提示内容</param>
        public FtException(string message)
            : base(message)
        {

        }
        public FtException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
