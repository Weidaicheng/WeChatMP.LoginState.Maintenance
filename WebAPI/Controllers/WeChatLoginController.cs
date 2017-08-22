using Cache.Redis;
using Configuration.Helper;
using Redis.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WeChat.Core;
using WeChat.Model;

namespace WebAPI.Controllers
{
    public class WeChatLoginController : ApiController
    {
        #region field
        private readonly WeChatHelper weChatHelper = new WeChatHelper();
        private readonly RedisHelper redisHelper = new RedisHelper();
        #endregion

        #region API
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public Guid? Login(string code, Guid? id)
        {
            try
            {
                if(string.IsNullOrEmpty(code))
                {
                    throw new ArgumentException("Code为空");
                }

                if(id == null)
                {
                    //直接调用微信接口登录
                    dynamic result = weChatHelper.GetOpenId(code);
                    if(result is OpenIdResultSuccess)
                    {
                        var oirs = result as OpenIdResultSuccess;
                        OpenIdResult openIdResultSaved = redisHelper.SaveOpenId(oirs, new TimeSpan(ConfigurationHelper.ExpireDays.Value, 0, 0, 0).Ticks);
                        return openIdResultSaved.Id;
                    }
                    else
                    {
                        var oirf = result as OpenIdResultFail;
                        return null;
                    }
                }
                else
                {
                    //检查缓存中OpenId是否过期
                    var openId = redisHelper.GetSavedOpenId(id.Value);
                    if(openId == null)
                    {
                        //OpenId已过期，重新调用微信接口登录
                        dynamic result = weChatHelper.GetOpenId(code);
                        if (result is OpenIdResultSuccess)
                        {
                            var oirs = result as OpenIdResultSuccess;
                            OpenIdResult openIdResultSaved = redisHelper.SaveOpenId(oirs, new TimeSpan(ConfigurationHelper.ExpireDays.Value, 0, 0, 0).Ticks);
                            return openIdResultSaved.Id;
                        }
                        else
                        {
                            var oirf = result as OpenIdResultFail;
                            return null;
                        }
                    }
                    else
                    {
                        //返回OpenId
                        return openId.Id;
                    }
                }
            }
            catch(Exception ex)
            {
                return null;
            }
        }
        #endregion
    }
}
