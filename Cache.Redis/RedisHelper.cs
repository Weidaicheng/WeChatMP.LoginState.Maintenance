using Configuration.Helper;
using NLog;
using Redis.Model;
using ServiceStack.Redis;
using System;
using WeChat.Model;

namespace Cache.Redis
{
    public class RedisHelper
    {
        #region log
        private readonly Logger logger = LogManager.GetCurrentClassLogger();
        #endregion

        /// <summary>
        /// 保存OpenId到Redis服务器
        /// </summary>
        /// <param name="oirs"></param>
        /// <param name="ticks"></param>
        public OpenIdResult SaveOpenId(OpenIdResultSuccess oirs, long ticks)
        {
            try
            {
                using (var redisManager = new PooledRedisClientManager($"{(string.IsNullOrEmpty(ConfigurationHelper.RedisPassword) ? string.Empty :  $"{ConfigurationHelper.RedisPassword}@") + ConfigurationHelper.RedisServerHost}:{ConfigurationHelper.RedisServerPort}"))
                {
                    using (var client = redisManager.GetClient())
                    {
                        var redisOIR = client.As<OpenIdResult>();

                        OpenIdResult model = new OpenIdResult()
                        {
                            Id = Guid.NewGuid(),
                            openid = oirs.openid,
                            session_key = oirs.session_key,
                            unionid = oirs.unionid
                        };

                        redisOIR.Store(model, new TimeSpan(ticks));
                        return model;
                    }
                }
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
        public OpenIdResult GetSavedOpenId(Guid id)
        {
            try
            {
                using (var redisManager = new PooledRedisClientManager($"{(string.IsNullOrEmpty(ConfigurationHelper.RedisPassword) ? string.Empty : $"{ConfigurationHelper.RedisPassword}@") + ConfigurationHelper.RedisServerHost}:{ConfigurationHelper.RedisServerPort}"))
                {
                    using(var client = redisManager.GetClient())
                    {
                        var redisOIR = client.As<OpenIdResult>();

                        OpenIdResult savedOpenId = redisOIR.GetById(id);
                        return savedOpenId;
                    }
                }
            }
            catch(Exception ex)
            {
                logger.Error(ex);
                throw ex;
            }
        }
    }
}
