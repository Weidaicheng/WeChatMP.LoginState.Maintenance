namespace Model.WeChat
{
    /// <summary>
    /// 图片消息
    /// </summary>
    public class ImageMessage : MessageBase
    {
        public string PicUrl { get; set; }
        public string MediaId { get; set; }
        public int MsgId { get; set; }
    }
}
