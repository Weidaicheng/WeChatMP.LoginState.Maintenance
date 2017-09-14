using Configuration.Helper;
using NLog;
using StackExchange.Redis;
using System;
using RedisOpenId = Model.Redis.OpenId;
using WeChatOpenId = Model.WeChat.OpenId;

namespace Cache.Redis
{
    public class RedisHandler
    {
        #region Connection
        private static ConnectionMultiplexer _connection;
        private static ConnectionMultiplexer Connection
        {
            get
            {
                if (_connection == null || !_connection.IsConnected)
                {
                    _connection = ConnectionMultiplexer.Connect($"{ConfigurationHelper.RedisServerHost}:{ConfigurationHelper.RedisServerPort}{(string.IsNullOrEmpty(ConfigurationHelper.RedisPassword) ? string.Empty : string.Concat(",password=", ConfigurationHelper.RedisPassword))}");
                }

                return _connection;
            }
        }
        #endregion

        #region log
        private readonly Logger logger = LogManager.GetCurrentClassLogger();
		#endregion

		#region OpenId
		/// <summary>
		/// 保存OpenId到Redis服务器
		/// </summary>
		/// <param name="openId"></param>
		public RedisOpenId SaveOpenId(WeChatOpenId openId)
        {
            try
            {
                var db = Connection.GetDatabase();

                RedisOpenId model = new RedisOpenId()
                {
                    Token = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
                    openid = openId.openid,
                    session_key = openId.session_key,
                    unionid = openId.unionid
                };

                db.ObjectSet(model.Token, model, TimeSpan.FromDays(ConfigurationHelper.ExpireDays.Value).Ticks);
                return model;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw ex;
            }
        }

        /// <summary>
        /// 通过Token获取OpenId
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public RedisOpenId GetSavedOpenId(string token)
        {
            try
            {
                var db = Connection.GetDatabase();

                RedisOpenId savedOpenId = db.ObjectGet<RedisOpenId>(token);
                return savedOpenId;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw ex;
            }
        }
		#endregion

		#region Access Token
		/// <summary>
		/// 保存Access Token
		/// </summary>
		/// <param name="accessToken"></param>
		public void SaveAccessToken(string accessToken)
		{
			try
			{
				var db = Connection.GetDatabase();

				db.StringSet("AccessToken", accessToken, TimeSpan.FromSeconds(ConfigurationHelper.AccessTokenExpireSeconds.Value));
			}
			catch (Exception ex)
			{
				logger.Error(ex);
				throw ex;
			}
		}

		/// <summary>
		/// 获取Access Token
		/// </summary>
		/// <returns></returns>
		public string GetAccessToken()
		{
			try
			{
				var db = Connection.GetDatabase();

				string accessToken = db.StringGet("AccessToken");
				return accessToken;
			}
			catch (Exception ex)
			{
				logger.Error(ex);
				throw ex;
			}
		}
		#endregion
	}
}
