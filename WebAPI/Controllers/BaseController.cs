using Cache.Redis;
using NLog;
using RestSharp;
using System.Web.Http;
using WeChat.Core;

namespace WebAPI.Controllers
{
    public class BaseController : ApiController
    {
        #region field
        private readonly WeChatServiceHandler _weChatServiceHandler;
        private readonly RedisHandler _redisHandler;
        #endregion

        #region Log
        private static Logger logger = LogManager.GetCurrentClassLogger();
        #endregion

        #region .ctor
        public BaseController()
        {
            _weChatServiceHandler = new WeChatServiceHandler(new RestClient());
            _redisHandler = new RedisHandler();
        }
        #endregion

        #region 通用属性
        /// <summary>
        /// Access Token
        /// </summary>
        public string AccessToken
        {
            get
            {
                string accessTokenStr = _redisHandler.GetAccessToken();
                if (string.IsNullOrEmpty(accessTokenStr))
                {
                    accessTokenStr = _weChatServiceHandler.GetAccessToken().access_token;
                    _redisHandler.SaveAccessToken(accessTokenStr);
                }

                return accessTokenStr;
            }
        }
        #endregion
    }
}
