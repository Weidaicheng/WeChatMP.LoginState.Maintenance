using System;

namespace Model.Redis
{
	public class OpenId : WeChat.OpenId
	{
		public Guid UserId { get; set; }
	}
}
