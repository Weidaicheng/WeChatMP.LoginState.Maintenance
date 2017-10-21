namespace Model.WeChat
{
    /// <summary>
    /// 进入会话事件消息
    /// </summary>
    public class EventMessage : MessageBase
    {
        public string Event { get; set; }
        public string SessionFrom { get; set; }
    }
}
