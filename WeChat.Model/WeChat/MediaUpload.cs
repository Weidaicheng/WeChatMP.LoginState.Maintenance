namespace Model.WeChat
{
    public class MediaUpload
    {
        public string type { get; set; }
        public string media_id { get; set; }
        public int created_at { get; set; }
    }

    public enum MediaType
    {
        Image
    }
}
