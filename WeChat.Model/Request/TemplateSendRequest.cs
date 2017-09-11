namespace Model.Request
{

	public class TemplateSendRequest
	{
		public string ToUser { get; set; }
		public string TemplateId { get; set; }
		public string Page { get; set; }
		public string FormId { get; set; }
		public Data Data { get; set; }
		public string EmphasisKeyword { get; set; }
	}

	public class Data
	{
		public KeywordValue keyword1 { get; set; }
		public KeywordValue keyword2 { get; set; }
		public KeywordValue keyword3 { get; set; }
		public KeywordValue keyword4 { get; set; }
		public KeywordValue keyword5 { get; set; }
		public KeywordValue keyword6 { get; set; }
		public KeywordValue keyword7 { get; set; }
		public KeywordValue keyword8 { get; set; }
		public KeywordValue keyword9 { get; set; }
		public KeywordValue keyword10 { get; set; }
	}

	public class KeywordValue
	{
		public string value { get; set; }
		public string color { get; set; }
	}
}
