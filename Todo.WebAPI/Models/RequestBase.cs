using System;

namespace Todo.WebAPI.Models
{
    /// <summary>
    /// 请求数据类基类
    /// </summary>
    public class RequestBase
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public Guid? UserId { get; set; }
    }
}
