using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestSharp;
using Cache.Redis;
using Model.WeChat;

namespace WeChat.Core.Test
{
    [TestClass]
    public class MediaTest
    {
        private readonly WeChatServiceHandler _weChatServiceHandler = new WeChatServiceHandler(new RestClient());
        private readonly RedisHandler _redisHandler = new RedisHandler();

        [TestMethod]
        public void GetMediaTest()
        {
            string accessTokenStr = _redisHandler.GetAccessToken();
            if (string.IsNullOrEmpty(accessTokenStr))
            {
                accessTokenStr = _weChatServiceHandler.GetAccessToken().access_token;
                _redisHandler.SaveAccessToken(accessTokenStr);
            }

            dynamic result = _weChatServiceHandler.GetMedia(accessTokenStr, "2xM6y0SJHZMkLaN95_v23_eNz-HF3rj3lSDN8U8EnAkXOoEauqXHWMC7r54zATmI");

            if (result is MediaVideo)
            {
                MediaVideo media = result as MediaVideo;
                Console.WriteLine($"{media.video_url}");
            }
            else if(result is Error)
            {
                Error error = result as Error;
                Console.WriteLine($"{error.errcode}, {error.errmsg}");
            }
            else
            {
                Console.WriteLine($"{result}");
            }
        }

        [TestMethod]
        public void UploadMediaTest()
        {
            string accessTokenStr = _redisHandler.GetAccessToken();
            if (string.IsNullOrEmpty(accessTokenStr))
            {
                accessTokenStr = _weChatServiceHandler.GetAccessToken().access_token;
                _redisHandler.SaveAccessToken(accessTokenStr);
            }

            dynamic result = _weChatServiceHandler.UploadMedia(accessTokenStr, @"C:\Users\wdcda\Pictures\timg.jpg", MediaType.Image);

            if (result is MediaUpload)
            {
                MediaUpload mediaUpload = result as MediaUpload;
                Console.WriteLine($"{mediaUpload.media_id}, {mediaUpload.type}, {mediaUpload.created_at}");
            }
            else
            {
                Error error = result as Error;
                Console.WriteLine($"{error.errcode}, {error.errmsg}");
            }
        }
    }
}
