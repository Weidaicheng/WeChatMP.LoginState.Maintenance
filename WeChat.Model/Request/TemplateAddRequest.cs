namespace Model.Request
{
    public class TemplateAddRequest : RequestBase
    {
        public string Id { get; set; }
        public int[] KyewordIdList { get; set; }
    }
}
