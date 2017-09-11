using Cache.Redis;
using Model;
using Model.Request;
using Model.Response;
using Model.WeChat;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WeChat.Core;

namespace WebAPI.Controllers
{
    public class TemplateController : ApiController
    {
		#region field
		private readonly WeChatServiceHandler _weChatServiceHandler;
		private readonly RedisHandler _redisHandler;
		#endregion

		#region Log
		private static Logger logger = LogManager.GetCurrentClassLogger();
		#endregion

		#region .ctor
		public TemplateController(WeChatServiceHandler weChatServiceHandler, RedisHandler redisHandler)
		{
			_weChatServiceHandler = weChatServiceHandler;
			_redisHandler = redisHandler;
		}
		#endregion

		/// <summary>
		/// 获取小程序模板库标题列表
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		public ResponseResult<TemplateTitleResponse> GetTemplateTitleList([FromBody]TemplateTitleListRequest model)
		{
			try
			{
				string accessTokenStr = _redisHandler.GetAccessToken();
				if(string.IsNullOrEmpty(accessTokenStr))
				{
					accessTokenStr = _weChatServiceHandler.GetAccessToken().access_token;
					_redisHandler.SaveAccessToken(accessTokenStr);
				}

				TemplateTitleList templateTitleList = _weChatServiceHandler.GetTemplateTitleList(accessTokenStr, model.Offset, model.Count);
				if (templateTitleList.errcode == 0)
				{
					return new ResponseResult<TemplateTitleResponse>()
					{
						ErrCode = 0,
						ErrMsg = "success",
						Data = new TemplateTitleResponse()
						{
							list = templateTitleList.list,
							total_count = templateTitleList.total_count
						}
					}; 
				}
				else
				{
					logger.Error(templateTitleList.errmsg);
					return new ResponseResult<TemplateTitleResponse>()
					{
						ErrCode = 1001,
						ErrMsg = templateTitleList.errmsg,
						Data = null
					};
				}
			}
			catch (Exception ex)
			{
				logger.Error(ex);
				return new ResponseResult<TemplateTitleResponse>()
				{
					ErrCode = 1003,
					ErrMsg = ex.Message,
					Data = null
				};
			}
		}

		/// <summary>
		/// 获取模板库某个模板标题下关键词库
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		public ResponseResult<KeywordResponse> GetKeywordList([FromBody]KeywordListRequest model)
		{
			try
			{
				string accessTokenStr = _redisHandler.GetAccessToken();
				if (string.IsNullOrEmpty(accessTokenStr))
				{
					accessTokenStr = _weChatServiceHandler.GetAccessToken().access_token;
					_redisHandler.SaveAccessToken(accessTokenStr);
				}

				KeywordList keywordList = _weChatServiceHandler.GetKeywordList(accessTokenStr, model.Id);
				if (keywordList.errcode == 0)
				{
					return new ResponseResult<KeywordResponse>()
					{
						ErrCode = 0,
						ErrMsg = "success",
						Data = new KeywordResponse()
						{
							id = keywordList.id,
							title = keywordList.title,
							keyword_list = keywordList.keyword_list
						}
					};
				}
				else
				{
					logger.Error(keywordList.errmsg);
					return new ResponseResult<KeywordResponse>()
					{
						ErrCode = 1001,
						ErrMsg = keywordList.errmsg,
						Data = null
					};
				}
			}
			catch (Exception ex)
			{
				logger.Error(ex);
				return new ResponseResult<KeywordResponse>()
				{
					ErrCode = 1003,
					ErrMsg = ex.Message,
					Data = null
				};
			}
		}

        /// <summary>
        /// 组合模板并添加至帐号下的个人模板库
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ResponseResult<string> AddTemplate([FromBody]TemplateAddRequest model)
        {
            try
            {
                string accessTokenStr = _redisHandler.GetAccessToken();
                if (string.IsNullOrEmpty(accessTokenStr))
                {
                    accessTokenStr = _weChatServiceHandler.GetAccessToken().access_token;
                    _redisHandler.SaveAccessToken(accessTokenStr);
                }

                TemplateAdd templateAdd = _weChatServiceHandler.AddTemplate(accessTokenStr, model.Id, model.KyewordIdList);
                if (templateAdd.errcode == 0)
                {
                    return new ResponseResult<string>()
                    {
                        ErrCode = 0,
                        ErrMsg = "success",
                        Data = templateAdd.template_id
                    };
                }
                else
                {
                    logger.Error(templateAdd.errmsg);
                    return new ResponseResult<string>()
                    {
                        ErrCode = 1001,
                        ErrMsg = templateAdd.errmsg,
                        Data = null
                    };
                }
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

		/// <summary>
		/// 获取帐号下已存在的模板列表
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		public ResponseResult<TemplateResponse> GetTemplateList([FromBody]TemplateListRequest model)
		{
			try
			{
				string accessTokenStr = _redisHandler.GetAccessToken();
				if (string.IsNullOrEmpty(accessTokenStr))
				{
					accessTokenStr = _weChatServiceHandler.GetAccessToken().access_token;
					_redisHandler.SaveAccessToken(accessTokenStr);
				}

				TemplateList templateList = _weChatServiceHandler.GetTemplateList(accessTokenStr, model.Offset, model.Count);
				if (templateList.errcode == 0)
				{
					return new ResponseResult<TemplateResponse>()
					{
						ErrCode = 0,
						ErrMsg = "success",
						Data = new TemplateResponse()
						{
							list = templateList.list
						}
					};
				}
				else
				{
					logger.Error(templateList.errmsg);
					return new ResponseResult<TemplateResponse>()
					{
						ErrCode = 1001,
						ErrMsg = templateList.errmsg,
						Data = null
					};
				}
			}
			catch (Exception ex)
			{
				logger.Error(ex);
				return new ResponseResult<TemplateResponse>()
				{
					ErrCode = 1003,
					ErrMsg = ex.Message,
					Data = null
				};
			}
		}

		/// <summary>
		/// 删除账号下某个模板
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		public ResponseResult<object> DeleteTemplate([FromBody]TemplateDeleteRequest model)
		{
			try
			{
				string accessTokenStr = _redisHandler.GetAccessToken();
				if (string.IsNullOrEmpty(accessTokenStr))
				{
					accessTokenStr = _weChatServiceHandler.GetAccessToken().access_token;
					_redisHandler.SaveAccessToken(accessTokenStr);
				}

				Error error = _weChatServiceHandler.DeleteTemplate(accessTokenStr, model.TemplateId);
				if (error.errcode == 0)
				{
					return new ResponseResult<object>()
					{
						ErrCode = 0,
						ErrMsg = "success",
						Data = null
					};
				}
				else
				{
					logger.Error(error.errmsg);
					return new ResponseResult<object>()
					{
						ErrCode = 1001,
						ErrMsg = error.errmsg,
						Data = null
					};
				}
			}
			catch (Exception ex)
			{
				logger.Error(ex);
				return new ResponseResult<object>()
				{
					ErrCode = 1003,
					ErrMsg = ex.Message,
					Data = null
				};
			}
		}
	}
}
