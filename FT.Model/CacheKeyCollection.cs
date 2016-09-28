using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT.Model
{
    public class CacheKeyCollection
    {
        /// <summary>
        /// 数据采集登录用户缓存键
        /// </summary>
        public static string LoginJobUid = "LoginJobUid";

        /// <summary>
        /// 系统角色缓存键
        /// </summary>
        public static string SysRoleCache = "SysRoleCache";

        /// <summary>
        /// 系统菜单缓存键
        /// </summary>
        public static string MneuCache = "MneuCache";

        /// <summary>
        /// 操作列权限
        /// </summary>
        public static string RoleCell = "{0}_Role_Cell";
        /// <summary>
        /// 菜单权限
        /// </summary>
        public static string RoleMenu = "{0}_Role_Menu";
        /// <summary>
        /// 按钮权限
        /// </summary>
        public static string RoleButton = "{0}_Role_Button";
        /// <summary>
        /// 系统设置
        /// </summary>
        public static string SystemSetting = "SystemSetting";
        /// <summary>
        /// 当前滚球数据(比对赔率变化用)
        /// </summary>
        public static string RollBallCurrent = "RollBallCurrent";
        /// <summary>
        /// 滚球数据日志
        /// </summary>
        public static string RollBallMatchLog = "RollBallMatchLog";
        /// <summary>
        /// 系统已设定时区集合
        /// </summary>
        public static string SystemTimeZone = "SystemTimeZone";
    }
}
