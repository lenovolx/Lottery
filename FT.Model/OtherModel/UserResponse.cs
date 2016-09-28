using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FT.Model.ViewModel;
using Newtonsoft.Json;

namespace FT.Model
{
    
    public class UserResponse : ResponseBase
    {
        /// <summary>
        /// 用户信息
        /// </summary>
        public UserViewModel User { get; set; }
        /// <summary>
        /// 系统配置信息
        /// </summary>
        public SystemSetting System { get; set; }
    }
    public class ManagerResponse : ResponseBase
    {
        public Admin Admin { get; set; }
    }
    [JsonObject(MemberSerialization.OptOut)]
    public class ResponseBase
    {
        public ResponseBase()
        {
            this.ExpiresIn = 30*60;
            this.ErrMsg = string.Empty;
            this.BeUsed = 0;
        }
        /// <summary>
        /// 是否自动登录(设置true,登录信息保存7天)
        /// </summary>
        public bool AutoLogin { get; set; }
        /// <summary>
        /// 错误代码
        /// </summary>
        public int ErrCode { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrMsg { get; set; }
        /// <summary>
        /// 超时时间【秒；不设置默认30分钟】
        /// </summary>
        public int ExpiresIn { get; set; }
        /// <summary>
        /// 登录返回ticket
        /// </summary>
        public string Ticket { get; set; }
        /// <summary>
        /// 登录成功返回Json字符串
        /// </summary>
        [JsonIgnore]
        public string DataJson { get; set; }
        /// <summary>
        /// 是否被使用
        /// </summary>
        public int? BeUsed { get; set; }
    }
}
