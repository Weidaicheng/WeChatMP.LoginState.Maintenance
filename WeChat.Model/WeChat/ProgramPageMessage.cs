namespace Model.WeChat
{
    /// <summary>
    /// 小程序卡牌消息
    /// </summary>
    public class ProgramPageMessage : MessageBase
    {
        public int MsgId { get; set; }
        public string Title { get; set; }
        public string AppId { get; set; }
        public string PagePath { get; set; }
        public string ThumbUrl { get; set; }
        public string ThumbMediaId { get; set; }
    }
}
