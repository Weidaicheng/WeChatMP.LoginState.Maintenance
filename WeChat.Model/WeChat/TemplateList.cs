namespace Model.WeChat
{
	public class TemplateList : Error
	{
		public Template[] list { get; set; }
	}

	public class Template
	{
		public string template_id { get; set; }
		public string title { get; set; }
		public string content { get; set; }
		public string example { get; set; }
	}
}
