namespace Model.WeChat
{
    /// <summary>
    /// 文本消息
    /// </summary>
    public class TextMessage : MessageBase
    {
        public string Content { get; set; }
        public int MsgId { get; set; }
    }
}
