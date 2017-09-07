using System;

namespace Model
{
    /// <summary>
    /// 请求数据类基类
    /// </summary>
    public abstract class RequestBase
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public Guid? UserId { get; set; }
    }
}
