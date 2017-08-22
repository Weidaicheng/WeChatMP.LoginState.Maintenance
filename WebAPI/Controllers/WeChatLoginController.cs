using Cache.Redis;
using Configuration.Helper;
using NLog;
using Redis.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebAPI.Models;
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

        #region log
        private readonly Logger logger = LogManager.GetCurrentClassLogger();
        #endregion

        #region API
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpPost]
        public ResponseResult<Guid?> Login([FromBody]LoginRequest model)
        {
            try
            {
                if(string.IsNullOrEmpty(model.Code))
                {
                    throw new ArgumentException("Code为空");
                }

                if(model.Id == null)
                {
                    //直接调用微信接口登录
                    dynamic result = weChatHelper.GetOpenId(model.Code);
                    if(result is OpenIdResultSuccess)
                    {
                        var oirs = result as OpenIdResultSuccess;
                        OpenIdResult openIdResultSaved = redisHelper.SaveOpenId(oirs, new TimeSpan(ConfigurationHelper.ExpireDays.Value, 0, 0, 0).Ticks);
                        return new ResponseResult<Guid?>()
                        {
                            ErrCode = 0,
                            ErrMsg = "Success",
                            Data = openIdResultSaved.Id
                        };
                    }
                    else
                    {
                        var oirf = result as OpenIdResultFail;
                        return new ResponseResult<Guid?>()
                        {
                            ErrCode = 1001,
                            ErrMsg = "获取OpenId失败",
                            Data = null
                        };
                    }
                }
                else
                {
                    //检查缓存中OpenId是否过期
                    var openId = redisHelper.GetSavedOpenId(model.Id.Value);
                    if(openId == null)
                    {
                        //OpenId已过期，重新调用微信接口登录
                        dynamic result = weChatHelper.GetOpenId(model.Code);
                        if (result is OpenIdResultSuccess)
                        {
                            var oirs = result as OpenIdResultSuccess;
                            OpenIdResult openIdResultSaved = redisHelper.SaveOpenId(oirs, new TimeSpan(ConfigurationHelper.ExpireDays.Value, 0, 0, 0).Ticks);
                            return new ResponseResult<Guid?>()
                            {
                                ErrCode = 0,
                                ErrMsg = "Success",
                                Data = openIdResultSaved.Id
                            };
                        }
                        else
                        {
                            var oirf = result as OpenIdResultFail;
                            return new ResponseResult<Guid?>()
                            {
                                ErrCode = 1001,
                                ErrMsg = "获取OpenId失败",
                                Data = null
                            };
                        }
                    }
                    else
                    {
                        //返回OpenId
                        return new ResponseResult<Guid?>()
                        {
                            ErrCode = 0,
                            ErrMsg = "Success",
                            Data = openId.Id
                        };
                    }
                }
            }
            catch(Exception ex)
            {
                logger.Error(ex);
                return new ResponseResult<Guid?>()
                {
                    ErrCode = 1002,
                    ErrMsg = ex.Message,
                    Data = null
                };
            }
        }
        #endregion
    }
}
