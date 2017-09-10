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

        /// <summary>
        /// 保存OpenId到Redis服务器
        /// </summary>
        /// <param name="openId"></param>
        /// <param name="ticks"></param>
        public RedisOpenId SaveOpenId(WeChatOpenId openId, long ticks)
        {
            try
            {
                var db = Connection.GetDatabase();

                RedisOpenId model = new RedisOpenId()
                {
                    UserId = Guid.NewGuid(),
                    openid = openId.openid,
                    session_key = openId.session_key,
                    unionid = openId.unionid
                };

                db.ObjectSet(model.UserId.ToString(), model, TimeSpan.FromDays(ConfigurationHelper.ExpireDays.Value).Ticks);
                return model;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw ex;
            }
        }

        /// <summary>
        /// 通过ID获取OpenId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public RedisOpenId GetSavedOpenId(Guid id)
        {
            try
            {
                var db = Connection.GetDatabase();

                RedisOpenId savedOpenId = db.ObjectGet<RedisOpenId>(id.ToString());
                return savedOpenId;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw ex;
            }
        }
    }
}
