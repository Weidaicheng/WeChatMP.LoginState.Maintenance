using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Model
{
    /// <summary>
    /// 登录请求数据类
    /// </summary>
    public class LoginRequest : RequestBase
    {
        public string Code { get; set; }
    }
}