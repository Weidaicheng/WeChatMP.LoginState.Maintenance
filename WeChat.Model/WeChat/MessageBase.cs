namespace Model.WeChat
{
    /// <summary>
    /// 客服消息基类
    /// </summary>
    public class MessageBase
    {
        public string ToUserName { get; set; }
        public string FromUserName { get; set; }
        public int CreateTime { get; set; }
        public string MsgType { get; set; }
    }
}
