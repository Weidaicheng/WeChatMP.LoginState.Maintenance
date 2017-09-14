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
using Model.Request;
using Model.Response;
using RedisOpenId = Model.Redis.OpenId;
using WeChatOpenId = Model.WeChat.OpenId;
using Model.WeChat;
using Model.Request.Login;

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
        public ResponseResult<string> Login([FromBody]LoginRequest model)
        {
            try
            {
                if(model.Token == null)
                {
                    //直接调用微信接口登录
                    //检查Code
                    if (string.IsNullOrEmpty(model.Code))
                    {
                        return new ResponseResult<string>()
                        {
                            ErrCode = 1002,
                            ErrMsg = "Code为空",
                            Data = null
                        };
                    }

                    dynamic result = _weChatServiceHandler.GetOpenId(model.Code);
                    if(result is WeChatOpenId)
                    {
                        var openId = result as WeChatOpenId;
                        var redisOpenId = _redisHandler.SaveOpenId(openId);
                        return new ResponseResult<string>()
                        {
                            ErrCode = 0,
                            ErrMsg = "Success",
                            Data = redisOpenId.Token
                        };
                    }
                    else
                    {
                        var error = result as Error;
                        logger.Error(error.errmsg);
                        return new ResponseResult<string>()
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
                    var redisOpenId = _redisHandler.GetSavedOpenId(model.Token);
                    if(redisOpenId == null)
                    {
                        //OpenId已过期，重新调用微信接口登录
                        //检查Code
                        if (string.IsNullOrEmpty(model.Code))
                        {
                            return new ResponseResult<string>()
                            {
                                ErrCode = 1002,
                                ErrMsg = "Code为空",
                                Data = null
                            };
                        }

                        dynamic result = _weChatServiceHandler.GetOpenId(model.Code);
                        if (result is WeChatOpenId)
                        {
                            var openId = result as WeChatOpenId;
                            var redisOpenIdSaved = _redisHandler.SaveOpenId(openId);
                            return new ResponseResult<string>()
                            {
                                ErrCode = 0,
                                ErrMsg = "Success",
                                Data = redisOpenIdSaved.Token
                            };
                        }
                        else
                        {
                            var error = result as Error;
                            logger.Error(error.errmsg);
                            return new ResponseResult<string>()
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
                        return new ResponseResult<string>()
                        {
                            ErrCode = 0,
                            ErrMsg = "Success",
                            Data = redisOpenId.Token
                        };
                    }
                }
            }
            catch(Exception ex)
            {
                logger.Error(ex);
                return new ResponseResult<string>()
                {
                    ErrCode = 1003,
                    ErrMsg = ex.Message,
                    Data = null
                };
            }
        }

        /// <summary>
        /// 通过Token换取OpenId
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ResponseResult<string> GetOpenId(RequestBase model)
        {
            try
            {
                if(model.Token == null)
                {
                    throw new ArgumentNullException("UserId为空");
                }

                var openId = _redisHandler.GetSavedOpenId(model.Token);
                if(openId == null)
                {
                    return new ResponseResult<string>()
                    {
                        ErrCode = 1005,
                        ErrMsg = "UserId不存在，请重新登录",
                        Data = null
                    };
                }

                return new ResponseResult<string>()
                {
                    ErrCode = 0,
                    ErrMsg = "success",
                    Data = openId.openid
                };
            }
            catch(Exception ex)
            {
                logger.Error(ex);
                return new ResponseResult<string>()
                {
                    ErrCode = 1003,
                    ErrMsg = ex.Message,
                    Data = null
                };
            }
        }

        /// <summary>
        /// 通过Token获取UnionId
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ResponseResult<string> GetUnionId(RequestBase model)
        {
            try
            {
                if (model.Token == null)
                {
                    throw new ArgumentNullException("UserId为空");
                }

                var openId = _redisHandler.GetSavedOpenId(model.Token);
                if (openId == null)
                {
                    return new ResponseResult<string>()
                    {
                        ErrCode = 1005,
                        ErrMsg = "UserId不存在，请重新登录",
                        Data = null
                    };
                }

                return new ResponseResult<string>()
                {
                    ErrCode = 0,
                    ErrMsg = "success",
                    Data = openId.unionid
                };
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return new ResponseResult<string>()
                {
                    ErrCode = 1003,
                    ErrMsg = ex.Message,
                    Data = null
                };
            }
        }
        #endregion
    }
}
