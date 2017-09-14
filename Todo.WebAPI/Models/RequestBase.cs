using System;

namespace Todo.WebAPI.Models
{
    /// <summary>
    /// 请求数据类基类
    /// </summary>
    public class RequestBase
    {
        /// <summary>
        /// Token
        /// </summary>
        public string Token { get; set; }
    }
}
