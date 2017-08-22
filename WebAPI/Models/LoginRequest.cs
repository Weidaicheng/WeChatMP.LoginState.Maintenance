using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models
{
    public class LoginRequest
    {
        public string Code { get; set; }
        public Guid? Id { get; set; }
    }
}