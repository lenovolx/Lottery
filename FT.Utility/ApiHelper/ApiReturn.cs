using System;
using System.Collections.Generic;
using System.Linq;
using FT.Utility.Helper;

namespace FT.Utility.ApiHelper
{
    /*
     * Description:统一返回类型(属性都小写是为了序列化后到js中都是小写的)
     */
    public class ApiReturn
    {
        public ApiReturn()
        {
            code = 1;
        }

        #region 属性集合

        /// <summary>
        /// 操作返回编号是否顺利完成 (0,成功;1,服务端捕捉异常;2,程序异常;3,登录失效;)
        /// </summary>
        public int code
        {
            get;
            set;
        }
        /// <summary>
        /// 错误
        /// </summary>
        public string errors
        {
            get;
            set;
        }

        /// <summary>
        /// 消息
        /// </summary>
        public string message
        {
            get
            {
                string msg;
                if (code != 0)
                    msg = this.errors == "" ? string.Format("操作超时,请重试") : string.Format("{0}", errors);
                else
                    msg = string.Format("操作成功");
                return msg;
            }
        }

        /// <summary>
        /// 返回数据(序列化数据)
        /// </summary>
        public dynamic data
        {
            get;
            set;
        }

        private int _total = 0;
        /// <summary>
        /// 总记录数,默认取tSoucre.Count()
        /// </summary>
        public int total
        {
            get
            {
                if (_total != 0) return _total;
                var source = data as IEnumerable<object>;
                if (source != null)
                {
                    _total = source.Count();
                }
                return _total;
            }
            set
            {
                _total = value;
            }
        }
        #endregion

        #region 方法集合
        /// <summary>
        /// Twi.Pages统一执行方法
        /// </summary>
        /// <param name="ret"></param>
        /// <param name="action"></param>
        public static void Execute(ref ApiReturn ret, Action action)
        {
            try
            {
                action();
                ret.code = 0;
            }
            catch (Exception ex)
            {
                ret.errors = ex.Message;
                Log.Error(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public static ApiReturn Execute(Func<ApiReturn> func)
        {
            var ret = new ApiReturn();
            try
            {
                ret = func();
                ret.code = 0;
            }
            catch (Exception ex)
            {
                ret.errors = ex.Message;
                Log.Error(ex);
            }
            return ret;
        }

        /// <summary>
        /// 统一执行方法,返回JsonReturn
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static ApiReturn Execute(Action action)
        {
            var ret = new ApiReturn();
            try
            {
                action();
                ret.code = 0;
            }
            catch (Exception ex)
            {
                ret.errors = ex.Message;
                Log.Error(ex);
            }
            return ret;
        }
        #endregion

    }
}
