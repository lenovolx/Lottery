namespace FT.Model.QueryModel
{
    using System;
    /// <summary>
    /// 查询条件基类
    /// </summary>
    public class BaseQueryModel
    {
        public BaseQueryModel()
        {
            this.Language = "cn";
            this.AgentId = 0;
            this.AgentLevel = 0;
            this.Page = 1;
            this.PageSize = 50;
            this.TimeZone = 8;
        }
        /// <summary>
        /// 页码
        /// </summary>
        public int? Page { get; set; }
        /// <summary>
        /// 分页数
        /// </summary>
        public int? PageSize { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartDate { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndDate { get; set; }
        /// <summary>
        /// 排序字段
        /// </summary>
        public string SortField { get; set; }
        /// <summary>
        /// 是否倒序（true,倒序;false,升序）
        /// </summary>
        public bool IsDesc { get; set; }

        #region PlatAdmin 查询可用
        public int MenuId { get; set; }
        public int RoleId { get; set; }
        /// <summary>
        /// 代理商编号
        /// </summary>
        public long? AgentId { get; set; }
        /// <summary>
        /// 代理商级别(只针对会员类型为代理商)
        /// </summary>
        public int? AgentLevel { get; set; }
        #endregion

        /// <summary>
        /// 语言
        /// </summary>
        public string Language { get; set; }
        public double? TimeZone { get; set; }
    }
}
