using Configuration.Helper;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeChat.Model;

namespace WeChat.Core
{
    public class WeChatHelper
    {
        #region field
        private readonly IRestClient client;
        #endregion

        #region .ctor
        public WeChatHelper()
        {
            client = new RestClient(ConfigurationHelper.WeChatApiAddr);
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

                IRestResponse response = client.Execute(request);
                if (response.Content.Contains("openid"))
                {
                    OpenIdResultSuccess result = JsonConvert.DeserializeObject<OpenIdResultSuccess>(response.Content);
                    return result;
                }
                else
                {
                    OpenIdResultFail result = JsonConvert.DeserializeObject<OpenIdResultFail>(response.Content);
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
