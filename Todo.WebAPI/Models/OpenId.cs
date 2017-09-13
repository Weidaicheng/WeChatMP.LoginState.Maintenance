using System;

namespace Todo.WebAPI.Models
{
	public class OpenId
	{
		public Guid UserId { get; set; }
        public string openid { get; set; }
        public string session_key { get; set; }
        public string unionid { get; set; }
    }
}
