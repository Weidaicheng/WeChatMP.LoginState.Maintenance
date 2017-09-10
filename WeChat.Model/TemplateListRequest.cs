namespace Model
{
	public class TemplateListRequest : RequestBase
	{
		public int Offset { get; set; }
		public int Count { get; set; }
	}
}
