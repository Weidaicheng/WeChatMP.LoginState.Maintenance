using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeChat.Model
{
    /// <summary>
    /// OpenId成功结果
    /// </summary>
    public class OpenIdResultSuccess
    {
        public string openid { get; set; }
        public string session_key { get; set; }
        public string unionid { get; set; }
    }


    /// <summary>
    /// OpenId失败结果
    /// </summary>
    public class OpenIdResultFail
    {
        public int errcode { get; set; }
        public string errmsg { get; set; }
    }
}
