using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeChat.Model;

namespace Redis.Model
{
    public class OpenIdResult : OpenIdResultSuccess
    {
        public Guid Id { get; set; }
    }
}
