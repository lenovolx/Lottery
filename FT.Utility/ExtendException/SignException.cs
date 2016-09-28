using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT.Utility.ExtendException
{
    /// <summary>
    /// API 参数校验异常
    /// </summary>
    public class SignException:Exception
    {
        public SignException() { }
        /// <summary>
        /// YB.Mall 自定义异常错误处理类
        /// </summary>
        /// <param name="message">错误提示内容</param>
        public SignException(string message)
            : base(message)
        {

        }
        public SignException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
