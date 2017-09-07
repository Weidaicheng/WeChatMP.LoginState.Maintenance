using Cache.Redis;
using Configuration.Helper;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WeChat.Core;
using Model;

namespace WebAPI.Controllers
{
    public class WeChatLoginController : ApiController
    {
        #region field
        private readonly WeChatServiceHandler _weChatServiceHandler;
        private readonly RedisHandler _redisHandler;
        #endregion

        #region log
        private readonly Logger logger = LogManager.GetCurrentClassLogger();
        #endregion

        #region .ctor
        public WeChatLoginController(WeChatServiceHandler weChatServiceHandler, RedisHandler redisHandler)
        {
            _weChatServiceHandler = weChatServiceHandler;
            _redisHandler = redisHandler;
        }
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
                if(model.UserId == null)
                {
                    //直接调用微信接口登录
                    //检查Code
                    if (string.IsNullOrEmpty(model.Code))
                    {
                        return new ResponseResult<Guid?>()
                        {
                            ErrCode = 1002,
                            ErrMsg = "Code为空",
                            Data = null
                        };
                    }

                    dynamic result = _weChatServiceHandler.GetOpenId(model.Code);
                    if(result is OpenIdResultSuccess)
                    {
                        var oirs = result as OpenIdResultSuccess;
                        OpenIdResultModel openIdResultSaved = _redisHandler.SaveOpenId(oirs, new TimeSpan(ConfigurationHelper.ExpireDays.Value, 0, 0, 0).Ticks);
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
                        logger.Error(oirf.errmsg);
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
                    var openId = _redisHandler.GetSavedOpenId(model.UserId.Value);
                    if(openId == null)
                    {
                        //OpenId已过期，重新调用微信接口登录
                        //检查Code
                        if (string.IsNullOrEmpty(model.Code))
                        {
                            return new ResponseResult<Guid?>()
                            {
                                ErrCode = 1002,
                                ErrMsg = "Code为空",
                                Data = null
                            };
                        }

                        dynamic result = _weChatServiceHandler.GetOpenId(model.Code);
                        if (result is OpenIdResultSuccess)
                        {
                            var oirs = result as OpenIdResultSuccess;
                            OpenIdResultModel openIdResultSaved = _redisHandler.SaveOpenId(oirs, new TimeSpan(ConfigurationHelper.ExpireDays.Value, 0, 0, 0).Ticks);
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
                            logger.Error(oirf.errmsg);
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
                        //OpenId未过期
                        //返回用户Id
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
