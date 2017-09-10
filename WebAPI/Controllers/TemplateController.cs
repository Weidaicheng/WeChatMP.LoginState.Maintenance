using Model;
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
		public ResponseResult<List<TemplateTitle>> GetTemplateTitleList([FromBody]TemplateListRequest model)
		{
			try
			{
				AccessToken accessToken = _weChatServiceHandler.GetAccessToken();

				TemplateTitleList templateList = _weChatServiceHandler.GetTemplateTitleList(accessToken.access_token, model.Offset, model.Count);
				if (templateList.errcode == 0)
				{
					return new ResponseResult<List<TemplateTitle>>()
					{
						ErrCode = 0,
						ErrMsg = "success",
						Data = templateList.list.ToList()
					}; 
				}
				else
				{
					logger.Error(templateList.errmsg);
					return new ResponseResult<List<TemplateTitle>>()
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
				return new ResponseResult<List<TemplateTitle>>()
				{
					ErrCode = 1003,
					ErrMsg = ex.Message,
					Data = null
				};
			}
		}
	}
}
