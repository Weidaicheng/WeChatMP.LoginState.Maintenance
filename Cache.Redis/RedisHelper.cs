using Configuration.Helper;
using Redis.Model;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeChat.Model;

namespace Cache.Redis
{
    public class RedisHelper
    {
        /// <summary>
        /// 保存OpenId到Redis服务器
        /// </summary>
        /// <param name="oirs"></param>
        /// <param name="ticks"></param>
        public Guid SaveOpenId(OpenIdResultSuccess oirs, long ticks)
        {
            try
            {
                using (var redisManager = new PooledRedisClientManager($"{ConfigurationHelper.RedisServerHost}:{ConfigurationHelper.RedisServerPort}"))
                {
                    using (var client = redisManager.GetClient())
                    {
                        var redisOIR = client.As<OpenIdResult>();

                        OpenIdResult model = oirs as OpenIdResult;
                        model.Id = Guid.NewGuid();

                        redisOIR.Store(model, new TimeSpan(ticks));
                        return model.Id;
                    }
                }
            }
            catch (Exception ex)
            {
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
                using (var redisManager = new PooledRedisClientManager($"{ConfigurationHelper.RedisServerHost}:{ConfigurationHelper.RedisServerPort}"))
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
                throw ex;
            }
        }
    }
}
