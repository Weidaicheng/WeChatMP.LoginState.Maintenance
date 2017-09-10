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
		#endregion

		#region Log
		private static Logger logger = LogManager.GetCurrentClassLogger();
		#endregion

		#region .ctor
		public TemplateController(WeChatServiceHandler weChatServiceHandler)
		{
			_weChatServiceHandler = weChatServiceHandler;
		}
		#endregion

		/// <summary>
		/// 获取小程序模板库标题列表
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		public ResponseResult<TemplateTitleList> GetTemplateTitleList([FromBody]TemplateListRequest model)
		{
			try
			{
				AccessToken accessToken = _weChatServiceHandler.GetAccessToken();

				TemplateTitleList templateList = _weChatServiceHandler.GetTemplateTitleList(accessToken.access_token, model.Offset, model.Count);
				if (templateList.errcode == 0)
				{
					return new ResponseResult<TemplateTitleList>()
					{
						ErrCode = 0,
						ErrMsg = "success",
						Data = templateList
					}; 
				}
				else
				{
					logger.Error(templateList.errmsg);
					return new ResponseResult<TemplateTitleList>()
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
				return new ResponseResult<TemplateTitleList>()
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
		public ResponseResult<List<Keyword>> GetKeywordList([FromBody]KeywordListRequest model)
		{
			try
			{
				AccessToken accessToken = _weChatServiceHandler.GetAccessToken();

				KeywordList keywordList = _weChatServiceHandler.GetKeywordList(accessToken.access_token, model.Id);
				if (keywordList.errcode == 0)
				{
					return new ResponseResult<List<Keyword>>()
					{
						ErrCode = 0,
						ErrMsg = "success",
						Data = keywordList.keyword_list.ToList()
					};
				}
				else
				{
					logger.Error(keywordList.errmsg);
					return new ResponseResult<List<Keyword>>()
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
				return new ResponseResult<List<Keyword>>()
				{
					ErrCode = 1003,
					ErrMsg = ex.Message,
					Data = null
				};
			}
		}
	}
}
