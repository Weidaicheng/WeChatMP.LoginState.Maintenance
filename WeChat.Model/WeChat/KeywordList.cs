namespace Model.WeChat
{
	public class KeywordList : Error
	{
		public string id { get; set; }
		public string title { get; set; }
		public Keyword[] keyword_list { get; set; }
	}

	public class Keyword
	{
		public int keyword_id { get; set; }
		public string name { get; set; }
		public string example { get; set; }
	}
}
