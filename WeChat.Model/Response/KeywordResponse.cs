using Model.WeChat;

namespace Model.Response
{

	public class KeywordResponse
	{
		public string id { get; set; }
		public string title { get; set; }
		public Keyword[] keyword_list { get; set; }
	}
}
