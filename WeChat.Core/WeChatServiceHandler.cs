using Configuration.Helper;
using Newtonsoft.Json;
using NLog;
using RestSharp;
using System;
using Model.WeChat;
using Model.Request.Template;

namespace WeChat.Core
{
    /// <summary>
    /// 微信接口处理类
    /// </summary>
    public class WeChatServiceHandler
    {
        #region field
        private readonly IRestClient _client;
        #endregion

        #region log
        private readonly Logger logger = LogManager.GetCurrentClassLogger();
        #endregion

        #region .ctor
        public WeChatServiceHandler(IRestClient client)
        {
            _client = client;
			_client.BaseUrl = new Uri(ConfigurationHelper.WeChatApiAddr);
        }
        #endregion

        #region 微信方法
        /// <summary>
        /// 获取OpenId
        /// </summary>
        /// <param name="code"></param>
        /// <returns>成功时返回OpenIdResultSuccess，失败时返回OpenIdResultError</returns>
        public dynamic GetOpenId(string code)
        {
            try
            {
                IRestRequest request = new RestRequest("sns/jscode2session", Method.GET);
                request.AddParameter("appid", ConfigurationHelper.AppId);
                request.AddParameter("secret", ConfigurationHelper.AppSecret);
                request.AddParameter("js_code", code);
                request.AddParameter("grant_type", "authorization_code");

                IRestResponse response = _client.Execute(request);
                if (response.Content.Contains("openid"))
                {
                    OpenId result = JsonConvert.DeserializeObject<OpenId>(response.Content);
                    return result;
                }
                else
                {
                    Error result = JsonConvert.DeserializeObject<Error>(response.Content);
                    return result;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw ex;
            }
        }

		/// <summary>
		/// 获取AccessToken
		/// </summary>
		/// <returns></returns>
		public AccessToken GetAccessToken()
		{
			try
			{
				IRestRequest request = new RestRequest("cgi-bin/token", Method.GET);
				request.AddQueryParameter("grant_type", "client_credential");
				request.AddQueryParameter("appid", ConfigurationHelper.AppId);
				request.AddQueryParameter("secret", ConfigurationHelper.AppSecret);

				IRestResponse response = _client.Execute(request);
				string content = response.Content;
				if (content.Contains("access_token"))
				{
					return JsonConvert.DeserializeObject<AccessToken>(content);
				}

				throw new Exception("未能获取AccessToken");
			}
			catch (Exception ex)
			{
				logger.Error(ex);
				throw ex;
			}
		}

		#region 消息模板
		/// <summary>
		/// 获取小程序模板库标题列表
		/// </summary>
		/// <param name="accessToken"></param>
		/// <param name="offset"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public TemplateTitleList GetTemplateTitleList(string accessToken, int offset, int count)
		{
			try
			{
				if(string.IsNullOrEmpty(accessToken))
				{
					throw new ArgumentNullException("AccessToken为空");
				}
				if(offset < 0)
				{
					throw new ArgumentOutOfRangeException($"offset: {offset}值不合法");
				}
				if(count < 0 || count > 20)
				{
					throw new ArgumentOutOfRangeException($"count: {count}值不合法");
				}

				IRestRequest request = new RestRequest("cgi-bin/wxopen/template/library/list", Method.POST);
				request.AddQueryParameter("access_token", accessToken);
				request.AddJsonBody(new
				{
					offset = offset,
					count = count
				});

				IRestResponse response = _client.Execute(request);
				return JsonConvert.DeserializeObject<TemplateTitleList>(response.Content);
			}
			catch(Exception ex)
			{
				logger.Error(ex);
				throw ex;
			}
		}

		/// <summary>
		/// 获取模板库某个模板标题下关键词库
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public KeywordList GetKeywordList(string accessToken, string id)
		{
			try
			{
				if (string.IsNullOrEmpty(accessToken))
				{
					throw new ArgumentNullException("AccessToken为空");
				}
				if(string.IsNullOrEmpty(id))
				{
					throw new ArgumentNullException("id为空");
				}

				IRestRequest request = new RestRequest("cgi-bin/wxopen/template/library/get", Method.POST);
				request.AddQueryParameter("access_token", accessToken);
				request.AddJsonBody(new
				{
					id = id
				});

				IRestResponse response = _client.Execute(request);
				return JsonConvert.DeserializeObject<KeywordList>(response.Content);
			}
			catch (Exception ex)
			{
				logger.Error(ex);
				throw ex;
			}
		}

		/// <summary>
		/// 组合模板并添加至帐号下的个人模板库
		/// </summary>
		/// <param name="accessToken"></param>
		/// <param name="id"></param>
		/// <param name="keywordIds"></param>
		/// <returns></returns>
		public TemplateAdd AddTemplate(string accessToken, string id, int[] keywordIds)
		{
			try
			{
				if (string.IsNullOrEmpty(accessToken))
				{
					throw new ArgumentNullException("AccessToken为空");
				}
				if (string.IsNullOrEmpty(id))
				{
					throw new ArgumentNullException("id为空");
				}
				if(keywordIds == null || keywordIds.Length == 0)
				{
					throw new ArgumentException("Keyword id列表为空");
				}

				IRestRequest request = new RestRequest("cgi-bin/wxopen/template/add", Method.POST);
				request.AddQueryParameter("access_token", accessToken);
				request.AddJsonBody(new
				{
					id = id,
					keyword_id_list = keywordIds
				});

				IRestResponse response = _client.Execute(request);
				return JsonConvert.DeserializeObject<TemplateAdd>(response.Content);
			}
			catch (Exception ex)
			{
				logger.Error(ex);
				throw;
			}
		}

		/// <summary>
		/// 获取帐号下已存在的模板列表
		/// </summary>
		/// <param name="accessToken"></param>
		/// <param name="offset"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public TemplateList GetTemplateList(string accessToken, int offset, int count)
		{
			try
			{
				if (string.IsNullOrEmpty(accessToken))
				{
					throw new ArgumentNullException("AccessToken为空");
				}
				if (offset < 0)
				{
					throw new ArgumentOutOfRangeException($"offset: {offset}值不合法");
				}
				if (count < 0 || count > 20)
				{
					throw new ArgumentOutOfRangeException($"count: {count}值不合法");
				}

				IRestRequest request = new RestRequest("cgi-bin/wxopen/template/list", Method.POST);
				request.AddQueryParameter("access_token", accessToken);
				request.AddJsonBody(new
				{
					offset = offset,
					count = count
				});

				IRestResponse response = _client.Execute(request);
				return JsonConvert.DeserializeObject<TemplateList>(response.Content);
			}
			catch (Exception ex)
			{
				logger.Error(ex);
				throw;
			}
		}

		/// <summary>
		/// 删除账号下某个模板
		/// </summary>
		/// <param name="accessToken"></param>
		/// <param name="templateId"></param>
		/// <returns></returns>
		public Error DeleteTemplate(string accessToken, string templateId)
		{
			try
			{
				if (string.IsNullOrEmpty(accessToken))
				{
					throw new ArgumentNullException("AccessToken为空");
				}
				if(string.IsNullOrEmpty(templateId))
				{
					throw new ArgumentNullException("TemplateId为空");
				}

				IRestRequest request = new RestRequest("cgi-bin/wxopen/template/del", Method.POST);
				request.AddQueryParameter("access_token", accessToken);
				request.AddJsonBody(new
				{
					template_id = templateId
				});

				IRestResponse response = _client.Execute(request);
				return JsonConvert.DeserializeObject<Error>(response.Content);
			}
			catch (Exception ex)
			{
				logger.Error(ex);
				throw;
			}
		}

		/// <summary>
		/// 发送模板消息
		/// </summary>
		/// <param name="accessToken"></param>
		/// <param name="model"></param>
		/// <returns></returns>
		public Error SendTemplate(string accessToken, TemplateSendRequest model)
		{
			try
			{
				if (string.IsNullOrEmpty(accessToken))
				{
					throw new ArgumentNullException("AccessToken为空");
				}
				if(model == null)
				{
					throw new ArgumentNullException("消息参数为空");
				}
				if(string.IsNullOrEmpty(model.ToUser))
				{
					throw new ArgumentNullException("OpenId为空");
				}
				if (string.IsNullOrEmpty(model.TemplateId))
				{
					throw new ArgumentNullException("TemplateId为空");
				}
				if(string.IsNullOrEmpty(model.FormId))
				{
					throw new ArgumentNullException("FormId为空");
				}
				if(string.IsNullOrEmpty(model.FormId))
				{
					throw new ArgumentNullException("Data为空");
				}

				IRestRequest request = new RestRequest("cgi-bin/message/wxopen/template/send", Method.POST);
				request.AddQueryParameter("access_token", accessToken);
				request.AddJsonBody(new
				{
					touser = model.ToUser,
					template_id = model.TemplateId,
					page = model.Page,
					form_id = model.FormId,
					data = model.Data,
					emphasis_keyword = model.EmphasisKeyword
				});

				IRestResponse response = _client.Execute(request);
				return JsonConvert.DeserializeObject<Error>(response.Content);
			}
			catch (Exception ex)
			{
				logger.Error(ex);
				throw;
			}
		}
        #endregion

        #region 临时素材
        /// <summary>
        /// 获取临时素材
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="mediaId"></param>
        /// <returns></returns>
        public Media GetMedia(string accessToken, string mediaId)
        {
            try
            {
                if(string.IsNullOrEmpty(accessToken))
                {
                    throw new ArgumentNullException("AccessToken为空");
                }
                if(string.IsNullOrEmpty(mediaId))
                {
                    throw new ArgumentNullException("Media Id为空");
                }

                IRestRequest request = new RestRequest("cgi-bin/media/get", Method.POST);
                request.AddQueryParameter("access_token", accessToken);
                request.AddQueryParameter("media_id", mediaId);

                IRestResponse response = _client.Execute(request);
                if(response.Content.Contains("video_url"))
                {
                    return JsonConvert.DeserializeObject<Media>(response.Content);
                }

                Error error = JsonConvert.DeserializeObject<Error>(response.Content);
                throw new Exception(error.errmsg);
            }
            catch(Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }
        #endregion
        #endregion
    }
}
